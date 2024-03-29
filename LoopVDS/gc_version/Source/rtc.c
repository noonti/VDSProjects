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

void getOsTime(SYSTEMTIME *lpSysTime)
{
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
