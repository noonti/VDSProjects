/*********************************************************************************
/* Filename    : serial.c
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
#include <sys/socket.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>

#include <sys/types.h>
#include <sys/stat.h>
#include <termios.h>
#include <sys/un.h>

#include "systypes.h"
#include "ftms_cpu.h"
#include "tcpip.h"
#include "serial.h"
#include "user_term.h"

#define BAUDRATE 	B115200
#define FALSE 		0
#define TRUE 		1

//#define DEBUG_SYS_COMM

#define DLE		0x10
#define STX		0x02
#define ETX		0x03

#define MODEMDEVICE "/dev/ttyS5" // IO Board Comm.
#define MODEMDEVICE_1 "/dev/ttyS1" // external port
#define LDBOARDDEVICE "/dev/ttyS6" // LD Board Comm.

//extern loop_count;
//extern max_msec_count;
//extern term_stat;

MODEM_DATA	recvDataPortVDS;
uint8		smRecvVDS;
uint16		chksumPortVDS;
uint16		dataLenPortVDS;

MODEM_DATA	recvDataPortSub;
uint8		smRecvSub=1;
uint16		chksumPortSub;
uint16		dataLenPortSub;

MODEM_DATA	recvDataPortLdb;
uint8		smRecvLdb=1;
uint16		chksumPortLdb;
uint16		dataLenPortLdb;

MODEM_DATA	recvDataPortA;
uint8		smRecvPortA;
uint16		chksumPortA;
uint16		dataLenPortA;

uint8 Recv1[MAX_MODEM_DATA_QUEUE_NO];
uint8 Recv2[MAX_MODEM_DATA_QUEUE_NO];

volatile int STOP = FALSE;
int fd_tty;
int fd_ttys1, fd_ttys6;

uint8 polling_data_valid = FALSE;

void init_console(void)
{
    fprintf(stdout," Initialize Console\n");
}

void initModemPortA()
{
    int i;
    chksumPortA = 0;
    recvDataPortA.Start = 0;
    recvDataPortA.End = 0;
    dataLenPortA = 0;
    smRecvPortA = MODEM_WAIT_START_1;

    for(i=0; i<MAX_MODEM_DATA_QUEUE_NO; i++)
        recvDataPortA.Length[i] = 0;
}

void initModemPortVDS()
{
    int i;
    chksumPortVDS = 0;
    recvDataPortVDS.Start = 0;
    recvDataPortVDS.End = 0;
    dataLenPortVDS = 0;
    smRecvVDS = MODEM_WAIT_START_1;

    for(i=0; i<MAX_MODEM_DATA_QUEUE_NO; i++)
        recvDataPortVDS.Length[i] = 0;
}
static void initEventBuff()
{
    evt_buff.rd_ptr = 0;
    evt_buff.wr_ptr = 0;
}

void initVdsGlobalVal ()
{
    uint32 i, j, bit_msk;
    uint8 s_loop, e_loop;

    for (i=0; i<MAX_LANE_NUM; i++)
    {
        lane_volume[i] = 0;
        acc_speed[i] = 0;
        occupy[0] = 0;
        invers_st.occ_state_with_inv[i] = 0; //by capi 2014.08
    }

    for(i=0; i<MAX_LOOP_NUM; i++)
    {
        loop_st[i].time[PRESS_TIME] = INVALID_TIME;
        loop_st[i].time[REL_TIME] = INVALID_TIME;
        loop_st[i].status = RELEASED;
    }

    for (i=0; i<MAX_LANE_NUM; i++)
    {
        gstCurrData.speed[i] = 0;
        gstCurrData.length[i] = 0;

        gstCenterAccuData.speed[i] = 0;
        gstCenterAccuData.length[i] = 0;

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
        gstLocalAccuData.speed[i] = 0;
        gstLocalAccuData.length[i] = 0;
#endif
    }

    for (i=0; i<MAX_LOOP_NUM; i++)
    {
        gstCenterAccuData.volume[i] = 0;
        gstCenterAccuData.occupy[i] = 0;

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
        gstLocalAccuData.speed[i] = 0;
        gstLocalAccuData.length[i] = 0;
#endif
    }

    for (i=0; i<MAX_LOOP_NUM; i++)
        gstReportsOfLoop[i].volume = 0;

    for (i=0; i<MAX_LANE_NUM; i++)
    {
        for (j=0; j<SPEED_CATEGORY_NO; j++)
            gstReportsOfLane[i].speed_category[j] = 0;

        for (j=0; j<LENGTH_CATEGORY_NO; j++)
            gstReportsOfLane[i].speed_category[j] = 0;
    }

    for (i=0; i<MAX_LANE_NUM; i++)
    {
        lane_evt_flg[i] = NO_EVENT;
        invers_st.gRT_Reverse[i] = FALSE;
        invers_st.gRT_Reverse_retrans[i] = FALSE;		//by capi for inverse driving
    }

    for (i=0; i<MAX_RIO_LANE_NUM; i++)
        realtime_data_valid[i] = NO_EVENT;


#if 0 // for test
    for (i=0; i<MAX_LANE_NUM; i++)
    {
        gstLaneLink[i].s_loop = i*2;
        gstLaneLink[i].e_loop = i*2+1;

        lane_st[i].time[START_POS] = INVALID_TIME;
        lane_st[i].time[END_POS] = INVALID_TIME;
        lane_st[i].status= 0;
    }

    for (i=0; i<MAX_LOOP_NUM/2; i++)
    {
        lp2ln[i*2].lane = i;
        lp2ln[i*2].pos = START_POS;
        lp2ln[i*2].used = USED;

        lp2ln[i*2+1].lane = i;
        lp2ln[i*2+1].pos = END_POS;
        lp2ln[i*2+1].used = USED;
    }

    for (i=0; i<MAX_LANE_NUM; i++)
        loop_diam[i] = 18;
    for (i=0; i<MAX_LANE_NUM; i++)
        lane_dist[i] = 45;
#else

    loop_enable_msk = (uint32) (gstSysConfig.param_cfg.loop_enable_table[0])
                      + (uint32) (gstSysConfig.param_cfg.loop_enable_table[1] <<8 )
                      + (uint32) (gstSysConfig.param_cfg.loop_enable_table[2] << 16 )
                      + (uint32) (gstSysConfig.param_cfg.loop_enable_table[3] << 24 );

    fprintf(stdout,"loop_enable_msk = %08x\n", loop_enable_msk);
    loop_enable_msk &= REAL_IO_POINT_MSK;

    for (i=0; i<MAX_LANE_NUM; i++)
    {
#if 0 // for reverse test eeee
        gstLaneLink[i].s_loop = gstSysConfig.param_cfg.speed_loop_map[i][END_POS];
        gstLaneLink[i].e_loop = gstSysConfig.param_cfg.speed_loop_map[i][START_POS];
#else
        gstLaneLink[i].s_loop = gstSysConfig.param_cfg.speed_loop_map[i][START_POS];
        gstLaneLink[i].e_loop = gstSysConfig.param_cfg.speed_loop_map[i][END_POS];
#endif

        lane_st[i].time[START_POS] = INVALID_TIME;
        lane_st[i].time[END_POS] = INVALID_TIME;
        lane_st[i].intime[START_POS] = INVALID_TIME;		//2015.07.14 by capi, for inverse driving
        lane_st[i].intime[END_POS] = INVALID_TIME;			//2015.07.14 by capi, for inverse driving
        lane_st[i].status= 0;
    }

    for (i=0; i<MAX_LANE_NUM; i++)
    {
        s_loop = gstSysConfig.param_cfg.speed_loop_map[i][START_POS];
        e_loop = gstSysConfig.param_cfg.speed_loop_map[i][END_POS];

        if ( s_loop < MAX_LOOP_NUM && e_loop < MAX_LOOP_NUM )
        {
#if 0 // for reverse test eeee
            lp2ln[s_loop].lane = i;
            lp2ln[s_loop].pos = END_POS;
            lp2ln[s_loop].used = USED;

            lp2ln[e_loop].lane = i;
            lp2ln[e_loop].pos = START_POS;
            lp2ln[e_loop].used = USED;
#else
            lp2ln[s_loop].lane = i;
            lp2ln[s_loop].pos = START_POS;
            lp2ln[s_loop].used = USED;

            lp2ln[e_loop].lane = i;
            lp2ln[e_loop].pos = END_POS;
            lp2ln[e_loop].used = USED;
#endif
        }
    }

    fprintf(stdout," Unused Loops [ ", i);
    for (i=0, bit_msk=1; i<MAX_LOOP_NUM; i++, bit_msk<<=1)
    {
        if ( !(loop_enable_msk & bit_msk) )
        {
            lp2ln[i].used = UNUSED;
            fprintf(stdout,"%d ", i);
        }
    }
    fprintf(stdout,"\n");

    for (i=0; i<MAX_LANE_NUM; i++)
        loop_diam[i] = gstSysConfig.param_cfg.spd_loop_diam[i];

    for (i=0; i<MAX_LANE_NUM; i++)
        lane_dist[i] = gstSysConfig.param_cfg.spd_loop_dist[i];
#endif

    initEventBuff();

    gCountVehicle = 0;
    //invers_st.start_inv_flag = FALSE;	//by capi 2015.01.07 for inverse Driving
	 invers_st.start_inv_flag = TRUE;	//by capi 2020.11.07 by avogadro
    invers_st.inverse_time = 0;
    invers_st.inverse_flag = 0;
    //real_monitoring_mode = FALSE;
}

//io board
init_UART_485(void)
{
    struct termios new_tio;
    int modemctlline;
    int baud=115200;

    //fd_tty = open(path, O_RDWR | O_NOCTTY | O_NDELAY);
    fd_ttys6 = open( LDBOARDDEVICE, O_RDWR | O_NOCTTY | O_NDELAY | O_NONBLOCK);
    if (fd_ttys6 == -1)
        return -errno;

    memset(&new_tio, 0, sizeof(struct termios));

    new_tio.c_cflag = baud | CS8 | CREAD | CLOCAL;
    //new_tio.c_cflag = ~CRTSCTS | baud | CS8 | CREAD | CLOCAL;

    new_tio.c_cflag = BAUDRATE | CS8 | CLOCAL | CREAD;

    new_tio.c_iflag = IGNPAR;

    new_tio.c_oflag = 0;

    new_tio.c_lflag = 0;
    new_tio.c_cc[VTIME] = 0;
    new_tio.c_cc[VMIN] = 0;

    tcflush(fd_ttys6, TCIFLUSH);
    tcsetattr(fd_ttys6, TCSANOW, &new_tio);

    /*  struct termios arg;
      unsigned char flag = 0x00;
      //int temp, temp2;

      if((fd_ttys6 = open(LDBOARDDEVICE, O_RDWR | O_NOCTTY | O_NONBLOCK)) < 0) return(-1);

    fcntl(fd_ttys6, F_SETFL, 0);		//

    //------ Communication Setting
    tcgetattr(fd_ttys6, &arg);

    // Input, Output BaudRate Setting
    cfsetispeed(&arg, B115200);
    cfsetospeed(&arg, B115200);

    arg.c_iflag &= ~(IXOFF | IXON |ICRNL);

    //arg.c_iflag &= ~(BRKINT|IGNBRK|ICRNL|IGNPAR|PARMRK|INPCK|INLCR|IGNCR|IUCLC|IXANY|IMAXBEL|IUTF8);

    // Receiver Enable
    arg.c_cflag |= (CLOCAL | CREAD);	//Enable receiver, Ignore modem control lines

    // None Parity, 1stop bit, 8bits
    arg.c_cflag &=  ~(CSIZE | PARENB |CSTOPB);
    arg.c_cflag |= CS8;

    //no flow control
    arg.c_cflag &= ~CRTSCTS;
    //arg.c_cflag = 0;

    arg.c_lflag &= ~(ICANON | ECHO | ECHOE | ISIG);

    arg.c_cc[VTIME] = 10;	//set time out 1 second
      arg.c_cc[VMIN] = 0;

    arg.c_oflag &= ~OPOST;
    //ioctl(fd_ttys6, 0x54F0, 0x0001);

    //tcflush(fd_ttys6, TCIFLUSH);

    tcsetattr(fd_ttys6, TCSANOW, &arg);
    */
}

//io board
init_InternalUART(void)
{
    struct termios new_tio;
    int modemctlline;
    int baud=115200;

    //fd_tty = open(path, O_RDWR | O_NOCTTY | O_NDELAY);
    fd_tty = open( MODEMDEVICE, O_RDWR | O_NOCTTY | O_NDELAY | O_NONBLOCK);
    if (fd_tty == -1)
        return -errno;

    memset(&new_tio, 0, sizeof(struct termios));

    new_tio.c_cflag = baud | CS8 | CREAD | CLOCAL;
    //new_tio.c_cflag = ~CRTSCTS | baud | CS8 | CREAD | CLOCAL;

    new_tio.c_cflag = BAUDRATE | CS8 | CLOCAL | CREAD;

    /*
    IGNPAR : ignore bytes with parity errors
    otherwise make device raw (no other input processing)
    */
    new_tio.c_iflag = IGNPAR;

    /*
    Raw output.
    */
    new_tio.c_oflag = 0;

    new_tio.c_lflag = 0;
    new_tio.c_cc[VTIME] = 0;
    new_tio.c_cc[VMIN] = 0;

    tcflush(fd_tty, TCIFLUSH);
    tcsetattr(fd_tty, TCSANOW, &new_tio);

    //tcflush(fd_tty, TCIFLUSH);
    //tcsetattr(fd_tty, TCSAFLUSH, &new_tio);

    // modemctlline = TIOCM_RTS;
    //ioctl( fd_tty, TIOCMBIS, &modemctlline );

    return 0;
}

#if 0
//  Initialize ttyS1
init_ExternalUART_1(void)
{
    struct termios new_tio;
    int modemctlline;
    int baud=115200;

    //fd_tty = open(path, O_RDWR | O_NOCTTY | O_NDELAY);
    fd_ttys1 = open( MODEMDEVICE_1, O_RDWR | O_NOCTTY | O_NDELAY | O_NONBLOCK);
    if (fd_ttys1 == -1)
        return -errno;

    memset(&new_tio, 0, sizeof(struct termios));

    new_tio.c_cflag = baud | CS8 | CREAD | CLOCAL;
    //new_tio.c_cflag = ~CRTSCTS | baud | CS8 | CREAD | CLOCAL;

    new_tio.c_cflag = BAUDRATE | CS8 | CLOCAL | CREAD;

    /*
    IGNPAR : ignore bytes with parity errors
    otherwise make device raw (no other input processing)
    */
    new_tio.c_iflag = IGNPAR;

    /*
    Raw output.
    */
    new_tio.c_oflag = 0;

    new_tio.c_lflag = 0;
    new_tio.c_cc[VTIME] = 0;
    new_tio.c_cc[VMIN] = 0;

    tcflush(fd_ttys1, TCIFLUSH);
    tcsetattr(fd_ttys1, TCSANOW, &new_tio);

//    tcflush(fd_ttys1, TCIFLUSH);
//   tcsetattr(fd_ttys1, TCSAFLUSH, &new_tio);

    // modemctlline = TIOCM_RTS;
    //ioctl( fd_ttys1, TIOCMBIS, &modemctlline );



    return 0;
}
#else
init_ExternalUART_1(void)
{
    struct termios arg;
    unsigned char flag = 0x00;
    //int temp, temp2;

    if((fd_ttys1 = open(MODEMDEVICE_1, O_RDWR | O_NOCTTY | O_NONBLOCK)) < 0)
        return(-1);

    fcntl(fd_ttys1, F_SETFL, 0);		//

    //------ Communication Setting
    tcgetattr(fd_ttys1, &arg);

    // Input, Output BaudRate Setting
    cfsetispeed(&arg, B115200);
    cfsetospeed(&arg, B115200);

    arg.c_iflag &= ~(IXOFF | IXON |ICRNL);

    //arg.c_iflag &= ~(BRKINT|IGNBRK|ICRNL|IGNPAR|PARMRK|INPCK|INLCR|IGNCR|IUCLC|IXANY|IMAXBEL|IUTF8);

    // Receiver Enable
    arg.c_cflag |= (CLOCAL | CREAD);	//Enable receiver, Ignore modem control lines

    // None Parity, 1stop bit, 8bits
    arg.c_cflag &=  ~(CSIZE | PARENB |CSTOPB);
    arg.c_cflag |= CS8;

    //no flow control
    arg.c_cflag &= ~CRTSCTS;

    arg.c_lflag &= ~(ICANON | ECHO | ECHOE | ISIG);

    arg.c_cc[VTIME] = 10;	//set time out 1 second
    arg.c_cc[VMIN] = 0;

    arg.c_oflag &= ~OPOST;

    tcsetattr(fd_ttys1, TCSANOW, &arg);
}
#endif

static uint8 checkCRC16(uint8 *Data, uint16 len, uint16 CRC )
{
    if( updateCRC16( Data, len, 0 ) == CRC )
        return 1;
    return 0;
}


io_comtask(char rev_ch)
{
    int res,j;
    //char rev_ch;

    switch(smRecvSub)
    {
    case MODEM_WAIT_START_1:
        if(rev_ch == DLE)
        {
            smRecvSub = MODEM_WAIT_START_2;
        }
        break;

    case MODEM_WAIT_START_2:
        if(rev_ch== STX)
        {
            smRecvSub = MODEM_WAIT_END_1;
            /* Start Frame */
            dataLenPortSub = 0;
            break;
            //continue;
        }
        smRecvSub = MODEM_WAIT_START_1;
        break;

    case MODEM_WAIT_END_1:
        if(rev_ch == DLE)
        {
            smRecvSub = MODEM_WAIT_END_2;
        }
        recvDataPortSub.Data[recvDataPortSub.End][dataLenPortSub]	= rev_ch;
        dataLenPortSub++;
        break;

    case MODEM_WAIT_END_2:
        if(rev_ch == DLE)
        {
            smRecvSub = MODEM_WAIT_END_1;
            /* dataLenPortVDS--; */
            break;
            //continue;
        }
        if( rev_ch == ETX )
        {
            smRecvSub = MODEM_WAIT_CHECKSUM_1;
            dataLenPortSub--;
            break;
            //continue;
        }
        smRecvSub = MODEM_WAIT_END_1;
        recvDataPortSub.Data[recvDataPortSub.End][dataLenPortSub]	= rev_ch;
        dataLenPortSub++;
        break;

    case MODEM_WAIT_CHECKSUM_1:
        chksumPortSub	= rev_ch<<8;
        smRecvSub = MODEM_WAIT_CHECKSUM_2;
        break;

    case MODEM_WAIT_CHECKSUM_2:
        chksumPortSub	= chksumPortSub + rev_ch;
        smRecvSub = MODEM_WAIT_START_1;
        /* Frame End */

        /*
        Check Ckeck Sum and Handling the buffer
        */
        if( checkCRC16(recvDataPortSub.Data[recvDataPortSub.End], dataLenPortSub, chksumPortSub ) != 1 )
        {
            //	if (term_stat == DEBUG_MONITOR_MODE) fprintf(stdout,"CRC error 2");
            fprintf(stdout,"CRC error 2");
            dataLenPortSub = 0;
            smRecvSub = MODEM_WAIT_START_1;
            return;
        }

        recvDataPortSub.Length[recvDataPortSub.End] = dataLenPortSub;

#if 0
        fprintf(stdout,"IO RX : ");

        for(j=0; j<dataLenPortSub; j++)
        {
            fprintf(stdout, "<%02x>", recvDataPortSub.Data[recvDataPortSub.End][j]);
        }
        fprintf(stdout,"\n");
#endif

        dataLenPortSub = 0;
        smRecvSub = MODEM_WAIT_START_1;

        recvDataPortSub.End ++;
        recvDataPortSub.End %= MAX_MODEM_DATA_QUEUE_NO;

        break;
    }
    rev_ch=0;
}

//#define LDC_DEBUG

void interpreteLDBmPacket()
{
    uint8 opcode = Recv2[1], sbuf[64], size = 0;
    ushort val;
    int modemctlline, i;
    int Board_Id;

    switch(opcode)
    {
    case 0x01:
    {
        revLDB_ID[Recv2[3]-1] = TRUE;
        revLDB_cnt[Recv2[3]-1] = 0;

        Board_Id = Recv2[3]-1;
#ifdef LDC_DEBUG
        fprintf(stdout,"\n------------------------------------------------------\n");
        fprintf(stdout,"Board ID : [%d]\n",Recv2[3]-1);
#endif
        // LoopStatus - normal, Open, Short, 25%
        DetInfo[Board_Id].LoopStatus[0] = Recv2[5]>>6 & 0x03;

        // sensitivity 111 : 7, 110 : 6, 101 : 5r, 100 : 4, 011 : 3, 010 : 2, 001 : 1
        DetInfo[Board_Id].FreqLevel[0] = Recv2[5] & 0x07;
        DetInfo[Board_Id].FreqLevel[1] = (Recv2[5]>>3) & 0x07;
        DetInfo[Board_Id].FreqLevel[2] = Recv2[4] & 0x07;
        DetInfo[Board_Id].FreqLevel[3] = (Recv2[4]>>3) & 0x07;

        // frequency --- 11 : high, 10 : medium high, 01 : medium low, 00 : low
        for(i=0; i<4; i++)
            DetInfo[Board_Id].FreqSel[i] = Recv2[6]>>(i*2) & 0x03;

        // mode --- 1 : pulse mode(PL), 0 : occupy mode(PR)
        for(i=0; i<4; i++)
            DetInfo[Board_Id].DetMode[i] = Recv2[7]>>(i+4) & 0x01;

        // occupy time set --- 11 : 16, 10 : 60, 01 : 120, 00 : unlimited
        DetInfo[Board_Id].OccTime = Recv2[7]>>2 & 0x03;

        // fail check --- 1 : no detect, 0 : yes
        DetInfo[Board_Id].DetFail = Recv2[7]>>1 & 0x01;

        // setting -- 1 : Console, 0 : DIP SW
        DetInfo[Board_Id].SetMode = Recv2[7] & 0x01;

#ifdef LDC_DEBUG
        fprintf(stdout,"occupy time : %d, fail check : %d, set mode:  %d\n",
                DetInfo[Board_Id].OccTime, DetInfo[Board_Id].DetFail, DetInfo[Board_Id].SetMode);
#endif

        // Loop Status
        for(i=0; i<4; i++)
            DetInfo[Board_Id].LoopStatus[i] = Recv2[8]>>(i*2) & 0x03;

#ifdef LDC_DEBUG
        fprintf(stdout,"CH  Sensitivity Freq  Mode  Status\n");
        for(i=0; i<4; i++)
        {
            fprintf(stdout," %d      %d,        %d,    %d     %d\n",
                    i, DetInfo[Board_Id].FreqLevel[i], DetInfo[Board_Id].FreqSel[i],
                    DetInfo[Board_Id].DetMode[i], DetInfo[Board_Id].LoopStatus[i]);
        }
#endif
    }
    break;
    case 0x02:
    {
    }
    break;
    case 0x03:
    {
    }
    break;
    case 0x04:
    {
    }
    break;
    case 0x05:
    {
    }
    break;
    case 0x06:
    {
    }
    break;
    case 0x07:
    {
    }
    break;
    case 0x08:
    {
    }
    break;
    case 0x09:
    {
    }
    break;
    case 0x0A:
    {
    }
    break;
    default :
#if defined(DEBUG_SYS_COMM)
        //fprintf(stdout,"\n\ropcode");
#endif
        break;
    }
}

void ldb_processtask()
{
    int i, len;

    while(recvDataPortLdb.Start != recvDataPortLdb.End)
    {
        len = recvDataPortLdb.Length[recvDataPortLdb.Start];

        for(i=0; i<len; i++)
        {
            Recv2[i] = recvDataPortLdb.Data[recvDataPortLdb.Start][i];
        }

        interpreteLDBmPacket();

        recvDataPortLdb.Start++;
        recvDataPortLdb.Start %= MAX_MODEM_DATA_QUEUE_NO;
    }
}

ldb_comtask(char * rev_buf, int size)
{
    int res, j, cnt;
    //char rev_ch;
#if 0
    fprintf(stdout,"size =  %d\n", size);
#endif

    if(rev_buf[0] == STX && rev_buf[12] == ETX)
    {
        for(cnt=1; cnt<size-1; cnt++)
        {
            recvDataPortLdb.Data[recvDataPortLdb.End][dataLenPortLdb]	= rev_buf[cnt];
            dataLenPortLdb++;
        }
#if 0
        for(j=0; j<dataLenPortLdb; j++)
        {
            fprintf(stdout,"\n\r[%02X]",recvDataPortLdb.Data[recvDataPortLdb.End][j]);
        }

        fprintf(stdout,"\n\r");
#endif
        chksumPortLdb
            = recvDataPortLdb.Data[recvDataPortLdb.End][dataLenPortLdb-2]<<8
              + recvDataPortLdb.Data[recvDataPortLdb.End][dataLenPortLdb-1];

        dataLenPortLdb = dataLenPortLdb-2;

        if( check_crc16ldb(recvDataPortLdb.Data[recvDataPortLdb.End], dataLenPortLdb, chksumPortLdb ) != 1 )
        {
            fprintf(stdout,"CRC error LD Board");
            dataLenPortLdb = 0;
            smRecvLdb = MODEM_WAIT_START_1;
            return;
        }

        recvDataPortLdb.Length[recvDataPortLdb.End] = dataLenPortLdb;

        dataLenPortLdb = 0;
        recvDataPortLdb.End ++;
        recvDataPortLdb.End %= MAX_MODEM_DATA_QUEUE_NO;

        ldb_processtask();
    }
    else
    {
        if(term_stat !=REALTIME_MONITOR_MODE )
            fprintf(stdout,"STX: %X, ETX: %X\n", rev_buf[0], rev_buf[7]);
    }
}

void interpreteIomPacket()
{
    uint8 opcode = Recv1[0];
    switch(opcode)
    {
    case OP_LOOP_EVENT :
    case 0x01:
    {
        uint32 evt_counter;
        EVENT_TYPE2_PACKET * pkt;

        pkt = (EVENT_TYPE2_PACKET *) &Recv1[0];

        evt_counter = ((uint32)(pkt->counter[0]) << 16)
                      + ((uint32)(pkt->counter[1]) << 8)
                      + ((uint32)(pkt->counter[2]));

        // by jwank 061128
        io_time_counter = evt_counter;   //insert by copy
#if 0
        fprintf(stdout,"event time : %d", evt_counter);
        fprintf(stdout,"\tloop val : %08x", pkt->loop_val);
#endif

        evt_buff.buff[evt_buff.wr_ptr].evt_time = evt_counter;
        evt_buff.buff[evt_buff.wr_ptr].loop_val = pkt->loop_val;

        evt_buff.wr_ptr++;
        evt_buff.wr_ptr %= EVT_BUFF_SIZE;

        //check_iocomm = 0;

        //fprintf(stdout," time[%010d]: 0x%08X\n", evt_counter, pkt->loop_val);
    }
    break;

    case OP_RIO_RESTART :
    {
        RIO_RESTART_PACKET * pkt;

        pkt = (RIO_RESTART_PACKET *) &Recv1[0];

        initVdsGlobalVal();

        pre_value = pkt->loop_val;

#if 0 // defined(DEBUG_SYS_COMM)
        fprintf(stdout," Recv RIO restarting. ![0x%08x]\n\r", pre_value);
#endif

        cur_non_mask_val = pre_value;

        // Mask io input value;
        pre_value &= loop_enable_msk;

        sys_comm_counter = 0;
    }

    break;

    default :
#if defined(DEBUG_SYS_COMM)
        //fprintf(stdout,"\n\ropcode");
#endif
        break;
    }
}

void tx_over_485_port(uint8 * p_src, uint32 len)
{
    uint32 i, stuffed_no=0;
    uint8 tmpb[64], * tx_ptr=p_src;

    for(i=0; i<len; i++)
        tmpb[i] = tx_ptr[i];

    if(write(fd_ttys6, tmpb, len) == -1)
    {
        fprintf(stdout,"\nrs485 trans error \n");
    }
}

void Req_LDB_ID(int nID)
{
    uint8 opcode = 1/*Recv2[1]*/, sbuf[64], size = 0;
    ushort val;

    int modemctlline, i;

    //fprintf(stdout,"\n\rREQ Board ID : [%d]",nID+1);

    ioctl( fd_ttys6, TIOCMBIC, &modemctlline );

    modemctlline = TIOCM_RTS;
    ioctl( fd_ttys6, TIOCMBIC, &modemctlline );

    sbuf[size++] = 0x02;
    sbuf[size++] = 4;
    sbuf[size++] = 0x01;
    sbuf[size++] = nID+1;
    sbuf[size++] = 0xFF;

    val = crc16_ccitt((char *)&sbuf[1], 4);

    sbuf[size++] = (char)((val >> 8) & 0xFF);
    sbuf[size++] = (char)(val & 0xFF);
    sbuf[size++] = 0x03;

    for(i=0; i<500000; i++);

    if(write(fd_ttys6, sbuf, size) == -1)
    {
        if (term_stat == DEBUG_MONITOR_MODE)
            fprintf(stdout,"\nReq_LDB_ID transe  error \n");
    }

    //for(i=0;i<500000;i++);
    //usleep(2000);
    while(1)
    {
        ioctl(fd_ttys6, TIOCSERGETLSR, &modemctlline);
        if(modemctlline)
            break;
    }

    modemctlline = TIOCM_RTS;
    ioctl( fd_ttys6, TIOCMBIS, &modemctlline );
}

void io_processtask()
{
    int i, len;

    while(recvDataPortSub.Start != recvDataPortSub.End)
    {
        len = recvDataPortSub.Length[recvDataPortSub.Start];

        for(i=0; i<len; i++)
        {
            Recv1[i] = recvDataPortSub.Data[recvDataPortSub.Start][i];
        }

        interpreteIomPacket();

        recvDataPortSub.Start++;
        recvDataPortSub.Start %= MAX_MODEM_DATA_QUEUE_NO;
    }
}

void insertSpeedCategory(uint8 t_lane, uint32 t_speed)
{
    uint8 speed_class = 0;

    if (t_lane >= MAX_LANE_NUM)
        return;

    if (t_speed < gstSysConfig.param_cfg.speed_category[0])
        speed_class = 0;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[1])
        speed_class = 1;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[2])
        speed_class = 2;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[3])
        speed_class = 3;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[4])
        speed_class = 4;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[5])
        speed_class = 5;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[6])
        speed_class = 6;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[7])
        speed_class = 7;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[8])
        speed_class = 8;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[9])
        speed_class = 9;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[10])
        speed_class = 10;
    else if (t_speed < gstSysConfig.param_cfg.speed_category[11])
        speed_class = 11;
    else
        return;

    if (speed_class >= SPEED_CATEGORY_NO )
        return;

    gstReportsOfLane[t_lane].speed_category[speed_class]++;
}

uint8 insertLengthCategory(uint8 t_lane, uint32 t_length)
{
    uint8 length_class = 0;

    if (t_lane >= MAX_LANE_NUM)
        return;

    if (t_length < gstSysConfig.param_cfg.length_category[0])
        length_class = 0;
    else if (t_length < gstSysConfig.param_cfg.length_category[1])
        length_class = 1;
    else
        length_class = 2;

    if (length_class >= LENGTH_CATEGORY_NO )
        return;

    gstReportsOfLane[t_lane].length_category[length_class]++;

    return length_class;
}

/*
char *uintToBinary(unsigned int i) {
  static char s[32 + 1] = { '0', };
  int count = 32;

  do { s[--count] = '0' + (char) (i & 1);
       i = i >> 1;
  } while (count);

  return s;
}
*/


void make_traffictask()
{
    uint32 i, bit_msk, xor_value, loop_gap, lane_gap, speed_integer, t_lane, delta_t;
    uint8 t_length_class;
    int tmp_len;
    uint8 isValid;

    //2014.05.12 by capi
    /*for inverse test*/
    /*역주행 테스트(현장 루프를 반대로 결선하여 테스트) 시만 필요한 알고리즘으로 연속적인 역주행에 대한 보완 알고리즘*/

#if 1
    for(i=0; i<gucActiveDualLoopNum; i+=2)
    {
        t_lane = lp2ln[i].lane;

        if(t_lane > MAX_LANE_NUM)
            break;

        if(lane_st[t_lane].status != IN_STS)
            continue;  // in status 상태

        if(invers_st.occ_state_with_inv[t_lane] == TRUE)
        {
            //fprintf(stdout, "OCC Over.  Time Clear, Keep IN_STS status!! \r\n ");
            continue; //2015.07.24 by capi
        }

        if((loop_st[t_lane*2].status == PRESSED) || (loop_st[t_lane*2+1].status == PRESSED))
        {
            s_time[lp2ln[i].lane] = 0;
            delta_time[lp2ln[i].lane] = 0;	//2014.05.12 by capi

            //fprintf(stdout, "loop is pressed!! Time Clear, Keep IN_STS status\r\n ", t_lane+1);
            continue; //2015.07.24 by capi
        }

        // check time
        // 시간 체크
        if(s_time[t_lane])
        {
            if ( getSystemTick() <  s_time[t_lane])
                delta_time[t_lane] = getSystemTick() + 3600000 - s_time[t_lane];
            else
                delta_time[t_lane] =  getSystemTick() - s_time[t_lane];

            if(delta_time[t_lane]>1000)
            {
                //fprintf(stdout, "Clear IN_STS %d!! \r\n ", t_lane+1);
                lane_st[t_lane].status = EXIT_STS;
            }
        }
        else
        {
            s_time[t_lane] = getSystemTick();
        }

    }
#endif

    // read event
    // 읽지 않은 이벤트가 있다면 이벤트 버퍼에서 꺼내서 확인.
    while(evt_buff.rd_ptr != evt_buff.wr_ptr)
    {
        // 이벤트 버퍼에서 이벤트 발생한 시간과 IO 값을 꺼내옴.
        cur_count = evt_buff.buff[evt_buff.rd_ptr].evt_time /* * _TICK_MS_*/; //by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
        cur_value = evt_buff.buff[evt_buff.rd_ptr].loop_val;

        cur_non_mask_val = cur_value;

        // Mask io input value.
        // 사용하지 않는 Loop는 걸러냄.
        cur_value &= loop_enable_msk;

        // generating a conversion value by using a xor operation
        // 예전 IO값과 비교(XOR)를 해서 변한 IO만 걸러냄.
        xor_value = cur_value ^ pre_value;

#if defined(DEBUG_SYS_COMM)
        fprintf(stdout, " cur_count : %d \n", cur_count);
        fprintf(stdout, " cur_value : %08x %08x \n", cur_value, xor_value);
#endif

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        // If the current status differs from the previous state
        // 그전과 변한 IO가 있다면 즉, Loop의 상태의 변동이 있다면
        if (xor_value)
        {
            //printf("xor_value = %s\n", uintToBinary(xor_value));

            // check All IO.
            // 전체 IO를 확인하다
            for(i=0, bit_msk=1; i<gucActiveDualLoopNum; i++, bit_msk<<=1)
            {
                // loop state changes
                // 해당 Loop의 상태가 변했다면.
                if (xor_value & bit_msk)
                {
                    //printf("xor_value & bit_msk = %s\n", uintToBinary(xor_value & bit_msk));

                    // Pressed loop
                    // 해당 Loop 위에 차량이 진입했다면
                    if (cur_value & bit_msk)		// Pressed
                    {
							  //// printf("avogadro....1\n");
                        //printf("cur_value & bit_msk = %s\n", uintToBinary(cur_value & bit_msk));

                        //------------------------------------------------------------------------------------------------------
                        //2015.07.14 by capi	역주행 오류 관련 추가
                        //prevention inverse algorithm error
                        //진입 시간을 저장
                        if (lp2ln[i].pos == START_POS)
                        {
									// printf("avogadro....2\n");
                            lane_st[lp2ln[i].lane].intime[START_POS] = cur_count;
                        }
                        else
                        {
									// printf("avogadro....3\n");
                            //If you update the pressed time from a inverse driving state, an error occurs
                            //이미 역주행인 상태에서 진출루프의 프레스 시간을 업데이트하면 데이터 오류 가능성이 발생한다
                            if (lane_st[lp2ln[i].lane].status != IN_RVS)
                                lane_st[lp2ln[i].lane].intime[END_POS] = cur_count;			//2015.07.14 by capi	역주행 오류 관련 추가
                        }
                        //------------------------------------------------------------------------------------------------------

                        // 해당 Loop를 진입한 것이니 계산할 것은 없다.
                        // 단지 진입 시간과 진입했음을 기록할 뿐.
                        // saving pressed time
                        loop_st[i].time[PRESS_TIME] = cur_count;
                        loop_st[i].status = PRESSED;

                        stuckoff_counter[i] = 0;		//on-off 구분 2011.06.28 by capidra

                        //fprintf(stdout, "DB %d count Reset: %d, loop : %d", lp2ln[i].lane/2, Detectorcounter[lp2ln[i].lane/2], gucActiveLoopNum);

#if defined(_LDBOARD_CHECK)
                        Detectorcounter[lp2ln[i].lane/2] = 0;		//by capi 2014.09.18
                        DetectorStatus[lp2ln[i].lane/2] = 0;
#endif

                        s_time[lp2ln[i].lane] = 0;
                        delta_time[lp2ln[i].lane] = 0;	//2014.05.12 by capi
                    }
                    // Released
                    // 해당 Loop를 차량을 벗어났다면
                    else
                    {
							  // printf("avogadro....4 \n");
                        // exit the loop
                        // 해당 Loop를 진입한 상태에서 빠져나가는 거라면
                        if (loop_st[i].status == PRESSED)
                        {

									 // printf("avogadro....5\n");
                            // saving release time
                            // 해당 Loop를 나간 시간을 기록한다.
                            loop_st[i].time[REL_TIME] = cur_count;
                            // 점유시간을 계산.
                            // wjkim insert 2007.11.14
                            if (cur_count <  loop_st[i].time[PRESS_TIME])
                                loop_gap = cur_count + 3600000 - loop_st[i].time[PRESS_TIME];
                            else
                                loop_gap = cur_count - loop_st[i].time[PRESS_TIME];

                            // 해당 Loop를 나갔음을 표시.
                            loop_st[i].status = RELEASED;

                            // reset Loop Stuck count
                            // Loop Stuck 카운트를 리셋.
                            // 오랫동안 Loop의 상태가 변하지 않는다면 (이건 도로에 있을 수 없는 일)
                            // Loop 카드 및 IO 보드 문제일 수 있으니 모니터링 해야 된다.
                            stuckon_counter[i] = 0;

                            // 점유시간이 음수라면 무시함.
                            // dddd  .. 버그 임.
                            //if ( loop_gap <=0 ) continue;

#if !defined(SYSTEM_TIMER_AUTO_RESTART)
                            // for tolerance of tick by jwank 060113
                            // 1ms Tick의 부정확함으로 오차를 보상해준다.
                            // 괜찮은 정확도를 가져옴.
                            loop_gap += 1 + (((loop_gap*15)+500)/1000);
#endif

                            // 해당 Loop의 교통량 카운트를 증가. 한대가 지나간 것이니
                            // dddd 흠. 해당 Loop를 나갔을때 증가하는 것이 맞는 것인가 ?
                            // 아니면 진입할 때 증가하는것이 맞는 것인가 ?
                            gstCenterAccuData.volume[i]++;
                            gstIncidentAccuData.volume[i]++;

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
                            gstLocalAccuData.volume[i]++;
#endif

                            // by jwank 061128
                            // 현재 Loop 점유시간을 총 누적점유시간에 더함.

                            // 버그 수정.
                            // 현재 Loop 점유시간이 Polling 시간보다 크다면 Polling 시간으로 대체.
                            // 실 점유율 계산 시의 버그를 고침.
                            if (loop_gap >= centerPollCounter)
                            {
                                gstCenterAccuData.occupy[i] = centerPollCounter;
                                gstIncidentAccuData.occupy[i] = incident_counter;
                            }
                            else
                            {
                                gstCenterAccuData.occupy[i] += loop_gap;
                                gstIncidentAccuData.occupy[i] += loop_gap;
                            }

                            if (loop_gap >= localPollCounter)
                                gstLocalAccuData.occupy[i] = localPollCounter;
                            else
                                gstLocalAccuData.occupy[i] += loop_gap;

                            // by jwank 060113
                            // 현재 Loop 점유시간을 저장.
                            gstCurrData.occupy[i] = loop_gap;

                            // 현재 Loop의 교통량을 증가.
                            gstReportsOfLoop[i].volume++;
                        }

                        // 현재 Loop가 Lane으로 속해있지 않는다면
                        // 즉, 이런 경우가 있는지는 모르겠지만, 속도와 길이는 측정하지 않고
                        // 교통량과 점유시간만 측정하는 용도의 Loop인 경우.

                        if (lp2ln[i].used == UNUSED )
                            continue;

                        // 해당 Lane(해당 Loop가 속한 Lane)
                        t_lane = lp2ln[i].lane;
                        if (t_lane >= MAX_LANE_NUM )
                            continue;

                        // 해당 Loop가 Lane의 앞 Loop라면
                        if (lp2ln[i].pos == START_POS)
                        {
									// printf("avogadro....6 \n");
                            // fprintf(stdout, "111 \r\n");
                            if(lane_st[t_lane].status == IN_RVS)	// IN_RVS인 상태에서 다시 진출 루프를 감지해도 인버스로 들어온다
                            {
										 // printf("avogadro....7 invers_st.start_inv_flag = %d \n", invers_st.start_inv_flag);
                                // 이 루틴은 스타트 포스 확인 조건문에 삽입되어야한다
                                lane_st[t_lane].status = EXIT_STS;
                                lane_st[t_lane].time[START_POS] = cur_count;

                                // fprintf(stdout, "222 \r\n");

                                if(invers_st.start_inv_flag == FALSE)
                                    continue;			//역주행 수집시작  2015.01.07 by capi

										 // printf("avogadro....7 - 1 \n");
                                //------------------------------------------------------------------------------------------------------
                                // inverse driving error handling 1
                                // 역주행 오류 처리 1
                                // 이전 수집 주기의 점유율 확인(15% 이상일때 제거) - 정체시 차선변경에 의한 오류
                                //------------------------------------------------------------------------------------------------------
                                //kiss fprintf(stdout,"\n\r inverse driving error handling 1\r");
                                if(invers_st.occ_state_with_inv[t_lane] == TRUE)
                                {
                                    //fprintf(stdout, "date %02d : %02d-%02d case 1 Lane %d OCC Delete\r\n ",sys_time.wDay, sys_time.wHour, sys_time.wMinute , t_lane+1);
                                    continue; // 2014.05.13 by capi 역주행 관련
                                }

										 // printf("avogadro....7 - 2 \n");
                                /*When the vehicle is outside the suspected inverse driving s-loop, the e-loop is not to be occupied.
                                If the e-loop determines that it is not inverse driving been occupied. 2014.01.08 by capi*/
                                /*역주행으로 의심되는 차량이 진입루프를 나갈 경우 진출루프는 점유되지 않아야한다.
                                진출 루프가 점유되어진 경우 역주행이 아니라고 판단한다. 2014.01.08 by capi*/
                                //------------------------------------------------------------------------------------------------------

                                //------------------------------------------------------------------------------------------------------
                                // inverse driving error handling 2
                                // 역주행 오류 처리 2
                                // 진입루프의 Press 시간이 진출루프의 Press 시간보다 크면, 진입루프의 Release 시간이 진출루프의 Release 시간보다
                                // 커야함. – 차선 변경 또는 트레일러와 같은 차량에 의한 오류
                                //------------------------------------------------------------------------------------------------------
                                //2015.07.14 by capi
                                //It confirms the press time and release time of the loop
                                //역주행으로 의심되는 차량의 오류 데이터를 필터링한다.
                                //각 루프의 진입시간과 진출 시간을 각각 조사한다.
                                //kiss fprintf(stdout,"\n\r inverse driving error handling 2\r");

										 // printf("avogadro....7 - 3 \n");
                                if(lane_st[t_lane].intime[START_POS] > lane_st[t_lane].time[START_POS])		//카운터 값이 최대치를 초과한 경우
                                    lane_st[t_lane].time[START_POS] += 3600000;

                                if(lane_st[t_lane].intime[END_POS] > lane_st[t_lane].time[END_POS])
                                    lane_st[t_lane].time[END_POS] += 3600000;

                                if(lane_st[t_lane].intime[START_POS] > lane_st[t_lane].intime[END_POS])
                                    invers_st.din = 1;
                                else
                                    invers_st.din = 0;

                                if(lane_st[t_lane].time[START_POS] > lane_st[t_lane].time[END_POS])
                                    invers_st.dout = 1;
                                else
                                    invers_st.dout = 0;

										 // printf("avogadro....7 - 4 \n");
                                if(invers_st.din != invers_st.dout)
                                {
                                    //fprintf(stdout, "date %02d : %02d-%02d case 2 Lane %d Data Delete - press1: %d, press2: %d release1: %d, release2: %d\r\n"
                                    //	,sys_time.wDay, sys_time.wHour, sys_time.wMinute ,t_lane+1 , lane_st[t_lane].intime[START_POS], lane_st[t_lane].intime[END_POS], lane_st[t_lane].time[START_POS], lane_st[t_lane].time[END_POS]);

                                    //fprintf(stdout, "ORG press1: %d, press2: %d release1: %d, release2: %d\r\n"
                                    //	,t_lane+1 , loop_st[t_lane*2].time[PRESSED], loop_st[t_lane*2+1].time[PRESSED], loop_st[t_lane*2].time[REL_TIME], loop_st[t_lane*2+1].time[REL_TIME]);

                                    continue;
                                }

                                //                              _____________________________
                                //_____________________________ㅣ                            ㅣ_______________ 진입
                                //
                                //                                    _______________
                                //___________________________________ㅣ              ㅣ______________________  진출

                                //------------------------------------------------------------------------------------------------------

                                lane_gap = abs(cur_count - lane_st[t_lane].time[END_POS]);

                                gstCurrData.speed[t_lane] = (360 * 100 * lane_dist[t_lane]) / lane_gap;
                                tmp_len = (lane_dist[t_lane] * loop_gap) / lane_gap - loop_diam[t_lane];
                                speed_integer = gstCurrData.speed[t_lane] / 100;

                                //------------------------------------------------------------------------------------------------------
                                // inverse driving error handling 3
                                // 역주행 오류 처리 3
                                //------------------------------------------------------------------------------------------------------
                                //kiss fprintf(stdout,"\n\r inverse driving error handling 3\r");

										 // printf("avogadro....7 - 5 \n");
                                if(gstCurrData.occupy[t_lane*2] > gstCurrData.occupy[t_lane*2+1])
                                    invers_st.inv_rate = 10*gstCurrData.occupy[t_lane*2]/(gstCurrData.occupy[t_lane*2+1]+1);		//소수점 연산을 위해 10을 곱한다
                                else
                                    invers_st.inv_rate = 10*gstCurrData.occupy[t_lane*2+1]/(gstCurrData.occupy[t_lane*2]+1);

										 // printf("avogadro....7 - 6 \n");
                                if(invers_st.inv_rate >= 20)		//역주행관련 4->2
                                {
                                    //fprintf(stdout, "date %02d : %02d-%02d case 3 Lane %d inv_rate: %d, Á¡À¯½Ã°£1: %d Á¡À¯½Ã°£2: %d\r\n"
                                    //	,sys_time.wDay, sys_time.wHour, sys_time.wMinute ,t_lane+1 , invers_st.inv_rate, gstCenterAccuData.occupy[t_lane*2], gstCenterAccuData.occupy[t_lane*2+1]);

                                    continue;
                                }

                                //------------------------------------------------------------------------------------------------------

                                //------------------------------------------------------------------------------------------------------
                                // inverse driving error handling 4
                                // 역주행 오류 처리 4
                                // 진입, 진출 점유시간 확인(100 이하일때 제거) - 차량이 루프 가장자리를 지나간 경우
                                //------------------------------------------------------------------------------------------------------
                                //kiss fprintf(stdout,"\n\r inverse driving error handling 4\r");
										 // printf("avogadro....7 - 7 \n");
                                if(gstCurrData.occupy[t_lane*2] < 100 || gstCurrData.occupy[t_lane*2+1] < 100)
                                {
                                    //printd_dbg("date %02d : %02d-%02d case 4  Lane %d Occ1: %d Occ2: %d\r\n"
                                    //	,sys_time.wDay, sys_time.wHour, sys_time.wMinute ,t_lane+1 , gstCenterAccuData.occupy[t_lane*2], gstCenterAccuData.occupy[t_lane*2+1]);

                                    continue;
                                }

                                //------------------------------------------------------------------------------------------------------

                                //------------------------------------------------------------------------------------------------------
                                // inverse driving error handling 5
                                // 역주행 오류 처리 5
                                //compare the press time and release time of the loop. Removing the difference between the value of the comparison is greater
                                //각 루프 센서의 프레스 타임과 릴리즈 타임을 비교하여 큰차이가 발생할 경우 제거
                                //------------------------------------------------------------------------------------------------------
                                //kiss fprintf(stdout,"\n\r inverse driving error handling 5\r");
										 // printf("avogadro....7 - 8 \n");
                                if(lane_st[t_lane].intime[START_POS] > lane_st[t_lane].intime[END_POS])
                                {
                                    lane_st[t_lane].delta_ptime = lane_st[t_lane].intime[START_POS]-lane_st[t_lane].intime[END_POS];
                                }
                                else
                                {
                                    lane_st[t_lane].delta_ptime = lane_st[t_lane].intime[END_POS]-lane_st[t_lane].intime[START_POS];
                                }

                                if(lane_st[t_lane].time[START_POS] > lane_st[t_lane].time[END_POS])
                                {
                                    lane_st[t_lane].delta_rtime = lane_st[t_lane].time[START_POS]-lane_st[t_lane].time[END_POS];
                                }
                                else
                                {
                                    lane_st[t_lane].delta_rtime = lane_st[t_lane].time[END_POS]-lane_st[t_lane].time[START_POS];
                                }

										 // printf("avogadro....7 - 9 \n");
                                // Tspeed Sample 10km/h - 1620 | 20km/h - 810 | 30km/h - 540
                                if((lane_st[t_lane].delta_ptime > 2000) || (lane_st[t_lane].delta_rtime > 2000))
                                {
                                    //fprintf(stdout, "date %02d : %02d-%02d case 5 Lane %d Data Delete - Delta Press time: %d, Delta Release time: %d \r\n"
                                    //	,sys_time.wDay, sys_time.wHour, sys_time.wMinute ,t_lane+1 , lane_st[t_lane].delta_ptime, lane_st[t_lane].delta_rtime);

                                    continue;
                                }

                                //                              _____________________________
                                //_____________________________ㅣ                            ㅣ_______________ 진입
                                //
                                //                            __________________________
                                //___________________________|                          ㅣ______________________  진출

                                //------------------------------------------------------------------------------------------------------

                                //fprintf(stdout, "Lane %d inv!!!! :  Occ1: %d Occ2: %d, occ_state_with_inv: %d \r\n"
                                //		,t_lane+1 , gstCenterAccuData.occupy[t_lane*2], gstCenterAccuData.occupy[t_lane*2+1], invers_st.occ_state_with_inv[t_lane] );

                                //앞의 차가 지나간 후 뒤 차가 지나가면 lane_st[t_lane].intime 과 loop_st[t_lane*2].time[PRESSED]는 틀릴 수 잇다. 역주행일경우 lane_st[t_lane].intime는 재 입력을 막음

                                //fprintf(stdout, "press1: %d, press2: %d release1: %d, release2: %d \r\n"
                                //		, lane_st[t_lane].intime[START_POS], lane_st[t_lane].intime[END_POS], lane_st[t_lane].time[START_POS], lane_st[t_lane].time[END_POS]);

                                //fprintf(stdout, "ORG press1: %d, press2: %d release1: %d, release2: %d\r\n"
                                //		,t_lane+1 , loop_st[t_lane*2].time[PRESSED], loop_st[t_lane*2+1].time[PRESSED], loop_st[t_lane*2].time[REL_TIME], loop_st[t_lane*2+1].time[REL_TIME]);

										 // printf("avogadro....7 - 10 \n");
                                gCountVehicle++;
                                if (gCountVehicle > 10000)
                                    gCountVehicle = 0;

#if defined(SUPPORT_INDIV_TRAFFIC_DATA_PROTOCOL)
										  // printf("avogadro....9\n");
                                if(invers_st.inv_rate < 20)  //역주행관련 4->2
                                {
											 // printf("avogadro....7 - 11 \n");
                                    /*
                                    fprintf(stdout, "\r\n--------------------------------------------------------------------------------------------\r\n");
                                    fprintf(stdout, "Inverse Data: date %02d-%02d, %02d-%02d, Lane [%d]\r\n", sys_time.wMonth,sys_time.wDay, sys_time.wHour, sys_time.wMinute, t_lane+1);
                                    fprintf(stdout, "--------------------------------------------------------------------------------------------\r\n");

                                    fprintf(stdout, "Delete Data - press1: %d, press2: %d release1: %d, release2: %d\r\n"
                                    	, lane_st[t_lane].intime[START_POS], lane_st[t_lane].intime[END_POS], lane_st[t_lane].time[START_POS], lane_st[t_lane].time[END_POS]);

                                    fprintf(stdout, "ORG Data    - press1: %d, press2: %d release1: %d, release2: %d\r\n"
                                    	, loop_st[t_lane*2].time[PRESSED], loop_st[t_lane*2+1].time[PRESSED], loop_st[t_lane*2].time[REL_TIME], loop_st[t_lane*2+1].time[REL_TIME]);

                                    fprintf(stdout, "OCC Time1: %d, OCC Time2: %d, inv_rate: %d\r\n"
                                    	, gstCurrData.occupy[t_lane*2], gstCurrData.occupy[t_lane*2+1], invers_st.inv_rate);

                                    fprintf(stdout, "Delta Press time: %d, Delta Release time: %d \r\n"
                                    	, lane_st[t_lane].delta_ptime, lane_st[t_lane].delta_rtime);

                                    fprintf(stdout, "--------------------------------------------------------------------------------------------\r\n");
                                    */
											   // printf("avogadro....10 \n");
                                    //fprintf(stdout,"\n\r inverse driving error handling Done.\r");

                                    // KISS 정주행도 inverse 설종되는것 확인해서 방지하도록

                                    lane_evt_flg[t_lane] = EVENT_CREATED;
                                    invers_st.gRT_Reverse[t_lane] = TRUE;
                                    invers_st.inverse_flag = TRUE;	//by capi 2014.09.02
												//printf("avogadro....7 - 12 \n");
                                    if(gstCurrData.speed[t_lane] < 20000)
                                    {
													//printf("avogadro....7 - 13 \n");
													// printf("avogadro....11 \n");
                                        // for 유지보수
                                        sendRealtimeIndivDataPacket(t_lane, TRUE); //by capi 2014

                                        //saveRealtimeDatatoNand(t_lane, TRUE); //by capi 2014.08
                                        if(rt_web_flag == RTD_ON)
                                            saveRealtimeDatatoText(t_lane, TRUE);
                                    }
                                }

#endif
                            }
                            else
                            {


											// printf("avogadro....12 \n");
                                //START_POS에서 진출 한뒤 END_POS로 진입 -> 문제 잇음. 작은 차량은 안걸릴지도

                                // 현재 Lane에 진입했음을 표시. 시간도 저장.
                                // 주의해야 될 것은 시간이 앞 Loop의 진입이 아니라 나가는 시간이라는 것이다.
                                // 속도 계산을 두 Loop의 진입 시간 차이로 해도 되지만,
                                // 현재 이 소프트웨어는 나가는 시간 차이로 계산하고 있다.
                                lane_st[t_lane].status = IN_STS;
                                lane_st[t_lane].time[START_POS] = cur_count;
                            }
                        }
                        else if(lp2ln[i].pos == END_POS)
                        {
									// printf("avogadro....13 \n");
                            if (lane_st[t_lane].status == IN_STS)	//2011.09.20 by capidra
                            {

										 // printf("avogadro....14 \n");


                                // 현재 Loop가 앞 Loop가 이니고(즉 뒷 Loop), 현재 Lane에 진입한 상태라면
                                // 두 Loop간의 시간차를 계산.
                                lane_st[t_lane].time[END_POS] = cur_count;
                                // wjkim insert 2007.11.14
                                if(cur_count < lane_st[t_lane].time[START_POS])
                                    lane_gap = 3600000 + cur_count - lane_st[t_lane].time[START_POS];
                                else
                                    lane_gap = cur_count - lane_st[t_lane].time[START_POS];

                                //Exit Current lane
                                // 현 Lane를 나갔다.
                                lane_st[t_lane].status = EXIT_STS;

#if !defined(SYSTEM_TIMER_AUTO_RESTART)
                                lane_gap += 1 + (((lane_gap*15)+500)/1000);
#endif

                                // ###############################################################
                                // ##################### 속도 계산. ##############################
                                // Loop간 거리를 이용해서 차량 속도와 길이를 계산.
                                gstCurrData.speed[t_lane] = (360*100 * lane_dist[t_lane]) / lane_gap;
                                tmp_len = (lane_dist[t_lane] * loop_gap) / lane_gap - loop_diam[t_lane];
                                speed_integer = gstCurrData.speed[t_lane] / 100;

                                //fprintf(stdout,"\n\r 1 invers_st.gRT_Reverse[%d] = %d \n\r", t_lane, invers_st.gRT_Reverse[t_lane]);
										  //printf("t_lane=%d ,tmp_len = %d lane_dist[t_lane] =%d loop_gap=%d lane_gap=%d loop_diam[t_lane]=%d \n", t_lane, tmp_len, lane_dist[t_lane], loop_gap, lane_gap, loop_diam[t_lane]);
                                if(speed_integer > 220)
                                {
                                    speed_integer = 220;
                                    gstCurrData.speed[t_lane] = 22000;
                                }

                                // minus value pass
                                // 차량 길이가 음수이면 건너뜀.
                                if (tmp_len <= 0)
                                {
                                    isValid = FALSE;
                                }
                                else
                                {
                                    gstCurrData.length[t_lane] = tmp_len;
                                    isValid = TRUE;
                                }

#if 0 //defined(DEBUG_SYS_COMM)
                                fprintf(stdout,"[%02d:%02d:%02d.%03d]", sys_time.wHour, sys_time.wMinute,
                                        sys_time.wSecond, loop_count * 1000 / max_msec_count);
                                fprintf(stdout,"lane[%d]: lane_gap = %d, speed = %d, len=%d, valid=%d\n", t_lane, lane_gap, \
                                        gstCurrData.speed[t_lane], tmp_len, isValid);
#endif

                                //fprintf(stdout,"traffic : %d, %d\n", xor_value, invers_st.inverse_flag);

                                if (isValid == TRUE)
                                {
                                    //fprintf(stdout,"\n\r 4 isValid == TRUE \n\r");
                                    //Calculated speed and langth rating classification
                                    // 위에서 계산된 속도와 길이를 등급을 나눠서 카운트함.
                                    insertSpeedCategory(t_lane, speed_integer);
                                    t_length_class = insertLengthCategory(t_lane, gstCurrData.length[t_lane]);

                                    // It accumulates the speed and length. calculate average per polling cycle.
                                    // 속도와 길이를 누적시킴. 나중에 Polling 시간 동안의 평균값을 계산할 것임.
                                    gstCenterAccuData.speed[t_lane] += speed_integer;
                                    gstCenterAccuData.length[t_lane] += gstCurrData.length[t_lane];

                                    gstIncidentAccuData.speed[t_lane] += speed_integer;

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
                                    gstLocalAccuData.speed[t_lane] += speed_integer;
                                    gstLocalAccuData.length[t_lane] += gstCurrData.length[t_lane];
#endif

                                    //Record the event of one vehicle.
                                    // 현 Lane에 관한 이벤트를 기록함. 즉, 차량 하나를 발견한 것 임.
                                    lane_evt_flg[t_lane] = EVENT_CREATED;
                                    invers_st.gRT_Reverse[t_lane] = FALSE;
                                    realtime_data_valid[t_lane] = EVENT_CREATED;

                                    //accumulates the volumn count and occ
                                    // 현 Lane에 대한 교통량과 점유시간을 누적합산함.
                                    lane_volume[t_lane]++;
                                    occupy[t_lane] += loop_gap;

                                    //record realtime data

                                    // kiss : not save the data when network is disconnect -- 20-09-04 (강원 충북)
                                    //  initRealtimePool 할지 말지 추후 확인
                                    if(vds_st.state == SM_VDS_CLIENT_CONNECTED)  // kiss this line insert - 20-09-04
                                        saveRealtimeData(t_lane);  //by capi 2014.08

                                    saveRealtimeDatatoNand(t_lane, FALSE); //by capi 2014.08
                                    if(rt_web_flag == RTD_ON)
                                        saveRealtimeDatatoText(t_lane, FALSE);

                                    gCountVehicle++;
                                    if (gCountVehicle > 10000)
                                        gCountVehicle = 0;

                                    sendRealtimeIndivDataPacket(t_lane, FALSE); //by capi 2014
                                }
                            }
                            else if(loop_st[gstLaneLink[t_lane].s_loop].status == PRESSED)
									 //else if (loop_st[gstLaneLink[t_lane].s_loop].status == RELEASED)
                            {
										// printf("avogadro....15 \n");

                                lane_st[t_lane].status = IN_RVS;
                                lane_st[t_lane].time[END_POS] = cur_count;
                            }
                            else	//추가 2015.07.14
                            {
										// printf("avogadro....16 t_lane=%d,  gstLaneLink[t_lane].s_loop = %d ,loop_st[gstLaneLink[t_lane].s_loop].status=%02x \n", t_lane, gstLaneLink[t_lane].s_loop, loop_st[gstLaneLink[t_lane].s_loop].status);
                                lane_st[t_lane].status = EXIT_STS;
                            }

									// printf("avogadro gstLaneLink[t_lane].s_loop = %d , loop_st[gstLaneLink[t_lane].s_loop].status =%d \n", gstLaneLink[t_lane].s_loop, loop_st[gstLaneLink[t_lane].s_loop].status);
                        }// END_POS
                    }
                }
                else
                {
                    //reset stock on/off 2013.10.28
                    //여기서 stock on/off 체크해주어야함 2013.10.28
                    if (cur_value & bit_msk)
                        stuckoff_counter[i] = 0;
                    else
                        stuckon_counter[i] = 0;
                }
            }
#if 1
            //fprintf(stdout,"\n\r gucActiveSingleLoopNum = %d\n\r", gucActiveSingleLoopNum);
            // Single loop ----------------------------
            for(i=0, bit_msk = 1<<gucActiveDualLoopNum; i<gucActiveSingleLoopNum; i++, bit_msk<<=1)
            {
                //loop state changes
                // 해당 Loop의 상태가 변했다면.
                if (xor_value & bit_msk)
                {
                    // pressed loop
                    // 해당 Loop 위에 차량이 진입했다면
                    if (cur_value & bit_msk)		// Pressed
                    {
                        // saving pressed time
                        // 해당 Loop를 진입한 것이니 계산할 것은 없다.
                        // 단지 진입 시간과 진입했음을 기록할 뿐.
                        loop_st[gucActiveDualLoopNum + i*2].time[PRESS_TIME] = cur_count;
                        loop_st[gucActiveDualLoopNum + i*2].status = PRESSED;

                        //on-off 구분 2011.06.28 by capidra
                        stuckoff_counter[gucActiveDualLoopNum + i*2] = 0;
                        stuckoff_counter[gucActiveDualLoopNum + i*2+1] = stuckoff_counter[gucActiveDualLoopNum + i*2];
                    }
                    else // Released // 해당 Loop를 차량을 벗어났다면
                    {
                        // 해당 Loop를 진입한 상태에서 빠져나가는 거라면
                        if (loop_st[gucActiveDualLoopNum + i*2].status == PRESSED)
                        {
                            // 해당 Loop를 나간 시간을 기록한다.
                            loop_st[gucActiveDualLoopNum + i*2].time[REL_TIME] = cur_count;

                            // 점유시간을 계산.
                            // wjkim insert 2007.11.14
                            if ( cur_count <  loop_st[gucActiveDualLoopNum + i*2].time[PRESS_TIME])
                                loop_gap = cur_count + 3600000 - loop_st[gucActiveDualLoopNum + i*2].time[PRESS_TIME];
                            else
                                loop_gap =  cur_count -loop_st[i*2].time[PRESS_TIME];

                            // 해당 Loop를 나갔음을 표시.
                            loop_st[gucActiveDualLoopNum + i*2].status = RELEASED;

                            // Loop Stuck 카운트를 리셋.
                            // 오랫동안 Loop의 상태가 변하지 않는다면 (이건 도로에 있을 수 없는 일)
                            // Loop 카드 및 IO 보드 문제일 수 있으니 모니터링 해야 된다.
                            //데이터를 복사하여 듀얼루프로 만들어준다
                            stuckon_counter[gucActiveDualLoopNum + i*2] = 0;
                            stuckon_counter[gucActiveDualLoopNum + i*2+1] = stuckon_counter[gucActiveDualLoopNum + i*2];

#if !defined(SYSTEM_TIMER_AUTO_RESTART)
                            // for tolerance of tick by jwank 060113
                            // 1ms Tick의 부정확함으로 오차를 보상해준다.
                            // 괜찮은 정확도를 가져옴.
                            loop_gap += 1 + (((loop_gap*15)+500)/1000);
#endif

                            // 해당 Loop의 교통량 카운트를 증가. 한대가 지나간 것이니
                            // dddd 흠. 해당 Loop를 나갔을때 증가하는 것이 맞는 것인가 ?
                            // 아니면 진입할 때 증가하는것이 맞는 것인가 ?

                            //단루프 데이터 복사하여 듀얼루프로 만든다
                            gstCenterAccuData.volume[gucActiveDualLoopNum + i*2]++;
                            gstCenterAccuData.volume[gucActiveDualLoopNum + i*2+1] = gstCenterAccuData.volume[gucActiveDualLoopNum + i*2];

                            gstIncidentAccuData.volume[gucActiveDualLoopNum + i*2]++;
                            gstIncidentAccuData.volume[gucActiveDualLoopNum + i*2+1] = gstIncidentAccuData.volume[gucActiveDualLoopNum + i*2];

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
                            gstLocalAccuData.volume[gucActiveDualLoopNum + i*2]++;
                            gstLocalAccuData.volume[gucActiveDualLoopNum + i*2+1] = gstLocalAccuData.volume[gucActiveDualLoopNum + i*2];

#endif

                            // by jwank 061128
                            // 현재 Loop 점유시간을 총 누적점유시간에 더함.

                            // 버그 수정.
                            // 현재 Loop 점유시간이 Polling 시간보다 크다면 Polling 시간으로 대체.
                            // 실 점유율 계산 시의 버그를 고침.
                            if (loop_gap >= centerPollCounter)
                            {
                                gstCenterAccuData.occupy[gucActiveDualLoopNum + i*2] = centerPollCounter;
                                gstIncidentAccuData.occupy[gucActiveDualLoopNum + i*2] = incident_counter;
                            }
                            else
                            {
                                gstCenterAccuData.occupy[gucActiveDualLoopNum + i*2] += loop_gap;
                                gstIncidentAccuData.occupy[gucActiveDualLoopNum + i*2] += loop_gap;
                            }

                            //빈루프에 복사
                            gstCenterAccuData.occupy[gucActiveDualLoopNum + i*2+1] = gstCenterAccuData.occupy[gucActiveDualLoopNum + i*2];
                            gstIncidentAccuData.occupy[gucActiveDualLoopNum + i*2+1] = gstIncidentAccuData.occupy[gucActiveDualLoopNum + i*2];

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
                            if (loop_gap >= localPollCounter)
                                gstLocalAccuData.occupy[gucActiveDualLoopNum + i*2] = localPollCounter;
                            else
                                gstLocalAccuData.occupy[gucActiveDualLoopNum + i*2] += loop_gap;

                            //빈루프에 복사
                            gstLocalAccuData.occupy[gucActiveDualLoopNum + i*2+1] = gstLocalAccuData.occupy[gucActiveDualLoopNum + i*2];

#endif

                            // by jwank 060113
                            // 현재 Loop 점유시간을 저장. //단루프 루프별 점유율 및 교통량 산출 : 실존하는 루프의 데이터를 복사하여 빈루프에 저장하여 0이되지 않게 한다.
                            gstCurrData.occupy[gucActiveDualLoopNum + i*2] = loop_gap;
                            gstCurrData.occupy[gucActiveDualLoopNum + i*2+1] = loop_gap;

                            // 현재 Loop의 교통량을 증가.
                            gstReportsOfLoop[gucActiveDualLoopNum + i*2].volume++;
                            gstReportsOfLoop[gucActiveDualLoopNum + i*2+1].volume++;
                        }

                        // 현재 Loop가 Lane으로 속해있지 않는다면
                        // 즉, 이런 경우가 이쓴지는 모르겠지만, 속도와 길이는 측정하지 않고
                        // 교통량과 점유시간만 측정하는 용도의 Loop인 경우.
                        if ( lp2ln[gucActiveDualLoopNum + i*2].used == UNUSED )
                            continue;

                        // 해당 Lane(해당 Loop가 속한 Lane)
                        t_lane = lp2ln[gucActiveDualLoopNum + i*2].lane;
                        if (t_lane >= MAX_LANE_NUM )
                            continue;

                        // 단루프에서는 속도 산출 안함

                        gstCurrData.speed[t_lane] = 0;
                        tmp_len = 0;
                        speed_integer = 0;

                        gstCurrData.length[t_lane] = 0;

                        gstCenterAccuData.speed[t_lane] = 0;
                        gstCenterAccuData.length[t_lane] = 0;

                        gstIncidentAccuData.speed[t_lane] = 0;
                        gstIncidentAccuData.length[t_lane] = 0;

#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
                        gstLocalAccuData.speed[t_lane] = 0;
                        gstLocalAccuData.length[t_lane] = 0;
#endif

                        // 현 Lane에 관한 이벤트를 기록함. 즉, 차량 하나를 발견한 것 임.
                        lane_evt_flg[t_lane] = EVENT_CREATED;
                        realtime_data_valid[t_lane] = EVENT_CREATED;

                        // 현 Lane에 대한 교통량과 점유시간을 누적합산함.
                        lane_volume[t_lane]++;
                        occupy[t_lane] = 0;


                        // kiss : not save the data when network is disconnect -- 20-09-04 (강원 충북)
                        if(vds_st.state == SM_VDS_CLIENT_CONNECTED)  // kiss insert - 20-09-04
                            saveRealtimeData(t_lane);  //by capi 2014.08

                        saveRealtimeDatatoNand(t_lane, FALSE); //by capi 2014.08
                        if(rt_web_flag == RTD_ON)
                            saveRealtimeDatatoText(t_lane, FALSE);

                        gCountVehicle++;
                        if (gCountVehicle > 10000)
                            gCountVehicle = 0;
                        //kiss
                        sendRealtimeIndivDataPacket(t_lane, FALSE);
                    }
                }
                else
                {
                    //reset stockcount
                    //여기서 stock on/off 체크해주어야함 2013.10.28
                    if (cur_value & bit_msk)
                    {
                        stuckoff_counter[gucActiveDualLoopNum + i*2] = 0;
                        stuckoff_counter[gucActiveDualLoopNum + i*2+1] = stuckoff_counter[gucActiveDualLoopNum + i*2];
                    }
                    else
                    {
                        stuckon_counter[gucActiveDualLoopNum + i*2] = 0;
                        stuckon_counter[gucActiveDualLoopNum + i*2+1] = stuckon_counter[gucActiveDualLoopNum + i*2];
                    }
                }
            }
#endif

#if 0  //kiss 역주행
            if(vds_st.state == SM_VDS_CLIENT_DEVID_SEND)	//2014.03.26 insert by capi 	SM_VDS_CLIENT_DEVID_SEND = 3
            {
                if(invers_st.inverse_flag)
                {
                    // fprintf(stdout,"KISS enter inverse_flag \n\r");
                    if(invers_st.inverse_cnt == 0)
                    {
                        // fprintf(stdout,"KISS inverse_cnt == 0 \n\r");
                        invers_st.inverse_cnt = getSystemTick();
                        invers_st.inverse_time = 1;

//                        fprintf(stdout,"KISS SendInversData(0) \n\r");
                        SendInversData(0);

                        for (i=0; i<MAX_LANE_NUM; i++)
                            invers_st.gRT_Reverse[i] = FALSE;  // kiss // mark delete
                    }
                    else
                    {
                        if ( getSystemTick() <	invers_st.inverse_cnt)
                            delta_t = getSystemTick() + 3600000 - invers_st.inverse_cnt;
                        else
                            delta_t =  getSystemTick() - invers_st.inverse_cnt;

                        if(delta_t > 10000)
                        {
                            invers_st.inverse_cnt = getSystemTick();


//                            fprintf(stdout,"KISS SendInversData(1) \n\r");
                            SendInversData(1);

                            invers_st.inverse_time++;

                            if(invers_st.inverse_time>2)
                            {
                                invers_st.inverse_flag = 0;
                                invers_st.inverse_cnt = 0;
                                invers_st.inverse_time = 0;

                                for (i=0; i<MAX_LANE_NUM; i++)
                                {
                                    // fprintf(stdout,"\n\r 5 single loop \n\r");

                                    invers_st.gRT_Reverse_retrans[i] = FALSE;
                                    invers_st.gRT_Reverse[i] = FALSE;// 추가 확인
                                }
                            }

                            //fprintf(stdout, DBG_PFX_VDSCLIENT " Send Inverse Data 2 %d / %d\n\r", invers_st.inverse_cnt, getSystemTick());
                        }
                        else
                        {
//                            fprintf(stdout,"KISS delta_t <= 10000 \n\r");
                        }

                    }

                }
                else
                {
//                    fprintf(stdout,"KISS not send inverse flag. \n\r");
                }
            }
            else
            {
                invers_st.inverse_flag = 0;
                invers_st.inverse_cnt = 0;
                invers_st.inverse_time = 0;

                for (i=0; i<MAX_LANE_NUM; i++)
                {
                    invers_st.gRT_Reverse_retrans[i] = FALSE;
                    invers_st.gRT_Reverse[i] = FALSE;
                }
            }
#endif
        }
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        pre_count = cur_count;
        pre_value = cur_value;

        evt_buff.rd_ptr++;
        evt_buff.rd_ptr %= EVT_BUFF_SIZE;
    }
}


//by capi
void mainteCommTask(char *ch)
{
    uint16 i;
    int len ;
    unsigned char rev_ch, ch_num;

    rev_ch = *ch;
    /*
    fprintf(stdout,"%02X|", rev_ch);

    len = dataLenPortA%15;
    if(len == 12) fprintf(stdout,"\n");
    */
    switch(smRecvPortA)
    {
    case MODEM_WAIT_START_1:
        if(rev_ch == DLE)
        {
            smRecvPortA = MODEM_WAIT_START_2;
        }
        break;

    case MODEM_WAIT_START_2:
        if(rev_ch== STX)
        {
            smRecvPortA = MODEM_WAIT_END_1;
            /* Start Frame */
            dataLenPortA = 0;
            return;
        }
        smRecvPortA = MODEM_WAIT_START_1;
        break;

    case MODEM_WAIT_END_1:
        if(rev_ch == DLE)
        {
            smRecvPortA = MODEM_WAIT_END_2;
        }
        recvDataPortA.Data[recvDataPortA.End][dataLenPortA]	= rev_ch;
        dataLenPortA++;
        break;

    case MODEM_WAIT_END_2:
        if(rev_ch == DLE)
        {
            smRecvPortA = MODEM_WAIT_END_1;
            /* dataLenPortVDS--; */
            return;
        }
        if(rev_ch == ETX)
        {
            smRecvPortA = MODEM_WAIT_CHECKSUM_1;
            dataLenPortA--;
            return;
        }
        smRecvPortA = MODEM_WAIT_END_1;
        recvDataPortA.Data[recvDataPortA.End][dataLenPortA]	= rev_ch;
        dataLenPortA++;
        break;
    case MODEM_WAIT_CHECKSUM_1:
        chksumPortA	= rev_ch<<8;
        smRecvPortA = MODEM_WAIT_CHECKSUM_2;
        break;

    case MODEM_WAIT_CHECKSUM_2:
        chksumPortA	= chksumPortA + rev_ch;
        smRecvPortA = MODEM_WAIT_START_1;
        /* Frame End */

        /*
        Check Ckeck Sum and Handling the buffer
        */
        if( checkCRC16(recvDataPortA.Data[recvDataPortA.End], dataLenPortA, chksumPortA) != 1 )
        {
            if (term_stat == DEBUG_MONITOR_MODE)
                fprintf(stdout," CRC error 3");
            fprintf(stdout,"\n\rCRC ERROR Rev size : %d \r\n", dataLenPortA);
            dataLenPortA = 0;
            smRecvPortA = MODEM_WAIT_START_1;
            return;
        }

        recvDataPortA.Length[recvDataPortA.End] = dataLenPortA;

        fprintf(stdout,"\n\rRev size : %d \r\n", dataLenPortA);

#ifdef DEBUG_SYS_COMM
        fprintf(stdout,"\n\rRX2 : ");

        for(j=0; j<dataLenPortA; j++)
        {
            fprintf(stdout, "%02x", recvDataPortA.Data[recvDataPortA.End][j]);
        }
        fprintf(stdout,"\n\r");
#endif

        dataLenPortA = 0;
        smRecvPortA = MODEM_WAIT_START_1;

        recvDataPortA.End ++;
        recvDataPortA.End %= MAX_MODEM_DATA_QUEUE_NO;

        break;
    }

#if defined(ENABLE_WATCHDOG)
    touchWatchdog();
#endif
}
