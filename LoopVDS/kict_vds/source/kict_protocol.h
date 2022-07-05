#ifndef __KICT_PROTOCOL_H__
#define __KICT_PROTOCOL_H__


#define MAX_DATA_LEN 1024*8 


#define VDS_VERSION 0x01
#define HEADER_SIZE 12


//OP CODE
#define OPCODE_REQUEST									0xA1  // 정보 요청 프레임(Request Frame)
#define OPCODE_RESPONSE									0xA2  // 정보 응답 프레임(Response Frame)

#define OPCODE_RE_REQUEST								0xA3  // 정보 재전송 요청 프레임(Re-request Frame)
#define OPCODE_RE_RESPONSE								0xA4  // 정보 재전송 응답 프레임(Re-response Frame)




// DATA OP CODE 
#define OPCODE_REALTIME_TRAFFIC_DATA_REQUEST		0xB0 // 교통 데이터 전송
#define OPCODE_REALTIME_TRAFFIC_DATA_RESPONSE		0xB1 // 교통 데이터 수신응답

#define OPCODE_HISTORICAL_TRAFFIC_DATA_REQUEST		0xB2 // HISTORICAL 교통 데이터 요청
#define OPCODE_HISTORICAL_TRAFFIC_DATA_RESPONSE		0xB3 // HISTORICAL 교통 데이터 응답

#define OPCODE_START_VDS_REQUEST					0xB8 // 차량 검지 시작/정지 요청
#define OPCODE_START_VDS_RESPONSE					0xB9 // 차량 검지 시작/정지 응답

#define OPCODE_ECHO_BACK_REQUEST					0xBA // Echo Back 요청
#define OPCODE_ECHO_BACK_RESPONSE					0xBB // Echo Back 응답

#define OPCODE_VDS_STATUS_REQUEST					0xBC // 검지기 장비 상태 요청
#define OPCODE_VDS_STATUS_RESPONSE					0xBD // 검지기 장비 상태 응답

#define OPCODE_SET_TIME_REQUEST						0xBE // 검지기 시각 설정 요청
#define OPCODE_SET_TIME_RESPONSE					0xBF // 검지기 시각 설정 응답

#define VDS_DETECT_START							0x01 // VDS 검지 시작
#define VDS_DETECT_STOP								0x02 // VDS 검지 정지


enum FRAME_STATE
{
	NONE = 0,
	VERSION ,  // 0
	ADDRESS_1, // 1
	ADDRESS_2, // 2
	ADDRESS_3, // 3
	ADDRESS_4, // 4
	ADDRESS_5, // 5
	ADDRESS_6, // 6
	ADDRESS_7, // 7
	ADDRESS_8, // 8
	OPCODE,    // 9
	SIZE_1,      // 10
	SIZE_2 //,      // 11
//	COMPLETE 
};

#pragma pack(1)
typedef struct {
	uint8 version;
	uint8 address[8];
	uint8 opcode;
	UINT8 data_size[2];
} FRAME_HEADER;


typedef struct
{
	enum FRAME_STATE state;
	FRAME_HEADER header;
	uint16 data_size;
	uint8 data[MAX_DATA_LEN];  // 
}  KICT_FRAME;


typedef struct
{
	uint8 lane;
	uint8 direction;
	uint8 detect_time[8];
	uint8 velocity[2];
	uint8 occupyTime[2];
	uint8 length[2];
	uint8 reserved;

} TRAFFIC_DATA;



uint16 get_historical_data(uint8 start, uint8 * frame_data);
//int set_frame_data(YSE_FRAME * frame, uint8 data);
//int make_response_frame(uint8 * address, uint8 opcode, uint8 * data, int data_len, uint8 result, uint8 * packet);
//int send_ack(int fd, uint8 * address, uint8 opcode, uint8 result);
#endif	// __VDS_PROTOCOL_H__
