#ifndef __VDS_PARAMETER_H__
#define __VDS_PARAMETER_H__

// Index of parameter
#define IDX_01_LOOP_ENABLE				1
#define IDX_02_SPD_LOOP_DEF				2
#define IDX_03_POLL_CYCLE				3
#define IDX_04_DETECT_THRH				4
#define IDX_05_SPD_CATEGORY				5
#define IDX_06_LEN_CATEGORY				6
#define IDX_07_SPD_ACC_ENB				7
#define IDX_08_LEN_ACC_ENB				8
#define IDX_09_SPD_CALC_ENB				9

#define IDX_10_LEN_CALC_ENB				10
#define IDX_11_SPD_LOOP_DIM				11
#define IDX_12_UP_VOL_LIMIT				12
#define IDX_13_UP_SPD_LIMIT				13
#define IDX_14_UP_LEN_LIMIT				14
#define IDX_15_INCID_THRD				15
#define IDX_16_STUCK_THRD				16
#define IDX_17_LOOP_OSC_THRD			17

#define IDX_18_UNIT_ADDR				18
#define IDX_19_PARAM_RAM_STS			19

#define IDX_20_AUTO_RESYNC				20
#define IDX_21_SIMUL_TEMPL				21

#define IDX_22_SOFT_VERSION				22
#define IDX_23_RESPOND_LEN				23
#define IDX_24_SPARE_PARM				24

#define IDX_25_USER_FOR_LOOP			25
#define IDX_26_USER_FOR_VENHICLE		26
#define IDX_27_USRE_FOR_THRESHOLD		27

////////////////////////////////////////////////////////////

// for CMD_SET_SYSTEM_CONFIG_B, CMD_GET_SYSTEM_CONFIG_B
#define IDX_01_SET_SYSTEM_TIME			1
#define IDX_02_SET_GLOBAL_ADDRESS		2
#define IDX_03_SET_BAUD_OF_VDS_PORT		3
#define IDX_04_SET_FLOW_OF_VDS_PORT		4
#define IDX_05_SET_TOTAL_NUM_OF_LOOP	5
#define IDX_06_LEN_GAP_OF_LOOP			6
#define IDX_07_SET_SYNC_CYCLE			7
#define IDX_08_SET_ALL_SYSTEM_CONFIG	8

#define IDX_09_SET_LOCAL_POLL_TIME		9

#define IDX_10_SET_CONTROLLER_ADDR		10
#define IDX_11_SET_IP_ADDRESS			11
#define IDX_12_SET_SUBNET_MASK			12
#define IDX_13_SET_GATEWAY_IP			13
#define IDX_14_SET_SERVER_IP			14
#define IDX_15_SET_SERVER_PORT			15
//#define IDX_16_SET_TUNNEL_ID			16  //by capi 2014.08
//#define IDX_17_SET_CONTROLLER_ID		17
////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////

// for CMD_SET_SYSTEM_CONFIG_B, CMD_GET_SYSTEM_CONFIG_B
#define IDX_B_01_SET_SYSTEM_TIME		1
#define IDX_B_02_SET_CONTROLLER_ADDR	2
#define IDX_B_03_SET_IP_ADDRESS			3
#define IDX_B_04_SET_SUBNET_MASK		4
#define IDX_B_05_SET_GATEWAY_IP			5
#define IDX_B_06_SET_SERVER_IP			6
#define IDX_B_07_SET_SERVER_PORT		7
#define IDX_B_08_SET_TUNNEL_ID			8
#define IDX_B_09_SET_CONTROLLER_ID		9
#define IDX_B_10_SET_TOTAL_NUM_OF_LOOP	10
#define IDX_B_11_LEN_GAP_OF_LOOP		11
#define IDX_B_12_SET_SYNC_CYCLE			12
#define IDX_B_13_SET_LOCAL_POLL_TIME	13
#define IDX_B_14_SET_ALL_SYSTEM_CONFIG	14
////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////

#define IDX_01_SET_ETH_MAC_ADDR			1
#define IDX_02_SET_NETWORK_IP			2
#define IDX_03_SET_GATEWAY_IP			3
#define IDX_04_SET_SUBNET_MASK			4
#define IDX_05_SET_SERVER_IP			5
#define IDX_06_SET_SERVER_PORT			6
#define IDX_07_LEN_STATION_NUM			7
////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////
// Flag definition for chk_n_SaveStrIpToNVRAM()
#define FLAG_WRITE_NVRAM					(1<<0)	
#define FLAG_SEND_TO_MMI					(1<<1)	
#define FLAG_SET_IP_STR						(1<<2)	

#define FLAG_NOT_SAVE						(FLAG_SEND_TO_MMI|FLAG_SET_IP_STR)	
#define FLAG_ALL							(FLAG_WRITE_NVRAM|FLAG_SEND_TO_MMI|FLAG_SET_IP_STR)
////////////////////////////////////////////////////////////

// VDS Parameter Structure.
typedef struct VDS_PARAM_DEF {
	// Index 1
	uint8 loop_enable_table[MAX_LOOP_NUM/8];
	// Index 2
	uint8 speed_loop_map[MAX_LANE_NUM][2];
	// Index 3
	uint8 polling_cycle;
	// Index 4
	uint8 num_pluse_present;
	uint8 num_pluse_no_present;
	// Index 5
	uint8 speed_category[SPEED_CATEGORY_NO];
	// Index 6
	uint8 length_category[LENGTH_CATEGORY_NO];
	// Index 7
	uint8 spd_acc_enable;
	// Index 8
	uint8 len_acc_enable;
	// Index 9
	uint8 spd_calc_enable;
	// Index 10
	uint8 len_calc_enable;
	// Index 11
	uint8 spd_loop_dist[MAX_LANE_NUM];
	uint8 spd_loop_diam[MAX_LANE_NUM];	
	// Index 12
	uint8 upper_vol_limit;
	// Index 13
	uint8 upper_spd_limit;
	// Index 14
	uint8 upper_len_limit;
	// Index 15
	uint8 incid_exec_cycle;
	uint8 persist_period;
	uint8 incid_algorithm;
	uint16 K_factor[2];
	uint16 T_value[MAX_LOOP_NUM];
	// Index 16
	uint8 stuck_high_vol;
	uint8 stuck_on_high;
	uint8 stuck_off_high;
	uint8 stuck_low_vol;
	uint8 stuck_on_low;
	uint8 stuck_off_low;	
	// Index 17
	uint8 oscillation_thr;
	// Index 18
	//reserv
	// Index 19
	// Reserv
	// Index 20
	uint8 auto_resync_wait;
	// Index 21
	uint8 data_1_stream_num;
	uint8 simul_1_onoff;
	uint8 sim_1_length;
	uint8 sim_1_speed;
	uint8 sim_1_headway;
	uint8 sim_1_lane_dist;
	uint8 sim_1_loop_diam;
	uint8 data_2_stream_num;
	uint8 simul_2_onoff;
	uint8 sim_2_length;
	uint8 sim_2_speed;
	uint8 sim_2_headway;
	uint8 sim_2_lane_dist;
	uint8 sim_2_loop_diam;
	// Index 22
	// Reserv
	// Index 23
	// Reserv
	// Index 24
	// Reserv
	uint8 loop_divi_table[MAX_LANE_NUM];		//Dual or Single Loop Select  by capidra 2011.06.16
} VDS_PARAM_ST;

uint16 getSysConfigByIndex_B(uint8 index, uint8 *p_config);
uint16 setSysConfigByIndex_B(uint8 index, uint8 *p_config, uint8 save_flg);
int chk_n_SaveStrIpToNVRAM(uint8 index, char *psOrgIP, uint8 *pResultIp, uint8 byFlag);
void setAllSystemConfig_B(uint8 *p_config);

#endif	// __VDS_PARAMETER_H__
