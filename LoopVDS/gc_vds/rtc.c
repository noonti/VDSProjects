/*********************************************************************************
/* Filename    : rtc.c
 * Description :
 * Author      : kwj
 * Notes       :
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
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>
#include <dirent.h>         //by capi 2014.08
#include <sys/stat.h>       //by capi 2014.08

#include <termios.h>
#include <unistd.h>
#include <linux/i2c.h>
#include <linux/i2c-dev.h>
#include <linux/rtc.h>
#include <mtd/mtd-user.h>

#include "ftms_cpu.h"
#include "serial.h"
#include "tcpip.h"
#include "user_term.h"


//int i2c_rtc;
extern unsigned int loop_count;
extern int max_msec_count;
extern int System_Debug;
//extern global_counter;
extern int network_alive;

//extern term_stat;

/** time variable */
time_t t;
struct tm *locp, loc, *tt;
int osec;
static int ohour = 100;
static int oday = 100;
static uint8 isSave_RTdata;        //by capi

static void * sendSavingTrafficDatatoNand(void *arg);

extern int sync_time_flag;


int deleteFolder(const char* path, int force)
{
    DIR* dir_ptr = NULL;
    struct dirent* file = NULL;
    struct stat   buf;
    char   filename[1024];
    char strbuf[1024];
    printf("deleteFolder...%s \n", path);

    sprintf(strbuf, "deleteFolder[%s] enter....", path);
    write_debug_log(strbuf);

    /* 목록을 읽을 디렉토리명으로 DIR *를 return 받습니다. */
    if ((dir_ptr = opendir(path)) == NULL) {
        /* path가 디렉토리가 아니라면 삭제하고 종료합니다. */
        return remove(path);
    }

    /* 디렉토리의 처음부터 파일 또는 디렉토리명을 순서대로 한개씩 읽습니다. */
    while ((file = readdir(dir_ptr)) != NULL) {
        // readdir 읽혀진 파일명 중에 현재 디렉토리를 나타네는 . 도 포함되어 있으므로 
        // 무한 반복에 빠지지 않으려면 파일명이 . 이면 skip 해야 함
        if (strcmp(file->d_name, ".") == 0 || strcmp(file->d_name, "..") == 0) {
            continue;
        }

        sprintf(filename, "%s/%s", path, file->d_name);

        /* 파일의 속성(파일의 유형, 크기, 생성/변경 시간 등을 얻기 위하여 */
        if (lstat(filename, &buf) == -1) {
            continue;
        }


        if (S_ISDIR(buf.st_mode)) { // 검색된 이름의 속성이 디렉토리이면
            
            sprintf(strbuf, "delete...filename = %s  is directory \n", filename);
            write_debug_log(strbuf);

            /* 검색된 파일이 directory이면 재귀호출로 하위 디렉토리를 다시 검색 */
            if (deleteFolder(filename, force) == -1 && !force) {
                return -1;
            }
        }
        else if (S_ISREG(buf.st_mode) || S_ISLNK(buf.st_mode)) { // 일반파일 또는 symbolic link 이면
            
            sprintf(strbuf,"delete...filename = %s  is file ", filename);
            write_debug_log(strbuf);

            if (remove(filename) == -1 && !force) {
                return -1;
            }
        }
    }

    /* open된 directory 정보를 close 합니다. */
    closedir(dir_ptr);

    sprintf(strbuf, "deleteFolder[%s] leave....", path);
    write_debug_log(strbuf);


    return rmdir(path);
}

int checkSavedDataDirectory(char* szFilePath, int days)
{
    DIR* dir_ptr = NULL;
    struct dirent* file = NULL;
    struct stat   buf;

    char   filename[1024];
    struct tm* fileTime;
    struct tm* pCurTime = NULL;
    time_t curTime = time(NULL);
    pCurTime = localtime(&curTime);

    /* 목록을 읽을 디렉토리명으로 DIR *를 return */
    if ((dir_ptr = opendir(szFilePath)) == NULL) {
        /* path가 디렉토리가 아니면  종료. */
        return 0;
    }
    write_debug_log("checkSavedDataDirectory ...enter");
    /* 디렉토리의 처음부터 파일 또는 디렉토리명을 순서대로 한개씩 읽습니다. */
    while ((file = readdir(dir_ptr)) != NULL) {
        // readdir 읽혀진 파일명 중에 현재 디렉토리를 나타네는 . 도 포함되어 있으므로 
        // 무한 반복에 빠지지 않으려면 파일명이 . 이면 skip 해야 함
        if (strcmp(file->d_name, ".") == 0 || strcmp(file->d_name, "..") == 0) {
            continue;
        }

        sprintf(filename, "%s/%s", szFilePath, file->d_name);

        /* 파일의 속성(파일의 유형, 크기, 생성/변경 시간 등을 얻기 위하여 */
        if (lstat(filename, &buf) == -1) {
            continue;
        }

        double passedSecond = (curTime - buf.st_ctime) / (60 * 60 * 24); // 

        if (passedSecond >= days) // 정한 기간을 초과하였을 경우 삭제 진행 
        {
            if (S_ISDIR(buf.st_mode))  // 검색된 이름의 속성이 디렉토리이면
            {
                deleteFolder(filename, 1);
            }
            else if (S_ISREG(buf.st_mode) || S_ISLNK(buf.st_mode)) // 일반파일 또는 symbolic link 이면
            {
                remove(filename);
            }

        }
    }
    /* open된 directory 정보를 close 합니다. */
    closedir(dir_ptr);
    write_debug_log("checkSavedDataDirectory ...leave");
    return 1;
}


void getOsTime(SYSTEMTIME *lpSysTime)
{

	time_t timev;
	struct tm tm_src;
	struct tm *tm_data;
	struct tm * date;
	const time_t t = time(NULL);
	tm_data = localtime(&t);
	fprintf(stdout, "[%02d/%02d/%02d %02d:%02d:%02d]\n", tm_data->tm_year + 1900, tm_data->tm_mon + 1, tm_data->tm_mday,
		tm_data->tm_hour, tm_data->tm_min, tm_data->tm_sec);

	lpSysTime->wYear = tm_data->tm_year + 1900;
	lpSysTime->wMonth = tm_data->tm_mon + 1;
	lpSysTime->wDay = tm_data->tm_mday;
	lpSysTime->wHour = tm_data->tm_hour;
	lpSysTime->wMinute = tm_data->tm_min;
	lpSysTime->wSecond = tm_data->tm_sec;


	/*
	2021.03.23 하드코딩 제거 by avogadro
    time_t timev;
    struct tm tm_src;
    struct tm *tm_data;

    tm_src.tm_year	= 2016 - 1900;
    tm_src.tm_mon 	= 9 - 1;
    tm_src.tm_mday 	=11;
    tm_src.tm_hour 	= 5;
    tm_src.tm_min 	= 10;
    tm_src.tm_sec 	= 7;

    timev = mktime(&tm_src);
    tm_data = gmtime(&timev);

    fprintf(stdout,"[%02d/%02d/%02d %02d:%02d:%02d]\n", tm_data->tm_year + 1900, tm_data->tm_mon + 1, tm_data->tm_mday,
            tm_data->tm_hour, tm_data->tm_min, tm_data->tm_sec);

    lpSysTime->wSecond 	=  tm_data->tm_year + 1900;
    lpSysTime->wMinute 	= tm_data->tm_mon + 1;
    lpSysTime->wHour   	= tm_data->tm_mday;
    lpSysTime->wDay    	= tm_data->tm_hour;
    lpSysTime->wMonth  	= tm_data->tm_min;
    lpSysTime->wYear  	 = tm_data->tm_sec;
	 */
}

int getRtcTime (SYSTEMTIME *lpSysTime, uint8 debug)
{
    unsigned char rtc[64];
    int i2c_rtc;

    i2c_rtc = open("/dev/i2c-1",O_RDWR);

    if(i2c_rtc < 0)
    {
        fprintf(stdout,"i2c-1 open fail");
        //exit(1);
        return;
    }

    //if(ioctl(i2c,I2C_SLAVE, 0x50)<0) //eeprom
    if(ioctl(i2c_rtc,I2C_SLAVE, 0x68)<0)
    {
        fprintf(stdout,"rtc dev addr set error \n");
        //exit(1);
        return;
    }

#if 0 // internal RTC

    t = time(NULL);
    locp = localtime(&t);
    loc = *locp;

    lpSysTime->wYear = loc.tm_year+1900;
    lpSysTime->wMonth = loc.tm_mon+1;
    lpSysTime->wDay = loc.tm_mday;
    lpSysTime->wHour = loc.tm_hour;
    lpSysTime->wMinute = loc.tm_min;
    lpSysTime->wSecond = loc.tm_sec;

#else // external RTC
    //read RTC
    rtc[0] = 0x0;
    write(i2c_rtc,rtc,1);
    read(i2c_rtc,rtc,0x10);
    //sleep(1);


    //lpSysTime->wMilliseconds = loop_count * 1000 / max_msec_count;
    lpSysTime->wMilliseconds = loop_count *1000/ max_msec_count;
    lpSysTime->wSecond = (rtc[0] & 0x0f) + (rtc[0]>>4) *10;
    lpSysTime->wMinute = (rtc[1] & 0x0f) + (rtc[1]>>4) *10;
    lpSysTime->wHour   = (rtc[2] & 0x0f) + (rtc[2]>>4) *10;
    lpSysTime->wDay    = (rtc[4] & 0x0f) + (rtc[4]>>4) *10;
    lpSysTime->wMonth  = (rtc[5] & 0x0f) + (rtc[5]>>4) *10;
    lpSysTime->wYear   = (rtc[6] & 0x0f) + (rtc[6]>>4) *10 + 2000;

	 
    //fprintf(stdout,"  [%02d/%02d/%02d, %d:%d:%d]\n",  year,mon,day, hour,min, sec);
#endif

    if (debug == 1)
    {
        fprintf(stdout,"[%02d/%02d/%02d %02d:%02d:%02d]\n", lpSysTime->wYear, lpSysTime->wMonth, lpSysTime->wDay,
                lpSysTime->wHour, lpSysTime->wMinute, lpSysTime->wSecond);
    }

    close(i2c_rtc);
    return 0;
}

void setOsTime(SYSTEMTIME *lpSysTime)
{
    struct tm tm_src;
    struct timeval tv = {0, 0};

    tm_src.tm_year	= lpSysTime->wYear - 1900;
    tm_src.tm_mon 	= lpSysTime->wMonth - 1;
    tm_src.tm_mday 	= lpSysTime->wDay;
    tm_src.tm_hour 	= lpSysTime->wHour;
    tm_src.tm_min 	= lpSysTime->wMinute;
    tm_src.tm_sec 	= lpSysTime->wSecond;

    tv.tv_sec = mktime(&tm_src);

    settimeofday(&tv, NULL);
}

void setSysTimeToRTC(SYSTEMTIME *lpSysTime)
{
    int i2c_rtc;

    //OS Time Change
    setOsTime(lpSysTime);

    //RTC Time Change
    i2c_rtc = open("/dev/i2c-1",O_RDWR);

    if(i2c_rtc < 0)
    {
        fprintf(stdout,"i2c-1 open fail");
        //exit(1);
        return;
    }

    //if(ioctl(i2c,I2C_SLAVE, 0x50)<0) //eeprom
    if(ioctl(i2c_rtc,I2C_SLAVE, 0x68)<0)
    {
        fprintf(stdout,"rtc dev addr set error \n");
        //exit(1);
        return;
    }

    unsigned char byYear=0, byMonth=0, byDate=0, byDay=0;
    unsigned char byHour=0, byMinute=0, bySecond=0;
    unsigned char rtc[64];

	 // 2021.04.14 초기화 되지 않아서 쓰레기 값으로 시간 저장 될 경우 오류 발생. 초기화 코드 추가 by avogadro
	 memset(rtc, 0, 64);

    byYear = (unsigned char)(lpSysTime->wYear - 2000);
    rtc[7] =  (((byYear/10)<<4) & 0xF0) | ( byYear % 10 );

    byMonth= (unsigned char)lpSysTime->wMonth;
    rtc[6] =  (((byMonth/10)<<4) & 0x10) | ( byMonth % 10 );

    byDate = (unsigned char)lpSysTime->wDay;
    rtc[5] =  (((byDate/10)<<4) & 0x30) | ( byDate % 10 );

//	byDay = (unsigned char)lpSysTime->wDayOfWeek;
//	rtc[4] =  byDay & 0x07;

    byHour = (unsigned char)lpSysTime->wHour;
    rtc[3] =  (((byHour/10)<<4) & 0x30) | ( byHour % 10 );

    byMinute = (unsigned char)lpSysTime->wMinute;
    rtc[2] =  (((byMinute/10)<<4) & 0x70) | ( byMinute % 10 );

    bySecond = (unsigned char)lpSysTime->wSecond;
    rtc[1] =  (((bySecond/10)<<4) & 0x70) | ( bySecond % 10 );

	 /*printf("setSysTimeToRTC...byYear=%d byMonth=%d byDate=%d byHour=%d byMinute=%d bySecond=%d  \n", byYear, byMonth, byDate, byHour, byMinute, bySecond);
	 printf("setSysTimeToRTC...rtc[7]=%d rtc[6]=%d rtc[5]=%d rtc[4]=%d rtc[3]=%d rtc[2]=%d rtc[1]=%d  rtc[0]=%d \n", rtc[7], rtc[6], rtc[5], rtc[4], rtc[3], rtc[2], rtc[1], rtc[0]);
*/
    //fprintf(stdout,"time save\n");
    //sleep(1);
    //time_delay(10000);

    if( write(i2c_rtc,rtc,8) == -1)
        fprintf(stdout,"write error\n");
    else
        fprintf(stdout,"RTC time saved\n");

    close(i2c_rtc);
    //sleep(1);
}

initRealTimeClock()
{
    int i2c_rtc;
    i2c_rtc = open("/dev/i2c-1",O_RDWR);
    if(i2c_rtc < 0)
    {
        fprintf(stdout,"i2c-1 open fail");
        exit(1);
    }

    //if(ioctl(i2c,I2C_SLAVE, 0x50)<0) //eeprom
    if(ioctl(i2c_rtc,I2C_SLAVE, 0x68)<0)
    {
        fprintf(stdout,"rtc dev addr set error \n");
        exit(1);
    }
    close(i2c_rtc);
}

void load_n_SaveTimeTask()
{
//      if (time_read_counter >= 150)
//      {
//              time_read_counter = 0;

    if ( getRtcTime(&sys_time, 0) )
    {
        fprintf(stdout," RTC error!");
        system_valid.realtime_clock = _SYS_INVALID_;
    }
    else
    {
        system_valid.realtime_clock = _SYS_VALID_;
#if 0
        fprintf(stdout,"read RTC\n");
        fprintf(stdout,"\n[%02d/%02d/%02d %02d:%02d:%02d] loop_count = %d\n",
                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                sys_time.wHour, sys_time.wMinute, sys_time.wSecond, loop_count);
#endif
    }


//              put_data_to_RTC(&sys_time);
//      }

}


//--- 웹 서버버로 데이터 확인을 윈함 ----------------
static void * sendSavingTrafficDatatoNand(void *arg)
{
    int stick, etick, i,j;
    unsigned int ridx, idx = 1;
    char name[256];
    char rtd_str[56];
    //FILE *fp;
    int fd_save;

    sprintf(name,"/root/am1808/savefolder/%04d%02d%02d/%02d%02d%02d.txt"
            , sys_time.wYear, sys_time.wMonth, sys_time.wDay, sys_time.wMonth, sys_time.wDay, sys_time.wHour);

    if((fd_save = open(name, O_WRONLY | O_CREAT | O_TRUNC, 0777))== -1)
    {
        fprintf(stdout,"file open error\n");
    }
    //else fprintf(stdout,"save realtime data[%d] : %06d, %06d\n", savendBank, read_nd_idx, read_nd_num);

    /*
    	if(!(fp = fopen(name, "w")))
    	{
    	fprintf(stdout,"file open error\n");
    	return FALSE;
    }
    	else fprintf(stdout,"save realtime data[%d] : %06d, %06d\n", savendBank, read_nd_idx, read_nd_num);
    */
    uint8 isEnd = 0;

    while(!isEnd)
    {
        ridx = read_nd_idx;
        memset(rtd_str, 0, 56);

		  //printf("avogadro sendSavingTrafficDatatoNand  gstSaveRTdataNAND[savendBank][ridx].inverse=%d\n", gstSaveRTdataNAND[savendBank][ridx].inverse);
        if(gstSaveRTdataNAND[savendBank][ridx].inverse)
        {
            sprintf(rtd_str, "%04d %04d/%02d/%02d %02d:%02d:%02d:%02d 차로:%02d 속도:-%05lu 점유시간1:%05lu 점유시간2:%05lu 길이:%05lu\n",
                    idx,
                    gstSaveRTdataNAND[savendBank][ridx].time.year+2000, gstSaveRTdataNAND[savendBank][ridx].time.month, gstSaveRTdataNAND[savendBank][ridx].time.day,
                    gstSaveRTdataNAND[savendBank][ridx].time.hour, gstSaveRTdataNAND[savendBank][ridx].time.min, gstSaveRTdataNAND[savendBank][ridx].time.sec,
                    (gstSaveRTdataNAND[savendBank][ridx].time.msec/10)%100,
                    gstSaveRTdataNAND[savendBank][ridx].lane+1, gstSaveRTdataNAND[savendBank][ridx].speed,
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].occupy[0],
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].occupy[1],
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].length);
        }

        else
        {
            sprintf(rtd_str, "%04d %04d/%02d/%02d %02d:%02d:%02d:%02d 차로:%02d 속도:%05lu 점유시간1:%05lu 점유시간2:%05lu 길이:%05lu\n",
                    idx,
                    gstSaveRTdataNAND[savendBank][ridx].time.year+2000, gstSaveRTdataNAND[savendBank][ridx].time.month, gstSaveRTdataNAND[savendBank][ridx].time.day,
                    gstSaveRTdataNAND[savendBank][ridx].time.hour, gstSaveRTdataNAND[savendBank][ridx].time.min, gstSaveRTdataNAND[savendBank][ridx].time.sec,
                    (gstSaveRTdataNAND[savendBank][ridx].time.msec/10)%100,
                    gstSaveRTdataNAND[savendBank][ridx].lane+1, gstSaveRTdataNAND[savendBank][ridx].speed,
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].occupy[0],
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].occupy[1],
                    (uint16) gstSaveRTdataNAND[savendBank][ridx].length);
        }

        write(fd_save, rtd_str, strlen(rtd_str));

        // data write.
        if ( -1 == fsync(fd_save))
            fprintf(stdout, "fsync() fail");

        /*
        fprintf(fp, "%04d  ", idx);
        fprintf(fp, "%04d/", gstSaveRTdataNAND[savendBank][ridx].time.year+2000);
        fprintf(fp, "%02d/", gstSaveRTdataNAND[savendBank][ridx].time.month);
        fprintf(fp, "%02d  ", gstSaveRTdataNAND[savendBank][ridx].time.day);
        fprintf(fp, "%02d:", gstSaveRTdataNAND[savendBank][ridx].time.hour);
        fprintf(fp, "%02d:", gstSaveRTdataNAND[savendBank][ridx].time.min);
        fprintf(fp, "%02d:", gstSaveRTdataNAND[savendBank][ridx].time.sec);
        fprintf(fp, "%02d  ", gstSaveRTdataNAND[savendBank][ridx].time.msec/10);

        fprintf(fp, "Lane :%02d  ", gstSaveRTdataNAND[savendBank][ridx].lane+1);

        if(gstSaveRTdataNAND[savendBank][ridx].inverse)
        	fprintf(fp, "Speed :-%05d  ", gstSaveRTdataNAND[savendBank][ridx].speed);
        else
        	fprintf(fp, "Speed : %05d  ", gstSaveRTdataNAND[savendBank][ridx].speed);

        fprintf(fp, "OCC1 :%05d  ", gstSaveRTdataNAND[savendBank][ridx].occupy[0]);
        fprintf(fp, "OCC2 :%05d  ", gstSaveRTdataNAND[savendBank][ridx].occupy[1]);
        fprintf(fp, "Length :%02d \r\n", gstSaveRTdataNAND[savendBank][ridx].length);
        */
        watchdog_counter = 0;

        idx++;
        ridx++;
        if (ridx >= MAX_SAVE_RT_DATA_NAND)
            ridx=0;

        read_nd_idx = ridx;

        read_nd_num--;
        if (!read_nd_num)
            isEnd = 1;

        usleep(150);
    }
    close(fd_save);
    //close(fp);

    if(term_stat !=REALTIME_MONITOR_MODE )
        fprintf(stdout,"realtime data saved: %06d\n", read_nd_idx);

    //return ( TRUE );
}

//by cap 2014.10.13
int LD_Send_val = 300;
int display_ldb_status = 0;
int preID = 0, curID = 0;
int onetime;

check_time()
{
    int num, tmp,rtn, tp, date, one=0, i, ret;
    char buf[50];
    char strTar[256], stryear[10], strmonth[10], strday[10];
    DIR *dp, *sdp;
    struct dirent *entry, *sentry;
    struct stat statbuf;
    mode_t mode;
    SYSTEMTIME ostime;

    if( sys_time.wSecond != osec)
    {
        if( System_Debug)
        {
            fprintf(stdout,"\n[%02d/%02d/%02d %02d:%02d:%02d] loop_count = %d, %d \n",
                    sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                    sys_time.wHour, sys_time.wMinute, sys_time.wSecond, loop_count);
        }

        // calculate msec. 1/100 sec
        tmp = loop_count;
        if( tmp > max_msec_count)
        {
            max_msec_count = tmp;
            //fprintf(stdout,"max_msec_count = %d\n", max_msec_count);
        }

        //fprintf(stdout,"loop_count = %ld, max = %ld, global_counter = %ld\n",
        //          loop_count, max_msec_count, global_counter);

        //periodic_timer_handler();  //by cap 2014.08.27

        //by cap 2014.10.13
        LD_Send_val++;
        display_ldb_status ++;
        //check_iocomm++;
        loop_count=0;
        osec = sys_time.wSecond;

        checkSystemStatus();    //by cap 2014.08.27
    }

    if(oday != sys_time.wDay)
    {
        //RTC and OsTime Synchronization
        getOsTime(&ostime);
        setSysTimeToRTC(&ostime);

        //create SaveFolder
        memset(strTar,0,256);
        sprintf(strTar,"/root/am1808/savefolder/%04d%02d%02d",sys_time.wYear,sys_time.wMonth,sys_time.wDay);

        // Permission Mask Clear. by wook 2015.11.24
        umask(0000);
        //mode = S_IRUSR | S_IWUSR | S_IXUSR;
        mode = 0777;

        mkdir(strTar, mode);

        //kiss insert start  --- 30초 데이타 저장 폴더
        /*char strTar30s[256];

        memset(strTar30s,0,256);
        sprintf(strTar30s,"/root/am1808/savefolder/%04d%02d%02d/data30s/",sys_time.wYear,sys_time.wMonth,sys_time.wDay);
        mkdir(strTar30s, mode);
*/
        //kiss insert end

        oday = sys_time.wDay;
        if((dp = opendir("/root/am1808/savefolder")) == NULL)
        {
            fprintf(stdout,"connot open dir!\n");
            return;
        }
        
        //20230207 특정 기간 지난 파일/디렉토리 삭제 추가 by avogadro start
        checkSavedDataDirectory("/root/am1808/savefolder", 8); // /root/am1808/savefolder 아래 8일 지난 디렉토리/파일 모두 삭제 
        //20230207 특정 기간 지난 파일/디렉토리 삭제 추가 by avogadro end 


        /* 20220712 버그로 파일삭제 안됨. 코멘트로 막고 main_proc의 main 루프에서 처리하는 것으로 변경 by avogadro
        chdir("/root/am1808/savefolder");

        while((entry = readdir(dp)) != NULL)
        {
            lstat (entry->d_name, &statbuf);
            if(S_ISDIR(statbuf.st_mode))
            {
                //fprintf(stdout,"DIR : %s\n", entry->d_name);
                if(strcmp(".", entry->d_name) == 0 || strcmp("..", entry->d_name) == 0 )
                    continue;
                memset(stryear,0,10);
                strncpy(stryear, entry->d_name, 4);
                memset(strmonth,0,10);
                strncpy(strmonth, entry->d_name+4, 2);
                memset(strday,0,10);
                strncpy(strday, entry->d_name+6, 2);

                if(atoi(strmonth)>sys_time.wMonth)
                {
                    switch(atoi(strmonth))
                    {
                    case 1:
                        date = sys_time.wDay +31 - atoi(strday);
                        break;
                    case 2:
                        date = sys_time.wDay +28- atoi(strday);
                        break;
                    case 3:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    case 4:
                        date = sys_time.wDay +30- atoi(strday);
                        break;
                    case 5:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    case 6:
                        date = sys_time.wDay +30- atoi(strday);
                        break;
                    case 7:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    case 8:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    case 9:
                        date = sys_time.wDay +30- atoi(strday);
                        break;
                    case 10:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    case 11:
                        date = sys_time.wDay +30- atoi(strday);
                        break;
                    case 12:
                        date = sys_time.wDay +31- atoi(strday);
                        break;
                    }

                }
                else
                {
                    date = sys_time.wDay - atoi(strday);
                }
					 //printf("data count = %d \n", date);
                if(date >= 8)  //kiss  1020-09-03 Nand 저장 삭제  3 --> 8 로 변경 (1주일 저장)
                {
                    memset(strTar,0,256);
                    sprintf(strTar,"./%s",entry->d_name);

                    if((sdp = opendir(strTar)) == NULL)
                    {
                        fprintf(stdout,"connot open dir!\n");
                        return;
                    }

                    chdir(strTar);

                    while((sentry = readdir(sdp)) != NULL)
                    {
                        if(strcmp(".", sentry->d_name) == 0 ||
                                strcmp("..", sentry->d_name) == 0 )
                            continue;

                        fprintf(stdout," %s\n", sentry->d_name);
                        remove(sentry->d_name);
                    }

                    chdir("..");
                    closedir(sdp);
                    rmdir(entry->d_name);
                }
            }
            else
            {
                fprintf(stdout,"File : %s", entry->d_name);
            }

            //memset(strTar,0,256);
            //sprintf(strTar,"./%s",entry->d_name);
            //chdir(strTar);

            //remove("141213.dat");
        }

        chdir("..");
        closedir(dp);
        */
		  sync_time_flag = 1;
		  //printf("avogadro sync_time_flag=%d", sync_time_flag);
    }

    //by capi 2014.08
    //if( sys_time.wHour != ohour)
    if(/* !(sys_time.wHour%3) &&*/ (ohour != sys_time.wHour))
    {
        pthread_t thd;

        if(kickRTSavingDatatoNand_m())
        {
            //sendSavingTrafficDatatoNand();

            ret = pthread_create(&thd, NULL, sendSavingTrafficDatatoNand, "create saving data task\n");
            if( ret != 0)
                fprintf(stdout,"create saving data task error\n");
        }
        else
        {
            fprintf(stdout,"RT Saving Buffer Empty!\n");
        }

        ohour = sys_time.wHour;
    }

    //if(check_iocomm>300) save_log(IOBOARD_COMM_ERROR, 0);

    if(display_ldb_status>15)
    {

        if(revLDB_ID[curID] == FALSE)
        {
#ifdef SEGMENT
            i2cSegDigit(1, 8);
            i2cSegDigit(0, curID+1);
#endif
        }

        display_ldb_status = 0;
        /*
        if(check_iocomm>300)
        {
        i2cSegDigit(1, 9);
        i2cSegDigit(0, 0);
        }
        */
    }

    /*2014.10.13 by capi - Ld Board Check*/
    /* 2017.5.2 by kwj - Update Ld Board Check */
    if(LD_Send_val > 2)
    {
        curID %=gucActiveLDBNum;

        if(curID >= gucActiveLDBNum)
            curID = 0;

        Req_LDB_ID(curID);

        revLDB_cnt[curID]++;

        //  if(curID) preID = curID -1;
        //  else preID = gucActiveLDBNum -1;

        revLDB_cnt[curID]++;

        //  if(term_stat !=REALTIME_MONITOR_MODE )
        //      fprintf(stdout,"Request Board ID : [%d] %d\n", curID, gucActiveLDBNum);


        if(revLDB_cnt[curID] > 4)   //If it record more than five times an recieve error
        {
#if 0
            i2cSegDigit(1, 8);
            i2cSegDigit(0, preID+1);
#endif
            if(revLDB_cnt[curID] == 5)      //only one time display
            {
                save_log(LDBOARD_COMM_ERROR, preID);
#ifdef LDB_PRINT
                if(term_stat !=REALTIME_MONITOR_MODE )
                    fprintf(stdout,"LD Board ID : [%d] non exist ERROR \n", curID);
#endif
            }
            // clear
            revLDB_ID[curID] = FALSE;
            revLDB_cnt[curID] = 0;
        }
        else
        {
#ifdef LDB_PRINT
            if(term_stat !=REALTIME_MONITOR_MODE )
                if(revLDB_ID[curID]==FALSE)
                    fprintf(stdout,"LD Board ID [%d] - Retry Count: %d \n", curID, revLDB_cnt[curID] );
#endif
        }

        curID++;

        LD_Send_val = 0;
    }
}
