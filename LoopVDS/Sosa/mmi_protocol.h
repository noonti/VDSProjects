
#ifndef __MMI_PROTOCOL_H__
#define __MMI_PROTOCOL_H__


//#include "ftms_cpu.h"

//////////////////////////////////////////////////////////////////////////////
// Opcode definition.   CPM --> MMI
// CPM 에서 MMI 로 보내는 Opcode
#define OP_CPM_STARTED				('s')	
#define OP_POLLING_DATA				('P')	// Polling Data Display
#define OP_REALTIME_DATA			('R')	// Realtime Data Display
#define OP_ALIVE_CPM				('l')
#define OP_DATE_SET					('d')	// Date Set
#define OP_TIME_SET					('t')	// Time Set
#define OP_COMM_CFG					('c')	
#define OP_REV_ACK					('A')
#define OP_FORCE_RESET				('r')

#define OP_NET_CONFIG				('N')	
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
// Index for OP_NET_CONFIG
#define MMI_CFG_IDX_01_IP_ADDR				(1)
#define MMI_CFG_IDX_02_GATEWAY_IP			(2)
#define MMI_CFG_IDX_03_SUBN_MSK				(3)
#define MMI_CFG_IDX_04_SERVER_IP			(4)
#define MMI_CFG_IDX_05_SERVER_PORT			(5)
#define MMI_CFG_IDX_06_STATION_NUM			(6)
#define MMI_CFG_IDX_07_TUNNEL_ID			(7)
#define MMI_CFG_IDX_08_CONTR_ID				(8)

#define MMI_CFG_IDX_99_ALL_NET_CFG			(99)
//////////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////////
// Opcode definition.   MMI --> CPM
#define OP_INITIALIZED				('I')	
#define OP_ALIVE_MMI				('L')
#define OP_TIME_CHG					('T')	
#define OP_FLOW_CTRL				('F')	
#define OP_VDS_BPS					('B')	
#define OP_SYNC_TIME				('S')	
#define OP_MAX_LOOP					('M')	
#define OP_LOOP_GAP					('G')	
#define OP_ID_ADDR					('A')	

#define OP_NORMAL_MODE				('N')	
#define OP_POLLING_MODE				('P')	
#define OP_REALTIME_MODE			('R')	

#define OP_REQ_TIME					('q')	

#define OP_REQ_TEMP					('H')	
#define OP_REQ_PASS					('W')	

#define OP_CHG_NET_CONFIG			('Z')	

#define OP_CHG_IP_IP_ADDR			(OP_CHG_NET_CONFIG + 1)
#define OP_CHG_IP_GATEWAY_IP		(OP_CHG_NET_CONFIG + 2)
#define OP_CHG_IP_SUBN_MSK			(OP_CHG_NET_CONFIG + 3)
#define OP_CHG_IP_SERVER_IP			(OP_CHG_NET_CONFIG + 4)
#define OP_CHG_IP_SERVER_PORT		(OP_CHG_NET_CONFIG + 5)
#define OP_CHG_IP_STATION_NUM		(OP_CHG_NET_CONFIG + 6)
#define OP_CHG_IP_TUNNEL_ID			(OP_CHG_NET_CONFIG + 7)
#define OP_CHG_IP_CONTR_ID			(OP_CHG_NET_CONFIG + 8)
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
// Terminal Character
#define _END_CH_					0x0d
//////////////////////////////////////////////////////////////////////////////

// MMI Init message state-machine.
typedef enum{
	PRE_INIT_STS = 0x60,
	RECVED_INIT_MSG,
	SEND_DATE,
	SEND_TIME,
	SEND_COMM_CFG,
#if 1//defined(FTMS_TCP_APP_OPERATION)
	SEND_IP_ADDRESS,
	SEND_GATEWAY_IP,
	SEND_SUBNET_SMK,
	SEND_SERVER_IP,
	SEND_SERVER_PORT,
	SEND_STATION_NUM,
	SEND_TUNNEL_ID,
	SEND_CONTROL_ID,
#endif
	SEND_FINISHED
} MMI_STATE_MACH;


#pragma pack(1)
typedef  struct {
	uint8 opcode;			// OPCODE
	uint8 index;			
	uint8 volume;			
	uint8 occupy;			
	uint8 speed;			
	uint8 length;			
	uint8 end_ch;			// END
} POLLING_DATA_FOR_MMI;
#pragma pack()


#pragma pack(1)
typedef struct {
	uint8 opcode;			// OPCODE
	uint8 index;			
	uint8 rt_type;			
	uint8 volume;			
	uint8 occupy[2][2];		
	uint8 end_ch;			// END
} RT_DATA_T0_FOR_MMI;
#pragma pack()


#pragma pack(1)
typedef struct {
	uint8 opcode;			// OPCODE
	uint8 index;			
	uint8 rt_type;			
	uint8 speed;			
	uint8 length;			
	uint8 end_ch;			// END
} RT_DATA_T1_FOR_MMI;
#pragma pack()


#pragma pack(1)
typedef struct {
	uint8 opcode;			// OPCODE
	uint8 index;			
	uint8 rt_type;			
	uint8 acc_vol[2][2];	
	uint8 end_ch;			// END
} RT_DATA_T2_FOR_MMI;
#pragma pack()


#endif // #ifndef __MMI_PROTOCOL_H__

