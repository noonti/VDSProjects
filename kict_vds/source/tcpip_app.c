/* vim: set ts=4 sw=4: */
/* Filename    : io_epoll.c
 * Description : I/O multiplexing implementation with epoll
 * Author      : kwj 
 * Notes       : when define ENABLE_MSG_OOB, it allow to receive OOB data
 */
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
#include "stdalsp.h"
#include "ftms_cpu.h"
#include "tcpip.h"
#include "serial.h"
#include "user_term.h"
#include "tcp_vdsapp_protocol.h"
#include "mmi_protocol.h"

//typedef unsigned char uint8_t;

#define LISTEN_BACKLOG			256

#define MAX_VDS_RT_DATA_BANK		(20000)		//uip에서 전달가능 수치

#define LONG_PWR_FAIL_MSK		0
#define SHORT_PWR_FAIL_MSK		1	
#define DEFAULT_PARAM_MSK		2
#define RECV_BROADCAST_MSK		3
#define AUTO_RESYNC_MSK			4
#define FRONT_DOOR_OPEN_MSK		5
#define REAR_DOOR_OPEN_MSK		6
#define FAN_OPERATE_MSK			7
#define HEATER_OPERATE_MSK		8
#define CONTROLLER_RST_MSK		9
//#define ACKNOWLEDGE_MSK		14
//#define SYS_COMM_FAIL_MSK		15


#define FRONT_DOOR_PT_OFFSET	24	
#define REAR_DOOR_PT_OFFSET		25
#define FAN_OP_PT_OFFSET		26
#define HEATER_OP_PT_OFFSET		27
#define FRONT_DOOR_PT_MSK		(1<<FRONT_DOOR_PT_OFFSET)	
#define REAR_DOOR_PT_MSK		(1<<REAR_DOOR_PT_OFFSET)
#define FAN_OP_PT_MSK			(1<<FAN_OP_PT_OFFSET)
#define HEATER_OP_PT_MSK		(1<<HEATER_OP_PT_OFFSET)

#define _DOOR_OPEN_				1
#define _DOOR_CLOSE_			0

#define _OPERATE_				1
#define _NOT_OPERATE_			0

#define ADD_EV(a, b)	if (add_ev(a, b) == -1) { pr_err("Fail: add_ev"); exit(EXIT_FAILURE); }
#define DEL_EV(a, b)	if (del_ev(a, b) == -1) { pr_err("Fail: del_ev"); exit(EXIT_FAILURE); }

#define REALTIME_CLOCK_MSK		0

TCP_ONE_HOUR_VENHICLE_t gstVdsRTdataPool_tcp[MAX_VDS_RT_DATA_BANK];

/* I/O multiplexing var */
struct epoll_event	*ep_events;
int		ret_poll;
const int	max_ep_events	= 256;
int	epollfd;		/* epoll fd */
int fcntl_setnb(int fd);		/* set non-blocking mode */
int add_ev(int efd, int fd);	/* Add epoll variable to epollfd */
int del_ev(int efd, int fd); /* delete epoll variabe from epollfd */

/* network var			*/
socklen_t	len_saddr;
int		fd, fd_listener;
int		ret_recv;
char	*port, buf[1024];

uint8 incident_per_loop[MAX_LOOP_NUM/8];

/* logical var 			*/
int	i, startflag;
uint8 frame_num;

static unsigned char vds_tx_buf[sizeof(INDIV_VDS_DATA_RESP_PACKET_t) + sizeof(REALTIME_VENHICLE_t)*(MAX_VDS_RT_DATA_BANK-1)];
static char ch_src_ip[IP_STR_LEN];
static char ch_dest_ip[IP_STR_LEN];
static uint8 currSaveBank, sendRdyBank, currSaveBank_tcp, sendRdyBank_tcp;
static uint32 vdsRT_widx[2], vdsRT_widx_tcp[2];

char ipaddress[16]="192.168.0.27";
char *portnum="30100";
uint8 system_result;
uint16 sys_status_data;

uint8 gucStuckPerLoop[MAX_LOOP_NUM/4];

static uint32 vds_tx_len;
extern uint8 polling_data_valid;

//by capi
extern uint8 local_poll_lane[];
extern uint8 local_poll_data_flg;

extern uint8 center_poll_lane[];
extern uint8 center_poll_data_flg;

static uint32 one_tcp_num;
static uint32 one_tcp_wd;
int tcpip_connected=-1;

extern fd_tty;
extern int fd_ttys1;
extern int fd_ttys6;
extern u8 histFrame[100][17] ; 
extern u16 histCount;
u16 histCountBak=0;
u16 histCountTmp=0;

//extern network_alive;

uint8 Start_Stop_Flag;

static void __dump_message(uint8_t *buf, size_t sz)
{
    fprintf(stdout,"<");
    while (sz --) {
        fprintf(stdout,"%02X;", *buf);
        buf++;
    }
    fprintf(stdout,">\n");
}

// Bank 스위칭을 해서 보낼 데이터와 저장데이터를 분리한다.
void switchVdsBuffBank()
{
	// Bank 를 바꾼다.
	sendRdyBank = currSaveBank;
	currSaveBank = currSaveBank ? 0 : 1;

	// Bank 를 스위칭하고 나서 저장 index 를 0으로 해줘야 함.
	// 그렇지 않으면 그냥 쌓일 뿐 임.
	vdsRT_widx[currSaveBank] = 0;	// 091027
}
/////////////////////////////////////////////////////////////////////////////////////


void clearSystemResultcode()
{
	system_result = _SYS_VALID_;
}

void initSysValidGlobalVal(void)
{
	//system_status.invalid_req = _SYS_VALID_;
	//system_status.data_not_ready = _SYS_VALID_;
	//system_status.unknown_opcode = _SYS_VALID_;
	system_status.long_pwr_fail = _SYS_VALID_;
	system_status.short_pwr_fail = _SYS_VALID_;
	//system_status.invalid_data_field = _SYS_VALID_;
	system_status.default_param = _SYS_VALID_;
	system_status.recv_broadcast_msg = _SYS_VALID_;
	system_status.auto_resync = _SYS_VALID_;

	system_status.front_door_open = _DOOR_CLOSE_;
	system_status.rear_door_open = _DOOR_CLOSE_;
	system_status.fan_operate = _NOT_OPERATE_;
	system_status.heater_operate = _NOT_OPERATE_;

	system_status.controller_reset = _SYS_VALID_;
	//system_status.acknowledge = _SYS_VALID_;
	//system_status.sys_comm_fail = _SYS_INVALID_;

	system_valid.realtime_clock = _SYS_VALID_;
	system_valid.diom = _SYS_VALID_;

	system_valid.loop_det_amp[0] = _SYS_VALID_;
	system_valid.loop_det_amp[1] = _SYS_VALID_;
	system_valid.loop_det_amp[2] = _SYS_VALID_;
	system_valid.loop_det_amp[3] = _SYS_VALID_;
	
	system_valid.ultrasonic_det[0] = _SYS_VALID_;
	system_valid.ultrasonic_det[1] = _SYS_VALID_;
	
	system_valid.pwr_supply[0][0] = _SYS_VALID_;
	system_valid.pwr_supply[0][1] = _SYS_VALID_;
	system_valid.pwr_supply[1][0] = _SYS_VALID_;
	system_valid.pwr_supply[1][1] = _SYS_VALID_;
	
	system_valid.rom = _SYS_VALID_;
	system_valid.ram = _SYS_VALID_;
	
	system_valid.rev[0] = _SYS_VALID_;
	system_valid.rev[0] = _SYS_VALID_;

	//gstSysConfig.temper[0] = DEFAULT_FAN_TEMP;
	//gstSysConfig.temper[1] = DEFAULT_HT_TEMP;
	g_currSysStatus.main_sensor = TEMPER1;	
	g_currSysStatus.temp_status = 0;
	g_currSysStatus.temp_init = 0;
	g_currSysStatus.temp_save_time = 0;
	g_currSysStatus.timecnt = 0;
	g_currSysStatus.temp_error = 0;
	
	g_currSysStatus.relay_state[0] = RELAY_OFF;
	g_currSysStatus.relay_state[1] = RELAY_OFF;
	g_currSysStatus.relay_state[2] = RELAY_ON;
	g_currSysStatus.relay_state[3] = RELAY_ON;

	system_status.fan_operate = _NOT_OPERATE_;		
	system_status.heater_operate = _NOT_OPERATE_;		

	g_currSysStatus.relay_sw_delay[0] = 0;		//2011.11.23 by capidra
	g_currSysStatus.relay_sw_delay[1] = 0;
	g_currSysStatus.relay_sw_delay[2] = 0;
	g_currSysStatus.relay_sw_delay[3] = 0;

	//check_iocomm = 0; //I/O board comm check
}

void checkSystemStatus()
{

        //system_status.long_pwr_fail = _SYS_VALID_;                    //Delete by capi 2012.04.03
        //system_status.short_pwr_fail = _SYS_VALID_;

        system_status.default_param = \
                (gstSysConfig.is_default_param == _DEFAULT_PARAM_) ? _SYS_INVALID_ : _SYS_VALID_;

        system_status.recv_broadcast_msg = _SYS_VALID_;
        system_status.auto_resync = _SYS_VALID_;

        system_status.front_door_open = \
                (cur_non_mask_val & FRONT_DOOR_PT_MSK) ? _DOOR_OPEN_ : _DOOR_CLOSE_;


        system_status.rear_door_open = \
                (cur_non_mask_val & REAR_DOOR_PT_MSK) ? _DOOR_OPEN_ : _DOOR_CLOSE_;


#if 1 //ndef FAN_HEATER_AUTO_CONTROL
                system_status.fan_operate =
                        (cur_non_mask_val & FAN_OP_PT_MSK) ? _OPERATE_ : _NOT_OPERATE_;
#if 0
                if( system_status.fan_operate == 1) fprintf(stdout,"fan on\n");
                else fprintf(stdout,"fan off\n");
#endif
                system_status.heater_operate =
                        (cur_non_mask_val & HEATER_OP_PT_MSK) ? _OPERATE_ : _NOT_OPERATE_;
#if 0
                if( system_status.heater_operate == 1) fprintf(stdout,"heater on\n");
                else fprintf(stdout,"heater off\n");
#endif
        #endif
}
//by capidra 
uint16 sys_status_data;
uint16 getSysStatusData()
{
	sys_status_data = (uint16)(system_status.long_pwr_fail << 0)
		+ (uint16)(system_status.short_pwr_fail << 1)
		+ (uint16)(system_status.front_door_open << 2)
		+ (uint16)(system_status.rear_door_open << 3)
		+ (uint16)(system_status.fan_operate << 4)
		+ (uint16)(system_status.heater_operate << 5)
		+ (uint16)(system_status.controller_reset << 6);
//		+ (uint16)(system_status.data_not_ready << 7)
//		+ (uint16)(system_status.sys_comm_fail <<8);



/*	(uint16)(system_status.invaild_req << INVAILD_REQ_MSK)
		+ (uint16)(system_status.unknown_opcode << UNKNOWN_OP_MSK)
		+ (uint16)(system_status.invaild_data_field << INVAILD_D_FIELD_MSK)
		+ (uint16)(system_status.default_param << DEFAULT_PARAM_MSK)
		+ (uint16)(system_status.recv_broadcast_msg << RECV_BROADCAST_MSK)
		+ (uint16)(system_status.auto_resync << AUTO_RESYNC_MSK)
		+ (uint16)(system_status.acknowledge << ACKNOWLEDGE_MSK)
*/
	system_status.long_pwr_fail = _SYS_VALID_;
	system_status.short_pwr_fail = _SYS_VALID_;	

#ifdef DEBUG_SYS_COMM
	printf("\n\r\n\rSystem Status : %04x\n\r", sys_status_data);
#endif

	return (uint16)( (sys_status_data>>8) | (sys_status_data<<8) );
}

//by capidra 
uint8 getSysStatusDataIdx(uint8 idx)
{
	//retouch by capidra 2011.05.27
	sys_status_data = (uint16)(system_status.long_pwr_fail << LONG_PWR_FAIL_MSK)
		+ (uint16)(system_status.short_pwr_fail << SHORT_PWR_FAIL_MSK)		
		+ (uint16)(system_status.default_param << DEFAULT_PARAM_MSK)
		+ (uint16)(system_status.recv_broadcast_msg << RECV_BROADCAST_MSK)
		+ (uint16)(system_status.auto_resync << AUTO_RESYNC_MSK)
		+ (uint16)(system_status.front_door_open << FRONT_DOOR_OPEN_MSK)
		+ (uint16)(system_status.rear_door_open << REAR_DOOR_OPEN_MSK)
		+ (uint16)(system_status.fan_operate << FAN_OPERATE_MSK)
		+ (uint16)(system_status.heater_operate << HEATER_OPERATE_MSK)
		+ (uint16)(system_status.controller_reset << CONTROLLER_RST_MSK);
		
	//fprintf(stdout," System Status : 0x%04X", sys_status_data);
	
	if (idx == 1) 
	{
		system_status.long_pwr_fail = _SYS_VALID_;
		system_status.short_pwr_fail = _SYS_VALID_;	
		system_status.auto_resync = _SYS_VALID_;			//by capidra 2014.09.17
		system_status.recv_broadcast_msg = _SYS_VALID_;
	}

	sys_status_data = sys_status_data&0x03FF;
	
#ifdef DEBUG_SYS_COMM
	fprintf(stdout," System Status : 0x%04X", sys_status_data);
#endif

	if (idx == 1) return (uint8)(sys_status_data >> 8);		//버그 2011.07.06.by capidra 
	else return (uint8)(sys_status_data);
}

uint16 getSysValidData(void)
{
	sys_valid_data = (uint16)(system_valid.realtime_clock << REALTIME_CLOCK_MSK)
		+ (uint16)(system_valid.diom << DIOM_MSK)
		//(uint16)(system_valid.loop_det_amp[] << REALTIME_CLOCK_MSK)
		//(uint16)(system_valid.ultrasonic_det[] << REALTIME_CLOCK_MSK)
		//(uint16)(system_valid.pwr_supply[] << REALTIME_CLOCK_MSK)
		+ (uint16)(system_valid.rom << ROM_MSK)
		+ (uint16)(system_valid.ram << RAM_MSK)
		//(uint16)(system_valid.rev[] << REALTIME_CLOCK_MSK)
		;

	return (uint16)( (sys_valid_data>>8) | (sys_valid_data<<8) );
}

uint8 getSysValidDataIdx(uint8 idx)
{
	sys_valid_data = (uint16)(system_valid.realtime_clock << REALTIME_CLOCK_MSK)
		+ (uint16)(system_valid.diom << DIOM_MSK)
		//(uint16)(system_valid.loop_det_amp[] << REALTIME_CLOCK_MSK)
		//(uint16)(system_valid.ultrasonic_det[] << REALTIME_CLOCK_MSK)
		//(uint16)(system_valid.pwr_supply[] << REALTIME_CLOCK_MSK)
		+ (uint16)(system_valid.rom << ROM_MSK)
		+ (uint16)(system_valid.ram << RAM_MSK)
		//(uint16)(system_valid.rev[] << REALTIME_CLOCK_MSK)
		;

#ifdef DEBUG_SYS_COMM
	fprintf(stdout," System valid : 0x%04X\n", sys_valid_data);
#endif

	if (idx == 0) return (uint8)(sys_valid_data >> 8);
	else return (uint8)(sys_valid_data);
}

void checkSystemValid()
{

#if 1
	//system_valid.realtime_clock = _SYS_VALID_;

// by jwank 060208
#if 0
//	if ( sys_comm_counter > _3_SEC_TICK_ )
//		system_valid.diom = _SYS_INVALID_;
//	else
//		system_valid.diom = _SYS_VALID_;
#else
	system_valid.diom = _SYS_VALID_;
#endif
	
	system_valid.rom = _SYS_VALID_;
	system_valid.ram = _SYS_VALID_;
	
#else
	system_valid.realtime_clock = _SYS_VALID_;
	system_valid.diom = _SYS_VALID_;
	//system_valid.loop_det_amp[4];
	//system_valid.ultrasonic_det[2];
	//system_valid.pwr_supply[2][2];
	system_valid.rom = _SYS_VALID_;
	system_valid.ram = _SYS_VALID_;
	//system_valid.rev[2];
#endif

}

uint8 scanStuckLoop (void)	//2011.07.06 by capidra
{
	int i, offset=0;
	uint32 bit_msk;
	uint8 on_limit, off_limit;
	
	switch(gstSysConfig.stuck_thrh) 
	{
		case LOW_VOLUME_THRH :
			on_limit = gstSysConfig.param_cfg.stuck_on_low;
			off_limit = gstSysConfig.param_cfg.stuck_off_low;
			break;

		case HIGH_VOLUME_THRH :
			on_limit = gstSysConfig.param_cfg.stuck_on_high;
			off_limit = gstSysConfig.param_cfg.stuck_off_high;
			break;

		default :
			return FALSE;
			break;
	}
	// init.
	for (i=0; i<MAX_LOOP_NUM/4; i++) gucStuckPerLoop[i] = 0;	

	// 장애정보 55 출력 Err. -> 0으로 마스크. --> 추후수정 2016-09-08
	/*
	//for (i=0, bit_msk=1; i<MAX_LOOP_NUM; i++, bit_msk<<=1)
	for (i=0, bit_msk=1; i<gucActiveLoopNum; i++, bit_msk<<=1)
	{		
		if ( ( stuckon_counter[i] >= on_limit * _60_SEC_TICK_ ) && ( !( cur_value & bit_msk ) ) )		//2013.10.28 
		{
			gucStuckPerLoop[i/4] |= STUCK_ON_STS << offset;

#ifdef SEGMENT
			i2cSegDigit(1,2);
			i2cSegDigit(0,(int)(i/4));
#endif			
		}
		else if ( ( stuckoff_counter[i] >= off_limit *  _60_SEC_TICK_ ) && ( cur_value & bit_msk ) )		//2013.10.28 
		{
			gucStuckPerLoop[i/4] |= STUCK_OFF_STS << offset;

#ifdef SEGMENT
			i2cSegDigit(1,3);
			i2cSegDigit(0,(int)(i/4));
#endif			
		}
		else if ( gstCenterAccuData.volume[i] >= gstSysConfig.param_cfg.oscillation_thr )
		{
			gucStuckPerLoop[i/4] |= STUCK_OSC_STS << offset;

#ifdef SEGMENT
			i2cSegDigit(1,4);
			i2cSegDigit(0,(int)(i/4));
#endif			
		}
		
		offset += 2;
		offset %= 8;
	}
	*/	
	return TRUE;
	
}

void prepareCenterPollData()
{
	int i,j,k;
	u_int polling_count;
	uint32 t_occupy, cal;

	polling_count = centerPollCounter /** _TICK_MS_*/;  //by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
	
	// Reset counter for polling.	
	centerPollCounter = 0;

	// Reset counter for auto-sync.	
	centerAutosyncCounter = 0;
	localAutosyncCounter = 0;
	
	invers_st.start_inv_flag = TRUE;  //by capi 2015.01.07

	if (!polling_count) return;

	// 2011.06.21 by capidra
	// Syntax to produce the correct occupancy	
	for(i=0; i<gucActiveDualLoopNum; i++)
	{
		if (loop_st[i].status == PRESSED)
		{
			//It adds the current occupancy
			t_occupy = io_time_counter - loop_st[i].time[PRESS_TIME];			
			
			if (t_occupy >= polling_count) gstCenterAccuData.occupy[i] = polling_count;  // 100 %
			else gstCenterAccuData.occupy[i] += t_occupy;			
		}		
	}	

	//ramp
	for(i=0; i<gucActiveSingleLoopNum; i++)
	{
		if (loop_st[gucActiveDualLoopNum+i*2].status == PRESSED)
		{
			// It adds the current occupancy
			t_occupy = io_time_counter - loop_st[gucActiveDualLoopNum+i*2].time[PRESS_TIME];
			
			if (t_occupy >= polling_count) gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2] = polling_count;  // 100 %
			else gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2] += t_occupy;

			gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2+1] = gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2];
		}
	}

	// Save Local Polling time
	gstCenterPolling.ptime = gstCenterPolling.ctime;
	gstCenterPolling.ctime.year_hi = sys_time.wYear/100;
	gstCenterPolling.ctime.year_lo = sys_time.wYear%100;
	gstCenterPolling.ctime.month = sys_time.wMonth;
	gstCenterPolling.ctime.day = sys_time.wDay;
	gstCenterPolling.ctime.hour = sys_time.wHour;
	gstCenterPolling.ctime.min = sys_time.wMinute;
	gstCenterPolling.ctime.sec = sys_time.wSecond;


	//Calculation Average Data
	for (i=0; i<MAX_LANE_NUM; i++)
	{		
		if (gstLaneLink[i].e_loop < MAX_LOOP_NUM && gstCenterAccuData.volume[gstLaneLink[i].e_loop])	
		{			
			gstCenterPolling.speed[i] = gstCenterAccuData.speed[i] / gstCenterAccuData.volume[gstLaneLink[i].e_loop];
			gstCenterPolling.length[i] = gstCenterAccuData.length[i] /gstCenterAccuData.volume[gstLaneLink[i].e_loop];
		}
		else
		{			
			gstCenterPolling.speed[i] = 0;
			gstCenterPolling.length[i] = 0;
		}
	}

	// Reset counter for checking incident.
	for (i=0; i<(MAX_LOOP_NUM/8); i++) incident_per_loop[i] = 0;
	
	//If occtime to be occupied for more than 30 seconds, gstCenterPolling.occupy of 100
	//If the vehicle is stationary for more than 30 seconds, a volumn of 100	
	for (i=0; i<MAX_LOOP_NUM; i++)
	{
		gstCenterPolling.volume[i] = gstCenterAccuData.volume[i];

	#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
		// calculation three Decimal places
		t_occupy = 1000 * 100 * gstCenterAccuData.occupy[i];
		t_occupy /= polling_count;
		if (t_occupy > 100*1000) t_occupy = 100*1000;
		
		gstCenterPolling.occupy[i] = t_occupy;
		
	#else		
		
		#if 1 // bug fix ?? pppp

			// // calculation one Decimal places
			t_occupy = 10 * 100 * gstCenterAccuData.occupy[i];
			t_occupy /= polling_count;
			
			t_occupy += 5;
			t_occupy /= 10;

			// more than 100 per -> 100
			if (t_occupy > 100) t_occupy = 100;
		#else
			t_occupy = 10 * 100 * gstCenterAccuData.occupy[i] / polling_count;
			t_occupy = t_occupy / 5;
			t_occupy = (t_occupy * 5)/10;
			if (t_occupy > 100) t_occupy = 100;
		#endif
		
		gstCenterPolling.occupy[i] = t_occupy;
	#endif	

	//check incident insert by capi 2016.02.24
		cal = (gstCenterAccuData.occupy[i]/100) * gstSysConfig.param_cfg.K_factor[0] / 10;			
				
		if(cal >= gstSysConfig.param_cfg.T_value[i])
		{			
			keepup_incident[i]++;
			//keep_tm = keepup_incident[i]*5;
			if(keepup_incident[i] > gstSysConfig.param_cfg.persist_period)
			{
				k = (int)(i / 8);
				j = (int)(i % 8);
				incident_per_loop[k] |= 0x01<<j;
#ifdef SEGMENT				
				i2cSegDigit(1, 5);
				i2cSegDigit(0, k%8);	//current only digit
#endif				
			}
		}
		else
		{
			keepup_incident[i] = 0;
		}
	}
	
	// check Stuck.
	scanStuckLoop();

	// Polling Data reset
	for (i=0; i<MAX_LANE_NUM; i++) 
	{
		gstCenterAccuData.speed[i] = 0;
		gstCenterAccuData.length[i] = 0;
		
	}	
	for (i=0; i<MAX_LOOP_NUM; i++)
	{
		gstCenterAccuData.volume[i] = 0;
		gstCenterAccuData.occupy[i] = 0;
	}

	//2014.05.12 by capi	
	for (i=0; i<MAX_LANE_NUM; i++)
	{
		//printf("gstCenterPolling.occupy[%d] = %d\n", i*2, gstCenterPolling.occupy[i*2]);
		//printf("gstCenterPolling.occupy[%d] = %d\n", i*2+1, gstCenterPolling.occupy[i*2+1]);

		if((gstCenterPolling.occupy[i*2] > 15000)||(gstCenterPolling.occupy[i*2+1] > 15000)) invers_st.occ_state_with_inv[i] = TRUE;
		else invers_st.occ_state_with_inv[i] = FALSE;		
	}
	polling_data_valid = TRUE;
	polling_disp_flg = _POLLING_DATA_RDY_;

	savePollingData();

#if defined(SUPPORT_CENTER_TRAFFIC_DATA_PROTOCOL)
	if (isCenterPoll_SendFlag() == TRUE)
	{
		if (isCenterPoll_AllLane() == TRUE)
		{
			sendAllCenterPollDataPkt();
		}
		else
		{
			sendCenterPollDataPacket(center_poll_lane[0]);
			sendCenterPollDataPacket(center_poll_lane[1]);
		}
	}
#endif

}

//----------------------------------------------------------------------------------
// One Hour Data Send with TCP/IP
//--------------------------------------------------------------------------------
uint32 insertRtDataToVdsPacket(TCP_ONE_HOUR_VENHICLE_t *pRtDataBuf)  //2013.01.22 by capi
{
	uint32 i, chunk_num, send_num, rtpos;
	
	//int len;	
	//send_num = 
#if 1 // eeee

	if(one_tcp_num >= MAX_VDS_RT_DATA_BANK) rtpos = one_tcp_wd;
	else rtpos = 0;
		
	for (i=0, chunk_num=0; i<one_tcp_num; i++, chunk_num++, rtpos++)
	{
		if (i >= MAX_VDS_RT_DATA_BANK) break;

		rtpos %=  MAX_VDS_RT_DATA_BANK;
			
		pRtDataBuf[i].lane = gstVdsRTdataPool_tcp[rtpos].lane;
		pRtDataBuf[i].time = gstVdsRTdataPool_tcp[rtpos].time;
		pRtDataBuf[i].spd = gstVdsRTdataPool_tcp[rtpos].spd;
		//pRtDataBuf[i].spd_lo = gstVdsRTdataPool[i].spd_lo;		//Delete by capi 2013.01.22

		#if 1 // eeee bug fix 091026
		pRtDataBuf[i].occupy[START_POS] = gstVdsRTdataPool_tcp[rtpos].occupy[START_POS];
		pRtDataBuf[i].occupy[END_POS] = gstVdsRTdataPool_tcp[rtpos].occupy[END_POS];
		#else
		//pRtDataBuf[i].occupy[0] = ((uint16)gstVdsRTdataPool[i].occupy[END_POS] >> 8);
		//pRtDataBuf[i].occupy[1] = (gstVdsRTdataPool[i].occupy[END_POS] & 0xFF);
		#endif
		
		pRtDataBuf[i].kind = gstVdsRTdataPool_tcp[rtpos].kind;	
		
	}	

	one_tcp_num = 0;
	one_tcp_wd = 0;
#else // for protocol test

	send_num = MAX_VDS_RT_DATA_BANK;
	//send_num = 200;

	for (i=0, chunk_num=0; i<send_num; i++, chunk_num++)
	{
		if (i >= MAX_VDS_RT_DATA_BANK) break;

		#if 1
		pRtDataBuf[i].lane = 0xDD;
		pRtDataBuf[i].time = 0xDD;
		pRtDataBuf[i].speed = 0xDD;
		pRtDataBuf[i].speed = 0xDD;
		pRtDataBuf[i].occupy[0] = chunk_num>>8;
		pRtDataBuf[i].occupy[1] = chunk_num;
		pRtDataBuf[i].kind = 0xDD;
		#else
		pRtDataBuf[i].lane = i;
		pRtDataBuf[i].time = i*10;
		pRtDataBuf[i].speed = i*12;
		pRtDataBuf[i].speed = 99;
		pRtDataBuf[i].occupy[0] = chunk_num>>8;
		pRtDataBuf[i].occupy[1] = chunk_num;
		pRtDataBuf[i].kind = i+1;
		#endif
	}

#endif

	return (chunk_num);
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////
void op_det_req(u8_t *tmpFrame, u8 opcode)
{
    	DET_RESP_PACKET_t *pRespPkt = (DET_RESP_PACKET_t *) vds_tx_buf;
     	DET_RESP_PACKET_t respPkt;

     	respPkt.head.ver = 0x01;
     	for(i=0;i<8;i++)  respPkt.head.addr[i] = tmpFrame[i+1];
     	respPkt.head.opcode = opcode;
     	respPkt.head.len_hi = tmpFrame[10];
     	respPkt.head.len_lo = tmpFrame[11];

     	respPkt.opcode = OP_DET_RES; //0xB9
     	startflag = tmpFrame[13];
	respPkt.flag = startflag;	// 1: start, 2: stop

	int  flag_send = 0;
	send(fd_listener, (uint8 *)&respPkt, sizeof(respPkt), flag_send)  ;
	fprintf(stdout, "detect data send %d\n",startflag);	
}
void op_echo_req(u8_t *tmpFrame, u8 opcode)
{
   	ECHO_RESP_PACKET_t *pRespPkt = (ECHO_RESP_PACKET_t *) vds_tx_buf;
     	ECHO_RESP_PACKET_t respPkt;

     	respPkt.head.ver = 0x01;
     	for(i=0;i<8;i++)  respPkt.head.addr[i] = tmpFrame[i+1];
     	respPkt.head.opcode = opcode;
     	respPkt.head.len_hi = tmpFrame[10];
     	respPkt.head.len_lo = tmpFrame[11];

     	respPkt.opcode = OP_ECHO_RES; //0xBB
     	for(i=0;i<100;i++)  respPkt.echodata[i] = tmpFrame[i+13];

	int  flag_send = 0;
	send(fd_listener, (uint8 *)&respPkt, sizeof(respPkt), flag_send)  ;
	fprintf(stdout, "echo data send\n");	
}
void op_stat_req(u8_t *tmpFrame, u8 opcode)
{
    	STAT_RESP_PACKET_t *pRespPkt = (STAT_RESP_PACKET_t *) vds_tx_buf;
     	STAT_RESP_PACKET_t respPkt;
	uint16 kict_status;

     	respPkt.head.ver = 0x01;
     	for(i=0;i<8;i++)  respPkt.head.addr[i] = tmpFrame[i+1];
     	respPkt.head.opcode = opcode;
     	respPkt.head.len_hi = 0x00;
     	respPkt.head.len_lo = 0x11;

     	respPkt.opcode = OP_STAT_RES; // 0xBD
	// TIME
	respPkt.time.year_hi  	= BIN2BCD(sys_time.wYear/100);
	respPkt.time.year_lo 	= BIN2BCD(sys_time.wYear%100);
	respPkt.time.month  	= BIN2BCD(sys_time.wMonth);
	respPkt.time.day      	= BIN2BCD(sys_time.wDay);
	respPkt.time.hour     	= BIN2BCD(sys_time.wHour);
	respPkt.time.min 	= BIN2BCD(sys_time.wMinute);
	respPkt.time.sec_hi   	= BIN2BCD(sys_time.wSecond);
	respPkt.time.sec_lo   	= BIN2BCD((io_time_counter/10)%100); // ?ъ떎 臾몄젣?먯씠 留롮? 諛⑹떇?? by jwank 090720 dddd

	// STATUS
	kict_status =0;
	checkSystemStatus();
	kict_status = (getSysStatusData() & 0x01ff);			

	respPkt.status[0] =  (uint8)(kict_status >> 8 & 0xFF); 
	respPkt.status[1] =  (uint8)(kict_status  & 0xFF); 

	int  flag_send = 0;
	send(fd_listener, (uint8 *)&respPkt, sizeof(respPkt), flag_send)  ;
//	fprintf(stdout, "status data send %02x\n",kict_status);
}
void op_time_req(u8_t *tmpFrame, u8 opcode)
{
     	TIME_RESP_PACKET_t *pRespPkt = (TIME_RESP_PACKET_t *) vds_tx_buf;
     	TIME_RESP_PACKET_t respPkt;
	SYSTEMTIME saveTime;
	char timebuf[100];
			
	uint16 kict_status;
	int tmp, tmp1;
/*
	for(i=0;i<8;i++) fprintf(stdout,"%02x",tmpFrame[i+13]);
	fprintf(stdout,"\n");
	fprintf(stdout,"%02d:%02d.%02d\n",sys_time.wMinute,sys_time.wSecond,sys_time.wMilliseconds );
*/
     	respPkt.head.ver = 0x01;
     	for(i=0;i<8;i++)  respPkt.head.addr[i] = tmpFrame[i+1];
     	respPkt.head.opcode = opcode;
     	respPkt.head.len_hi = 0x00;
     	respPkt.head.len_lo = 0x09; 

     	respPkt.opcode = OP_TIME_RES; // 0xBF
	// TIME
     	tmp = BCD2BIN(tmpFrame[13]);
     	tmp1 = BCD2BIN(tmpFrame[14]);

	sys_time.wYear = tmp * 100 + tmp1;
	sys_time.wMonth = BCD2BIN(tmpFrame[15]);
	sys_time.wDay = BCD2BIN(tmpFrame[16]);
	sys_time.wHour = BCD2BIN(tmpFrame[17]);
	sys_time.wMinute = BCD2BIN(tmpFrame[18]);
	sys_time.wSecond = BCD2BIN(tmpFrame[19]);
	sys_time.wMilliseconds = BCD2BIN(tmpFrame[20]);
/*
              //OS Time change
                        sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
                                         ,saveTime.wYear, saveTime.wMonth, saveTime.wDay
                                         , saveTime.wHour, saveTime.wMinute, saveTime.wSecond);
                        system(timebuf);

                        msec_sleep(300);
                        setSysTimeToRTC(&saveTime);
*/								
	// STATUS
//	if ( getRtcTime(&sys_time, 0) ) 
//	{
//		printf("\n\rRTC error!\n\r");
//	}		

	respPkt.time.year_hi  	= tmpFrame[13];
	respPkt.time.year_lo 	= tmpFrame[14];
	respPkt.time.month  	= tmpFrame[15];
	respPkt.time.day      	= tmpFrame[16];
	respPkt.time.hour     	= tmpFrame[17];
	respPkt.time.min 	= tmpFrame[18];
	respPkt.time.sec_hi   	= tmpFrame[19];
	respPkt.time.sec_lo   	= tmpFrame[20]*10;

	int  flag_send = 0;
	send(fd_listener, (uint8 *)&respPkt, sizeof(respPkt), flag_send)  ;
//	fprintf(stdout, "time set data send \n");	
}
void op_hist_req(u8_t *tmpFrame, u8 opcode)
{
	u8 respPkt[5000];
	u16 iCount,tmp;
	u16 i,j;

	iCount = 0; 
	respPkt[iCount++] = 0x01;  	// [0] ver
     	for(i=0;i<8;i++)  respPkt[iCount++] = tmpFrame[i+1];
     	respPkt[iCount++] = opcode;		//[9] opcode
     	tmp = 1+1+(17*histCount);
     	respPkt[iCount++] = (tmp >> 8 & 0xff);	// [10] len_hi
     	respPkt[iCount++] = tmp & 0xff;		// [11] len_lo

     	respPkt[iCount++] = OP_HIST_RES;		// [12] 0xB3

     	if(histCount != 0) {
     	// Data
     	     if((histCount - histCountBak)>255) { 
     	     	tmp = 255;
     	      	respPkt[iCount++] = tmp;	// [13] data count   
     	     }  
     	     else{ tmp = (histCount - histCountBak) ; // [13] data count    
     	      	respPkt[iCount++] = tmp;	// [13] data count   
     	     	tmp = tmp-1;
     	     }     
//     	     for(i=0;i<iCount;i++) {
//     	     	fprintf(stdout, "%02x: ",respPkt[i] ); 
//     	     }
//     	          	fprintf(stdout, "\n"); 	
     	          	
     	     histCountTmp++;
     	     for(i=0;i<(tmp+1);i++) {
//     	                fprintf(stdout, "[%03d] ",histCountBak); 
     	           for(j=0;j<17;j++) {
     	                respPkt[iCount] = histFrame[i+(histCountTmp-1)*256][j];
//    	                fprintf(stdout, "%02x:",respPkt[iCount]); 
     	                iCount++;
     	     	}
 //    	           fprintf(stdout, "\n"); 	
    	     	watchdog_counter = 0;
     	     	histCountBak++;
     	     }
     	}
     	else {
     		respPkt[iCount++] = 0x00;	// [13] data count   
 	}
 //          fprintf(stdout, "%03d-%03d: %03d\n",histCount,histCountBak,iCount); 

     	if(histCount == histCountBak)  {
     	     histCount = 0;	// historical count clear
     	     histCountBak = 0;
     	     histCountTmp = 0;
     	}
 #if 1 
	int  flag_send = 0;
	send(fd_listener, (uint8 *)&respPkt, iCount /*sizeof(respPkt)*/, flag_send)  ;
//	fprintf(stdout, "historical data send \n");	
//	fflush(stdout);
#endif

}
////////////////////////////////////////////
void interprete_VDS_TCP_pkt(int fd, u8_t *pktFrame, u16_t frameLen)
{
	int i, j;
	int sys_init_flag = 0;
	uint32 pktLen = 0;		//2013.01.24 by capi
	
	VDS_TCP_PACKET_HEADER_t_1 *pRecvHead = (VDS_TCP_PACKET_HEADER_t_1 *) pktFrame;
	VDS_TCP_PKT_RESP_HEAD_t_1 *pSendHead = (VDS_TCP_PKT_RESP_HEAD_t_1 *) vds_tx_buf;
	
//	for(i=0;i<frameLen;i++) {
//		fprintf(stdout,"%02x",pktFrame[i]);
//	}

//	fprintf(stdout,"\n");

	switch(pktFrame[9])
	{
	     case OP_REQUEST:		//0xA1(161)
	     {
		switch(pktFrame[12] )	 // OP_CODE
		{
		     case OP_HIST_REQ: //0xB2
		     {
		     	op_hist_req(pktFrame,OP_RESPONCE);
			
		     	break;
		     }			
		     case OP_DET_REQ: //0xB8
		     {
		     	op_det_req(pktFrame,OP_RESPONCE);
			
		     	break;
		     }
		     case OP_ECHO_REQ: //0xBA
		     {
		     	op_echo_req(pktFrame,OP_RESPONCE);

		     	break;
		     }
		     case OP_STAT_REQ: //0xBC
		     {
		     	op_stat_req(pktFrame,OP_RESPONCE);
			
		     	break;
		     }
		     case OP_TIME_REQ: //0xBE
		     {
		     	op_time_req(pktFrame,OP_RESPONCE);
			fprintf(stdout,"Time sync...\n");
		     	break;
		     }	
		     default: break;	     
		}     	

	     }
	     case OP_RESPONCE:	//0xA2(162)
	     {
//		EVT_RESP_PACKET_t *pRespPkt = (EVT_RESP_PACKET_t *) vds_tx_buf;
		switch(pktFrame[12] )	 // OP_CODE
		{
		     case OP_DATA_RES:
		     {
//		     	fprintf(stdout,"responce opcode = %d\n",	pktFrame[12] );
		     }
		}
//		fprintf(stdout,"opcode = %x\n",	pktFrame[12] );
	     	break;
	     }	     
	     case OP_reREQUEST:	//0xA3(163)
	     {
		switch(pktFrame[12] )	 // OP_CODE
		{
		     case OP_HIST_REQ: //0xB2
		     {
		     	op_hist_req(pktFrame,OP_reRESPONCE);
			
		     	break;
		     }				
		     case OP_DET_REQ: //0xB8
		     {
		     	op_det_req(pktFrame,OP_reRESPONCE);
			
		     	break;
		     }
		     case OP_ECHO_REQ: //0xBA
		     {
		     	op_echo_req(pktFrame,OP_reRESPONCE);

		     	break;
		     }
		     case OP_STAT_REQ: //0xBC
		     {
		     	op_stat_req(pktFrame,OP_reRESPONCE);
			
		     	break;
		     }
		     case OP_TIME_REQ: //0xBE
		     {
		     	op_time_req(pktFrame,OP_reRESPONCE);

		     	break;
		     }	
		     default: break;	     
		}   
	     }	     
	     case OP_reRESPONCE:	//0xA4(164)
	     {

//		fprintf(stdout,"opcode = %d\n",	pktFrame[12] );
		break;
	     }
	     default : 
	            break;
	}
	///////////////////////////////////////////////////////////////
	fflush(stdout);

    // OP_REQ_SYS_INIT Process
	if(sys_init_flag == 1){
		sys_init_flag = 0;
		
		close_keyboard();

		save_log(SERVER_COMMAND_SYSINIT,0);  // kwj insert. 2017.07.19

		exit(1);				
	}
	
}

//----------------------------------------------------------------------------------
//2014.06.23 by capi
//----------------------------------------------------------------------------------
void SendInversData(int retrans)
{	

	uint32 pktLen = 0;		//by capi 2013.01.24
	uint32 tlen, i, t_index;

	if(vds_st.state != SM_VDS_CLIENT_DEVID_SEND) return;
	
	VDS_TCP_INVDATA_PACKET_t *pSenddata = (VDS_TCP_INVDATA_PACKET_t *) vds_tx_buf;

	//uip_buf, uip_len	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////
	#if defined(TTMS_VDS_TCP_PROTOCOL)
	// Hyphen
	pSenddata->hyphen_1 = _HYPHEN_CH_;
	pSenddata->hyphen_2 = _HYPHEN_CH_;
	#else
	// SOH
	pSenddata->soh_ch[0] = _SOH_CH_;
	pSenddata->soh_ch[1] = _SOH_CH_;
	#endif

	// Sender IP
	for (i=0; i<IP_STR_LEN; i++) pSenddata->sip[i] = ch_dest_ip[i];
	// Destination IP
	for (i=0; i<IP_STR_LEN; i++) pSenddata->dip[i] = ch_src_ip[i];

	// Controller Kind
	pSenddata->kind[0] = _VDS_KIND_CH_0_;
	pSenddata->kind[1] = _VDS_KIND_CH_1_;
	
	for (i=0; i<STATION_NUM_LEN; i++) pSenddata->snum[i] = gstNetConfig.station_num[i] ;

	tlen = gucActiveLoopNum + 1/*OP CODE*/ + 1 + 4;

	// Total Length	
	pSenddata->tlen[0] = 0 ;
	pSenddata->tlen[1] = 0;
	pSenddata->tlen[2] = (tlen & 0xFF00) >> 8;
	pSenddata->tlen[3] = tlen & 0x00FF;	
	
	// Opcode
	pSenddata->opcode = OP_INVERS_VDS_DATA;	
	pSenddata->loop_num = gucActiveLoopNum;

	t_index = 0;

	for(i=0;i<gucActiveLoopNum;i++) pSenddata->data[i] = 0;

	for(i=0;i<gucActiveDualLoopNum/2;i++)
	{
		if(invers_st.gRT_Reverse[i]) invers_st.gRT_Reverse_retrans[i] = invers_st.gRT_Reverse[i];
		//invers_st.gRT_Reverse[i] = FALSE;
			
		if(invers_st.gRT_Reverse_retrans[i])
		{
			pSenddata->data[i*2] = 1;
			pSenddata->data[i*2+1] = 1;			
		}
	}

	t_index = gucActiveLoopNum;
	
	pSenddata->data[t_index++] = 0;		
	pSenddata->data[t_index++] = 0;		
	pSenddata->data[t_index++] = 0;		
	pSenddata->data[t_index++] = 0;		
 
	pktLen = (sizeof(*pSenddata)-1) + t_index;
	//////////////////////////////////////////////////////////////////////////////////////////////////

	vds_tx_len = pktLen;
			
	vds_st.send_left = pktLen;
	vds_st.send_ptr = 0;	
	
	if( vds_tx_len ==0) return;

	///////////////////////////////////////////////////////////////
	/// Data send
	int flag_send=0;
	
    if ( send(fd_listener, vds_tx_buf, vds_tx_len, flag_send) == -1) {
            pr_err("[TCP client] : Fail: send()");
    }

	fprintf(stdout,"Send RevData length : %d bytes\n", pktLen);
		
	///////////////////////////////////////////////////////////////	
}

//통신 유효성 체크 2011.06.28 capidra
void checkvalidTask()
{	
	uint32 pktLen = 0;		//by capi 2013.01.24
	uint32 tlen, i;
	
	VDS_VALID_SESSION_PACKET_t *pSendHead = (VDS_VALID_SESSION_PACKET_t *) vds_tx_buf;

	//uip_buf, uip_len	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////
	#if defined(TTMS_VDS_TCP_PROTOCOL)
	// Hyphen
	pSendHead->hyphen_1 = _HYPHEN_CH_;
	pSendHead->hyphen_2 = _HYPHEN_CH_;
	#else
	// SOH
	pSendHead->soh_ch[0] = _SOH_CH_;
	pSendHead->soh_ch[1] = _SOH_CH_;
	#endif

	// Sender IP
	for (i=0; i<IP_STR_LEN; i++) pSendHead->sip[i] = ch_dest_ip[i];
	// Destination IP
	for (i=0; i<IP_STR_LEN; i++) pSendHead->dip[i] = ch_src_ip[i];

	// Controller Kind
	pSendHead->kind[0] = _VDS_KIND_CH_0_;
	pSendHead->kind[1] = _VDS_KIND_CH_1_;
	
	for (i=0; i<STATION_NUM_LEN; i++) pSendHead->snum[i] = gstNetConfig.station_num[i] ;

	tlen = 10;		

	// Total Length	
	pSendHead->tlen[0] = 0 ;
	pSendHead->tlen[1] = 0;
	pSendHead->tlen[2] = (tlen & 0xFF00) >> 8;
	pSendHead->tlen[3] = tlen & 0x00FF;	
	
	// Opcode
	pSendHead->opcode = OP_VAILD_SESSION;	

	pktLen = sizeof(*pSendHead);

	//////////////////////////////////////////////////////////////////////////////////////////////////

	vds_tx_len = pktLen;
			
	vds_st.send_left = pktLen;
	vds_st.send_ptr = 0;	
	
	if( vds_tx_len ==0) return;

	///////////////////////////////////////////////////////////////
	/// Data send
	int flag_send=0;
	
    if ( send(fd_listener, vds_tx_buf, vds_tx_len, flag_send) == -1) {
            pr_err("[TCP client] : Fail: send()");
    }

	fprintf(stdout,"Send RevData length : %d bytes\n", pktLen);
		
	///////////////////////////////////////////////////////////////	
}

int communication_task()
{
	int modemctlline;
	unsigned char buff;
	unsigned char buff1;
	unsigned char buff6;
	unsigned char LDBbuf[10];


	/* waiting for data (no timeout) */
	if ((ret_poll = epoll_wait(epollfd, ep_events, max_ep_events, 0)) == -1) {
		/* error */
	}	

	if( ret_poll < 1) return;		

	/* Are there data to read? */
	for (i=0; i<ret_poll; i++) {
		if (ep_events[i].events & EPOLLIN) {
			printf("avogadro ...EPOLLIN \n");
			/* IS New connection ? */
			if (ep_events[i].data.fd == fd_listener) {
				// server mode 
				/*struct sockaddr_storage saddr_c;
				while(1) {
					len_saddr = sizeof(saddr_c);
					fd = accept(fd_listener, (struct sockaddr *)&saddr_c, &len_saddr);
					if (fd == -1) {
						if (errno == EAGAIN) { 
							break;
						}
						pr_err("Error get connection from listen socket");
						break;
					}
					fcntl_setnb(fd);
					ADD_EV(epollfd, fd);
					pr_out("accept : add socket (%d)", fd);
				}
				continue;
				*/
			}
			//---- IO BOARD serial device -------------------    
			if( ep_events[i].data.fd == fd_tty){

 				modemctlline = TIOCM_RTS;
				ioctl( fd_tty, TIOCMBIC, &modemctlline );

				ret_recv = read(fd_tty, &buff, 1);

 				modemctlline = TIOCM_RTS;
				ioctl( fd_tty, TIOCMBIS, &modemctlline );

    				if (ret_recv != 0) {
					io_comtask(buff);
        			
        				//__dump_message(buff, ret_recv);
    				}else 
					fprintf(stdout,"Io error\n");
			}
                        //---- Ext serial device -------------------
	    		else if( ep_events[i].data.fd == fd_ttys1){
	    			//by capi 2014.08
		            	//modemctlline = TIOCM_RTS;
		            	//ioctl( fd_ttys1, TIOCMBIC, &modemctlline );
		            
		            	//ret_recv = read(fd_ttys1, blockbuff, sizeof(blockbuff));
		            	ret_recv = read(fd_ttys1, &buff1, 1);
		            	//fprintf(stdout,"Received. ttys1 %02X\n", buff);

		            	//modemctlline = TIOCM_RTS;
		            	//ioctl( fd_ttys1, TIOCMBIS,minicom &modemctlline );
	            
		           	if (ret_recv > 0) {
		           			mainteCommTask(&buff1);  
		            	}else
		                    fprintf(stdout,"Ext serial error\n");
	    		}
	    		//---- LD Board device 485-----------------------
	    		else if( ep_events[i].data.fd == fd_ttys6){
	    			
	            		ret_recv = read(fd_ttys6, LDBbuf, 13); 

				//usleep(3000);
	            
	           		if (ret_recv > 0) {
	           			ldb_comtask(LDBbuf, ret_recv);                   		
			#if 0		
	              		fprintf(stdout,"\n\rReceived. ttys6 [%03d] Byte\n",ret_recv);
	              			for(i=0;i<ret_recv;i++)
	             				fprintf(stdout,"%02X | ", LDBbuf[i]);

	               			fprintf(stdout,"\n");
			#endif
	             			//__dump_message(buff, ret_recv);
		            	}else
		                   fprintf(stdout,"LDB error\n");
	    		}
			//---- ethernet device -----------------------
			else if (ep_events[i].data.fd == fd_listener) 
			{
				printf("avogadro ...fd_listener \n");
				ret_recv = recv(ep_events[i].data.fd, buf, sizeof(buf), 0);
				if (ret_recv == -1) {
					/* error */
              				if(term_stat !=REALTIME_MONITOR_MODE ){
						pr_out("fd(%d) : server process closed error ", ep_events[i].data.fd);
					}

					DEL_EV(epollfd, ep_events[i].data.fd);
					//network_alive = FALSE;
					vds_st.state = SM_VDS_CLIENT_CLOSEED; //by capi 2014.09.04
					save_log(NET_COLSE_SERVER , 0);
				}
				else if (ret_recv == 0) {
					/* closed */
              				if(term_stat !=REALTIME_MONITOR_MODE )
						pr_out("fd(%d) : Session closed", ep_events[i].data.fd);
					DEL_EV(epollfd, ep_events[i].data.fd);
					//network_alive = FALSE;
					vds_st.state = SM_VDS_CLIENT_CLOSEED; //by capi 2014.09.04

					save_log(NET_COLSE_SESSION , 0);
				} 
				else {
					/* normal */
					//pr_out("recv(fd=%d,n=%d) :  ", ep_events[i].data.fd, ret_recv);
        				//__dump_message(buf, ret_recv);

					//fprintf(stdout," Rev Packet\n");

					/////////////// interpret TCP Protocol////////////////////
					interprete_VDS_TCP_pkt(ep_events[i].data.fd, (u8_t *)buf, ret_recv);
					////////////////////////////////////////////////////////////////////
				}
			}
#ifdef ENABLE_MSG_OOB
		} 
		else if (ep_events[i].events & EPOLLPRI) {
			pr_out("EPOLLPRI : Urgent data detected");

			if ((ret_recv = recv(ep_events[i].data.fd, buf, 1, MSG_OOB)) == -1) {
					/* error */
			}
				pr_out("recv(fd=%d,n=1) = %.*s (OOB)", ep_events[i].data.fd, 1, buf);
#endif
			} else if (ep_events[i].events & EPOLLERR) {
				fprintf(stdout,"EPOLLERR\n");
				/* error */
			} else {
				pr_out("fd(%d) epoll event(%d) err(%s)", 
						ep_events[i].data.fd, ep_events[i].events, strerror(errno));
			}
		}

	return 0;
}

init_epoll()
{
	vds_st.state = SM_VDS_CLIENT_CLOSEED;		//by capi 2014.09.04

        if ((epollfd = epoll_create(1)) == -1) {
			save_log( POLL_CREATE_EXIT, 0); // kwj insert. 2017.07.19                

            /* error */
            exit(EXIT_FAILURE);
        }
        if ((ep_events = calloc(max_ep_events, sizeof(struct epoll_event))) == NULL) {

			save_log( POLL_CALLOC_EXIT, 0); // kwj insert. 2017.07.19
            /* error */
            exit(EXIT_FAILURE);
        }

        //fcntl_setnb(fd_tty); /* set nonblock flag */
        ADD_EV(epollfd, fd_tty);

        ADD_EV(epollfd, fd_ttys1);

        ADD_EV(epollfd, fd_ttys6); //insert by capi 2014.10.07
#if 0
	if( tcpip_connected == 1)
	{
	//	fprintf(stdout,"add tcpip  epoll\n");
        	ADD_EV(epollfd, fd_listener);
	} 
#endif
}

add_epoll_socket()
{
	if( tcpip_connected == 1)
	{	
        	ADD_EV(epollfd, fd_listener);
	} 
}

del_epoll_socket()
{
	DEL_EV(epollfd, fd_listener);
}

int init_tcpip()
{
	int ret;
	char serverIP[16], s_port[8];

	//if(term_stat !=REALTIME_MONITOR_MODE) fprintf(stdout,"\n\rinit_tcpip\n\r");

	port = strdup(portnum);; //

	struct addrinfo ai, *ai_ret;
	int     rc_gai;
	memset(&ai, 0, sizeof(ai));
	ai.ai_family = AF_INET;
	ai.ai_socktype = SOCK_STREAM;
	ai.ai_flags = AI_ADDRCONFIG | AI_PASSIVE;

	//if ((rc_gai = getaddrinfo(NULL, port, &ai, &ai_ret)) != 0) {
	memset(serverIP, 0, 16);
	sprintf(serverIP, "%d.%d.%d.%d", 
					gstNetConfig.server_ip[0], 
					gstNetConfig.server_ip[1], 
					gstNetConfig.server_ip[2], 
					gstNetConfig.server_ip[3]);

	memset(s_port, 0, 8);
	sprintf(s_port, "%d",gstNetConfig.server_port);

	if ((rc_gai = getaddrinfo(serverIP, s_port, &ai, &ai_ret)) != 0) {
		pr_err("Fail: getaddrinfo():%s", gai_strerror(rc_gai));
		exit(EXIT_FAILURE);
	}

       	if(term_stat !=REALTIME_MONITOR_MODE )
		fprintf(stdout,"IP = %s, Port = %s \n", serverIP, s_port);

	if ((fd_listener = socket(
					ai_ret->ai_family,
					ai_ret->ai_socktype,
					ai_ret->ai_protocol)) == -1) {
		pr_err("Fail: socket()");
		exit(EXIT_FAILURE);
	}

	fcntl_setnb(fd_listener); /* set nonblock flag */

        (void)connect(fd_listener, ai_ret->ai_addr, ai_ret->ai_addrlen);
#if 1
	// non blocking process
        if(errno != EINPROGRESS){
        	vds_st.state = SM_VDS_CLIENT_ABORTED;
        	fprintf(stdout,"socket not connected\n");
		tcpip_connected = -1;
        }
	else {
       		if(term_stat !=REALTIME_MONITOR_MODE )
        		fprintf(stdout,"socket connected\n");
        	vds_st.state = SM_VDS_CLIENT_CONNECTED;  //by capi 2014.09.04
		tcpip_connected = 1;
#ifdef SEGMENT		
		i2cSegDigit(1,1);
		i2cSegDigit(1,0);
#endif		
	}
#else 
	// check server connect 
	int sockopt;
	socklen_t len_sockopt = sizeof(sockopt);
	if( getsockopt(fd, SOL_SOCKET, SO_ERROR, &sockopt, &len_sockopt) == -1)
	{
       		if(term_stat !=REALTIME_MONITOR_MODE )
        		fprintf(stdout,"tcpip not connected\n");
		tcpip_connected = -1;
	}
	else {
       		if(term_stat !=REALTIME_MONITOR_MODE )
        		fprintf(stdout,"tcpip connect\n");
		tcpip_connected = 1;
	}
#endif

//	init_epoll();
#if 0

	if ((epollfd = epoll_create(1)) == -1) {
		/* error */
		exit(EXIT_FAILURE);
	}
	if ((ep_events = calloc(max_ep_events, sizeof(struct epoll_event))) == NULL) {
		/* error */
		exit(EXIT_FAILURE);
	}


	//fcntl_setnb(fd_tty); /* set nonblock flag */
	ADD_EV(epollfd, fd_tty);

	ADD_EV(epollfd, fd_ttys1);

	ADD_EV(epollfd, fd_listener);
#endif
	return 0;
}

/* Name : add_ev
 * Desc : add socket descriptor to epoll
 * Argv : int efd, int fd
 * Ret  : zero(if success), negative(if fail)
 */
int add_ev(int efd, int fd)
{
	struct epoll_event ev;

	/* if want to use edge trigger, add EPOLLET to events */
#ifdef ENABLE_MSG_OOB	
	ev.events = EPOLLIN | EPOLLPRI;
#else
	ev.events = EPOLLIN | EPOLLERR;
#endif
	ev.data.fd = fd;
	if (epoll_ctl(efd, EPOLL_CTL_ADD, fd, &ev) == -1) {
		pr_out("fd(%d) EPOLL_CTL_ADD  Error(%d:%s)", fd, errno, strerror(errno));
		return -1;
	}

	return 0;
}

/* Name : del_ev
 * Desc : delete socket descriptor from epoll
 * Argv : int efd, int fd
 * Ret  : zero(if success), negative(if fail)
 */
int del_ev(int efd, int fd)
{
	if (epoll_ctl(efd, EPOLL_CTL_DEL, fd, NULL) == -1) {
		pr_out("fd(%d) EPOLL_CTL_DEL Error(%d:%s)", fd, errno, strerror(errno));
		return -1;
	}
	close(fd);
	return 0;
}

/* Name : fcntl_setnb
 * Desc : set non-blocking mode
 * Argv : int fd
 * Ret  : zero(if success), negative(if fail)
 */
int fcntl_setnb(int fd)
{
	/* only influence about O_ASYNC, O_APPEND, O_NONBLOCK on Linux-specific */
	if (fcntl(fd, F_SETFL, O_NONBLOCK | fcntl(fd, F_GETFL)) == -1) {	
		return errno;
	}
	return 0;
}

void autoSyncTask()
{
	int main_sinc;
	
	main_sinc = 0;
	// Center 쪽 Auto-Sync 임.
	if ( centerAutosyncCounter >= (gstSysConfig.param_cfg.polling_cycle \
		+ gstSysConfig.param_cfg.auto_resync_wait ) * 1000 )
	{
		//by capi
		//if (term_stat == REALTIME_MONITOR_MODE)
		//	fprintf(stdout,"\nDATA AUTO-RESYNCHRONIZATION\n");

		// 서버로 부터 Sync 패킷이 오지 않으면 
		// Frame number를 auto 로 하나 증가한다.
		//frame_num++;
		
		//switchVdsBuffBank();
		//switchVdsBuffBank_tcp();
		//prepareCenterPollData();

		// Set system status auto_resync flag.
		system_status.auto_resync = _SYS_INVALID_;
		main_sinc = 1;
	}

//by capi

	if (localAutosyncCounter >= (gstSysConfig.param_cfg.polling_cycle \
		+ gstSysConfig.param_cfg.auto_resync_wait ) * 1000 )	// local polling time
	{
		/*if (term_stat == REALTIME_MONITOR_MODE)
			fprintf(stdout,"\n\rDATA AUTO-RESYNCHRONIZATION");*/
		//if (term_stat == REALTIME_MONITOR_MODE)
		//	fprintf(stdout,"\n\rDATA AUTO-RESYNCHRONIZATION");
		/*
		fprintf(stdout,"%d %d %d\n\r",localAutosyncCounter
						, gstSysConfig.param_cfg.polling_cycle
						, gstSysConfig.param_cfg.auto_resync_wait);
		*/

		if(term_stat !=REALTIME_MONITOR_MODE )
			fprintf(stdout,"\n\rDATA AUTO-RESYNCHRONIZATION \n");

		frame_num++;

		switchVdsBuffBank();
		//switchVdsBuffBank_tcp();
		prepareLocalPollData();
	}
}

void prepareIncidentPollData()
{
	int i;
	u_int polling_count;
	uint32 t_occupy;

	polling_count = incident_counter /* * _TICK_MS_*/;  //by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
	
	// Reset counter for polling.	
	incident_counter = 0;	

	if (!polling_count) return;	
	
	for(i=0; i<gucActiveDualLoopNum; i++)
	{
		if (loop_st[i].status == PRESSED)
		{
			t_occupy = io_time_counter - loop_st[i].time[PRESS_TIME];
			
			if (t_occupy >= polling_count) gstIncidentAccuData.occupy[i] = polling_count;  // 100 %
			else gstIncidentAccuData.occupy[i] += t_occupy;
		}
	}

	for(i=0; i<gucActiveSingleLoopNum; i++)
	{
		if (loop_st[gucActiveDualLoopNum+i*2].status == PRESSED)
		{
			t_occupy = io_time_counter - loop_st[gucActiveDualLoopNum+i*2].time[PRESS_TIME];
			
			if (t_occupy >= polling_count) gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2] = polling_count;  // 100 %
			else gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2] += t_occupy;

			gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2+1] = gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2];
		}
	}
	for (i=0; i<MAX_LANE_NUM; i++)
	{		
		if (gstLaneLink[i].e_loop < MAX_LOOP_NUM && gstIncidentAccuData.volume[gstLaneLink[i].e_loop])	
		{			
			gstIncidentPolling.speed[i] = gstIncidentAccuData.speed[i] / gstIncidentAccuData.volume[gstLaneLink[i].e_loop];
			gstIncidentPolling.length[i] = gstIncidentAccuData.length[i] /gstIncidentAccuData.volume[gstLaneLink[i].e_loop];
		}
		else
		{			
			gstIncidentPolling.speed[i] = 0;
			gstIncidentPolling.length[i] = 0;
		}
	}	

	fprintf(stdout, "%d, %d \n", gstIncidentAccuData.occupy[0], t_occupy);
	
	for (i=0; i<MAX_LOOP_NUM; i++)
	{
		gstIncidentPolling.volume[i] = gstIncidentAccuData.volume[i];

		#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
		
		t_occupy = 1000 * 100 * gstIncidentAccuData.occupy[i];
		t_occupy /= polling_count;
		if (t_occupy > 100*1000) t_occupy = 100*1000;
		
		gstIncidentPolling.occupy[i] = t_occupy;
		
		//fprintf(stdout, "%d loop  %d",i ,gstIncidentPolling.occupy[i]);
		#else		
		
		#if 1 // bug fix ?? pppp
		
		t_occupy = 10 * 100 * gstIncidentAccuData.occupy[i];
		t_occupy /= polling_count;
		
		t_occupy += 5;
		t_occupy /= 10;
		
		if (t_occupy > 100) t_occupy = 100;
		#else
		t_occupy = 10 * 100 * gstCenterAccuData.occupy[i] / polling_count;
		t_occupy = t_occupy / 5;
		t_occupy = (t_occupy * 5)/10;
		if (t_occupy > 100) t_occupy = 100;
		#endif
		
		gstIncidentPolling.occupy[i] = t_occupy;
		
		#endif
	}

}

void checkIncidentTask()
{
	int i,j, k;
	long cal/*, keep_tm*/;	
	//uint8 inc;
	
	if ( incident_counter >= gstSysConfig.param_cfg.incid_exec_cycle*1000 )
	{

		//gstIncidentAccuData.speed[t_lane] = 0;
			//			gstIncidentAccuData.length[t_lane] = 0;
		//prepareCenterPollData();

		// Reset counter for checking incident.
		for (i=0; i<(MAX_LOOP_NUM/8); i++) incident_per_loop[i] = 0;
		
		prepareIncidentPollData();

		fprintf(stdout, "%d  %d  ",gstSysConfig.param_cfg.K_factor[0], gstSysConfig.param_cfg.T_value[0]);
		
		//fprintf(stdout, "\n");
		for(i=0;i<gucActiveLoopNum;i++)
		{
			cal = gstIncidentPolling.occupy[i]*gstSysConfig.param_cfg.K_factor[0]/10;			
				
			if(cal>=gstSysConfig.param_cfg.T_value[i] )
			{
				//fprintf(stdout, "[%d] loop  %d  ",i ,cal);
				
				keepup_incident[i]++;
				//keep_tm = keepup_incident[i]*5;
				if(keepup_incident[i]>gstSysConfig.param_cfg.persist_period)
				{
					k=(int)(i/8);
					j = (int)(i%8);
					incident_per_loop[k] |= 0x01<<j;
#ifdef SEGMENT					
					i2cSegDigit(1,5);
					i2cSegDigit(0,k%8);	//current only digit
#endif					
				}
			}
			else
			{
				keepup_incident[i]= 0;
			}
		}
		incident_counter = 0;
		
		for (i=0; i<MAX_LANE_NUM/8; i++) 
		{
			gstIncidentAccuData.speed[i] = 0;
			gstIncidentAccuData.length[i] = 0;
			
		}	
		for (i=0; i<MAX_LOOP_NUM; i++)
		{
			gstIncidentAccuData.volume[i] = 0;
			gstIncidentAccuData.occupy[i] = 0;
		}	
	}
}

u_int getSystemTick()
{
	return (global_counter);  //by capi 2014.09.02
}

void savestucktime()
{
	int i;
	u_int  curtime, passtime;
	
	curtime = getSystemTick();

	//2011.07.06. by capidra
	if(pre_stuck_count<curtime) passtime = curtime - pre_stuck_count;
	else passtime = 3600000 + curtime - pre_stuck_count;

	pre_stuck_count = curtime;
	
	for(i=0;i<MAX_LOOP_NUM;i++)
	{		
		stuckon_counter[i] = stuckon_counter[i]+passtime;
		stuckoff_counter[i] = stuckoff_counter[i]+passtime;
		
	}	
}