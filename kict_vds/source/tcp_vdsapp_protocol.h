#ifndef __TCP_VDSAPP_PROTOCOL_H__
#define __TCP_VDSAPP_PROTOCOL_H__

/////////////////////////////////////////////////////////////////////////////////////
#define TTMS_VDS_TCP_PROTOCOL		

/////////////////////////////////////////////////////////////////////////////////////
#define USE_REAL_VDS_DATA			// ddddd

/////////////////////////////////////////////////////////////////////////////////////
#define _SOH_CH_				(0x7E)
#define _HYPHEN_CH_				('-')
#define	_VDS_KIND_CH_0_				('V')
#define	_VDS_KIND_CH_1_				('D')				//retiutch by capidra

#if 0
#define MAC_ADDR_LEN				(6)
#define IP_STR_LEN				(15)
#define IP_NUM_LEN				(4)
#define STATION_NUM_LEN				(4)
#endif

// OPCODE
#define OP_REQ_CSN				(0xFF)
#define OP_REQ_SYNC				(0x01)

#define OP_REQ_VDS_DATA				(0x04)
#define OP_REQ_SPEED_DATA			(0x05)
#define OP_REQ_LENGTH_DATA			(0x06)
#define OP_REQ_VOLUME_DATA			(0x07)
#define OP_SEL_ERR_THRESHOLD			(0x08)

#define OP_REQ_VALIDATION			(0x0B)
#define OP_REQ_SYS_RESET			(0x0C)
#define OP_REQ_SYS_INIT				(0x0D)
#define OP_DOWNLOAD_PARAM			(0x0E)
#define OP_UPLOAD_PARAM				(0x0F)

#define OP_REQ_ONLINE_STS			(0x11)
#define OP_REQ_MEMORY_CHECK			(0x12)
#define OP_REQ_ECHO_MESSAGE			(0x13)
#define OP_SEQ_NUMBER				(0x14)
#define OP_REQ_VERSION_REQ			(0x15)

#define OP_REQ_INDIV_VDS_DATA			(0x16)
#define OP_VAILD_SESSION			(0x18)
#define OP_INVERS_VDS_DATA			(0x19)		//2014.09.02 by capi

#define OP_CHANGE_IP				(0x32)
#define OP_CHG_STATION_NUM			(0x33)
#define OP_CHANGE_MAC				(0x34)
#define OP_CHANGE_SUBN_MSK			(0x35)
#define OP_CHANGE_GATEWAY_IP			(0x36)
#define OP_CHANGE_SERVER_IP			(0x37)
#define OP_CHANGE_SERVER_PORT			(0x38)

#define OP_CHANGE_TUNNEL_ID			(0x39)
#define OP_CHANGE_CONTROLLER_ID			(0x3A)

// for RTC Time.
#define OP_GET_RTC_TIME				(0x40)
#define OP_SET_RTC_TIME				(0x41)

// for Temp.
#define OP_SET_TEMP				(0x42)
#define OP_GET_TEMP				(0x43)

// for power.
#define OP_POWER1				(0x44)
#define OP_POWER2				(0x45)

// by jwank 100407
#define OP_REQ_SYS_STATUS			(0x60)

#define OP_INDIVIDUAL_TCP			(0x88)
/////////////////////////////////////////////////////////////////
//----------------------------------------------------
//                    KICT OPCODE                                                  
//----------------------------------------------------
#define OP_REQUEST 		0xA1
#define OP_RESPONCE 	0xA2
#define OP_reREQUEST 	0xA3
#define OP_reRESPONCE	0xA4

#define	OP_DATA_SEND	0xB0
#define	OP_DATA_RES		0xB1
#define	OP_HIST_REQ		0xB2
#define	OP_HIST_RES		0xB3
#define	OP_DET_REQ		0xB8
#define	OP_DET_RES		0xB9
#define	OP_ECHO_REQ		0xBA
#define	OP_ECHO_RES		0xBB
#define	OP_STAT_REQ		0xBC
#define	OP_STAT_RES		0xBD
#define	OP_TIME_REQ		0xBE
#define	OP_TIME_RES		0xBF
///////////////////////////////////////////////////////////////
#if defined(TTMS_VDS_TCP_PROTOCOL)

///////////////////////////////////////////////////

#pragma pack(1)
typedef   struct {
	uint8 year_hi;
	uint8 year_lo;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec_hi;
	uint8 sec_lo;
} TIME_TAG_ST_1; 

#pragma pack()

#pragma pack(1)
typedef   struct {
	TIME_TAG_ST_1 time;
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
} REALTIME_VENHICLE_ST_o;
#pragma pack()

#pragma pack(1)
typedef   struct {
	uint8 opcode;
	uint8 lane;
	uint8 direct;
	TIME_TAG_ST_1 time;
	uint8 spd_hi;
	uint8 spd_lo;
	uint8 lp_oc_hi;
	uint8 lp_oc_lo;
	uint8 len_hi;
	uint8 len_lo;
	uint8 reserved ;
} REALTIME_VENHICLE_ST_1; //mdy
#pragma pack()

#pragma pack(1)
typedef   struct {
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];	
	u8_t	opcode;
	u8_t transnum[8];		//2011.05.27 insert by Capidra
} VDS_TCP_PACKET_HEADER_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	u8_t	ver;
	u8_t 	addr[8];
	u8_t	opcode;
	u8_t	len_hi;
	u8_t	len_lo;
} VDS_TCP_PACKET_HEADER_t_1;
#pragma pack()


#pragma pack(1)
typedef   struct {
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];
	u8_t	opcode;
	u8_t transnum[8];		//2011.05.27 insert by Capidra
	u8_t resultcode;		//2011.05.27 insert by Capidra
	u8_t	status[2];	
} VDS_TCP_PKT_RESP_HEAD_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	u8_t	ver;
	u8_t 	addr[8];
	u8_t	opcode;
	u8_t	 len_hi;
	u8_t 	len_lo;
} VDS_TCP_PKT_RESP_HEAD_t_1;
#pragma pack()

#pragma pack(1)
typedef   struct {		
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];	
	u8_t	opcode;
} VDS_VALID_SESSION_PACKET_t;
#pragma pack()

#else // #if defined(TTMS_VDS_TCP_PROTOCOL)

#pragma pack(1)
typedef   struct {
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];	
	u8_t	opcode;
	u8_t transnum[8];		//2011.05.27 insert by Capidra
} VDS_TCP_PACKET_HEADER_t;

#pragma pack()

#pragma pack(1)
typedef   struct {
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];
	u8_t	opcode;
	u8_t transnum[8];		//2011.05.27 insert by Capidra
	u8_t resultcode;		//2011.05.27 insert by Capidra
	u8_t	status[2];	
} VDS_TCP_PKT_RESP_HEAD_t;
#pragma pack()

#pragma pack(1)
typedef   struct {		
	char	sip[IP_STR_LEN];
	char	hyphen_1;
	char	dip[IP_STR_LEN];
	char	hyphen_2;
	char	kind[2];
	u8_t	snum[STATION_NUM_LEN];
	u8_t	tlen[4];	
	u8_t	opcode;
} VDS_VALID_SESSION_PACKET_t;

#pragma pack()


#endif // #if defined(TTMS_VDS_TCP_PROTOCOL)
//
///////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////

// for OP_REQ_SYNC
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				frame;
} SYNC_REQ_PACKET_t;
#pragma pack()

// for OP_REQ_SPEED_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				lane;
} SPEED_DATA_REQ_PACKET_t;
#pragma pack()

// Length data 
// for OP_REQ_LENGTH_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				lane;
} LENS_DATA_REQ_PACKET_t;
#pragma pack()

// OP_REQ_VOLUME_DATA 는 헤더만 있고 데이터는 없음.

// Error Threshold 변경 요구 패킷 구조체.
// for OP_SEL_ERR_THRESHOLD
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				threshold;
} SEL_ERR_THRESHOLD_REQ_PACKET_t;
#pragma pack()

// OP_REQ_VALIDATION 
// OP_REQ_SYS_RESET 
// OP_REQ_SYS_INIT

// Parameter Download
// for OP_DOWNLOAD_PARAM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				index;
	u8_t				param_base;
} DOWNLOAD_PARAM_REQ_PACKET_t;
#pragma pack()

// Parameter Upload
// for OP_UPLOAD_PARAM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				index;
} UPLOAD_PARAM_REQ_PACKET_t;
#pragma pack()

// OP_REQ_ONLINE_STS
// OP_REQ_MEMORY_CHECK 
// Echo Message 
// for OP_REQ_ECHO_MESSAGE
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				echo_msg[1];	
	// start_echo
} ECHO_MSG_REQ_PACKET_t;
#pragma pack()

// Sequence Number
// for OP_SEQ_NUMBER
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				start_seq;
	u8_t				count;
} SEQ_NUM_REQ_PACKET_t;
#pragma pack()

// OP_REQ_VERSION_REQ
// OP_REQ_INDIV_VDS_DATA
// Change IP Address
// for OP_CHANGE_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_ip[IP_STR_LEN];
} CHG_IP_REQ_PACKET_t;
#pragma pack()

// Change Station number
// for OP_CHG_STATION_NUM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_st_num[STATION_NUM_LEN];
} CHG_ST_NUM_REQ_PACKET_t;
#pragma pack()

// Change MAC Address
// for OP_CHANGE_MAC
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_mac[MAC_ADDR_LEN];
} CHG_MAC_REQ_PACKET_t;
#pragma pack()

// Change Subnet Mask
// for OP_CHANGE_SUBN_MSK
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_nmsk[IP_STR_LEN];
} CHG_SUBN_MSK_REQ_PACKET_t;
#pragma pack()

// Change Gateway IP
// for OP_CHANGE_GATEWAY_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_gip[IP_STR_LEN];
} CHG_GATEWAY_IP_REQ_PACKET_t;
#pragma pack()

// Change Server IP
// for OP_CHANGE_SERVER_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_server_ip[IP_STR_LEN];
} CHG_SERVER_IP_REQ_PACKET_t;
#pragma pack()

// Change Server TCP Port
// for OP_CHANGE_SERVER_PORT
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	u8_t				new_server_port[2];
} CHG_SERVER_PORT_REQ_PACKET_t;
#pragma pack()

// Change Tunnel ID
// for OP_CHANGE_TUNNEL_ID
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_tunnel_id[TUNNEL_ID_STR_LEN];
} CHG_TUNNEL_ID_REQ_PACKET_t;
#pragma pack()

// Change Controller ID
// for OP_CHANGE_CONTROLLER_ID
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	char				new_controller_id[CONTROLLER_ID_STR_LEN];
} CHG_CNTR_ID_REQ_PACKET_t;
#pragma pack()

// Pan, Heater
// for OP_SET_RTC_TIME
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;		
	u8_t				temp[2];		
} CHG_PAN_HEATER_REQ_PACKET_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;		
	u8_t				status;	
} CHG_POWER_REQ_PACKET_t;
#pragma pack()

///////////////////////////////////////////////
// for OP_CHANGE_SERVER_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
} INDIV_TEST_REQ_PACKET_t;

#pragma pack()


// OP_GET_RTC_TIME

#pragma pack(1)
typedef   struct {
	u8_t year_hi;
	u8_t year_lo;
	u8_t month;
	u8_t day;
	u8_t hour;
	u8_t min;
	u8_t sec;
} RTC_TIME_t;
#pragma pack()

// OP_SET_RTC_TIME
// for OP_SET_RTC_TIME
#pragma pack(1)
typedef   struct {
	VDS_TCP_PACKET_HEADER_t		head;
	RTC_TIME_t			new_time;
} SET_RTC_TIME_REQ_PACKET_t;
#pragma pack()

#if 0
#endif

///////////////////////////////////////////////////////////////
// CSN
// for OP_REQ_CSN
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				CSN[4];	
} CSN_RESP_PACKET_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t_1		head;		
	uint8 opcode;
	uint8 lane;
	uint8 direct;
	TIME_TAG_ST_1 time;	
	uint8 result ;	
} EVT_RESP_PACKET_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t_1		head;		
	uint8 opcode;
	uint8 echodata[100];
} ECHO_RESP_PACKET_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t_1		head;		
	uint8 opcode;
	uint8 flag;
} DET_RESP_PACKET_t;

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t_1		head;		
	uint8 opcode;
	TIME_TAG_ST_1 time;	
	uint8 status[2] ;	
} STAT_RESP_PACKET_t;

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t_1		head;		
	uint8 opcode;
	TIME_TAG_ST_1 time;	
} TIME_RESP_PACKET_t;

#pragma pack()
// VDS data
// for OP_REQ_VDS_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				frame;		
	u8_t				data[1];	
} VDS_DATA_RESP_PACKET_t;
#pragma pack()

//////////////////////////////////////////////////////////////////
// Controller -> Host PC
// OP_INDIVIDUAL_TCP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;
	REALTIME_VENHICLE_ST_o		data;
} INDIV_DATA_RSP_PACKET_t;
#pragma pack()
//////////////////////////////////////////////////////////////////

// Speed data
// for OP_REQ_SPEED_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				lane;		
	u8_t				speed[SPEED_CATEGORY_NO][2];	
} SPEED_DATA_RESP_PACKET_t;
#pragma pack()

// Length data
// for OP_REQ_LENGTH_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				lane;		
	u8_t						length[LENGTH_CATEGORY_NO][2];	
} LENS_DATA_RESP_PACKET_t;
#pragma pack()

// Volume data
// for OP_REQ_VOLUME_DATA
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				volume[MAX_LOOP_NUM][2];	
} VOL_DATA_RESP_PACKET_t;
#pragma pack()

// Error Threshold
// for OP_SEL_ERR_THRESHOLD
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				threshold;	
} SEL_ERR_THRESHOLD_RESP_PACKET_t;
#pragma pack()

// Controller Validation
// for OP_REQ_VALIDATION
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t pwr_device;
	u8_t pwr_valid;
	u8_t LD_device;
	u8_t LD_valid[2];	
} SYS_VALIDATION_RESP_PACKET_t;
#pragma pack()

// Controller Reset
// for OP_REQ_SYS_RESET
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
} SYS_RESET_RESP_PACKET_t;
#pragma pack()

// Controller Initialize
// for OP_REQ_SYS_INIT
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
} SYS_INIT_RESP_PACKET_t;
#pragma pack()

// Parameter Download
// for OP_DOWNLOAD_PARAM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
//	u8_t				index;
} DOWNLOAD_PARAM_RESP_PACKET_t;
#pragma pack()

// Parameter Upload
// for OP_UPLOAD_PARAM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				index;
	u8_t				param_base;
} UPLOAD_PARAM_RESP_PACKET_t;
#pragma pack()

// Online Status
// for OP_REQ_ONLINE_STS
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				passed_time[4];
} ONLINE_STS_RESP_PACKET_t;
#pragma pack()

// Memory Check
// for OP_REQ_MEMORY_CHECK
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
} MEM_CHK_RESP_PACKET_t;
#pragma pack()

// Echo Message
// for OP_REQ_ECHO_MESSAGE
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				echo_msg[1];	
} ECHO_MSG_RESP_PACKET_t;
#pragma pack()

// Sequence Number
// for OP_SEQ_NUMBER
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				seq_num[1];
	
} SEQ_NUM_RESP_PACKET_t;
#pragma pack()

// Controller Initialize
// for OP_REQ_VERSION_REQ
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				version;	// Version No. or Release No.
	u8_t				year;		
	u8_t				month;		
	u8_t				day;		
	//u8_t				chksum[2];	// Checksum.
} VERSION_RESP_PACKET_t;
#pragma pack()

// 2014.09.02 by capi
#pragma pack(1)
typedef struct {		
	char						sip[IP_STR_LEN];
	char						hyphen_1;
	char						dip[IP_STR_LEN];
	char						hyphen_2;
	char						kind[2];
	u8_t						snum[STATION_NUM_LEN];
	u8_t						tlen[4];		//10	
	u8_t						opcode;
	u8_t						loop_num;		
	u8_t						data[1];	
} VDS_TCP_INVDATA_PACKET_t;
#pragma pack()

// for OP_REQ_INDIV_VDS_DATA
#pragma pack(1)
typedef   struct {
	u8_t				lane;	
	u8_t				time;	
	u8_t				speed;	
	u8_t				occupy[2];	
	u8_t				kind;	
} REALTIME_VENHICLE_t;
#pragma pack()

//insert by capi 2013.01.22
#pragma pack(1)
typedef   struct {
	u8_t				lane;	
	u8_t				time;	
	u8_t				spd;	
	u8_t				occupy[2];	
	u8_t				kind;	
} TCP_ONE_HOUR_VENHICLE_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				frame_num;
	u8_t				data_num[2];
	TCP_ONE_HOUR_VENHICLE_t		rt_data[1];
} INDIV_VDS_DATA_RESP_PACKET_t;			
#pragma pack()

//-----------------------------------------------------------------------------------
//Tcp/IP Test
//-----------------------------------------------------------------------------------

//121201
#define MAX_RTDATA_TCP		8

#pragma pack(1)
typedef   struct {
	uint8 year_hi;
	uint8 year_lo;
	uint8 month;
	uint8 day;
	uint8 hour;
	uint8 min;
	uint8 sec_hi;
	uint8 sec_lo;
} RTDATA_TIME_TCP;
#pragma pack()

#pragma pack(1)
typedef   struct {
	RTDATA_TIME_TCP time;
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
} RTDATA_PACK_TCP;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t 	head;
	uint8				data_num;
	RTDATA_PACK_TCP			data[MAX_RTDATA_TCP];
} RTDATA_RESP_PACKET_t;
#pragma pack()

//-----------------------------------------------------------------------------------

// Invalid Opcode
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
} INVALID_OPCODE_RESP_PACKET_t;
#pragma pack()


// Change IP Address
// for OP_CHANGE_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				ip[IP_STR_LEN];
} CHG_IP_RESP_PACKET_t;
#pragma pack()

// Change Station number
// for OP_CHG_STATION_NUM
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				st_num[STATION_NUM_LEN];
} CHG_ST_NUM_RESP_PACKET_t;
#pragma pack()


// Change MAC Address
// for OP_CHANGE_MAC
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				mac[MAC_ADDR_LEN];
} CHG_MAC_RESP_PACKET_t;
#pragma pack()

// Change Subnet Mask
// for OP_CHANGE_SUBN_MSK
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				nmsk[IP_STR_LEN];
} CHG_SUBN_MSK_RESP_PACKET_t;
#pragma pack()

// Change Gateway IP
// for OP_CHANGE_GATEWAY_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				gip[IP_STR_LEN];
} CHG_GATEWAY_IP_RESP_PACKET_t;
#pragma pack()

// Change Server IP
// for OP_CHANGE_SERVER_IP
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				server_ip[IP_STR_LEN];
} CHG_SERVER_IP_RESP_PACKET_t;
#pragma pack()

// Change Server TCP Port
// for OP_CHANGE_SERVER_PORT
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				server_port[2];
} CHG_SERVER_PORT_RESP_PACKET_t;
#pragma pack()

// Change Tunnel ID
// for OP_CHANGE_TUNNEL_ID
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				tunnel_id[TUNNEL_ID_STR_LEN];
} CHG_TUNNEL_ID_RESP_PACKET_t;
#pragma pack()

// Change Controller ID
// for OP_CHANGE_CONTROLLER_ID
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	char				controller_id[CONTROLLER_ID_STR_LEN];
} CHG_CNTR_ID_RESP_PACKET_t;
#pragma pack()

// Current RTC Time
// for OP_GET_RTC_TIME
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	RTC_TIME_t			curr_time;
} GET_RTC_TIME_RESP_PACKET_t;
#pragma pack()

// Current RTC Time
// for OP_SET_RTC_TIME
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
} SET_RTC_TIME_RESP_PACKET_t;
#pragma pack()

// Pan, Heater
// for OP_SET_RTC_TIME
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;			
	u8_t				temp[2];	
	u8_t				curtemp[2];
	u8_t				divi;
} CHG_PAN_HEATER_RESP_PACKET_t;
#pragma pack()

#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u8_t				status;	
} CHG_POWER_RESP_PACKET_t;
#pragma pack()

// Current System Status
// for OP_REQ_SYS_STATUS
#pragma pack(1)
typedef   struct {
	VDS_TCP_PKT_RESP_HEAD_t		head;		
	u16_t				temper[2];
	u16_t				pwrvol[2];
} REQ_SYS_STS_RESP_PACKET_t;
#pragma pack()

#endif /* __TCP_VDSAPP_PROTOCOL_H__ */

