PREFIX_DIR	= ../..
#Compiler
#CC	= gcc
CC	= /opt/arm-2009q3/bin/arm-none-linux-gnueabi-gcc
CFLAGS	= -l$(PREFIX_DIR)/include

all:
	$(CC) -c main_proc.c
	$(CC) -c adc.c
	$(CC) -c rtc.c
	$(CC) -c crc-16.c
	$(CC) -c serial.c
	$(CC) -c tcpip_app.c
	$(CC) -c vds_parameter.c
	$(CC) -c saving_data.c
	$(CC) -c util.c
	$(CC) -c tcp_vdsapp.c
	$(CC) -c user_term.c
	$(CC) -c pseudo_spi.c
	$(CC) -c mmi_comm.c
	$(CC) -c maintenance_comm.c
	$(CC) -c I2c7segment.c
	$(CC) -c web_server.c
	$(CC) -c ../yse_protocol.c
	$(CC) -c ../yse_utils.c
	$(CC) -o main_proc adc.o web_server.o I2c7segment.o maintenance_comm.o mmi_comm.o pseudo_spi.o rtc.o tcp_vdsapp.o util.o saving_data.o tcpip_app.o user_term.o serial.o crc-16.o vds_parameter.o yse_protocol.o yse_utils.o main_proc.o -L ../lib -I ../include -pthread
	cp ./main_proc /root/Work/TargetFs/root/am1808

	$(CC) -o netcatcmd netcatcmd.c
	cp ./netcatcmd /root/Work/TargetFs/root/am1808
clean:
	@rm -rf *.exe *.o *~ *.bak $(UNAME)
