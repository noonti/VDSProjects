#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/epoll.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <locale.h>
#include <time.h>
#include <sys/time.h>

#include "systypes.h"
//#include "crc-16.h"
#include "tcpip.h"
#include "ftms_cpu.h"
#include "serial.h"



#include "kict_utils.h"
#include "kict_protocol.h"





int print_hexa(uint8 *packet, int frameLen)
{
	int i;
	printf("******* packet %d bytes received  *******\n", frameLen);
	for (i = 0; i < frameLen; i++)
	{
		printf("0x%02X ", packet[i]);
	}
	printf("\n", packet[i]);
	printf("*****************************************\n");
	return 1;
}

