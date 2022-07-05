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



#include "yse_protocol.h"
#include "yse_utils.h"


extern uint8 frame_num;


////////////////////////////////////////////

YSE_FRAME * g_prevFrame = NULL;
uint16 g_curFrameNo = 0;


void init_frame(YSE_FRAME * frame)
{
	memset(frame, 0, sizeof(YSE_FRAME) * sizeof(char));
	frame->state = NONE;
	frame->timestamp = 0;
	frame->data_size = 0;

}



long long current_timestamp() {
	struct timeval te;
	gettimeofday(&te, NULL); // get current time
	long long milliseconds = te.tv_sec * 1000LL + te.tv_usec / 1000; 
	return milliseconds;
}


void process_YSE_TCP_pkt(int fd, uint8 *pktFrame, int frameLen)
{
	int i = 0;
	int prev_dle = 0;
	uint8 data;
	//printf("process_YSE_TCP_pkt...\n");

	print_hexa(pktFrame, frameLen);

	while (i < frameLen)
	{
		if (g_prevFrame == NULL)
		{
			g_prevFrame = malloc(sizeof(YSE_FRAME) * sizeof(char));
			init_frame(g_prevFrame);
		}
		else if(g_prevFrame->timestamp > 0) // time out check
		{
			if (is_timeout(g_prevFrame->timestamp) == 1) // 
			{
				//printf("timeout....error\n");
				free(g_prevFrame);
				g_prevFrame = NULL;
				prev_dle = 0;
				i++;
				continue;
			}
		}
		data = *(pktFrame + i);
		switch (g_prevFrame->state)
		{
			case NONE :
				if(data == FRAME_DLE)
					g_prevFrame->state = START_DLE;
				else 
					init_frame(g_prevFrame);
				break;
			case START_DLE :
				if (data == FRAME_STX)
					g_prevFrame->state = START_STX;
				else 
					init_frame(g_prevFrame);
				break;
			case START_STX :
				set_frame_data(g_prevFrame, data);
				break;
			default:
				if (data == FRAME_DLE)
				{
					if (g_prevFrame != NULL && g_prevFrame->state == END_ETX) //
						set_frame_data(g_prevFrame, data);
					else if (prev_dle)
					{
						set_frame_data(g_prevFrame, data);
						prev_dle = 0;
					}
					else
					{
						prev_dle = 1;
					}
				}
				else
				{
					if (prev_dle) // DLE 가 있는 경우
					{
						if(data == FRAME_ETX)
							g_prevFrame->state = END_ETX;
						else
							init_frame(g_prevFrame);
						prev_dle = 0;
					}
					else
					{
						set_frame_data(g_prevFrame, data);
					}
						
				}
				break;
			
		}

		if (g_prevFrame->state == ADDRESS_4)
		{
			if (check_address(g_prevFrame->address))
			{
				g_prevFrame->timestamp = current_timestamp(); //  유효한 제어기 주소인지 확인 필요
			}
			else
			{
				free(g_prevFrame);
				g_prevFrame = NULL;
				prev_dle = 0;
			}
		}
		else if (g_prevFrame->state == COMPLETE)
		{
			if (check_crc(g_prevFrame) == 0)
			{
				process_YSE_command(fd, g_prevFrame);
			}
			free(g_prevFrame);
			g_prevFrame = NULL;
			prev_dle = 0;
		}
		i++;
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



int set_frame_data(YSE_FRAME * frame, uint8 data)
{
	int ret = 1;
	if (frame->state < START_STX)
		return ret;
	
	if(frame->state != DATA)
		frame->state++;

	switch (frame->state)
	{
		case ADDRESS_1:
			frame->address[0] = data;
			break;
		case ADDRESS_2:
			frame->address[1] = data;
			break;
		case ADDRESS_3:
			frame->address[2] = data;
			break;
		case ADDRESS_4:
			frame->address[3] = data;
			break;
		case OPCODE :
			frame->opcode = data;
			break;
		case DATA:
			frame->data[frame->data_size++] = data;
			break;
		case END_ETX:
			// check timeout
			break;
		case CRC_1 :
			frame->crc[0] = data;
			break;
		case CRC_2:
			frame->crc[1] = data;
			frame->state++;
			break;
	}
	return ret;
}

int process_YSE_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = 1;
	int i;
	int send_len = 0;
	uint8 data[MAX_DATA_LEN];
	uint8 packet[MAX_DATA_LEN];
	int data_len = 0;
	if (request_frame == NULL)
	{


		return 0;
	}
		

	memset(data, 0, MAX_DATA_LEN);

	switch (request_frame->opcode)
	{
		case OPCODE_SYNC_COMMAND :
			ret = process_sync_command(fd, request_frame);
			break;
		case OPCODE_DATA_COMMAND:
			ret = process_data_command(fd, request_frame);
			break;

		case OPCODE_VDS_RESET :
			ret = process_reset_command(fd, request_frame);
			break;
		case OPCODE_PARAM_DOWNLOAD:
			ret = process_param_download_command(fd, request_frame);
			break;
		case OPCODE_PARAM_UPLOAD:
			ret = process_param_upload_command(fd, request_frame);
			break;
		case OPCODE_ONLINE_STATUS_REQ:
			ret = process_online_status_command(fd, request_frame);
			break;
		case OPCODE_CHECK_MEMORY :
			ret = process_check_memory_command(fd, request_frame);
			break;
		case OPCODE_ECHO_MESSAGE:
			ret = process_echo_message_command(fd, request_frame);
			break;
		case OPCODE_CHECK_SEQ:
			ret = process_check_sequence_command(fd, request_frame);
			break;
		case OPCODE_VDS_VER_CHECK:
			ret = process_check_version_command(fd, request_frame);
			break;
		case OPCODE_TEMPER_VOLT_CHECK:
			ret = process_temper_volt_check_command(fd, request_frame);
			break;
		case OPCODE_CHANGE_RTC  :
			ret = process_change_rtc_command(fd, request_frame);
			break;
		default:
			ret = UNKNOWN_MESSAGE_TYPE;
			printf("unknown op code \n");
			break;
	}
	// 카운터 리셋 
	validsession_counter = 0;	//2011.09.14 by capidra
	vds_st.con_state = 1;
	host_req_counter = 0; //by capi 2014.09.18

	return ret;
}
int process_sync_command(int fd, YSE_FRAME * request_frame)
{
	//printf("process_sync_command...\n");
	uint8 ret = PROCESS_OK;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	g_curFrameNo = (request_frame->data[0]<<8) + request_frame->data[1];
	//printf("frameNo = %u \n", g_curFrameNo);

	// Set system status auto_resync flag.
	system_status.auto_resync = _SYS_VALID_;
	frame_num = g_curFrameNo;

	switchVdsBuffBank();

	//centerPollCounter

	prepareYSEReportData(centerPollCounter);
	prepareCenterPollData();		// eeee
	
	// 다시 초기화..
	activeYSEReportIndex = !activeYSEReportIndex;

	return ret;
}




int process_data_command(int fd, YSE_FRAME * request_frame)
{

	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	uint8 data[MAX_DATA_LEN];
	int packet_len = 0;
	float occupy_ratio = 0;
	uint8 headway_time = 0;
	int i;
	int index = 0;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_data_command....\n");

	int reportIndex = activeYSEReportIndex == 0 ? 1 : 0;
	
	memset(packet, 0, MAX_DATA_LEN);
	memset(data, 0, MAX_DATA_LEN);

	// Frame No
	data[index++] = (g_curFrameNo & 0xFF00)>>8;
	data[index++] = g_curFrameNo & 0x00FF;

	//  감지기 장애 코드
	//for (i = 0; i < MAX_LOOP_NUM / 4; i++)
	for (i = 0; i < MAX_LOOP_NUM / 8; i++)
		data[index++] = gucStuckPerLoop[i];	//2011.06.17 by capidra

	for (i = 0; i < 8  ; i++) //MAX_LANE_NUM
	{
		if (i < gucActiveDualLoopNum) //
		{
			// 대형 교통량
			data[index++] = gstYSEReportsOfLane[reportIndex][i].length_category[0];
			// 중형 교통량
			data[index++] = gstYSEReportsOfLane[reportIndex][i].length_category[1];
			// 소형 교통량 
			data[index++] = gstYSEReportsOfLane[reportIndex][i].length_category[2];


			//printf("gstYSEReportsOfLane[reportIndex][i].length_category[0] = %d , gstYSEReportsOfLane[reportIndex][i].length_category[0] = %d, gstYSEReportsOfLane[reportIndex][i].length_category[0] = %d \n", gstYSEReportsOfLane[reportIndex][i].length_category[0], gstYSEReportsOfLane[reportIndex][i].length_category[1], gstYSEReportsOfLane[reportIndex][i].length_category[2]);
			// 평균속도
			if (gstYSEReportsOfLane[reportIndex][i].total_volume > 0)
				data[index++] = gstYSEReportsOfLane[reportIndex][i].sum_of_speed / gstYSEReportsOfLane[reportIndex][i].total_volume;
			else
				data[index++] = 0;

			//printf("gstYSEReportsOfLane[reportIndex][i].sum_of_speed = %d  gstYSEReportsOfLane[reportIndex][i].total_volume=%d  avg speed=%d \n", gstYSEReportsOfLane[reportIndex][i].sum_of_speed, gstYSEReportsOfLane[reportIndex][i].total_volume, data[index-1]);


			// 점유율
			if (!gstYSEReportsOfLane[reportIndex][i].polling_time)
				occupy_ratio = 0;
			else
			{
				occupy_ratio = ((float)gstYSEReportsOfLane[reportIndex][i].sum_of_occupy / (float)gstYSEReportsOfLane[reportIndex][i].polling_time) * 100;
				if (occupy_ratio > 100)
					occupy_ratio = 100; 
			}

			//printf("gstYSEReportsOfLane[reportIndex][i].sum_of_occupy = %d  gstYSEReportsOfLane[reportIndex][i].polling_time=%d \n", gstYSEReportsOfLane[reportIndex][i].sum_of_occupy, gstYSEReportsOfLane[reportIndex][i].polling_time);
			//printf("occupy_ratio = %f.. \n", occupy_ratio);


			// 정수부
			data[index++] = (uint8)occupy_ratio;

			//printf("occupy_ratio int part= %d.. \n", data[index-1]);

			// 소수부
			data[index++] = (uint8) ( ( occupy_ratio - (uint8)occupy_ratio)*100);

			//printf("occupy_ratio zeropoint part= %d.. \n", data[index - 1]);
			

			// 차두시간 = 3600 / 교통량 . 3600초 동안의 교통량. 즉 1대가 통과할 때까지의 시간(초) 
			if (gstYSEReportsOfLane[reportIndex][i].total_volume != 0)
				headway_time = (uint8) ( (3600 / gstYSEReportsOfLane[reportIndex][i].total_volume)*(((float)gstYSEReportsOfLane[reportIndex][i].polling_time / 1000) / 3600) );
			else
				headway_time = 0;

			//printf("headway_time =%d gstYSEReportsOfLane[reportIndex][i].total_volume=%d gstYSEReportsOfLane[reportIndex][i].polling_time=%d \n", headway_time, gstYSEReportsOfLane[reportIndex][i].total_volume, gstYSEReportsOfLane[reportIndex][i].polling_time);

			data[index++] = (uint8)headway_time;
		}
		else
		{
			// 대형 교통량
			data[index++] = 0xFF;
			// 중형 교통량
			data[index++] = 0xFF;
			// 소형 교통량 
			data[index++] = 0xFF;


			// 평균속도
			data[index++] = 0;
			
			// 점유율
			data[index++] = 0;
			data[index++] = 0;

			// 차두시간
			data[index++] = 0;

		}
		// 다시 초기화..
		memset(&gstYSEReportsOfLane[reportIndex][i], 0, sizeof(YSE_REPORT_PER_LANE));
	}

	packet_len = make_response_frame(request_frame->address, request_frame->opcode, data, index, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}


int process_reset_command(int fd, YSE_FRAME * request_frame)
{

	uint8 ret = PROCESS_OK;
	if (request_frame == NULL)
		return INVALID_REQUEST;
	uint32 tlen;

	

	// Status
	system_status.controller_reset = _SYS_INVALID_;
	isReqForceReset = 0x89;
	 
	//printf("process_reset_command....\n");
	send_ack(fd, request_frame->address, OPCODE_ACK, ret);

	//if (request_frame->opcode != OPCODE_SYNC_COMMAND) // 동기화 명령은 응답 없음
	//{
	//	memset(packet, 0, MAX_DATA_LEN);
	//	send_len = make_response_frame(request_frame->address, request_frame->opcode, data, data_len, ret, packet);
	//	int flag_send = 0;
	//	if (send(fd, packet, send_len, flag_send) == -1)
	//	{
	//		pr_err("[TCP client] : Fail: send()");
	//	}
	//}




	return ret;
}


int process_param_download_command(int fd, YSE_FRAME * request_frame)
{

	uint8 ret = PROCESS_OK;
	if (request_frame == NULL)
		return INVALID_REQUEST;
	
	uint8 paramIndex = request_frame->data[0];
	switch (paramIndex)
	{
		case PARAM_IDX_LOOP_ENABLE:  // 검지기 지정  2 bytes 
			ret = process_set_detector(fd, request_frame);
			break;
		case 2: // reserved     
			break;
		case PARAM_IDX_POLLYNG_CYCLE:  // Polling Cycle  1 byte
			ret = process_param_index(fd, request_frame);
			break;
		case 4: // reserved i
			break;
		case PARAM_IDX_SPEED_CATEGORY: // 차량속도구분(12단계)  12 bytes
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_LENGTH_CATEGORY: // 차량길이 구분 3 bytes
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_SPEED_ACCU_ENABLE: // 속도별 누적치 계산 1 byte
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_LENGTH_ACCU_ENABLE: // 차량길이별 누적치 계산 1 byte 
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_SPEED_CALCU_ENABLE: // 속도 계산 가능 여부 1 byte
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_LENGTH_CALCU_ENABLE: // 차량길이 계산 가능 여부 1 byte 
			ret = process_param_index(fd, request_frame);
			break;
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16: // reserved 
		case 18: // Unit HW address 
		case 19: // Parameter RAM status 
		case 21: // 제어기 프로그램 버젼 reserved 
		case 22: // VDS Response Data Length Reserved 
		case 23: // reserved 
			break;
		case PARAM_IDX_OSC_THRESHOLD: // 검지기 Oscilation Threshold  1 byte 
			ret = process_param_index(fd, request_frame);
			break;
		case PARAM_IDX_AUTO_RESYNC: // Auto Re-Sync 대기 시간 1 byte
			ret = process_param_index(fd, request_frame);
			break;

	}
	return ret;
}


int  get_system_status(uint8 * status, uint8 result)
{
	memset(status, 0, sizeof(uint8) * 2);

	checkSystemStatus();

	uint16 val = 0;

	val = (system_status.controller_reset << 13)
		+ (system_status.heater_operate << 12)
		+ (system_status.fan_operate << 11)
		+ (system_status.rear_door_open << 10)
		+ (system_status.front_door_open << 9)
		+ (system_status.auto_resync << 8)
		+ (system_status.recv_broadcast_msg << 7)
		+ (system_status.default_param << 6)
		+ (system_status.short_pwr_fail << 4)
		+ (system_status.long_pwr_fail << 3)
		+ result
		;

	status[0] = val >> 8;
	status[1] = val & 0x00FF;

	


	/*printf("system_status.long_pwr_fail=%d,system_status.short_pwr_fail=%d, system_status.recv_broadcast_msg=%d, system_status.auto_resync=%d,system_status.front_door_open=%d, system_status.rear_door_open = %d ,system_status.fan_operate=%d ,system_status.heater_operate = %d, system_status.controller_reset=%d \n",
		system_status.long_pwr_fail, system_status.short_pwr_fail, system_status.recv_broadcast_msg, system_status.auto_resync, system_status.front_door_open, system_status.rear_door_open, system_status.fan_operate, system_status.heater_operate, system_status.controller_reset);
	*/
	return 1;
}



int make_response_frame(uint8 * address, uint8 opcode, uint8 * data, int data_len, uint8 result, uint8 * packet)
{
	int index = 0;
	int i;
	uint16 crc = 0;

	packet[index++] = FRAME_DLE;
	packet[index++] = FRAME_STX;

	// address stuffing 
	for (i = 0; i < 4; i++)
	{
		packet[index++] = address[i];
		if (address[i] == FRAME_DLE)
		{
			packet[index++] = FRAME_DLE;
		}
	}
	// opcode stuffing 
	packet[index++] = opcode;
	if (opcode == FRAME_DLE)
	{
		packet[index++] = FRAME_DLE;
	}

	// status 
	uint8 status[2];
	get_system_status(status, result);
	//status[0] = 
	for (i = 0; i < 2; i++)
	{
		packet[index++] = status[i];
		if (status[i] == FRAME_DLE)
		{
			packet[index++] = FRAME_DLE;
		}
	}

	for (i = 0; i < data_len; i++)
	{
		packet[index++] = data[i];
		if (data[i] == FRAME_DLE)
		{
			packet[index++] = FRAME_DLE;
		}
	}

	// address 
	crc = updateCRC16_ibm(address, 4, crc);
	// opcode 
	crc = updateCRC16_ibm(&opcode, 1, crc);
	// status 
	crc = updateCRC16_ibm(status, 2, crc);
	// data
	crc = updateCRC16_ibm(data, data_len, crc);

	packet[index++] = FRAME_DLE;
	packet[index++] = FRAME_ETX;

	packet[index++] = crc >> 8;
	packet[index++] = crc & 0x00FF;

	return index;

}


int send_ack(int fd, uint8 * address, uint8 opcode, uint8 result)
{
	int ret = 0;

	uint8 packet[MAX_DATA_LEN];
	int packet_len = 0;
	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(address, opcode, NULL, 0, result,  packet);
	ret = send_frame(fd, packet, packet_len);

	//printf("send_ack...\n");
	return ret;
}

int send_frame(int fd, uint8 * packet, int len)
{
	int ret = 0;
	int flag_send = 0;
	ret = send(fd, packet, len, flag_send);
	//printf("send_frame %d byte sent \n", ret);
	if ( ret == -1)
	{
		pr_err("[TCP client] : Fail: send()");
	}
	return ret;
}

int process_set_detector(int fd, YSE_FRAME * request_frame)
{
	int ret = PROCESS_OK;
	
	PARAM_DETECTOR * detector =  &request_frame->data[1];
	uint8 paramIndex = request_frame->data[0];

	if (detector != NULL)
	{
		putVdsParamLoopenable(paramIndex,(uint8 *)detector, TRUE);
		send_ack(fd, request_frame->address, OPCODE_ACK, ret);
	}
	else
	{
		ret = NOT_READY_RESPONSE;
		send_ack(fd, request_frame->address, OPCODE_NAK, ret);
	}
	return ret;
}


int process_param_index(int fd, YSE_FRAME * request_frame)
{
	int ret = PROCESS_OK;
	int i;
	uint8 paramIndex = request_frame->data[0];
	uint8 * params = &request_frame->data[1];

	if (params != NULL)
	{
		putVdsParameter(paramIndex, params, TRUE);
		send_ack(fd, request_frame->address, OPCODE_ACK, ret);
	}
	else
	{
		ret = INVALID_REQUEST;
		send_ack(fd, request_frame->address, OPCODE_NAK, ret);
	}
	return ret;
}


int process_param_upload_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	uint8 paramData[MAX_DATA_LEN];
	int packet_len = 0;

	if (request_frame == NULL)
		return INVALID_REQUEST;

	uint8 paramIndex = request_frame->data[0];
	memset(packet, 0, MAX_DATA_LEN);
	memset(paramData, 0, MAX_DATA_LEN);
	paramData[0] = paramIndex;
	int paramSize = getVdsParameter(paramIndex, paramData + 1);	// wwww _packed 문제.

	//printf("process_param_upload_command  index=%d, paramSize= %d \n", paramIndex, paramSize);

	packet_len = make_response_frame(request_frame->address, request_frame->opcode, paramData,paramSize+1, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}

int process_online_status_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;

	uint8 packet[MAX_DATA_LEN];
	uint8 passed_time[4];
	
	int packet_len = 0;

	if (request_frame == NULL)
		return INVALID_REQUEST;

	u_int host_link_passed_time;

	host_link_passed_time = host_req_counter / _1_SEC_TICK_;

	//printf("process_online_status_command...pass time = %d \n", host_link_passed_time);

	passed_time[0] = (u8_t)((host_link_passed_time & 0xFF000000) >> 24);
	passed_time[1] = (u8_t)((host_link_passed_time & 0x00FF0000) >> 16);
	passed_time[2] = (u8_t)((host_link_passed_time & 0x0000FF00) >> 8);
	passed_time[3] = (u8_t)((host_link_passed_time & 0x000000FF) >> 0);

	memset(packet, 0, MAX_DATA_LEN);

	packet_len = make_response_frame(request_frame->address, request_frame->opcode, passed_time, 4, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}

int process_check_memory_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	
	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_check_memory_command....\n");
	send_ack(fd, request_frame->address, OPCODE_ACK, ret);
	return ret;
}

int process_echo_message_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	int packet_len = 0;

	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_echo_message_command....echo message=%s\n", request_frame->data);
	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(request_frame->address, request_frame->opcode, request_frame->data, request_frame->data_size, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
	return ret;
}

int process_check_sequence_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	uint8 sequence[256];
	int packet_len = 0;
	int i = 0;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	uint8 startSeqNo = request_frame->data[0];
	uint8 count = request_frame->data[1];

	if (count > 256)
		count = 256;

	//printf("process_check_sequence_command....start number=%d count=%d \n", startSeqNo, count);
	memset(sequence, 0, 256);
	for (i = 0; i < count; i++)
	{
		sequence[i] =  (uint8)(startSeqNo + i);
		//printf("seq index=%d seq=%d \n", i, sequence[i]);
	}
	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(request_frame->address, request_frame->opcode, sequence, count, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}


int process_check_version_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	uint8 data[6];
	int packet_len = 0;
	int i = 0;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_check_version_command...VER=%d YEAR=%d MONTH=%d DAY=%d.\n", VERSION_NUM, PRODUCTED_YEAR, PRODUCTED_MONTH, PRODUCTED_DAY);

	memset(data, 0, 6);

	data[0] = VERSION_NUM;
	data[1] = PRODUCTED_YEAR;
	data[2] = PRODUCTED_MONTH;
	data[3] = PRODUCTED_DAY;
	
	

	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(request_frame->address, request_frame->opcode, data, 6, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}

int process_temper_volt_check_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];
	uint8 data[3];
	int packet_len = 0;
	int i = 0;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_temper_volt_check_command...\n");



	memset(data, 0, 3);

	// 아래는 임시코드 
	data[0] = 0x00;  // TEMPERATURE 
	data[1] = 0x00;  // INPUT VOLTAGE
	data[2] = 0x00; // OUTPUT VOLTAGE

	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(request_frame->address, request_frame->opcode, data, 3, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}

int process_change_rtc_command(int fd, YSE_FRAME * request_frame)
{
	uint8 ret = PROCESS_OK;
	uint8 packet[MAX_DATA_LEN];

	char timebuf[255];
	uint8 data[7];
	SYSTEMTIME currntTime;
	SYSTEMTIME nextTime;

	int packet_len = 0;
	int i = 0;
	if (request_frame == NULL)
		return INVALID_REQUEST;

	//printf("process_change_rtc_command...%d-%d-%d %d:%d:%d \n", sys_time.wYear, sys_time.wMonth, sys_time.wDay
	//	, sys_time.wHour, sys_time.wMinute, sys_time.wSecond);


	currntTime = sys_time;
	//0x20 0x21 0x02 0x03 0x12 0x06 0x38 : 2021-02-03 12:06:38
	nextTime.wYear = (request_frame->data[0] >> 4) * 1000 + (request_frame->data[0] & 0x0F) * 100 + (request_frame->data[1] >> 4) * 10 + (request_frame->data[1] & 0x0F);// request_frame->data[0] * 100 + request_frame->data[1];// year 
	nextTime.wMonth = (request_frame->data[2] >>4)*10 + (request_frame->data[2] &0x0F) ;
	nextTime.wDay = (request_frame->data[3] >> 4) * 10 + (request_frame->data[3] & 0x0F); ;// request_frame->data[3];
	nextTime.wHour = (request_frame->data[4] >> 4) * 10 + (request_frame->data[4] & 0x0F);  //request_frame->data[4];
	nextTime.wMinute = (request_frame->data[5] >> 4) * 10 + (request_frame->data[5] & 0x0F);  //request_frame->data[5];
	nextTime.wSecond = (request_frame->data[6] >> 4) * 10 + (request_frame->data[6] & 0x0F);  //request_frame->data[6];



	// os time 변경 
	sprintf(timebuf, "date -s '%d-%d-%d %d:%d:%d'"
		, nextTime.wYear, nextTime.wMonth, nextTime.wDay
		, nextTime.wHour, nextTime.wMinute, nextTime.wSecond);
	system(timebuf);

	//printf("next time = %s \n", timebuf);

	// RTC 변경 
	msec_sleep(300);
	setSysTimeToRTC(&nextTime);

	
	memset(data, 0, 7);

	data[0] = ((((currntTime.wYear / 100) / 10) << 4) | ((currntTime.wYear / 100) % 10));  //(currntTime.wYear / 100) ; // 0x20 
	data[1] = ((((currntTime.wYear % 100) / 10) << 4) | ((currntTime.wYear % 100) % 10)); // (currntTime.wYear % 100); // 0x21
	data[2] = (((currntTime.wMonth / 10) << 4) | (currntTime.wMonth % 10));// currntTime.wMonth;
	data[3] = (((currntTime.wDay / 10) << 4) | (currntTime.wDay % 10)); //currntTime.wDay;
	data[4] = (((currntTime.wHour / 10) << 4) | (currntTime.wHour % 10)); //currntTime.wHour;
	data[5] = (((currntTime.wMinute / 10) << 4) | (currntTime.wMinute % 10)); //currntTime.wMinute;
	data[6] = (((currntTime.wSecond / 10) << 4) | (currntTime.wSecond % 10)); // currntTime.wSecond;

	memset(packet, 0, MAX_DATA_LEN);
	packet_len = make_response_frame(request_frame->address, request_frame->opcode, data, 7, ret, packet);
	ret = send_frame(fd, packet, packet_len);
	return ret;
}

int prepareYSEReportData(u_int polling_time)
{
	int i;
	for (i = 0; i < MAX_LANE_NUM; i++)
	{
		gstYSEReportsOfLane[activeYSEReportIndex][i].polling_time = polling_time;
	}
	return 1;
}
