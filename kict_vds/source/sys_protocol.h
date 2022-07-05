#ifndef __SYS_PROTOCOL_H__
#define __SYS_PROTOCOL_H__

typedef __packed struct {
	uint8 dle;
	uint8 etx;
	uint8 crc[2];
} PACKET_TAIL;
#endif	// __SYS_PROTOCOL_H__

