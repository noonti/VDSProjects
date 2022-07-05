#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <string.h>
#include <linux/i2c.h>
#include <linux/i2c-dev.h>

#include "ftms_cpu.h"
#include "serial.h"

/////////////// ADC /////////////////////////////
#define ADC_RESOLUTION                          (8)     // 8-bit
#define TEMP_SENSOR_REF                         (4096)  // mV
#define ZERO_TEMP_VOLTAGE                       (1200)  // mV
#define ADC_VOL_PER_BIT                         (TEMP_SENSOR_REF / (1 << ADC_RESOLUTION))
#define ZERO_TEMP_ADC_VAL                       (ZERO_TEMP_VOLTAGE / ADC_VOL_PER_BIT)

#define MAX_1038_39_CH              (12)
#define MAX_ADC_CH                  MAX_1038_39_CH
#define TEMP_CH_0                   (6)
#define TEMP_CH_1                   (4)
#define _OPERATE_               1
#define _NOT_OPERATE_           0

// ADC channel define.
#define POWER_VOL_0                 (2)         //2011.07.06 by capidra 0->2
#define POWER_VOL_1                 (0)         //2011.07.06 by capidra 2->0
#define TEMP_CH_0                   (6)
#define TEMP_CH_1                   (4)

#define turnOnFAN()
#define turnOffFAN()
#define turnOnHeater()
#define turnOffHeater()

#define turnOnPower1()         
#define turnOffPower1()
#define turnOnPower2()
#define turnOffPower2()

//#define ADC_DEBUG 

int i2c_adc;
void initADC(void)
{

        fprintf(stdout,"External ADC init\n");
        i2c_adc = open("/dev/i2c-1",O_RDWR);
        if(i2c_adc< 0)
        {
                 fprintf(stdout,"i2c-1 open fail");
                 //exit(1);
        }

        if(ioctl(i2c_adc,I2C_SLAVE, 0x65)<0)
        {
                 fprintf(stdout,"rtc dev addr set error \n");
                 //exit(1);
        }
}
u32 conv_Adc2Vol(u32 adc_val, u16 ref_vol, u8 adc_bits)
{
    u32 adc_resol = (1 << adc_bits);
    u32 adc_min_v = ref_vol / adc_resol;

    return (adc_val * adc_min_v);
}

void readADC(u16 num, u8 *pDptr)
{
        int i, temp[2];
        unsigned char adc[8];

        //read RTC
        adc[0] = 0xc2;
        adc[1] = 0x0F;
        write(i2c_adc,adc,2);
        read(i2c_adc,adc,0x10);
        //      sleep(1);
//      for(i=0;i<0x10;i++)
 //             fprintf(stdout,"<%d-%02x>\n",i,adc[i]);

        pDptr[TEMP_CH_0] = (int)adc[7];
        pDptr[TEMP_CH_1] = (int)adc[6];
}

void dia_Temp_Status()
{
    u8 adc_val[MAX_ADC_CH], i, num = 8;
    readADC(num, adc_val);                   //AD Convertor Read

    dia_status.temp[0] = (int)adc_val[TEMP_CH_0] - ZERO_TEMP_ADC_VAL;       
    dia_status.temp[1] = (int)adc_val[TEMP_CH_1] - ZERO_TEMP_ADC_VAL;   
}

void checkTemp_n_PowerTask()
{
    u8 adc_val[MAX_ADC_CH], i, num = 8;
    int temp, delta_t;

    SYSTEMTIME st;

    // 1 minute : 60000/1000 = 60 sec
    if (monitor_counter >= 30000)
    {
        monitor_counter = 0;

        //----------------------------------------- read a/d convertor ------------------------------------//
        //readADC(num, &adc_val[0]);
        readADC(num, adc_val);                  //AD Convertor Read
#ifdef ADC_DEBUG 
        fprintf(stdout, " [ Read ADC ]");
        for(i=0; i<num; i++)
            fprintf(stdout, "Ch%d) => %d [0x%02X] ", i, adc_val[i], adc_val[i]);
        fprintf(stdout,"\r\n");
#endif
        #if defined(TEMPERATURE_MONITORING)
        g_currSysStatus.temper[TEMPER1] = (int)adc_val[TEMP_CH_0] - ZERO_TEMP_ADC_VAL;
        g_currSysStatus.temper[TEMPER2] = (int)adc_val[TEMP_CH_1] - ZERO_TEMP_ADC_VAL;

#ifdef ADC_DEBUG 
        fprintf(stdout,"Operated Sensor : %d, Current Temper : %d-%d\n"\
            , g_currSysStatus.main_sensor+1, g_currSysStatus.temper[TEMPER1] , g_currSysStatus.temper[TEMPER2] );

        fprintf(stdout,"Setting Temper : %d %d\n", gstSysConfig.temper[0] , gstSysConfig.temper[1] );
#endif

        temp = g_currSysStatus.temper[g_currSysStatus.main_sensor];

        if(g_currSysStatus.timecnt>60)
        {
            for(i=0;i<23;i++) g_currSysStatus.temp_24hour_buf[23-i] = g_currSysStatus.temp_24hour_buf[22-i];
            g_currSysStatus.temp_24hour_buf[0] = temp;
            
            g_currSysStatus.timecnt = 0;
        }

        g_currSysStatus.timecnt++;

        sendSysConfigMsgToMMI();
        //------------------------------------ troubleshooting ------------------------------------//       
        if((g_currSysStatus.temp_error > 5) && (g_currSysStatus.main_sensor == TEMPER1))
        {
            g_currSysStatus.main_sensor = TEMPER2;
            g_currSysStatus.temp_status = g_currSysStatus.temp_status|0x02;

            g_currSysStatus.temp_init = 0;
            g_currSysStatus.temp_error = 0;

            //fprintf(stdout,"sensor1 ERROR3 ");
            //send status to MMI
        }
        else if((g_currSysStatus.temp_error > 5) && (g_currSysStatus.main_sensor == TEMPER2))
        {
            g_currSysStatus.main_sensor = 0xFF;
            g_currSysStatus.temp_status = g_currSysStatus.temp_status|0x01;

            //turnOffFAN();
            //turnOffHeater();

            write(fan_fd, "0", 1);
            write(heater_fd, "0", 1);

            //fprintf(stdout,"sensor2 ERROR3 ");
            //send status to MMI

            return;    
        } //all sensor  fault condition

        //------------------------- temperature sensor  fault condition------------------------------------//
        if((temp > 45) || (temp<-12)&& g_currSysStatus.main_sensor == TEMPER1)
        {
            g_currSysStatus.temp_error++;

            //fprintf(stdout,"Wrong Temper Sensor1, Current Temper %d ", temp);
            //display MMI 
            //sendSysConfigMsgToMMI();
            return;
        }
        else if((temp > 45) || (temp<-12)&& g_currSysStatus.main_sensor == TEMPER2)
        {
            g_currSysStatus.temp_error++;
            //sendSysConfigMsgToMMI();

            //fprintf(stdout,"Wrong Temper Sensor2, Current Temper %d ", temp);
            //display MMI  

            return;     
        }//all sensor  fault condition

        //----------------------------------- temperature sensor diagnosis 1 : No changes data during the day------------------------------------//
        if((sys_time.wHour == 3)&&(sys_time.wMinute<3))
        {
            g_currSysStatus.temp_buf_1day[0] = temp;
        }
        if((sys_time.wHour == 15)&&(sys_time.wMinute<3))
        {
            g_currSysStatus.temp_buf_1day[1] = temp;
        }

        if(g_currSysStatus.temp_save_time<1445)     //24*60(24시간) + 5
        {
            g_currSysStatus.temp_save_time++;
        }
        else
        {
            if((sys_time.wHour == 3)&&(sys_time.wMinute<3))
            {
                delta_t = abs( g_currSysStatus.temp_buf_1day[1] - g_currSysStatus.temp_buf_1day[0]);

                if(delta_t == 0)
                {
                    g_currSysStatus.temp_error++;
                    fprintf(stdout,"Wrong Temper Sensor%d, Oneday Buf1 : %d Buf2 : %d "\
                                   , g_currSysStatus.main_sensor+1, g_currSysStatus.temp_buf_1day[0], g_currSysStatus.temp_buf_1day[1]);
                    return;
                }
            }

            if((sys_time.wHour == 15)&&(sys_time.wMinute<3))
            {
                delta_t = abs( g_currSysStatus.temp_buf_1day[1] - g_currSysStatus.temp_buf_1day[0]);

                if(delta_t == 0)
                {
                    g_currSysStatus.temp_error++;
                    fprintf(stdout,"Wrong Temper Sensor%d, Oneday Buf1 : %d Buf2 : %d "\
                                   , g_currSysStatus.main_sensor+1, g_currSysStatus.temp_buf_1day[0], g_currSysStatus.temp_buf_1day[1]);
                    return;
                }
            }
        }
        //------------------------- saving temperature buffer ------------------------------------//
        g_currSysStatus.temp_buf = temp;

        g_currSysStatus.temp_error = 0;

        if(g_currSysStatus.main_sensor  == 0xFF)
        {
            //turnOffFAN();
            //turnOffHeater();

            write(fan_fd, "0", 1);
            write(heater_fd, "0", 1);

#ifdef ADC_DEBUG 
            fprintf(stdout,"sensor1_2 ERROR,  Relay1 state : %d,  Relay2 state : %d",
                        g_currSysStatus.relay_state[0], g_currSysStatus.relay_state[1]);
#endif
            return;    //all sensor  fault condition
        }
        //------------------------------------------- operating relay------------------------------------//
        if(temp>=gstSysConfig.temper[0])
        {
            if(g_currSysStatus.relay_sw_delay[0] <5)        //2011.11.23 by capidra preventing malfunction
            {
                g_currSysStatus.relay_state[0] = RELAY_ON;
                system_status.fan_operate = _OPERATE_;
                g_currSysStatus.relay_sw_delay[0]++;
                g_currSysStatus.relay_sw_delay[1] =0;
                //turnOnFAN();
                write(fan_fd, "1", 1);

#ifdef ADC_DEBUG 
                fprintf(stdout,"turn on Fan ");
#endif
            }
        }
        else if(temp<(gstSysConfig.temper[0]-1))
        {
            if(g_currSysStatus.relay_sw_delay[1]<5)
            {
                g_currSysStatus.relay_state[0] = RELAY_OFF;
                system_status.fan_operate = _NOT_OPERATE_;
                g_currSysStatus.relay_sw_delay[1]++;
                g_currSysStatus.relay_sw_delay[0] =0;
                //turnOffFAN();
                write(fan_fd, "0", 1);


#ifdef ADC_DEBUG 
                fprintf(stdout,"turn off Fan ");
#endif
            }
        }


        if(temp<=gstSysConfig.temper[1])
        {
            if(g_currSysStatus.relay_sw_delay[2] <5)        //2011.11.23 by capidra preventing malfunction
             {
   		        g_currSysStatus.relay_state[1] = RELAY_ON;
                system_status.heater_operate = _OPERATE_;
                g_currSysStatus.relay_sw_delay[2]++;
                g_currSysStatus.relay_sw_delay[3] =0;
                //turnOnHeater();
                write(heater_fd, "1", 1);

                //fprintf(stdout,"on system_status.heater_operate = %d ", system_status.heater_operate);

#ifdef ADC_DEBUG 
                fprintf(stdout,"turn on Heater ");
#endif
            }
        }
        else if(temp>(gstSysConfig.temper[1]+1))
        {
            if(g_currSysStatus.relay_sw_delay[3]<5)
            {
                g_currSysStatus.relay_state[1] = RELAY_OFF;
                system_status.heater_operate = _NOT_OPERATE_;
                g_currSysStatus.relay_sw_delay[3]++;
                g_currSysStatus.relay_sw_delay[2] = 0;
                //turnOffHeater();
                write(heater_fd, "0", 1);

                //fprintf(stdout,"off system_status.heater_operate = %d ", system_status.heater_operate);

#ifdef ADC_DEBUG 
                fprintf(stdout,"turn off Heater ");
#endif
            }

        }

        //fprintf(stdout,msg_system_monitor, " Temperature[0]: %d", g_currSysStatus.temper[0]);
        //fprintf(stdout,msg_system_monitor, " Temperature[1]: %d", g_currSysStatus.temper[1]);
        #endif

        //#if defined(POWER_VOLTAGE_MONITORING)
       
        g_currSysStatus.pwr_vol[0] = (uint8)((adc_val[POWER_VOL_0] *16/100)* 25/15);     //2011.07.06 by capidra
        g_currSysStatus.pwr_vol[1] = (uint8)((adc_val[POWER_VOL_1] *16/100)*128/16);

        //fprintf(stdout,"POWER_VOL 1 : %d\n", g_currSysStatus.pwr_vol[0]);
        //fprintf(stdout,"POWER_VOL 2 : %d\n", g_currSysStatus.pwr_vol[1]);
       
        //#endif

        //#if defined(FAN_HEATER_AUTO_CONTROL)
        //fprintf(stdout,msg_fan_heater_control, " fan test");


    }
}

