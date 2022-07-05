#ifndef __TCPIP_H__
#define __TCPIP_H__

//#include "stdalsp.h"
#define DIOM_MSK				1
#define ROM_MSK					12
#define RAM_MSK					13

#define SM_VDS_CLIENT_INIT				0   // by capi 2014.09.14
#define SM_VDS_CLIENT_TRY_CONNECT		1
#define SM_VDS_CLIENT_CONNECTED			2
#define SM_VDS_CLIENT_DEVID_SEND		3
#define SM_VDS_CLIENT_ABORTED			4
#define SM_VDS_CLIENT_TIMEOUT			5
#define SM_VDS_CLIENT_CLOSEED			10


//Result code
#define TCP_SYSTEM_ERROR			1	
#define TCP_OPCODE_ERROR			2
#define TCP_CSN_ERROR				3
#define TCP_DATA_LENGTH_ERROR			4
#define TCP_FIELD_VALUE_ERROR		5
#define TCP_DATA_NOT_READY		6
#define TCP_TRAN_TIMEOUT			7

/*
struct vdsd_state {
	char state;				// state machine
	
	//u16_t send_len;
	//u16_t send_ptr;			// 보낼 데이터 포인터.
	//u16_t send_left;		// 보낼 데이터 개수.
	unsigned long send_ptr;			// 보낼 데이터 포인터.		2013.01.24 by capi
	unsigned long send_left;		// 보낼 데이터 개수.

	//unsigned char timer;
	unsigned int timer;		// bug fix by jwank 091104
	short wait;	// ms		// 	
	int  con_state; 
};

struct vdsd_state vds_st;
*/
uint16 sys_valid_data;

int tcpip_app();

#endif /* __TCPIP_H__ */

