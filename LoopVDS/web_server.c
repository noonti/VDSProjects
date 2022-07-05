/*********************************************************************************
/* Filename    : web_server.c
 * Description : dcs9s web server(boa) program
 * Author      : wook
 * Notes       : 
 *********************************************************************************/
//#include <sys/types.h>
//#include <sys/stat.h>
//#include <sys/ioctl.h>
//#include <sys/time.h>
//#include <sys/types.h>
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>

#include <string.h>

#include <errno.h>
//#include <time.h>

#include "ftms_cpu.h"


// [WebServer] FIFO File Open. 2016.01.26
open_fifo_web()
{
    mode_t oldmask;

    // temp mask
    oldmask = umask(0000);

    if(mkfifo(RTD_WEB_FIFO_PATH, 0666) == -1){
        if(errno == 17){
            unlink(RTD_WEB_FIFO_PATH);
            mkfifo(RTD_WEB_FIFO_PATH, 0666);
        }
        else{
            fprintf(stdout, "[Web Server] Failed to call fifo 1(%d) : %s\n", errno, strerror(errno));
            exit(1);            
        }
    }

    if((fifo_web_fd = open(RTD_WEB_FIFO_PATH, O_RDWR)) < 0){
        fprintf(stdout, "[Web Server] Fail to call fifo 2(%d) : %s\n", errno, strerror(errno));
        exit(1);
    }
}

// [WebServer] Real Time Monitorng TextFile Open. 2016.01.26
open_textfile_web()
{
	if((rt_web_fd = open(RTD_WEB_TEXT_PATH, O_WRONLY | O_CREAT | O_TRUNC, 0666))== -1){
	fprintf(stdout, "[Web Server] Failed to create file 1");
	}
}

// [WebServer] Real Time Monitorng Cheak Time. 2016.01.26
chk_rtdata_web()
{
	int cnt_sec = 60000; // 60 sec

	if(rt_web_counter > (cnt_sec * RTD_WEB_TIMER)){
		//fprintf(stdout, "rt_web_counter init\n");
		fprintf(stdout, "[Web Server] RT DATA STOP(%02d:%02d:%02d)\n", sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
		rt_web_flag = RTD_OFF;
		rt_web_counter = 0;
	} 
}

// [WebServer] Set Time(RTC). 2016.01.26
set_rtc_web()
{
	if(st_web_flag == 1){
		/*
		// OS Time Set
		sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
						,web_server_time.wYear, web_server_time.wMonth, web_server_time.wDay
						, web_server_time.wHour, web_server_time.wMinute, web_server_time.wSecond);
		system(timebuf);
		*/

		msec_sleep(300);
		setSysTimeToRTC(&web_server_time);	
		st_web_flag = 0;
	}
}

int sts_fd; //status file descripter
BOOL writeStatusToNAND(uint8 erase)
{
	//int stick, etick;
	uint16 crc16;

	//gstNetConfig.magic_num = NETCFG_MAGIC_NUMBER;
	
	// Calculate CRC-16 for gstSysConfig.
	crc16 = updateCRC16((uint8 *)&web_status, sizeof(web_status), 0);

	// Permission Mask Clear. by wook 2015.11.24
	umask(0000);
	
    sts_fd = open("/root/am1808/status.dat", O_RDWR | O_CREAT, 0666);
    if(sts_fd == -1) {
		fprintf(stdout,"File Open Error\n");
		return FALSE;
	}
    else fprintf(stdout,"Status Saved\n");

    write(sts_fd, &web_status, sizeof(WEB_SERVER_STATUS));

    close(sts_fd);

#if defined(DEBUG_PARAMETER)
	fprintf(stdout, " Write Status to NAND [CRC-16 : 0x%04x]", crc16);
#endif
	
	return ( TRUE );
}

// [WebServer] FIFO Process. 2016.01.26
void fifo_process_web(char *fmsg)
{
    char *ptr;
    //SYSTEMTIME web_server_time;
    char timebuf[255];
    BOOL ret;

	uint val1, val2;
	char sprintbuffer[256];
	char strdate[5];

	int i, j;

	sprintf(sprintbuffer,"%s", __DATE__);	

    char rta_msg[] = "RTD-START";
    char rtb_msg[] = "RTD-STOP";
    char ct_msg[] = "CFG-TIME";
    char sts_msg[] = "STATUS";
    char reset_msg[] = "RESET";

	//fprintf(stdout, "recv: %s_\n", fmsg); // warning: print blk 

	// get command
	ptr = strtok(fmsg, "_");
	//fprintf(stdout, "code = %s\n",ptr);

	if(!strcmp(ptr, rta_msg)){
		fprintf(stdout, "[Web Server] RT DATA START(%02d:%02d:%02d)\n", sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
		rt_web_flag = RTD_ON;
	}

	else if(!strcmp(ptr, rtb_msg)){
		fprintf(stdout, "[Web Server] RT DATA STOP(%02d:%02d:%02d)\n", sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
		rt_web_flag = RTD_OFF;
	}	

	else if(!strcmp(ptr, ct_msg)){

		// Year
		ptr = strtok(NULL, "/");
		//web_server_time.wYear = atoi(ptr);
        ret = str2num(ptr, (int *)&web_server_time.wYear);
        if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Year value !\n");

		// Month
		ptr = strtok(NULL, "/");
		//web_server_time.wMonth = atoi(ptr);
		ret = str2num(ptr, (int *)&web_server_time.wMonth);
		if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Month value !\n");

		// Day
		ptr = strtok(NULL, " ");
		//web_server_time.wDay = atoi(ptr);
		ret = str2num(ptr, (int *)&web_server_time.wDay);
		if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Day value !\n");

		// Hour
		ptr = strtok(NULL, ":");
		//web_server_time.wHour = atoi(ptr);
		ret = str2num(ptr, (int *)&web_server_time.wHour);
		if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Hour value !\n");

		// Minute					
		ptr = strtok(NULL, ":");
		//web_server_time.wMinute = atoi(ptr);
		ret = str2num(ptr, (int *)&web_server_time.wMinute);
		if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Minute value !\n");

		// Second
		//ptr = ptr + strlen(ptr) + 1;
		ptr = strtok(NULL, "\0");
		//web_server_time.wSecond = atoi(ptr);
		ret = str2num(ptr, (int *)&web_server_time.wSecond);

		if (ret == FALSE) fprintf(stdout,"[Web Server] invalid Second value !\n");

		fprintf(stdout,"[Web Server] Set Data & Time to RTC! - ");
		fprintf(stdout,"%d/%d/%d %d:%d:%d\n", web_server_time.wYear, web_server_time.wMonth, web_server_time.wDay,
			web_server_time.wHour, web_server_time.wMinute, web_server_time.wSecond);

		st_web_flag = 1;
	}

	else if(!strcmp(ptr, sts_msg)){
		fprintf(stdout, "[Web Server] Status Check(%02d:%02d:%02d)\n", sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

		web_status.hw_version = VERSION_NUM;
		web_status.sw_version = SW_VERSION_NUM;			

		//fprintf(stdout, "hw = %d, sw = %d\n", web_status.hw_version, web_status.sw_version);

		sprintf(strdate,"%c%c%c", sprintbuffer[0], sprintbuffer[1],  sprintbuffer[2]);	
		
		if(strcmp(strdate, "Jan") == 0) web_status.manu_month =  1;
		else if(strcmp(strdate, "Feb") == 0) web_status.manu_month =  2;	
		else if(strcmp(strdate, "Mar") == 0) web_status.manu_month =  3;	
		else if(strcmp(strdate, "Apr") == 0) web_status.manu_month =  4;	
		else if(strcmp(strdate, "May") == 0) web_status.manu_month =  5;	
		else if(strcmp(strdate, "Jun") == 0) web_status.manu_month =  6;	
		else if(strcmp(strdate, "Jul") == 0) web_status.manu_month =  7;
		else if(strcmp(strdate, "Aug") == 0) web_status.manu_month =  8;
		else if(strcmp(strdate, "Sep") == 0) web_status.manu_month =  9;
		else if(strcmp(strdate, "Oct") == 0) web_status.manu_month =  10;
		else if(strcmp(strdate, "Nov") == 0) web_status.manu_month =  11;
		else if(strcmp(strdate, "Dec") == 0) web_status.manu_month =  12;
		else web_status.manu_month =  0;			

		sprintf(strdate,"%c%c%c%c", sprintbuffer[7], sprintbuffer[8], sprintbuffer[9], sprintbuffer[10]);	
		
		web_status.manu_year[0] = (uint8) (atoi(strdate)/100);
		web_status.manu_year[1] =  (uint8)(atoi(strdate)%100);

		sprintf(strdate,"%c%c", sprintbuffer[4], sprintbuffer[5]);				;
		web_status.manu_day = (uint8) atoi(strdate);

		//fprintf(stdout, "cdate = %d%d/%d/%d\n", web_status.manu_year[0], web_status.manu_year[1], web_status.manu_month , web_status.manu_day);

		web_status.last_opc = dia_status.last_opc;
	 	web_status.lc_year[0] = vds_server_stamp.wYear/100;
	 	web_status.lc_year[1] = vds_server_stamp.wYear%100;
	 	web_status.lc_month = vds_server_stamp.wMonth;
	 	web_status.lc_day = vds_server_stamp.wDay;
	 	web_status.lc_hour = vds_server_stamp.wHour;
	 	web_status.lc_min = vds_server_stamp.wMinute;
	 	web_status.lc_sec = vds_server_stamp.wSecond;

		//fprintf(stdout, "last opc = %d\n", web_status.last_opc);
		//fprintf(stdout, "fcdate = %d%d/%d/%d ", web_status.lc_year[0], web_status.lc_year[1], web_status.lc_month , web_status.lc_day);
		//fprintf(stdout, "fctime = %d:%d:%d\n", web_status.lc_hour, web_status.lc_min , web_status.lc_sec);

		dia_Temp_Status();
		web_status.temp[0] = dia_status.temp[0];
		web_status.temp[1] = dia_status.temp[1];

		//fprintf(stdout, "temp(s1) = %d, temp(s2) = %d\n", web_status.temp[0], web_status.temp[1]);

		gpio_read(power1_fd, &val1);		
		gpio_read(power2_fd, &val2);
		web_status.pw[0] = val1;
		web_status.pw[1] = val2;

		//fprintf(stdout, "power(1) = %d, power(2) = %d\n", web_status.pw[0], web_status.pw[1]);

		checkSystemStatus();
		web_status.head.status = getSysStatusData();

		//fprintf(stdout, "status = %x\n", web_status.head.status);

		web_status.LDBNum = gucActiveLDBNum;

		for(i=0;i<5;i++){
			for(j=0;j<MAX_LOOP_CHANNEL;j++){
				web_status.DetInfo[i].LoopStatus[j] = DetInfo[i].LoopStatus[j];
				web_status.DetInfo[i].FreqLevel[j] = DetInfo[i].FreqLevel[j];
				web_status.DetInfo[i].FreqSel[j] = DetInfo[i].FreqSel[j];
				web_status.DetInfo[i].DetMode[j] = DetInfo[i].DetMode[j];
			}
			web_status.DetInfo[i].OccTime = DetInfo[i].OccTime;
			web_status.DetInfo[i].DetFail = DetInfo[i].DetFail;
			web_status.DetInfo[i].SetMode = DetInfo[i].SetMode;
		}

		writeStatusToNAND(TRUE);
	}
	else if(!strcmp(ptr, reset_msg)){
		fprintf(stdout, "[Web Server] Reset(%02d:%02d:%02d)\n", sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

		// OS Reset
		save_log( WEB_SERVER_REBOOT,0);
		
		system("reboot");

		// Process Reset
		//isReqForceReset = 0x89;
	}	
}