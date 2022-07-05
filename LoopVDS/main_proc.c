/*********************************************************************************
/* Filename    : main_proc.c
 * Description : dcs9s main program
 * Author      : kwj
 * Notes       : initialize and polling
 *********************************************************************************/
/*********************************************************************************
/*************** Update Record ***************************************************
 *  1. 2017.07.19  kwj update and insert
 *     1) 프로그램 전반적으로 exit 처리를 함
 *        - 초기화시 exit는 수정하지 않고, 실행시 exit는 return으로 변경
 *     2) 프로그램이 종료되는 경우 log 파일로 저장
 *        - watchdog, server reset, server init, userterm reset,
 *          poll create, poll calloc, webserver reboot
**********************************************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/epoll.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/time.h>
#include <sys/stat.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <linux/i2c.h>
#include <linux/i2c-dev.h>
#include <linux/rtc.h>
#include <mtd/mtd-user.h>
#include <pthread.h>

#include <termios.h>
#include <signal.h>
#include <mtd/mtd-user.h>

#include "ftms_cpu.h"
#include "serial.h"
#include "tcpip.h"
#include "user_term.h"

//int i2c;

/** time variable */
//time_t t;
//struct tm *locp, loc, *tt;
//int osec;

//unsigned int loop_count=1000;
//int max_msec_count=100;


//int rt_fd; // realtime data file pointer

static void *watchdog_task(void *arg);
static void *network_check_task(void *arg);
static void *fifo_read_task(void *arg);

int System_Debug;
static uint16 cntNumRtData=1;
static uint8 maintenance_mode = FALSE;

int ledrun_fd, lederr_fd;
//int fan_fd, heater_fd;

static struct termios initial_settings, new_settings;

//int network_alive = FALSE;

led_off()
{
    write(ledrun_fd, "1", 1);
}

led_on()
{

    write(ledrun_fd, "0", 1);
}

// 100 millisec sleep
msec_sleep(int n)
{

    struct timespec request, remain;

    request.tv_sec = 0;
    request.tv_nsec = n * 1000000; // 100 msec * n

    nanosleep(&request, &remain);

}

static int sys_tick_count;
int oosec;
int bit;
//void periodic_timer_handler (int signum)

static int sys_tick_count;
void periodic_timer_handler (int signum)
{
    // 10ms Tick 임.
    global_counter+=10;
    if ( global_counter >= (3600000 /*/ _TICK_MS_*/) )
        global_counter = 0; //by capi 2015.12.09 delete / _TICK_MS_. DCS9S _TICK_MS_ = 10

    //if(!(global_counter%100)) fprintf(stdout,"\nGC %d\r\n", global_counter);

    watchdog_counter +=10;

    sys_tick_count += 10;

    time_read_counter +=10;
    monitor_counter +=10;
    saving_send_counter	+=10;
    //modem_ack_counter +=10;

    centerPollCounter +=10;
    centerAutosyncCounter +=10;

    //by capi
    localPollCounter +=10;
    localAutosyncCounter +=10;

    incident_counter +=10;

    sys_comm_counter +=10;
    io_time_counter +=10;
    validsession_counter +=10;				//2011.07.05 by capidra
    host_req_counter += 10;		//2014.09.25 by capi

    if(rt_web_flag == RTD_ON)
        rt_web_counter += 10;		// by wook for Web

    // by jwank 061112
    if (vds_comm_counter)
    {
        vds_comm_counter -=10;
        //if (!vds_comm_counter) zeroModemData();
    }

#if 1 //test
    if(!(global_counter%100)) 	//by capi 2014.08.27
    {
        //fprintf(stdout,"global_counterrrrrrrr = %d\n", global_counter);
        oosec = sys_time.wSecond;

        if( bit)
            led_on();
        else
            led_off();
        bit ^= 1;
    }
#endif

//	if (xmodem_wait) xmodem_wait--;

}

//by capi
init_periodic_timer()
{
    u_int i;

    struct sigaction sa;
    struct itimerval timer;

    // global counter init.
    global_counter = 0;
    sys_tick_count = 0;
    watchdog_counter = 0;

    time_read_counter = 0;
    monitor_counter = 0;
    saving_send_counter = 0;
    modem_ack_counter = 0;

    centerPollCounter = 0;
    centerAutosyncCounter = 0;

    //by capi
    localPollCounter = 0;
    localAutosyncCounter = 0;

    for (i=0; i<MAX_LOOP_NUM; i++)
        stuckon_counter[i] = 0;
    for (i=0; i<MAX_LOOP_NUM; i++)
        stuckoff_counter[i] = 0;
    pre_stuck_count = 0;
    incident_counter = 0;
    for (i=0; i<MAX_LOOP_NUM; i++)
        keepup_incident[i] = 0;

    sys_comm_counter = 0;
    io_time_counter = 0;
    validsession_counter = 0;				//2011.07.05 by capidra
    host_req_counter = 0;
    //xmodem_wait = 0;

    vds_comm_counter = 0;
    validsession_flag = 0;

    // by wook for web
    rt_web_flag = RTD_OFF;
    rt_web_counter = 0;

    ////////// initialize 10msec timer //////////////////////////////////////
    /* Install timer_handler as the signal handler for SIGVTALRM. */
    memset (&sa, 0, sizeof (sa));
    sa.sa_handler = &periodic_timer_handler;
    //sigaction (SIGVTALRM, &sa, NULL);
    sigaction (SIGALRM, &sa, NULL);

    /* Configure the timer to expire after 1 msec... */
    timer.it_value.tv_sec = 0;
    timer.it_value.tv_usec = 10000; //현재 타이머 값, 종료 후 it_interval.tv_usec 값으로 재설정

    /* ... and every 1 msec after that. */
    timer.it_interval.tv_sec = 0;
    timer.it_interval.tv_usec = 10000; // 현재 진행(감소)되는 타이머가 종료한후 다음 타이머 재설정 값

    /* Start a virtual timer. It counts down whenever this process is executing. */
    if(setitimer (ITIMER_REAL, &timer, NULL) == -1)
    {
        fprintf(stdout,"\r\n error!!!! \r\n");
    }
    /////////////////////////////////////////////////////////////////////////////
}



void printLogo(void)
{
    fprintf(stdout,"");
    fprintf(stdout,"  ######################################################\n");
    fprintf(stdout,"  ######                                          ######\n");
    fprintf(stdout,"  ######        VDS Application by LINUX          ######\n");
    fprintf(stdout,"  ######                                          ######\n");
    fprintf(stdout,"  ######               v2.8 --> 3.0               ######\n");
    fprintf(stdout,"  ######                                          ######\n");
    fprintf(stdout,"  ######        2014.07.07  Created By ITSLAB     ######\n");
    fprintf(stdout,"  ######        2017.07.17  Update                ######\n");
    fprintf(stdout,"  ######        2020.01.14  Last Update           ######\n");
    fprintf(stdout,"  ######        Compiled : %s %s   ######\n", __DATE__, __TIME__);
    fprintf(stdout,"  ######        2020.06.05 update by KISS          ######\n");
    fprintf(stdout,"  ######################################################\n");
    fprintf(stdout,"");
}

/*********************************************************************
 * initialize system parameter and variable                          *
 *********************************************************************/
init_system()
{
    uint8 config_dirty = FALSE, i;
    char setStr[255];

    //initialize gpio8[8,9,10,11] for power1,2 on/off check
    power1_fd = open("/sys/class/gpio/gpio137/value",O_RDONLY);		//2014.09.25 by capi
    power2_fd = open("/sys/class/gpio/gpio136/value",O_RDONLY);		//2014.09.25 by capi

    fprintf(stdout," Initialize System\n");
    if(!read_parameter_NAND())
    {
        fprintf(stdout,"Default Parameter......\n");

        // Magic Number를 넣어서 나중에 제대로 된 시스템 정보인지를 확인한다.
        gstSysConfig.magic_num = SYS_MAGIC_NUMBER;

        // 서버쪽으로의 통신 포트 설정치를 디플트로.
        gstSysConfig.comm_cfg.flow_cntrl = DEFAULT_FLOW_CNTRL;
        gstSysConfig.comm_cfg.bps = DEFAULT_BPS;
        gstSysConfig.comm_cfg.local_poll = LOCAL_POLLING_TIME;	// local polling time.

        gstSysConfig.temper[0] = DEFAULT_FAN_TEMP;
        gstSysConfig.temper[1] = DEFAULT_HT_TEMP;

        gstSysConfig.password[0] = 0;
        gstSysConfig.password[1] = 0;
        gstSysConfig.password[2] = 0;
        gstSysConfig.password[3] = 0;

        //------------------------------------------------------
        // by wook 2013.02.18 진단 프로그램용 변수 초기화
        //------------------------------------------------------
        dia_status.hw_link = 0;
        dia_status.svr_link = 0;
        dia_status.last_opc = 0;
        dia_status.temp[0] = 0;
        dia_status.temp[1] = 0;

        st_web_flag = 0;

        gstSysConfig.myaddr[0] = 0xFF;
        gstSysConfig.myaddr[1] = 0xFF;

        // 파라미터 값들을 디폴트값으로 설정.
        defaultVdsParameter();

        // 파라미터가 디폴트임을 표시함.
        gstSysConfig.is_default_param = _DEFAULT_PARAM_;

        // 설정이 바뀌었으면 저장.
        writeParamToNAND();
    }
    else
    {
        if (gstSysConfig.param_cfg.polling_cycle <= 0 \
                || gstSysConfig.param_cfg.polling_cycle > MAX_CENTER_POLL_TIME)	// center polling time.
        {
            config_dirty = TRUE;
            gstSysConfig.param_cfg.polling_cycle = CENTER_POLLING_TIME;
        }

        if (gstSysConfig.comm_cfg.local_poll <= 0 \
                || gstSysConfig.comm_cfg.local_poll > MAX_LOCAL_POLL_TIME)	// local polling time.
        {
            config_dirty = TRUE;
            gstSysConfig.comm_cfg.local_poll = LOCAL_POLLING_TIME;	// local polling time.
        }

        if(gstSysConfig.temper[0]>40 \
                ||gstSysConfig.temper[0]<20)
            gstSysConfig.temper[0] = DEFAULT_FAN_TEMP;

        if(gstSysConfig.temper[1]>10 \
                ||(gstSysConfig.temper[1]+10)<0)
            gstSysConfig.temper[1] = DEFAULT_HT_TEMP;

        if(gstSysConfig.password[0]>=10)
            gstSysConfig.password[0] = 0;
        if(gstSysConfig.password[1]>=10)
            gstSysConfig.password[1] = 0;
        if(gstSysConfig.password[2]>=10)
            gstSysConfig.password[2] = 0;
        if(gstSysConfig.password[3]>=10)
            gstSysConfig.password[3] = 0;

        // K factor
        for (i=0; i<2; i++)
            gstSysConfig.param_cfg.K_factor[i] = 12;
        // Threshold
        for (i=0; i<MAX_LOOP_NUM; i++)
            gstSysConfig.param_cfg.T_value[i] = 100;

#if defined(FAN_HEATER_AUTO_CONTROL)
        fprintf(stdout,"pass : %d %d %d %d\n", gstSysConfig.password[0], gstSysConfig.password[1],
                gstSysConfig.password[2], gstSysConfig.password[3]);
#else
        fprintf(stdout,"TTMS use DEFAUT PASSWORD\n");
#endif

        // 문제가 있으면.
        // 설정이 바뀌었으면 저장.
        if (config_dirty == TRUE)
        {
            //fprintf(stdout," Invalid Config.. ==> ");

            // 설정이 바뀌었으면 저장.
            //writeParamToNVRAM(TRUE);	// wwww
        }

    }
    // active loop 갯수를 가져온다.
    getActiveLoopNum();

    ///////////////////////////////////////////////////////////////////////////
    // RTC로 부터 시간을 가져온다.
    getRtcTime(&sys_time, 0);

#if 1 // by jwank 090608 대주 요구.
    boot_stamp = sys_time;
    fprintf(stdout, " [Booting Time]");
    fprintf(stdout, " %d-year %d-mon %d-day", boot_stamp.wYear, boot_stamp.wMonth, boot_stamp.wDay);
    fprintf(stdout, " %d-hour %d-min %d-sec", boot_stamp.wHour, boot_stamp.wMinute, boot_stamp.wSecond);
#endif
    ///////////////////////////////////////////////////////////////////////////

    fprintf(stdout, "\n [System Check] NAND Flash Memory(ROM) Check ...........256MB. OK.");
    fprintf(stdout, "\n [System Check] DRAM(RAM) Check ........................128MB. OK.");
    fprintf(stdout, "\n [System Check] WATCHDOG Timer Initailize Check .............. OK.");
    fprintf(stdout, "\n [System Check] Power Fail Check ............... [Long Power Fail]\n");

    //netMacStr2Bin(ETHERNET_ADDR_STRING, gstNetConfig.mac);
    //gstNetConfig.mac[5] = gucMacAddr;		// DIP 스위치 하위 바이트 하나를 MAC 어드레스의 최하위로 사용.

    // 네트웍 설정을 디폴트 값을 설정.
    //defaultNetworkConfig();  //delete by capi 2014.08.27

#if defined(ENABLE_WATCHDOG)
    touchWatchdog();
#endif

    if (readNetConfigFromNAND())
    {
        uint8 tmpD[] = {0xff, 0xff, 0xff, 0xff, 0xff, 0xff};

        fprintf(stdout," Check network config..............\n");

        // dddd
        if (!memcmp(gstNetConfig.mac, tmpD, MAC_ADDR_LEN))
        {
            fprintf(stdout, " Default MAC address\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.ip, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default IP address\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.gip, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default Gateway IP\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.nmsk, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default Subnet mask\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        setInterfaces();	//by capi interface file write
    }
    else
    {
        fprintf(stdout, " Default network config..............\n");

        defaultNetworkConfig();
        writeNetConfigToNAND(TRUE);
    }

    //by capi 2014.08
    //net setting
    /*
    memset (setStr, 0, 255);

    sprintf(setStr,"ifconfig eth0 %d.%d.%d.%d netmask %d.%d.%d.%d up"
    	, gstNetConfig.ip[0],gstNetConfig.ip[1],gstNetConfig.ip[2],gstNetConfig.ip[3]
    	, gstNetConfig.nmsk[0],gstNetConfig.nmsk[1],gstNetConfig.nmsk[2],gstNetConfig.nmsk[3]);

    fprintf(stdout,"\n %s", setStr);
    system(setStr);
    //gw IP setting
    memset (setStr, 0, 255);

    sprintf(setStr,"route add default gw %d.%d.%d.%d dev eth0"
    	, gstNetConfig.gip[0],gstNetConfig.gip[1],gstNetConfig.gip[2],gstNetConfig.gip[3]);

    fprintf(stdout,"\n %s", setStr);
    system(setStr);
    */

    makeMyIpString(gstNetConfig.ip, FALSE);
    makeServerIpString(gstNetConfig.server_ip, FALSE);

    //fprintf(stdout,"\nd\n");
    // 네트웍 설정을 dump 함.
    dumpNetworkConfig();

    //fprintf(stdout,"\ne\n");
    // ethernet init

    // serial init
}

unsigned char nor_buf[64];
mtd_info_t mtd_info;


void init_Nor_flash()
{
    int i;
    fprintf(stdout,"Initailize Nor Flash!!!\n");

    nor_fd = open("/dev/mtd0", O_RDWR);

    if(nor_fd < 0)
    {
        fprintf(stdout,"\n /dev/mtd0 nor flash open error!!");
        return;
    }

    ioctl(nor_fd, MEMGETINFO, &mtd_info);

    fprintf(stdout,"MTD type : %u\n", mtd_info.type);
    fprintf(stdout,"MTD total size : %u bytes\n", mtd_info.size);
    fprintf(stdout,"MTD erase size : %u bytes\n", mtd_info.erasesize);

    //erase_Nor_flash();
    //for(i=0;i<sizeof(nor_buf);i++) nor_buf[i] = i%256;
    //read_Nor_flash(5);

    //write_Nor_flash(5);

    //read_Nor_flash(5);

    //fprintf(stdout,"\n Current Nor Flash \n\n %s", nor_buf);

    //fprintf(stdout," Current Nor Flash \n\n");
    //for(i=0;i<sizeof(nor_buf);i++) fprintf(stdout,"/%02X",nor_buf[i]);
    //fprintf(stdout,"\n");


}

void read_Nor_flash_all()
{
    lseek(nor_fd, 0, SEEK_SET);
    read(nor_fd, nor_buf, sizeof(nor_buf));

}

void write_Nor_flash_all()
{
    lseek(nor_fd, 0, SEEK_SET);
    write(nor_fd, nor_buf, sizeof(nor_buf));
}

/*
void read_Nor_flash(unsigned int page)
{
	unsigned char c;
	lseek(nor_fd, page, SEEK_CUR);
	read(nor_fd, c, 1);

	fprintf(stdout," Current Nor Flash page %d : %d\n",page, c);
}

void write_Nor_flash(unsigned int page)
{
	unsigned char c = 0xFF;

	lseek(nor_fd, page, SEEK_CUR);
	write(nor_fd, c, 1);
}
*/

void erase_Nor_flash()
{
    erase_info_t erase_info;

    erase_info.start = 0;
    erase_info.length = mtd_info.erasesize-erase_info.start;
    //erase_info.length = 8192;

    ioctl(nor_fd,MEMERASE, &erase_info);

}

#define  BUFF_SIZE      1024

// 인터넷이 연결되어 있는지 여부를 반환
// TRUE : 인터넷과 연결되어 있음
int is_internet_connected( void)
{
    char    str[100], buff[BUFF_SIZE];
    int     is_connected    = FALSE;
    FILE   *fp_n;

	 //avogadro
	 is_connected = TRUE;
	 return is_connected;


    // 인터넷 연결 상태는 웹 주소에 PING을 때리는 결과를 확인
    // popen() 함수 호출 실패도 인터넷과 연결되지 않음으로 판단
    // -c 1 : 1회 수신, -w 1 : 1초 기다림
    //fp_n = popen( "ping 192.168.0.27 -c 1 -w 1 | grep \"1 packets received\" | wc -l", "r");

    //sprintf( str, "ping %d.%d.%d.%d -c 1 -w 1 | grep \"1 packets received\" | wc -l",
    sprintf( str, "ping %d.%d.%d.%d -c 1 -w 2 | grep \"1 packets received\" | wc -l",
             gstNetConfig.server_ip[0], gstNetConfig.server_ip[1], gstNetConfig.server_ip[2], gstNetConfig.server_ip[3]);
    fp_n = popen( str, "r");
    if ( NULL != fp_n)
    {
        while( fgets( buff, BUFF_SIZE, fp_n) )
        {
            //fprintf(stdout,"buf = %s\n", buff);
            if ( NULL != index( buff, '1'))     // 결과 값으로 1 이 있는지를 확인
            {
                is_connected    = TRUE;         // 있으면 TRUE
                break;
            }
        }

        pclose( fp_n);
    }
    return  is_connected;		// 검색 결과를 반환
}

// 인터넷이 연결되어 있는지 여부를 반환
// TRUE : 인터넷과 연결되어 있음
int is_serverport_ready( void)
{
    char str[100], buff[BUFF_SIZE];
    int is_ready = FALSE;
    FILE *fp_n;

    char *strcmd ="netcat_main";
    int num=1;

    //is_ready = netcat_main(num, strcmd);
#if 1
    // 서버 포트가 준비되었는가를 확인
    // netcattst를 호출하여 서버 포트가 ready상태인가를 확인
    // option : -n -v -z -w 1
    // fp_n = popen( "netcatcmd -n -v -z -w 1 192.168.0.27 30100 | grep \"open\" | wc -l", "r");

    sprintf( str, "netcatcmd -n -v -z -w 1 %d.%d.%d.%d %05d | grep \"open\" | wc -l",
             gstNetConfig.server_ip[0], gstNetConfig.server_ip[1], gstNetConfig.server_ip[2],
             gstNetConfig.server_ip[3], gstNetConfig.server_port);
    fp_n = popen( str, "r");
    if ( NULL != fp_n)
    {
        while( fgets( buff, BUFF_SIZE, fp_n) )
        {
            //fprintf(stdout,"buf = %s\n", buff);
            if ( NULL != index( buff, '1'))     // 결과 값으로 1 이 있는지를 확인
            {
                is_ready = TRUE;         // 있으면 TRUE
                break;
            }
        }
        pclose( fp_n);
    }
#endif

    return  is_ready;		// 검색 결과를 반환
}

// software reboot
watchdog_reset()
{
    watchdog_counter = 0;
}

forceResetwithWatchdog()
{
    while(1);
}

void parameter_update()
{
    if(!read_parameter_NAND())
    {
        fprintf(stdout,"Parameter Read fail!! \n");
    }
    else
    {
        if (gstSysConfig.param_cfg.polling_cycle <= 0 \
                || gstSysConfig.param_cfg.polling_cycle > MAX_CENTER_POLL_TIME)	// center polling time.
        {
            //config_dirty = TRUE;
            gstSysConfig.param_cfg.polling_cycle = CENTER_POLLING_TIME;
        }

        if (gstSysConfig.comm_cfg.local_poll <= 0 \
                || gstSysConfig.comm_cfg.local_poll > MAX_LOCAL_POLL_TIME)	// local polling time.
        {
            //config_dirty = TRUE;
            gstSysConfig.comm_cfg.local_poll = LOCAL_POLLING_TIME;	// local polling time.
        }

        if(gstSysConfig.temper[0]>40 \
                ||gstSysConfig.temper[0]<20)
            gstSysConfig.temper[0] = DEFAULT_FAN_TEMP;

        if(gstSysConfig.temper[1]>10 \
                ||(gstSysConfig.temper[1]+10)<0)
            gstSysConfig.temper[1] = DEFAULT_HT_TEMP;

        if(gstSysConfig.password[0]>=10)
            gstSysConfig.password[0] = 0;
        if(gstSysConfig.password[1]>=10)
            gstSysConfig.password[1] = 0;
        if(gstSysConfig.password[2]>=10)
            gstSysConfig.password[2] = 0;
        if(gstSysConfig.password[3]>=10)
            gstSysConfig.password[3] = 0;

#if defined(FAN_HEATER_AUTO_CONTROL)
        fprintf(stdout,"pass : %d %d %d %d\n", gstSysConfig.password[0], gstSysConfig.password[1],
                gstSysConfig.password[2], gstSysConfig.password[3]);
#else
        fprintf(stdout,"TTMS use DEFAUT PASSWORD\n");
#endif

    }
    // active loop 갯수를 가져온다.
    getActiveLoopNum();
}

void Netconfig_update()
{
    char setStr[255];

    if (readNetConfigFromNAND())
    {
        uint8 tmpD[] = {0xff, 0xff, 0xff, 0xff, 0xff, 0xff};

        fprintf(stdout," Check network config..............\n");

        // dddd
        if (!memcmp(gstNetConfig.mac, tmpD, MAC_ADDR_LEN))
        {
            fprintf(stdout, " Default MAC address\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.ip, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default IP address\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.gip, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default Gateway IP\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }

        if (!memcmp(gstNetConfig.nmsk, tmpD, IP_NUM_LEN))
        {
            fprintf(stdout, " Default Subnet mask\n");
            //gstNetConfig.mac

            //for (i=0; i<MAC_ADDR_LEN; ii+
        }
    }


    //by capi 2014.08
    //net setting
    setInterfaces();	//by capi interface file write and restart
    /*
    memset (setStr, 0, 255);

    sprintf(setStr,"ifconfig eth0 %d.%d.%d.%d netmask %d.%d.%d.%d up"
    	, gstNetConfig.ip[0],gstNetConfig.ip[1],gstNetConfig.ip[2],gstNetConfig.ip[3]
    	, gstNetConfig.nmsk[0],gstNetConfig.nmsk[1],gstNetConfig.nmsk[2],gstNetConfig.nmsk[3]);

    fprintf(stdout,"\n %s", setStr);
    system(setStr);
    //gw IP setting
    memset (setStr, 0, 255);

    sprintf(setStr,"route add default gw %d.%d.%d.%d dev eth0"
    	, gstNetConfig.gip[0],gstNetConfig.gip[1],gstNetConfig.gip[2],gstNetConfig.gip[3]);

    fprintf(stdout,"\n %s", setStr);
    system(setStr);
    */

    makeMyIpString(gstNetConfig.ip, FALSE);
    makeServerIpString(gstNetConfig.server_ip, FALSE);

    //fprintf(stdout,"\nd\n");
    // 네트웍 설정을 dump 함.
    dumpNetworkConfig();

    //fprintf(stdout,"\ne\n");
    // ethernet init

    // serial init
}

//It processes the signals from monitoring_proc
void sighandler(int signo, siginfo_t *si, void *uarg)
{
    if(si->si_code == SI_QUEUE)
    {
        //fprintf(stdout," pid : %d \n",si->si_pid);
        //fprintf(stdout," sig num : %d \n",si->si_signo);
        //fprintf(stdout," user num : %d \n",si->si_value.sival_int);

        switch(si->si_value.sival_int)
        {
        case 101 :
            fprintf(stdout,"***** Parameter Upload ***** \n");
            parameter_update();
            save_log(PARAMETER_UPDATE, 0);
            break;

        case 102 :
            save_log(NETCONFIG_UPDATE, 0);
            fprintf(stdout,"***** Netconfig Upload ***** \n");
            Netconfig_update();

            break;
        }
    }
}

//It processes the signals from monitoring_proc
int Init_monitoring_proc_signal()
{
    struct sigaction act;

    //act.sa_handler = sighandler;

    //sigemptyset (&act.sa_mask);
    act.sa_flags	= SA_SIGINFO;
    sigemptyset (&act.sa_mask);
    //act.sa_restorer	= NULL;
    act.sa_sigaction = sighandler;

    if(sigaction(SIGUSR2, &act, 0) == 1)
    {
        fprintf(stdout," Signal Error \n");
        return 0;
    }

    return 1;
}

#define USER_TERM_PROMPT_STRING         (" VDS>")

#define MIN_LOOP_COUNT	300
//extern int network_alive;

int watchdog_flag=0;

int main(int argc, char *argv[])
{
    char buf[100];
    int ret;
    pthread_t t1,t2,t3;

    loop_count = 1000;
    max_msec_count = 100;

    init_console(); // debug port
    fprintf(stdout,"Start Application\n");

    printLogo();

    //init_watchdog();
    //avogadro initRealTimeClock();

    //RTC and OS Time Synchlonization
    if ( getRtcTime(&sys_time, 1) )
        fprintf(stdout,"****** Real Time Clock is [Fail]\n");
    else
        setOsTime(&sys_time);

    init_system();
#ifdef SEGMENT
    i2cSegInit();
#endif
    initADC();   //by capi 2014.08.27
    init_keyboard();
    //checkPowerFail();
    initSysValidGlobalVal();
    initVdsGlobalVal();

    init_InternalUART(); // io board, ttyS5, 115200, no parity
    init_ExternalUART_1(); // external port, ttyS1, 115200, no parity
    init_UART_485();		//by capi 2014.10.08

    initRealtimePool();
    initPollingPool();
    initRealTimeVdsBuff();	// for vdsapp

    initModemPortVDS();
    initModemPortA(); // PC maintenance protocol

    init_Nor_flash(); // initailize Nor Flash

    // epoll init, add file desc( ethernet, ttyS2, ttyS1...)
    init_epoll();

    //fprintf(stdout,"net state : %d \n", vds_st.state);

    // init MMI SPI
    //avogadro initPseudoSPI();

    //avogadro init_periodic_timer();

    watchdog_reset();

	 //avogadro 
    /****************** create watchdog thread *******************/
    /*ret = pthread_create(&t1, NULL, watchdog_task, (void *)(&watchdog_counter) );
    if( ret != 0)
        fprintf(stdout,"create watchdog\n");*/
    /************************************************************************/
	 //avogadro

    /****************** create network check thread *******************/
    ret = pthread_create(&t2, NULL, network_check_task, "create network check task\n");
    if( ret != 0)
        fprintf(stdout,"create network\n");
    /************************************************************************/
    /****************** create fifo check thread for Web *******************/
    ret = pthread_create(&t3, NULL, fifo_read_task, "create fifo read task\n");
    if( ret != 0)
        fprintf(stdout, "create fifo\n");
    /************************************************************************/

    /***************** LED On/Off **************************************/
	 //avogadro ledrun_fd = open("/sys/class/gpio/gpio6/value", O_WRONLY, S_IWRITE);
	 //avogadro lederr_fd = open("/sys/class/gpio/gpio0/value", O_WRONLY, S_IWRITE);

	 //avogadro write(lederr_fd, "1", 1);

    /***************** FAN HEATER On/Off **************************************/
	 //avogadro fan_fd 	  = open("/sys/class/gpio/gpio106/value", O_WRONLY, S_IWRITE);
	 //avogadro heater_fd = open("/sys/class/gpio/gpio107/value", O_WRONLY, S_IWRITE);

	 //avogadro write(fan_fd, "0", 1);
	 //avogadro write(heater_fd, "0", 1);

    //sleep(1000);

    //write(fan_fd, "1", 1);
    //write(heater_fd, "1", 1);

    Init_monitoring_proc_signal();

	 //avogadro save_log(START_PROC, 0);

    // by wook for Web
	 //avogadro open_textfile_web();
	 //avogadro open_fifo_web();

#ifdef SEGMENT
    //i2cSegDigit(0,0);
    //i2cSegDigit(1,0);
#endif
    while(1)
    {
		 
        communication_task();

        /*
        if( sys_time.wYear > 2017 && sys_time.wMonth >= 3)
        {
            //fprintf(stdout,"Time Passed Error =======================> \n");
        }
        else
        {
            io_processtask();

            make_traffictask();
        }
        */

        io_processtask();

        make_traffictask();

        //kiss - 200525
        InverseDataCheckAndSend();

        autoSyncTask();

        //checkIncidentTask();		//delete by capi 2016.02.24

        savestucktime();
        // loop_count max : 30000


		  //avogadro
        /*if((loop_count % 4000) == 0)
            mmicomtask();*/
		  //avogadro

        if(isReqForceReset == 0x89)
        {
            fprintf(stdout," ######### Force Reset................................. !!!");

            save_log(SERVER_COMMAND_RESET,0); // kwj insert. 2017.07.19

            forceResetwithWatchdog();
        }

        mainteProcessTask();

        userTermTask();

        //one hour data send
        //avogadro sendSavingTrafficData_m();

        // loop_count : 20000 ~ 30000, MAX_LOOP_COUNT = 2000
        /*avogadro if( (loop_count % MAX_LOOP_COUNT) == 0)
            load_n_SaveTimeTask();*/

#if defined(TEMPERATURE_MONITORING) || defined(POWER_VOLTAGE_MONITORING)
        // avogadro checkTemp_n_PowerTask();
#endif

        //avogadro check_time();

        //avogadro watchdog_reset();

        // main loop count/sec
        // 130000 ~ 150000
        loop_count++;

        //avogadro chk_rtdata_web();

        //avogadro set_rtc_web();
    }
    close(rt_web_fd);
    unlink(RTD_WEB_FIFO_PATH);
}

/*******************************************************************************/
/*                      watchdog task          			                       */
/*******************************************************************************/
static void *watchdog_task(void *cnt)
{
    int *wdcnt;

    wdcnt = (int *)cnt;

    while(1)
    {
        if((*wdcnt) > 2000)
        {
            //reset
            fprintf(stdout,"WatchDog Reset !!!!!! cnt : %d, watchdog_flag = %d \n", *wdcnt, watchdog_flag);

            save_log(WATCHDOG_RESET, 0); // kwj insert. 2017.07.19

            //system("reboot");
            exit(EXIT_FAILURE);
        }

        //fprintf(stdout,"WatchDog Reset count!!!!!! cnt : %d \n", *wdcnt);
        sleep(5);
    }
}

/********************************************************************************/
/*                      network check task                               		*/
/********************************************************************************/
int print_disconnect_msg = 0;
int print_connect_msg = 0;
int print_connected_msg = 0;
int retry_count = 0;
int process_check_retry = 0;
int timeout_check_retry = 0;

static void *network_check_task(void *arg)
{
    char *s=(char *)arg;
    int rtn, ldb,i;

    while(1)
    {
        // after respond, passed 5  minute
        if(validsession_counter > 300000 && validsession_flag == 0)
        {
            // if send CSN
            if(vds_st.state == SM_VDS_CLIENT_DEVID_SEND)		//2014.03.26 insert by capi
            {
                if(term_stat !=REALTIME_MONITOR_MODE )
                    fprintf(stdout,"Valid Session: %d , %d \n",validsession_counter, validsession_flag);

                if( is_internet_connected()==TRUE)
                {
                    checkvalidTask();	//유효성 확인
                    validsession_flag = 1;
                }
                else
                {
                    del_epoll_socket();
                    validsession_counter = 0;
                    validsession_flag = 0;
                    vds_st.state = SM_VDS_CLIENT_ABORTED;
                    save_log(ABORT_NET_NOPING, 0);

                    sleep(2);
                }
            }
            else
            {
                //fprintf(stdout,"else\n");
                sleep(20);
            }
        }
        else if(validsession_counter>320000 && validsession_flag ==1)
        {

            if(term_stat !=REALTIME_MONITOR_MODE )
                fprintf(stdout,"\n\r validsession-timeout [t: %d] !! \n\r", getSystemTick());

            del_epoll_socket();

            validsession_counter = 0;
            validsession_flag = 0;
            vds_st.state = SM_VDS_CLIENT_ABORTED;

            save_log(ABORT_NET_VALIDSETION, 0);

            sleep(2);
        }
        else if(vds_st.state == SM_VDS_CLIENT_CONNECTED && validsession_counter > 10000)
        {
            // Connect and wait CSN
#ifdef SEGMENT
            i2cSegDigit(1,1);
            i2cSegDigit(0,2);
#endif
            save_log(ABORT_NET_NOCSN, 0);

            if(term_stat !=REALTIME_MONITOR_MODE)
                fprintf(stdout,"\n\r No Recieved CSN.  Disconnect!!\n\r");
            del_epoll_socket();
            vds_st.state = SM_VDS_CLIENT_ABORTED;
            validsession_counter = 0;
            validsession_flag = 0;
            sleep(5);
        }
        //else if (vds_st.state == SM_VDS_CLIENT_CONNECTED && validsession_counter <= 10000)
        //{
        //fprintf(stdout,"waiting connect\n");
        //sleep(2);
        //}
        else if(vds_st.state >= SM_VDS_CLIENT_ABORTED)
        {
            validsession_counter = 0;

#ifdef SEGMENT
            //i2cSegDigit(1,1);
            //i2cSegDigit(0,8);
#endif
            rtn = is_internet_connected();

            if(rtn == TRUE)
            {
#ifdef SEGMENT
                i2cSegDigit(1,1);
                i2cSegDigit(0,3);
#endif
                vds_st.state = SM_VDS_CLIENT_TRY_CONNECT;
                init_tcpip();

					 init_yse_procotol();
					 
                if(vds_st.state == SM_VDS_CLIENT_CONNECTED)
                {
                    validsession_flag = 0;
                    add_epoll_socket();
                }

                validsession_counter = 0;		//2016.01.21 by capi
                print_disconnect_msg = 0;
                print_connect_msg = 0;
                timeout_check_retry=0;
                process_check_retry++;

                if(process_check_retry < 5)
                {
                    if(term_stat !=REALTIME_MONITOR_MODE)
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network is connecting(10)...............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

                    sleep(10);
                }
                else if(process_check_retry < 20)
                {
                    if(term_stat !=REALTIME_MONITOR_MODE)
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network is connecting(30)...............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
                    sleep(30);
                }
                else
                {
                    if(term_stat !=REALTIME_MONITOR_MODE)
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network is connecting(250)...............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
                    sleep(250);
                }
            }
            else
            {
                vds_st.state = SM_VDS_CLIENT_ABORTED;		//2016.01.21 by capi

#ifdef SEGMENT
                i2cSegDigit(1,1);
                i2cSegDigit(0,7);
#endif
                save_log(ABORT_NET_NOPING, 0);

                process_check_retry = 0;

                validsession_counter = 0;					//2016.01.21 by capi

                timeout_check_retry++;
                if(timeout_check_retry < 5)
                {
                    if(term_stat !=REALTIME_MONITOR_MODE )
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network was disconnected(10)..............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
                    sleep(10);
                }
                else if(timeout_check_retry < 20)
                {
                    if(term_stat !=REALTIME_MONITOR_MODE )
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network was disconnected(30)..............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
                    sleep(30);
                }
                else
                {
                    if(term_stat !=REALTIME_MONITOR_MODE )
                        fprintf(stdout, "[%04d/%02d/%02d %02d:%02d:%02d] Network was disconnected(250)..............\n",
                                sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                                sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
                    sleep(250);
                }

            }
        }
        else if( vds_st.state == SM_VDS_CLIENT_DEVID_SEND)  // normal running
        {
            if( print_connected_msg == 0)
            {
                if(term_stat !=REALTIME_MONITOR_MODE )
                    fprintf(stdout, "\n[%04d/%02d/%02d %02d:%02d:%02d] Network connected...............\n",
                            sys_time.wYear, sys_time.wMonth, sys_time.wDay,
                            sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
            }
            print_connected_msg = 1;
            process_check_retry =0;
            timeout_check_retry=0;

            for(i =0; i<gucActiveLDBNum; i++)
            {
                if(revLDB_ID[i] == FALSE)
                {
                    ldb = 1;
                    break;
                }
                ldb = 0;
            }

            if(ldb == 0)
            {
#ifdef SEGMENT
                i2cSegDigit(1,0);
                i2cSegDigit(0,0);
#endif
            }

            sleep(30);
        }
        else
        {
            sleep(20);
        }

    }

}

/*******************************************************************************/
/*          [WebServer] fifo read task by wook 2016.01.26                      */
/*******************************************************************************/
static void *fifo_read_task(void *arg)
{
    char *s=(char *)arg;

    char fmsg[FIFO_MSG_SIZE];
    int firead;

    int msg_size;

    while(1)
    {
        if((firead = read(fifo_web_fd, fmsg, FIFO_MSG_SIZE)) < 0 )
        {
            fprintf(stdout, "[Web Server] Fail to call read(%d) : %s\n", errno, strerror(errno));
            exit(1);
        }

        msg_size = strlen(fmsg);

        //if (msize>0) fprintf(stdout, "recv: %s\n", fmsg);
        if(msg_size>0)
            fifo_process_web(fmsg);
    }
}
