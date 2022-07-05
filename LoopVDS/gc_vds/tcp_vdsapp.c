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
#include "serial.h"
#include "tcp_vdsapp.h"
//#include "User_term.h"

#define SM_VDS_CLIENT_INIT				0
#define SM_VDS_CLIENT_TRY_CONNECT		1
#define SM_VDS_CLIENT_CONNECTED			2
#define SM_VDS_CLIENT_DEVID_SEND		3
#define SM_VDS_CLIENT_ABORTED			4
#define SM_VDS_CLIENT_TIMEOUT			5
#define SM_VDS_CLIENT_CLOSEED			10

 //struct vdsd_state vds_st;

static uint8 currSaveBank, sendRdyBank, currSaveBank_tcp, sendRdyBank_tcp;
static uint32 vdsRT_widx[2], vdsRT_widx_tcp[2];


void initRealTimeVdsBuff()
{
	vdsRT_widx[0] = 0;
	vdsRT_widx[1] = 0;

	currSaveBank = 0;
	sendRdyBank = 1;

	vdsRT_widx_tcp[0] = 0;
	vdsRT_widx_tcp[1] = 0;

	currSaveBank_tcp = 0;
	sendRdyBank_tcp = 1;
//	read_idx_tcp = 0;
}

