#ifndef __YSE_PROTOCOL_H__
#define __YSE_PROTOCOL_H__


#define MAX_DATA_LEN 1024 


#define FRAME_DLE 0x10 
#define FRAME_STX 0x02
#define FRAME_ETX 0x03

// OP CODE 

#define OPCODE_SYNC_COMMAND			0x01 // 제어 동기화 
#define OPCODE_DATA_COMMAND			0x04 //검지기 데이터 요구 
#define OPCODE_VDS_RESET				0x0C // VDS 리셋

#define OPCODE_PARAM_DOWNLOAD			0x0E // 파라메터 다운로드
#define OPCODE_PARAM_UPLOAD			0x0F // 파라메터 업로드
#define OPCODE_ONLINE_STATUS_REQ		0x11 // ONLINE STATUS REQUEST 
#define OPCODE_CHECK_MEMORY			0x12 // Memory Check 
#define OPCODE_ECHO_MESSAGE			0x13 // echo message
#define OPCODE_CHECK_SEQ				0x14 // Sequence Number Check
#define OPCODE_VDS_VER_CHECK			0x15 // VDS Identification Request
#define OPCODE_TEMPER_VOLT_CHECK		0x1E // 
#define OPCODE_CHANGE_RTC				0x18 // 

// ACK / NAK OPCODE
#define OPCODE_ACK				0x70
#define OPCODE_NAK				0x71

// status result code 
#define PROCESS_OK				0x00
#define INVALID_REQUEST			0x01
#define NOT_READY_RESPONSE		0x02
#define UNKNOWN_MESSAGE_TYPE	0x04


// parameter index 
#define PARAM_IDX_LOOP_ENABLE				1
#define PARAM_IDX_POLLYNG_CYCLE			3
#define PARAM_IDX_SPEED_CATEGORY			5
#define PARAM_IDX_LENGTH_CATEGORY		6
#define PARAM_IDX_SPEED_ACCU_ENABLE		7
#define PARAM_IDX_LENGTH_ACCU_ENABLE	8
#define PARAM_IDX_SPEED_CALCU_ENABLE	9
#define PARAM_IDX_LENGTH_CALCU_ENABLE	10
#define PARAM_IDX_OSC_THRESHOLD			17
#define PARAM_IDX_AUTO_RESYNC				20

enum FRAME_STATE
{
	NONE = 0,
	START_DLE ,
	START_STX ,
	ADDRESS_1,
	ADDRESS_2,
	ADDRESS_3,
	ADDRESS_4,
	OPCODE,
	DATA,
	END_DLE,
	END_ETX,
	CRC_1,
	CRC_2,
	COMPLETE
};


typedef struct
{
	enum FRAME_STATE state;
	uint8 address[4];	
	uint8 opcode;
	int data_size;
	uint8 data[MAX_DATA_LEN];  // 
	uint8 crc[2];
	long long timestamp;

}  YSE_FRAME;



#pragma pack(1)
typedef struct
{
	uint8 volume[3]; // 0: 대형, 중형, 소형
	uint8 speed;
	uint8 occupy[2];  //0 :  정수부분, 1: 소수부분 
	uint8 head_time;
} TRAFFIC_INFO;
#pragma pack()

#pragma pack(1)
typedef struct
{
	uint8 frameNo[2];
	uint8 detector[4];
	TRAFFIC_INFO traffic[8];

} TRAFFIC_DATA ;
#pragma pack()


#pragma pack(1)
typedef struct
{
	uint8 detector[2];
} PARAM_DETECTOR;
#pragma pack()



int set_frame_data(YSE_FRAME * frame, uint8 data);
int make_response_frame(uint8 * address, uint8 opcode, uint8 * data, int data_len, uint8 result, uint8 * packet);
int send_ack(int fd, uint8 * address, uint8 opcode, uint8 result);
#endif	// __VDS_PROTOCOL_H__
