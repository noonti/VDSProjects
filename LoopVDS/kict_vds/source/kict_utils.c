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

	/*printf("******* packet %d bytes received  *******\n", frameLen);
	for (i = 0; i < frameLen; i++)
	{
		printf("0x%02X ", packet[i]);
	}
	printf("\n", packet[i]);
	printf("*****************************************\n");*/
	return 1;
}


uint8 byte2bcd(uint8 value)
{
	uint8 result = 0;
	result = (uint8)(((value / 10) << 4) + value % 10);
	return result;
}


uint8 bcd2byte(uint8 value)
{
	uint8 result = 0;
	result = (uint8)((value & 15) + (value >> 4) * 10);


	//printf("bcd2byte ..value=0x%02x  result=0x%02x \n",value,result);

	return result;
}

int date2bcd(uint8 * result, SYSTEMTIME *time)
{
	int index = 0;

	result[index++] = byte2bcd(time->wYear / 100);
	result[index++] = byte2bcd(time->wYear % 100);
	result[index++] = byte2bcd(time->wMonth);
	result[index++] = byte2bcd(time->wDay);
	result[index++] = byte2bcd(time->wHour);
	result[index++] = byte2bcd(time->wMinute);
	result[index++] = byte2bcd(time->wSecond);
	result[index++] = byte2bcd((time->wMilliseconds/10)%100);

	return index;
}

int bcd2date(uint8 * time, SYSTEMTIME * result)
{
	int index = 0;
	result->wYear = bcd2byte(time[index++]) *100;
	result->wYear += bcd2byte(time[index++]);

	result->wMonth = bcd2byte(time[index++]);
	result->wDay = bcd2byte(time[index++]);

	result->wHour = bcd2byte(time[index++]);

	result->wMinute = bcd2byte(time[index++]);

	result->wSecond = bcd2byte(time[index++]);

	result->wMilliseconds = bcd2byte(time[index++])*10;

	return index;
      
}
