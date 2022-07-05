/*********************************************************************************
/* Filename    : maintenance_comm.c
 * Description :
 * Author      : kwj
 * Notes       :
 *********************************************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/epoll.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>

#include <sys/types.h>
#include <sys/stat.h>
#include <termios.h>
#include <sys/un.h>

#include "systypes.h"
#include "ftms_cpu.h"
#include "serial.h"
#include "user_term.h"
//#include "sys_comm.h"
#include "vds_protocol.h"

#include "maintenance_protocol.h"

uint8 send_saved_rt_data_flg = FALSE;
uint8 send_saved_pl_data_flg = FALSE;
uint8 seq_num_of_saved_pl;
uint8 seq_num_of_saved_rt;

uint8 local_poll_lane[2];
uint8 local_poll_data_flg = 0;

uint8 center_poll_lane[2];
uint8 center_poll_data_flg = 0;

static uint8 indiv_data_lane;
static uint8 indiv_data_flg = 0;

extern MODEM_DATA recvDataPortA;

extern uint8 gucStuckPerLoop[MAX_LOOP_NUM/4];
extern uint8 incident_per_loop[MAX_LOOP_NUM/8];

uint8 recvFramePortA[MAX_MODEM_DATA_QUEUE_NO];


void tx_over_maintenance_port(uint8 * p_src, uint32 len)
{
	uint32 i, stuffed_no=0;
	uint8 tmpb[256+sizeof(RESPOND_PACKET)+100], * tx_ptr=p_src;
	

	for(i=0; i<2; i++) tmpb[i] = tx_ptr[i];

	// DLE Stuffing
	for (i=2; i<( len-sizeof(PACKET_TAIL) ); i++) 
	{
		if( tx_ptr[i] == DLE ) 
		{
			tmpb[i + stuffed_no] = DLE;
			stuffed_no++;
		}
		
		tmpb[i + stuffed_no] = tx_ptr[i];
	}

	for(i= len-sizeof(PACKET_TAIL); i<len; i++) tmpb[i + stuffed_no] = tx_ptr[i];

#if 0
	fprintf(stdout,"\n\r[SEND DATA]\n\r");
	for(i=0; i<(len+stuffed_no); i++) fprintf(stdout,"%02x |", tmpb[i]); 
	fprintf(stdout," \n\r ---------------------------------------------------\n\r");
#else
	usleep(3000);
#endif

	if(write(fd_ttys1, tmpb, len+stuffed_no) == -1)
	{
		fprintf(stdout,"\n-- MNTNC Transe error n");
	}
}

BOOL isCenterPoll_SendFlag(void)
{
	if (center_poll_data_flg) return TRUE;
	return FALSE;
}

BOOL isCenterPoll_AllLane(void)
{
	if (center_poll_lane[0] == _ALL_LANE_) return TRUE;
	return FALSE;
}

BOOL isLocalPoll_SendFlag(void)
{
	if (local_poll_data_flg) return TRUE;
	return FALSE;
}

BOOL isLocalPoll_AllLane(void)
{
	if (local_poll_lane[0] == _ALL_LANE_) return TRUE;
	return FALSE;
}

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)

void sendAllLocalPollDataPkt(void)
{
	int i;
	uint8 *tmptr;
	uint16 crc, t_lane;
	ALL_LOCAL_DATA_RSP_PACKET respPkt;
	
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_LOCAL_POLL_DATA_REQ;

	// 시간
	respPkt.time.year_hi = gstLocalPolling.ctime.year_hi;
	respPkt.time.year_lo = gstLocalPolling.ctime.year_lo;
	respPkt.time.month = gstLocalPolling.ctime.month;
	respPkt.time.day = gstLocalPolling.ctime.day;
	respPkt.time.s.hour = gstLocalPolling.ptime.hour;
	respPkt.time.s.min = gstLocalPolling.ptime.min;
	respPkt.time.s.sec = gstLocalPolling.ptime.sec;
	respPkt.time.e.hour = gstLocalPolling.ctime.hour;
	respPkt.time.e.min = gstLocalPolling.ctime.min;
	respPkt.time.e.sec = gstLocalPolling.ctime.sec;

	// 데이터.
	for (t_lane=0; t_lane<MAX_RIO_LANE_NUM; t_lane++)
	{
		respPkt.data[t_lane].lane = t_lane+1;
		respPkt.data[t_lane].vol = gstLocalPolling.volume[gstLaneLink[t_lane].e_loop];
		respPkt.data[t_lane].spd_hi = gstLocalPolling.speed[t_lane]/100;
		respPkt.data[t_lane].spd_lo = gstLocalPolling.speed[t_lane]%100;
		respPkt.data[t_lane].occupy = gstLocalPolling.occupy[gstLaneLink[t_lane].e_loop];
		respPkt.data[t_lane].len_hi = gstLocalPolling.length[t_lane]/100;
		respPkt.data[t_lane].len_lo = gstLocalPolling.length[t_lane]%100;
	}

	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout,"Recv : [All-Local-Poll]\r\n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout,"TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout,"%02x ", tmptr[i]);

		fprintf(stdout,"\r\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	

}

void sendLocalPollDataPacket(uint32 t_lane)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	LOCAL_DATA_RSP_PACKET respPkt;


#if 1
	if (!local_poll_data_flg) return;
	
	if (local_poll_lane[0] == _ALL_LANE_)
	{
		// .................
		return;		
	}
	
	if (t_lane >= MAX_LANE_NUM) return;
	if (t_lane != local_poll_lane[0] && t_lane != local_poll_lane[1]) return;
#else
	if (!local_poll_data_flg || t_lane != local_poll_lane[0]) return;
	if (t_lane >= MAX_LANE_NUM) return;
#endif
	
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_LOCAL_POLL_DATA_REQ;


	// 시간
	respPkt.time.year_hi = gstLocalPolling.ctime.year_hi;
	respPkt.time.year_lo = gstLocalPolling.ctime.year_lo;
	respPkt.time.month = gstLocalPolling.ctime.month;
	respPkt.time.day = gstLocalPolling.ctime.day;
	respPkt.time.s.hour = gstLocalPolling.ptime.hour;
	respPkt.time.s.min = gstLocalPolling.ptime.min;
	respPkt.time.s.sec = gstLocalPolling.ptime.sec;
	respPkt.time.e.hour = gstLocalPolling.ctime.hour;
	respPkt.time.e.min = gstLocalPolling.ctime.min;
	respPkt.time.e.sec = gstLocalPolling.ctime.sec;

	// 데이터.
	respPkt.data.lane = t_lane+1;
	respPkt.data.vol = gstLocalPolling.volume[gstLaneLink[t_lane].e_loop];
	respPkt.data.spd_hi = gstLocalPolling.speed[t_lane]/100;
	respPkt.data.spd_lo = gstLocalPolling.speed[t_lane]%100;
	respPkt.data.occupy = gstLocalPolling.occupy[gstLaneLink[t_lane].e_loop];
	respPkt.data.len_hi = gstLocalPolling.length[t_lane]/100;
	respPkt.data.len_lo = gstLocalPolling.length[t_lane]%100;

	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout,"Recv : [Local-Poll]\r\n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout,"TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout,"%02x ", tmptr[i]);

		fprintf(stdout,"\r\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	
}

void prepareLocalPollData(void)
{
	int i;
	u_int polling_count;
	uint32 t_occupy;

	polling_count = localPollCounter /** _TICK_MS_*/;	//by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
	
	//Reset Polling count
	localPollCounter = 0;

	invers_st.start_inv_flag = TRUE;  //by capi 2015.01.07

	// Reset counter for auto-sync.
	// Auto-Sync 
	localAutosyncCounter = 0;

	if (!polling_count) return;

	// for detail calculation occ
	for(i=0; i<MAX_LOOP_NUM; i++)
	{
		if (loop_st[i].status == PRESSED)
		{
			//add current occupy
			t_occupy = io_time_counter - loop_st[i].time[PRESS_TIME];
			
			if (t_occupy >= polling_count) gstLocalAccuData.occupy[i] = polling_count;  // 100 %
			else gstLocalAccuData.occupy[i] += t_occupy;
		}
	}

	//Saving  Local Polling time
	gstLocalPolling.ptime = gstLocalPolling.ctime;
	gstLocalPolling.ctime.year_hi = sys_time.wYear/100;
	gstLocalPolling.ctime.year_lo = sys_time.wYear%100;
	gstLocalPolling.ctime.month = sys_time.wMonth;
	gstLocalPolling.ctime.day = sys_time.wDay;
	gstLocalPolling.ctime.hour = sys_time.wHour;
	gstLocalPolling.ctime.min = sys_time.wMinute;
	gstLocalPolling.ctime.sec = sys_time.wSecond;
	

	for (i=0; i<MAX_LANE_NUM; i++)
	{
		if (gstLaneLink[i].e_loop < MAX_LOOP_NUM && gstLocalAccuData.volume[gstLaneLink[i].e_loop])
		{
			//Calculation average Value
			gstLocalPolling.speed[i] = gstLocalAccuData.speed[i] / gstLocalAccuData.volume[gstLaneLink[i].e_loop];
			gstLocalPolling.length[i] = gstLocalAccuData.length[i] /gstLocalAccuData.volume[gstLaneLink[i].e_loop];
		}
		else
		{
			// if volumn is zero 
			gstLocalPolling.speed[i] = 0;
			gstLocalPolling.length[i] = 0;
		}
	}
	
	for (i=0; i<MAX_LOOP_NUM; i++)
	{
		gstLocalPolling.volume[i] = gstLocalAccuData.volume[i];
		
		t_occupy = 10 * 100 * gstLocalAccuData.occupy[i] / polling_count;
		t_occupy = t_occupy / 5;
		t_occupy = (t_occupy * 5)/10;
		if (t_occupy > 100) t_occupy = 100;
		
		gstLocalPolling.occupy[i] = t_occupy;
	}

	//2014.05.13 by capi
	//printd("occ : ");
	for (i=0; i<(gucActiveDualLoopNum/2); i++)
	{
		if((gstLocalPolling.occupy[i*2] > 15)||(gstLocalPolling.occupy[i*2+1] > 15)) invers_st.occ_state_with_inv[i] = TRUE; 
		else invers_st.occ_state_with_inv[i] = FALSE;		
	}	
	
	for (i=0; i<MAX_LANE_NUM; i++) 
	{
		gstLocalAccuData.speed[i] = 0;
		gstLocalAccuData.length[i] = 0;
		
	}	
	for (i=0; i<MAX_LOOP_NUM; i++)
	{
		gstLocalAccuData.volume[i] = 0;
		gstLocalAccuData.occupy[i] = 0;
	}

	//polling_data_valid = TRUE;
	//polling_disp_flg = _POLLING_DATA_RDY_;

#if defined(SUPPORT_CENTER_TRAFFIC_DATA_PROTOCOL)
	if (isLocalPoll_SendFlag() == TRUE)
	{
		if (isLocalPoll_AllLane() == TRUE)
		{
			sendAllLocalPollDataPkt();
		}
		else
		{
			sendLocalPollDataPacket(center_poll_lane[0]);
			sendLocalPollDataPacket(center_poll_lane[1]);
		}
	}
#endif

}
#endif // #if defined(SUPPORT_LOCAL_TRAFFIC_DATA)


void sendRealtimeIndivDataPacket(uint32 t_lane, uint8 reverse)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	uint32 speed_integer;
	INDIV_DATA_RSP_PACKET respPkt;

#if 1
	if (!indiv_data_flg) return;
	if (indiv_data_lane != _ALL_LANE_)
	{
		if (t_lane >= MAX_LANE_NUM || t_lane != indiv_data_lane) return;
	}
#else
	if (!indiv_data_flg || t_lane != indiv_data_lane) return;
	if (t_lane >= MAX_LANE_NUM) return;
#endif
	
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	
	respPkt.head.opcode = CMD_INDIVIDUAL_DATA_REQ;

	// 시간
	respPkt.data.time.year_hi = sys_time.wYear/100;
	respPkt.data.time.year_lo = sys_time.wYear%100;
	respPkt.data.time.month = sys_time.wMonth;
	respPkt.data.time.day = sys_time.wDay;
	respPkt.data.time.hour = sys_time.wHour;
	respPkt.data.time.min = sys_time.wMinute;
	respPkt.data.time.sec_hi = sys_time.wSecond;
	respPkt.data.time.sec_lo = (io_time_counter/10)%100; // 사실 문제점이 많은 방식임. by jwank 090720 dddd

	// 데이터.
	respPkt.data.lane = t_lane+1;	
	speed_integer = gstCurrData.speed[t_lane]/100;
	
#if defined(INCREASE_SPEED_PRECISION)
	respPkt.data.spd_hi = speed_integer/100;
	if (reverse == TRUE) respPkt.data.spd_hi |= 0x80;	// reverse 차량.
	respPkt.data.spd_md = speed_integer%100;
	respPkt.data.spd_lo = gstCurrData.speed[t_lane]%100;
#else // #if defined(INCREASE_SPEED_PRECISION)
	respPkt.data.spd_hi = gstCurrData.speed[t_lane]/100;
	if (reverse == TRUE) respPkt.data.spd_hi |= 0x80;	// reverse 차량.
	respPkt.data.spd_lo = gstCurrData.speed[t_lane]%100;
#endif // #if defined(INCREASE_SPEED_PRECISION)

	respPkt.data.lp1_oc_hi = gstCurrData.occupy[gstLaneLink[t_lane].s_loop]/100;
	respPkt.data.lp1_oc_lo = gstCurrData.occupy[gstLaneLink[t_lane].s_loop]%100;
	respPkt.data.lp2_oc_hi = gstCurrData.occupy[gstLaneLink[t_lane].e_loop]/100;
	respPkt.data.lp2_oc_lo = gstCurrData.occupy[gstLaneLink[t_lane].e_loop]%100;
	respPkt.data.len_hi = gstCurrData.length[t_lane]/100;
	respPkt.data.len_lo = gstCurrData.length[t_lane]%100;

	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout,"Recv : [Individual Data] \n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout,"TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout,"%02x ", tmptr[i]);

		fprintf(stdout, "\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	
}

void sendNoSavingdataPacket(void)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	SAVING_DATA_RSP_PACKET respPkt;

	// Header.
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_SAVED_TRAFFIC_DATA_REQ;

	// Sub Opcode.
	respPkt.sub_op = SUBCMD_NO_SAVED_DATA;

 	// Tail.
	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout," Send : [No Save Data] \n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout, " TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout, "%02x ", tmptr[i]);

		fprintf(stdout, "\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	
}


void sendAllCenterPollDataPkt(void)
{
	int i;
	uint8 *tmptr;
	uint16 crc, t_lane;
	ALL_CENTER_DATA_RSP_PACKET respPkt;

	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_CENTER_POLL_DATA_REQ;

	// 시간
	respPkt.time.year_hi = gstCenterPolling.ctime.year_hi;
	respPkt.time.year_lo = gstCenterPolling.ctime.year_lo;
	respPkt.time.month = gstCenterPolling.ctime.month;
	respPkt.time.day = gstCenterPolling.ctime.day;
	respPkt.time.s.hour = gstCenterPolling.ptime.hour;
	respPkt.time.s.min = gstCenterPolling.ptime.min;
	respPkt.time.s.sec = gstCenterPolling.ptime.sec;
	respPkt.time.e.hour = gstCenterPolling.ctime.hour;
	respPkt.time.e.min = gstCenterPolling.ctime.min;
	respPkt.time.e.sec = gstCenterPolling.ctime.sec;

	// 데이터.
	for (t_lane=0; t_lane<MAX_RIO_LANE_NUM; t_lane++)
	{
		respPkt.data[t_lane].lane = t_lane+1;
		respPkt.data[t_lane].lp1_vol = gstCenterPolling.volume[gstLaneLink[t_lane].s_loop];
		respPkt.data[t_lane].lp2_vol = gstCenterPolling.volume[gstLaneLink[t_lane].e_loop];
		respPkt.data[t_lane].spd_hi = gstCenterPolling.speed[t_lane]/100;
		respPkt.data[t_lane].spd_lo = gstCenterPolling.speed[t_lane]%100;

		#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
		respPkt.data[t_lane].lp1_oc = round_for_occupy(gstCenterPolling.occupy[gstLaneLink[t_lane].s_loop], 0);
		respPkt.data[t_lane].lp2_oc = round_for_occupy(gstCenterPolling.occupy[gstLaneLink[t_lane].e_loop], 0);
		#else
		respPkt.data[t_lane].lp1_oc = gstCenterPolling.occupy[gstLaneLink[t_lane].s_loop];
		respPkt.data[t_lane].lp2_oc = gstCenterPolling.occupy[gstLaneLink[t_lane].e_loop];
		#endif
		
		respPkt.data[t_lane].len_hi = gstCenterPolling.length[t_lane]/100;
		respPkt.data[t_lane].len_lo = gstCenterPolling.length[t_lane]%100;
	}

	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout, " Recv : [All-Center-Poll] \n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout, " TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout, "%02x ", tmptr[i]);

		fprintf(stdout, "\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));
}

void sendCenterPollDataPacket(uint32 t_lane)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	CENTER_DATA_RSP_PACKET respPkt;

#if 1
	if (!center_poll_data_flg) return;
	
	if (center_poll_lane[0] == _ALL_LANE_)
	{
		// .................
		return;		
	}
	
	if (t_lane >= MAX_LANE_NUM) return;
	if (t_lane != center_poll_lane[0] && t_lane != center_poll_lane[1]) return;
#else
	if (!center_poll_data_flg || t_lane != center_poll_lane[0]) return;
	if (t_lane >= MAX_LANE_NUM) return;
#endif

	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_CENTER_POLL_DATA_REQ;
	
	respPkt.time.year_hi = gstCenterPolling.ctime.year_hi;
	respPkt.time.year_lo = gstCenterPolling.ctime.year_lo;
	respPkt.time.month = gstCenterPolling.ctime.month;
	respPkt.time.day = gstCenterPolling.ctime.day;
	respPkt.time.s.hour = gstCenterPolling.ptime.hour;
	respPkt.time.s.min = gstCenterPolling.ptime.min;
	respPkt.time.s.sec = gstCenterPolling.ptime.sec;
	respPkt.time.e.hour = gstCenterPolling.ctime.hour;
	respPkt.time.e.min = gstCenterPolling.ctime.min;
	respPkt.time.e.sec = gstCenterPolling.ctime.sec;
	
	respPkt.data.lane = t_lane+1;
	respPkt.data.lp1_vol = gstCenterPolling.volume[gstLaneLink[t_lane].s_loop];
	respPkt.data.lp2_vol = gstCenterPolling.volume[gstLaneLink[t_lane].e_loop];
	respPkt.data.spd_hi = gstCenterPolling.speed[t_lane]/100;
	respPkt.data.spd_lo = gstCenterPolling.speed[t_lane]%100;

	#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee	
	respPkt.data.lp1_oc = round_for_occupy(gstCenterPolling.occupy[gstLaneLink[t_lane].s_loop], 0);
	respPkt.data.lp2_oc = round_for_occupy(gstCenterPolling.occupy[gstLaneLink[t_lane].e_loop], 0);
	#else
	respPkt.data.lp1_oc = gstCenterPolling.occupy[gstLaneLink[t_lane].s_loop];
	respPkt.data.lp2_oc = gstCenterPolling.occupy[gstLaneLink[t_lane].e_loop];
	#endif
	
	respPkt.data.len_hi = gstCenterPolling.length[t_lane]/100;
	respPkt.data.len_lo = gstCenterPolling.length[t_lane]%100;

	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout, " Recv : [Center-Poll] \n");
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout, " TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout, "%02x ", tmptr[i]);

		fprintf(stdout, "\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	
}


void interprete_maintenance_packet(uint8 *pktFrame)
{
	int i, g_addr, p_addr;;
	
	PACKET_TINY_HEAD *p_thead;


	g_addr = (pktFrame[0]<<8) | pktFrame[1];
	p_addr = (pktFrame[2]<<8) | pktFrame[3];	
		
	p_thead = (PACKET_TINY_HEAD *) &pktFrame[0];

	#if defined(DEBUG_MAINTENACE_COMM)
	fprintf(stdout, "\n\r OPCODE: [%02X]", p_thead->opcode);
	#endif // #if defined(DEBUG_MAINTENACE_COMM)
	
	fprintf(stdout, "\n\r OPCODE: [%02X] , CSN : %d-%d \n", p_thead->opcode, g_addr, p_addr);

	//fprintf(stdout, "\n\r  [%02X] [%02X] [%02X] [%02X] [%02X] [%02X] [%02X] [%02X] \n", pktFrame[0],pktFrame[1],pktFrame[2],pktFrame[3],pktFrame[4],pktFrame[5],pktFrame[6],pktFrame[7]);
	
	switch(p_thead->opcode)
	{
		case CMD_GET_SYSTEM_CONFIG_B:
		{
			uint8 tmpb[140], t_index;
			uint16 crc, count=0;

			PACKET_RESPOND_HEAD *p_head;
			PACKET_TAIL *p_tail;

			SYSTEM_CONFIG_PACKET * p_cpkt;

			p_cpkt = (SYSTEM_CONFIG_PACKET *) &pktFrame[0];

			p_head = (PACKET_RESPOND_HEAD *) &tmpb[0];

			p_head->dle = DLE;
			p_head->stx = STX;
			p_head->grb_addr[0] = (unsigned char) (g_addr>>8);
			p_head->grb_addr[1] = (unsigned char) (g_addr&0xFF);
			p_head->ctrl_addr[0] = (unsigned char) p_addr>>8;
			p_head->ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			//p_head->opcode = CMD_GET_SYSTEM_CONFIG;
			p_head->opcode = CMD_GET_SYSTEM_CONFIG_B;	// bug fix 091028

			checkSystemStatus();
			p_head->status = getSysStatusData();

			t_index = sizeof(PACKET_RESPOND_HEAD);

			///////////////////////////////////////////////////////////////////////////
			// System config Index
			tmpb[t_index++] = p_cpkt->index;

			//fprintf(stdout, "\n\r index: [%02X] , SIZE : %d \n", p_cpkt->index, t_index + -1);

			count = getSysConfigByIndex_B(p_cpkt->index, &tmpb[t_index]);
			t_index += count;
			///////////////////////////////////////////////////////////////////////////

			p_tail = (PACKET_TAIL *) &tmpb[t_index];

			p_tail->dle = DLE;
			p_tail->etx = ETX;

			crc = updateCRC16( (uint8 *) p_head->grb_addr, t_index - 2, 0 );

			p_tail->crc16 = ((crc & 0xff) << 8) | (crc >> 8);	

			fprintf(stdout, "\n\r status: [%04X] , SIZE : %d \n", p_head->status, t_index + sizeof(PACKET_TAIL));		

			tx_over_maintenance_port((uint8 *) &tmpb[0], t_index + sizeof(PACKET_TAIL));
		}
		break;

	case CMD_SET_SYSTEM_CONFIG_B:
		{
			uint8 *tmptr;
			uint16 crc;
			RESPOND_PACKET resp_pkt;
			SYSTEM_CONFIG_PACKET * p_cpkt;

			p_cpkt = (SYSTEM_CONFIG_PACKET *) &pktFrame[0];
			
			setAllSystemConfig_B((uint8 *)&p_cpkt->config[0]);
			
			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			//resp_pkt.head.opcode = CMD_SET_SYSTEM_CONFIG;
			//resp_pkt.head.opcode = CMD_RSPND_SET_SYS_CFG;
			resp_pkt.head.opcode = CMD_SET_SYSTEM_CONFIG_B;	// bug fix 091028

			system_status.controller_reset = _SYS_INVALID_;
			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) resp_pkt.head.grb_addr, sizeof( RESPOND_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);			
			
			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(RESPOND_PACKET));
		}
			break;

		case CMD_CONTROLLER_VALID:
			{
				//by capi 
				uint8 *tmptr;
				uint16 crc;
				CNTRL_VALID_PACKET resp_pkt;
				
				resp_pkt.head.dle = DLE;
				resp_pkt.head.stx = STX;
				resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
				resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
				resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
				resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
				resp_pkt.head.opcode = CMD_CONTROLLER_VALID;

				checkSystemStatus();
				resp_pkt.head.status = getSysStatusData();

				checkSystemValid();
				resp_pkt.valid_data = getSysValidData();

				resp_pkt.tail.dle = DLE;
				resp_pkt.tail.etx = ETX;

				crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( CNTRL_VALID_PACKET ) \
					- 2 - sizeof ( PACKET_TAIL ), 0 );

				resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

				fprintf(stdout," Recv : [Valid] \r\n");
				
				tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(CNTRL_VALID_PACKET));
			}
			break;
			
		case CMD_CONTROLLER_RESET:
			{
				uint8 *tmptr;
				uint16 crc;
				RESPOND_PACKET resp_pkt;
				
				resp_pkt.head.dle = DLE;
				resp_pkt.head.stx = STX;
				resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
				resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
				resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
				resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
				resp_pkt.head.opcode = CMD_CONTROLLER_RESET;

				system_status.controller_reset = _SYS_INVALID_;
				checkSystemStatus();
				resp_pkt.head.status = getSysStatusData();

				resp_pkt.tail.dle = DLE;
				resp_pkt.tail.etx = ETX;

				crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( RESPOND_PACKET ) \
					- 2 - sizeof ( PACKET_TAIL ), 0 );

				resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

				tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(RESPOND_PACKET));
				
				system("reboot");
				//for (i=0; i<10000; i++) transToMainte();

				//wait(3000);

				//RESET_FUNC();
			}
			break;
			
		case CMD_CONTROLLER_INIT:
			{
				uint8 *tmptr;
				uint16 crc;
				RESPOND_PACKET resp_pkt;
				
				resp_pkt.head.dle = DLE;
				resp_pkt.head.stx = STX;
				resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
				resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
				resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
				resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
				resp_pkt.head.opcode = CMD_CONTROLLER_INIT;

				checkSystemStatus();
				resp_pkt.head.status = getSysStatusData();

				resp_pkt.tail.dle = DLE;
				resp_pkt.tail.etx = ETX;

				crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( RESPOND_PACKET ) \
					- 2 - sizeof ( PACKET_TAIL ), 0 );

				resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);				

				tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(RESPOND_PACKET));
				//for (i=0; i<10000; i++) transToMainte();

			}

			fprintf(stdout," Recv : [Init] \r\n");
			// Inserted by jwank 061112
			//defaultVdsParameter();
			//gstSysConfig.is_default_param = _DEFAULT_PARAM_;
			//writeParamToNAND(TRUE); 
			//wait(3000);	

			close_keyboard();
			
			save_log( MAINTENANCE_RESET,0);

			exit(1);

			break;

		// 진단프로그램
		case OP_SERVER_STATUS_CHK :
		{
			//////////////////////////////////////////////////////////////////////
			//fprintf(stdout," Recv : OP_SERVER_STATUS_CHK \r\n");
			//////////////////////////////////////////////////////////////////////

			DIA_CONNSTATUS_RSP_PACKET resp_pkt;
			uint16 crc;

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);

			resp_pkt.head.opcode = OP_SERVER_STATUS_CHK;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			resp_pkt.hw_link = dia_status.hw_link;
			resp_pkt.svr_link = dia_status.svr_link;
			resp_pkt.last_opc = dia_status.last_opc;
		 	resp_pkt.year[0] = vds_server_stamp.wYear/100;
		 	resp_pkt.year[1] = vds_server_stamp.wYear%100;			
		 	resp_pkt.month = vds_server_stamp.wMonth;
		 	resp_pkt.day = vds_server_stamp.wDay;
		 	resp_pkt.hour = vds_server_stamp.wHour;
		 	resp_pkt.min = vds_server_stamp.wMinute;
		 	resp_pkt.sec = vds_server_stamp.wSecond;

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof(DIA_CONNSTATUS_RSP_PACKET) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(DIA_CONNSTATUS_RSP_PACKET));					
		}
		break;

		case OP_IO_STATUS_CHK :
		{
			//////////////////////////////////////////////////////////////////////
			//fprintf(stdout," Recv : OP_IO_STATUS_CHK \r\n");
			//////////////////////////////////////////////////////////////////////	

			uint val1, val2;	

			DIA_IOSTATUS_RSP_PACKET resp_pkt;
			uint16 crc;

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = OP_IO_STATUS_CHK;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();
			
			gpio_read(power1_fd, &val1);			
			gpio_read(power2_fd, &val2);	

			//상태정보만 한번더 보냄(추후 확장 대비)
			//resp_pkt.status = getSysStatusData();

			resp_pkt.pw[0] = val1;
			resp_pkt.pw[1] = val2;

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof(DIA_IOSTATUS_RSP_PACKET) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(DIA_IOSTATUS_RSP_PACKET));
		}
		break;

		case OP_TEMP_SENSOR_CHK :
		{
			//////////////////////////////////////////////////////////////////////
			//fprintf(stdout," Recv : OP_TEMP_SENSOR_CHK \r\n");
			//////////////////////////////////////////////////////////////////////

			DIA_TEMPSTATUS_RSP_PACKET resp_pkt;
			uint16 crc;

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = OP_TEMP_SENSOR_CHK;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();
			
			dia_Temp_Status();
			resp_pkt.temp[0] = dia_status.temp[0];
			resp_pkt.temp[1] = dia_status.temp[1];

			//resp_pkt.temp[0] = g_currSysStatus.temper[TEMPER1];
			//resp_pkt.temp[1] = g_currSysStatus.temper[TEMPER2];

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof(DIA_TEMPSTATUS_RSP_PACKET) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(DIA_TEMPSTATUS_RSP_PACKET));			
		}
		break;

		case OP_RELAY_CHK :
		{	
			//////////////////////////////////////////////////////////////////////
			//fprintf(stdout," Recv : OP_RELAY_CHK \r\n");
			//////////////////////////////////////////////////////////////////////	
			
			DIA_TEMPSTATUS_RSP_PACKET resp_pkt;
			uint16 crc;

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = OP_RELAY_CHK;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();
			
			resp_pkt.temp[0] = pktFrame[7];
			resp_pkt.temp[1] = pktFrame[8];

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( DIA_TEMPSTATUS_RSP_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(DIA_TEMPSTATUS_RSP_PACKET));

			//fprintf(stdout," pktFrame[5] : %d \r\n", pktFrame[5]);
			//fprintf(stdout," pktFrame[6] : %d \r\n", pktFrame[6]);

			if(pktFrame[5] == 0){						// fan
				if(pktFrame[6] == 1){
					g_currSysStatus.relay_state[0] = RELAY_ON;	
					//system_status.fan_operate = _OPERATE_;
					system_status.fan_operate = 1;
					//turnOnFAN(); 			// fan start
					write(fan_fd, "1", 1);
					}
				else if(pktFrame[6] == 0){
					g_currSysStatus.relay_state[0] = RELAY_OFF;	
					//system_status.fan_operate = _NOT_OPERATE_;					
					system_status.fan_operate = 0;
					//turnOffFAN(); 			// fan stop
					write(fan_fd, "0", 1);
					}
				}
			else if(pktFrame[5] == 1){					// heater
				if(pktFrame[6] == 1){
					g_currSysStatus.relay_state[1] = RELAY_ON;	
					//system_status.heater_operate = _OPERATE_;
					system_status.heater_operate = 1;
					//turnOnHeater();	 	// heater start
					write(heater_fd, "1", 1);
					}
				else if(pktFrame[6] == 0){
					g_currSysStatus.relay_state[1] = RELAY_OFF;
					//system_status.heater_operate = _NOT_OPERATE_;					
					system_status.heater_operate = 0;
					//turnOffHeater(); 		// heater stop
					write(heater_fd, "0", 1);
					}
				}
		}
		break;

		case OP_PACKET_CHK :
		{
			//////////////////////////////////////////////////////////////////////
			fprintf(stdout," Recv : OP_PACKET_CHK \r\n");
			//////////////////////////////////////////////////////////////////////		
		}
		break;						

		case CMD_GET_SYSTEM_VERSION:
		{			
			//uint8 *tmptr;
			uint16 crc;
			VERSION_RSP_PACKET resp_pkt;
			//int year, day;
			char sprintbuffer[256];
			char strdate[5];

			sprintf(sprintbuffer,"%s", __DATE__);

			/*memset (month, 0x0, sizeof(month));		
			strncpy(month, sprintbuffer, sizeof(month));
			memset (sday, 0x0, sizeof(sday));
			strncpy(sday, sprintbuffer+4, sizeof(sday));
			memset (syear, 0x0, sizeof(syear));
			strncpy(syear, sprintbuffer+7, sizeof(syear));
			*/			
						
			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = CMD_GET_SYSTEM_VERSION;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			//////////////////////////////////////////////////////////////////////
			
			resp_pkt.hw_version = VERSION_NUM;
			resp_pkt.sw_version = SW_VERSION_NUM;			

			sprintf(strdate,"%c%c%c", sprintbuffer[0], sprintbuffer[1],  sprintbuffer[2]);	
			
			if(strcmp(strdate, "Jan") == 0) resp_pkt.manu_month =  1;
			else if(strcmp(strdate, "Feb") == 0) resp_pkt.manu_month =  2;	
			else if(strcmp(strdate, "Mar") == 0) resp_pkt.manu_month =  3;	
			else if(strcmp(strdate, "Apr") == 0) resp_pkt.manu_month =  4;	
			else if(strcmp(strdate, "May") == 0) resp_pkt.manu_month =  5;	
			else if(strcmp(strdate, "Jun") == 0) resp_pkt.manu_month =  6;	
			else if(strcmp(strdate, "Jul") == 0) resp_pkt.manu_month =  7;
			else if(strcmp(strdate, "Aug") == 0) resp_pkt.manu_month =  8;
			else if(strcmp(strdate, "Sep") == 0) resp_pkt.manu_month =  9;
			else if(strcmp(strdate, "Oct") == 0) resp_pkt.manu_month =  10;
			else if(strcmp(strdate, "Nov") == 0) resp_pkt.manu_month =  11;
			else if(strcmp(strdate, "Dec") == 0) resp_pkt.manu_month =  12;
			else resp_pkt.manu_month =  0;			

			sprintf(strdate,"%c%c%c%c", sprintbuffer[7], sprintbuffer[8], sprintbuffer[9], sprintbuffer[10]);	
			
			resp_pkt.manu_year[0] = (uint8) (atoi(strdate)/100);
			resp_pkt.manu_year[1] =  (uint8)(atoi(strdate)%100);

			sprintf(strdate,"%c%c", sprintbuffer[4], sprintbuffer[5]);				;
			resp_pkt.manu_day = (uint8) atoi(strdate);			

			//////////////////////////////////////////////////////////////////////

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( VERSION_RSP_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(VERSION_RSP_PACKET));
		}
		break;

		case CMD_SET_MAC_ADDRESS :
		{
			MACADDRESS_REQ_PACKET *pkt;
			//CHG_MAC_RESP_PACKET_t *pRespPkt = (CHG_MAC_RESP_PACKET_t *) vds_tx_buf;
			
			pkt = (MACADDRESS_REQ_PACKET *) &pktFrame[0];
			//////////////////////////////////////////////////////////////////////

			for(i=0; i<MAC_ADDR_LEN; i++) gstNetConfig.mac[i] = pkt->new_mac[i];
			
			setInterfaces();	//by capi interface file write
			writeNetConfigToNAND(TRUE);
			dumpNetworkConfig();		

			//////////////////////////////////////////////////////////////////////		
		}
		break;

		case CMD_GET_MAC_ADDRESS:
		{
			MACADDRESS_RSP_PACKET resp_pkt;
			uint16 crc;
			
			
			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = CMD_GET_MAC_ADDRESS;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			for(i=0; i<MAC_ADDR_LEN; i++) resp_pkt.mac[i] =  gstNetConfig.mac[i];

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( MACADDRESS_RSP_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(MACADDRESS_RSP_PACKET));

			//////////////////////////////////////////////////////////////////////			
		}
		break;

		case CMD_SET_CUR_TIME:
		{

                      	SYSTEMTIME tmptime;
                        char timebuf[100];
                        SYSTEM_TIME_REQ_PACKET *pkt;

                        pkt = (SYSTEM_TIME_REQ_PACKET *) &pktFrame[0];
                        //for(i=0;i<7;i++)
                        //      printf("%d ", pkt->config[i]);

                        //----------------------------------------------------------------------
                        tmptime.wYear = pkt->config[0]+2000;
                        tmptime.wMonth = pkt->config[1];
                        tmptime.wDay = pkt->config[2];
                        tmptime.wHour = pkt->config[3];
                        tmptime.wMinute = pkt->config[4];

			// 2016.11.29   insert kwj. 
			// compensation time delay -> curtime + 1
			if( pkt->config[4] >= 59)
                        	tmptime.wSecond = 0;
			else
                        	tmptime.wSecond = pkt->config[5]+1;

                        fprintf(stdout,"\nSet Data & Time to RTC!\n");
                        fprintf(stdout,"%d/%d/%d %d:%d:%d\n\r", tmptime.wYear, tmptime.wMonth, \
                                    tmptime.wDay, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);

                        //OS Time change
                        sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
                                         ,tmptime.wYear, tmptime.wMonth, tmptime.wDay
                                         , tmptime.wHour, tmptime.wMinute, tmptime.wSecond);
                        system(timebuf);

                        msec_sleep(100);
                        setSysTimeToRTC(&tmptime);
                        //----------------------------------------------------------------------

		}				
		break;

		case CMD_SET_TEMP:
		{
			SET_TEMPER_REQ_PACKET * p_cpkt;
			GET_TEMPER_RSP_PACKET resp_pkt;
			uint16 crc;
			
			p_cpkt = (SET_TEMPER_REQ_PACKET *) &pktFrame[0];
			

			gstSysConfig.temper[0] = p_cpkt->temp[0];
			gstSysConfig.temper[1] = p_cpkt->temp[1];					
			
			sendSysConfigMsgToMMI();	
			writeParamToNAND(TRUE); 		

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = CMD_GET_TEMP;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			resp_pkt.divi = 0;
			resp_pkt.temp[0] = gstSysConfig.temper[0];
			resp_pkt.temp[1] = gstSysConfig.temper[1];
			resp_pkt.curtemp[0] = g_currSysStatus.temper[0];
			resp_pkt.curtemp[1] = g_currSysStatus.temper[1];

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( GET_TEMPER_RSP_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(GET_TEMPER_RSP_PACKET));
			//if(gstSysConfig.temper[0]<0) pRespPkt->divi |= 0x08;
			//if(gstSysConfig.temper[1]<0) pRespPkt->divi |= 0x04;
			//if(g_currSysStatus.temper[0]<0) pRespPkt->divi |= 0x02;
			//if(g_currSysStatus.temper[1]<0) pRespPkt->divi |= 0x01;

			//////////////////////////////////////////////////////////////////////			
		}
		break;
		
	case CMD_GET_TEMP:
		{
			GET_TEMPER_RSP_PACKET resp_pkt;
			uint16 crc;
			//CHG_MAC_RESP_PACKET_t *pRespPkt = (CHG_MAC_RESP_PACKET_t *) vds_tx_buf;
			
			//pkt = (MACADDRESS_RSP_PACKET *) &pktFrame[0];
			//////////////////////////////////////////////////////////////////////
			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			resp_pkt.head.opcode = CMD_GET_TEMP;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();
			
			resp_pkt.temp[0] = gstSysConfig.temper[0];
			resp_pkt.temp[1] = gstSysConfig.temper[1];
			resp_pkt.curtemp[0] = g_currSysStatus.temper[0];
			resp_pkt.curtemp[1] = g_currSysStatus.temper[1];
			resp_pkt.divi = 0;

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16( (uint8 *) &resp_pkt.head.grb_addr, sizeof( GET_TEMPER_RSP_PACKET ) \
				- 2 - sizeof ( PACKET_TAIL ), 0 );

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(GET_TEMPER_RSP_PACKET));
			

			//////////////////////////////////////////////////////////////////////			
		}
		break;	

	// parameter upload
	case CMD_PARAMETER_DOWNLOAD:
		{
			uint8 *tmptr;
			uint16 crc;
			RESPND_PARAM_DOWN_PACKET resp_pkt;
			DOWN_PARAM_PACKET * p_cpkt;

			p_cpkt = (DOWN_PARAM_PACKET *) &pktFrame[0];

			putVdsParameter(p_cpkt->index, (uint8 *) &p_cpkt->param[0], TRUE);

			resp_pkt.head.dle = DLE;
			resp_pkt.head.stx = STX;
			resp_pkt.head.grb_addr[0] = (unsigned char) (g_addr>>8);
			resp_pkt.head.grb_addr[1] = (unsigned char) (g_addr&0xFF);
			resp_pkt.head.ctrl_addr[0] = (unsigned char) p_addr>>8;
			resp_pkt.head.ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
			//respPkt.head.opcode = CMD_PARAMETER_DOWNLOAD;
			resp_pkt.head.opcode = CMD_RSPND_DOWN_PARAM;

			checkSystemStatus();
			resp_pkt.head.status = getSysStatusData();

			resp_pkt.index = p_cpkt->index;

			resp_pkt.tail.dle = DLE;
			resp_pkt.tail.etx = ETX;

			crc = updateCRC16((uint8 *)&resp_pkt.head.grb_addr, sizeof(resp_pkt) \
				- 2 - sizeof(PACKET_TAIL), 0);

			resp_pkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);			

			tx_over_maintenance_port((uint8 *)&resp_pkt, sizeof(resp_pkt));
		}
		break;

	case CMD_PARAMETER_UPLOAD:
		{
			uint8 tmpb[140], t_index;
			uint16 crc, count=0;

			PACKET_RESPOND_HEAD *p_head;
			PACKET_TAIL *p_tail;

			DOWN_PARAM_PACKET * p_cpkt;

			p_cpkt = (DOWN_PARAM_PACKET *) &pktFrame[0];

			p_head = (PACKET_RESPOND_HEAD *) &tmpb[0];

			p_head->dle = DLE;
			p_head->stx = STX;

			p_head->grb_addr[0] = (unsigned char) (g_addr>>8);
			p_head->grb_addr[1] = (unsigned char) (g_addr&0xFF);
			p_head->ctrl_addr[0] = (unsigned char) p_addr>>8;
			p_head->ctrl_addr[1] = (unsigned char) (p_addr&0xFF);
		
			p_head->opcode = CMD_PARAMETER_UPLOAD;

			checkSystemStatus();
			p_head->status = getSysStatusData();

			t_index = sizeof(PACKET_RESPOND_HEAD);

			// Parameter Index
			tmpb[t_index++] = p_cpkt->index;

			count = getVdsParameter( p_cpkt->index, &tmpb[t_index] );

			t_index += count;
			//if (count)

			p_tail = (PACKET_TAIL *) &tmpb[t_index];

			p_tail->dle = DLE;
			p_tail->etx = ETX;

			crc = updateCRC16( (uint8 *) &p_head->grb_addr, t_index - 2, 0 );

			p_tail->crc16 = ((crc & 0xff) << 8) | (crc >> 8);			

			tx_over_maintenance_port((uint8 *) &tmpb[0], t_index + sizeof(PACKET_TAIL));
		}
		break;

	case CMD_INDIVIDUAL_DATA_REQ:
		{
			INDIV_DATA_REQ_PKT * p_cpkt;
			p_cpkt = (INDIV_DATA_REQ_PKT *) &pktFrame[0];

			if (p_cpkt->lane_no == _ALL_LANE_)
				indiv_data_lane = _ALL_LANE_;
			else if (p_cpkt->lane_no > 0 && p_cpkt->lane_no <= MAX_LANE_NUM)
				indiv_data_lane = p_cpkt->lane_no - 1;
			else
				indiv_data_lane = 0xff;
				
			indiv_data_flg = p_cpkt->on_off;
			
			#if defined(DEBUG_MAINTENACE_COMM)
			if (getml() == msg_mainte_protocol)
			{
				fprintf(stdout," [Individual data req.]: %d, %d \r\n", 
						p_cpkt->lane_no, p_cpkt->on_off);
			}
			#endif // #if defined(DEBUG_MAINTENACE_COMM)			
		}
		break;
	case CMD_LOCAL_POLL_DATA_REQ:
		{
			POLL_DATA_REQ_PKT * p_cpkt;
			p_cpkt = (POLL_DATA_REQ_PKT *) &pktFrame[0];

			// lane[0]
			if (p_cpkt->lane_no[0] == _ALL_LANE_)
				local_poll_lane[0] = _ALL_LANE_;
			else if (p_cpkt->lane_no[0] > 0 && p_cpkt->lane_no[0] <= MAX_LANE_NUM)
				local_poll_lane[0] = p_cpkt->lane_no[0] - 1;
			else
				local_poll_lane[0] = 0xFF;

			// lane[1]
			if (p_cpkt->lane_no[1] == _ALL_LANE_)
				local_poll_lane[1] = _ALL_LANE_;
			else if (p_cpkt->lane_no[1] > 0 && p_cpkt->lane_no[1] <= MAX_LANE_NUM)
				local_poll_lane[1] = p_cpkt->lane_no[1] - 1;
			else
				local_poll_lane[1] = 0xFF;
				
			local_poll_data_flg = p_cpkt->on_off;
			
			#if defined(DEBUG_MAINTENACE_COMM)
			if (getml() == msg_mainte_protocol)
			{
				fprintf(stdout," [Local polling data req.]: %d, %d\r\n", 
						p_cpkt->lane_no, p_cpkt->on_off);
			}
			#endif // #if defined(DEBUG_MAINTENACE_COMM)
		}
		break;

	case CMD_CENTER_POLL_DATA_REQ:
		{
			POLL_DATA_REQ_PKT * p_cpkt;
			p_cpkt = (POLL_DATA_REQ_PKT *) &pktFrame[0];

			// lane[0]
			if (p_cpkt->lane_no[0] == _ALL_LANE_)
				center_poll_lane[0] = _ALL_LANE_;
			else if (p_cpkt->lane_no[0] > 0 && p_cpkt->lane_no[0] <= MAX_LANE_NUM)
				center_poll_lane[0] = p_cpkt->lane_no[0] - 1;
			else
				center_poll_lane[0] = 0xFF;

			// lane[1]
			if (p_cpkt->lane_no[1] == _ALL_LANE_)
				center_poll_lane[1] = _ALL_LANE_;
			else if (p_cpkt->lane_no[1] > 0 && p_cpkt->lane_no[1] <= MAX_LANE_NUM)
				center_poll_lane[1] = p_cpkt->lane_no[1] - 1;
			else
				center_poll_lane[1] = 0xFF;
				
			center_poll_data_flg = p_cpkt->on_off;
			
			#if defined(DEBUG_MAINTENACE_COMM)
			if (getml() == msg_mainte_protocol)
			{
				fprintf(stdout," [Center polling data req.]: %d, %d\r\n", 
						p_cpkt->lane_no, p_cpkt->on_off);
			}
			#endif // #if defined(DEBUG_MAINTENACE_COMM)
		}
		break;
	
	case CMD_SAVED_TRAFFIC_DATA_REQ:
		{
			SAVED_TAFFIC_DATA_REQ_PKT * p_cpkt;
			p_cpkt = (SAVED_TAFFIC_DATA_REQ_PKT *) &pktFrame[0];

			switch(p_cpkt->sub_op)
			{
			case SUBCMD_REQ_REALTIME_DATA:
				if (p_cpkt->d.on_off && kickRTSavingData_m())
				{
					send_saved_rt_data_flg = p_cpkt->d.on_off;
					send_saved_pl_data_flg = 0;	
					center_poll_data_flg = 0;	
					local_poll_data_flg = 0;	
					indiv_data_flg = 0;			

					saving_send_counter = 0;					
					seq_num_of_saved_rt = 0;
				}
			 	else
			 	{
			 		//send_saved_rt_data_flg = p_cpkt->d.on_off; // by jwank 090721
			 		send_saved_rt_data_flg = 0;
			 		send_saved_pl_data_flg = 0;	

			 		saving_send_counter = 0;					
					seq_num_of_saved_rt = 0;
					
			 		sendNoSavingdataPacket();
			 	}
			 		
				
				#if defined(DEBUG_MAINTENACE_COMM)
				chkprintf(msg_mainte_protocol, " [Request Saved-RT-Data] - %d", send_saved_rt_data_flg);
				#endif
				break;
				
			case SUBCMD_REQ_POLLING_DATA:
				if (p_cpkt->d.on_off && kickPLSavingData_m())
				{
					send_saved_pl_data_flg = p_cpkt->d.on_off;
					send_saved_rt_data_flg = 0;	
					center_poll_data_flg = 0;	
					local_poll_data_flg = 0;	
					indiv_data_flg = 0;			

					saving_send_counter = 0;
					seq_num_of_saved_pl = 0;
				}
				else
				{
					//send_saved_pl_data_flg = p_cpkt->d.on_off; // by jwank 090721
					send_saved_pl_data_flg = 0;
					send_saved_rt_data_flg = 0;	

					saving_send_counter = 0;
					seq_num_of_saved_pl = 0;
					
					sendNoSavingdataPacket();
				}
					
				
				#if defined(DEBUG_MAINTENACE_COMM)
				chkprintf(msg_mainte_protocol, " [Request Saved-PL-Data] - %d", send_saved_pl_data_flg);
				#endif
				break;
				
			default:

				#if defined(DEBUG_MAINTENACE_COMM)
				chkprintf(msg_mainte_protocol, " [This Sub-opcode is not Supported]");
				#endif
				break;
			}			
		}
		break;

	}	
}


void mainteProcessTask(void)
{
	int i, len;
	
	while(recvDataPortA.Start != recvDataPortA.End)
	{
		len = recvDataPortA.Length[recvDataPortA.Start];
	
		for(i=0; i<len; i++)
		{
			recvFramePortA[i] = recvDataPortA.Data[recvDataPortA.Start][i];
		}
		
		interprete_maintenance_packet(recvFramePortA);
						
		recvDataPortA.Start++;
		recvDataPortA.Start %= MAX_MODEM_DATA_QUEUE_NO;
	}
}


void sendSavingRTdataPacket(REALTIME_DATA_PACK_t *pRtData, uint8 isEnd)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	SAVING_INDIV_DATA_RSP_PACKET respPkt;

	// Header.
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_SAVED_TRAFFIC_DATA_REQ;

	// Sub Opcode.
	respPkt.sub_op = SUBCMD_REALTIME_DATA;

	// Sequence number.
	if (isEnd) seq_num_of_saved_rt = 254;	
	respPkt.seq_num = seq_num_of_saved_rt++;
	if (seq_num_of_saved_rt > 250) seq_num_of_saved_rt = 0;

	// Data.
 	respPkt.data = *pRtData;

 	// Tail.
	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout, " Send : [Save-Indiv] - %d \n", isEnd);
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout, " TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout, "%02x ", tmptr[i]);

		fprintf(stdout, "\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));
}


void sendSavingPLdataPacket(POLLING_DATA_PACK_t *pPlData, uint8 isEnd)
{
	int i;
	uint8 *tmptr;
	uint16 crc;
	SAVING_CENTER_DATA_RSP_PACKET respPkt;

	// Header.
	respPkt.head.dle = DLE;
	respPkt.head.stx = STX;
	respPkt.head.grb_addr[0] = gstNetConfig.station_num[0];
	respPkt.head.grb_addr[1] = gstNetConfig.station_num[1];
	respPkt.head.ctrl_addr[0] = gstNetConfig.station_num[2];
	respPkt.head.ctrl_addr[1] = gstNetConfig.station_num[3];
	respPkt.head.opcode = CMD_SAVED_TRAFFIC_DATA_REQ;

	// Sub Opcode.
	respPkt.sub_op = SUBCMD_POLLING_DATA;

	// Sequence number.
	if (isEnd) seq_num_of_saved_pl = 254;	
	respPkt.seq_num = seq_num_of_saved_pl++;
	if (seq_num_of_saved_pl > 250) seq_num_of_saved_pl = 0;

	// Data.
 	respPkt.data = *pPlData;

 	// Tail.
	respPkt.tail.dle = DLE;
	respPkt.tail.etx = ETX;

	crc = updateCRC16( (uint8 *) &respPkt.head.grb_addr, sizeof(respPkt) \
		- 2 - sizeof ( PACKET_TAIL ), 0 );

	respPkt.tail.crc16 = ((crc & 0xff) << 8) | (crc >> 8);

	#if defined(DEBUG_MAINTENACE_COMM)
	if (getml() == msg_mainte_protocol)
	{
		fprintf(stdout, " Send : [Save-Poll] - %d \n", isEnd);
		
		tmptr = (uint8 *) &respPkt;
		
		fprintf(stdout, " TX: ");
		for (i=0; i<sizeof(respPkt); i++) fprintf(stdout, "%02x ", tmptr[i]);

		fprintf(stdout,"\n");
	}
	#endif

	tx_over_maintenance_port((uint8 *)&respPkt, sizeof(respPkt));	
}

