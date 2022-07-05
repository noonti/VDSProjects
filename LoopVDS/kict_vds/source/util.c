#include <stdio.h>
#include <string.h>
#include "systypes.h"
//#include "util.h"

static const unsigned long _dec_unit_[] ={ 10, 100, 1000, 10000, 100000, 1000000, 10000000};

void num2BcdStr(char *pDst, uint num, uchar bitmsk)
{
	uint8 i, j;

	if ( num >= 0x0000ffff || bitmsk > 6 ) return;

	for(i=bitmsk-1, j=0; i>0; i--, j++)
	{
		pDst[j] = ((num/_dec_unit_[i-1]) - (num/_dec_unit_[i])*10) % 10;
		pDst[j] += '0';
	}

	pDst[j] = (num%_dec_unit_[0]) + '0';
}

uint32 round_for_occupy(uint32 org_occ, uint8 pcs)
{
	if (pcs == 2)
	{
		org_occ += 5;
		org_occ /= 10;
	}
	else //if (pcs == 0)
	{
		org_occ += 500;
		org_occ /= 1000;
	}

	return org_occ;
}

int netMacStr2Bin(char *ascii, uchar *binary)
{
	int	i, digit;
	char *acpy;

	acpy = ascii;
	for(i=0;i<6;i++)
	{
		digit = (int)strtol(acpy,&acpy,16);
		if (((i != 5) && (*acpy++ != ':')) ||
			((i == 5) && (*acpy != 0)) ||
			(digit < 0) || (digit > 255))
		{
			return(-1);
		}
		
		binary[i] = (uchar)digit;
	}
	
	return(0);
}

int netIpStr2Bin(char *ascii, uchar *binary)
{
	int	i, digit;
	char *acpy;

	acpy = ascii;
	for(i=0;i<4;i++)
	{
		digit = (int)strtol(acpy,&acpy,10);
		if (((i != 3) && (*acpy++ != '.')) ||
			((i == 3) && (*acpy != 0)) ||
			(digit < 0) || (digit > 255))
		{
			return(-1);
		}
		
		binary[i] = (uchar)digit;
	}
	
	return(0);
}

int netBin2IpStr(uchar *binary, char *ascii)
{
	char tmpStr[15+5];

	sprintf(tmpStr, "%03d.%03d.%03d.%03d", \
		binary[0], binary[1], binary[2], binary[3]);
		
	memcpy(ascii, tmpStr, 15);
}


static uint8 hex2num( char *str, uint32 *ret_num )
{
	long hex;
	int hexlen;
	int i;
	unsigned char medium[256];
	char c;

	hex = 0;
	hexlen = strlen(str);
	for ( i=0; i<hexlen; i++ ) {
		c = str[i];
		if ( (c>='0' && c<='9') ) {
			medium[i] = c-'0';
		}
		else if ( (c>='a' && c<='f') ) {
			medium[i] = c-'a'+0xa;
		}
		else if ( (c>='A' && c<='F') ) {
			medium[i] = c-'A'+0xa;
		}
		else return FALSE;
	}

	for ( i=0; i<hexlen; i++ ) {
		hex |= ( (medium[i] & 0xff) << (4*(hexlen-i-1)) );
	}

	*ret_num = hex;
	return TRUE;
}



static BOOL is_digit( char c )
{
        if( (c>='0' && c<='9') )
                return TRUE;
        return FALSE;
}

static BOOL is_num( char *str )
{
        int len = strlen(str);
        int i;
        for (i=0;i<len;i++) {
                if ( !is_digit( str[i] ) )
                        return FALSE;
        }
        return TRUE;
}

BOOL str2num( char *str, int *retv )
{
	char *pstr = str;
	int len, i, digit;
	uint32 num = 0;

	
	if (!is_num(pstr))
	{
				
		if (pstr[0]!='0' || pstr[1]!='x') return FALSE;
		
		// hex to dec
		pstr+=2;
		if ( !hex2num(pstr, &num) ) return FALSE;
		
		*retv = (int) num;
		return TRUE;
	}
	
	len = strlen(pstr);
	for (i=0; i<len; i++) {
		num *= 10;
		digit = pstr[i] - '0';
		num += digit;
	}

	*retv = (int) num;
	return TRUE;
}

