/*********************************************************************************
/* Filename    : I2c7segment.c
 * Description :
 * Author      : 
 * Notes       :
 *********************************************************************************/
#include <sys/types.h>
#include <sys/stat.h>
#include <sys/ioctl.h>
#include <sys/time.h>
#include <sys/types.h>
#include <fcntl.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <linux/i2c.h>
#include <linux/i2c-dev.h>

#define _POSIX_SOURCE 1 /* POSIX compliant source */

#define SEG_ON		0x0
#define SEG_OFF		0x1
#define SEG_BLK		0x2
#define	SEG_IBLK	0x3

typedef struct
{
	unsigned char psc0;
	unsigned char pwm0;
	unsigned char psc1;
	unsigned char pwm1;
} I2C_SEG_CNTL;

unsigned char segval[4];

unsigned short fontDigit[10] = { 0x5000, 0x5541, 0x4410, 0x4500, 0x4141, 0x4104, 0x4004,0x5140,0x4000,0x4100}; 

void i2cSegCntl(I2C_SEG_CNTL *cntl)
{
	int i2c;
	unsigned char ledcntl[17];
	
	i2c = open("/dev/i2c-1",O_RDWR);
	if(i2c < 0)
	{
		fprintf(stdout,"i2c-1 open fail");
		exit(1);
	}
	ioctl(i2c,I2C_SLAVE, 0x60);

	ledcntl[0] = 0x12;	// auto increment & LS0
	ledcntl[1] = cntl->psc0;
	ledcntl[2] = cntl->pwm0;
	ledcntl[3] = cntl->psc1;
	ledcntl[4] = cntl->pwm1;

	
	write(i2c,ledcntl,5);
	close(i2c);
}

void i2cSegOut(void)
{
	int i2c;
	unsigned char ledcntl[5];
	
	i2c = open("/dev/i2c-1",O_RDWR);
	if(i2c < 0)
	{
		fprintf(stdout,"i2c-1 open fail");
		exit(1);
	}
	ioctl(i2c,I2C_SLAVE, 0x60);

	ledcntl[0] = 0x16;	// auto increment & LS0
	ledcntl[1] = segval[0];
	ledcntl[2] = segval[1];
	ledcntl[3] = segval[2];
	ledcntl[4] = segval[3];

	
	write(i2c,ledcntl,5);
	close(i2c);
}

void i2cSgmtSet(int pos, int mode)
{
	int valPos, bitPos;
	unsigned char ledcntl[2];
	
	if(pos > 15) return;
	if(pos < 0) return;
	
	valPos = pos>>2;
	bitPos = (pos&0x03)<<1;
	fprintf(stdout,"pos = %d, valPos = %d, bitPos = %d, segval = %02X ",pos,valPos,bitPos,segval[valPos]);
	segval[valPos] &= ~(0x3 << bitPos);
	segval[valPos] |= (mode & 0x03) << bitPos;
	fprintf(stdout," -> %02X\n",segval[valPos]);
}

void i2cSegDigit(int pos, int val)
{
	unsigned char lval0,lval1;
	lval0 = (unsigned char)(fontDigit[val] );
	lval1 = (unsigned char)(fontDigit[val] >> 8);
	if(pos)
	{
		segval[0] = lval0;
		segval[1] = lval1;
	}
	else
	{
		segval[2] = lval0;
		segval[3] = lval1;
	}
	i2cSegOut();
}

void i2cSegInit(void)
{
	I2C_SEG_CNTL cntl;
	int i;

	cntl.psc0 = 19;
	cntl.pwm0 = 128;
	cntl.psc1 = 38;
	cntl.pwm1 = 192;
	memset(segval,0x55,4);
	i2cSegOut();
	i2cSegCntl(&cntl);	
	i2cSegDigit(0,0);
	i2cSegDigit(1,0);
}