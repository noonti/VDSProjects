#ifndef __SERIAL_H__
#define __SERIAL_H__

// DCS9 serial.h, Sys_comm.h

// State machine for Serial bus protocol.
#define	MODEM_WAIT_START_1		(0x01)
#define	MODEM_WAIT_START_2		(0x02)
#define	MODEM_WAIT_END_1		(0x10)
#define	MODEM_WAIT_END_2		(0x11)
#define	MODEM_WAIT_CHECKSUM_1		(0x30)
#define	MODEM_WAIT_CHECKSUM_2		(0x31)

#define EVT_BUFF_SIZE			8000

#define PRESS_TIME			0
#define REL_TIME			1
#define INVALID_TIME			0xffffffff

#define USED				0x40
#define UNUSED				0x41

#define PRESSED				0x20
#define RELEASED			0x21
#define START_POS			0
#define END_POS				1
#define IN_STS					0x30	
#define EXIT_STS				0x31	
#define REVERSE_IN_STS			0x39	

#define ERROR_STS			0x40	//
#define REVERSE_ERR_STS			0x41	//
#define IN_RVS				100			
#define REAL_IO_POINT_MSK		0x00ffffff

//Result Code
#define _SYS_VALID_			0
#define _SYS_INVALID_			1
#define _OPCODE_INVALID_		2
#define _CTRL_STATION_NUM_ERR		3
#define _DATA_LENGTH_ERR		4
#define _NOT_DEFINE_VALUE		5
#define _DATA_NOT_RDY_MSK		6
#define _TRANSACTION_TIMEOUT		7

// Opcode def. from RIO.
#define OP_LOOP_EVENT			0x70
#define OP_RIO_RESTART			0x71

void init_console(void);

#pragma pack(1)
typedef  struct {
	uint8 opcode;
	uint8 counter[3];
	uint32 loop_val;
} EVENT_TYPE2_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	uint8 opcode;
	uint32 loop_val;
} RIO_RESTART_PACKET;
#pragma pack()

typedef struct {
	uint32 evt_time;
	uint32 loop_val;
} EVENT_ST;

typedef struct {
	EVENT_ST buff[EVT_BUFF_SIZE];
	uint32 rd_ptr;
	uint32 wr_ptr;
} EVENT_BUFF;

// Status Structure.
typedef struct {
	uint32 time[2];		// 0: Start pos. 1: End pos.
	uint8 status;
} LOOP_STATUS;

typedef struct {
	uint32 	time[2];		// 0: Start pos. 1: End pos.
	uint32 	intime[2];		// 0: Start pos. 1: End pos.
	uint8 	status;
	uint32 	delta_ptime;		
	uint32 	delta_rtime;		
} LANE_STATUS;

typedef struct {
	uint32 	inv_rate;		
	uint32 	din;		
	uint8 	dout;
	uint8 	inverse_flag;
	uint8 	inverse_time;			// response time after sending inverse data
	u_int 	inverse_cnt;
	BOOL 	start_inv_flag;			//2015.01.07 by capi
	BOOL 	occ_state_with_inv[MAX_LANE_NUM];
	uint8 	gRT_Reverse[MAX_LANE_NUM];
	uint8 	gRT_Reverse_retrans[MAX_LANE_NUM];
} INVERS_STATUS;


//시스템 상태정보
typedef struct {	
	uint8 long_pwr_fail;				//
	uint8 short_pwr_fail;	
	uint8 default_param;
	uint8 recv_broadcast_msg;
	uint8 auto_resync;
	uint8 front_door_open;
	uint8 rear_door_open;
	uint8 fan_operate;				
	uint8 heater_operate;
	uint8 controller_reset;	
} SYSTEM_STATUS_T;

typedef struct {
	uint8 realtime_clock;
	uint8 diom;
	uint8 loop_det_amp[4];
	uint8 ultrasonic_det[2];
	uint8 pwr_supply[2][2];
	uint8 rom;
	uint8 ram;
	uint8 rev[2];
} SYSTEM_VALID_T;

/////////////////////////////////////////////////////////////////


EVENT_BUFF evt_buff;
LOOP_STATUS loop_st[MAX_LOOP_NUM];
LANE_STATUS lane_st[MAX_LANE_NUM];
INVERS_STATUS invers_st;		//2015.07.15 by capi inverse Driving


LANE_LINK gstLaneLink[MAX_LANE_NUM];
LANE_LINK gstLaneLink[MAX_LANE_NUM];


TRAFFIC_ST1 gstCenterAccuData;
TRAFFIC_ST1 gstIncidentAccuData;

TRAFFIC_ST1 gstCenterPolling;
TRAFFIC_ST1 gstIncidentPolling;


REPORT_PER_LOOP gstReportsOfLoop[MAX_LOOP_NUM];
REPORT_PER_LANE gstReportsOfLane[MAX_LANE_NUM];

//by capi 2014.08
#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)							
TRAFFIC_ST1 gstLocalAccuData;
TRAFFIC_ST1 gstLocalPolling;
#endif

//BOOL occ_state_with_inv[MAX_LANE_NUM];
uint32 s_time[MAX_LANE_NUM], delta_time[MAX_LANE_NUM]; //2014.09.02 by capi

//REPORT_PER_LOOP gstReportsOfLoop[MAX_LOOP_NUM];
//REPORT_PER_LANE gstReportsOfLane[MAX_LANE_NUM];

LOOP_TO_LANE_t lp2ln[MAX_LOOP_NUM];
TRAFFIC_ST1 gstCurrData;
SYSTEM_STATUS_T system_status;
SYSTEM_VALID_T system_valid;


u_int sys_comm_counter;
uint32 pre_count, pre_value;
uint32 cur_count, cur_value;
uint32 cur_non_mask_val;
uint32 loop_enable_msk;
uint32 loop_diam[MAX_LANE_NUM];
uint32 lane_dist[MAX_LANE_NUM];

uint8 lane_evt_flg[MAX_LANE_NUM];
//uint8 gRT_Reverse[MAX_LANE_NUM];
uint16 gCountVehicle;
uint8 realtime_data_valid[MAX_RIO_LANE_NUM];
uint32 lane_volume[MAX_LANE_NUM];
uint32 occupy[MAX_LANE_NUM];
uint32 acc_speed[MAX_LANE_NUM];

uint8 gucStuckPerLoop[MAX_LOOP_NUM/4];

extern int fd_ttys1;


#endif /* __SERIAL_H__ */

