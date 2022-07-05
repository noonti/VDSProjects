#ifndef __MAINTENACE_PROTOCOL_H__
#define __MAINTENACE_PROTOCOL_H__


//#include "ftms_cpu.h"

// DLE protocol definition.
#define DLE							0x10
#define STX							0x02
#define ETX							0x03


//Maintenance Program OPCODE
#define CMD_SET_SYSTEM_CONFIG		0x01	// system param save
#define CMD_GET_SYSTEM_CONFIG		0x0C	// system param load

#define CMD_RSPND_SET_SYS_CFG		0x16	// 22 ==> CMD_SET_SYSTEM_CONFIG 에 대한 응답.

#define CMD_PARAMETER_DOWNLOAD		0x02	// Traffic Param save
#define CMD_PARAMETER_UPLOAD		0x03	// Traffic Param Load

#define CMD_RSPND_DOWN_PARAM		0x0E	// 14 ==> CMD_PARAMETER_DOWNLOAD 에 대한 응답.

#define CMD_CONTROLLER_VALID		0x04	
#define CMD_CONTROLLER_RESET		0x05	
#define CMD_CONTROLLER_INIT			0x06	

#define CMD_INDIVIDUAL_DATA_REQ		0x07	
#define CMD_LOCAL_POLL_DATA_REQ		0x08	
#define CMD_CENTER_POLL_DATA_REQ	0x09	
#define CMD_SAVED_TRAFFIC_DATA_REQ	0x0A	

#define CMD_LOOP_STATUS_REQ			0x0B	

#define CMD_TIME_STAMP_REQ			0x0F	

#define CMD_SET_SYSTEM_CONFIG_B		0x1C	// eeee
#define CMD_GET_SYSTEM_CONFIG_B		0x1D	// eeee

#define CMD_SET_SYSTEM_MACADDRESS	0x30	// eeee		
#define CMD_GET_SYSTEM_MACADDRESS	0x31	// eeee		
#define CMD_SET_SYSTEM_TEMP			0x32	// eeee		
#define CMD_GET_SYSTEM_TEMP			0x33	// eeee		

#define CMD_GET_SYSTEM_VERSION		0xF0	// eeee		
#define CMD_SET_MAC_ADDRESS			0xF1	//			//Mac Add
#define CMD_GET_MAC_ADDRESS			0xF2	//
#define CMD_SET_TEMP				0xF3
#define CMD_GET_TEMP				0xF4
#define CMD_GET_CUR_TEMP			0xF5
#define CMD_SET_CUR_TIME			0x14	//0xF6->0x16 by kwj

// 진단프로그램
#define OP_SERVER_STATUS_CHK		0xC0	// 서버 접속 상태 확인
#define OP_IO_STATUS_CHK			0xC1	// I/O 보드와의 통신 상태 확인
#define OP_TEMP_SENSOR_CHK			0xC2	// 온도센서 READ
#define OP_RELAY_CHK				0xC3	// 릴레이 동작 ON/OFF
#define OP_PACKET_CHK				0xC4	// 센터 송,수신 패킷 확인 

// Sub-Opcode of CMD_SAVED_TRAFFIC_DATA_REQ
#define SUBCMD_QUERY_SAVED_DATA		0x01	//
#define SUBCMD_NO_SAVED_DATA		0x02	//
#define SUBCMD_REQ_REALTIME_DATA	0x03	//
#define SUBCMD_REQ_POLLING_DATA		0x04	//
#define SUBCMD_REALTIME_DATA		0x05	//
#define SUBCMD_POLLING_DATA			0x06	//
/////////////////////////////////////////////////////////////////////

#define _ALL_LANE_					0xEE

// Host PC -> Controller
// CMD_SET_SYSTEM_CONFIG
#pragma pack(1)
typedef  struct {
	PACKET_TINY_HEAD head;
	uint8 index;
	uint8 config[15];
} SYSTEM_CONFIG_PACKET;
#pragma pack()

// Controller -> Host PC
// CMD_SAVED_TRAFFIC_DATA_REQ-SUBCMD_REALTIME_DATA
#pragma pack(1)
typedef  struct {
	PACKET_TINY2_HEAD head;
	uint8 sub_op;
	uint8 seq_num;
	REALTIME_DATA_PACK_t data;
	PACKET_TAIL tail;
} SAVING_INDIV_DATA_RSP_PACKET;
#pragma pack()

// Controller -> Host PC
// CMD_SAVED_TRAFFIC_DATA_REQ-SUBCMD_POLLING_DATA
#pragma pack(1)
typedef  struct {
	PACKET_TINY2_HEAD head;
	uint8 sub_op;
	uint8 seq_num;
	POLLING_DATA_PACK_t data;
	PACKET_TAIL tail;
} SAVING_CENTER_DATA_RSP_PACKET;
#pragma pack()


#pragma pack(1)
typedef  struct {
	PACKET_RESPOND_HEAD head;
	uint8 hw_version;
	uint8 sw_version;
	uint8 manu_year[2];
	uint8 manu_month;
	uint8 manu_day;	
	PACKET_TAIL tail;
} VERSION_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8						new_mac[MAC_ADDR_LEN];	
} MACADDRESS_REQ_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8						mac[MAC_ADDR_LEN];	
	PACKET_TAIL tail;
} MACADDRESS_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8						config[7];			
} SYSTEM_TIME_REQ_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8	temp[2];		
} SET_TEMPER_REQ_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint16 valid_data;
	PACKET_TAIL tail;
} CNTRL_VALID_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8	temp[2];	
	uint8	curtemp[2];
	uint8	divi;
	PACKET_TAIL tail;
} GET_TEMPER_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8 index;
	PACKET_TAIL tail;
} RESPND_PARAM_DOWN_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	uint8 year_hi;
	uint8 year_lo;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec_hi;
	uint8 sec_lo;
} TIME_TAG_ST;
#pragma pack()

#pragma pack(1)
typedef struct {
	uint8 year_hi;
	uint8 year_lo;
	uint8 month;
	uint8 day;
	#pragma pack(1)
	 struct {
		uint8 hour;
		uint8 min;
		uint8 sec;
	} s;
	#pragma pack()

	#pragma pack(1)
	struct {
		uint8 hour;
		uint8 min;
		uint8 sec;
	} e;
	#pragma pack()	
} TIME_TAG2_ST;
#pragma pack()

#pragma pack(1)
typedef struct {
	TIME_TAG_ST time;
	uint8 lane;
	uint8 spd_hi;
	uint8 spd_md;
	uint8 spd_lo;
	uint8 lp1_oc_hi;
	uint8 lp1_oc_lo;
	uint8 lp2_oc_hi;
	uint8 lp2_oc_lo;
	uint8 len_hi;
	uint8 len_lo;
} REALTIME_VENHICLE_ST;
#pragma pack()


#pragma pack(1)
typedef struct {
	uint8 lane;
	uint8 vol;
	uint8 spd_hi;
	uint8 spd_lo;
	uint8 occupy;
	uint8 len_hi;
	uint8 len_lo;
} LOCAL_POLL_DATA_ST;
#pragma pack()

#pragma pack(1)
typedef struct {
	uint8 lane;
	uint8 lp1_vol;
	uint8 lp2_vol;
	uint8 spd_hi;
	uint8 spd_lo;
	uint8 lp1_oc;
	uint8 lp2_oc;
	uint8 len_hi;
	uint8 len_lo;
} CENTER_POLL_DATA_ST;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8 lane_no;
	uint8 on_off;
} INDIV_DATA_REQ_PKT;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	REALTIME_VENHICLE_ST data;
	PACKET_TAIL tail;
} INDIV_DATA_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8 lane_no[2];
	uint8 on_off;
} POLL_DATA_REQ_PKT;
#pragma pack()

// CMD_LOCAL_POLL_DATA_REQ
#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	TIME_TAG2_ST time;
	LOCAL_POLL_DATA_ST data;
	PACKET_TAIL tail;
} LOCAL_DATA_RSP_PACKET;
#pragma pack()

// Controller -> Host PC
// CMD_CENTER_POLL_DATA_REQ
#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	TIME_TAG2_ST time;
	CENTER_POLL_DATA_ST data;
	PACKET_TAIL tail;
} CENTER_DATA_RSP_PACKET;
#pragma pack()

// Controller -> Host PC
// CMD_LOCAL_POLL_DATA_REQ
#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	TIME_TAG2_ST time;
	LOCAL_POLL_DATA_ST data[MAX_RIO_LANE_NUM];
	PACKET_TAIL tail;
} ALL_LOCAL_DATA_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	TIME_TAG2_ST time;
	CENTER_POLL_DATA_ST data[MAX_RIO_LANE_NUM];
	PACKET_TAIL tail;
} ALL_CENTER_DATA_RSP_PACKET;
#pragma pack()

// Controller -> Host PC
// CMD_SAVED_TRAFFIC_DATA_REQ-SUBCMD_NO_SAVED_DATA
#pragma pack(1)
typedef struct {
	PACKET_TINY2_HEAD head;
	uint8 sub_op;
	PACKET_TAIL tail;
} SAVING_DATA_RSP_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8 sub_op;
	#pragma pack(1)
		union {
			uint8 on_off;
			TIME_TAG_ST time;
		} d;	
	#pragma pack()	
} SAVED_TAFFIC_DATA_REQ_PKT;
#pragma pack()

//------------------------------------------------------
// by wook 2013.02.18 진단 프로그램 구조체
//------------------------------------------------------
// 서버 접속 상태 확인
#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8 hw_link;
	uint8 svr_link;
	uint8 last_opc;
	uint8 year[2];
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec;
	PACKET_TAIL tail;
} DIA_CONNSTATUS_RSP_PACKET;
#pragma pack()

// I/O 상태 확인
#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	//uint16 status;
	uint8 pw[2];
	PACKET_TAIL tail;
} DIA_IOSTATUS_RSP_PACKET;
#pragma pack()

// 온도센서 READ
#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8 temp[2];
	PACKET_TAIL tail;
} DIA_TEMPSTATUS_RSP_PACKET;
#pragma pack()

// 센터 송신 패킷 확인
#pragma pack(1)
typedef struct {
	PACKET_RESPOND_HEAD head;
	uint8 len;
	uint8 packet[100];
	PACKET_TAIL tail;
} DIA_SVRPACKET_RSP_PACKET;
#pragma pack()

#endif	// __MAINTENACE_PROTOCOL_H__
