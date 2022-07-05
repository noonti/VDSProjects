/*********************************************************************************
/* Filename    : mmi_comm.c
 * Description :
 * Author      : 
 * Notes       :
 *********************************************************************************/
#include <stdio.h>
#include <string.h>
#include <stdarg.h>

#include "ftms_cpu.h"
#include "systypes.h"
#include "serial.h"
#include "mmi_protocol.h"

//#define 	DEBUG_MMI_COMM

extern uint8 polling_data_valid;
extern TRAFFIC_ST1 gstCenterPolling;
extern TRAFFIC_ST1 gstLocalPolling;

uint8 mmi_data_mode = OP_NORMAL_MODE;
uint8 mmi_disp_index = 0;
uint8 mmi_rt_data_type = 0;

uint8 mmi_now_disp_idx;

uint8 mmi_initialized = FALSE;
MMI_STATE_MACH mmi_init_sts = PRE_INIT_STS;

void sendSysConfigMsgToMMI(void)
{
	uint8 tx_buf[20];
	
	tx_buf[0] = OP_COMM_CFG;
	tx_buf[1] = (uint8) (gstSysConfig.comm_cfg.flow_cntrl);
	tx_buf[2] = (uint8) (gstSysConfig.comm_cfg.bps);
	tx_buf[3] = (uint8) (gstSysConfig.param_cfg.polling_cycle);	
	tx_buf[4] = gucActiveDualLoopNum;
	tx_buf[5] = gucActiveSingleLoopNum;
	tx_buf[6] = (uint8) (gstSysConfig.param_cfg.spd_loop_dist[0]);
	tx_buf[7] = gstSysConfig.myaddr[0];
	tx_buf[8] = gstSysConfig.myaddr[1];

	#if defined(FAN_HEATER_AUTO_CONTROL)
		tx_buf[9] = gstSysConfig.temper[0];
		tx_buf[10] = gstSysConfig.temper[1];
		tx_buf[11] = g_currSysStatus.temper[0];
		tx_buf[12] = g_currSysStatus.temper[1];

		g_currSysStatus.temp_status = 0;
		
		if(g_currSysStatus.temper[0]<0) g_currSysStatus.temp_status |= 0x02;
		if(g_currSysStatus.temper[1]<0) g_currSysStatus.temp_status |= 0x01;

		tx_buf[13] = g_currSysStatus.temp_status;

		#if defined(MMI_PASSWORD)
			tx_buf[14] = gstSysConfig.password[0];
			tx_buf[15] = gstSysConfig.password[1];
			tx_buf[16] =  gstSysConfig.password[2];
			tx_buf[17] =  gstSysConfig.password[3];		
			
			/*(tx_buf[4] = gucActiveLoopNum;	//2011.06.16 by capidra
			tx_buf[5] = (uint8) (gstSysConfig.param_cfg.spd_loop_dist[0]);
			tx_buf[6] = gstSysConfig.myaddr[0];
			tx_buf[7] = gstSysConfig.myaddr[1];
			*/
			pseudo_spi_stream_write(tx_buf, 18); 	//2011.06.16 by capidra 8->9
		#else
			pseudo_spi_stream_write(tx_buf, 14); 
		#endif
	#else
		#if defined(MMI_PASSWORD)
			tx_buf[9] = gstSysConfig.password[0];
			tx_buf[10] = gstSysConfig.password[1];
			tx_buf[11] =  gstSysConfig.password[2];
			tx_buf[12] =  gstSysConfig.password[3];		
			
			pseudo_spi_stream_write(tx_buf, 13); 	//2011.06.16 by capidra 8->9
		#else
			pseudo_spi_stream_write(tx_buf, 9); 
		#endif
	#endif
}

void sendNetConfigMsgToMMI(uint8 index)
{
	int i;
	uint8 tx_buf[26+4], count=0;

#if defined(DEBUG_MMI_COMM)
	fprintf(stdout, "sendNetConfigMsgToMMI(%d)", index);
#endif

	// Opcode
	tx_buf[count++] = OP_NET_CONFIG;
	// Index
	tx_buf[count++] = index;

	switch(index)
	{
		case MMI_CFG_IDX_01_IP_ADDR:
			for(i=0; i<4; i++) tx_buf[count++] = gstNetConfig.ip[i];
			break;

		case MMI_CFG_IDX_02_GATEWAY_IP:
			for(i=0; i<4; i++) tx_buf[count++] = gstNetConfig.gip[i];
			break;

		case MMI_CFG_IDX_03_SUBN_MSK:
			for(i=0; i<4; i++) tx_buf[count++] = gstNetConfig.nmsk[i];
			break;

		case MMI_CFG_IDX_04_SERVER_IP:
			for(i=0; i<4; i++) tx_buf[count++] = gstNetConfig.server_ip[i];
			break;

		case MMI_CFG_IDX_05_SERVER_PORT:
			tx_buf[count++] = gstNetConfig.server_port >> 8;
			tx_buf[count++] = gstNetConfig.server_port & 0xFF;
			break;

		case MMI_CFG_IDX_06_STATION_NUM:
			for(i=0; i<4; i++) tx_buf[count++] = gstNetConfig.station_num[i];
			break;

		case MMI_CFG_IDX_07_TUNNEL_ID: // wwww
			{
				int tid;
				//char tmpStr[7];	// add NULL

				//memcpy(tmpStr, gstNetConfig.tunnel_id, TUNNEL_ID_STR_LEN);
				//tmpStr[TUNNEL_ID_STR_LEN] = NULL;
				//tmpStr[6] = 0;

				tid = 0;

				//fprintf(stdout, " send tid : %x", tid);

				//if (tid >= 0)
				{
					tx_buf[count++] = tid >> 8;
					tx_buf[count++] = tid & 0xFF;
				}
			}
			break;

		case MMI_CFG_IDX_08_CONTR_ID: // wwww
			{
				int cid;
				//char tmpStr[CONTROLLER_ID_STR_LEN+1];	// add NULL

				//memcpy(tmpStr, gstNetConfig.controller_id, CONTROLLER_ID_STR_LEN);
				//tmpStr[CONTROLLER_ID_STR_LEN] = 0;

				cid = 0;

				//fprintf(stdout, " send cid : %x", cid);

				//if (cid >= 0)
				{
					tx_buf[count++] = cid >> 8;
					tx_buf[count++] = cid & 0xFF;
				}
			}
			break;

		case MMI_CFG_IDX_99_ALL_NET_CFG:
			mmi_initialized = FALSE;
			mmi_init_sts = SEND_COMM_CFG;
			return;
			
		default:
			// 아무것도 보내지 않는다.
			return;
			break;
	}
	
	pseudo_spi_stream_write(tx_buf, count);
}


void saveNetConfigFromMMI(uint8 *pCfgBuf)
{
	int i, count=0;
	uint8 opcode = pCfgBuf[count++], bSave = FALSE;
	
#if defined(DEBUG_MMI_COMM)
	fprintf(stdout," New net-config [%d]\n", opcode);
#endif

	//fprintf(stdout," New net-config [%d]\n", opcode);

	// Op Code Check / MMI-CPU SPI Err??? / 추후 검토 2016-09-07
	if(opcode == OP_CHG_IP_SUBN_MSK){
		if(pCfgBuf[1] != 255){
			//fprintf(stdout, " invalid subnet mask 1\n");
			
			sendNetConfigMsgToMMI(MMI_CFG_IDX_03_SUBN_MSK);
			mmi_init_sts = SEND_SUBNET_SMK;

			return;
		}
	}
	if(opcode == OP_CHG_IP_SERVER_PORT){

		//fprintf(stdout," SERVER_PORT = %05d\n", (pCfgBuf[1] << 8) + pCfgBuf[2]);
		//fprintf(stdout," pCfgBuf[] %02x : %02d - %02d\n", pCfgBuf[0], pCfgBuf[1], pCfgBuf[2]);

		// 도공 30100 pCfgBuf[1] = 117(0x75), pCfgBuf[2] = 148(0x94)		
		if(pCfgBuf[1] != 117){
			//fprintf(stdout, " invalid server port 1\n");
			
			sendNetConfigMsgToMMI(MMI_CFG_IDX_05_SERVER_PORT);
			mmi_init_sts = SEND_SERVER_PORT;

			return;
		}
	}

	/*
	// 네트웍 설정값 오류/예외처리 2016-09-13
	if(opcode == OP_CHG_IP_IP_ADDR){
		if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0) return;
	}	
	if(opcode == OP_CHG_IP_GATEWAY_IP){
		if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0) return;
	}	
	if(opcode == OP_CHG_IP_SUBN_MSK){
		if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0) return;
	}	
	if(opcode == OP_CHG_IP_SERVER_IP){
		if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0) return;
	}
	if(opcode == OP_CHG_IP_SERVER_PORT){
		if((pCfgBuf[1] << 8) + pCfgBuf[2] == 0) return;
	}
	if(opcode == OP_CHG_IP_STATION_NUM){
		if((pCfgBuf[1] << 8) + pCfgBuf[2] == 0) return;
		else if((pCfgBuf[3] << 8) + pCfgBuf[4] == 0) return;
	}
	*/

	switch(opcode)
	{
		case OP_CHG_IP_IP_ADDR:
			{
				uint8 tmpIP[IP_NUM_LEN];

				if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0){
					//fprintf(stdout, " invalid ip address\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_01_IP_ADDR);
					mmi_init_sts = SEND_IP_ADDRESS;

					return;
				} 

				//for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[count++];
				for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[i+1];

				if (memcmp(gstNetConfig.ip, tmpIP, IP_NUM_LEN))
				{
					for(i=0; i<IP_NUM_LEN; i++) gstNetConfig.ip[i] = tmpIP[i];
					
					fprintf(stdout, " New ip address : %03d.%03d.%03d.%03d\n", 
						gstNetConfig.ip[0], gstNetConfig.ip[1], gstNetConfig.ip[2], gstNetConfig.ip[3]);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %03d.%03d.%03d.%03d\n", 
						tmpIP[0], tmpIP[1], tmpIP[2], tmpIP[3]);
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_GATEWAY_IP:
			{
				uint8 tmpIP[IP_NUM_LEN];

				if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0){
					//fprintf(stdout, " invalid gateway ip\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_02_GATEWAY_IP);
					mmi_init_sts = SEND_GATEWAY_IP;

					return;
				}

				//for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[count++];
				for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[i+1];

				//fprintf(stdout, " rev gateway ip : [%x] [%d] [%d] [%d] [%d]\n", pCfgBuf[0], pCfgBuf[1], pCfgBuf[2], pCfgBuf[3], pCfgBuf[4]);

				if (memcmp(gstNetConfig.gip, tmpIP, IP_NUM_LEN))
				{
					for(i=0; i<IP_NUM_LEN; i++) gstNetConfig.gip[i] = tmpIP[i];
					
					fprintf(stdout, " New gateway ip : %03d.%03d.%03d.%03d\n", 
						gstNetConfig.gip[0], gstNetConfig.gip[1], gstNetConfig.gip[2], gstNetConfig.gip[3]);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %03d.%03d.%03d.%03d\n",
						tmpIP[0], tmpIP[1], tmpIP[2], tmpIP[3]);
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_SUBN_MSK:
			{
				uint8 tmpIP[IP_NUM_LEN];

				if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0){
					//fprintf(stdout, " invalid subnet mask 2\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_03_SUBN_MSK);
					mmi_init_sts = SEND_SUBNET_SMK;

					return;
				}

				//for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[count++];
				for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[i+1];	

				if (memcmp(gstNetConfig.nmsk, tmpIP, IP_NUM_LEN))
				{
					for(i=0; i<IP_NUM_LEN; i++) gstNetConfig.nmsk[i] = tmpIP[i];

					fprintf(stdout, " New subnet mask : %03d.%03d.%03d.%03d\n", 
						gstNetConfig.nmsk[0], gstNetConfig.nmsk[1], gstNetConfig.nmsk[2], gstNetConfig.nmsk[3]);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %03d.%03d.%03d.%03d\n", 
						tmpIP[0], tmpIP[1], tmpIP[2], tmpIP[3]);
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_SERVER_IP:
			{
				uint8 tmpIP[IP_NUM_LEN];

				if(pCfgBuf[1] == 0 || pCfgBuf[2] == 0){
					//fprintf(stdout, " invalid server ip\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_04_SERVER_IP);
					mmi_init_sts = SEND_SERVER_IP;

					return;
				}

				//for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[count++];
				for(i=0; i<IP_NUM_LEN; i++) tmpIP[i] = pCfgBuf[i+1];	
			
				if (memcmp(gstNetConfig.server_ip, tmpIP, IP_NUM_LEN))
				{
					for(i=0; i<IP_NUM_LEN; i++) gstNetConfig.server_ip[i] = tmpIP[i];
					
					fprintf(stdout, " New server ip : %03d.%03d.%03d.%03d\n", 
						gstNetConfig.server_ip[0], gstNetConfig.server_ip[1], gstNetConfig.server_ip[2], gstNetConfig.server_ip[3]);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %03d.%03d.%03d.%03d\n", 
						tmpIP[0], tmpIP[1], tmpIP[2], tmpIP[3]);
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_SERVER_PORT:
			{
				uint16 tmpPort;

				//uint16 tmpPort = (pCfgBuf[count++] << 8) + pCfgBuf[count++];
				tmpPort = (pCfgBuf[1] << 8) + pCfgBuf[2];

				if(tmpPort == 0){
					//fprintf(stdout, " invalid server port\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_05_SERVER_PORT);
					mmi_init_sts = SEND_SERVER_PORT;

					return;
				}
				
				//fprintf(stdout, " rev server port : [%x] [%x] [%x]\n", pCfgBuf[0], pCfgBuf[1], pCfgBuf[2]);
				
				if (tmpPort != gstNetConfig.server_port)
				{
					gstNetConfig.server_port = tmpPort;

					fprintf(stdout, " New server port : %d\n", gstNetConfig.server_port);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %d\n", tmpPort);
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_STATION_NUM:
			{
				uint8 tmpStNum[STATION_NUM_LEN];
				uint16 usTmpStNum[2];
				
				//for(i=0; i<STATION_NUM_LEN; i++) tmpStNum[i] = pCfgBuf[count++];
				for(i=0; i<STATION_NUM_LEN; i++) tmpStNum[i] = pCfgBuf[i+1];	

				usTmpStNum[0] = (tmpStNum[0]<<8) + tmpStNum[1];
				usTmpStNum[1] = (tmpStNum[2]<<8) + tmpStNum[3];

				if(usTmpStNum[0] == 0 || usTmpStNum[1] == 0){
					//fprintf(stdout, " invalid station num\n");
					
					sendNetConfigMsgToMMI(MMI_CFG_IDX_06_STATION_NUM);
					mmi_init_sts = SEND_STATION_NUM;

					return;
				}

				if (memcmp(gstNetConfig.station_num, tmpStNum, STATION_NUM_LEN))
				{
					for(i=0; i<STATION_NUM_LEN; i++) gstNetConfig.station_num[i] = tmpStNum[i];

					#if 1
					fprintf(stdout, " New station num : %05d-%05d\n", usTmpStNum[0], usTmpStNum[1]);
					#else
					fprintf(stdout, " New station num : %03d.%03d-%03d.%03d\n", \
						gstNetConfig.station_num[0], gstNetConfig.station_num[1], gstNetConfig.station_num[2], gstNetConfig.station_num[3]);
					#endif
					
					bSave = TRUE;
				}
				else
				{
					#if 1
					fprintf(stdout, " ########## Same value : %05d-%05d\n", usTmpStNum[0], usTmpStNum[1]);
					#else
					fprintf(stdout, " ########## Same value : %03d.%03d.%03d.%03d\n", \
						tmpIP[0], tmpIP[1], tmpIP[2], tmpIP[3]);
					#endif
					
					bSave = FALSE;
				}
			}
			break;

		case OP_CHG_IP_TUNNEL_ID: // eeee
			{
				/*  Delete by capi 2014.08
				char strID[TUNNEL_ID_STR_LEN+1]; // add NULL
				uint16 tmpID;

				tmpID = (pCfgBuf[count++] << 8) + pCfgBuf[count++];
				num2BcdStr(strID, tmpID, TUNNEL_ID_STR_LEN);
				strID[TUNNEL_ID_STR_LEN] = 0;

				if (memcmp(gstNetConfig.tunnel_id, strID, TUNNEL_ID_STR_LEN))
				{
					memcpy(gstNetConfig.tunnel_id, strID, TUNNEL_ID_STR_LEN);

					fprintf(stdout, " New tunnel id : %d [%s]\n", tmpID, strID);
					makeDeviceIDSting(gstNetConfig.tunnel_id, gstNetConfig.controller_id, TRUE);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %d [%s]\n", tmpID, strID);
					bSave = FALSE;
				}
				*/
			}
			break;

		case OP_CHG_IP_CONTR_ID: // eeee
			{
				/* Delete by capi 2014.08
				char strID[CONTROLLER_ID_STR_LEN+1]; // add NULL
				uint16 tmpID;

				tmpID = (pCfgBuf[count++] << 8) + pCfgBuf[count++];
				num2BcdStr(strID, tmpID, CONTROLLER_ID_STR_LEN);
				strID[CONTROLLER_ID_STR_LEN] = 0;

				if (memcmp(gstNetConfig.controller_id, strID, CONTROLLER_ID_STR_LEN))
				{
					//for(i=0; i<CONTROLLER_ID_STR_LEN; i++) gstNetConfig.controller_id[i] = strID[i];
					memcpy(gstNetConfig.controller_id, strID, CONTROLLER_ID_STR_LEN);

					fprintf(stdout, " New controller id : %d [%s]\n", tmpID, strID);
					makeDeviceIDSting(gstNetConfig.tunnel_id, gstNetConfig.controller_id, TRUE);
					bSave = TRUE;
				}
				else
				{
					fprintf(stdout, " ########## Same value : %d [%s]\n", tmpID, strID);
					bSave = FALSE;
				}	
				*/			
			}
			break;

		default:
			return;
			break;
	}

	// 저장한다.
	if (bSave == TRUE)
	{
		setInterfaces();	//by capi interface file write
		writeNetConfigToNAND(TRUE);
		dumpNetworkConfig();
	}
}

time_delay(int t)
{
	int i,j;
	for(i=0;i<t;i++)
		for(j=0;j<10000;j++)
			;
}

void mmicomtask(void)
{
	static uint8 send_index = 0;
	uint8 tmp_buff[50], tx_buf[20], tmp_size =0, opcode;
	int i,tmpv, dlane;
	

	/////////////////////////////////////////////////////////////
	// MMI 초기화 완료를 위해서 각종 설정값을 보내줘야 됨.
	if (mmi_initialized == FALSE) 
	{
		switch(mmi_init_sts)
		{
			case PRE_INIT_STS :
				if ( tmp_size = pseudo_spi_stream_read(tmp_buff) )
				{
					opcode = tmp_buff[0];
#if defined(DEBUG_MMI_COMM)
					fprintf(stdout,"Init opcode = 0x%x, %c\n", opcode, opcode);
#endif
					tx_buf[0] = OP_REV_ACK;
					tx_buf[1] = opcode;
					pseudo_spi_stream_write(tx_buf, 2);

					switch(opcode ) {
						
						case OP_INITIALIZED :
							
							mmi_init_sts = RECVED_INIT_MSG;
#if defined(DEBUG_MMI_COMM)
							fprintf(stdout,"\n\r RECVED_INIT_MSG\n\r");
#endif
							break;
					}
				}
				break;

			// MMI에서 Init 메세지를 받은 이후. 각종 설정 데이터를 넘겨야 됨.
			// 먼저 날짜부터.
			case RECVED_INIT_MSG :
				// 날짜를 보냄.
				tx_buf[0] = OP_DATE_SET;
				tx_buf[1] = (uint8) (sys_time.wYear - 2000);
				tx_buf[2] = (uint8) (sys_time.wMonth);
				tx_buf[3] = (uint8) (sys_time.wDay);
				pseudo_spi_stream_write(tx_buf, 4);

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_DATE\n\r");
#endif

				// 다음 시간을 보낼 것임.
				mmi_init_sts = SEND_DATE;
				break;

			case SEND_DATE :
				// 시간을 보낸다.
				tx_buf[0] = OP_TIME_SET;
				tx_buf[1] = (uint8) (sys_time.wHour);
				tx_buf[2] = (uint8) (sys_time.wMinute);
				tx_buf[3] = (uint8) (sys_time.wSecond);
				pseudo_spi_stream_write(tx_buf, 4);

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_TIME\n\r");
#endif

				// 통신 설정을 보낼 것임.
				mmi_init_sts = SEND_TIME;
				break;

			case SEND_TIME :
				// 통신 설정을 보낸다.
				tx_buf[0] = OP_COMM_CFG;
				tx_buf[1] = (uint8) (gstSysConfig.comm_cfg.flow_cntrl);
				tx_buf[2] = (uint8) (gstSysConfig.comm_cfg.bps);
				tx_buf[3] = (uint8) (gstSysConfig.param_cfg.polling_cycle);

				tx_buf[4] = (uint8) (gucActiveDualLoopNum);	//2011.06.16 by capidra
				tx_buf[5] = (uint8) (gucActiveSingleLoopNum);
				tx_buf[6] = (uint8) (gstSysConfig.param_cfg.spd_loop_dist[0]);
				tx_buf[7] = gstSysConfig.myaddr[0];
				tx_buf[8] = gstSysConfig.myaddr[1];

#if defined(FAN_HEATER_AUTO_CONTROL)
				tx_buf[9] = gstSysConfig.temper[0];	//2011.08.30 by capidra
				tx_buf[10] = gstSysConfig.temper[1];
				tx_buf[11] = g_currSysStatus.temper[0];
				tx_buf[12] = g_currSysStatus.temper[1];

				if(g_currSysStatus.temper[0]<0) g_currSysStatus.temp_status |= 0x02;
				if(g_currSysStatus.temper[1]<0) g_currSysStatus.temp_status |= 0x01;

				tx_buf[13] = g_currSysStatus.temp_status;

				#if defined(MMI_PASSWORD)	//2012.04.4 by capidra 
					tx_buf[14] = gstSysConfig.password[0];
					tx_buf[15] = gstSysConfig.password[1];
					tx_buf[16] =  gstSysConfig.password[2];
					tx_buf[17] =  gstSysConfig.password[3];						
					
					pseudo_spi_stream_write(tx_buf, 18); 					
				#else
					pseudo_spi_stream_write(tx_buf, 14); 
				#endif					
#else
				#if defined(MMI_PASSWORD)			//2012.04.4 by capidra 
					tx_buf[9] = gstSysConfig.password[0];
					tx_buf[10] = gstSysConfig.password[1];
					tx_buf[11] =  gstSysConfig.password[2];
					tx_buf[12] =  gstSysConfig.password[3];	
					
					/*(tx_buf[4] = gucActiveLoopNum;	//2011.06.16 by capidra
					tx_buf[5] = (uint8) (gstSysConfig.param_cfg.spd_loop_dist[0]);
					tx_buf[6] = gstSysConfig.myaddr[0];
					tx_buf[7] = gstSysConfig.myaddr[1];
					*/
					pseudo_spi_stream_write(tx_buf, 13); 					
				#else
					pseudo_spi_stream_write(tx_buf, 9); 
				#endif					
				
#endif
#if defined(DEBUG_MMI_COMM)
				fprintf(stdout,"\n\r SEND_COMM_CFG\n\r");
#endif

				mmi_init_sts = SEND_COMM_CFG;
				break;			
			case SEND_COMM_CFG:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_01_IP_ADDR);
				mmi_init_sts = SEND_IP_ADDRESS;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_IP_ADDRESS\n\r");
#endif
				break;

			case SEND_IP_ADDRESS:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_02_GATEWAY_IP);
				mmi_init_sts = SEND_GATEWAY_IP;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_GATEWAY_IP\n\r");
#endif
				break;
				
			case SEND_GATEWAY_IP:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_03_SUBN_MSK);
				mmi_init_sts = SEND_SUBNET_SMK;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_SUBNET_SMK\n\r");
#endif
				break;
				
			case SEND_SUBNET_SMK:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_04_SERVER_IP);
				mmi_init_sts = SEND_SERVER_IP;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_SERVER_IP\n\r");
#endif
				break;
				
			case SEND_SERVER_IP:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_05_SERVER_PORT);
				mmi_init_sts = SEND_SERVER_PORT;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_SERVER_PORT\n\r");
#endif
				break;
				
			case SEND_SERVER_PORT:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_06_STATION_NUM);
				mmi_init_sts = SEND_STATION_NUM;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_STATION_NUM\n\r");
#endif
				break;
			case SEND_STATION_NUM:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_07_TUNNEL_ID);
				mmi_init_sts = SEND_TUNNEL_ID;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_TUNNEL_ID\n\r");
#endif
				break;
				
			case SEND_TUNNEL_ID:
				sendNetConfigMsgToMMI(MMI_CFG_IDX_08_CONTR_ID);
				mmi_init_sts = SEND_CONTROL_ID;

#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_CONTROL_ID\n\r");
#endif
				break;
				
			case SEND_CONTROL_ID:
#if defined(DEBUG_MMI_COMM)
				fprintf(stdout, "\n\r SEND_FINISHED\n\r");
#endif				
				mmi_init_sts = SEND_FINISHED;
				mmi_initialized = TRUE;
				break;
			default :
				break;
		}
		
		return;
	}
	/////////////////////////////////////////////////////////////

	
	if (tmp_size = pseudo_spi_stream_read(tmp_buff))
	{
		opcode = tmp_buff[0];

#if defined(DEBUG_MMI_COMM)
		fprintf(stdout, "mmicomm_task opcode = 0x%x (%c) tmp_size = %d\n", opcode, opcode, tmp_size);
#endif

#if 0
		fprintf(stdout, "mmicomm_task opcode = 0x%x (%c) tmp_size = %d\n", opcode, opcode, tmp_size);

		fprintf(stdout, "MMI recv x : ");
		for(i=0; i<tmp_size; i++) fprintf(stdout, " [%x]", tmp_buff[i]);
		fprintf(stdout, "\n");
#endif

		if (opcode == OP_REALTIME_MODE || opcode == OP_POLLING_MODE)
		{
			tx_buf[0] = OP_COMM_CFG;
			tx_buf[1] = (uint8) (gstSysConfig.comm_cfg.flow_cntrl);
			tx_buf[2] = (uint8) (gstSysConfig.comm_cfg.bps);
			tx_buf[3] = (uint8) (gstSysConfig.param_cfg.polling_cycle);

			tx_buf[4] = (uint8) (gucActiveDualLoopNum);		//2011.06.16 by capidra
			tx_buf[5] = (uint8) (gucActiveSingleLoopNum);
			tx_buf[6] = (uint8) (gstSysConfig.param_cfg.spd_loop_dist[0]);
			tx_buf[7] = gstSysConfig.myaddr[0];
			tx_buf[8] = gstSysConfig.myaddr[1];

#if defined(FAN_HEATER_AUTO_CONTROL)

			tx_buf[9] = gstSysConfig.temper[0];
			tx_buf[10] = gstSysConfig.temper[1];
			tx_buf[11] = g_currSysStatus.temper[0];
			tx_buf[12] = g_currSysStatus.temper[1];

			if(g_currSysStatus.temper[0]<0) g_currSysStatus.temp_status |= 0x02;
			if(g_currSysStatus.temper[1]<0) g_currSysStatus.temp_status |= 0x01;

			tx_buf[13] = g_currSysStatus.temp_status;
			
			#if defined(MMI_PASSWORD)		//2012.04.4 by capidra 
				tx_buf[14] = gstSysConfig.password[0];
				tx_buf[15] = gstSysConfig.password[1];
				tx_buf[16] =  gstSysConfig.password[2];
				tx_buf[17] =  gstSysConfig.password[3];						
				
				pseudo_spi_stream_write(tx_buf, 18); 					
			#else
				pseudo_spi_stream_write(tx_buf, 14); 
			#endif				
			
#else
			#if defined(MMI_PASSWORD)		//2012.04.4 by capidra 
				tx_buf[9] = gstSysConfig.password[0];
				tx_buf[10] = gstSysConfig.password[1];
				tx_buf[11] =  gstSysConfig.password[2];
				tx_buf[12] =  gstSysConfig.password[3];						
				
				pseudo_spi_stream_write(tx_buf, 13); 					
			#else
				pseudo_spi_stream_write(tx_buf, 9); 
			#endif			
			
#endif
		}
		else if (opcode == OP_REQ_TIME)	// MMI 
		{
			// 먼저 날짜를 전달.
			// by jwank 060118
			tx_buf[0] = OP_DATE_SET;
			tx_buf[1] = (uint8) (sys_time.wYear - 2000);
			tx_buf[2] = (uint8) (sys_time.wMonth);
			tx_buf[3] = (uint8) (sys_time.wDay);
			pseudo_spi_stream_write(tx_buf, 4);

			// 시간을 전달.
			tx_buf[0] = OP_TIME_SET;
			tx_buf[1] = (uint8) (sys_time.wHour);
			tx_buf[2] = (uint8) (sys_time.wMinute);
			tx_buf[3] = (uint8) (sys_time.wSecond);
			pseudo_spi_stream_write(tx_buf, 4);
		}

		tx_buf[0] = OP_REV_ACK;
		tx_buf[1] = tmp_buff[0];
		pseudo_spi_stream_write(tx_buf, 2);

		switch(opcode)
		{		
		case OP_INITIALIZED :
			
			mmi_init_sts = RECVED_INIT_MSG;			
			mmi_initialized = FALSE;
			mmi_data_mode = OP_NORMAL_MODE;
				
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\r##Receive init msg. from mmi\n\r");
#endif
			break;
		
		case OP_TIME_CHG :
			{
			SYSTEMTIME tmp_time;
			int j;
			char timebuf[255];
			//for( j=0;j<7;j++) 
			//	fprintf(stdout,"tmp_buf[%d]=%d\n", j,tmp_buff[j]);

			if(tmp_buff[2] == 0 || tmp_buff[3] == 0){
				//fprintf(stdout, " invalid Date\n");
				
				tx_buf[0] = OP_DATE_SET;
				tx_buf[1] = (uint8) (sys_time.wYear - 2000);
				tx_buf[2] = (uint8) (sys_time.wMonth);
				tx_buf[3] = (uint8) (sys_time.wDay);
				pseudo_spi_stream_write(tx_buf, 4);

				return;
			}			

			tmp_time.wYear = tmp_buff[1] + 2000;
			tmp_time.wMonth = tmp_buff[2];
			tmp_time.wDay = tmp_buff[3];
			tmp_time.wHour = tmp_buff[4];
			tmp_time.wMinute = tmp_buff[5];
			tmp_time.wSecond = tmp_buff[6];

			//OS Time change
			sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
							,tmp_time.wYear, tmp_time.wMonth, tmp_time.wDay
							, tmp_time.wHour, tmp_time.wMinute, tmp_time.wSecond);
			system(timebuf);

			// 시간을 RTC 에 설정.
			#if 1
			//sleep(1);
			//fprintf(stdout,"msec_sleep\n");
			//msec_sleep(50000);
			usleep(1000);
			setSysTimeToRTC(&tmp_time);	//rtc time change			
			#else
			set_rtc_systime(&tmp_time);
			rtc_write(0x0e, 0x18);
			rtc_write(0x0f, 0x00);
			#endif
			
			
#if 1 //defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rChange Time : ");
			fprintf(stdout, "%d/%d/%d %d:%d:%d\n\r", tmp_time.wYear, tmp_time.wMonth, \
					tmp_time.wDay, tmp_time.wHour, tmp_time.wMinute, tmp_time.wSecond);
#endif
			}
			break;

		// MMI 에서 BPS 가 바뀌었음을 알림.
		case OP_VDS_BPS :
			if ( gstSysConfig.comm_cfg.bps != tmp_buff[1] \
				&& tmp_buff[1] <= MAX_COMM_BPS )
			{
				gstSysConfig.comm_cfg.bps = tmp_buff[1];
				//InitialiseDuartChannel(CHANNEL_C, gstSysConfig.comm_cfg.bps, 0, 0, 0);
				writeParamToNAND(TRUE);
			}
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rChange VDS BPS : %d\n\r", tmp_buff[1]);
#endif
			break;

		// MMI 에서 센터 주기가 바뀌었음을 알림.
		case OP_SYNC_TIME:
			if ( gstSysConfig.param_cfg.polling_cycle != tmp_buff[1] )
			{
				if(tmp_buff[1] < 14){
					//fprintf(stdout, " invalid Sync time = %d\n", tmp_buff[1]);

					return;
				} 

				gstSysConfig.param_cfg.polling_cycle = tmp_buff[1];
				writeParamToNAND(TRUE);
			}
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rChange Sync time : %d\n\r", tmp_buff[1]);
#endif
			break;

		// MMI에서 Loop 수가 바뀌었음을 알림.
		case OP_MAX_LOOP :			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rChange Max loop : %d\n\r", tmp_buff[1]);
#endif
			{
				uint8 j, t1, t2, msk, p_param[10];

				if(tmp_buff[1] == 0 && tmp_buff[2] == 0){
					//fprintf(stdout, " invalid Max loop\n");

					return;
				}				
				
				tmpv = tmp_buff[1];

				if (tmpv > MAX_RIO_LOOP_NUM ) tmpv = MAX_RIO_LOOP_NUM;
						
				for (i=0; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

				t1 = tmpv / 8;
				t2 = tmpv % 8;
		
				for (i=0; i<t1; i++) p_param[i] = 0xff;
				for (j=0, msk=1; j<t2; j++, msk<<=1) p_param[i] |= msk;
				i++;
				for (; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

				//gucActiveLoopNum = tmpv;
				gucActiveDualLoopNum = tmpv;			//2011.06.16 by capidra
				gucActiveSingleLoopNum= tmp_buff[2];	//2011.06.16 by capidra

				gucActiveLoopNum = gucActiveDualLoopNum + gucActiveSingleLoopNum*2;
				
				for(i=0;i<MAX_LANE_NUM;i++) gucDivDualLoopLane[i] = 0;

				dlane = gucActiveDualLoopNum/2;
				for(i=0;i<gucActiveSingleLoopNum;i++) gucDivDualLoopLane[dlane+i] = 1;

				if ( gucActiveLoopNum > MAX_RIO_LOOP_NUM ) gucActiveLoopNum = MAX_RIO_LOOP_NUM;

				for (i=0; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

				t1 = gucActiveLoopNum / 8;
				t2 = gucActiveLoopNum % 8;
		
				for (i=0; i<t1; i++) p_param[i] = 0xff;
				for (j=0, msk=1; j<t2; j++, msk<<=1) p_param[i] |= msk;
				i++;
				for (; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;					

				putVdsParameter(IDX_01_LOOP_ENABLE, p_param, TRUE);
			}				
			break;
		
		case OP_LOOP_GAP :
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\r New gap of loop : %d\n\r", tmp_buff[1]);
#endif
			if(tmp_buff[1] < 15){
				//fprintf(stdout, " invalid gap of loop\n");
				
				return;
			}
	
			tmpv = tmp_buff[1];

			if ( tmpv <= 0 ) tmpv = 45;		// default is 45 DM;
			else if ( tmpv > 250 ) tmpv = 250;

			for (i=0; i<MAX_LANE_NUM; i++)
				gstSysConfig.param_cfg.spd_loop_dist[i] = tmpv;

			pullSpeedLoopDim();

			// out of defalut parameter.
			gstSysConfig.is_default_param = _USER_SETTING_PARAM_;
			// write parameter to flash.
			writeParamToNAND(TRUE);
			break;
		
		case OP_ID_ADDR :
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\r New Address : %d-%d\n\r", tmp_buff[1], tmp_buff[2]);
#endif

			if(tmp_buff[1] == 0 || tmp_buff[2] == 0){
				//fprintf(stdout, " invalid Address\n");
				
				return;
			}
	
			gstSysConfig.myaddr[0] = tmp_buff[1];
			gstSysConfig.myaddr[1] = tmp_buff[2];
			
			// write parameter to flash.
			writeParamToNAND(TRUE);
			break;			
		
		case OP_REQ_TEMP :
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout,"\n\r New Temperature : %d-%d\n\r", tmp_buff[1], tmp_buff[2]);
#endif

			if(tmp_buff[1] == 0 || tmp_buff[2] == 0){
				//fprintf(stdout, " invalid Temperature\n");

				return;
			}

			gstSysConfig.temper[0] = tmp_buff[1];
			gstSysConfig.temper[1] = tmp_buff[2];
						
			//pullSpeedLoopDim();

			// out of defalut parameter.
			gstSysConfig.is_default_param = _USER_SETTING_PARAM_;
			// write parameter to flash.
			writeParamToNAND(TRUE);
			break;

		case OP_REQ_PASS :
						
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout,"\n\r New Password : %d%d%d%d\n\r", tmp_buff[1], tmp_buff[2], tmp_buff[3], tmp_buff[4]);
#endif
			fprintf(stdout,"\n\r New Password : %d%d%d%d\n\r", tmp_buff[1], tmp_buff[2], tmp_buff[3], tmp_buff[4]);

			gstSysConfig.password[0] = tmp_buff[1];
			gstSysConfig.password[1] = tmp_buff[2];
			gstSysConfig.password[2] = tmp_buff[3];
			gstSysConfig.password[3] = tmp_buff[4];
						
			//pullSpeedLoopDim();

			// out of defalut parameter.
			gstSysConfig.is_default_param = _USER_SETTING_PARAM_;
			// write parameter to flash.
			writeParamToNAND(TRUE);
			break;
		
		case OP_CHG_IP_IP_ADDR:
		case OP_CHG_IP_GATEWAY_IP:
		case OP_CHG_IP_SUBN_MSK:
		case OP_CHG_IP_SERVER_IP:
		case OP_CHG_IP_SERVER_PORT:
		case OP_CHG_IP_STATION_NUM:
		case OP_CHG_IP_TUNNEL_ID:
		case OP_CHG_IP_CONTR_ID:
			{
				saveNetConfigFromMMI(&tmp_buff[0]);
			}
			break;
		
		case OP_NORMAL_MODE :
			mmi_data_mode = OP_NORMAL_MODE;
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rNormal Mode\n\r");
#endif
			break;
		
		case OP_POLLING_MODE :
			mmi_data_mode = OP_POLLING_MODE;
			mmi_disp_index = tmp_buff[1];
			
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rPolling Mode [%d]\n\r", tmp_buff[1]);
#endif
			break;
		
		case OP_REALTIME_MODE :
			mmi_data_mode = OP_REALTIME_MODE;
			mmi_disp_index = tmp_buff[1];
			mmi_rt_data_type = tmp_buff[2];

			mmi_now_disp_idx = mmi_disp_index;
				
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rRealtime Mode [%d][%d]\n\r", tmp_buff[1], tmp_buff[2]);
#endif
			break;
			
		default :
			break;
		}
		
		return;
	}
	/////////////////////////////////////////////////////////////


	/////////////////////////////////////////////////////////////
	if ( mmi_data_mode == OP_POLLING_MODE )
	{
		POLLING_DATA_FOR_MMI mmi_pkt;

		if ( polling_data_valid == TRUE )
		{
#if defined(DEBUG_MMI_COMM)
			fprintf(stdout, "\n\rSending Polling data to MMI");
#endif
			send_index = 0;
			polling_data_valid = FALSE;
		}
	
		//if (send_index >= MAX_LANE_NUM) return;
		if (send_index >= MAX_RIO_LANE_NUM) return;

		mmi_pkt.opcode = OP_POLLING_DATA;
		mmi_pkt.index = send_index;

		mmi_pkt.speed = gstCenterPolling.speed[send_index];
		mmi_pkt.length = gstCenterPolling.length[send_index];
		mmi_pkt.volume = gstCenterPolling.volume[gstLaneLink[send_index].e_loop];

		#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
		mmi_pkt.occupy = round_for_occupy(gstCenterPolling.occupy[gstLaneLink[send_index].e_loop], 0);
		#else
		mmi_pkt.occupy = gstCenterPolling.occupy[gstLaneLink[send_index].e_loop];
		#endif

		mmi_pkt.end_ch = _END_CH_;

		pseudo_spi_stream_write((uint8 *) &mmi_pkt, sizeof (POLLING_DATA_FOR_MMI));

		send_index++;
		
	}
	else if ( mmi_data_mode == OP_REALTIME_MODE )
	{
		mmi_now_disp_idx++;
		if ( mmi_now_disp_idx > MAX_RIO_LANE_NUM || mmi_now_disp_idx > (mmi_disp_index+4) )
			mmi_now_disp_idx = mmi_disp_index;

		if ( realtime_data_valid[mmi_now_disp_idx] != EVENT_CREATED )  return;
		
		if ( mmi_rt_data_type == 0 )
		{
			RT_DATA_T0_FOR_MMI mmi_pkt;

			mmi_pkt.opcode = OP_REALTIME_DATA;
			mmi_pkt.index = mmi_now_disp_idx;
			mmi_pkt.rt_type = 0;

			mmi_pkt.volume = gstCenterAccuData.volume[gstLaneLink[mmi_now_disp_idx].e_loop];

#if 1			
			mmi_pkt.occupy[0][0] = gstCurrData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop] >> 8;
			mmi_pkt.occupy[0][1] = (uint8) gstCurrData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop];
			mmi_pkt.occupy[1][0] = gstCurrData.occupy[gstLaneLink[mmi_now_disp_idx].e_loop] >> 8;
			mmi_pkt.occupy[1][1] = (uint8) gstCurrData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop];
#else
			mmi_pkt.occupy[0][0] = gstCenterAccuData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop] >> 8;
			mmi_pkt.occupy[0][1] = (uint8) gstCenterAccuData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop];
			mmi_pkt.occupy[1][0] = gstCenterAccuData.occupy[gstLaneLink[mmi_now_disp_idx].e_loop] >> 8;
			mmi_pkt.occupy[1][1] = (uint8) gstCenterAccuData.occupy[gstLaneLink[mmi_now_disp_idx].s_loop];
#endif

			mmi_pkt.end_ch = _END_CH_;

			pseudo_spi_stream_write((uint8 *) &mmi_pkt, sizeof (RT_DATA_T0_FOR_MMI));

			realtime_data_valid[mmi_now_disp_idx] = NO_EVENT;
		}
		else if ( mmi_rt_data_type == 1 )
		{
			RT_DATA_T1_FOR_MMI mmi_pkt;

			mmi_pkt.opcode = OP_REALTIME_DATA;
			mmi_pkt.index = mmi_now_disp_idx;
			mmi_pkt.rt_type = 1;

			#if defined(INCREASE_SPEED_PRECISION)
			// bug fix by jwank 090916
			mmi_pkt.speed = gstCurrData.speed[mmi_now_disp_idx] / 100;
			#else
			mmi_pkt.speed = gstCurrData.speed[mmi_now_disp_idx];
			#endif
			mmi_pkt.length = gstCurrData.length[mmi_now_disp_idx];

			mmi_pkt.end_ch = _END_CH_;

			pseudo_spi_stream_write((uint8 *) &mmi_pkt, sizeof (RT_DATA_T1_FOR_MMI));

			realtime_data_valid[mmi_now_disp_idx] = NO_EVENT;
		}
		else if ( mmi_rt_data_type == 2 )
		{
			RT_DATA_T2_FOR_MMI mmi_pkt;

			mmi_pkt.opcode = OP_REALTIME_DATA;
			mmi_pkt.index = mmi_now_disp_idx;
			mmi_pkt.rt_type = 2;

			mmi_pkt.acc_vol[0][0] = gstReportsOfLoop[gstLaneLink[mmi_now_disp_idx].s_loop].volume >> 8;
			mmi_pkt.acc_vol[0][1] = (uint8) gstReportsOfLoop[gstLaneLink[mmi_now_disp_idx].s_loop].volume;
			mmi_pkt.acc_vol[1][0] = gstReportsOfLoop[gstLaneLink[mmi_now_disp_idx].e_loop].volume >> 8;
			mmi_pkt.acc_vol[1][1] = (uint8) gstReportsOfLoop[gstLaneLink[mmi_now_disp_idx].e_loop].volume;

			mmi_pkt.end_ch = _END_CH_;

			pseudo_spi_stream_write((uint8 *) &mmi_pkt, sizeof (RT_DATA_T2_FOR_MMI));

			realtime_data_valid[mmi_now_disp_idx] = NO_EVENT;
		}
		
	}
}