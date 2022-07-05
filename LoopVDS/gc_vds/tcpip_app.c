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

#include <locale.h>
#include <time.h>
#include <sys/time.h>



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
int	i;
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

int sync_time_flag = 0;

//extern network_alive;
uint8 save_data30s(VDS_TCP_PACKET_HEADER_t *pRecvHead,VDS_DATA_RESP_PACKET_t *pRespPkt, u16_t len);
static void __dump_message(uint8_t *buf, size_t sz)
{
    fprintf(stdout,"<");
    while (sz --)
    {
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

    //system_status.long_pwr_fail = _SYS_VALID_;			//Delete by capi 2012.04.03
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
    //system_status.fan_operate = (cur_non_mask_val & FAN_OP_PT_MSK) ? _OPERATE_ : _NOT_OPERATE_;
#if 0
    if( system_status.fan_operate == 1)
        fprintf(stdout,"fan on\n");
    else
        fprintf(stdout,"fan off\n");
#endif
    //system_status.heater_operate = (cur_non_mask_val & HEATER_OP_PT_MSK) ? _OPERATE_ : _NOT_OPERATE_;
#if 0
    if( system_status.heater_operate == 1)
        fprintf(stdout,"heater on\n");
    else
        fprintf(stdout,"heater off\n");
#endif
#endif
}
//by capidra
uint16 getSysStatusData()
{
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

    system_status.long_pwr_fail = _SYS_VALID_;
    system_status.short_pwr_fail = _SYS_VALID_;

#ifdef DEBUG_SYS_COMM
    fprintf(stdout," System Status : 0x%04X", sys_status_data);
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

    if (idx == 1)
        return (uint8)(sys_status_data >> 8);		//버그 2011.07.06.by capidra
    else
        return (uint8)(sys_status_data);
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

    if (idx == 0)
        return (uint8)(sys_valid_data >> 8);
    else
        return (uint8)(sys_valid_data);
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
    for (i=0; i<MAX_LOOP_NUM/4; i++)
        gucStuckPerLoop[i] = 0;

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

    if (!polling_count)
        return;

    // 2011.06.21 by capidra
    // Syntax to produce the correct occupancy
    for(i=0; i<gucActiveDualLoopNum; i++)
    {
        if (loop_st[i].status == PRESSED)
        {
            //It adds the current occupancy
            t_occupy = io_time_counter - loop_st[i].time[PRESS_TIME];

            if (t_occupy >= polling_count)
                gstCenterAccuData.occupy[i] = polling_count;  // 100 %
            else
                gstCenterAccuData.occupy[i] += t_occupy;
        }
    }

    //ramp
    for(i=0; i<gucActiveSingleLoopNum; i++)
    {
        if (loop_st[gucActiveDualLoopNum+i*2].status == PRESSED)
        {
            // It adds the current occupancy
            t_occupy = io_time_counter - loop_st[gucActiveDualLoopNum+i*2].time[PRESS_TIME];

            if (t_occupy >= polling_count)
                gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2] = polling_count;  // 100 %
            else
                gstCenterAccuData.occupy[gucActiveDualLoopNum+i*2] += t_occupy;

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
    for (i=0; i<(MAX_LOOP_NUM/8); i++)
        incident_per_loop[i] = 0;

    //If occtime to be occupied for more than 30 seconds, gstCenterPolling.occupy of 100
    //If the vehicle is stationary for more than 30 seconds, a volumn of 100
    for (i=0; i<MAX_LOOP_NUM; i++)
    {
        gstCenterPolling.volume[i] = gstCenterAccuData.volume[i];

#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
        // calculation three Decimal places
        t_occupy = 1000 * 100 * gstCenterAccuData.occupy[i];
        t_occupy /= polling_count;
        if (t_occupy > 100*1000)
            t_occupy = 100*1000;

        gstCenterPolling.occupy[i] = t_occupy;

#else

#if 1 // bug fix ?? pppp

        // // calculation one Decimal places
        t_occupy = 10 * 100 * gstCenterAccuData.occupy[i];
        t_occupy /= polling_count;

        t_occupy += 5;
        t_occupy /= 10;

        // more than 100 per -> 100
        if (t_occupy > 100)
            t_occupy = 100;
#else
        t_occupy = 10 * 100 * gstCenterAccuData.occupy[i] / polling_count;
        t_occupy = t_occupy / 5;
        t_occupy = (t_occupy * 5)/10;
        if (t_occupy > 100)
            t_occupy = 100;
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

        if((gstCenterPolling.occupy[i*2] > 15000)||(gstCenterPolling.occupy[i*2+1] > 15000))
            invers_st.occ_state_with_inv[i] = TRUE;
        else
            invers_st.occ_state_with_inv[i] = FALSE;
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

    if(one_tcp_num >= MAX_VDS_RT_DATA_BANK)
        rtpos = one_tcp_wd;
    else
        rtpos = 0;

    for (i=0, chunk_num=0; i<one_tcp_num; i++, chunk_num++, rtpos++)
    {
        if (i >= MAX_VDS_RT_DATA_BANK)
            break;

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
        if (i >= MAX_VDS_RT_DATA_BANK)
            break;

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

////////////////////////////////////////////
void interprete_VDS_TCP_pkt(int fd, u8_t *pktFrame, u16_t frameLen)
{
    int i, j;
    int sys_init_flag = 0;
    uint32 pktLen = 0;		//2013.01.24 by capi

    VDS_TCP_PACKET_HEADER_t *pRecvHead = (VDS_TCP_PACKET_HEADER_t *) pktFrame;
    VDS_TCP_PKT_RESP_HEAD_t *pSendHead = (VDS_TCP_PKT_RESP_HEAD_t *) vds_tx_buf;

#if !defined(USE_REAL_VDS_DATA)
    u8_t testPattern = 0x80;
#endif

    if( term_stat == POLLING_MONITOR_MODE)
    {
        if( pRecvHead->opcode == 0x01)
            fprintf(stdout,"\n==============================================================================================\n");
        fprintf(stdout,"sip = %s, dip = %s, kind=%C%C num =%d-%d, OP code = %d\n",
                pRecvHead->sip, pRecvHead->dip, pRecvHead->kind[0],
                pRecvHead->kind[1], pRecvHead->snum[1], pRecvHead->snum[3], pRecvHead->opcode);
        fprintf(stdout,"transaction num : ");
        for(i=0; i<8; i++)
            fprintf(stdout," %d ", pRecvHead->transnum[i]);
        //fprintf(stdout,"\n");
    }

    //Station Number Check			-> by capidra 2011.0608
    for(i=0; i<STATION_NUM_LEN; i++)
    {
        if(gstNetConfig.station_num[i] != pRecvHead->snum[i])
        {
            system_result =  TCP_CSN_ERROR;				//by capidra 2014.09.17;
            //fprintf(stdout, "\n%d  - %d \n\r", gstNetConfig.station_num[i] , pRecvHead->snum[i]);
        }
    }

    for (i=0; i<IP_STR_LEN; i++)
        ch_src_ip[i] = pRecvHead->sip[i];
    for (i=0; i<IP_STR_LEN; i++)
        ch_dest_ip[i] = pRecvHead->dip[i];

    // Set broadcast msg flag.
    if (pRecvHead->snum[0] == 0xFF && pRecvHead->snum[1] == 0xFF && pRecvHead->snum[2] == 0xFF && pRecvHead->snum[3] == 0xFF)
        system_status.recv_broadcast_msg = _SYS_INVALID_;
    else
        system_status.recv_broadcast_msg = _SYS_VALID_;

    dia_status.last_opc = pRecvHead->opcode;

    vds_st.state = SM_VDS_CLIENT_DEVID_SEND;	// KISS

	 
    switch(pRecvHead->opcode)
    {
    // Device ID
    case OP_REQ_CSN:
    {
		  write_debug_log("OP_REQ_CSN rpcoessing");
        CSN_RESP_PACKET_t *pRespPkt = (CSN_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        vds_st.state = SM_VDS_CLIENT_DEVID_SEND;	//insert by capi 2014.09.04

        save_log(CONNECT_SERVER, 0);

        validsession_counter = 0;
        validsession_flag = 0;
        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27

        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        for (i=0; i<4; i++)
            pRespPkt->CSN[i] = gstNetConfig.station_num[i];

        fprintf(stdout,"Send CSN : %d,%d,%d,%d\r\n", pRespPkt->CSN[0],pRespPkt->CSN[1],pRespPkt->CSN[2],pRespPkt->CSN[3]);
#ifdef SEGMENT
        i2cSegDigit(1,1);
        i2cSegDigit(0,1);
#endif
        //////////////////////////////////////////////////////////////////////

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;


		  // 2021.03.23 CSN 체크 시 Transaction Number내 timestamp 로 시간 동기화 진행 by avogadro
		  syncTimeToServer(pRecvHead);
		  sync_time_flag = 0;
		  
		  //2020.10.11  접속 후 인증된 단말에서만 센터 전송 데이터 초기화(네트워크 단절 기간동안의 데이터 버린다)  by avogadro
		  //rtDataClear();
		  resetCenterPollData();
		  // end 

    }
    break;

    case OP_REQ_SYNC:
    {
		 write_debug_log("OP_REQ_SYNC rpcoessing");
        SYNC_REQ_PACKET_t *pReqPkt = (SYNC_REQ_PACKET_t *) pktFrame;
        u8_t frame_no = pReqPkt->frame;

        //////////////////////////////////////////////////////////////////////

        // Set system status auto_resync flag.
        system_status.auto_resync = _SYS_VALID_;
        frame_num = frame_no;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        switchVdsBuffBank();
        prepareCenterPollData();		// eeee

        //fprintf(stdout,"[rev sync] 0x01\n");
#ifdef SEGMENT
        i2cSegDigit(1,0);
        i2cSegDigit(0,1);
#endif
    }
    break;

    case OP_REQ_VDS_DATA:
    {
		 write_debug_log("OP_REQ_VDS_DATA rpcoessing");

        VDS_DATA_RESP_PACKET_t *pRespPkt = (VDS_DATA_RESP_PACKET_t *) vds_tx_buf;
        u16_t tlen, t_index, t_occupy;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;
        host_req_counter = 0; //by capi 2014.09.18

        //fprintf(stdout,"[REV Data Req] 0x04\n");

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27

        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

#ifdef SEGMENT
        i2cSegDigit(1,0);
        i2cSegDigit(0,4);
#endif
        //////////////////////////////////////////////////////////////////////

        // Frame Number.
        pRespPkt->frame = frame_num;

        t_index = 0;

        //  Stuck of Loop.
        for (i=0; i<MAX_LOOP_NUM/4; i++)
            pRespPkt->data[t_index++] = gucStuckPerLoop[i];	//2011.06.17 by capidra

        // Incident Info.
        for (i=0; i<MAX_LOOP_NUM/8; i++)
            pRespPkt->data[t_index++] = incident_per_loop[i]; //0->incident_per_loop[i] by capidra 2012.02.02

        if (gucActiveLoopNum > MAX_LOOP_NUM)
        {
            fprintf(stdout," Please check number of loop !!");
            gucActiveLoopNum = MAX_LOOP_NUM;
        }

        pRespPkt->data[t_index++] = (u8_t) gucActiveLoopNum;	//insert by capidra 2011.06.17

        //  Info per loop.

        for (i=0; i<gucActiveLoopNum; i++)
        {
            pRespPkt->data[t_index++] = (u8_t) gstCenterPolling.volume[i];

#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
            t_occupy = round_for_occupy(gstCenterPolling.occupy[i], 2);
            pRespPkt->data[t_index++] = t_occupy / 100;
            pRespPkt->data[t_index++] = t_occupy % 100;
#else
            pRespPkt->data[t_index++] = gstCenterPolling.occupy[i];
            pRespPkt->data[t_index++] = 00;			 // eeee
#endif
        }

        pRespPkt->data[t_index++] = gucActiveLoopNum/2;		//2011.09.14 by capidra

        // Info per lane.
        //for(i=0; i<MAX_RIO_LANE_NUM; i++)
        for(i=0; i<gucActiveLoopNum/2; i++)
        {
            pRespPkt->data[t_index++] = (u8_t) gstCenterPolling.speed[i];

            //delete by capidra 2011.06.17
            pRespPkt->data[t_index++] = (u8_t) gstCenterPolling.length[i];
        }

		  //2020.10.11 30 초 데이터 저장 by avogadro
		  save_data30s(pRecvHead,pRespPkt, t_index);
		  //2020.10.11  

        pktLen = (sizeof(*pRespPkt) -1) + t_index;

        //retouch by capidra 2011.05.27

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_SPEED_DATA:
    {

		 write_debug_log("OP_REQ_SPEED_DATA rpcoessing");

        SPEED_DATA_REQ_PACKET_t *pReqPkt = (SPEED_DATA_REQ_PACKET_t *) pktFrame;
        SPEED_DATA_RESP_PACKET_t *pRespPkt = (SPEED_DATA_RESP_PACKET_t *) vds_tx_buf;
        u8_t respLane = pReqPkt->lane;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;
        host_req_counter = 0; //by capi 2014.09.18

        if(respLane > gucActiveLoopNum)
            system_result =  TCP_FIELD_VALUE_ERROR;				//by capidra 2014.09.17

        if(pRecvHead->tlen[3] != 10) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27

        clearSystemResultcode();						//by capi 2014.09.18
        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////

        pRespPkt->lane = respLane;

        if (respLane > 0)
            respLane--;

        // Speed
#if !defined(USE_REAL_VDS_DATA)
        for (i=0; i<SPEED_CATEGORY_NO; i++)
        {
            pRespPkt->speed[i][0] = testPattern++;
            pRespPkt->speed[i][1] = testPattern++;
        }
#else
        for (i=0; i<SPEED_CATEGORY_NO; i++)
        {
            pRespPkt->speed[i][0] = gstReportsOfLane[respLane].speed_category[i] >> 8;
            pRespPkt->speed[i][1] = (uint8) gstReportsOfLane[respLane].speed_category[i];
        }
#endif
        // Reset value.
        for (i=0; i<SPEED_CATEGORY_NO; i++)
            gstReportsOfLane[respLane].speed_category[i] = 0;

        //////////////////////////////////////////////////////////////////////

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        fprintf(stdout," head (%d bytes) [tot:%d]", sizeof(VDS_TCP_PKT_RESP_HEAD_t), pktLen);

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_REQ_LENGTH_DATA:
    {

		 write_debug_log("OP_REQ_LENGTH_DATA rpcoessing");
        LENS_DATA_REQ_PACKET_t *pReqPkt = (LENS_DATA_REQ_PACKET_t *) pktFrame;
        LENS_DATA_RESP_PACKET_t *pRespPkt = (LENS_DATA_RESP_PACKET_t *) vds_tx_buf;
        u8_t respLane = pReqPkt->lane;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(respLane > gucActiveLoopNum)
            system_result =  TCP_FIELD_VALUE_ERROR;				//by capidra 2014.09.17

        if(pRecvHead->tlen[3] != 10) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27

        clearSystemResultcode();						//by capi 2014.09.18
        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);


        pRespPkt->lane = respLane;	  // insert by capi 2012.04.12 // by jwank 091013

        if (respLane > 0)
            respLane--;

#if !defined(USE_REAL_VDS_DATA)
        for (i=0; i<LENGTH_CATEGORY_NO; i++)
        {
            pRespPkt->length[i][0] = testPattern++;
            pRespPkt->length[i][1] = testPattern++;
        }
#else
        for (i=0; i<LENGTH_CATEGORY_NO; i++)
        {
            pRespPkt->length[i][0] = gstReportsOfLane[respLane].length_category[i] >> 8;
            pRespPkt->length[i][1] = (uint8) gstReportsOfLane[respLane].length_category[i];
        }
#endif
        // Reset value.
        for (i=0; i<LENGTH_CATEGORY_NO; i++)
            gstReportsOfLane[respLane].length_category[i] = 0;

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_REQ_VOLUME_DATA:
    {
		 write_debug_log("OP_REQ_VOLUME_DATA rpcoessing");


        VOL_DATA_RESP_PACKET_t *pRespPkt = (VOL_DATA_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Volume
#if !defined(USE_REAL_VDS_DATA)
        for (i=0; i<MAX_LOOP_NUM; i++)
        {
            pRespPkt->volume[i][0] = testPattern++;
            pRespPkt->volume[i][1] = testPattern++;
        }
#else
        for (i=0; i<MAX_LOOP_NUM; i++)
        {
            pRespPkt->volume[i][0] = gstReportsOfLoop[i].volume >> 8;
            pRespPkt->volume[i][1] = (uint8) gstReportsOfLoop[i].volume;
        }
#endif

        // Reset value.
        for (i=0; i<MAX_LOOP_NUM; i++)
            gstReportsOfLoop[i].volume = 0;

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_SEL_ERR_THRESHOLD:
    {

		 write_debug_log("OP_SEL_ERR_THRESHOLD rpcoessing");

        SEL_ERR_THRESHOLD_REQ_PACKET_t *pReqPkt = (SEL_ERR_THRESHOLD_REQ_PACKET_t *) pktFrame;
        SEL_ERR_THRESHOLD_RESP_PACKET_t *pRespPkt = (SEL_ERR_THRESHOLD_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 10) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Detecter Error Threshold 변경.
        ChangeErrThreshold(pReqPkt->threshold);

        pRespPkt->threshold = pReqPkt->threshold;

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_REQ_VALIDATION:				//2011.06.21 by capidra
    {

		 write_debug_log("OP_REQ_VALIDATION rpcoessing");


        SYS_VALIDATION_RESP_PACKET_t *pRespPkt = (SYS_VALIDATION_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;
        u8_t ldb_num, tmp,bit;
        uint val1, val2;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Controller Validation 데이터.	2011.06.21 by capidra
        //gusDipSwitch = (*(volatile unsigned short *)(DIP_SWITCH_BASE)) & 0xF000;
        //tmp = gusDipSwitch>>12;

        gpio_read(power1_fd, &val1);

        gpio_read(power2_fd, &val2);

        pRespPkt->pwr_device = POWER_NUM;

        pRespPkt->pwr_valid = 0;

        if(val1)
            pRespPkt->pwr_valid = pRespPkt->pwr_valid | 0x01;
        if(val2)
            pRespPkt->pwr_valid = pRespPkt->pwr_valid | 0x02;

        //fprintf(stdout, "power1 -2 : %d - %d , %X\r\n", val1, val2, pRespPkt->pwr_valid);

        //ldb_num = (gucActiveLoopNum+3)/4;  //2014.10.13 by capi

        pRespPkt->LD_device = gucActiveLDBNum;

        bit = tmp = 0;

        pRespPkt->LD_valid[0] = 0;
        pRespPkt->LD_valid[1] = 0;

        //2014.10.13 by capi
        for(i=0; i<ldb_num; i++)
        {
            if(revLDB_ID[i])
                tmp = 0;
            else
                tmp = 1;

            bit = tmp<<(i%8);
            pRespPkt->LD_valid[gucActiveLDBNum/8] |= bit;				//check LD Board
        }

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_SYS_RESET:
    {

		 write_debug_log("OP_REQ_SYS_RESET rpcoessing");


        SYS_RESET_RESP_PACKET_t *pRespPkt = (SYS_RESET_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        // Status
        system_status.controller_reset = _SYS_INVALID_;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        isReqForceReset = 0x89;

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_SYS_INIT:
    {

		 write_debug_log("OP_REQ_SYS_INIT rpcoessing");


        SYS_INIT_RESP_PACKET_t *pRespPkt = (SYS_INIT_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        //fprintf(stdout, "OP_REQ_SYS_INIT\r\n");

        // Status
        //system_status.controller_reset = _SYS_INVALID_;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27\
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //defaultVdsParameter();
        //gstSysConfig.is_default_param = _DEFAULT_PARAM_;
        //writeParamToNAND(TRUE);

        //isReqForceReset = 0x89;

        //close_keyboard();
        //exit(1);

        sys_init_flag = 1;

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_DOWNLOAD_PARAM:
    {

		 write_debug_log("OP_DOWNLOAD_PARAM rpcoessing");


        DOWNLOAD_PARAM_REQ_PACKET_t *pReqPkt = (DOWNLOAD_PARAM_REQ_PACKET_t *) pktFrame;
        DOWNLOAD_PARAM_RESP_PACKET_t *pRespPkt = (DOWNLOAD_PARAM_RESP_PACKET_t *) vds_tx_buf;
        u16_t tlen, param_idx;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        param_idx = pReqPkt->index;

        //2011.06.11 by capidra
#if !defined(USE_REAL_VDS_DATA)
#else
        if(param_idx == IDX_01_LOOP_ENABLE)
        {
            if(pReqPkt->param_base != 0)
            {
                putVdsParamLoopenable(param_idx, (uint8 *)&pReqPkt->param_base, TRUE);
            }
            else
            {
                fprintf(stdout, "[Parameter Error] LoopMask = %x\r\n", pReqPkt->param_base);
            }
        }

        else
            putVdsParameter(param_idx, (uint8 *)&pReqPkt->param_base, TRUE);
#endif

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_UPLOAD_PARAM:
    {

		 write_debug_log("OP_UPLOAD_PARAM rpcoessing");


        UPLOAD_PARAM_REQ_PACKET_t *pReqPkt = (UPLOAD_PARAM_REQ_PACKET_t *) pktFrame;
        UPLOAD_PARAM_RESP_PACKET_t *pRespPkt = (UPLOAD_PARAM_RESP_PACKET_t *) vds_tx_buf;
        u16_t tlen, paramSize = 0;
        u8_t param_idx = pReqPkt->index;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 10) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Parameter Index
        pRespPkt->index = param_idx;

        paramSize = getVdsParameter(param_idx, (uint8 *)&pRespPkt->param_base);	// wwww _packed 문제.

        pktLen = (sizeof(*pRespPkt) -1) + paramSize;	// param_base

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_ONLINE_STS:
    {

		 write_debug_log("OP_REQ_ONLINE_STS rpcoessing");

        ONLINE_STS_RESP_PACKET_t *pRespPkt = (ONLINE_STS_RESP_PACKET_t *) vds_tx_buf;
        u_int host_link_passed_time;
        uint32 tlen;

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////

        // Passed Time
#if !defined(USE_REAL_VDS_DATA)
        pRespPkt->passed_time[0] = testPattern++;
        pRespPkt->passed_time[1] = testPattern++;
        pRespPkt->passed_time[2] = testPattern++;
        pRespPkt->passed_time[3] = testPattern++;

#else

        host_link_passed_time = host_req_counter / _1_SEC_TICK_;

        pRespPkt->passed_time[0] = (u8_t) ( host_link_passed_time>> 24);
        pRespPkt->passed_time[1] = (u8_t) ( host_link_passed_time>> 16);
        pRespPkt->passed_time[2] = (u8_t) ( host_link_passed_time>> 8);
        pRespPkt->passed_time[3] = (u8_t) ( host_link_passed_time>> 0);
#endif

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_MEMORY_CHECK:
    {

		 write_debug_log("OP_REQ_MEMORY_CHECK rpcoessing");


        MEM_CHK_RESP_PACKET_t *pRespPkt = (MEM_CHK_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        // Status
        //system_status.acknowledge = _SYS_VALID_;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_ECHO_MESSAGE:
    {

		 write_debug_log("OP_REQ_ECHO_MESSAGE rpcoessing");


        ECHO_MSG_REQ_PACKET_t *pReqPkt = (ECHO_MSG_REQ_PACKET_t *) pktFrame;
        ECHO_MSG_RESP_PACKET_t *pRespPkt = (ECHO_MSG_RESP_PACKET_t *) vds_tx_buf;
        u16_t echoMsgCount = 0;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        echoMsgCount = 0;

        while (echoMsgCount < 256 && echoMsgCount < (frameLen - sizeof(pReqPkt->head)))
        {
            pRespPkt->echo_msg[echoMsgCount] = pReqPkt->echo_msg[echoMsgCount];
            echoMsgCount++;
        }
        //pRespPkt->echo_msg[echoMsgCount-1] = NULL;


        pktLen = (sizeof(*pRespPkt) -1) + echoMsgCount;	// echo_msg[1] 를 빼줘야 함.

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_SEQ_NUMBER:
    {
		 write_debug_log("OP_SEQ_NUMBER rpcoessing");


        SEQ_NUM_REQ_PACKET_t *pReqPkt = (SEQ_NUM_REQ_PACKET_t *) pktFrame;
        SEQ_NUM_RESP_PACKET_t *pRespPkt = (SEQ_NUM_RESP_PACKET_t *) vds_tx_buf;
        u8_t seqStartNum = pReqPkt->start_seq;
        u16_t seqNumCount = pReqPkt->count;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 11) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Sequence Number
        if (seqNumCount > 256)
            seqNumCount = 256;
        for (i=0; i<seqNumCount; i++)
            pRespPkt->seq_num[i] = (seqStartNum + i);

        pktLen = (sizeof(*pRespPkt) -1) + seqNumCount;	// seq_num[1]

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_VERSION_REQ:
    {

		 write_debug_log("OP_REQ_VERSION_REQ rpcoessing");

        VERSION_RESP_PACKET_t *pRespPkt = (VERSION_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Version
#if !defined(USE_REAL_VDS_DATA)
        pRespPkt->version = testPattern++;
        pRespPkt->year = testPattern++;
        pRespPkt->month = testPattern++;
        pRespPkt->day = testPattern++;
        pRespPkt->chksum[0] = testPattern++;
        pRespPkt->chksum[1] = testPattern++;
#else
        pRespPkt->version = VERSION_NUM;
        pRespPkt->year = PRODUCTED_YEAR;
        pRespPkt->month = PRODUCTED_MONTH;
        pRespPkt->day = PRODUCTED_DAY;
        //pRespPkt->chksum[0] = 0x55;
        //pRespPkt->chksum[1] = 0xee;
#endif

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;
    }
    break;

    case OP_REQ_INDIV_VDS_DATA:
    {
		 write_debug_log("OP_REQ_INDIV_VDS_DATA rpcoessing");

        //-----------------------------------------------------------------------------------
        // One Hour Data with Tcp/Ip
        //----------------------------------------------------------------------------------
        INDIV_VDS_DATA_RESP_PACKET_t *pRespPkt = (INDIV_VDS_DATA_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen, rt_data_num;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        host_req_counter = 0; //by capi 2014.09.18

        if(pRecvHead->tlen[3] != 9) 	//by capi 2014.09.18
            system_result = TCP_DATA_LENGTH_ERROR;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        // Frame number.
        pRespPkt->frame_num = frame_num;

        rt_data_num = insertRtDataToVdsPacket(&pRespPkt->rt_data[0]);

        // MSB first.
        pRespPkt->data_num[0] = (rt_data_num >> 8);
        pRespPkt->data_num[1] = rt_data_num & 0xFF;

        //zeroSendRtVdsBuff();

        pktLen = sizeof(*pRespPkt) - sizeof(TCP_ONE_HOUR_VENHICLE_t);
        pktLen += rt_data_num * sizeof(TCP_ONE_HOUR_VENHICLE_t);

        tlen = rt_data_num *6 +15;

        fprintf(stdout," (0x16) rt_data_num : %d, packet : %ld, tlen : %ld", rt_data_num, pktLen, tlen);
        // Total Length
        pRespPkt->head.tlen[0] = (tlen & 0xFF000000) >> 24;
        pRespPkt->head.tlen[1] = (tlen & 0x00FF0000) >> 16;
        pRespPkt->head.tlen[2] = (tlen & 0x0000FF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x000000FF;

    }
    break;

    case OP_VAILD_SESSION :
    {
		 write_debug_log("OP_VAILD_SESSION rpcoessing");

        validsession_flag = 0;
        validsession_counter = 0;
        vds_st.con_state = 1;
    }
    break;

    case OP_INVERS_VDS_DATA:	//insert by capi 2014.09.02

		 write_debug_log("OP_INVERS_VDS_DATA rpcoessing");


        //recv inverse data
        invers_st.inverse_time = 0;
        invers_st.inverse_flag = 0;
        invers_st.inverse_cnt = 0;
        for (i=0; i<MAX_LANE_NUM; i++)
            invers_st.gRT_Reverse[i] = FALSE;
        for (i=0; i<MAX_LANE_NUM; i++)
            invers_st.gRT_Reverse_retrans[i] = FALSE;

        break;

    case OP_CHANGE_IP:
    {

		 write_debug_log("OP_CHANGE_IP rpcoessing");

        CHG_IP_REQ_PACKET_t *pReqPkt = (CHG_IP_REQ_PACKET_t *) pktFrame;
        CHG_IP_RESP_PACKET_t *pRespPkt = (CHG_IP_RESP_PACKET_t *) vds_tx_buf;
        char tmpStrIp[IP_STR_LEN+1];
        uint32 tlen;
        int status;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        for (i=0; i<IP_STR_LEN; i++)
            tmpStrIp[i] = pReqPkt->new_ip[i];
        tmpStrIp[IP_STR_LEN] = 0;

#if 1
        chk_n_SaveStrIpToNVRAM(MMI_CFG_IDX_01_IP_ADDR, tmpStrIp, NULL, FLAG_ALL);
#else
        status = netIpStr2Bin(tmpStrIp, server_ip);

        if (status == 0)
        {
            for(i=0; i<IP_NUM_LEN; i++)
                gstNetConfig.ip[i] = ip_addr[i];
            writeNetConfigToNAND(TRUE);
            sendNetConfigMsgToMMI(MMI_CFG_IDX_01_IP_ADDR);
            dumpNetworkConfig();
        }
        else
        {
            fprintf(stdout," ############## Invalid IP Address (2) !!");
            fprintf(stdout," ==> ");
            for (i=0; i<IP_STR_LEN; i++)
                fprintf(stdout,"%c", pReqPkt->new_nmsk[i]);
            fprintf(stdout,"");
        }
#endif

        for(i=0; i<IP_STR_LEN; i++)
            pRespPkt->ip[i] = pReqPkt->new_ip[i];

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_CHG_STATION_NUM:
    {

		 write_debug_log("OP_CHG_STATION_NUM rpcoessing");

        CHG_ST_NUM_REQ_PACKET_t *pReqPkt = (CHG_ST_NUM_REQ_PACKET_t *) pktFrame;
        CHG_ST_NUM_RESP_PACKET_t *pRespPkt = (CHG_ST_NUM_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18
        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);


        for(i=0; i<STATION_NUM_LEN; i++)
            gstNetConfig.station_num[i] = pReqPkt->new_st_num[i];
        writeNetConfigToNAND(TRUE);

        //mmmmmmmmmmmmmmmmmmmmmmmmmmmmm
        sendNetConfigMsgToMMI(MMI_CFG_IDX_06_STATION_NUM);
        dumpNetworkConfig();

        for(i=0; i<STATION_NUM_LEN; i++)
            pRespPkt->st_num[i] = pReqPkt->new_st_num[i];

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_CHANGE_MAC:
    {
		 write_debug_log("OP_CHANGE_MAC rpcoessing");

        CHG_MAC_REQ_PACKET_t *pReqPkt = (CHG_MAC_REQ_PACKET_t *) pktFrame;
        CHG_MAC_RESP_PACKET_t *pRespPkt = (CHG_MAC_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////

        for(i=0; i<MAC_ADDR_LEN; i++)
            gstNetConfig.mac[i] = pReqPkt->new_mac[i];

        setInterfaces();	//by capi interface file write
        writeNetConfigToNAND(TRUE);
        dumpNetworkConfig();

        for(i=0; i<MAC_ADDR_LEN; i++)
            pRespPkt->mac[i] = pReqPkt->new_mac[i];

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

    }
    break;

    case OP_GET_TEMP:
    {
		 write_debug_log("OP_GET_TEMP rpcoessing");


        CHG_PAN_HEATER_RESP_PACKET_t *pRespPkt = (CHG_PAN_HEATER_RESP_PACKET_t *) vds_tx_buf;

        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//insert by Capidra 2011.05.27

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        fprintf(stdout,"Temp : \n\r");
        fprintf(stdout,"Cur : %d/%d  Set : %d %d: \n\r"
                , g_currSysStatus.temper[0], g_currSysStatus.temper[1], gstSysConfig.temper[0], gstSysConfig.temper[1]);

        pRespPkt->divi = 0;
        //////////////////////////////////////////////////////////////////////

        pRespPkt->temp[0] = gstSysConfig.temper[0];
        pRespPkt->temp[1] = gstSysConfig.temper[1];
        pRespPkt->curtemp[0] = g_currSysStatus.temper[0];
        pRespPkt->curtemp[1] = g_currSysStatus.temper[1];


        if(gstSysConfig.temper[0]<0)
            pRespPkt->divi |= 0x08;
        if(gstSysConfig.temper[1]<0)
            pRespPkt->divi |= 0x04;
        if(g_currSysStatus.temper[0]<0)
            pRespPkt->divi |= 0x02;
        if(g_currSysStatus.temper[1]<0)
            pRespPkt->divi |= 0x01;

        //////////////////////////////////////////////////////////////////////
        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        // 패킷 전송.
        goto pkt_send;
    }
    break;

    case OP_SET_TEMP:
    {
		 write_debug_log("OP_SET_TEMP rpcoessing");

        CHG_PAN_HEATER_REQ_PACKET_t *pReqPkt = (CHG_PAN_HEATER_REQ_PACKET_t *) pktFrame;
        CHG_PAN_HEATER_RESP_PACKET_t *pRespPkt = (CHG_PAN_HEATER_RESP_PACKET_t *) vds_tx_buf;

        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//insert by Capidra 2011.05.27
        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////

        pRespPkt->curtemp[0] = g_currSysStatus.temper[0];
        pRespPkt->curtemp[1] = g_currSysStatus.temper[1];

        gstSysConfig.temper[0] = pReqPkt->temp[0];
        gstSysConfig.temper[1] = pReqPkt->temp[1];

        writeParamToNAND(TRUE);
        sendSysConfigMsgToMMI();

        pRespPkt->temp[0] = gstSysConfig.temper[0];
        pRespPkt->temp[1]  = gstSysConfig.temper[1];


        if(gstSysConfig.temper[0]<0)
            pRespPkt->divi |= 0x08;
        if(gstSysConfig.temper[1]<0)
            pRespPkt->divi |= 0x04;
        if(g_currSysStatus.temper[0]<0)
            pRespPkt->divi |= 0x02;
        if(g_currSysStatus.temper[1]<0)
            pRespPkt->divi |= 0x01;

        fprintf(stdout," Temp : /n/r");
        fprintf(stdout,"cur: %d/%d  Set : %d %d: \n\r"
                ,g_currSysStatus.temper[0], g_currSysStatus.temper[1], (int)gstSysConfig.temper[0], (int)gstSysConfig.temper[1]);

        //////////////////////////////////////////////////////////////////////

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        // 패킷 전송.
        goto pkt_send;
    }
    break;

    case OP_POWER1:
    {

		 write_debug_log("OP_POWER1 rpcoessing");


        CHG_POWER_REQ_PACKET_t *pReqPkt =  (CHG_POWER_REQ_PACKET_t *) pktFrame;
        CHG_POWER_RESP_PACKET_t *pRespPkt = (CHG_POWER_RESP_PACKET_t *) vds_tx_buf;

        uint32 tlen;
        u8_t  tmp;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			// by Capidra 2011.05.27
        clearSystemResultcode();						//insert by Capidra 2011.05.27

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////
        /*
        gusDipSwitch = (*(volatile unsigned short *)(DIP_SWITCH_BASE)) & 0xF000;
        tmp = gusDipSwitch>>12;

        pRespPkt->status = tmp&0x03;

        if(pReqPkt->status == 1 && g_currSysStatus.relay_state[2] == RELAY_OFF)
        {
        	turnOnPower1();
        	g_currSysStatus.relay_state[2] = RELAY_ON;
        }
        else if(pReqPkt->status == 0 && g_currSysStatus.relay_state[2] == RELAY_ON)
        {
        	turnOffPower1();
        	g_currSysStatus.relay_state[2] = RELAY_OFF;
        }
        */
        //////////////////////////////////////////////////////////////////////

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        // 패킷 전송.
        goto pkt_send;
    }
    break;

    case OP_POWER2:
    {

		 write_debug_log("OP_POWER2 rpcoessing");

        CHG_POWER_REQ_PACKET_t *pReqPkt =  (CHG_POWER_REQ_PACKET_t *) pktFrame;
        CHG_POWER_RESP_PACKET_t *pRespPkt = (CHG_POWER_RESP_PACKET_t *) vds_tx_buf;

        uint32 tlen;
        u8_t  tmp;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//insert by Capidra 2011.05.27

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////
        /*
        gusDipSwitch = (*(volatile unsigned short *)(DIP_SWITCH_BASE)) & 0xF000;
        tmp = gusDipSwitch>>12;

        pRespPkt->status = tmp&0x03;

        if(pReqPkt->status == 1 && g_currSysStatus.relay_state[3] == RELAY_OFF)
        {
        	turnOnPower2();
        	g_currSysStatus.relay_state[3] = RELAY_ON;
        }
        else if(pReqPkt->status == 0 && g_currSysStatus.relay_state[3] == RELAY_ON)
        {
        	turnOffPower2();
        	g_currSysStatus.relay_state[3] = RELAY_OFF;
        }
        //////////////////////////////////////////////////////////////////////
        */
        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        goto pkt_send;
    }
    break;

    //////////////////////////////////////////////////////////////////////////////////////////////////
    case OP_REQ_SYS_STATUS:
    {
		 write_debug_log("OP_REQ_SYS_STATUS rpcoessing");


        REQ_SYS_STS_RESP_PACKET_t *pRespPkt = (REQ_SYS_STS_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;

        validsession_counter = 0;	//2011.09.14 by capidra
        vds_st.con_state = 1;

        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27
        clearSystemResultcode();						//insert by Capidra 2011.05.27

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //////////////////////////////////////////////////////////////////////
        pRespPkt->temper[0] = g_currSysStatus.temper[0];
        pRespPkt->temper[1] = g_currSysStatus.temper[1];
        pRespPkt->pwrvol[0] = g_currSysStatus.pwr_vol[0];
        pRespPkt->pwrvol[1] = g_currSysStatus.pwr_vol[1];
        //////////////////////////////////////////////////////////////////////

        pktLen = sizeof(*pRespPkt);

        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(REQ_SYS_STS_RESP_PACKET_t))/*DATA Field*/;
        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(REQ_SYS_STS_RESP_PACKET_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        goto pkt_send;
    }
    break;

    default:
        // Invalid Opcode
    {

		 write_debug_log("Invalid Opcode rpcoessing");

        INVALID_OPCODE_RESP_PACKET_t *pRespPkt = (INVALID_OPCODE_RESP_PACKET_t *) vds_tx_buf;
        uint32 tlen;
        //delete by capidra 2011.05.30
        // Invalid Opcode 를 Set하고...
        //system_status.unknown_opcode = _SYS_INVALID_;
        system_result =  TCP_OPCODE_ERROR;				//by capidra 2014.09.17
        pRespPkt->head.resultcode = system_result;			//by Capidra 2011.05.27

        clearSystemResultcode();						//by capi 2014.09.18

        // Status
        checkSystemStatus();
        pRespPkt->head.status[0] = getSysStatusDataIdx(0);
        pRespPkt->head.status[1] = getSysStatusDataIdx(1);

        //delete by capidra 2011.05.30
        //IInvalid Opcode
        //system_status.unknown_opcode = _SYS_VALID_;

        pktLen = sizeof(*pRespPkt);

        //retouch by capidra 2011.05.27

        //tlen = 1/*opcode*/ + 2/*status*/ + (pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;
        tlen = 1/*opcode*/ + 1 /*result code*/+ 2/*status*/ + 8/*transaction Num*/ +(pktLen - sizeof(VDS_TCP_PKT_RESP_HEAD_t))/*DATA Field*/;

        // Total Length
        pRespPkt->head.tlen[0] = 0 ;
        pRespPkt->head.tlen[1] = 0;
        pRespPkt->head.tlen[2] = (tlen & 0xFF00) >> 8;
        pRespPkt->head.tlen[3] = tlen & 0x00FF;

        goto pkt_send;
    }
    break;

    }

pkt_send :

	 // 2021.04.14 날짜 변경 시 체크 Transaction Number내 timestamp 로 시간 동기화 진행 by avogadro
	 if (sync_time_flag == 1)
	 {
		 sync_time_flag = 0;
		 syncTimeToServer(pRecvHead);
	 }
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Hyphen
    pSendHead->hyphen_1 = _HYPHEN_CH_;
    pSendHead->hyphen_2 = _HYPHEN_CH_;

    // Sender IP
    for (i=0; i<IP_STR_LEN; i++)
        pSendHead->sip[i] = ch_dest_ip[i];
    // Destination IP
    for (i=0; i<IP_STR_LEN; i++)
        pSendHead->dip[i] = ch_src_ip[i];

    // Controller Kind
    pSendHead->kind[0] = _VDS_KIND_CH_0_;
    pSendHead->kind[1] = _VDS_KIND_CH_1_;

    // Station Number
    for (i=0; i<STATION_NUM_LEN; i++)
        pSendHead->snum[i] = gstNetConfig.station_num[i];		//2011.09.14 by capidra

    // Opcode
    pSendHead->opcode = pRecvHead->opcode;

    //Transaction Number
    for (i=0; i<TRANSACTION_NUM_LEN; i++)
        pSendHead->transnum[i] = pRecvHead->transnum[i];		//insert by capidra 2011.05.27

    pSendHead->resultcode =  system_result;	//by capidra 110608

    // Status
    ///////////////////////////////////

    vds_server_stamp = sys_time;	// by jwank 090608
    system_result = 0;				//by capidra 110608

    //////////////////////////////////////////////////////////////////////////////////////////////////
    /*if(term_stat !=REALTIME_MONITOR_MODE )
    {
    	//fprintf(stdout," Controller Kind : %c%c", pSendHead->kind[0], pSendHead->kind[1]);
    	//fprintf(stdout," Station Number : %02d %02d %02d %02d", pSendHead->snum[0], pSendHead->snum[1], pSendHead->snum[2], pSendHead->snum[3]);
    	//fprintf(stdout," Total Length : %02d %02d %02d %02d", pSendHead->tlen[0], pSendHead->tlen[1], pSendHead->tlen[2], pSendHead->tlen[3]);
    	fprintf(stdout," Opcode : 0x%02X", pSendHead->opcode);

    	//fprintf(stdout," Transact Num : ");
    	//	for (i=0; i<8; i++) fprintf(stdout,"%c", pSendHead->transnum[i]);
    	//fprintf(stdout,"");

    	//fprintf(stdout," Send Data length (%d bytes) [t:%d]", pktLen, getSystemTick());

    	//for(i=0; i<pktLen; i++) fprintf(stdout,"%X ", vds_tx_buf[i]);

    	//fprintf(stdout,"");
    } */

    vds_tx_len = pktLen;

    vds_st.send_left = pktLen;
    vds_st.send_ptr = 0;

    //------------------------------------------------------
    // by wook 2013.02.18
    //------------------------------------------------------
    if(pRecvHead->opcode != OP_REQ_INDIV_VDS_DATA)
    {
        vds_tx_dump_len = pktLen + 2;
        for(i=0; i<pktLen; i++)
            vds_tx_dump[i] =  vds_tx_buf[i];
    }

    if(vds_tx_len == 0)
        return;

    ///////////////////////////////////////////////////////////////
    /// KISS --- 30초 데이타 NAND에 저장
    //char savename[256];
    //int fd_save;

    //if (pRecvHead->opcode == OP_REQ_VDS_DATA)
    //{
    //    sprintf(savename,"/root/am1808/savefolder/%04d%02d%02d/data30s/%02d%02d%02d.txt"
    //            , sys_time.wYear, sys_time.wMonth, sys_time.wDay, sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

    //    if((fd_save = open(savename, O_WRONLY | O_CREAT | O_TRUNC, 0777))== -1)
    //    {
    //        fprintf(stdout,"file open error\n");
    //    }

    //    write(fd_save, vds_tx_dump, strlen(vds_tx_dump));

    //    // data write.
    //    if ( -1 == fsync(fd_save))
    //        fprintf(stdout, "30s data save is failed");

    //    close(fd_save);
    //}
    ///////////////////////// KISS --- 30초 데이타 NAND에 저장

    ///////////////////////////////////////////////////////////////
    /// Data send
    int flag_send = 0;

    if(send(fd, vds_tx_buf, vds_tx_len, flag_send) == -1)
    {
        pr_err("[TCP client] : Fail: send()");
    }
    ///////////////////////////////////////////////////////////////

    // OP_REQ_SYS_INIT Process
    if(sys_init_flag == 1)
    {
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

	 char szLog[255];
	 char szInfo[255];

	 sprintf(szLog, "SendInversData enter...");
	 write_debug_log(szLog);

	 if (vds_st.state != SM_VDS_CLIENT_DEVID_SEND)
	 {
		 sprintf(szLog, "vds_st.state != SM_VDS_CLIENT_DEVID_SEND. so return  SendInversData ");
		 write_debug_log(szLog);

		 return;
	 }

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
    for (i=0; i<IP_STR_LEN; i++)
        pSenddata->sip[i] = ch_dest_ip[i];
    // Destination IP
    for (i=0; i<IP_STR_LEN; i++)
        pSenddata->dip[i] = ch_src_ip[i];

    // Controller Kind
    pSenddata->kind[0] = _VDS_KIND_CH_0_;
    pSenddata->kind[1] = _VDS_KIND_CH_1_;

    for (i=0; i<STATION_NUM_LEN; i++)
        pSenddata->snum[i] = gstNetConfig.station_num[i] ;

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

    for(i=0; i<gucActiveLoopNum; i++)
        pSenddata->data[i] = 0;

	 memset(szInfo, 0, 255);

	 strcpy(szInfo, " inverse loop info \n");

    for(i=0; i<gucActiveDualLoopNum/2; i++)
    {
        if(invers_st.gRT_Reverse[i])
            invers_st.gRT_Reverse_retrans[i] = invers_st.gRT_Reverse[i];
        //invers_st.gRT_Reverse[i] = FALSE;
        if(invers_st.gRT_Reverse_retrans[i])
        {
            pSenddata->data[i*2] = 1;
            pSenddata->data[i*2+1] = 1;

				sprintf(szInfo, " %s %d loop : %d, %d loop : %d \n", szInfo, i * 2, pSenddata->data[i * 2], i * 2 + 1, pSenddata->data[i * 2 + 1]);
        }
    }

	 write_debug_log(szInfo);


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

    if( vds_tx_len ==0)
        return;

    ///////////////////////////////////////////////////////////////
    /// Data send
    int flag_send=0;

    if ( send(fd_listener, vds_tx_buf, vds_tx_len, flag_send) == -1)
    {
        pr_err("[TCP client] : Fail: send()");
    }

    fprintf(stdout,"Send Inverse Data length : %d bytes\n", pktLen);

	 sprintf(szLog, "Send Inverse Data length : %d bytes", pktLen);
	 write_debug_log(szLog);

	 write_debug_log("SendInversData leave...");

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
    for (i=0; i<IP_STR_LEN; i++)
        pSendHead->sip[i] = ch_dest_ip[i];
    // Destination IP
    for (i=0; i<IP_STR_LEN; i++)
        pSendHead->dip[i] = ch_src_ip[i];

    // Controller Kind
    pSendHead->kind[0] = _VDS_KIND_CH_0_;
    pSendHead->kind[1] = _VDS_KIND_CH_1_;

    for (i=0; i<STATION_NUM_LEN; i++)
        pSendHead->snum[i] = gstNetConfig.station_num[i] ;

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

    if( vds_tx_len ==0)
        return;

    ///////////////////////////////////////////////////////////////
    /// Data send
    int flag_send=0;

    if ( send(fd_listener, vds_tx_buf, vds_tx_len, flag_send) == -1)
    {
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
    if ((ret_poll = epoll_wait(epollfd, ep_events, max_ep_events, 0)) == -1)
    {
        /* error */
    }

    if( ret_poll < 1)
        return;

    /* Are there data to read? */
    for (i=0; i<ret_poll; i++)
    {
        if (ep_events[i].events & EPOLLIN)
        {
            /* IS New connection ? */
            if (ep_events[i].data.fd == fd_listener)
            {
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
            if( ep_events[i].data.fd == fd_tty)
            {

                modemctlline = TIOCM_RTS;
                ioctl( fd_tty, TIOCMBIC, &modemctlline );

                ret_recv = read(fd_tty, &buff, 1);

                modemctlline = TIOCM_RTS;
                ioctl( fd_tty, TIOCMBIS, &modemctlline );

                if (ret_recv != 0)
                {
                    io_comtask(buff);

                    //__dump_message(buff, ret_recv);
                }
                else
                    fprintf(stdout,"Io error\n");
            }
            //---- Ext serial device -------------------
            else if( ep_events[i].data.fd == fd_ttys1)
            {
                //by capi 2014.08
                //modemctlline = TIOCM_RTS;
                //ioctl( fd_ttys1, TIOCMBIC, &modemctlline );

                //ret_recv = read(fd_ttys1, blockbuff, sizeof(blockbuff));
                ret_recv = read(fd_ttys1, &buff1, 1);
                //fprintf(stdout,"Received. ttys1 %02X\n", buff);

                //modemctlline = TIOCM_RTS;
                //ioctl( fd_ttys1, TIOCMBIS,minicom &modemctlline );

                if (ret_recv > 0)
                {
                    mainteCommTask(&buff1);
                }
                else
                    fprintf(stdout,"Ext serial error\n");
            }
            //---- LD Board device 485-----------------------
            else if( ep_events[i].data.fd == fd_ttys6)
            {

                ret_recv = read(fd_ttys6, LDBbuf, 13);

                //usleep(3000);

                if (ret_recv > 0)
                {
                    ldb_comtask(LDBbuf, ret_recv);
#if 0
                    fprintf(stdout,"\n\rReceived. ttys6 [%03d] Byte\n",ret_recv);
                    for(i=0; i<ret_recv; i++)
                        fprintf(stdout,"%02X | ", LDBbuf[i]);

                    fprintf(stdout,"\n");
#endif
                    //__dump_message(buff, ret_recv);
                }
                else
                    fprintf(stdout,"LDB error\n");
            }
            //---- ethernet device -----------------------
            else if (ep_events[i].data.fd == fd_listener)
            {
                ret_recv = recv(ep_events[i].data.fd, buf, sizeof(buf), 0);
                if (ret_recv == -1)
                {
                    /* error */
                    if(term_stat !=REALTIME_MONITOR_MODE )
                    {
                        pr_out("fd(%d) : server process closed error ", ep_events[i].data.fd);
                    }
						  write_debug_log("server process closed error");
                    DEL_EV(epollfd, ep_events[i].data.fd);
                    //network_alive = FALSE;
                    vds_st.state = SM_VDS_CLIENT_CLOSEED; //by capi 2014.09.04
                    save_log(NET_COLSE_SERVER, 0);
                }
                else if (ret_recv == 0)
                {
                    /* closed */
                    if(term_stat !=REALTIME_MONITOR_MODE )
                        pr_out("fd(%d) : Session closed", ep_events[i].data.fd);
                    DEL_EV(epollfd, ep_events[i].data.fd);
                    //network_alive = FALSE;
                    vds_st.state = SM_VDS_CLIENT_CLOSEED; //by capi 2014.09.04

                    save_log(NET_COLSE_SESSION, 0);
                }
                else
                {
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
        else if (ep_events[i].events & EPOLLPRI)
        {
            pr_out("EPOLLPRI : Urgent data detected");

            if ((ret_recv = recv(ep_events[i].data.fd, buf, 1, MSG_OOB)) == -1)
            {
                /* error */
            }
            pr_out("recv(fd=%d,n=1) = %.*s (OOB)", ep_events[i].data.fd, 1, buf);
#endif
        }
        else if (ep_events[i].events & EPOLLERR)
        {
            fprintf(stdout,"EPOLLERR\n");
            /* error */
        }
        else
        {
            pr_out("fd(%d) epoll event(%d) err(%s)",
                   ep_events[i].data.fd, ep_events[i].events, strerror(errno));
        }
    }

    return 0;
}

init_epoll()
{
    vds_st.state = SM_VDS_CLIENT_CLOSEED;		//by capi 2014.09.04

    if ((epollfd = epoll_create(1)) == -1)
    {
        save_log( POLL_CREATE_EXIT, 0); // kwj insert. 2017.07.19

        /* error */
        exit(EXIT_FAILURE);
    }
    if ((ep_events = calloc(max_ep_events, sizeof(struct epoll_event))) == NULL)
    {

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

    if ((rc_gai = getaddrinfo(serverIP, s_port, &ai, &ai_ret)) != 0)
    {
        pr_err("Fail: getaddrinfo():%s", gai_strerror(rc_gai));
        exit(EXIT_FAILURE);
    }

	 


	 // 코드 추가 시작
	 if ((fd_listener = socket(AF_INET, SOCK_STREAM, 0)) < 0) 
	 {	 perror("error");
		  return 1;
	 }

	 struct sockaddr_in serveraddr; 
	 serveraddr.sin_family = AF_INET;
	 serveraddr.sin_addr.s_addr = inet_addr(serverIP);
	 serveraddr.sin_port = htons(gstNetConfig.server_port);
	 int len = sizeof(serveraddr);

	 //printf("connect_timeout...ip=%s port=%s \n", serverIP, s_port);
	 if (connect_timeout(fd_listener, (struct sockaddr *)&serveraddr, len, 2) < 0) //2초 timeout 설정
	 { 
		 	vds_st.state = SM_VDS_CLIENT_ABORTED;
		 	//fprintf(stdout,"socket not connected\n");
		 	tcpip_connected = -1;

			write_debug_log("server connection timeout");
	 }
	 else
	 {
		 // 연결 성공 시 non blocking 설정 
			fcntl_setnb(fd_listener); /* set nonblock flag */
		 	/*if(term_stat !=REALTIME_MONITOR_MODE )
				fprintf(stdout,"socket connected\n");*/
			vds_st.state = SM_VDS_CLIENT_CONNECTED;  //by capi 2014.09.04
			tcpip_connected = 1;
			write_debug_log("server connection success!!");
	 }
	 // 코드 추가 종료
	
	 // 2020.10.11 연결 오류 수정
	 // 증상: 연결이 되지 않았음에도 연결된것으로 처리됨(errno 가 EINPROGRESS 임에도 처리가 종료되지 않았는데 연결된것으로 처리하고 있음) 

	 // 기존 코드 코멘트 처리- 시작
	 //if ((fd_listener = socket(
	 //                       ai_ret->ai_family,
	 //                       ai_ret->ai_socktype,
	 //                       ai_ret->ai_protocol)) == -1)
	 //{
	 //    pr_err("Fail: socket()");
	 //    exit(EXIT_FAILURE);
	 //}

	 //fcntl_setnb(fd_listener); /* set nonblock flag */
	 //(void)connect(fd_listener, ai_ret->ai_addr, ai_ret->ai_addrlen);
#if 1
    // non blocking process
    //if(errno != EINPROGRESS)
    //{
    //    vds_st.state = SM_VDS_CLIENT_ABORTED;
    //    fprintf(stdout,"socket not connected\n");
    //    tcpip_connected = -1;
		  //
    //}
    //else
    //{
    //    if(term_stat !=REALTIME_MONITOR_MODE )
    //        fprintf(stdout,"socket connected\n");
    //    vds_st.state = SM_VDS_CLIENT_CONNECTED;  //by capi 2014.09.04
    //    tcpip_connected = 1;
#ifdef SEGMENT
        i2cSegDigit(1,1);
        i2cSegDigit(1,0);
#endif
    //}
	// 기존 코드 코멘트 처리- 종료
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
    else
    {
        if(term_stat !=REALTIME_MONITOR_MODE )
            fprintf(stdout,"tcpip connect\n");
        tcpip_connected = 1;
    }
#endif

//	init_epoll();
#if 0

    if ((epollfd = epoll_create(1)) == -1)
    {
        /* error */
        exit(EXIT_FAILURE);
    }
    if ((ep_events = calloc(max_ep_events, sizeof(struct epoll_event))) == NULL)
    {
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
    if (epoll_ctl(efd, EPOLL_CTL_ADD, fd, &ev) == -1)
    {
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
    if (epoll_ctl(efd, EPOLL_CTL_DEL, fd, NULL) == -1)
    {
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
    if (fcntl(fd, F_SETFL, O_NONBLOCK | fcntl(fd, F_GETFL)) == -1)
    {
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

    if (!polling_count)
        return;

    for(i=0; i<gucActiveDualLoopNum; i++)
    {
        if (loop_st[i].status == PRESSED)
        {
            t_occupy = io_time_counter - loop_st[i].time[PRESS_TIME];

            if (t_occupy >= polling_count)
                gstIncidentAccuData.occupy[i] = polling_count;  // 100 %
            else
                gstIncidentAccuData.occupy[i] += t_occupy;
        }
    }

    for(i=0; i<gucActiveSingleLoopNum; i++)
    {
        if (loop_st[gucActiveDualLoopNum+i*2].status == PRESSED)
        {
            t_occupy = io_time_counter - loop_st[gucActiveDualLoopNum+i*2].time[PRESS_TIME];

            if (t_occupy >= polling_count)
                gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2] = polling_count;  // 100 %
            else
                gstIncidentAccuData.occupy[gucActiveDualLoopNum+i*2] += t_occupy;

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
        if (t_occupy > 100*1000)
            t_occupy = 100*1000;

        gstIncidentPolling.occupy[i] = t_occupy;

        //fprintf(stdout, "%d loop  %d",i ,gstIncidentPolling.occupy[i]);
#else

#if 1 // bug fix ?? pppp

        t_occupy = 10 * 100 * gstIncidentAccuData.occupy[i];
        t_occupy /= polling_count;

        t_occupy += 5;
        t_occupy /= 10;

        if (t_occupy > 100)
            t_occupy = 100;
#else
        t_occupy = 10 * 100 * gstCenterAccuData.occupy[i] / polling_count;
        t_occupy = t_occupy / 5;
        t_occupy = (t_occupy * 5)/10;
        if (t_occupy > 100)
            t_occupy = 100;
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
        for (i=0; i<(MAX_LOOP_NUM/8); i++)
            incident_per_loop[i] = 0;

        prepareIncidentPollData();

        fprintf(stdout, "%d  %d  ",gstSysConfig.param_cfg.K_factor[0], gstSysConfig.param_cfg.T_value[0]);

        //fprintf(stdout, "\n");
        for(i=0; i<gucActiveLoopNum; i++)
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
    if(pre_stuck_count<curtime)
        passtime = curtime - pre_stuck_count;
    else
        passtime = 3600000 + curtime - pre_stuck_count;

    pre_stuck_count = curtime;

    for(i=0; i<MAX_LOOP_NUM; i++)
    {
        stuckon_counter[i] = stuckon_counter[i]+passtime;
        stuckoff_counter[i] = stuckoff_counter[i]+passtime;

    }
}

//kiss
void InverseDataCheckAndSend(void)
{
    uint32 i, delta_t;

    if(vds_st.state == SM_VDS_CLIENT_DEVID_SEND)	//2014.03.26 insert by capi 	SM_VDS_CLIENT_DEVID_SEND = 3
    {
        if(invers_st.inverse_flag)
        {
			   //write_debug_log("InverseDataCheckAndSend  inverse data found..");
            if(invers_st.inverse_cnt == 0)
            {
                invers_st.inverse_cnt = getSystemTick();
                invers_st.inverse_time = 1;

                SendInversData(0);
                fprintf(stdout, "\n\r SendInversData( 0000 ) \r"); //kiss debug
                invers_st.inverse_time++; //kiss debug

                for (i=0; i<MAX_LANE_NUM; i++)
                    invers_st.gRT_Reverse[i] = FALSE;

            }
            else
            {

                if ( getSystemTick() <	invers_st.inverse_cnt)
                    delta_t = getSystemTick() + 3600000 - invers_st.inverse_cnt;
                else
                    delta_t =  getSystemTick() - invers_st.inverse_cnt;

					 //printf("getSystemTick() = %d , invers_st.inverse_cnt=%d , delta_t=%d \n", getSystemTick(), invers_st.inverse_cnt, delta_t);
					 
					 if (delta_t > 10000)
                {
                    invers_st.inverse_cnt = getSystemTick();

                    SendInversData(1);
                    fprintf(stdout,"\n\r SendInversData( 1111 ) \r");  //kiss debug

                    invers_st.inverse_time++;

                    if(invers_st.inverse_time > 2)
                    {
                        invers_st.inverse_flag = 0;
                        invers_st.inverse_cnt = 0;
                        invers_st.inverse_time = 0;

                        for (i=0; i<MAX_LANE_NUM; i++)
                        {
                            invers_st.gRT_Reverse_retrans[i] = FALSE;
                            invers_st.gRT_Reverse[i] = FALSE;
                        }
                    }
                }
            }
        }
    }
    else  // 2020/06/05
    {
		 if (invers_st.inverse_flag)
		 {
			 write_debug_log("InverseDataCheckAndSend  inverse data found but vds_st.state != SM_VDS_CLIENT_DEVID_SEND ");
		 }
        invers_st.inverse_flag = 0;
        invers_st.inverse_cnt = 0;
        invers_st.inverse_time = 0;

        for (i=0; i<MAX_LANE_NUM; i++)
        {
            invers_st.gRT_Reverse_retrans[i] = FALSE;
            //kiss invers_st.gRT_Reverse[i] = FALSE;  // kiss 2020-07-24 역주생 mius 값 표시를 위해서 막
        }
    }
}


int connect_timeout(int sockfd, struct sockaddr *saddr, int addrsize, int sec)
{
	int newSockStat;
	int orgSockStat;
	int res, n;
	fd_set  rset, wset;
	struct timeval tval;

	int error = 0;
	int esize;

	if ((newSockStat = fcntl(sockfd, F_GETFL, NULL)) < 0)
	{
		perror("F_GETFL error");
		return -1;
	}

	orgSockStat = newSockStat;
	newSockStat |= O_NONBLOCK;

	// Non blocking 상태로 만든다.
	if (fcntl(sockfd, F_SETFL, newSockStat) < 0)
	{
		perror("F_SETLF error");
		return -1;
	}

	// 연결을 기다린다.
	// Non blocking 상태이므로 바로 리턴한다.
	if ((res = connect(sockfd, saddr, addrsize)) < 0)
	{
		if (errno != EINPROGRESS)
			return -1;
	}

	//printf("RES : %d\n", res);
	// 즉시 연결이 성공했을 경우 소켓을 원래 상태로 되돌리고 리턴한다.
	if (res == 0)
	{
		//printf("Connect Success\n");
		fcntl(sockfd, F_SETFL, orgSockStat);
		return 1;
	}

	FD_ZERO(&rset);
	FD_SET(sockfd, &rset);
	wset = rset;

	tval.tv_sec = sec;
	tval.tv_usec = 0;

	if ((n = select(sockfd + 1, &rset, &wset, NULL, &tval)) == 0)
	{
		// timeout
		errno = ETIMEDOUT;
		return -1;
	}

	// 읽거나 쓴 데이터가 있는지 검사한다.
	if (FD_ISSET(sockfd, &rset) || FD_ISSET(sockfd, &wset))
	{
		//printf("Read data\n");
		esize = sizeof(int);
		if ((n = getsockopt(sockfd, SOL_SOCKET, SO_ERROR, &error, (socklen_t *)&esize)) < 0)
			return -1;
	}
	else
	{
		perror("Socket Not Set");
		return -1;
	}


	fcntl(sockfd, F_SETFL, orgSockStat);
	if (error)
	{
		errno = error;
		perror("Socket");
		return -1;
	}

	return 1;
}


void syncTimeToServer(VDS_TCP_PACKET_HEADER_t *pRecvHead)
{
	int i;
	int timestamp = 0;
	char szlocalTime[255];
	char szServerTime[255];
	char szlog[255];
	char sztemp[255];
	char timebuf[255];
	memset(szlog, 0, 255);
	memset(sztemp, 0, 255);
	memset(timebuf, 0, 255);

	for (i = 0; i < 8; i++)
	{
		sprintf(sztemp, "0x%02X ", pRecvHead->transnum[i]);
		strcat(szlog, sztemp);
	}
	
	sprintf(sztemp, "[syncTimeToServer] ***** transaction number packet ******\n \t%s", szlog);
	//printf("sztemp= %s \n", sztemp);
	write_debug_log(sztemp);

	
	timestamp = pRecvHead->transnum[0] << 24;
	timestamp += pRecvHead->transnum[1] << 16;
	timestamp += pRecvHead->transnum[2] << 8;
	timestamp += pRecvHead->transnum[3] + 3600*9 ; // 9시간 추가(한국시간)
	
	sprintf(szlog, "[syncTimeToServer] timestamp = %d ", timestamp);
	write_debug_log(szlog);

	struct tm *tm_data;
	struct tm * date;

	const time_t t = timestamp;
	tm_data = localtime(&t);

	//printf("sync time ....[%04d/%02d/%02d %02d:%02d:%02d]\n", tm_data->tm_year + 1900, tm_data->tm_mon + 1, tm_data->tm_mday,
	//	tm_data->tm_hour, tm_data->tm_min, tm_data->tm_sec);

	sprintf(szServerTime, "%04d%02d%02d%02d%02d", tm_data->tm_year + 1900, tm_data->tm_mon + 1, tm_data->tm_mday,
		tm_data->tm_hour, tm_data->tm_min);

	sprintf(szlocalTime, "%04d%02d%02d%02d%02d", sys_time.wYear, sys_time.wMonth, sys_time.wDay, \
		sys_time.wHour, sys_time.wMinute);
	//printf("Server Time=%s, Local Time=%s \n", szServerTime, szlocalTime);

	if (strcmp(szServerTime, szlocalTime) != 0)
	{
		//printf(" need to time sync...\n");

		SYSTEMTIME nextTime;

		nextTime.wYear = tm_data->tm_year + 1900;
		nextTime.wMonth = tm_data->tm_mon + 1;
		nextTime.wDay = tm_data->tm_mday; 
		nextTime.wHour = tm_data->tm_hour;
		nextTime.wMinute = tm_data->tm_min;
		nextTime.wSecond = tm_data->tm_sec;

		/*
			2021.03.23 이 함수에서 바로 RTC 를 변경하려고 하면 변경이 되지 않는 버그 있음. 원인 파악 불가.
						  대신, st_web_flag 를 설정하여 웹 시간 설정 루틴 호출하도록 우회함. 
			by avogadro
		*/
		web_server_time.wYear = nextTime.wYear;
		web_server_time.wMonth = nextTime.wMonth;
		web_server_time.wDay = nextTime.wDay;
		web_server_time.wHour = nextTime.wHour;
		web_server_time.wMinute = nextTime.wMinute;
		web_server_time.wSecond = nextTime.wSecond;
		
		st_web_flag = 1;

		// os time 변경 
		/*sprintf(timebuf, "date -s '%d-%d-%d %d:%d:%d'"
			, nextTime.wYear, nextTime.wMonth, nextTime.wDay
			, nextTime.wHour, nextTime.wMinute, nextTime.wSecond);
		system(timebuf);
*/
		//printf("next time = %s \n", timebuf);

		// RTC 변경 
		/*msec_sleep(300);
		setSysTimeToRTC(&nextTime);

		msec_sleep(300);
		SYSTEMTIME rtcTime;
		getRtcTime(&rtcTime, 0);


		sprintf(szlocalTime, "getRTCTime : %04d-%02d-%02d %02d:%02d:%02d \n", rtcTime.wYear, rtcTime.wMonth, rtcTime.wDay, \
			rtcTime.wHour, rtcTime.wMinute, rtcTime.wSecond);
		printf(szlocalTime);
		*/

	}
	else
	{
		//printf("no need to time sync...\n");
	}

}

void resetCenterPollData()
{
	//printf("resetCenterPollData.....\n");
	memset(&gstCenterAccuData, 0, sizeof(TRAFFIC_ST1));
}

uint8 save_data30s(VDS_TCP_PACKET_HEADER_t *pRecvHead, VDS_DATA_RESP_PACKET_t *pRespPkt, u16_t len)
{
	uint8 ret = 1;
	u16_t index = 0;
	int i,j;
	char savename[256];
	char buffer[256];
	char logTime[256];
	int fd_save;


	sprintf(logTime, "%d/%02d/%02d %02d:%02d:%02d ", \
		sys_time.wYear, sys_time.wMonth, sys_time.wDay, \
		sys_time.wHour, sys_time.wMinute, sys_time.wSecond);


	sprintf(savename, "/root/am1808/savefolder/%04d%02d%02d/%04d%02d%02d_data30s.txt"
			, sys_time.wYear, sys_time.wMonth, sys_time.wDay, sys_time.wYear, sys_time.wMonth, sys_time.wDay);

	if ((fd_save = open(savename, O_CREAT| O_RDWR | O_APPEND , 0777)) == -1) //O_CREAT | 
	{
		fprintf(stdout, "file open error\n");
		return 0;
	}

	/*
	// frame no
	sprintf(buffer, "frame no: %d\n", pRespPkt->frame);
	write(fd_save, buffer, strlen(buffer));
	//printf(buffer);

	memset(buffer, 0, 256);
	//  Stuck of Loop.
	for (i = 0; i < MAX_LOOP_NUM / 4; i++)
		sprintf(buffer, "%s\t stuck %d:0x%02X", buffer, i+1, pRespPkt->data[index++]);

	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));
	//printf(buffer);

  // Incident Info.
	memset(buffer, 0, 256);
	for (i = 0; i < MAX_LOOP_NUM / 8; i++)
		sprintf(buffer, "%s\t incident %d:0x%02X", buffer, i + 1, pRespPkt->data[index++]);

	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));
	//printf(buffer);
	// loop count 
	sprintf(buffer, "loop count: %d\n", pRespPkt->data[index++]);
	write(fd_save, buffer, strlen(buffer));
	//printf(buffer);

	for (i = 0; i < gucActiveLoopNum; i++)
	{
		// 교통량 
		memset(buffer, 0, 256);
		sprintf(buffer, "%d loop\n\tvolume : %d\t",i+1, pRespPkt->data[index++]);
		write(fd_save, buffer, strlen(buffer));
		//printf(buffer);
		// 점유율
		memset(buffer, 0, 256);
		sprintf(buffer, "occupy: %d.%d\n", pRespPkt->data[index++], pRespPkt->data[index++]);
		write(fd_save, buffer, strlen(buffer));
		//printf(buffer);
	}
	// 차선수
	sprintf(buffer, "lane count: %d\n", pRespPkt->data[index++]);
	write(fd_save, buffer, strlen(buffer));
	//printf(buffer);

	for (i = 0; i < gucActiveLoopNum / 2; i++)
	{
		memset(buffer, 0, 256);
		sprintf(buffer, "%d lane\n\tspeed : %d\t length=%d\n",i+1, pRespPkt->data[index++], pRespPkt->data[index++]);
		write(fd_save, buffer, strlen(buffer));
		//printf(buffer);
	}
	*/

	//logTime

	sprintf(buffer, "[%s]\t,%05d-%05d, LOOPS=%02d, LANES=%02d, ECODE=,ICODE=[ 0][ 0][ 0]\n",logTime,
		(gstNetConfig.station_num[0] << 8) + gstNetConfig.station_num[1],
		(gstNetConfig.station_num[2] << 8) + gstNetConfig.station_num[3],
		gucActiveLoopNum, gucActiveLoopNum / 2);
	write(fd_save, buffer, strlen(buffer));

	sprintf(buffer, "\tVOL   :");

	for (j = 0; j < gucActiveLoopNum; j++)
		sprintf(buffer, "%s %3d ",buffer, gstCenterPolling.volume[j]);
	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));



#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
	sprintf(buffer, "\tOCC   :");
	for (j = 0; j < gucActiveLoopNum; j++)
		sprintf(buffer, "%s %3d ", buffer, round_for_occupy(gstCenterPolling.occupy[j], 0));
#else
	sprintf(buffer, "\tOCC   :");
	for (j = 0; j < gucActiveLoopNum; j++)
		sprintf(buffer, "%s %3d ", buffer, gstCenterPolling.occupy[j]);
#endif
	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));

	sprintf(buffer, "\tSPD   :");
	for (j = 0; j < gucActiveLoopNum / 2; j++)
		sprintf(buffer, "%s %7d ", buffer, gstCenterPolling.speed[j]);
	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));


	sprintf(buffer, "\tLEN   :");
	for (j = 0; j < gucActiveLoopNum / 2; j++)
		sprintf(buffer, "%s %7d ", buffer, gstCenterPolling.length[j]);
	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));


	sprintf(buffer, "\tsip = %s, dip = %s, kind=%C%C num =%d-%d, OP code = %d\n",
		pRecvHead->sip, pRecvHead->dip, pRecvHead->kind[0],
		pRecvHead->kind[1], pRecvHead->snum[1], pRecvHead->snum[3], pRecvHead->opcode);
	write(fd_save, buffer, strlen(buffer));
	
	sprintf(buffer, "\ttransaction num : ");
	write(fd_save, buffer, strlen(buffer));

	for (i = 0; i < 8; i++)
		sprintf(buffer, " %d ", buffer, pRecvHead->transnum[i]);

	sprintf(buffer, "%s\n", buffer);
	write(fd_save, buffer, strlen(buffer));


	if (-1 == fsync(fd_save))
		fprintf(stdout, "30s data save is failed");
		close(fd_save);
	return ret;
}