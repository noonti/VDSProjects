#ifndef __PLAT_CONFIG_H__
#define __PLAT_CONFIG_H__

#define ETHERNET_ADDR_STRING		("00:FA:01:00:02:01")

#define SUPPORT_ADC

//#define MMI_PASSWORD

#define MY_IP_ADDR			("192.168.0.31")
#define GATEWAY_ADDR			("192.168.0.1")		
#define SUBNET_MASK			("255.255.255.0")

#define VDS_SERVER_IP			("192.168.0.15")


#define VDS_SERVER_TCP_PORT		(30100)

#if defined(SUPPORT_ADC)
#define FAN_HEATER_AUTO_CONTROL

	#if defined(FAN_HEATER_AUTO_CONTROL)
	   // 온도 모니터링.
	#define TEMPERATURE_MONITORING 

		#if (defined(TEMPERATURE_MONITORING) || defined(POWER_VOLTAGE_MONITORING))
		   #define ADC_CH_TEMP1         (2)
		   #define ADC_CH_TEMP2         (3)
		   #define ADC_CH_5V                    (0)
		   #define ADC_CH_24V           (1)
		#endif
	#endif // #if defined(SUPPORT_ADC)
#endif // #if defined(SUPPORT_ADC)

#define SUPPORT_MAINTENANCE_PROTOCOL

#if defined(SUPPORT_MAINTENANCE_PROTOCOL)

 #define MAINTENANCE_PROTOCOL_TEST_OVER_VDS_PORT		// by jwank eeee

 #define SUPPORT_INDIV_TRAFFIC_DATA_PROTOCOL
 #define SUPPORT_LOCAL_TRAFFIC_DATA_PROTOCOL
 #define SUPPORT_CENTER_TRAFFIC_DATA_PROTOCOL

 #define SUPPORT_LOCAL_TRAFFIC_DATA
#endif


#endif // #ifndef __PLAT_CONFIG_H__

