#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/epoll.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>

#include "systypes.h"
#include "stdalsp.h"
//#include "crc-16.h"
#include "tcpip.h"
#include "ftms_cpu.h"
#include "serial.h"



#include "kict_protocol.h"
#include "kict_utils.h"


extern uint8 frame_num;


////////////////////////////////////////////

KICT_FRAME * g_prevFrame = NULL;

uint8 g_historical_data_index = 0;
uint8 g_historical_data_size = 255;

extern int fd_listener;
extern uint8 g_isServiceStart;
long long current_timestamp() {
	struct timeval te;
	gettimeofday(&te, NULL); // get current time
	long long milliseconds = te.tv_sec * 1000LL + te.tv_usec / 1000; 
	return milliseconds;
}


void init_frame(KICT_FRAME * frame)
{
	memset(frame, 0, sizeof(KICT_FRAME) * sizeof(char));
	frame->state = NONE;
	frame->header.version = VDS_VERSION;
}

void process_KICT_TCP_pkt(int fd, uint8 *pktFrame, int frameLen)
{
	int i = 0;
	int prev_dle = 0;
	uint8 readCount = 0;
	uint8 data;
	//printf("process_KICT_TCP_pkt...\n");

	print_hexa(pktFrame, frameLen);

	if (i < frameLen)
	{
		if (g_prevFrame == NULL)
		{
			g_prevFrame = malloc(sizeof(KICT_FRAME) * sizeof(char));
			init_frame(g_prevFrame);
		}

		if (g_prevFrame->state < SIZE_2) // header 완성 안되었을 경우 
		{
			if (frameLen - g_prevFrame->state >= HEADER_SIZE) // 받은 패킷이 현재 잔여 헤더보다 클경우
				readCount = HEADER_SIZE - g_prevFrame->state;
			else   // 헤더 
				readCount = frameLen;                         // 받은 패킷이 현재 잔여 헤더보다 작을 경우 

			memcpy(&(g_prevFrame->header), pktFrame + i, readCount);
			i += readCount;
			g_prevFrame->state += readCount;
		}
		//printf("avogadro frameLen=%d  readCount = %d g_prevFrame->state=%d \n", frameLen, readCount, g_prevFrame->state);
		if (g_prevFrame->state == SIZE_2) // header 완성되었을 경우
		{
			g_prevFrame->data_size = (g_prevFrame->header.data_size[0] << 8) + (g_prevFrame->header.data_size[1]);
			//printf("avogadro frame g_prevFrame->header.data_size[0]=0x%02x g_prevFrame->header.data_size[1]=0x%02x  data size=%d \n", g_prevFrame->header.data_size[0], g_prevFrame->header.data_size[1],g_prevFrame->data_size);
			readCount = g_prevFrame->data_size;
			memcpy(&g_prevFrame->data, pktFrame + i, readCount);
			i += readCount;


			process_KICT_command(fd, g_prevFrame);

			free(g_prevFrame);
			g_prevFrame = NULL;

		}

	}

}


int is_timeout(long long time)
{
	int ret = 0;
	long long current = current_timestamp();
	if (current - time > 2400)
		ret =1;
	return ret;
}

int check_address(uint8 address[4])
{
	int ret = 1;


	return ret;

}

int get_frame_addr(KICT_FRAME * frame)
{
	if (frame == NULL)
		return 0;
	frame->header.address[0] = '1';
	frame->header.address[1] = '2';
	frame->header.address[2] = '3';
	frame->header.address[3] = '4';
	frame->header.address[4] = '5';
	frame->header.address[5] = '6';
	frame->header.address[6] = '7';
	frame->header.address[7] = '8';

	return  1;
}

int make_initial_response(KICT_FRAME * request, KICT_FRAME * response)
{
	if (request == NULL || response == NULL)
		return 0;
	init_frame(response);

	
	get_frame_addr(response);
	response->header.opcode = request->header.opcode + 2; // 요청에 대한 응답코드 
	return 1;
}

int process_KICT_command(int fd, KICT_FRAME * request)
{
	uint8 ret = 1;
	int i;
	int send_len = 0;
	uint8 data[MAX_DATA_LEN];
	uint8 packet[MAX_DATA_LEN];
	int data_len = 0;
	if (request == NULL)
	{


		return 0;
	}
		

	memset(data, 0, MAX_DATA_LEN);

	switch (request->data[0])
	{
		case OPCODE_HISTORICAL_TRAFFIC_DATA_REQUEST:
			process_historical_data_request(fd, request);
			break;
		case OPCODE_START_VDS_REQUEST:
			process_start_vds_request(fd, request);
			break;
		case OPCODE_ECHO_BACK_REQUEST:
			process_echo_back_request(fd, request);
			break;
		case OPCODE_VDS_STATUS_REQUEST:
			process_vds_status_request(fd, request);
			break;
		case OPCODE_SET_TIME_REQUEST:
			process_set_time_request(fd, request);
			break;
		case OPCODE_REALTIME_TRAFFIC_DATA_RESPONSE:
			process_traffic_data_response(fd, request);
			break;
	}

	//printf("avogadro process_KICT_command opcode = 0x%02x request->data[0]=0x%02x \n", request->header.opcode, request->data[0]);
	// 카운터 리셋 
	validsession_counter = 0;	//2011.09.14 by capidra
	vds_st.con_state = 1;
	host_req_counter = 0; //by capi 2014.09.18

	return ret;
}



int process_traffic_data_response(int fd, KICT_FRAME * response)
{
	
	//printf("process_traffic_data_response...response.data_size =%d \n", response->data_size);
	
	return 1;
}



int process_historical_data_request(int fd, KICT_FRAME * request)
{
	KICT_FRAME response;

	make_initial_response(request, &response);
	response.data[0] = OPCODE_HISTORICAL_TRAFFIC_DATA_RESPONSE;
	response.data_size = get_historical_data(g_historical_data_index, &response.data) ;
	//printf("process_historical_data_request...response.data_size =%d \n", response.data_size);
	int ret = send_frame(fd, &response);
	// data op
	//response.
	// data count
	// 검지 정보....


	//printf("process_historical_data_request ...\n");
	return ret;
}


uint16 get_historical_data(uint8 start, uint8 * frame_data)
{
	int i = 0;
	int startIndex = 2;
	uint16 data_size = 2; // op code, count 포함

	// 테스트 코드 시작 
	// 아래 코드는 교통 데이터를 가져오는 부분으로 대체해야 한다. 현재는 프로토콜 테스트 코드
	TRAFFIC_DATA testData[300];
	SYSTEMTIME ostime;
	for (i = 0; i < 0; i++)
	{
		testData[i].lane = i % 255;
		testData[i].direction = (i % 2) + 1;
		testData[i].length[0] = (uint8)(i / 255);
		testData[i].length[1] = (uint8)(i % 255);
		testData[i].occupyTime[0] = (uint8)(i / 255);
		testData[i].occupyTime[1] = (uint8)(i % 255);

		testData[i].velocity[0] = 1;
		testData[i].velocity[1] = 23;
		
		getOsTime(&ostime);
		date2bcd(testData[i].detect_time,&ostime) ;

	}
	

	for (i = 0; i < 0 ; i++)
	{
		memcpy(frame_data + startIndex, &testData[start + i], sizeof(TRAFFIC_DATA));
		startIndex += sizeof(TRAFFIC_DATA);
		data_size += sizeof(TRAFFIC_DATA);
	}
	frame_data[1] = i; // traffic data count
	// 테스트 코드 종료
	return 	data_size;
}




int process_start_vds_request(int fd, KICT_FRAME * request)
{
	//printf("process_start_vds_request ...\n");
	KICT_FRAME response;
	make_initial_response(request, &response);


	response.data[0] = OPCODE_START_VDS_RESPONSE;
	response.data[1] = request->data[1];
	response.data_size = 2;

	int ret = send_frame(fd, &response);

	switch (response.data[1])
	{
		case VDS_DETECT_START:
			g_isServiceStart = 1;
			//printf("VDS DETECT START.....processing \n");
		break;
		case VDS_DETECT_STOP:
			g_isServiceStart = 0;
			//printf("VDS DETECT STOP.....processing \n");
			break;
	}
	return ret;
}

int process_echo_back_request(int fd, KICT_FRAME * request)
{
	//printf("process_echo_back_request ...\n");

	KICT_FRAME response;
	make_initial_response(request, &response);
	response.data[0] = OPCODE_ECHO_BACK_RESPONSE;


	memset(response.data + 1, request->data + 1, request->data_size - 1);

	response.data_size = request->data_size;

	int ret = send_frame(fd, &response);

	return ret;

}

int process_vds_status_request(int fd, KICT_FRAME * request)
{
	//printf("process_vds_status_request ...\n");
	SYSTEMTIME ostime;

	KICT_FRAME response;
	make_initial_response(request, &response);
	response.data[0] = OPCODE_VDS_STATUS_RESPONSE;

	// 전송시간 8 byte
	getOsTime(&ostime);
	date2bcd(response.data+1, &ostime);
	// 상태  2 byte 
	get_system_status(response.data + 1 + 8);
	response.data_size = 11;
	int ret = send_frame(fd, &response);


	return ret;
}
int process_set_time_request(int fd, KICT_FRAME * request)
{
	//printf("process_set_time_request ...\n");
	
	SYSTEMTIME ostime;
	SYSTEMTIME nextTime;

	KICT_FRAME response;
	make_initial_response(request, &response);
	response.data[0] = OPCODE_SET_TIME_RESPONSE;


	// 시간 설정....
	memset(&nextTime, 0, sizeof(SYSTEMTIME));
	bcd2date(request->data+1, &nextTime);

	//
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

	/*printf("process_set_time_request..set %d/%d/%d %d:%d:%d\n\r", nextTime.wYear, nextTime.wMonth, \
		nextTime.wDay, nextTime.wHour, nextTime.wMinute, nextTime.wSecond);*/

	// 전송시간 8 byte
	getOsTime(&ostime);
	date2bcd(response.data + 1, &nextTime);

	response.data_size = 9;
	int ret = send_frame(fd, &response);
	return ret;
}

int serialize_frame(KICT_FRAME * frame, uint8 * packet)
{
	int len = 0;
	frame->header.data_size[0] = (frame->data_size & 0xFF00) >> 8 ;
	frame->header.data_size[1] = frame->data_size & 0x00FF;
	// header
	memcpy(packet, &(frame->header), sizeof(FRAME_HEADER));
	len += sizeof(FRAME_HEADER);
	memcpy(packet+len, &(frame->data), frame->data_size);
	len += frame->data_size;

	// data


	return len;
}
int send_frame(int fd, KICT_FRAME * frame)
{
	int ret = 0;
	int flag_send = 0;
	uint8 packet[MAX_DATA_LEN];
	memset(packet, 0, MAX_DATA_LEN);
	int len = serialize_frame(frame, packet);
	ret = send(fd, packet, len, flag_send);
	if (ret == -1)
	{
		pr_err("[TCP client] : Fail: send()");
	}
	return ret;
}

int  get_system_status(uint8 * status)
{
	memset(status, 0, sizeof(uint8) * 2);
	checkSystemStatus();
	uint16 val = 0;
	/*
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
	*/

	val = //(system_status.controller_reset << 8)  // SYSTEM Halt 정의 안됨
		 //+ (system_status.heater_operate << 12) // 검지 에러    정의 안됨
		 (system_status.controller_reset << 6)      // 제어기 리셋    
		+ (system_status.heater_operate << 5)        //HEATER OPERATE
		+ (system_status.fan_operate << 4)
		+ (system_status.rear_door_open << 3)
		+ (system_status.front_door_open << 2)
		+ (system_status.short_pwr_fail << 1)
		+ system_status.long_pwr_fail;
		;
	//val = 8225;
	status[0] = val >> 8;
	status[1] = val & 0x00FF;




	/*printf("system_status.long_pwr_fail=%d,system_status.short_pwr_fail=%d, system_status.recv_broadcast_msg=%d, system_status.auto_resync=%d,system_status.front_door_open=%d, system_status.rear_door_open = %d ,system_status.fan_operate=%d ,system_status.heater_operate = %d, system_status.controller_reset=%d \n",
		system_status.long_pwr_fail, system_status.short_pwr_fail, system_status.recv_broadcast_msg, system_status.auto_resync, system_status.front_door_open, system_status.rear_door_open, system_status.fan_operate, system_status.heater_operate, system_status.controller_reset);
	*/
	return 1;
}

int sendTrafficDataEvent()
{
	char szLog[1024];
	sprintf(szLog, "SendInversData enter...");

	int ret = 0;
	int flag_send = 0;
	uint8 packet[1024];
	memset(packet, 0, 1024);
	memcpy(packet, "avogadro", strlen("avogadro"));
	int len = strlen("avogadro");

	ret = send(fd_listener, packet, len, flag_send);
	if (ret == -1)
	{
		pr_err("[TCP client] : Fail: send()");
	}
	return ret;

}

int sendRealtimeDataToKICTCenter(uint32 lane)
{ 
	
	KICT_FRAME request;
	init_frame(&request);
	get_frame_addr(&request);
	int ret = 0;
	uint16 speed = 0;
	uint16 length = 0;
	uint16 lane_occupy = 0;

	request.header.opcode = OPCODE_REQUEST;

	request.data[0] = OPCODE_REALTIME_TRAFFIC_DATA_REQUEST;

	// 테스트 코드 시작 
	// 아래 코드는 교통 데이터를 가져오는 부분으로 대체해야 한다. 현재는 프로토콜 테스트 코드
	TRAFFIC_DATA trafficData;
	memset(&trafficData, 0, sizeof(TRAFFIC_DATA));

	switch (lane)
	{
		case 0:
		case 2:
			trafficData.lane = 1;
			break;
		case 1:
		case 3:
			trafficData.lane = 2;
			break;
	}
	//trafficData.lane = lane+1;
	trafficData.direction = lane < 2 ? 1 : 2 ; // 1: 상행선, 2: 하행선 

	speed = (uint16)gstCurrData.speed[lane];
	length = (uint16)gstCurrData.length[lane] * 10; // 단위가 dm 으로 cm 로 변경 
	lane_occupy = (uint16)gstCurrData.lane_occupy;

	if (speed > 2 && length > 250) // 쓰레기 데이터 전송하는 경우가 있어서.. 체크(오토바이 2.5미터 이상)
	{


		trafficData.length[0] = length >> 8;
		trafficData.length[1] = length & 0x00FF;
		trafficData.occupyTime[0] = lane_occupy >> 8;
		trafficData.occupyTime[1] = lane_occupy & 0x00FF;

		trafficData.velocity[0] = speed / 100;  // 소숫점 처리 위해 
		trafficData.velocity[1] = speed % 100;  // 소숫점 처리 위해 

		date2bcd(trafficData.detect_time, &sys_time);

		memcpy(request.data + 1, &trafficData, sizeof(TRAFFIC_DATA));
		request.data_size = 18;
		if(length > 250)
			ret = send_frame(fd_listener, &request);
	}
	//printf("avogadro...sendRealtimeDataToKICTCenter lane=%d  gstCurrData.speed[lane]=%d , gstCurrData.length[lane]=%d , lane_occupy = %d  \n", lane, gstCurrData.speed[lane], gstCurrData.length[lane], lane_occupy);
	return ret;
}

int sendRealtimeDataTest()
{

	KICT_FRAME request;
	init_frame(&request);
	get_frame_addr(&request);

	uint16 speed = 0;
	uint16 length = 0;
	uint16 lane_occupy = 0;
	SYSTEMTIME ostime; 
	request.header.opcode = OPCODE_REQUEST;

	request.data[0] = OPCODE_REALTIME_TRAFFIC_DATA_REQUEST;

	// 테스트 코드 시작 
	// 아래 코드는 교통 데이터를 가져오는 부분으로 대체해야 한다. 현재는 프로토콜 테스트 코드
	TRAFFIC_DATA trafficData;
	memset(&trafficData, 0, sizeof(TRAFFIC_DATA));

	trafficData.lane =  1;
	trafficData.direction = 1 ; // 1: 상행선, 2: 하행선 

	speed = (uint16)50;
	length = (uint16)260;
	lane_occupy = (uint16)700;

	trafficData.length[0] = length >> 8;
	trafficData.length[1] = length & 0x00FF;
	trafficData.occupyTime[0] = lane_occupy >> 8;
	trafficData.occupyTime[1] = lane_occupy & 0x00FF;

	trafficData.velocity[0] = speed / 100;
	trafficData.velocity[1] = speed % 100;


	getOsTime(&ostime);
	date2bcd(trafficData.detect_time, &ostime);

	memcpy(request.data + 1, &trafficData, sizeof(TRAFFIC_DATA));
	request.data_size = 18;
	int ret = send_frame(fd_listener, &request);
	//printf("avogadro...sendRealtimeDataTest   \n");
	return ret;
}