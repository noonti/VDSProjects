#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include "systypes.h" 
#include "mmi_protocol.h" 

#define GPIO_OUTPUT 0
#define GPIO_INPUT 1
#define GPIO_HIGH  1
#define GPIO_LOW  0

#define GPIO_NONE  "none"
#define GPIO_FALLING "falling"
#define GPIO_RISING "rising"
#define GPIO_BOTH  "both"

#define SPCLK 		gpio45 
#define MOSI 		gpio42 
#define MISO 		gpio43 
#define CSN 		gpio46 

#define LOW 	"0"
#define HIGH 	"1"
#define SYSFS_GPIO_DIR "/sys/class/gpio"

#define MAX_BUF 64

//typedef  unsigned char	uint8;

FILE *fp;
char value[10];

int fd_miso, fd_mosi, fd_clk,fd_en;
int dtime = 10;

int spi_write_flag;

delay_time(int num)
{
        int i,j,a;

        for(i=0;i<num;i++)
                for(j=0;j<1000;j++)
                        ;
}

int gpio_read(int fd, unsigned int *val)
 {
     int ret;
     char ch;

     lseek(fd, 0, SEEK_SET);

     ret = read(fd, &ch, 1);
     if (ret != 1) {
         fprintf(stderr, "Can't read GPIO  pin: %s\n",  strerror(errno));
         return ret;
     }

     if (ch != '0')
         *val = GPIO_HIGH;
     else
         *val = GPIO_LOW;

     return 0;
 }

pseudo_spi_stream_write(uint8 *src, uint8 len)
{
	uint8 i, j, bit_msk;


	spi_write_flag = 1;

    write(fd_en, LOW, 2);
    delay_time(dtime);
    delay_time(dtime);

	for(j=0; j<len; j++)
	{
        	for(i=0, bit_msk=1;i<8;i++, bit_msk<<=1){
                	write(fd_clk, HIGH, 2);
                	delay_time(dtime);

                	if( src[j] & bit_msk)
                        	write(fd_mosi, HIGH, 2);
                	else
                        	write(fd_mosi, LOW, 2);

                	delay_time(dtime);

                	write(fd_clk, LOW, 2);
                	delay_time(dtime);
		}
	}

	
    delay_time(dtime);
    delay_time(dtime);
    write(fd_clk, HIGH, 2);
    write(fd_en, HIGH, 2);
    delay_time(20);
    delay_time(20);

	//delay_time(500);
	spi_write_flag = 0;
}

uint8 pseudo_spi_stream_read(uint8 *p_buf)
{
	char ch;
	uint8 i, j, buf_idx=0, bit_msk,  tmp;
	uint8 opcode, index, data_size=0;

	if( spi_write_flag == 1) {
		fprintf(stdout,"read return\n");
		return;
	}

    write(fd_clk, HIGH, 2);
    delay_time(dtime); 

    write(fd_en, LOW, 2);
    delay_time(dtime); 

	for (i=0, opcode=0, bit_msk=1; i<8; i++, bit_msk<<=1)
	{
        write(fd_clk, LOW, 2);
        delay_time(dtime);

    	lseek(fd_miso, 0, SEEK_SET);
     	read(fd_miso, &ch, 1);

		if(ch =='1')
			opcode |= bit_msk;

                write(fd_clk, HIGH, 2);
                delay_time(dtime);
	}

    delay_time(dtime);
    // write(fd_en, HIGH, 2);
    delay_time(dtime);

	p_buf[buf_idx++] = opcode;

	switch(opcode)
	{
		case OP_INITIALIZED :
			data_size = 0;
			break;
		case OP_ALIVE_MMI :
			data_size = 0;
			break;
		case OP_TIME_CHG :
			data_size = 7;
			break;
		case OP_FLOW_CTRL :
			data_size = 1;
			break;
		case OP_VDS_BPS :
			data_size = 1;
			break;
		case OP_SYNC_TIME :
			data_size = 1;
			break;
		case OP_MAX_LOOP :
			data_size = 2;
			break;
		case OP_LOOP_GAP :
			data_size = 1;
			break;
		case OP_ID_ADDR :
			data_size = 4;
			break;
		case OP_NORMAL_MODE :
			data_size = 0;
			break;
		case OP_POLLING_MODE :
			data_size = 1;
			break;
		case OP_REALTIME_MODE :
			data_size = 2;
			break;
		case OP_REQ_TIME :
			data_size = 0;
			break;
		case OP_REQ_TEMP :
			data_size = 2;
			break;
		case OP_REQ_PASS :
			data_size = 4;
			break;
		case OP_CHG_IP_IP_ADDR:
			data_size = 4;
			break;
			
		case OP_CHG_IP_GATEWAY_IP:
			data_size = 4;
			break;
			
		case OP_CHG_IP_SUBN_MSK:
			data_size = 4;
			break;
			
		case OP_CHG_IP_SERVER_IP:
			data_size = 4;
			break;
			
		case OP_CHG_IP_SERVER_PORT:
			data_size = 2;
			break;
			
		case OP_CHG_IP_STATION_NUM:
			data_size = 4;
			break;
			
		case OP_CHG_IP_TUNNEL_ID:
			data_size = 2;
			break;
			
		case OP_CHG_IP_CONTR_ID:
			data_size = 2;
			break;
		default :
			
			#if 0
			fprintf(stdout," Invalid opcode : 0x%02x\n\r", opcode);
			#endif

			/*DELAY_SPI_EN;
			at91_pio_write (&PIO_DESC, SPI_EN, PIO_SET_OUT);
			DELAY_SPI_EN;
			*/
       			delay_time(dtime);
       			write(fd_en, HIGH, 2);
       			delay_time(dtime);

			return 0;	// 함수에서 그대로 빠져나간다.
			break;		
	}

	// Data
	for (j=0; j<data_size; j++)
	{
        //	write(fd_clk, HIGH, 2);
        //	delay_time(dtime); 

        //	write(fd_en, LOW, 2);
        //	delay_time(dtime); 
		for (i=0, tmp=0, bit_msk=1; i<8; i++, bit_msk<<=1)
		{
                	write(fd_clk, LOW, 2);
                	delay_time(dtime);

                	lseek(fd_miso, 0, SEEK_SET);
                	read(fd_miso, &ch, 1);

                	if(ch =='1')
                        	tmp |= bit_msk;

                	write(fd_clk, HIGH, 2);
                	delay_time(dtime);

		}
	//	fprintf(stdout,"read  p_buf[%d] = %d\t", j, tmp);

		p_buf[buf_idx++] = tmp & 0xff;
/*
        	delay_time(dtime);
        	write(fd_en, HIGH, 2);
        	delay_time(dtime);
*/
	}
//	fprintf(stdout,"\n");

       	delay_time(dtime);
       	write(fd_en, HIGH, 2);
       	delay_time(dtime);


	return buf_idx;
}

//main()
initPseudoSPI()
{
	uint8	data = 's';
	static uint8 send_index = 0;
	uint8 tmp_buff[50], tx_buf[20], tmp_size =0, opcode,oopcode;
	int i,tmpv, dlane;
	uint8 count=0;
	int port;
	
	fprintf(stdout,"Initialize MMI SPI\n");
	fd_miso = open("/sys/class/gpio/gpio43/value", O_RDONLY);
	fd_mosi = open("/sys/class/gpio/gpio42/value", O_WRONLY);
	fd_clk = open("/sys/class/gpio/gpio45/value", O_WRONLY);
	fd_en = open("/sys/class/gpio/gpio46/value", O_WRONLY);
	if(fd_miso < 0){ 
		fprintf(stdout,"error\n");
		exit(0);
	}

	write(fd_clk, LOW, 2);
	delay_time(dtime);
	write(fd_en, LOW, 2);
	delay_time(dtime);
	write(fd_mosi, LOW, 2);
	delay_time(dtime);

	tx_buf[0] = OP_CPM_STARTED;
	pseudo_spi_stream_write( tx_buf, 1);

/*	while(1){

		tmp_size = pseudo_spi__stream_read(tmp_buff);
		if( tmp_size != 0){

		opcode = tmp_buff[0];
		
		switch( opcode){
			case OP_INITIALIZED :
				fprintf(stdout,"Initialized = 0x%x, %c\n", tmp_buff[0], opcode);
				tx_buf[0] = OP_REV_ACK;
				tx_buf[1] = opcode;
				pseudo_spi_stream_write(tx_buf, 2);
				break;
			case OP_SYNC_TIME:
				tx_buf[0] = OP_REV_ACK;
				tx_buf[1] = opcode;
				pseudo_spi_stream_write(tx_buf, 2);
				fprintf(stdout,"Synctime = %d\n", tmp_buff[1]);
				break;
			case OP_TIME_CHG:
				tx_buf[0] = OP_REV_ACK;
				tx_buf[1] = opcode;
				pseudo_spi_stream_write(tx_buf, 2);
				for(i=0;i<7;i++)
					fprintf(stdout,"%d\t", tmp_buff[i+1]);
				fprintf(stdout,"\n");
				break;
			}
			oopcode = opcode;
		}

	}

	close(fd_miso);
	close(fd_mosi);
	close(fd_clk);
	close(fd_en);
*/
}


