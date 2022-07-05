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
//#include "VDS_parameter.h"
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

uint8 eventLane;
extern uint8 Start_Stop_Flag;
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
	
	for (i=0; i<MAX_LOOP_NUM; i++) gstReportsOfLoop[i].volume = 0;

	for (i=0; i<MAX_LANE_NUM; i++) {
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

	for (i=0; i<MAX_RIO_LANE_NUM; i++) realtime_data_valid[i] = NO_EVENT;


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

	for (i=0; i<MAX_LANE_NUM; i++) loop_diam[i] = 18;
	for (i=0; i<MAX_LANE_NUM; i++) lane_dist[i] = 45;
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
	invers_st.start_inv_flag = FALSE;	//by capi 2015.01.07 for inverse Driving
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

    if((fd_ttys1 = open(MODEMDEVICE_1, O_RDWR | O_NOCTTY | O_NONBLOCK)) < 0) return(-1);

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

static uint8 checkCRC16(uint8 *Data, uint16 len , uint16 CRC )
{
	if( updateCRC16( Data, len, 0 ) == CRC ) return 1;
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

		for(j=0;j<dataLenPortSub;j++)
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
			for(i=0;i<4;i++)
				DetInfo[Board_Id].FreqSel[i] = Recv2[6]>>(i*2) & 0x03;

			// mode --- 1 : pulse mode(PL), 0 : occupy mode(PR)
			for(i=0;i<4;i++)
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
			for(i=0;i<4;i++)
				DetInfo[Board_Id].LoopStatus[i] = Recv2[8]>>(i*2) & 0x03;

#ifdef LDC_DEBUG
			fprintf(stdout,"CH  Sensitivity Freq  Mode  Status\n");
			for(i=0;i<4;i++){
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
		for(cnt=1;cnt<size-1;cnt++)
		{
			recvDataPortLdb.Data[recvDataPortLdb.End][dataLenPortLdb]	= rev_buf[cnt];
			dataLenPortLdb++;
		}
#if 0
		for(j=0; j<dataLenPortLdb;j++)
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
//		if(term_stat !=REALTIME_MONITOR_MODE )
//			fprintf(stdout,"STX: %X, ETX: %X\n", rev_buf[0], rev_buf[7]);
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

	for(i=0; i<len; i++) tmpb[i] = tx_ptr[i];	
	
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

	for(i=0;i<500000;i++);

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
		if(modemctlline) break;
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
	
	if (t_lane >= MAX_LANE_NUM) return;

	if (t_speed < gstSysConfig.param_cfg.speed_category[0]) speed_class = 0;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[1]) speed_class = 1;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[2]) speed_class = 2;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[3]) speed_class = 3;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[4]) speed_class = 4;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[5]) speed_class = 5;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[6]) speed_class = 6;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[7]) speed_class = 7;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[8]) speed_class = 8;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[9]) speed_class = 9;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[10]) speed_class = 10;
	else if (t_speed < gstSysConfig.param_cfg.speed_category[11]) speed_class = 11;
	else return;

	if (speed_class >= SPEED_CATEGORY_NO ) return;

	gstReportsOfLane[t_lane].speed_category[speed_class]++;
}

uint8 insertLengthCategory(uint8 t_lane, uint32 t_length)
{
	uint8 length_class = 0;
	
	if (t_lane >= MAX_LANE_NUM) return;

	if (t_length < gstSysConfig.param_cfg.length_category[0]) length_class = 0;
	else if (t_length < gstSysConfig.param_cfg.length_category[1]) length_class = 1;
	else length_class = 2;

	if (length_class >= LENGTH_CATEGORY_NO ) return;

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

//-------------------------------------------------------------
// kict test variable
uint32 kict_occupancy,kict_occupancy1, park_occupancy;
uint32 raw_speed,raw_speed_sp;
uint32 lane_gap_sp, lane_gap_ep;
uint32 lane_loop_gap;
//-------------------------------------------------------------

void make_traffictask()
{

	uint32 i, bit_msk, xor_value, loop_gap, lane_gap, speed_integer, t_lane, delta_t;
	uint8 t_length_class;
	int tmp_len;
	uint8 isValid;

	// read event
	while( evt_buff.rd_ptr != evt_buff.wr_ptr )
	{		
		cur_count = evt_buff.buff[evt_buff.rd_ptr].evt_time /* * _TICK_MS_*/; //by capi 2015.12.09 delete * _TICK_MS_. DCS9S _TICK_MS_ = 10
		cur_value = evt_buff.buff[evt_buff.rd_ptr].loop_val;

		cur_non_mask_val = cur_value;

		// Mask io input value;		
		cur_value &= loop_enable_msk;

		//generating a conversion value by using a xor operation
		xor_value = cur_value ^ pre_value;

		#if 0
		fprintf(stdout, " cur_count : %d \n", cur_count);
		fprintf(stdout, " cur_value : %08x %08x \n", cur_value, xor_value);
		#endif

		/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
		// If the current status differs from the previous state
		if (xor_value)
		{
			// check All IO.
			for(i=0, bit_msk=1; i<gucActiveDualLoopNum; i++, bit_msk<<=1)
			{
				//loop state changes
				if (xor_value & bit_msk)
				{
					//  Pressed loop
					if (cur_value & bit_msk)		// Pressed
					{
						// savi ng pressed time
						loop_st[i].time[PRESS_TIME] = cur_count;	
						loop_st[i].status = PRESSED;												
					}						
					else // Released // 
					{
						//printf("aaa floating error check\n");
						// exit the loop
						if (loop_st[i].status == PRESSED)
						{
							// saving release time
							loop_st[i].time[REL_TIME] = cur_count;
							
							if ( cur_count <  loop_st[i].time[PRESS_TIME])
								loop_gap = cur_count + 3600000 - loop_st[i].time[PRESS_TIME];
							else 
								loop_gap = cur_count - loop_st[i].time[PRESS_TIME];
							
							loop_st[i].status = RELEASED;

							// reset Loop Stuck count							
							stuckon_counter[i] = 0;
							

							#if !defined(SYSTEM_TIMER_AUTO_RESTART)
							// for tolerance of tick by jwank 060113							
							loop_gap += 1 + (((loop_gap*15)+500)/1000);
							#endif

							// kict test
							if( loop_gap < 130 || loop_gap > 200000){
							//	fprintf(stdout, "loop_gap error = %d\n\r", loop_gap);
								//loop_gap = 150;
								continue;
							}
							
							gstCenterAccuData.volume[i]++;
							gstIncidentAccuData.volume[i]++;							
							
							#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
								gstLocalAccuData.volume[i]++;
							#endif
							
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
							
							if (loop_gap >= localPollCounter) gstLocalAccuData.occupy[i] = localPollCounter;
							else gstLocalAccuData.occupy[i] += loop_gap;													
							
							gstCurrData.occupy[i] = loop_gap;							

							gstReportsOfLoop[i].volume++;
						}						
						
						if ( lp2ln[i].used == UNUSED ) continue;
						
						t_lane = lp2ln[i].lane;
						if (t_lane >= MAX_LANE_NUM ) continue;
						
						if ( lp2ln[i].pos == START_POS)
						{							
								lane_st[t_lane].status = IN_STS;
								lane_st[t_lane].time[START_POS] = cur_count;
								// kict test
								// save first loop press time
								lane_st[t_lane].loop_stime[START_POS] = loop_st[i].time[PRESS_TIME];			
						}
						else if (lane_st[t_lane].status == IN_STS)	//2011.09.20 by capidra
						{		
								//printf("bbb floating test\n");						
								//------------------------- KICT ------------------------------------------------
								// KICT TEST Program
								//Exit Current lane
								lane_st[t_lane].status = EXIT_STS;

								// rear loop occupy > 10 sec
								// ?뺤껜 ?뚯뒪??2016-06-15 10->50->70 sec								
								if( gstCurrData.occupy[gstLaneLink[t_lane].e_loop] > 70000)
									continue;
								
								// front loop occupy < rear loop occupy
								// ?뺤껜 ?뚯뒪??2016-06-15 10->50->70 sec, ??異??먯쑀?쒓컙 鍮꾧탳 ?쒓굅
								if(gstCurrData.occupy[gstLaneLink[t_lane].s_loop] > 70000){
									//if(gstCurrData.occupy[gstLaneLink[t_lane].s_loop] + 2000 < gstCurrData.occupy[gstLaneLink[t_lane].e_loop])
										continue;
								}

								lane_st[t_lane].time[END_POS] = cur_count;
								// save second loop press time
								lane_st[t_lane].loop_stime[END_POS] = loop_st[i].time[PRESS_TIME];		
	
								// END_POS(R) - START_POS(R)
								if( lane_st[t_lane].time[END_POS] < lane_st[t_lane].time[START_POS])
									lane_gap_ep = 3600000+ lane_st[t_lane].time[END_POS] - lane_st[t_lane].time[START_POS];
								else
									lane_gap_ep = lane_st[t_lane].time[END_POS] - lane_st[t_lane].time[START_POS];
								
								// END_POS(F) - START_POS(F)							
								if(lane_st[t_lane].loop_stime[END_POS] <  lane_st[t_lane].loop_stime[START_POS])
									lane_gap_sp = 3600000 + lane_st[t_lane].loop_stime[END_POS] - lane_st[t_lane].loop_stime[START_POS];
								else
									lane_gap_sp = lane_st[t_lane].loop_stime[END_POS] - lane_st[t_lane].loop_stime[START_POS];
	
								// add lane gap END_PS(R) - START_POS(F)
								if(lane_st[t_lane].time[END_POS] <  lane_st[t_lane].loop_stime[START_POS])
									lane_loop_gap = 3600000 + lane_st[t_lane].time[END_POS] - lane_st[t_lane].loop_stime[START_POS];
								else
									lane_loop_gap = lane_st[t_lane].time[END_POS] - lane_st[t_lane].loop_stime[START_POS];
							
								//----------  lane_gap processing ------------------
								lane_gap = lane_gap_ep;
								//--------------------------------------------------
	
								if ( lane_gap <= 0 ) 
									continue;	

								lane_gap += 1 + (((lane_gap*15)+500)/1000)/* - 1*/;
								lane_gap_sp += 1 + (((lane_gap_sp*15)+500)/1000)/* - 1*/;	//쩔?쩔쩔쩔??START POINT 쩔쩔?
								lane_loop_gap += 1 + (((lane_loop_gap*15)+500)/1000)/* - 1*/;	//쩔쩔쩔 쩔쩔쩔?? START POINT 쩔쩔쩔쩔쩔쩔??END POINT쩔?쩔쩔?

								//lane_gap += 1 + (((lane_gap*15)+500)/1000);
								
								// ###############################################################
								// ##################### calculation speed ################							
								
								raw_speed = (360*100 * lane_dist[t_lane]) / lane_gap;
								raw_speed_sp = (360*100 * lane_dist[t_lane]) / lane_gap_sp;
								//printf("ccc c\n");
								// 
								//if(raw_speed < 500)  // 5km/h
								if(raw_speed < 1000)  // 10km/h 140429 ?곸슜
									gstCurrData.speed[t_lane] = (raw_speed + raw_speed_sp)/2;
								else 
									gstCurrData.speed[t_lane] = raw_speed;

								//if(gstCurrData.speed[t_lane] > 13000) continue; // speed > 130
								if(gstCurrData.speed[t_lane] > 15000) continue; // speed > 150 140429 ?곸슜
								
								//if(gstCurrData.speed[t_lane] > 0 && gstCurrData.speed[t_lane] < 13000)
								//if(gstCurrData.speed[t_lane] > 0 && gstCurrData.speed[t_lane] < 15000) // speed<150 140430 ?곸슜
								if(gstCurrData.speed[t_lane] > 99 && gstCurrData.speed[t_lane] < 15000) // fix 160523
									speed_integer = gstCurrData.speed[t_lane] / 100;
								else speed_integer = 1;

								// length
								//tmp_len = (lane_dist[t_lane] * loop_gap) / lane_gap - loop_diam[t_lane];
								tmp_len = (lane_dist[t_lane] * loop_gap) * 10 / lane_gap - loop_diam[t_lane]*10;
	
								// 쩔쩔쩔 쩔쩔쩔쩔???쩔쩔?쩔쩔 쩔쩔쩔???
								if(tmp_len <= 200) { //2m
									isValid = FALSE;
								}
								else {
									gstCurrData.length[t_lane] = tmp_len+1;		//쩔쩔쩔??CRC쩔쩔쩔 쩔쩔쩔
									isValid = TRUE;
								}
												
								// kict occupancy : 3m 
								//kict_occupancy = (12*360)/speed_integer + gstCurrData.occupy[gstLaneLink[t_lane].e_loop]+6;
								//if(speed_integer == 0) 
									//printf("floating point test - speed_integer = %d\n", speed_integer);
								kict_occupancy = (12*360)/speed_integer + gstCurrData.occupy[gstLaneLink[t_lane].e_loop];	
								kict_occupancy = kict_occupancy + kict_occupancy * 2/100;	
							
								if(kict_occupancy < 150 || kict_occupancy > 100000)
									continue;
		
								// kict rule
								//kict_occupancy1 = lane_gap_sp * 12/45 + gstCurrData.occupy[gstLaneLink[t_lane].e_loop]+6;
								kict_occupancy1 = lane_gap_sp * 12/45 + gstCurrData.occupy[gstLaneLink[t_lane].e_loop];
								//fprintf(stdout,"occupy = %d\n",gstCurrData.occupy[gstLaneLink[t_lane].e_loop]);

	
								//kict_occupancy1 =  lane_loop_gap - lane_loop_gap*15/68;		//test by capidra
								//kict_occupancy1 =  lane_gap_sp*48/45;		//test by capidra
								//kict_occupancy1 = lane_gap * 12/45 + gstCurrData.occupy[gstLaneLink[t_lane].e_loop];
								//fprintf(stdout,"kict_occupy = %d, lane_gap = %d\n",kict_occupancy1, lane_gap);
	
								//kict_occupancy1 = kict_occupancy1 + kict_occupancy1 * 2/100+10;		//쩔쩔쩔쩔?: 7 by capidra

								//kict_occupancy1 = kict_occupancy1 + kict_occupancy1 * 2/100+78-50;		//蹂댁젙媛?: 7 by capidra

								//-------------------------------------------------------
								// kict test up/down isolation 
								if(t_lane<2)  kict_occupancy1 = kict_occupancy1 + kict_occupancy1 * 2/100+78-50-9-10;		//?곹뻾蹂댁젙媛?: -6
								else kict_occupancy1 = kict_occupancy1 + kict_occupancy1 * 2/100+78-50-5-10;	 	//?섑뻾蹂댁젙媛?: 0								
								
								//-------------------------------------------------------
								// kict test insert
#if 0
								if( kict_occupancy1 < 150 || kict_occupancy1 > 100000)
									continue;
#endif
								//-------------------------------------------------------

								// send occupy
								if( raw_speed < 1000)  // 10km/h
									gstCurrData.kict_occupy[t_lane] = kict_occupancy;
								else
									gstCurrData.kict_occupy[t_lane] = kict_occupancy1;

								
								#if 0 //defined(DEBUG_SYS_COMM)
								fprintf(stdout, "\n lane[%d]: %d %d %d -valid[%d]", t_lane, lane_gap, \
									gstCurrData.speed[t_lane], tmp_len, isValid);
								#endif												

								if (isValid == TRUE)
								{
									//Calculated speed and langth rating classification
									insertSpeedCategory(t_lane, speed_integer);
									t_length_class = insertLengthCategory(t_lane, gstCurrData.length[t_lane]);

									// It accumulates the speed and length. calculate average per polling cycle.
									gstCenterAccuData.speed[t_lane] += speed_integer;
									gstCenterAccuData.length[t_lane] += gstCurrData.length[t_lane];

									gstIncidentAccuData.speed[t_lane] += speed_integer;									

									#if defined(SUPPORT_LOCAL_TRAFFIC_DATA)
									gstLocalAccuData.speed[t_lane] += speed_integer;
									gstLocalAccuData.length[t_lane] += gstCurrData.length[t_lane];
									#endif

									//Record the event of one vehicle.
									lane_evt_flg[t_lane] = EVENT_CREATED;
									invers_st.gRT_Reverse[t_lane] = FALSE;
									realtime_data_valid[t_lane] = EVENT_CREATED;

									//accumulates the volumn count and occ
									lane_volume[t_lane]++;
									occupy[t_lane] += loop_gap;
									
									// kict real data save
					//				if( Start_Stop_Flag == 1)
					//					kict_saveRealtimeData(t_lane);

									//record realtime data
									saveRealtimeData(t_lane);

									saveRealtimeDatatoNand(t_lane, FALSE); //by capi 2014.08

									if(rt_web_flag == RTD_ON) saveRealtimeDatatoText(t_lane, FALSE);
									
									gCountVehicle++;
									if (gCountVehicle > 10000) gCountVehicle = 0;									
									
									sendRealtimeIndivDataPacket(t_lane, FALSE); //by capi 2014
									//printf("make traffic -- test floating error\n");
									
								}
							}
					}
				}
			}			
		}


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
			if (term_stat == DEBUG_MONITOR_MODE) fprintf(stdout," CRC error 3");
			fprintf(stdout,"\n\rCRC ERROR Rev size : %d \r\n", dataLenPortA);
			dataLenPortA = 0;
			smRecvPortA = MODEM_WAIT_START_1;				
			return;
		}

		recvDataPortA.Length[recvDataPortA.End] = dataLenPortA;

		fprintf(stdout,"\n\rRev size : %d \r\n", dataLenPortA);

		#ifdef DEBUG_SYS_COMM
		fprintf(stdout,"\n\rRX2 : ");

		for(j=0;j<dataLenPortA;j++)
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
