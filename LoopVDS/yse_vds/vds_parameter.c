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

#include "ftms_cpu.h"
#include "serial.h"
#include "tcpip.h"
#include "vds_parameter.h"
#include "mmi_protocol.h"
//#include "Plat_config.h"

#define DEBUG_PARAMETER

char sysStrMyIpAddr[IP_STR_LEN+1];
char sysStrOptIpAddr[IP_STR_LEN+1];
char sysStrServerIp[IP_STR_LEN+1];
//char sysStrDeviceID[DEVICE_ID_STR_LEN] = DEVICE_ID_STR;

unsigned int dbgWriteCount;

int para_fd; //parameter file descripter
int netpara_fd; //parameter file descripter

void pullSpeedLoopDim(void)
{
	uint32 i;
	
	for (i=0; i<MAX_LANE_NUM; i++) 
		loop_diam[i] = gstSysConfig.param_cfg.spd_loop_diam[i];
	
	for (i=0; i<MAX_LANE_NUM; i++) 
		lane_dist[i] = gstSysConfig.param_cfg.spd_loop_dist[i];
}

BOOL writeParamToNAND(uint8 erase)
{
	int stick, etick;
	uint16 crc16;
	u8 *src_ptr = (u8 *)&gstSysConfig;
	
	// Calculate CRC-16 for gstSysConfig.
	crc16 = updateCRC16((uint8 *) &gstSysConfig, sizeof(SYS_CONFIG_OLD_t), 0);
	//fprintf(stdout," - CRC val[0x%04x]", crc16);

	// 웹서버 접근 Permission Mask Clear. by wook 2015.11.24
	umask(0000);
	
	para_fd = open("/root/am1808/parameter.dat", O_RDWR | O_CREAT, 0666);
    if(para_fd == -1) {
			fprintf(stdout,"file open error\n");
			return FALSE;
	}
	else fprintf(stdout,"parameter saved\n");

	write(para_fd, &gstSysConfig, sizeof(SYS_CONFIG_OLD_t));

	close(para_fd);

#if defined(DEBUG_PARAMETER)
	printf  ( " Write parameter to NAND [CRC-16 : 0x%04x]\n", crc16);
#endif
	
	return ( TRUE );
}


BOOL writeNetConfigToNAND(uint8 erase)
{
	int stick, etick;
	uint16 crc16;

	gstNetConfig.magic_num = NETCFG_MAGIC_NUMBER;
	
	// Calculate CRC-16 for gstSysConfig.
	crc16 = updateCRC16((uint8 *)&gstNetConfig, sizeof(gstNetConfig), 0);

	// Permission Mask Clear. by wook 2015.11.24
	umask(0000);
	
    netpara_fd = open("/root/am1808/netconfig.dat", O_RDWR | O_CREAT, 0666);
    if(netpara_fd == -1) {
		fprintf(stdout,"file open error\n");
		return FALSE;
	}
    else fprintf(stdout,"parameter saved\n");

    write(netpara_fd, &gstNetConfig, sizeof(NETWORK_CONFIG_SAVED_t));

    close(netpara_fd);


#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Write Network Config to NAND [CRC-16 : 0x%04x]", crc16);
#endif
	
	return ( TRUE );
}


void getActiveLoopNum(void)				//2011.06.20 by capidra 
{
	uint16 i, j;
	uint8 msk, tmp = 0, atmp, dtmp=0, stmp=0;
	
	for (i=0; i<MAX_LOOP_NUM/8; i++)
	{
		if ( gstSysConfig.param_cfg.loop_enable_table[i] ==0 ) continue;

		for (j=8, msk=0x80; j>0; j--, msk>>=1)
		{
			if( gstSysConfig.param_cfg.loop_enable_table[i] & msk ) 
			{
				tmp = 8*i + j;
				break;
			}
		}
	}
	
	if ( tmp < 0 ) tmp = 0;
	else if ( tmp > MAX_RIO_LOOP_NUM ) tmp = MAX_RIO_LOOP_NUM;

	gucActiveLoopNum = tmp;

	//fprintf(stdout," active loop Dual & single : ");

	/*insert by capidra 2011.06.16*/	
	for(i = 0;i<MAX_LANE_NUM;i++)	
	{
		 gucDivDualLoopLane[i] = gstSysConfig.param_cfg.loop_divi_table[i];
		 
		//fprintf(stdout,"  %d:", gucDivDualLoopLane[i]);
	}

	gucActiveSingleLoopNum = 0; gucActiveDualLoopNum = 0;

	gucActiveLDBNum = (gucActiveLoopNum+3)/4; //2014.10.13 by capi
	
	for(i=0;i<8;i++)
	{
		revLDB_ID[i] = TRUE;		//init LD Board Check
		revLDB_cnt[i] = 0;
	}

	atmp = gucActiveLoopNum/2;
	
	for(i=0;i<atmp;i++)
	{
		if(gucDivDualLoopLane[i]) stmp++;
		else dtmp++;
	}

	gucActiveSingleLoopNum = stmp;
	gucActiveDualLoopNum = dtmp*2;
	
	fprintf(stdout," active loop num : %d dual: %d Single:%d, LD Board : %d\n", 
		gucActiveLoopNum, gucActiveDualLoopNum, gucActiveSingleLoopNum, gucActiveLDBNum);
	
}

// VDS 파라미터를 index별로 설정.
void putVdsParameter(uint8 index, uint8 *p_param, uint8 save_flg)
{
	int i, j, mask;
	uint16 count = 0;

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Put parameter : Index %d", index);
#endif
	
	switch(index)
	{
		case IDX_01_LOOP_ENABLE :
			for (i=0; i<MAX_LOOP_NUM/8; i++)
				gstSysConfig.param_cfg.loop_enable_table[i] = p_param[i];
			
			for (i=0; i<MAX_LANE_NUM; i++)			
				gstSysConfig.param_cfg.loop_divi_table[i] = 0;

			for (i=0; i<MAX_LANE_NUM; i++)		//insert by capidra 2011.06.16				
				gstSysConfig.param_cfg.loop_divi_table[i] = gucDivDualLoopLane[i];						

			getActiveLoopNum();
			initVdsGlobalVal();
			break;
			
		case IDX_02_SPD_LOOP_DEF :
			for (i=0; i<MAX_LANE_NUM; i++)
			{
				gstSysConfig.param_cfg.speed_loop_map[i][0] = p_param[i*2];
				gstSysConfig.param_cfg.speed_loop_map[i][1] = p_param[i*2+1];
			}
			
			initVdsGlobalVal();
			break;
			
		case IDX_03_POLL_CYCLE :
			gstSysConfig.param_cfg.polling_cycle = p_param[0];
			break;
			
		case IDX_04_DETECT_THRH :
			gstSysConfig.param_cfg.num_pluse_present = p_param[0];
			gstSysConfig.param_cfg.num_pluse_no_present = p_param[1];
			break;
			
		case IDX_05_SPD_CATEGORY :
			for (i=0; i<SPEED_CATEGORY_NO; i++)
				gstSysConfig.param_cfg.speed_category[i] = p_param[i];
			break;
			
		case IDX_06_LEN_CATEGORY :
			for (i=0; i<LENGTH_CATEGORY_NO; i++)
				gstSysConfig.param_cfg.length_category[i] = p_param[i];
			break;
			
		case IDX_07_SPD_ACC_ENB :
			gstSysConfig.param_cfg.spd_acc_enable = p_param[0];			
			break;
		case IDX_08_LEN_ACC_ENB :
			gstSysConfig.param_cfg.len_acc_enable = p_param[0];			
			break;
			
		case IDX_09_SPD_CALC_ENB :
			gstSysConfig.param_cfg.spd_calc_enable = p_param[0];
			break;

		case IDX_10_LEN_CALC_ENB :
			gstSysConfig.param_cfg.len_calc_enable = p_param[0];
			break;
			
		case IDX_11_SPD_LOOP_DIM :
			for (i=0; i<MAX_LANE_NUM; i++)
			{
				gstSysConfig.param_cfg.spd_loop_dist[i] = p_param[i];
				gstSysConfig.param_cfg.spd_loop_diam[i] = p_param[i+MAX_LANE_NUM];
			}
			
			pullSpeedLoopDim();
			break;
			
		case IDX_12_UP_VOL_LIMIT :
			gstSysConfig.param_cfg.upper_vol_limit= p_param[0];
			break;
			
		case IDX_13_UP_SPD_LIMIT :
			gstSysConfig.param_cfg.upper_spd_limit= p_param[0];
			break;
			
		case IDX_14_UP_LEN_LIMIT :
			gstSysConfig.param_cfg.upper_len_limit= p_param[0];
			break;
			
		case IDX_15_INCID_THRD :
			gstSysConfig.param_cfg.incid_exec_cycle = p_param[0];
			gstSysConfig.param_cfg.persist_period = p_param[1];
			gstSysConfig.param_cfg.incid_algorithm = p_param[2];
			
			/*		//delete by capi 2016.02.23 Default 
			for (i=0; i<2; i++)
				gstSysConfig.param_cfg.K_factor[i] = \
					(uint16) ( (p_param[2*i+3]<<8) | p_param[2*i+1+3] );
				
			for (i=0; i<MAX_LOOP_NUM; i++)
				gstSysConfig.param_cfg.T_value[i] = \
					(uint16) ( (p_param[2*i+7]<<8) | p_param[2*i+1+7] );
			*/
			for (i=0; i<2; i++)
				gstSysConfig.param_cfg.K_factor[i] = 12;	
			// Threshold
			for (i=0; i<MAX_LOOP_NUM; i++)
				gstSysConfig.param_cfg.T_value[i] = 100;		
			break;
			
		case IDX_16_STUCK_THRD :
			gstSysConfig.param_cfg.stuck_high_vol= p_param[0];
			gstSysConfig.param_cfg.stuck_on_high = p_param[1];
			gstSysConfig.param_cfg.stuck_off_high = p_param[2];
			gstSysConfig.param_cfg.stuck_low_vol= p_param[3];
			gstSysConfig.param_cfg.stuck_on_low= p_param[4];
			gstSysConfig.param_cfg.stuck_off_low= p_param[5];			
			break;
		case IDX_17_LOOP_OSC_THRD :
			gstSysConfig.param_cfg.oscillation_thr = p_param[0];
			break;
			
		case IDX_18_UNIT_ADDR :
		case IDX_19_PARAM_RAM_STS :
			return;
			break;

		case IDX_20_AUTO_RESYNC :
			gstSysConfig.param_cfg.auto_resync_wait = p_param[0];
			break;
			
		case IDX_21_SIMUL_TEMPL :
			gstSysConfig.param_cfg.data_1_stream_num = p_param[0];
			gstSysConfig.param_cfg.data_1_stream_num = p_param[1];
			gstSysConfig.param_cfg.simul_1_onoff = p_param[2];
			gstSysConfig.param_cfg.sim_1_length = p_param[3];
			gstSysConfig.param_cfg.sim_1_speed = p_param[4];
			gstSysConfig.param_cfg.sim_1_headway = p_param[5];
			gstSysConfig.param_cfg.sim_1_lane_dist = p_param[6];
			gstSysConfig.param_cfg.sim_1_loop_diam = p_param[7];
			gstSysConfig.param_cfg.data_2_stream_num = p_param[8];
			gstSysConfig.param_cfg.simul_2_onoff = p_param[9];
			gstSysConfig.param_cfg.sim_2_length = p_param[10];
			gstSysConfig.param_cfg.sim_2_speed = p_param[11];
			gstSysConfig.param_cfg.sim_2_headway = p_param[12];
			gstSysConfig.param_cfg.sim_2_lane_dist = p_param[13];
			gstSysConfig.param_cfg.sim_2_loop_diam = p_param[14];

			break;

		case IDX_22_SOFT_VERSION :
		case IDX_23_RESPOND_LEN :
		case IDX_24_SPARE_PARM :
			return;
			break;
		
		case IDX_25_USER_FOR_LOOP :		//2011.06.23 by capidra 	
			
			gucActiveDualLoopNum = 0;
			gucActiveSingleLoopNum = 0;

			for(i=0;i<MAX_LANE_NUM;i++) gucDivDualLoopLane[i] = 0;
			
			for(i=0;i<MAX_LOOP_NUM/8;i++)
			{
				mask=1;
				for(j=0;j<8;j+=2)
				{
					if(p_param[i]&mask<<j)
					{
						if(p_param[i]&(mask<<(j+1)))
						{
							gucActiveDualLoopNum+=2;
							gucDivDualLoopLane[(i*8+j)/2] = 0;
						}
						else 
						{
							gucActiveSingleLoopNum++; 
							p_param[i] = p_param[i]|(mask<<(j+1));
							gucDivDualLoopLane[(i*8+j)/2] = 1;
						}
					}
				}
			}
			gucActiveLoopNum = gucActiveDualLoopNum + gucActiveSingleLoopNum*2;	
			
			for (i=0; i<MAX_LOOP_NUM/8; i++)
				gstSysConfig.param_cfg.loop_enable_table[i] = p_param[count++];

			for (i=0; i<MAX_LANE_NUM; i++)			
				gstSysConfig.param_cfg.loop_divi_table[i] = 0;

			for (i=0; i<MAX_LANE_NUM; i++)		//insert by capidra 2011.06.16				
				gstSysConfig.param_cfg.loop_divi_table[i] = gucDivDualLoopLane[i];				

			
			for (i=0; i<MAX_LANE_NUM; i++)
			{
				gstSysConfig.param_cfg.speed_loop_map[i][0] = p_param[count++];
				gstSysConfig.param_cfg.speed_loop_map[i][1] = p_param[count++];
			}

			// Polling Cycle == Index 3
			gstSysConfig.param_cfg.polling_cycle = p_param[count++];
			
			gstSysConfig.param_cfg.num_pluse_present = p_param[count++];
			gstSysConfig.param_cfg.num_pluse_no_present = p_param[count++];

			getActiveLoopNum();
			initVdsGlobalVal();			
			break;

		
		case IDX_26_USER_FOR_VENHICLE :
			
			for (i=0; i<SPEED_CATEGORY_NO; i++)
				gstSysConfig.param_cfg.speed_category[i] = p_param[count++];			
			
			for (i=0; i<LENGTH_CATEGORY_NO; i++)
				gstSysConfig.param_cfg.length_category[i] = p_param[count++];			
			
			gstSysConfig.param_cfg.spd_acc_enable = p_param[count++];			
			
			gstSysConfig.param_cfg.len_acc_enable = p_param[count++];			
			
			gstSysConfig.param_cfg.spd_calc_enable = p_param[count++];
			
			gstSysConfig.param_cfg.len_calc_enable = p_param[count++];
			break;
		
		case IDX_27_USRE_FOR_THRESHOLD :
			
			gstSysConfig.param_cfg.upper_vol_limit= p_param[count++];			
			
			gstSysConfig.param_cfg.upper_spd_limit= p_param[count++];			
			
			gstSysConfig.param_cfg.upper_len_limit= p_param[count++];			
			
			gstSysConfig.param_cfg.stuck_high_vol= p_param[count++];
			gstSysConfig.param_cfg.stuck_on_high = p_param[count++];
			gstSysConfig.param_cfg.stuck_off_high = p_param[count++];
			gstSysConfig.param_cfg.stuck_low_vol= p_param[count++];
			gstSysConfig.param_cfg.stuck_on_low= p_param[count++];
			gstSysConfig.param_cfg.stuck_off_low= p_param[count++];			
			
			// Loop Detector Oscillation Threshold == Index 17
			gstSysConfig.param_cfg.oscillation_thr = p_param[count++];
			break;

		default :
			return;
			break;

	}
#if defined(DEBUG_PARAMETER)
	//if (getml() == msg_vds_parameter) dumpVdsParameter(index);
#endif

	// out of defalut parameter.
	gstSysConfig.is_default_param = _USER_SETTING_PARAM_;

	// write parameter to flash.
	if (save_flg == TRUE) writeParamToNAND(TRUE);
}

// 유지관리 프로그램 지원. by capi
uint16 getSysConfigByIndex_B(uint8 index, uint8 *p_config)
{
	int i, tmpV;
	uint16 count = 0, tmpID;
	char tmpStr[20];

	switch(index)
	{
	case IDX_B_01_SET_SYSTEM_TIME:
		p_config[count++] = sys_time.wYear/100;
		p_config[count++] = sys_time.wYear%100;
		p_config[count++] = sys_time.wMonth;
		p_config[count++] = sys_time.wDay;
		p_config[count++] = sys_time.wHour;
		p_config[count++] = sys_time.wMinute;
		p_config[count++] = sys_time.wSecond;
		break;

	case IDX_B_02_SET_CONTROLLER_ADDR:
		for(i=0; i<STATION_NUM_LEN; i++, count++) p_config[i] = gstNetConfig.station_num[i];
		break;
		
	case IDX_B_03_SET_IP_ADDRESS:
		netBin2IpStr(gstNetConfig.ip, (char *)&p_config[0]);
		count += IP_STR_LEN;
		break;
		
	case IDX_B_04_SET_SUBNET_MASK:
		netBin2IpStr(gstNetConfig.nmsk, (char *)&p_config[0]);
		count += IP_STR_LEN;
		break;
		
	case IDX_B_05_SET_GATEWAY_IP:
		netBin2IpStr(gstNetConfig.gip, (char *)&p_config[0]);
		count += IP_STR_LEN;
		break;
		
	case IDX_B_06_SET_SERVER_IP:
		netBin2IpStr(gstNetConfig.server_ip, (char *)&p_config[0]);
		count += IP_STR_LEN;
		break;
		
	case IDX_B_07_SET_SERVER_PORT:
		p_config[count++] = gstNetConfig.server_port >> 8;
		p_config[count++] = gstNetConfig.server_port & 0xFF;
		break;
	
	case IDX_B_10_SET_TOTAL_NUM_OF_LOOP:
		p_config[count++] = gucActiveLoopNum;
		break;
		
	case IDX_B_11_LEN_GAP_OF_LOOP:
		p_config[count++] = gstSysConfig.param_cfg.spd_loop_dist[0];
		break;
		
	case IDX_B_12_SET_SYNC_CYCLE:
		p_config[count++] = gstSysConfig.param_cfg.polling_cycle;
		break;

	case IDX_B_13_SET_LOCAL_POLL_TIME:
		//p_config[count++] = gstSysConfig.comm_cfg.local_poll;
		p_config[count++] = gstSysConfig.comm_cfg.local_poll / 10;		// 091106 by jwank
		break;

	case IDX_B_14_SET_ALL_SYSTEM_CONFIG:
		////////////////////////////////////////////////////////////////////
		// System Time.		

		p_config[count++] = sys_time.wYear/100;
		p_config[count++] = sys_time.wYear%100;
		p_config[count++] = sys_time.wMonth;
		p_config[count++] = sys_time.wDay;
		p_config[count++] = sys_time.wHour;
		p_config[count++] = sys_time.wMinute;
		p_config[count++] = sys_time.wSecond;

		////////////////////////////////////////////////////////////////////
		// Station number.
		for(i=0; i<STATION_NUM_LEN; i++) p_config[count++] = gstNetConfig.station_num[i];	//2011.06.23 by capidra

		////////////////////////////////////////////////////////////////////
		// IP addrees.
		netBin2IpStr(gstNetConfig.ip, (char *)&p_config[count]);
		count += IP_STR_LEN;

		////////////////////////////////////////////////////////////////////
		// Subnet mask.
		netBin2IpStr(gstNetConfig.nmsk, (char *)&p_config[count]);
		count += IP_STR_LEN;

		////////////////////////////////////////////////////////////////////
		// Gateway IP.
		netBin2IpStr(gstNetConfig.gip, (char *)&p_config[count]);
		count += IP_STR_LEN;

		////////////////////////////////////////////////////////////////////
		// Server IP.
		netBin2IpStr(gstNetConfig.server_ip, (char *)&p_config[count]);
		count += IP_STR_LEN;

		////////////////////////////////////////////////////////////////////
		// Server TCP port.
		p_config[count++] = gstNetConfig.server_port >> 8;
		p_config[count++] = gstNetConfig.server_port & 0xFF;

		////////////////////////////////////////////////////////////////////
		// Loop Number.
		p_config[count++] = gucActiveDualLoopNum;
		p_config[count++] = gucActiveSingleLoopNum;	

		////////////////////////////////////////////////////////////////////
		// Loop 간격.
		p_config[count++] = gstSysConfig.param_cfg.spd_loop_dist[0];

		////////////////////////////////////////////////////////////////////
		// Center Polling time.
		p_config[count++] = gstSysConfig.param_cfg.polling_cycle;

		////////////////////////////////////////////////////////////////////
		// Local Polling time.
		//p_config[count++] = gstSysConfig.comm_cfg.local_poll;
		p_config[count++] = gstSysConfig.comm_cfg.local_poll / 10;	// 091106 by jwank
		break;

	default :
		return 0;
		break;
	}


	return count;
}


//by capi
void setAllSystemConfig_B(uint8 *p_config)
{
	uint16 wrIdx = 0;
	
	fprintf(stdout,"\n setAllSystemConfig_B()\n");

	setSysConfigByIndex_B(IDX_B_01_SET_SYSTEM_TIME, &p_config[wrIdx], FALSE);			wrIdx = 7;// 1~7
	setSysConfigByIndex_B(IDX_B_02_SET_CONTROLLER_ADDR, &p_config[wrIdx], FALSE);		wrIdx = 11;// 8~11
	setSysConfigByIndex_B(IDX_B_03_SET_IP_ADDRESS, &p_config[wrIdx], FALSE);			wrIdx = 26;// 12~26
	setSysConfigByIndex_B(IDX_B_04_SET_SUBNET_MASK, &p_config[wrIdx], FALSE);			wrIdx = 41;// 27~41
	setSysConfigByIndex_B(IDX_B_05_SET_GATEWAY_IP, &p_config[wrIdx], FALSE);			wrIdx = 56;// 42~56
	setSysConfigByIndex_B(IDX_B_06_SET_SERVER_IP, &p_config[wrIdx], FALSE);				wrIdx = 71;// 57~71
	setSysConfigByIndex_B(IDX_B_07_SET_SERVER_PORT, &p_config[wrIdx], FALSE);			wrIdx = 73;// 72~73
	setSysConfigByIndex_B(IDX_B_10_SET_TOTAL_NUM_OF_LOOP, &p_config[wrIdx], FALSE);		wrIdx = 75;// 74~75
	setSysConfigByIndex_B(IDX_B_11_LEN_GAP_OF_LOOP, &p_config[wrIdx], FALSE);			wrIdx = 76;// 76
	setSysConfigByIndex_B(IDX_B_12_SET_SYNC_CYCLE, &p_config[wrIdx], FALSE);			wrIdx = 77;// 77
	setSysConfigByIndex_B(IDX_B_13_SET_LOCAL_POLL_TIME, &p_config[wrIdx], FALSE);		wrIdx = 78;// 78
		
	writeParamToNAND(TRUE); 	
	sendSysConfigMsgToMMI();	

	#if defined(SUPPORT_DYNAMIC_CHANGE_NET_CONFIG)
	#endif
}

//by capi
uint16 setSysConfigByIndex_B(uint8 index, uint8 *p_config, uint8 save_flg)
{
	int i, len=0, count=0;

#if defined(DEBUG_SYSTEM_CONFIG)
	if (getml() == msg_sysconfig)
	{
		fprintf(stdout," setSysConfigByIndex_B(%d)", index);

		switch(index)
		{
		case IDX_B_01_SET_SYSTEM_TIME:
			len = 7; break;
		case IDX_B_02_SET_CONTROLLER_ADDR:
			len = 4; break;
		case IDX_B_03_SET_IP_ADDRESS:
		case IDX_B_04_SET_SUBNET_MASK:
		case IDX_B_05_SET_GATEWAY_IP:
		case IDX_B_06_SET_SERVER_IP:
			len = 15; break;
		case IDX_B_07_SET_SERVER_PORT:		
			len = 2; break;
		case IDX_B_10_SET_TOTAL_NUM_OF_LOOP:
			len = 2; break;
		case IDX_B_11_LEN_GAP_OF_LOOP:		
		case IDX_B_12_SET_SYNC_CYCLE:
		case IDX_B_13_SET_LOCAL_POLL_TIME:
			len = 1; break;
		}

		for(i=0; i<len; i++) fprintf(stdout,"%d=>%d[0x%02X] ", i, p_config[i], p_config[i]);
		fprintf(stdout,"");
	}
#endif


#if defined(SYSTEM_CONFIG_NOT_SAVED)
	fprintf(stdout," ############## do not save ==> please check this code !!");
	save_flg = FALSE;
#endif

	switch(index)
	{
	case IDX_B_01_SET_SYSTEM_TIME:
		{
			SYSTEMTIME tmptime;
			char timebuf[255];

			tmptime.wYear = p_config[0]*100 + (p_config[1]%100);
			tmptime.wMonth = p_config[2];
			tmptime.wDay = p_config[3];
			tmptime.wHour = p_config[4];
			tmptime.wMinute = p_config[5];
			tmptime.wSecond = p_config[6];
			
			//if (tmptime.wMonth > 12 || tmptime.wMonth <= 0) goto no_save_time;	// oooo
			if (tmptime.wMonth > 12 || tmptime.wMonth == 0) goto no_save_time;
			//if (tmptime.wDay > 31 || tmptime.wDay <= 0) goto no_save_time;	// oooo
			if (tmptime.wDay > 31 || tmptime.wDay == 0) goto no_save_time;
			if (tmptime.wHour >= 24) goto no_save_time;
			if (tmptime.wMinute >= 60) goto no_save_time;
			if (tmptime.wSecond >= 60) goto no_save_time;

			fprintf(stdout," Set Data & Time to RTC -\n");
			fprintf(stdout," %d/%d/%d %d:%d:%d \n", tmptime.wYear, tmptime.wMonth, \
				tmptime.wDay, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);

			//OS Time change
			sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
							,tmptime.wYear, tmptime.wMonth, tmptime.wDay
							, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);
			system(timebuf);

			msec_sleep(300);
			setSysTimeToRTC(&tmptime);  //RTC Repair			
			break;
			
no_save_time:
			fprintf(stdout," invalid systime: %d/%d/%d %d:%d:%d", tmptime.wYear, tmptime.wMonth, \
				tmptime.wDay, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);
				
			break;
		}
		break;

	case IDX_B_02_SET_CONTROLLER_ADDR:
		{
			uint16 usStNum[2];

			usStNum[0] = (p_config[0]<<8) + p_config[1];
			usStNum[1] = (p_config[2]<<8) + p_config[3];

			count += STATION_NUM_LEN;

			if (memcmp(p_config, gstNetConfig.station_num, STATION_NUM_LEN))
			{
				fprintf(stdout," New Station number: %05d-%05d \n", usStNum[0], usStNum[1]);

				for (i=0; i<STATION_NUM_LEN; i++) gstNetConfig.station_num[i] = p_config[i];

				if (save_flg == TRUE)
				{
					writeNetConfigToNAND(TRUE);
					sendNetConfigMsgToMMI(MMI_CFG_IDX_06_STATION_NUM);
				}
			}
			else
			{
				fprintf(stdout," ############## Same Station number: %05d-%05d \n", usStNum[0], usStNum[1]);
			}
			break;
		}
		break;

	case IDX_B_03_SET_IP_ADDRESS:
		{
			// IP address 
			char *psIpAddr = (char *)&p_config[0];
			//int status;
			uint8 byFlg;

			count += IP_STR_LEN;
			
			if (save_flg == TRUE) byFlg = FLAG_ALL;
			else byFlg = FLAG_NOT_SAVE;
			
			chk_n_SaveStrIpToNVRAM(MMI_CFG_IDX_01_IP_ADDR, psIpAddr, NULL, byFlg);
				
			break;
		}
		break;
		
	case IDX_B_04_SET_SUBNET_MASK:
		{
			char *psIpAddr = (char *)&p_config[0];
			//int status;
			uint8 byFlg;

			count += IP_STR_LEN;
			
			if (save_flg == TRUE) byFlg = FLAG_ALL;
			else byFlg = FLAG_NOT_SAVE;
			
			chk_n_SaveStrIpToNVRAM(MMI_CFG_IDX_03_SUBN_MSK, psIpAddr, NULL, byFlg);
			
			break;
		}
		break;
		
	case IDX_B_05_SET_GATEWAY_IP:
		{
			char *psIpAddr = (char *)&p_config[0];
			//int status;
			uint8 byFlg;

			count += IP_STR_LEN;
			
			if (save_flg == TRUE) byFlg = FLAG_ALL;
			else byFlg = FLAG_NOT_SAVE;
			
			chk_n_SaveStrIpToNVRAM(MMI_CFG_IDX_02_GATEWAY_IP, psIpAddr, NULL, byFlg);			
			
			break;
		}
		break;

	case IDX_B_06_SET_SERVER_IP:
		{
			char *psIpAddr = (char *)&p_config[0];
			//int status;
			uint8 byFlg;

			count += IP_STR_LEN;
			
			if (save_flg == TRUE) byFlg = FLAG_ALL;
			else byFlg = FLAG_NOT_SAVE;
			
			chk_n_SaveStrIpToNVRAM(MMI_CFG_IDX_04_SERVER_IP, psIpAddr, NULL, byFlg);
			break;
		}
		break;
		
	case IDX_B_07_SET_SERVER_PORT:
		{
			uint16 usServPort;

			// MSB first.
			usServPort = (p_config[0]<<8) + p_config[1];
			count += 2;

			if (usServPort != gstNetConfig.server_port)
			{
				fprintf(stdout," New Server port : %d \r\n", usServPort);

				// MSB first.
				gstNetConfig.server_port = usServPort;				

				if (save_flg == TRUE)
				{
					writeNetConfigToNAND(TRUE);
					sendNetConfigMsgToMMI(MMI_CFG_IDX_05_SERVER_PORT);
				}
			}
			else
			{
				fprintf(stdout," ############## Same Server port: %d\r\n", usServPort);
			}
			break;
		}
		break;	

	case IDX_B_10_SET_TOTAL_NUM_OF_LOOP:
		{
			
			uint8 dlane, j, t1, t2, msk, p_param[10];
				
			uint8 tmpv = p_config[0];
			count += 2;

			if (tmpv > MAX_RIO_LOOP_NUM ) tmpv = MAX_RIO_LOOP_NUM;
					
			for (i=0; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

			t1 = tmpv / 8;
			t2 = tmpv % 8;
	
			for (i=0; i<t1; i++) p_param[i] = 0xff;
			for (j=0, msk=1; j<t2; j++, msk<<=1) p_param[i] |= msk;
			i++;
			for (; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

			//gucActiveLoopNum = tmpv;
			gucActiveDualLoopNum = tmpv;						//2011.06.16 by capidra 
			gucActiveSingleLoopNum= p_config[1];						//2011.06.16 by capidra 

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
		
	case IDX_B_11_LEN_GAP_OF_LOOP:
		{
			uint8 loop_gap = p_config[0];

			count += 1;
			
			if (loop_gap <= 250)
			{
				fprintf(stdout," New gap of loop: %d \n",  loop_gap);

				for (i=0; i<MAX_LANE_NUM; i++)
					gstSysConfig.param_cfg.spd_loop_dist[i] = p_config[0], loop_gap;

				pullSpeedLoopDim();

				// out of defalut parameter.
				gstSysConfig.is_default_param = _USER_SETTING_PARAM_;

				if (save_flg == TRUE)
				{		
					writeParamToNAND(TRUE); 									
					sendSysConfigMsgToMMI();					
				}
			}
			else
			{
				fprintf(stdout," ############## Invalid baudrate: %d[0x%02X]\n", loop_gap, loop_gap);
			}
		}
		break;
		
	case IDX_B_12_SET_SYNC_CYCLE:
		{
			uint8 sync_time = p_config[0];

			count += 1;
			
			if (sync_time != gstSysConfig.param_cfg.polling_cycle)
			{
				if (sync_time <= 250 && sync_time > 0)
				{
					fprintf(stdout," New polling cycle: %d\n",  sync_time);
					gstSysConfig.param_cfg.polling_cycle = sync_time;

					if (save_flg == TRUE)
					{
						//writeParamToNVRAM(TRUE);
						//sendSysConfigMsgToMMI();
					}
				}
				else
				{
					fprintf(stdout," ############## Invalid sync time: %d\n", sync_time);
				}
			}
			else
			{
				fprintf(stdout," ############## Same sync time: %d\n", sync_time);
			}
		}
		break;

	case IDX_B_13_SET_LOCAL_POLL_TIME:
		{
			//uint8 sync_time = p_config[0];
			uint8 sync_time = p_config[0] * 10;		// 091106 by jwank

			count += 1;
			
			if (sync_time != gstSysConfig.comm_cfg.local_poll)
			{
				if (sync_time <= MAX_LOCAL_POLL_TIME && sync_time > 0)
				{
					fprintf(stdout," New Local polling time: %d\n",  sync_time);
					gstSysConfig.comm_cfg.local_poll = sync_time;

					if (save_flg == TRUE)
					{
						//writeParamToNVRAM(TRUE);
						//sendSysConfigMsgToMMI();
					}
				}
				else
				{
					fprintf(stdout," ############## Invalid local polling time: %d\n", sync_time);
				}
			}
			else
			{
				fprintf(stdout," ############## Same local polling time: %d\n", sync_time);
			}
		}
		break;		

	default :
		return 0;
		break;
	}	

	return count;
}

BOOL readNetConfigFromNAND()
{
	uint16 crc16, t_CRC;
	int rtn;

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Read Network Config from the first region of FRAM -> size : %d / ", sizeof(gstNetConfig));
#endif
	
    	netpara_fd = open("/root/am1808/netconfig.dat", O_RDWR);
    	if(netpara_fd == -1) {
		fprintf(stdout,"file open error\n");
		return FALSE;
	}
    	else fprintf(stdout,"Open Netconfig Parameter \n");

    	rtn = read(netpara_fd, &gstNetConfig, sizeof(NETWORK_CONFIG_SAVED_t));
    	if(rtn <0) {
            fprintf(stdout,"parameter open error\n");
            return FALSE;
    	}

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Magic number for Network config. : 0x%08x", gstNetConfig.magic_num);
#endif
	
	// Check magic number.
	if (gstNetConfig.magic_num != NETCFG_MAGIC_NUMBER)
	{
		fprintf(stdout, " #################### - Magic num. Error !!", gstNetConfig.magic_num);
		return (FALSE);
	}
	
	// Check CRC of gstNetConfig.
	t_CRC = updateCRC16( (uint8 *)&gstNetConfig, sizeof(gstNetConfig), 0);
	
#if defined(DEBUG_PARAMETER)
	fprintf(stdout," CRC for gstNetConfig from FRAM. [0x%04x], CRC is calculated. [0x%04x]", crc16, t_CRC);
#endif
	/*
	if (crc16 != t_CRC) 
	{
		fprintf(stdout, " #################### - CRC Error !!");
		return (FALSE);
		//goto read_netcfg_backup;
	}*/

	return (TRUE);
}

int read_parameter_NAND()
{
	uint16 crc16, t_CRC;
	u8 *dst_ptr = (u8 *)&gstSysConfig;
	int rtn;

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Read parameter from the first region of FRAM -> size : %d / ", sizeof(gstSysConfig));
#endif
	
	para_fd = open("/root/am1808/parameter.dat", O_RDWR);
    if(para_fd == -1) {
		fprintf(stdout,"file open error\n");
		return FALSE;
	}
	else fprintf(stdout,"Open Parameter \n");

	rtn = read(para_fd, &gstSysConfig, sizeof(SYS_CONFIG_OLD_t));
	if(rtn <0) {
		fprintf(stdout,"parameter open error\n");
		return FALSE;
	}

	close(para_fd);	

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Magic number for System config. : 0x%08x", gstSysConfig.magic_num);
#endif
	
	// Check magic number.
	if ( gstSysConfig.magic_num != SYS_MAGIC_NUMBER )
	{
		fprintf(stdout,"[CFG] - Magic num. Error-1 [0x%04x]\n", gstSysConfig.magic_num);
		return FALSE;
	}

	return ( TRUE );
}


void defaultVdsParameter(void)
{
	int i, j, t1, t2;
	uint8 msk;

	gstSysConfig.stuck_thrh = LOW_VOLUME_THRH;

#if 1
	for (i=0; i<MAX_LOOP_NUM/8; i++)
		gstSysConfig.param_cfg.loop_enable_table[i] = 0;

	t1 = DEFAULT_LOOP_NUM / 8;
	t2 = DEFAULT_LOOP_NUM % 8;
			
	for (i=0; i<t1; i++) 
		gstSysConfig.param_cfg.loop_enable_table[i] = 0xff;
	for (j=0, msk=1; j<t2; j++, msk<<=1) 
		gstSysConfig.param_cfg.loop_enable_table[i] |= msk;
	i++;
	for (; i<MAX_LOOP_NUM/8; i++)
		gstSysConfig.param_cfg.loop_enable_table[i] = 0;
	
	for (; i<MAX_LANE_NUM; i++)
		gstSysConfig.param_cfg.loop_divi_table[i] = 0;			//insert by capidra 2011.06.16

#else
	for (i=0; i<MAX_LOOP_NUM/8; i++)
		gstSysConfig.param_cfg.loop_enable_table[i] = 0xff;
#endif

	for (i=0; i<MAX_LANE_NUM; i++)
	{
		gstSysConfig.param_cfg.speed_loop_map[i][0] = i*2;
		gstSysConfig.param_cfg.speed_loop_map[i][1] = i*2+1;
	}

	// (3) Polling Cycle
	gstSysConfig.param_cfg.polling_cycle = CENTER_POLLING_TIME;
	
	gstSysConfig.param_cfg.num_pluse_present = 6;
	gstSysConfig.param_cfg.num_pluse_no_present = 3;
	
	gstSysConfig.param_cfg.speed_category[0] = 0x14;			// 20 Km/h
	gstSysConfig.param_cfg.speed_category[1] = 0x1e;
	gstSysConfig.param_cfg.speed_category[2] = 0x28;
	gstSysConfig.param_cfg.speed_category[3] = 0x32;
	gstSysConfig.param_cfg.speed_category[4] = 0x3c;
	gstSysConfig.param_cfg.speed_category[5] = 0x46;
	gstSysConfig.param_cfg.speed_category[6] = 0x50;
	gstSysConfig.param_cfg.speed_category[7] = 0x5a;
	gstSysConfig.param_cfg.speed_category[8] = 0x64;
	gstSysConfig.param_cfg.speed_category[9] = 0x6e;
	gstSysConfig.param_cfg.speed_category[10] = 0x78;		// 120 Km/h
	gstSysConfig.param_cfg.speed_category[11] = 0x79;		// 121 Km/h
	
	gstSysConfig.param_cfg.length_category[0] = 80;			// 80 DM
	gstSysConfig.param_cfg.length_category[1] = 120;			// 120 DM
	
	gstSysConfig.param_cfg.spd_acc_enable = 1;				// 1: enable, 0: disable	
	
	gstSysConfig.param_cfg.len_acc_enable = 1;				// 1: enable, 0: disable	
	
	gstSysConfig.param_cfg.spd_calc_enable = 1;				// 1: enable, 0: disable	
	
	gstSysConfig.param_cfg.len_calc_enable = 1;				// 1: enable, 0: disable

	// (11) Speed Loop Demension.
	for (i=0; i<MAX_LANE_NUM; i++)
	{
		gstSysConfig.param_cfg.spd_loop_dist[i] = 45;		// 45 DM
		gstSysConfig.param_cfg.spd_loop_diam[i] = 18;		// 18 DM
	}
	
	gstSysConfig.param_cfg.upper_vol_limit = 25;				// 25 EA	
	
	gstSysConfig.param_cfg.upper_spd_limit = 175;			// 175 km/h	
	
	gstSysConfig.param_cfg.upper_len_limit = 250;			// 250 DM	
	
	gstSysConfig.param_cfg.incid_exec_cycle = 5;				// 5 sec.
	gstSysConfig.param_cfg.persist_period = 2;				// 2 sec.
	gstSysConfig.param_cfg.incid_algorithm = 1;				// Algorithm 1, 2, 3, or 4, 0 = disable)
	// K factor
	for (i=0; i<2; i++)
		gstSysConfig.param_cfg.K_factor[i] = 12;	
	// Threshold
	for (i=0; i<MAX_LOOP_NUM; i++)
		gstSysConfig.param_cfg.T_value[i] = 100;		
	
	gstSysConfig.param_cfg.stuck_high_vol = 120;				// min.
	gstSysConfig.param_cfg.stuck_on_high = 114;					
	gstSysConfig.param_cfg.stuck_off_high = 72;				
	gstSysConfig.param_cfg.stuck_low_vol = 240;
	gstSysConfig.param_cfg.stuck_on_low = 156;				
	gstSysConfig.param_cfg.stuck_off_low = 235;				

	// (17) Loop Detector Oscillation Threshold.
	gstSysConfig.param_cfg.oscillation_thr = 30;				// 30 EA

	// (18) Unit Hardware Address. - Reserved
	// (19) Parameter Identifier. - Reserved

	// (20) Auto Re-Sync를 위한 대기시간.
	gstSysConfig.param_cfg.auto_resync_wait = 1;				// 1 sec.

	// (21) Simulation Templates.
	gstSysConfig.param_cfg.data_1_stream_num = 1;			// Data Stream 1
	gstSysConfig.param_cfg.simul_1_onoff = 0;				// Simulation off (0: Off, 1: On)
	gstSysConfig.param_cfg.sim_1_length = 3;					// 3 meter
	gstSysConfig.param_cfg.sim_1_speed = 100;				// 100 Km/hr
	gstSysConfig.param_cfg.sim_1_headway = 4;				// 4 secs
	gstSysConfig.param_cfg.sim_1_lane_dist = 45;				// 45 dm
	gstSysConfig.param_cfg.sim_1_loop_diam = 18;				// 18 dm
	gstSysConfig.param_cfg.data_2_stream_num = 2;			// Data Stream 2
	gstSysConfig.param_cfg.simul_2_onoff = 0;				// Simulation off
	gstSysConfig.param_cfg.sim_2_length = 4;					// 4 meter
	gstSysConfig.param_cfg.sim_2_speed = 100;				// 90 Km/hr
	gstSysConfig.param_cfg.sim_2_headway = 2;				// 2 secs
	gstSysConfig.param_cfg.sim_2_lane_dist = 45;				// 45 dm
	gstSysConfig.param_cfg.sim_2_loop_diam = 18;				// 18 dm

	// (23) VDS Response Data Length. - Reserved
	// (24) Spare Parameter. - Reserved

}

// Detecter Error Threshold 변경.
void ChangeErrThreshold(uint8 threshold)
{
	if (threshold != gstSysConfig.stuck_thrh)
	{				
		if (threshold > HIGH_VOLUME_THRH) 
			gstSysConfig.stuck_thrh = LOW_VOLUME_THRH;
		else 
			gstSysConfig.stuck_thrh = threshold;

		// Save to NAND.
		writeParamToNAND(TRUE);
		// insert new
	}
}

void putVdsParamLoopenable(uint8 index, uint8 *p_param, uint8 save_flg)
{
	int i, j, msk;

	gucActiveDualLoopNum = 0;
	gucActiveSingleLoopNum = 0;

	for(i=0;i<MAX_LANE_NUM;i++) gucDivDualLoopLane[i] = 0;
	
	for(i=0;i<MAX_LOOP_NUM/8;i++)
	{
		msk=1;
		for(j=0;j<8;j+=2)
		{
			if(p_param[i]&msk<<j)
			{
				if(p_param[i]&(msk<<(j+1)))
				{
					gucActiveDualLoopNum+=2;
					gucDivDualLoopLane[(i*8+j)/2] = 0;
				}
				else 
				{
					gucActiveSingleLoopNum++; 
					p_param[i] = p_param[i]|(msk<<(j+1));
					gucDivDualLoopLane[(i*8+j)/2] = 1;
				}
			}
		}
	}
	gucActiveLoopNum = gucActiveDualLoopNum + gucActiveSingleLoopNum*2;
	
	fprintf(stdout,"\n\r New maximum loop : %d Dual : %d Single : %d",  gucActiveLoopNum, gucActiveDualLoopNum, gucActiveSingleLoopNum);
	for(i=0;i<MAX_LANE_NUM;i++) fprintf(stdout,"  %d", gucDivDualLoopLane[i]);
	fprintf(stdout," \n");
	fprintf(stdout,"  %X %X %X %X \n", p_param[0], p_param[1], p_param[2], p_param[3]);
	
	putVdsParameter(index, p_param, save_flg);
}

// VDS 파라미터를 index별로 가져옴.
uint16 getVdsParameter(uint8 index, uint8 *p_param)
{
	int i,tmp,t1,t2, msk=1;
	uint16 count = 0;

#if defined(DEBUG_PARAMETER)
	fprintf(stdout," Get parameter : Index %d", index);
#endif
	
	switch(index)
	{
		case IDX_01_LOOP_ENABLE :
			for (i=0; i<MAX_LOOP_NUM/8; i++, count++)
				p_param[i] = gstSysConfig.param_cfg.loop_enable_table[i];				
			

			for(i=0;i<MAX_LANE_NUM;i++)
			{
				if(gstSysConfig.param_cfg.loop_divi_table[i])
				{
					tmp = i*2;
					t1 = tmp/8;
					t2 = tmp%8+1;

					p_param[t1] = p_param[t1]^0xFF;
					p_param[t1] = p_param[t1]|(msk<<t2);
					p_param[t1] = p_param[t1]^0xFF;
				}
			}
			
			break;
			
		case IDX_02_SPD_LOOP_DEF :			//2011.06.23 by capidra
			for (i=0; i<MAX_LANE_NUM; i++, count+=2)
			{
				p_param[i*2] = gstSysConfig.param_cfg.speed_loop_map[i][0];

				if(gstSysConfig.param_cfg.loop_divi_table[i])					//2011.06.21 by capidra 
					p_param[i*2+1] = 0xFF;
				else
					p_param[i*2+1] = gstSysConfig.param_cfg.speed_loop_map[i][1];
			}
			
			break;
			
		case IDX_03_POLL_CYCLE :
			p_param[0]= gstSysConfig.param_cfg.polling_cycle; count++;
			break;
			
		case IDX_04_DETECT_THRH :
			p_param[0] = gstSysConfig.param_cfg.num_pluse_present; count++;
			p_param[1] = gstSysConfig.param_cfg.num_pluse_no_present; count++;
			break;
			
		case IDX_05_SPD_CATEGORY :
			for (i=0; i<SPEED_CATEGORY_NO; i++, count++)
				p_param[i] = gstSysConfig.param_cfg.speed_category[i];
			break;
		case IDX_06_LEN_CATEGORY :
			for (i=0; i<LENGTH_CATEGORY_NO; i++, count++)
				p_param[i] = gstSysConfig.param_cfg.length_category[i];
			break;
			
		case IDX_07_SPD_ACC_ENB :
			p_param[0] = gstSysConfig.param_cfg.spd_acc_enable; count++;			
			break;
			
		case IDX_08_LEN_ACC_ENB :
			p_param[0] = gstSysConfig.param_cfg.len_acc_enable; count++;			
			break;
			
		case IDX_09_SPD_CALC_ENB :
			p_param[0] = gstSysConfig.param_cfg.spd_calc_enable; count++;
			break;

		case IDX_10_LEN_CALC_ENB :
			p_param[0] = gstSysConfig.param_cfg.len_calc_enable; count++;
			break;
			
		case IDX_11_SPD_LOOP_DIM :
			for (i=0; i<MAX_LANE_NUM; i++, count+=2)
			{
				p_param[i] = gstSysConfig.param_cfg.spd_loop_dist[i];
				p_param[i+MAX_LANE_NUM] = gstSysConfig.param_cfg.spd_loop_diam[i];
			}
			break;
			
		case IDX_12_UP_VOL_LIMIT :
			p_param[0] = gstSysConfig.param_cfg.upper_vol_limit; count++;
			break;
			
		case IDX_13_UP_SPD_LIMIT :
			p_param[0] = gstSysConfig.param_cfg.upper_spd_limit; count++;
			break;
			
		case IDX_14_UP_LEN_LIMIT :
			p_param[0] = gstSysConfig.param_cfg.upper_len_limit; count++;
			break;
		case IDX_15_INCID_THRD :
			p_param[0] = gstSysConfig.param_cfg.incid_exec_cycle; count++;
			p_param[1] = gstSysConfig.param_cfg.persist_period; count++;
			p_param[2] = gstSysConfig.param_cfg.incid_algorithm; count++;
			
			for (i=0; i<2; i++, count+=2)
			{
				p_param[2*i+3] = (uint8) (gstSysConfig.param_cfg.K_factor[i] >> 8);
				p_param[2*i+1+3] = (uint8) gstSysConfig.param_cfg.K_factor[i];
			}
			
			for (i=0; i<MAX_LOOP_NUM; i++, count+=2)
			{
				p_param[2*i+7] = (uint8) (gstSysConfig.param_cfg.T_value[i] >> 8);
				p_param[2*i+1+7] = (uint8) gstSysConfig.param_cfg.T_value[i];
			}
			
			break;
			
		case IDX_16_STUCK_THRD :
			p_param[0] = gstSysConfig.param_cfg.stuck_high_vol; count++;
			p_param[1] = gstSysConfig.param_cfg.stuck_on_high; count++;
			p_param[2] = gstSysConfig.param_cfg.stuck_off_high; count++;
			p_param[3] = gstSysConfig.param_cfg.stuck_low_vol; count++;
			p_param[4] = gstSysConfig.param_cfg.stuck_on_low; count++;
			p_param[5] = gstSysConfig.param_cfg.stuck_off_low; count++;
			break;
			
		case IDX_17_LOOP_OSC_THRD :
			p_param[0] = gstSysConfig.param_cfg.oscillation_thr; count++;
			break;
			
		case IDX_18_UNIT_ADDR :
		case IDX_19_PARAM_RAM_STS :
			return 0;
			break;

		case IDX_20_AUTO_RESYNC :
			p_param[0] = gstSysConfig.param_cfg.auto_resync_wait; count++;
			break;
			
		case IDX_21_SIMUL_TEMPL :
			p_param[0] = gstSysConfig.param_cfg.data_1_stream_num; count++;
			p_param[1] = gstSysConfig.param_cfg.simul_1_onoff; count++;
			p_param[2] = gstSysConfig.param_cfg.sim_1_length; count++;
			p_param[3] = gstSysConfig.param_cfg.sim_1_speed; count++;
			p_param[4] = gstSysConfig.param_cfg.sim_1_headway; count++;
			p_param[5] = gstSysConfig.param_cfg.sim_1_lane_dist; count++;
			p_param[6] = gstSysConfig.param_cfg.sim_1_loop_diam; count++;
			p_param[7] = gstSysConfig.param_cfg.data_2_stream_num; count++;
			p_param[8] = gstSysConfig.param_cfg.simul_2_onoff; count++;
			p_param[9] = gstSysConfig.param_cfg.sim_2_length; count++;
			p_param[10] = gstSysConfig.param_cfg.sim_2_speed; count++;
			p_param[11] = gstSysConfig.param_cfg.sim_2_headway; count++;
			p_param[12] = gstSysConfig.param_cfg.sim_2_lane_dist; count++;
			p_param[13] = gstSysConfig.param_cfg.sim_2_loop_diam; count++;

			break;
		case IDX_22_SOFT_VERSION :
		case IDX_23_RESPOND_LEN :
		case IDX_24_SPARE_PARM :
			return 0;
			break;

		
		case IDX_25_USER_FOR_LOOP :
			// 2011.06.23 by capidra 
			//for (i=0; i<MAX_LOOP_NUM/8; i++)
			//	p_param[count++] = gstSysConfig.param_cfg.loop_enable_table[i];

			for (i=0; i<MAX_LOOP_NUM/8; i++, count++)
				p_param[i] = gstSysConfig.param_cfg.loop_enable_table[i];				
			

			for(i=0;i<MAX_LANE_NUM;i++)
			{
				if(gstSysConfig.param_cfg.loop_divi_table[i])
				{
					tmp = i*2;
					t1 = tmp/8;
					t2 = tmp%8+1;

					p_param[t1] = p_param[t1]^0xFF;
					p_param[t1] = p_param[t1]|(msk<<t2);
					p_param[t1] = p_param[t1]^0xFF;
				}
			}
			
			for (i=0; i<MAX_LANE_NUM; i++)
			{
				p_param[count++] = gstSysConfig.param_cfg.speed_loop_map[i][0];
				
				if(gstSysConfig.param_cfg.loop_divi_table[i]) p_param[count++] = 0xFF;
				else p_param[count++] = gstSysConfig.param_cfg.speed_loop_map[i][1];
			}			

			// Polling Cycle.
			p_param[count++]= gstSysConfig.param_cfg.polling_cycle;
			
			p_param[count++] = gstSysConfig.param_cfg.num_pluse_present;
			p_param[count++] = gstSysConfig.param_cfg.num_pluse_no_present;			
			break;
		
		case IDX_26_USER_FOR_VENHICLE :
			
			for (i=0; i<SPEED_CATEGORY_NO; i++)
				p_param[count++] = gstSysConfig.param_cfg.speed_category[i];
			
			for (i=0; i<LENGTH_CATEGORY_NO; i++)
				p_param[count++] = gstSysConfig.param_cfg.length_category[i];
			
			p_param[count++] = gstSysConfig.param_cfg.spd_acc_enable;
			
			p_param[count++] = gstSysConfig.param_cfg.len_acc_enable;
				p_param[count++] = gstSysConfig.param_cfg.spd_calc_enable;
			
			p_param[count++] = gstSysConfig.param_cfg.len_calc_enable;
			break;
		
		case IDX_27_USRE_FOR_THRESHOLD :
			
			p_param[count++] = gstSysConfig.param_cfg.upper_vol_limit;
			
			p_param[count++] = gstSysConfig.param_cfg.upper_spd_limit;
			
			p_param[count++] = gstSysConfig.param_cfg.upper_len_limit;
			
			p_param[count++] = gstSysConfig.param_cfg.stuck_high_vol;
			p_param[count++] = gstSysConfig.param_cfg.stuck_on_high;
			p_param[count++] = gstSysConfig.param_cfg.stuck_off_high;
			p_param[count++] = gstSysConfig.param_cfg.stuck_low_vol;
			p_param[count++] = gstSysConfig.param_cfg.stuck_on_low;
			p_param[count++] = gstSysConfig.param_cfg.stuck_off_low;

			// Loop Detector Oscillation Threshold.
			p_param[count++] = gstSysConfig.param_cfg.oscillation_thr;
			break;
			
		default :
			return 0;
			break;
	}

#if defined(DEBUG_PARAMETER)
	fprintf(stdout," parameter size : %d", count);
#endif

	return (uint16) count;
}

void setInterfaces()
{	
	int rtn, bend = 0, interface_fd;
	char buf[32], strtmp[32];
	FILE *fp;

	if(fp = fopen("/etc/network/interfaces", "r+"))
	{

		while(!bend)
		{
			memset(buf, 0, 32);
			if(fscanf(fp, "%s", buf) != EOF)
			{
				if(strcmp(buf, "address") == 0) 
				{
					//fscanf(fp, "%s", buf);
					memset(strtmp, 0, 32);
					sprintf(strtmp," %03d.%03d.%03d.%03d\n", gstNetConfig.ip[0], gstNetConfig.ip[1], gstNetConfig.ip[2], gstNetConfig.ip[3]);					 
					fprintf(fp, "%s", strtmp);
					//fprintf(stdout, "address : %s \n", strtmp);
				}
				else if(strcmp(buf, "netmask") == 0) 
				{
					memset(strtmp, 0, 32);
					sprintf(strtmp," %03d.%03d.%03d.%03d\n", gstNetConfig.nmsk[0], gstNetConfig.nmsk[1], gstNetConfig.nmsk[2], gstNetConfig.nmsk[3]);					 
					fprintf(fp, "%s", strtmp);
					//fprintf(stdout, "address : %s \n", strtmp);
				}
				else if(strcmp(buf, "network") == 0) 
				{
					memset(strtmp, 0, 32);
					sprintf(strtmp," %03d.%03d.0.0\n", gstNetConfig.ip[0], gstNetConfig.ip[1]);					 
					fprintf(fp, "%s", strtmp);
					//fprintf(stdout, "network : %s \n", strtmp);
				}
				else if(strcmp(buf, "gateway") == 0) 
				{
					memset(strtmp, 0, 32);
					sprintf(strtmp," %03d.%03d.%03d.%03d\n", gstNetConfig.gip[0], gstNetConfig.gip[1], gstNetConfig.gip[2], gstNetConfig.gip[3]);					 
					fprintf(fp, "%s", strtmp);
					//fprintf(stdout, "address : %s \n", strtmp);
				}
				else if(strcmp(buf, "hwaddress") == 0) 
				{
					memset(strtmp, 0, 32);
					sprintf(strtmp," %02X:%02X:%02X:%02X:%02X:%02X\n", gstNetConfig.mac[0], gstNetConfig.mac[1], gstNetConfig.mac[2], gstNetConfig.mac[3], gstNetConfig.mac[4], gstNetConfig.mac[5]);		
					fscanf(fp, "%s", buf);
					fprintf(fp, "%s", strtmp);
					//fprintf(stdout, "hwaddress : %s \n", strtmp);
				}
				else
				{
					//fprintf(stdout, "%s \n", buf);
				}
			}
			else
			{
				bend = 1;
				fclose(fp);
			}
		}

		system("ifdown eth0");
		sleep(1);
		system("ifup eth0");		
	}
	
    	//interface_fd = open("/etc/network/interfaces", O_RDWR);
    	//if(interface_fd == -1) {
		//fprintf(stdout,"file open error\n");
		//return FALSE;
	//}
    	//else fprintf(stdout,"open netconfig parameter \n");

    	//rtn = read(interface_fd, buf, 512);		
    	

	//fclose(fp);

	//return (TRUE);
}

void makeMyIpString(uint8 *pIpSrc, uint8 isStrSrcIp)
{

	if (isStrSrcIp == TRUE)
	{		
		memcpy(sysStrMyIpAddr, pIpSrc, IP_STR_LEN);
		sysStrMyIpAddr[IP_STR_LEN] = 0;		
	}
	else
	{		
		netBin2IpStr(pIpSrc, sysStrMyIpAddr);
	}
}

//delete by capi
/*
/////////////////////////////////////////////////////////////////////////////////////
void makeDeviceIDSting(char *pTid, char *pCid, u8_t dbg)
{
	int i;

	// Tunnel ID
	if (pTid != NULL)
		for(i=0; i<TUNNEL_ID_STR_LEN; i++) sysStrDeviceID[i+TUNNEL_ID_STR_POS] = pTid[i];
	else
		for(i=0; i<TUNNEL_ID_STR_LEN; i++) sysStrDeviceID[i+TUNNEL_ID_STR_POS] = gstNetConfig.tunnel_id[i];

	//memcpy(&sysStrDeviceID[DEV_ID_FIX_STR_POS], DEVICE_ID_FIX_STRING, DEV_ID_FIX_STR_LEN);		// eeee

	// Controller ID
	if (pCid != NULL)
		for(i=0; i<CONTROLLER_ID_STR_LEN; i++) sysStrDeviceID[i+CONTROLLER_ID_STR_POS] = pCid[i];
	else
		for(i=0; i<CONTROLLER_ID_STR_LEN; i++) sysStrDeviceID[i+CONTROLLER_ID_STR_POS] = gstNetConfig.controller_id[i];

	// 디버그 메세지 출력.
	if (dbg == TRUE)
	{
		fprintf(stdout, " Device ID : ");
		for(i=0; i<DEVICE_ID_STR_LEN; i++) fprintf(stdout,"%c", sysStrDeviceID[i]);
		fprintf(stdout,"");
	}
}
*/
/////////////////////////////////////////////////////////////////////////////////////
void makeServerIpString(uint8 *pIpSrc, uint8 isStrSrcIp)
{

	if (isStrSrcIp == TRUE)
	{		
		memcpy(sysStrServerIp, pIpSrc, IP_STR_LEN);
		sysStrServerIp[IP_STR_LEN] = 0;		
	}
	else
	{		
		netBin2IpStr(pIpSrc, sysStrServerIp);
	}
}

// Network 설정을 default 값으로....
void defaultNetworkConfig(void)
{

	// Magic number of network cofiguration.
	gstNetConfig.magic_num = NETCFG_MAGIC_NUMBER;

	// MAC Address.
	//netMacStr2Bin(ETHERNET_ADDR_STRING, gstNetConfig.mac);
	//gstNetConfig.mac[5] = gucMacAddr;
	
	// My IP.
	netIpStr2Bin(MY_IP_ADDR, gstNetConfig.ip);
	// Gateway IP.
	netIpStr2Bin(GATEWAY_ADDR, gstNetConfig.gip);
	// Subnet Mask.
	netIpStr2Bin(SUBNET_MASK, gstNetConfig.nmsk);

	// Server IP.
	netIpStr2Bin(VDS_SERVER_IP, gstNetConfig.server_ip);
	// Server Port.
	gstNetConfig.server_port = VDS_SERVER_TCP_PORT;

	// Station Number.
	gstNetConfig.station_num[0] = 0xFF;
	gstNetConfig.station_num[1] = 0xFF;
	gstNetConfig.station_num[2] = 0xFF;
	gstNetConfig.station_num[3] = 0xFF;

	//delete by capi
	// Tunnel ID.
	//memcpy(&gstNetConfig.tunnel_id[0], TUNNEL_ID_STRING, TUNNEL_ID_STR_LEN);

	// Controller ID.
	//memcpy(&gstNetConfig.controller_id[0], CONTROLLER_ID_STRING, CONTROLLER_ID_STR_LEN);

	//makeDeviceIDSting(gstNetConfig.tunnel_id, gstNetConfig.controller_id, TRUE);
	
}

void dumpNetworkConfig(void)
{
	fprintf(stdout,"\n================================================================\n");
	fprintf(stdout,"Dump NetworkConfig - [ Network Configuration ]\n\n");
	fprintf(stdout, " MAC Address : [ %02X:%02X:%02X:%02X:%02X:%02X ]\n", gstNetConfig.mac[0], gstNetConfig.mac[1], \
			gstNetConfig.mac[2], gstNetConfig.mac[3], gstNetConfig.mac[4], gstNetConfig.mac[5]);
	fprintf(stdout, " IP Address : [ %d.%d.%d.%d ]\n", gstNetConfig.ip[0], gstNetConfig.ip[1], \
			gstNetConfig.ip[2], gstNetConfig.ip[3]);

	fprintf(stdout, " Gateway IP Address : [ %d.%d.%d.%d ]\n", gstNetConfig.gip[0], gstNetConfig.gip[1], \
			gstNetConfig.gip[2], gstNetConfig.gip[3]);

	fprintf(stdout, " Netmask : [ %d.%d.%d.%d ]\n", gstNetConfig.nmsk[0], gstNetConfig.nmsk[1], \
			gstNetConfig.nmsk[2], gstNetConfig.nmsk[3]);

	fprintf(stdout, "\n");

	fprintf(stdout, " VDS Server IP : [ %d.%d.%d.%d ]\n", gstNetConfig.server_ip[0], gstNetConfig.server_ip[1], \
			gstNetConfig.server_ip[2], gstNetConfig.server_ip[3]);

	fprintf(stdout, " VDS Server Port : [ %02d ]\n", gstNetConfig.server_port);

	//		gstNetConfig.station_num[2], gstNetConfig.station_num[3]);

	fprintf(stdout," Station Number : [ %05d - %05d ]\n", (gstNetConfig.station_num[0]<<8) + gstNetConfig.station_num[1], \
			(gstNetConfig.station_num[2]<<8) + gstNetConfig.station_num[3]);

//de;ete by capi
/*
	#if 1
	makeDeviceIDSting(gstNetConfig.tunnel_id, gstNetConfig.controller_id, TRUE); // eeee
	#else
	{
		int i;

		for(i=0; i<TUNNEL_ID_STR_LEN; i++) 
			sysStrDeviceID[i+TUNNEL_ID_STR_POS] = gstNetConfig.tunnel_id[i];
		for(i=0; i<CONTROLLER_ID_STR_LEN; i++) 
			sysStrDeviceID[i+CONTROLLER_ID_STR_POS] = gstNetConfig.controller_id[i];

		fprintf(stdout," Device ID : ");
		for(i=0; i<DEVICE_ID_STR_LEN; i++) fprintf(stdout,"%c", sysStrDeviceID[i]);
	}
	#endif
	*/		
	fprintf(stdout,"\n================================================================\n");	
}

//
int chk_n_SaveStrIpToNVRAM(uint8 index, char *psOrgIP, uint8 *pResultIp, uint8 byFlag)
{
	int i, status;
	char tmpStrIp[IP_STR_LEN+1];	
	char setStr[255];
	
	uint8 server_ip[IP_NUM_LEN], *pConfigPtr;
	
	switch(index)
	{
	case MMI_CFG_IDX_01_IP_ADDR:
		pConfigPtr = gstNetConfig.ip;		
		break;
		
	case MMI_CFG_IDX_02_GATEWAY_IP:
		pConfigPtr = gstNetConfig.gip;	
		break;
		
	case MMI_CFG_IDX_03_SUBN_MSK:
		pConfigPtr = gstNetConfig.nmsk;
		break;
	
	case MMI_CFG_IDX_04_SERVER_IP:
		pConfigPtr = gstNetConfig.server_ip;
		break;

	default:
		return (-1);	// Fail
		break;
	}
	
	for (i=0; i<IP_STR_LEN; i++) tmpStrIp[i] = psOrgIP[i];
	tmpStrIp[IP_STR_LEN] = 0;
	
	status = netIpStr2Bin(tmpStrIp, server_ip);
	if (status != 0)
	{
		fprintf(stdout," ############## Invalid IP type string");
		fprintf(stdout," ==> ");
		for (i=0; i<IP_STR_LEN; i++) fprintf(stdout,"%c", psOrgIP[i]);
		fprintf(stdout,"");

		return (-2);	// Fail
	}
	
	// if (server_ip[0] == pConfigPtr[0] && server_ip[1] == pConfigPtr[1] &&
	//	server_ip[2] == pConfigPtr[2] && server_ip[3] == pConfigPtr[3])
	if (!memcmp(server_ip, pConfigPtr, IP_NUM_LEN))
	{
		fprintf(stdout," ############## Same value");
		fprintf(stdout," ==> ");
		for (i=0; i<IP_STR_LEN; i++) fprintf(stdout,"%c", psOrgIP[i]);
		fprintf(stdout,"\n");
		
		return (-3);	// Fail
	}
	
	for(i=0; i<IP_NUM_LEN; i++) pConfigPtr[i] = server_ip[i];

	// sysStrMyIpAddr 
	if ((byFlag & FLAG_SET_IP_STR) && index == MMI_CFG_IDX_01_IP_ADDR)
		makeMyIpString((uint8 *)psOrgIP, TRUE);

	// sysStrServerIp
	if ((byFlag & FLAG_SET_IP_STR) && index == MMI_CFG_IDX_04_SERVER_IP)
		makeServerIpString((uint8 *)psOrgIP, TRUE);

	////////////////////////////////////////////////////////////////////
	//
	fprintf(stdout," New value");
	fprintf(stdout," ==> ");
	for (i=0; i<IP_STR_LEN; i++) fprintf(stdout,"%c", psOrgIP[i]);
	fprintf(stdout,"\n");

	// 네트웍 설정 (Network configuration) 을 Dump.
	dumpNetworkConfig();
	//
	////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////
	// MMI 
	// mmmmmmmmmmmmmmmmmm
//	if (byFlag & FLAG_SEND_TO_MMI) sendNetConfigMsgToMMI(index);

	////////////////////////////////////////////////////////////////////	
	
	if (byFlag && FLAG_WRITE_NVRAM)  //by capi
	{		
		writeNetConfigToNAND(TRUE);
		
		switch(index)
		{
		case MMI_CFG_IDX_01_IP_ADDR:
			/*
			memset (setStr, 0, 255);		
			sprintf(setStr,"ifconfig eth0 %d.%d.%d.%d up"
				, gstNetConfig.ip[0],gstNetConfig.ip[1],gstNetConfig.ip[2],gstNetConfig.ip[3]);	
						
			system(setStr);	
			*/
			setInterfaces();	//by capi interface file write and restart
			break;
			
		case MMI_CFG_IDX_02_GATEWAY_IP:
			/*
			memset (setStr, 0, 255);

			sprintf(setStr,"route add default gw %d.%d.%d.%d dev eth0"
				, gstNetConfig.gip[0],gstNetConfig.gip[1],gstNetConfig.gip[2],gstNetConfig.gip[3]);			
						
			system(setStr);	
			*/
			setInterfaces();	//by capi interface file write and restart
			break;
			
		case MMI_CFG_IDX_03_SUBN_MSK:
			/*
			memset (setStr, 0, 255);

			sprintf(setStr,"ifconfig netmask  %d.%d.%d.%d up"
				, gstNetConfig.nmsk[0],gstNetConfig.nmsk[1],gstNetConfig.nmsk[2],gstNetConfig.nmsk[3]);			
						
			system(setStr);	
			*/
			setInterfaces();	//by capi interface file write and restart
			break;	

		default:
			return (-1);	// Fail
			break;
		}
	}
	
	if (pResultIp != NULL)
	{
		for(i=0; i<IP_NUM_LEN; i++) pResultIp[i] = server_ip[i];
	}

	return 0;
}