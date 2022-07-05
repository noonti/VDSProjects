#ifndef ___FTMS_CPU_H__
#define __FTMS_CPU_H__

#include "systypes.h"
#include "plat_config.h"

#define MAX_LOOP_COUNT				5000 // kwj 2014.04.21

#define CENTER_POLLING_TIME			(30)
#define LOCAL_POLLING_TIME			(30)
#define MAX_CENTER_POLL_TIME			(100)
#define MAX_LOCAL_POLL_TIME			(300)

// tick count per 10ms
//#define _TICK_MS_					1 	// dcs9
#define _TICK_MS_					10 	// dcs9s

#define _1_SEC_TICK_				(1000 /*/ _TICK_MS_*/)  //by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
#define _3_SEC_TICK_				(3000 /*/ _TICK_MS_*/)
#define _5_SEC_TICK_				(5000 /*/ _TICK_MS_*/)
#define _10_SEC_TICK_				(10000 /*/ _TICK_MS_*/)
#define _20_SEC_TICK_				(20000 /*/ _TICK_MS_*/)
#define _60_SEC_TICK_				(60000 /*/ _TICK_MS_*/)

//Serial
#define SYS_MAGIC_NUMBER			0x84329582
#define NETCFG_MAGIC_NUMBER			0x84329583

#define N7_RTS_CTS_DCD				7	// 111
#define MAX_FLOW_CNTRL				N7_RTS_CTS_DCD

#define N0_xRTS_xCTS_xDCD			0	// 000
#define N3_9600_BPS					3

#define DEFAULT_FLOW_CNTRL			N0_xRTS_xCTS_xDCD
#define N0_1200_BPS					0
#define N1_2400_BPS					1
#define N2_4800_BPS					2
#define N3_9600_BPS					3
#define N4_14400_BPS				4
#define N5_19200_BPS				5
#define N6_38400_BPS				6
#define N7_56000_BPS				7
#define N8_57600_BPS				8
#define N9_115200_BPS				9
#define MIN_COMM_BPS				N0_1200_BPS
#define MAX_COMM_BPS				N9_115200_BPS
// Default.
#define DEFAULT_BPS					N3_9600_BPS

//claculation second point  00.00
#define INCREASE_SPEED_PRECISION
#define INCREASE_OCCUPY_RATE_PRECISION

//------------------------------------------------------
// by wook 2013.02.18 diagnosis Program protocol define
//------------------------------------------------------
//by capi 2014.08
#define VERSION_NUM 				5	//DCS9S
#define SW_VERSION_NUM				1	//ETHERNET
#define PRODUCTED_YEAR				15
#define PRODUCTED_MONTH 			12
#define PRODUCTED_DAY				8
//-------------------------------------------------------------

#define MAX_LOOP_NUM				32
#define MAX_LANE_NUM				(MAX_LOOP_NUM/2)
#define MAX_RIO_LOOP_NUM			24
#define MAX_RIO_LANE_NUM			(MAX_RIO_LOOP_NUM/2)
#define DEFAULT_LOOP_NUM			8

#define SPEED_CATEGORY_NO			12
#define LENGTH_CATEGORY_NO			3
#define LOW_VOLUME_THRH				0
#define HIGH_VOLUME_THRH			1

#define MAC_ADDR_LEN				(6)
#define IP_STR_LEN					(15)
#define IP_NUM_LEN					(4)
#define STATION_NUM_LEN				(4)
#define TRANSACTION_NUM_LEN			(8)
#define CONTROLLER_ID_STR_LEN		(4)

#define DEVICE_ID_STR_LEN			(15)
#define TUNNEL_ID_STR_LEN			(6)
#define _DEFAULT_PARAM_				0x30
#define _USER_SETTING_PARAM_		0x31

#define MAX_MODEM_DATA_LENGTH		0x100
#define MAX_MODEM_DATA_QUEUE_NO		0x100

#define EVENT_CREATED				0x41
#define NO_EVENT					0x40

#define NETCFG_MAGIC_NUMBER			0x84329583

#define NOT_STUCK_STS				0x00
#define STUCK_ON_STS				0x01
#define STUCK_OFF_STS				0x02
#define STUCK_OSC_STS				0x03

#define POWER_NUM					2

#define DEFAULT_FAN_TEMP			(30)
#define DEFAULT_HT_TEMP				(5)
#define RELAY_OFF					(0)
#define RELAY_ON					(1)

#define TEMPER1						(0)
#define TEMPER2						(1)
//-------------------------------------------------------------------------------------------
// Log ERROR CODE
//------------------------------------------------------------------------------------------
#define START_PROC					201
#define ABORT_NET_NOPING			202
#define ABORT_NET_VALIDSETION		203
#define ABORT_NET_NOCSN				204
#define CONNECT_SERVER				205
#define NET_COLSE_SESSION			206
#define NET_COLSE_SERVER			207
#define IOBOARD_COMM_ERROR			208
#define LDBOARD_COMM_ERROR			209
#define PARAMETER_UPDATE			210
#define NETCONFIG_UPDATE			211
//#define RESERVE1					212
//#define RESERVE2					213
//#define RESERVE3					214
// kwj insert. 2017.07.19
#define WATCHDOG_RESET				212
#define SERVER_COMMAND_RESET		213
#define SERVER_COMMAND_SYSINIT		214
#define USERTERM_RESET				215
#define MAINTENANCE_RESET			216
#define POLL_CREATE_EXIT			217
#define POLL_CALLOC_EXIT			218
#define WEB_SERVER_REBOOT			219

//-------------------------------------------------------------------------------------------
// Saving Data Buffer define
//------------------------------------------------------------------------------------------
#define MAX_SAVE_RT_DATA			15000				//2000 -> 15000 	by capidra
#define MAX_SAVE_RT_DATA_TCP		10000				//2000 -> 15000 	by capidra
#define MAX_SAVE_PL_DATA			400					//200 -> 400
#define MAX_SAVING_DATA_BUF			2048
#define MAX_SAVE_RT_DATA_NAND		30000			//by capi 2014.08

//-------------------------------------------------------------------------------------------
// Web Server by wook
//-------------------------------------------------------------------------------------------
#define RTD_WEB_TEXT_PATH			"/root/am1808/boa/html/rtdata/rtdata.txt"
#define RTD_WEB_FIFO_PATH			"/root/am1808/boa/fifo/fifo"
#define RTD_ON						(1)
#define RTD_OFF						(0)
#define RTD_WEB_TIMER				(5) // min
#define FIFO_MSG_SIZE				(30)
#define RTD_WEB_MAX_SIZE			(100000) // 100 kByte - 1200 EA
//#define RTD_WEB_MAX_SIZE			(300) // 4

// kwj insert 2017.1.23
#define INDV_REALTIME_DATA

#define MAX_LOOP_CHANNEL			4

//-------------------------------------------------------------------------------------------

/////////////////////////////////////////////////////////////////////
#include "vds_parameter.h"
/////////////////////////////////////////////////////////////////////

typedef struct _SYSTEMTIME {
	unsigned int wYear;
	unsigned int wMonth;
	unsigned int wDayOfWeek;
	unsigned int wDay;
	unsigned int wHour;
	unsigned int wMinute;
	unsigned int wSecond;
	unsigned int wMilliseconds;
} SYSTEMTIME;

typedef struct {
	uint8 year;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec;
	uint16 msec;
} SYSTIME_TINY;

typedef struct {
	uint8 year;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec;
	uint16 msec;
} SYSTIME_TINY_PACK_t;

typedef struct {
	uint8 year_hi;
	uint8 year_lo;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec;
} TIME_TAG_t;

// Traffic Structure.
typedef struct {
	TIME_TAG_t ptime;
	TIME_TAG_t ctime;
	uint32 speed[MAX_LANE_NUM];		// speed
	uint32 length[MAX_LANE_NUM];	// length.
	uint32 volume[MAX_LOOP_NUM];	// volumn count
	uint32 occupy[MAX_LOOP_NUM];	// occ time
} TRAFFIC_ST1;

typedef struct{
	uint8 Start,End;
	uint8 Data[MAX_MODEM_DATA_QUEUE_NO][MAX_MODEM_DATA_LENGTH];
	uint16 Length[MAX_MODEM_DATA_QUEUE_NO];
} MODEM_DATA;

typedef struct COMM_CONFIG_DEF {
	uint8 flow_cntrl;
	uint8 bps;
	//uint16 sync;
	uint16 local_poll;		// local polling time.
} COMM_CONFIG_t;

// system set up data. old version
typedef struct SYS_CONFIG_OLD_DEF {
	uint32 magic_num;
	COMM_CONFIG_t comm_cfg;
	VDS_PARAM_ST param_cfg;
	uint8 stuck_thrh;
	uint8 myaddr[2];
	uint8 temper[2];
	uint8 is_default_param;
	uint32 password[4];
} SYS_CONFIG_OLD_t;

typedef struct {
	uint16 speed_category[SPEED_CATEGORY_NO];
	uint16 length_category[LENGTH_CATEGORY_NO];
} REPORT_PER_LANE;

// 1 Lane mapping
typedef struct {
	uint16 s_loop;	// start loop
	uint16 e_loop;	// end loop
} LANE_LINK;


typedef struct {
	uint16 lane;
	uint8 pos;
	uint8 used;
} LOOP_TO_LANE_t;

//Realtime Nand Saving Struct by capi 2014.08
typedef struct {
	SYSTIME_TINY time;
	uint8 	lane;
	uint16 	length;
	uint8 	inverse;
	uint32 	speed;
	uint16 	occupy[2];
} REALTIME_NAND_DATA_T;

//Realtime traffic Data saving struct
typedef struct {
	SYSTIME_TINY time;
	uint8 valid;
	uint8 lane;
	uint16 length;
	uint16 speed;
	uint16 volume;
	uint16 occupy[2];
} REALTIME_DATA_T;

typedef  struct {
	SYSTIME_TINY_PACK_t time;
	uint8 valid;
	uint8 lane;
	uint8 length[2];
	uint8 speed[2];
	uint8 volume[2];
	uint8 occupy[2][2];
} REALTIME_DATA_PACK_t;

// Polling traffic Data saving struct
typedef struct {
	SYSTIME_TINY time;
	uint8 valid;
	uint8 frame_num;
	//uint8 frame_num_opt;
	uint16 status;
	uint8 stuck_per_loop[MAX_LOOP_NUM/4];
	uint8 incid_per_loop[MAX_LOOP_NUM/8];
	uint16 volume[MAX_LOOP_NUM];
	uint16 occupy[MAX_LOOP_NUM];
	uint16 speed[MAX_LANE_NUM];
	uint16 length[MAX_LANE_NUM];
} POLLING_DATA_T;

typedef struct {
	SYSTIME_TINY_PACK_t time;
	uint8 valid;
	uint8 frame_num;
	//uint8 frame_num_opt;
	uint16 status;
	uint8 stuck_per_loop[MAX_LOOP_NUM/4];
	uint8 incid_per_loop[MAX_LOOP_NUM/8];
	uint16 volume[MAX_LOOP_NUM];
	uint16 occupy[MAX_LOOP_NUM];
	uint16 speed[MAX_LANE_NUM];
	uint16 length[MAX_LANE_NUM];
} POLLING_DATA_PACK_t;

typedef struct {
	uint16 volume;
} REPORT_PER_LOOP;

//------------------------------------------------------
// LD Board Information Structure
//------------------------------------------------------
typedef struct {
	uchar BoardId;						/* Board ID */
	uchar FreqSel[MAX_LOOP_CHANNEL];	/* freq select */
	uchar FreqLevel[MAX_LOOP_CHANNEL];	/* sensor Level */
	uchar DetMode[MAX_LOOP_CHANNEL];	/* PR, PL */
	uchar SetMode;						/* set Mode - Console, DIP SW */
	uchar LoopStatus[MAX_LOOP_CHANNEL];	/* normal, Open, Short, 25% */
	uchar OccTime;						/* Ocuupy Time */
	uchar DetFail;						/* Fail Check - enable, able */
} DetInfoStruct;
DetInfoStruct DetInfo[5];

//------------------------------------------------------
// by wook 2013.02.18 diagnosis Program - ethernet status check
//------------------------------------------------------
typedef struct{
	int hw_link;
	int svr_link;
	int last_opc;
	int temp[2];
} DIA_CPU_CHECK;
DIA_CPU_CHECK dia_status;

//------------------------------------------------------
// by wook 2017.05.16 Web Sertver - Status Check
//------------------------------------------------------
typedef struct{
	// IO Status
	uint8 dle;
	uint8 stx;
	uint8 grb_addr[2];
	uint8 ctrl_addr[2];
	uint8 opcode;
	uint16 status;
} WEB_SERVER_STATUS_HEAD;

typedef struct{
	WEB_SERVER_STATUS_HEAD head;
	// Ver. Info.
	uint8 hw_version;
	uint8 sw_version;
	uint8 manu_year[2];
	uint8 manu_month;
	uint8 manu_day;
	// Net Info.
	uint8 last_opc;
	uint8 lc_year[2];
	uint8 lc_month;
	uint8 lc_day;
	uint8 lc_hour;
	uint8 lc_min;
	uint8 lc_sec;
	// Temperature
	uint8 temp[2];
	// Power Status
	uint8 pw[2];
	uint8 LDBNum;
	DetInfoStruct DetInfo[5];
} WEB_SERVER_STATUS;
WEB_SERVER_STATUS web_status;

typedef struct {
	uint32 magic_num;					// Magic number of config.
	u8 mac[MAC_ADDR_LEN];				// MAC Address
	u8 ip[IP_NUM_LEN];					// IP Address
	u8 gip[IP_NUM_LEN];					// Gateway IP
	u8 nmsk[IP_NUM_LEN];				// Subnet mask
	u8 server_ip[IP_NUM_LEN];			// Server IP
	u16 server_port;					// Server Port.
	u8 station_num[STATION_NUM_LEN];	// Station Number.
	//char tunnel_id[TUNNEL_ID_STR_LEN];	// Tunnel ID. by capi 2014.08
	//char controller_id[CONTROLLER_ID_STR_LEN];	// Controller ID.
} NETWORK_CONFIG_SAVED_t;

//
typedef struct{
	SYSTEMTIME sys_time;
	SYSTEMTIME boot_stamp;
	SYSTEMTIME prehalt_time;
	SYSTEMTIME vds_server_stamp;
	u32 pwr_vol[2];					//input DC Power check
	U8 main_sensor;
	int temper[2];					//current temperature
	int temp_buf;
	int temp_buf_1day[2];
	int temp_save_time;
	int temp_init;
	int temp_error;
	uint8 temp_status;
	int temp_24hour_buf[24];
	int timecnt;
	int relay_state[4];				//relay on/off state
	int relay_sw_delay[4];			//preventing operation of the relay is repeated indefinitely
} FTMS_SYS_STATUS_T;

struct vdsd_state {
	char state;						// state machine

	//u16_t send_len;
	//u16_t send_ptr;				// 보낼 데이터 포인터.
	//u16_t send_left;				// 보낼 데이터 개수.
	unsigned long send_ptr;			// 보낼 데이터 포인터.		2013.01.24 by capi
	unsigned long send_left;		// 보낼 데이터 개수.

	//unsigned char timer;
	unsigned int timer;				// bug fix by jwank 091104
	short wait;	// ms				//
	int  con_state;
};

struct vdsd_state vds_st;

//------------------------------------------------------
// by wook 2013.02.18 진단 프로그램 최통 통신 패킷 확인용
uint16 vds_tx_dump_len;
unsigned char vds_tx_dump[240];
//------------------------------------------------------

////////////////////////////////////////////////////////////////////////////////////////////
////////////// Global Variable ///////////////////////////////////////////

SYSTEMTIME sys_time;
SYSTEMTIME boot_stamp;

SYSTEMTIME vds_server_stamp;
SYSTEMTIME web_server_time;

SYS_CONFIG_OLD_t gstSysConfig;

//-------------------------

NETWORK_CONFIG_SAVED_t gstNetConfig;
FTMS_SYS_STATUS_T g_currSysStatus;

//web server File Descriptor. by wook 2015.11.24
int fd_w;

// realtime data file pointer
int rt_fd;
int power1_fd;		//2014.09.25 by capi
int power2_fd;		//2014.09.25 by capi
int power3_fd;		//2014.09.25 by capi
int power4_fd;		//2014.09.25 by capi

//Nor Flash file pointer
int nor_fd;		//2014.09.25 by capi

uint8 gucActiveDualLoopNum;
uint8 gucActiveSingleLoopNum;
uint8 gucActiveLoopNum;
uint8 gucActiveLDBNum;	//2014.10.13 by capi
uint8 gucDivDualLoopLane[MAX_LANE_NUM];

u_int revLDB_ID[8];	//2014.10.13
u_int revLDB_cnt[8];	//2014.10.13

u_int stuckon_counter[MAX_LOOP_NUM];	//2011.06.27 by capidra
u_int stuckoff_counter[MAX_LOOP_NUM];

#if defined(_LDBOARD_CHECK)
 u_int Detectorcounter[MAX_LOOP_NUM/4];
 uint8 DetectorStatus[MAX_LOOP_NUM/4];
 #endif

u_int watchdog_counter;
u_int global_counter;
u_int centerPollCounter;		// Center Polling
u_int incident_counter;

unsigned int loop_count;
int max_msec_count;

u_int keepup_incident[MAX_LOOP_NUM];
u_int pre_stuck_count;

u_int validsession_counter;		//2011.07.05 by capidra
u_int validsession_flag;          //insert by capidra 2011.07.01

u_int host_req_counter;		//

u_int centerAutosyncCounter;

u_int vds_comm_counter;
u_int time_read_counter;
u_int io_time_counter;		// time of i/o board
u_int monitor_counter;		//temperature, power monitoring period
u_int saving_send_counter;
u_int modem_ack_counter;

u_int check_link_counter;

uint8 isReqForceReset;
//uint8 check_iocomm;

unsigned char gucMacAddr;

//by capi 2014.08
u_int localPollCounter;
u_int localAutosyncCounter;

//saving data
REALTIME_DATA_T gstSaveRTdataPool[MAX_SAVE_RT_DATA];

POLLING_DATA_T gstSavePLdataPool[MAX_SAVE_PL_DATA];

// nand에 savefolder에 시간대별 이벤트 저장
REALTIME_NAND_DATA_T gstSaveRTdataNAND[2][MAX_SAVE_RT_DATA_NAND];  //by capi 2014.08

uint8 nd_bank, savendBank;
uint32 rt_widx, rt_data_num;
uint32 pl_widx, pl_data_num;
uint32 rt_nand_widx, rt_nand_data_num;	//by capi 2014.08
uint32 lastest_pl_idx;
uint32 lastest_rt_idx[MAX_LANE_NUM];

uint32 read_idx, read_num;
uint32 read_nd_idx, read_nd_num;

// by wook 2013.02.18 diagnosis Program
DIA_CPU_CHECK dia_status;
uint16 vds_tx_dump_len;
unsigned char vds_tx_dump[240];
//-------------------------------------------
//uint8 inverse_flag;
//uint8 inverse_time;
//u_int inverse_cnt;

//-------------------------------------------------------------------------------------------
// Web Server by wook 2016.02.26
//-------------------------------------------------------------------------------------------
int rt_web_fd;		// RT Text File Descriptor
int fifo_web_fd;	// FIFO File Descriptor
int rt_web_flag;		// RT Data start/stop flag
int rt_web_counter;
int st_web_flag;

// relay fan, heater
int fan_fd, heater_fd;

#endif /* __FTMS_CPU_H__ */
