#ifndef __VDS_PROTOCOL_H__
#define __VDS_PROTOCOL_H__


#pragma pack(1)
typedef  struct
{
	uint8 grb_addr[2];  //by capi 2014.08
	uint8 ctrl_addr[2];	//by capi 2014.08
	uint8 opcode;
} PACKET_TINY_HEAD;
#pragma pack()

#pragma pack(1)
typedef struct
{
	uint8 dle;
	uint8 stx;
	uint8 grb_addr[2];	//by capi 2014.08
	uint8 ctrl_addr[2];	//by capi 2014.08
	uint8 opcode;
} PACKET_TINY2_HEAD;
#pragma pack()

#pragma pack(1)
typedef  struct {
	uint8 dle;
	uint8 etx;
	uint16 crc16;
} PACKET_TAIL;
#pragma pack()

#pragma pack(1)
typedef  struct
{
	uint8 dle;
	uint8 stx;
	uint8 grb_addr[2];	//by capi 2014.08
	uint8 ctrl_addr[2];	//by capi 2014.08
	uint8 opcode;
	uint16 status;
} PACKET_RESPOND_HEAD;
#pragma pack()

#pragma pack(1)
typedef  struct {
	PACKET_RESPOND_HEAD head;
	PACKET_TAIL tail;
} RESPOND_PACKET;
#pragma pack()

#pragma pack(1)
typedef struct {
	PACKET_TINY_HEAD head;
	uint8 index;
	uint8 param[72];
} DOWN_PARAM_PACKET;
#pragma pack()

#endif	// __VDS_PROTOCOL_H__
