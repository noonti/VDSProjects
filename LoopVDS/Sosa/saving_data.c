/*********************************************************************************
/* Filename    : Saving_data.c
 * Description : dcs9s main program
 * Author      : kwj
 * Notes       : initialize and polling
 *********************************************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/epoll.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>
 #include <sys/stat.h> //insert by capi 2014.08

#include "ftms_cpu.h"
#include "serial.h"
#include "tcpip.h"
#include "user_term.h"

/*
REALTIME_DATA_T gstSaveRTdataPool[MAX_SAVE_RT_DATA];
POLLING_DATA_T gstSavePLdataPool[MAX_SAVE_PL_DATA];
REALTIME_NAND_DATA_T gstSaveRTdataNAND[MAX_SAVE_RT_DATA_NAND];  //by capi 2014.08

static uint32 rt_widx, rt_data_num;
static uint32 pl_widx, pl_data_num;
static uint32 rt_nand_widx, rt_nand_data_num;	//by capi 2014.08
static uint32 lastest_pl_idx;
static uint32 lastest_rt_idx[MAX_LANE_NUM];

static uint32 read_idx, read_num;
*/
// Extern
extern uint8 send_saved_rt_data_flg;
extern uint8 send_saved_pl_data_flg;

extern uint8 gucStuckPerLoop[];
extern uint8 incident_per_loop[];
extern uint8 frame_num;
//extern term_stat;

static uint16 cntNumRtData = 1;

void initRealtimePool()
{
	int i;
	mode_t mode;

	nd_bank = 0;
	rt_widx = 0;
	rt_data_num = 0;

	rt_nand_widx = 0;	
	rt_nand_data_num = 0;
	
	for(i=0; i<MAX_SAVE_RT_DATA; i++) gstSaveRTdataPool[i].valid = 0;
	for(i=0; i<MAX_SAVE_RT_DATA_NAND; i++)
	{
		gstSaveRTdataNAND[0][i].inverse = 0;	//by capi 2014.08	
		gstSaveRTdataNAND[1][i].inverse = 0;	//by capi 2014.08
	}
	//for(i=0; i<MAX_SAVE_RT_DATA; i++) lastest_rt_idx[i] = 0;
	for(i=0; i<MAX_LANE_NUM; i++) lastest_rt_idx[i] = 0;		// by jwank 081218 bug fix.
	
	// by wook 2015.11.24
	umask(0000);
	//mode = S_IRUSR | S_IWUSR | S_IXUSR;
	mode = 0777;

	mkdir("/root/am1808/savefolder", mode);	// by jwank 081218 bug fix.
}

void initPollingPool()
{
	int i;

	pl_widx = 0;
	pl_data_num = 0;
	for(i=0; i<MAX_SAVE_PL_DATA; i++) gstSavePLdataPool[i].valid = 0;
	lastest_pl_idx = 0;
}

// 저장된 교통 데이터를 전송한다.
void sendSavingTrafficData()
{
	switch(term_stat)
	{
		case SEND_PL_SAVE_MODE :
#if 0
			if (chkXmodemEOB() == 1)
			{
				// 저장된 Polling 데이터를 만들어서 준비함.
				preparePLSavingData();
				putXmodHeaderStat();
			}
#endif
			break;
			
		case SEND_RT_SAVE_MODE :
#if 0
			if (chkXmodemEOB() == 1)
			{
				// 저장된 Realtime 데이터를 만들어서 준비함.
				prepareRTSavingData();
				putXmodHeaderStat();
			}
#endif
			break;
	}

	
#if 0
	xmodemTransBypass();
	if (chkXmodemFinished() == 1)
	{	
		// XMODEM 데이터 전송이 완료되면 전송모드에서 나옴.

		// 디버그 포트를 디버깅 메세지를 나올 수 있게 재설정.
		//console_enb = TRUE;
		setConsoleDebugMode(TRUE);
		
		term_stat = DATA_SAVE_MENU;
			
		fprintf(stdout, " XMODEM transfer is finished !!\n");
		fprintf(stdout, "\n");
		print_prompt();
	}
#endif

}

// Realtime데이터를 저장함. by capi 2014.08
void saveRealtimeDatatoNand(uint32 t_lane, uint8 inv)
{
	char buf[50];

	if (t_lane >= MAX_LANE_NUM) return;
	
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.year = sys_time.wYear % 100;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.month = sys_time.wMonth;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.day = sys_time.wDay;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.hour = sys_time.wHour;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.min = sys_time.wMinute;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.sec = sys_time.wSecond;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].time.msec = sys_time.wMilliseconds;
	
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].lane = t_lane;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].length = (uint16) gstCurrData.length[t_lane];
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].inverse = inv;
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].speed = (gstCurrData.speed[t_lane]);	// 2012.01.18 by capidra delete /100
	
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].occupy[START_POS] = (uint16) gstCurrData.occupy[gstLaneLink[t_lane].s_loop];
	gstSaveRTdataNAND[nd_bank][rt_nand_widx].occupy[END_POS] = (uint16) gstCurrData.occupy[gstLaneLink[t_lane].e_loop];
	
	rt_nand_widx++;
	if (rt_nand_widx >= MAX_SAVE_RT_DATA_NAND) rt_nand_widx=0;
	if (rt_nand_data_num < MAX_SAVE_RT_DATA_NAND) rt_nand_data_num++;

	// data save
	//sprintf(buf,"sec = %3d, lane = %d, speed = %d\n", sys_time.wSecond, t_lane, gstCurrData.speed[t_lane]);
	//write(rt_fd,buf,strlen(buf));
}

void saveRealtimeData(uint32 t_lane)
{
	char buf[50];

	if (t_lane >= MAX_LANE_NUM) return;
	
	gstSaveRTdataPool[rt_widx].time.year = sys_time.wYear % 100;
	gstSaveRTdataPool[rt_widx].time.month = sys_time.wMonth;
	gstSaveRTdataPool[rt_widx].time.day = sys_time.wDay;
	gstSaveRTdataPool[rt_widx].time.hour = sys_time.wHour;
	gstSaveRTdataPool[rt_widx].time.min = sys_time.wMinute;
	gstSaveRTdataPool[rt_widx].time.sec = sys_time.wSecond;
	gstSaveRTdataPool[rt_widx].time.msec = sys_time.wMilliseconds;
	
	gstSaveRTdataPool[rt_widx].lane = t_lane;
	gstSaveRTdataPool[rt_widx].length = (uint16) gstCurrData.length[t_lane];
	#if defined(INCREASE_SPEED_PRECISION)
	gstSaveRTdataPool[rt_widx].speed = (uint16) (gstCurrData.speed[t_lane]);	//2012.01.18 by capidra delete /100
	#else
	gstSaveRTdataPool[rt_widx].speed = (uint16) gstCurrData.speed[t_lane];	
	#endif
	gstSaveRTdataPool[rt_widx].volume = (uint16) gstCenterAccuData.volume[gstLaneLink[t_lane].e_loop];
	gstSaveRTdataPool[rt_widx].occupy[START_POS] = (uint16) gstCurrData.occupy[gstLaneLink[t_lane].s_loop];
	gstSaveRTdataPool[rt_widx].occupy[END_POS] = (uint16) gstCurrData.occupy[gstLaneLink[t_lane].e_loop];

	gstSaveRTdataPool[rt_widx].valid = 1;

	lastest_rt_idx[t_lane] = rt_widx;
	
	rt_widx++;
	if (rt_widx >= MAX_SAVE_RT_DATA) rt_widx=0;
	if (rt_data_num < MAX_SAVE_RT_DATA) rt_data_num++;

	// data save
	//sprintf(buf,"sec = %3d, lane = %d, speed = %d\n", sys_time.wSecond, t_lane, gstCurrData.speed[t_lane]);
	//write(rt_fd,buf,strlen(buf));
}

//Realtime data(Webserver) TextFile Write. by wook 2015.11.24
void saveRealtimeDatatoText(uint32 t_lane, uint8 inv)
{
	char rtd_str[128];
	int ret;
	struct stat buf;

	if (t_lane >= MAX_LANE_NUM) return;

	// file size check
	ret = stat(RTD_WEB_TEXT_PATH, &buf);
	if( ret != 0 ) {
		perror("stat()");
		//exit(errno);
		return;
	}
	//fprintf(stdout,"len = %ld\n", buf.st_size);

	if(buf.st_size > RTD_WEB_MAX_SIZE){

		close(rt_web_fd);

		if((rt_web_fd = open(RTD_WEB_TEXT_PATH, O_WRONLY | O_CREAT | O_TRUNC, 0777))== -1){
			fprintf(stdout,"[Web Server] Failed to create file 2");
		}
	}

    if(inv == 1){
		sprintf(rtd_str, "%04d %04d/%02d/%02d %02d:%02d:%02d:%02d 차로:%02d 속도:-%05lu 점유시간1:%05lu 점유시간2:%05lu 길이:%05lu\n",
			cntNumRtData % 10000,
			sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
			sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
			(sys_time.wMilliseconds/10)%100,
			t_lane + 1, gstCurrData.speed[t_lane],	
			(uint16) gstCurrData.occupy[gstLaneLink[t_lane].s_loop],
			(uint16) gstCurrData.occupy[gstLaneLink[t_lane].e_loop],
			(uint16) gstCurrData.length[t_lane]);
	}

	else{
		sprintf(rtd_str, "%04d %04d/%02d/%02d %02d:%02d:%02d:%02d 차로:%02d 속도:%05lu 점유시간1:%05lu 점유시간2:%05lu 길이:%05lu \n",
			cntNumRtData % 10000,
			sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
			sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
			(sys_time.wMilliseconds/10)%100,
			t_lane + 1, gstCurrData.speed[t_lane],	
			(uint16) gstCurrData.occupy[gstLaneLink[t_lane].s_loop],
			(uint16) gstCurrData.occupy[gstLaneLink[t_lane].e_loop],
			(uint16) gstCurrData.length[t_lane]);
	}

    cntNumRtData++;
    if(cntNumRtData > 10000) cntNumRtData = 1;

	write(rt_web_fd, rtd_str, strlen(rtd_str));

	// data write.
	if (fsync(rt_web_fd) == -1)
		fprintf(stdout, "[Web Server] Failed to fsync()");

   	//close(rt_web_fd);
}

// 저장된 Realtime 데이터를 만들어서 준비함.
static void prepareRTSavingData_m()
{
	REALTIME_DATA_PACK_t RtData;
	int j;
	uint32 ridx;
	uint8 isEnd = 0;
	
	ridx = read_idx;

	RtData.time.year = gstSaveRTdataPool[ridx].time.year;
	RtData.time.month = gstSaveRTdataPool[ridx].time.month;
	RtData.time.day = gstSaveRTdataPool[ridx].time.day;
	RtData.time.hour = gstSaveRTdataPool[ridx].time.hour;
	RtData.time.min = gstSaveRTdataPool[ridx].time.min;
	RtData.time.sec = gstSaveRTdataPool[ridx].time.sec;
	RtData.time.msec = gstSaveRTdataPool[ridx].time.msec;
	
	RtData.valid = gstSaveRTdataPool[ridx].valid;
	RtData.lane = gstSaveRTdataPool[ridx].lane+1;		
	RtData.length[0] = (uint8) (gstSaveRTdataPool[ridx].length/100);
	RtData.length[1] = (uint8) (gstSaveRTdataPool[ridx].length%100);
	RtData.speed[0] = (uint8) (gstSaveRTdataPool[ridx].speed/100);
	RtData.speed[1] = (uint8) (gstSaveRTdataPool[ridx].speed%100);
	RtData.volume[0] = (uint8) (gstSaveRTdataPool[ridx].volume/100);
	RtData.volume[1] = (uint8) (gstSaveRTdataPool[ridx].volume%100);
	for(j=0; j<2; j++)
	{
		RtData.occupy[j][0] = (uint8) (gstSaveRTdataPool[ridx].occupy[j]/100);
		RtData.occupy[j][1] = (uint8) (gstSaveRTdataPool[ridx].occupy[j]%100);
	}

	ridx++;
	if (ridx >= MAX_SAVE_RT_DATA) ridx=0;
	
	read_idx = ridx;

	read_num--;
	if (!read_num) isEnd = 1;


	if (isEnd)
	{
		send_saved_rt_data_flg = 0;
		sendSavingRTdataPacket(&RtData, 1);
	}
	else
		sendSavingRTdataPacket(&RtData, 0);
}


void savePollingData()
{
	int j;

	// 
	gstSavePLdataPool[pl_widx].time.year = sys_time.wYear % 100;
	gstSavePLdataPool[pl_widx].time.month = sys_time.wMonth;
	gstSavePLdataPool[pl_widx].time.day = sys_time.wDay;
	gstSavePLdataPool[pl_widx].time.hour = sys_time.wHour;
	gstSavePLdataPool[pl_widx].time.min = sys_time.wMinute;
	gstSavePLdataPool[pl_widx].time.sec = sys_time.wSecond;
	
	//
	checkSystemStatus();
	gstSavePLdataPool[pl_widx].status = getSysStatusData();
	gstSavePLdataPool[pl_widx].frame_num = frame_num;
			
	for(j=0; j<MAX_LOOP_NUM/4; j++) gstSavePLdataPool[pl_widx].stuck_per_loop[j] = gucStuckPerLoop[j];
	for(j=0; j<MAX_LOOP_NUM/8; j++) gstSavePLdataPool[pl_widx].incid_per_loop[j] = incident_per_loop[j];

	for(j=0; j<MAX_LOOP_NUM; j++)
	{
		gstSavePLdataPool[pl_widx].volume[j] = gstCenterPolling.volume[j];
		#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
		gstSavePLdataPool[pl_widx].occupy[j] = round_for_occupy(gstCenterPolling.occupy[j], 0);
		#else
		gstSavePLdataPool[pl_widx].occupy[j] = gstCenterPolling.occupy[j];
		#endif
	}

	for(j=0; j<MAX_LANE_NUM/2; j++)
	{
		gstSavePLdataPool[pl_widx].speed[j] = gstCenterPolling.speed[j];
		gstSavePLdataPool[pl_widx].length[j] = gstCenterPolling.length[j];
	}
	
	gstSavePLdataPool[pl_widx].valid = 1;

	lastest_pl_idx = pl_widx;
	
	pl_widx++;
	if (pl_widx >= MAX_SAVE_PL_DATA) pl_widx=0;
	if (pl_data_num < MAX_SAVE_PL_DATA) pl_data_num++;		//debug by capidra MAX_SAVE_RT_DATA->MAX_SAVE_PL_DATA
}


static void preparePLSavingData_m()
{
	POLLING_DATA_PACK_t PlData;
	int j;
	uint32 ridx;
	uint8 isEnd = 0;
	
	ridx = read_idx;

	PlData.time.year = gstSavePLdataPool[ridx].time.year;
	PlData.time.month = gstSavePLdataPool[ridx].time.month;
	PlData.time.day = gstSavePLdataPool[ridx].time.day;
	PlData.time.hour = gstSavePLdataPool[ridx].time.hour;
	PlData.time.min = gstSavePLdataPool[ridx].time.min;
	PlData.time.sec = gstSavePLdataPool[ridx].time.sec;
	PlData.time.msec = gstSavePLdataPool[ridx].time.msec;
	
	PlData.valid = gstSavePLdataPool[ridx].valid;
	PlData.frame_num = gstSavePLdataPool[ridx].frame_num;
	PlData.status = gstSavePLdataPool[ridx].status;

	for(j=0; j<(MAX_LOOP_NUM/4); j++)
		PlData.stuck_per_loop[j] = gstSavePLdataPool[ridx].stuck_per_loop[j];
	for(j=0; j<(MAX_LOOP_NUM/8); j++)
		PlData.incid_per_loop[j] = gstSavePLdataPool[ridx].incid_per_loop[j];
	for(j=0; j<(MAX_LOOP_NUM); j++)
		PlData.volume[j] = gstSavePLdataPool[ridx].volume[j];
	for(j=0; j<(MAX_LOOP_NUM); j++)
		PlData.occupy[j] = gstSavePLdataPool[ridx].occupy[j];
	for(j=0; j<(MAX_LANE_NUM); j++)
		PlData.speed[j] = gstSavePLdataPool[ridx].speed[j];
	for(j=0; j<(MAX_LANE_NUM); j++)
		PlData.length[j] = gstSavePLdataPool[ridx].length[j];
	
	ridx++;
	if (ridx >= MAX_SAVE_PL_DATA) ridx=0;
	
	read_idx = ridx;

	read_num--;
	if (!read_num) isEnd = 1;


	if (isEnd)
	{
		fprintf(stdout,"One Hour End \n");
		send_saved_pl_data_flg = 0;
		sendSavingPLdataPacket(&PlData, 1);
	}
	else
		sendSavingPLdataPacket(&PlData, 0);

}


void sendSavingTrafficData_m()
{
	if (saving_send_counter >= 30)		//by capidra 500 -> 20
	{
		saving_send_counter = 0;
		
		if (send_saved_rt_data_flg)
		{
			prepareRTSavingData_m();
			
		}
		else if (send_saved_pl_data_flg)
		{
			preparePLSavingData_m();
		}
	}
}

/*
static void * sendSavingTrafficDatatoNand(void *arg)
{
	int stick, etick, i,j;
	unsigned int ridx, idx = 0;
	char name[256];	
	FILE *fp;	
	
	sprintf(name,"/root/am1808/savefolder/%04d%02d%02d/%02d%02d%02d.dat"
		, sys_time.wYear, sys_time.wMonth, sys_time.wDay, sys_time.wMonth, sys_time.wDay, sys_time.wHour);
		
    
    	if(!(fp = fopen(name, "w")))
    	{   
		fprintf(stdout,"file open error\n");
		return FALSE;
	}
    	else fprintf(stdout,"save realtime data\n");

	uint8 isEnd = 0;

	while(!isEnd)
	{
		ridx = read_idx;		
		
		fprintf(fp, "%04d  ", idx);    
		fprintf(fp, "%04d/", gstSaveRTdataNAND[ridx].time.year+2000);
		fprintf(fp, "%02d/", gstSaveRTdataNAND[ridx].time.month);
		fprintf(fp, "%02d  ", gstSaveRTdataNAND[ridx].time.day);
		fprintf(fp, "%02d:", gstSaveRTdataNAND[ridx].time.hour);
		fprintf(fp, "%02d:", gstSaveRTdataNAND[ridx].time.min);
		fprintf(fp, "%02d:", gstSaveRTdataNAND[ridx].time.sec);
		fprintf(fp, "%02d  ", gstSaveRTdataNAND[ridx].time.msec/10);

		fprintf(fp, "Lane :%02d  ", gstSaveRTdataNAND[ridx].lane+1);

		if(gstSaveRTdataNAND[ridx].inverse)
			fprintf(fp, "Speed :-%05d  ", gstSaveRTdataNAND[ridx].speed);
		else
			fprintf(fp, "Speed : %05d  ", gstSaveRTdataNAND[ridx].speed);

		fprintf(fp, "OCC1 :%05d  ", gstSaveRTdataNAND[ridx].occupy[0]);
		fprintf(fp, "OCC2 :%05d  ", gstSaveRTdataNAND[ridx].occupy[1]);
		fprintf(fp, "Length :%02d \r\n", gstSaveRTdataNAND[ridx].length);

		watchdog_counter = 0;
		
		idx++;
		ridx++;
		if (ridx >= MAX_SAVE_RT_DATA_NAND) ridx=0;
		
		read_idx = ridx;

		read_num--;
		if (!read_num) isEnd = 1;

		usleep(1000);
	}    

   	fclose(fp);
	
	//return ( TRUE );
}
*/

void save_log( uint8 err_code, uint8 val)
{
	char logbuff[255];
	char strlog[255];
	int ret;
	int fd_log;
	struct stat buf;
	

	sprintf(logbuff,"/root/am1808/logdata.txt");
	//fd_log = creat(logbuff, 0666);
	fd_log = open(logbuff, O_WRONLY | O_CREAT | O_APPEND, 0666);

	if(fd_log == -1)
	{
		fprintf(stdout,"log file create error \n");
		return;
	}

	//ret = lseek(fd_log, 0, SEEK_END);

	if(ret == -1) fprintf(stdout, " Seek Error \n");

	// file size check
	
	//if((fd_w = open(fname, O_WRONLY | O_CREAT | O_TRUNC, 0777))== -1){
	//	fprintf(stdout,"Failed to create file");
	//}
	

	switch(err_code)
	{
		case START_PROC :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Start Program.. \n",													
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case ABORT_NET_NOPING :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Network Abort Pingtest.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case ABORT_NET_VALIDSETION :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Network Abort Valid Session.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case ABORT_NET_NOCSN :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Network Abort.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
			

		case CONNECT_SERVER :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Connecting Server.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case NET_COLSE_SERVER :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d] Server Process closed.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case NET_COLSE_SESSION :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Net Session closed.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;		

		case IOBOARD_COMM_ERROR :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  IO Board communication Error.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case LDBOARD_COMM_ERROR :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  LD Board-%d Communication Error.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100, val);
			break;

		case PARAMETER_UPDATE :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Parameter Update.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;

		case NETCONFIG_UPDATE :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Netconfig Update.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
			
		case WATCHDOG_RESET :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Watchdog Reset.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case SERVER_COMMAND_RESET :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Server Command Reset.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case SERVER_COMMAND_SYSINIT :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Server Command Systrem init.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case USERTERM_RESET :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  User Terminal Reset.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case MAINTENANCE_RESET :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Maintenance Reset.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case POLL_CREATE_EXIT :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Poll Create Exit Error.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case POLL_CALLOC_EXIT :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Poll calloc Exit Error.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		case WEB_SERVER_REBOOT :
			memset(strlog, 0, 255);
			sprintf(strlog, "[%04d/%02d/%02d %02d:%02d:%02d:%02d]  Web Server Reboot.. \n",
													sys_time.wYear, sys_time.wMonth, sys_time.wDay, 
													sys_time.wHour, sys_time.wMinute, sys_time.wSecond,
													(sys_time.wMilliseconds/10)%100);
			break;
		default :
			break;
	}   
    
	write(fd_log, strlog, strlen(strlog));

	// data write.
	if (fsync(fd_log) == -1)
	{
		if(errno == EINVAL)
		{
			if(fdatasync(fd_log) == -1) fprintf(stdout, "fdatasync() fail");
			else fprintf(stdout, "fsync() fail"); 
		}		   
	}
   	close(fd_log);
}


//by capi 2014.08
uint8 kickPLSavingData_m()
{
	if (pl_data_num == 0)
	{	
		return 0;
	}
	else if (pl_data_num == MAX_SAVE_PL_DATA)
	{
		read_idx = pl_widx;
		read_num = MAX_SAVE_PL_DATA;
	}
	else
	{
		read_idx = 0;
		read_num = pl_widx;
	}
	return 1;
}


//by capi 2014.08
uint8 kickRTSavingData_m()
{
	if (rt_data_num == 0)
	{	
		return 0;
	}
	else if (rt_data_num == MAX_SAVE_RT_DATA)
	{
		read_idx = rt_widx;
		read_num = MAX_SAVE_RT_DATA;
	}
	else
	{
		read_idx = 0;
		read_num = rt_widx;
	}
	return 1;
}

//by capi 2014.08 save realtime data to nand
uint8 kickRTSavingDatatoNand_m()
{
	if (rt_nand_data_num == 0)
	{	
		return 0;
	}
	else if (rt_nand_data_num == MAX_SAVE_RT_DATA_NAND)
	{
		read_nd_idx = rt_nand_widx;
		read_nd_num = MAX_SAVE_RT_DATA_NAND;
		//fprintf(stdout,"MAX %d: %d : %d\n",rt_nand_data_num , read_nd_idx, read_nd_num);
	}
	else
	{
		read_nd_idx = 0;
		read_nd_num = rt_nand_widx;
		
		//fprintf(stdout," %d: %d : %d\n",rt_nand_data_num, read_nd_idx, read_nd_num);
	}

        if(term_stat !=REALTIME_MONITOR_MODE )
		fprintf(stdout,"Save Realtime data Volumn Cnt : %06d \n", read_nd_num);
	
	rt_nand_widx = 0;
	rt_nand_data_num = 0;

	savendBank = nd_bank;
	nd_bank = nd_bank ? 0 : 1;
	
	return 1;
}

