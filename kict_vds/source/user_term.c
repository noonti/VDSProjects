#include <stdio.h>
#include <string.h>
#include <termios.h>
#include <signal.h>
#include <stdlib.h>
#include <fcntl.h>

#include "ftms_cpu.h"
#include "serial.h"
#include "tcpip.h"
#include "user_term.h"
#include "mmi_protocol.h"

#define USER_TERM_PROMPT_STRING         (" VDS>")

#define        TRUE        1
#define        FALSE       0

#define        BEL        0x07
#define        BS         0x7f
#define        LF         0x0a
#define        CR         0x0a
#define        ESC        0x1b
#define        DEL        0x7f

#define         MAX_USER_TERM_LINE      100

static uint16 cntNumRtData=1;

//USER_TERM_STAT term_stat;

static struct termios initial_settings, new_settings;
static int peek_character = -1;

static char user_term_buff[MAX_USER_TERM_LINE];
static int usertermcnt = 0;
static int peekch = -1;
static int signalch = 0;

//////////////////////////////////////////////////////////////////
/*==================
 * Inline functions
 =================*/

static __inline boolean is_xdigit( char c )
{
   	 if( (c>='0' && c<='9') || (c>='a' && c<='f') || (c>='A' && c<='F') )
       		 return TRUE;
    	return FALSE;
}

static __inline boolean is_hex( char *str )
{
   	 int len = strlen(str);
   	 int i;
    	for (i=0;i<len;i++) {
#if 0
       		 if ( !isxdigit( str[i] ) )
#else
        	if ( !is_xdigit( str[i] ) )
#endif
            return FALSE;
    	}
    	return TRUE;
}


static __inline boolean is_digit( char c )
{
   	 if( (c>='0' && c<='9') )
       		 return TRUE;
   	 return FALSE;
}

static __inline boolean is_num( char *str )
{
   	int len = strlen(str);
    	int i;
    	for (i=0;i<len;i++) {
        	if ( !is_digit( str[i] ) )
            		return FALSE;
    	}
    	//fprintf(stdout,"\n\r %s %d", str, len);
    	return TRUE;
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


void close_keyboard()
{
    	//new_settings.c_lflag |= ECHO | ICANON;
    	new_settings.c_cc[VMIN] = 1; 
    	tcsetattr(fileno(stdin), TCSANOW, &initial_settings);
}

void ouch(int sig)
{
    	//
    	signalch = 1;
    	(void) signal(SIGINT, SIG_DFL);
}

void init_keyboard()
{   
    	//struct sigaction act;
	int i;

	for(i=0;i< MAX_USER_TERM_LINE;i++)  user_term_buff[i] = 0;

    	tcgetattr(fileno(stdin), &initial_settings);
    	new_settings = initial_settings;             
     
    	new_settings.c_lflag &= ~ECHO & ~ICANON /*&~ ISIG*/;
     
    	new_settings.c_cc[VMIN] = 0; 
    	//new_settings.c_cc[VTIME] = 0;
    	if(tcsetattr(fileno(stdin), TCSANOW, &new_settings) != 0)
   	 {
        	fprintf(stderr, "Could not set attributes\n");
    	}

    	//(void) signal(SIGINT, ouch);    
}

static void funcHelp()
{
    	int i;

    	fprintf(stdout," ===================================================\n\n");
    	fprintf(stdout," Command \t Alt \t\t Comments\n");
    	fprintf(stdout," ===================================================\n");
    	//for(i=0; termCmdTable[i].pfFunc != NULL; i++)
   	 //{
	        //fprintf(stdout," "); fprintf(stdout,termCmdTable[i].psName); fprintf(stdout,"\t\t");
	        //fprintf(stdout,termCmdTable[i].psAltName); fprintf(stdout,"\t");
	        //fprintf(stdout, termCmdTable[i].psComments\n);
    	//}
    	fprintf(stdout,"\n");
	fflush(stdout);
}

void print_prompt(void)
{
    	switch(term_stat)
    	{
	    	case MAIN_MENU_STAT:
	        	fprintf(stdout, USER_TERM_PROMPT_STRING);
	        break;

	    	case DATA_SAVE_MENU:
	        	fprintf(stdout," DATA>");
	        break;

#if 0 // by jwank 070221
    case TEST_SYSTEM_MENU:
        fprintf(stdout," TEST>");
        break;
#endif

	    	case BOARD_TEST_MENU:
	        	fprintf(stdout," DEBUG>");
	        break;

	    	default:
	        	fprintf(stdout, USER_TERM_PROMPT_STRING);
	        break;
    	}
	fflush(stdout);
}

void print_main_menu(void)
{
	fprintf(stdout,"\n");
	fprintf(stdout," ----------------------- Main Commands --------------------\n");
	fprintf(stdout," C : Configuration mode\n");
	fprintf(stdout," M : real time Monitorng mode\n");
	fprintf(stdout," P : Polling time monitoring mode\n");
	//fprintf(stdout," S : data Save\n");

	fprintf(stdout," Y : Exit Mainproc\n");
	fprintf(stdout,"\n");
    
    	print_prompt();
	fflush(stdout);
}

void print_main_config_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ VDS Controller Configuration Commands -------------\n");
    	fprintf(stdout," T : date & Time                   A : group & Address\n");
    	fprintf(stdout," P : IP Address                    M : Subnet Mask\n");
    	fprintf(stdout," W : Gateway IP Address            D : Server IP Address\n");
    	fprintf(stdout," C : Server TCP Port               S : Synchronization\n");
    	fprintf(stdout," L : maximum Loop Dual & Single    G : Gap of loop\n"); 
    	fprintf(stdout," I : Initialize total volume \n");
    	fprintf(stdout,"\n");
    	print_prompt();
	fflush(stdout);
}


void print_save_data_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ Save Data Commands -----------------------------\n");
    	fprintf(stdout," RL : print Lastest Realtime data\n");
    	fprintf(stdout," PL : print Lastest Polling data\n");
    	fprintf(stdout," RI : print Realtime data by Index\n");
    	fprintf(stdout," PI : print Polling data by Index\n");
    	fprintf(stdout," RS : receive Realtime Saving data [XMODEM]\n");
    	fprintf(stdout," PS : receive Polling Saving data [XMODEM]\n");
    	fprintf(stdout,"\n");
   	print_prompt();
	fflush(stdout);
}

void print_sys_test_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ Module Test Commands -----------------------------\n");
    	fprintf(stdout," C : CPU module test\n");
    	fprintf(stdout," I : I/O module test\n");
    	fprintf(stdout," M : MMI module test\n");
    	fprintf(stdout,"\n");
    	print_prompt();
	fflush(stdout);
}

void print_cpm_test_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ Module Test Commands -----------------------------\n");
    	fprintf(stdout," C : RTC test\n");
    	fprintf(stdout," S : ROM test\n");
    	fprintf(stdout," I : RAM test\n");
    	fprintf(stdout," X : COMMUNICATION test\n");
    	fprintf(stdout,"\n");
    	print_prompt();
	fflush(stdout);
}

void print_hidden_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ Hidden Management Commands -----------------------------\n");
    	fprintf(stdout," D : Default parameter [SUBM all]\n");
    	fprintf(stdout,"\n");
    	print_prompt();
	fflush(stdout);
}

void print_board_test_menu(void)
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," ------------ Board Test Commands -----------------------------\n");
    	funcHelp();
    	fprintf(stdout,"\n");
    	print_prompt();
	fflush(stdout);
}

void print_time_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," date & Time\n");
    	fprintf(stdout," %d/%02d/%02d %02d:%02d:%02d => ", sys_time.wYear, sys_time.wMonth, \
        	sys_time.wDay, sys_time.wHour, sys_time.wMinute, sys_time.wSecond);
}

void print_addr_chg_menu()
{
    	uint16 usTmpStation[2];

    	// wwww
   	 // station_num 이 2바이트씩 MSB first 임.
    	usTmpStation[0] = (gstNetConfig.station_num[0] << 8) + gstNetConfig.station_num[1];
    	usTmpStation[1] = (gstNetConfig.station_num[2] << 8) + gstNetConfig.station_num[3];
    
    	fprintf(stdout,"\n");
    	fprintf(stdout," Group & Address\n");
    	fprintf(stdout," %05d-%05d => ", usTmpStation[0], usTmpStation[1]);
	fflush(stdout);
}

void print_ip_addr_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," IP Address\n");
    	fprintf(stdout," %03d.%03d.%03d.%03d => ", gstNetConfig.ip[0], gstNetConfig.ip[1], \
        	    gstNetConfig.ip[2], gstNetConfig.ip[3]);
	fflush(stdout);
}

void print_subnet_mask_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," Subnet Mask\n");
    	fprintf(stdout," %03d.%03d.%03d.%03d => ", gstNetConfig.nmsk[0], gstNetConfig.nmsk[1], \
        	    gstNetConfig.nmsk[2], gstNetConfig.nmsk[3]);
	fflush(stdout);
}

void print_gateway_ip_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," Gateway IP Address\n");
    	fprintf(stdout," %03d.%03d.%03d.%03d => ", gstNetConfig.gip[0], gstNetConfig.gip[1], \
        	    gstNetConfig.gip[2], gstNetConfig.gip[3]);
	fflush(stdout);
}

void print_server_ip_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," Server IP\n");
    	fprintf(stdout," %03d.%03d.%03d.%03d => ", gstNetConfig.server_ip[0], gstNetConfig.server_ip[1], \
        	    gstNetConfig.server_ip[2], gstNetConfig.server_ip[3]);
	fflush(stdout);
}

void print_server_port_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," Server TCP Port\n");
    	fprintf(stdout," %04d => ", gstNetConfig.server_port);
	fflush(stdout);
}
/*
void print_mac_addr_chg_menu()
{
    fprintf(stdout,"\n");
    fprintf(stdout," MAc Address\n");
    fprintf(stdout," %02d:%02d:%02d:%02d:%02d:%02d => ", gstNetConfig.mac[0], gstNetConfig.mac[1], \
            gstNetConfig.mac[2], gstNetConfig.mac[3],gstNetConfig.mac[4],gstNetConfig.mac[5]);
}
*/

void print_synctime_chg_menu()
{
    	fprintf(stdout,"\n");
    	fprintf(stdout," Synchronization\n");
    	fprintf(stdout," %d => ", gstSysConfig.param_cfg.polling_cycle);
	fflush(stdout);
}

int gets_alt(char *s, int *size)
{
    	char ch;  
   	 int nread;  
    
    	static char *fp; 

	fflush(stdin);
    	ch = fgetc(stdin); 	

    	if(signalch == 1) 
   	 {
       		signalch = 0;
        	//fprintf(stdout,"\r\n Re-input Ctrl + c \r\n");
        	fprintf(stdout,"\n close %s,  fp : %s  \r\n",s ,fp);
        	close_keyboard();
		exit(0);
   	 }       

   	 if(ch == 255)
	 {	 	
	 	return (FALSE);  
   	 }
    
   	 if(ch == '\n')
    	{            
	        if (usertermcnt == 0)
		{
			fp = s;
			//fprintf(stdout,"\n cnt : %d, test %s,  fp : %s  \r\n",usertermcnt ,s ,fp);
	        }
	        *fp++ = 0x00;           
	        
	        *size = usertermcnt;
	        usertermcnt = 0;

		//fprintf(stdout,"\n test %s,  fp : %s  \r\n",s ,fp);

	        return 1;
    	}         
      
    	switch(ch)
    	{
	        case 0x08:
	           	 if( usertermcnt > 0 )
	            	{ 
	            		//fprintf(stdout,"\n cnt : %d, test %s,  fp : %s  \r\n",usertermcnt, s ,fp);
		                usertermcnt--; *fp-- = ' '; 
		                fprintf(stdout,"\b \b");
	           	 }
	        break;

	        case 0x1b:

	            	fp = s;
	            	*fp++ = ESC; usertermcnt++;
	            	*fp++ = 0;
	            
	            	fp = s;
	            	*size = usertermcnt;
	            	usertermcnt = 0;

			//fprintf(stdout,"\n esc %s,  fp : %s  \r\n",s ,fp);

	            	return 1;
	        break;

	        case 0x0d:              
	            	*fp = 0;
		        fp = s;

			//fprintf(stdout,"bufferin %s,  fp : %s  \r\n",s ,fp);
		        *size = usertermcnt;
		        usertermcnt = 0;                
		         return 1;
	        break;

	        default :

	           	 if (usertermcnt == 0)
	            	{
	                	fp=s;                    
	            	}                

	            	usertermcnt++;
	            	if (usertermcnt > (MAX_USER_TERM_LINE-3)) break;
	            
	            	*fp++ = ch;

			//fprintf(stdout,"\n def cnt : %d, test %s,  fp : %s  \r\n",usertermcnt, s ,fp);
	            
	            	//if (ch >= 0x20) fputc(ch, stdout);
			if (ch >= 0x20) fprintf(stdout,"%c", ch);
			
	        
	    	break;
    	} 

    	*size = 0;
	fflush(stdout);
    	return (FALSE);
    
}

userTermTask()
{
    	int i, cmd_size, tmpv;
    	char *tptr;
    	unsigned char cmd;      

    	SYSTEMTIME tmptime; 

   	 if(gets_alt(user_term_buff, &cmd_size))
    	{
	        cmd = user_term_buff[0];
	        
	        if (!strncmp(user_term_buff, "debug", 5)) 
	        {
	            	fprintf(stdout,"\n");
	            	fprintf(stdout,"============ Enter to Debug monitoring mode ==========\n");
	            	fprintf(stdout,"\n");
	            	term_stat = DEBUG_MONITOR_MODE;
	        }
	        else if (!strncmp(user_term_buff, "hidden", 6)) 
	        {
	            	term_stat = HIDDEN_MANAGE_MENU;
	            	print_hidden_menu();
	            	return;
	        }           

	        switch(term_stat)
	        {
	        
	        //----------------------------------------------------------
	        // Main Menu
	        //----------------------------------------------------------
	        case MAIN_MENU_STAT:
	            	switch(cmd)
	            	{
	            	case ESC :
	                	print_main_menu();
	                break;
	                
		        case 0x00 :
		        case 0x0d:                
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;

	            	case '?':
		                print_main_menu();
		                break;
	                
		        case 'c':
		        case 'C':
		                fprintf(stdout, "\ninput config\n");
		                term_stat = MAIN_CONFIG_MENU;
		                print_main_config_menu();
	                break;
	                
	            	case 'm':
	            	case 'M':
		                fprintf(stdout,"\n");
		                fprintf(stdout,"============ Enter to Real time monitoring mode ==========");
		                fprintf(stdout,"\n");
		                term_stat = REALTIME_MONITOR_MODE;

		                cntNumRtData = 1;

		                for (i=0; i<MAX_LANE_NUM; i++)
		                    lane_evt_flg[i] = NO_EVENT;
		                
	                break;

	            	case 'p':
	            	case 'P':
		                fprintf(stdout,"\n");
		                fprintf(stdout,"============ Enter to Polling monitoring mode ============");
		                fprintf(stdout,"\n");
		                term_stat = POLLING_MONITOR_MODE;
	                break;


	            	case 'y':
	            	case 'Y':
		                fprintf(stdout,"\n\r");
		                fprintf(stdout,"\n\r*** Warning ***");
		                fprintf(stdout,"\n\r");
		                fprintf(stdout,"\n\r Main Controller & All Sub-Controller will be initialized.");
		                fprintf(stdout,"\n\r if you want to press ENTER or not press ESC");
		                fprintf(stdout,"\n\r");
		                //term_stat = RESET_SYSTEM; 	//Delete by capi

						save_log( USERTERM_RESET, 0); // kwj insert. 2017.07.19		                
		                exit(EXIT_FAILURE);
		               
	                break;

	            	default:                
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	           	 }
	        break;
	        //----------------------------------------------------------
	        // Config Menu
	        //----------------------------------------------------------
	        case MAIN_CONFIG_MENU:
	            	switch(cmd) 
	           	 {
	           	 case EXIT_CHAR :
		                term_stat = MAIN_MENU_STAT;             
		                print_main_menu();
	                break;

	            	case NUL :
	            	case CR:
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	                
	            	case '?':
	               		 print_main_config_menu();
	                break;
	                
	            	case 't':
	            	case 'T':
		                term_stat = INPUT_TIME_STAT;
		                print_time_chg_menu();                      
	                break;

	            	case 'a':
	            	case 'A':               
	                
		                term_stat = INPUT_ADDR_STAT;
		                print_addr_chg_menu();                      
	                break;

	            	case 'p':
	            	case 'P':
		                term_stat = CONFIG_IP_ADDR;
		                print_ip_addr_chg_menu();
	                break;

	            	case 'm':
	            	case 'M':
		                term_stat = CONFIG_SUBNET_MSK;
		                print_subnet_mask_chg_menu();
	                break;

	            	case 'w':
	            	case 'W':
		                term_stat = CONFIG_GATEWAY_IP;
		                print_gateway_ip_chg_menu();
	                break;

	            	case 'd':
	            	case 'D':
		                term_stat = CONFIG_SERVER_IP;
		                print_server_ip_chg_menu();
	                break;

	            	case 'c':
	            	case 'C':
		                term_stat = CONFIG_SERVER_PORT;
		                print_server_port_chg_menu();
	                break;          

	            	case 's':
	            	case 'S':
		                term_stat = CONFIG_SYNC;
		                print_synctime_chg_menu();
	                break;

	            	case 'l':
	            	case 'L':
		                fprintf(stdout,"\n\r maximum Loop Dual & Sing");
		                fprintf(stdout,"\n\r %02d-%02d => ", gucActiveDualLoopNum, gucActiveSingleLoopNum); //2011.06.16 by capidra.  dual Loop, single Loop
		                
		                term_stat = CONFIG_MAX_LOOP;
	                break;

	            	case 'g':
	            	case 'G':
		                fprintf(stdout,"\n\r Gap of loop");
		                fprintf(stdout,"\n\r %02d => ", gstSysConfig.param_cfg.spd_loop_dist[0]);
		                
		                term_stat = CONFIG_GAP_LOOP;
	                break;

	            	case 'i':
	            	case 'I':
		                fprintf(stdout,"\n\r");
		                fprintf(stdout,"\n\r Total volume is will be ZERO.");
		                fprintf(stdout,"\n\r if you want to press ENTER or not press ESC");
		                fprintf(stdout,"\n\r");

	                term_stat = INIT_VOL_STAT;
	                break;
	            	default :
		                fprintf(stdout,"\n");
		                print_prompt();
	            	break;
	            	}
	        break;

	        //----------------------------------------------------------
	        // HIDDEN Menu
	        //----------------------------------------------------------
	        case HIDDEN_MANAGE_MENU:
	            	switch(cmd)
	           	{
	            	case EXIT_CHAR :
		                term_stat = MAIN_MENU_STAT;
		                print_main_menu();
	                break;

	            	case NUL:
	            	case CR:
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	                
	            	case '?':
	                	print_hidden_menu();
	                break;

	            	case 'd':
	            	case 'D':
	                	fprintf(stdout,"\r\n Default Parameter !!!");	                
	                	
	                	defaultVdsParameter();
	                	
		                gstSysConfig.is_default_param = _DEFAULT_PARAM_;
		               
		                //writeParamToNVRAM(TRUE);

		                getActiveLoopNum();
		                initVdsGlobalVal();
		                pullSpeedLoopDim();
		        
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	            
	            	default: 
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	           	}
	        //break;
	        break;

	        //----------------------------------------------------------
	        // Board Test Menu
	        //----------------------------------------------------------
	        case BOARD_TEST_MENU:
	        //insertHistBuff(user_term_buff);
	        //runUserCmd(user_term_buff);
	        break;

	        //----------------------------------------------------------
	        // Realtime Monitor Mode
	        //----------------------------------------------------------
	        case REALTIME_MONITOR_MODE:
	            	switch(cmd) 
	           	 {
	            	case EXIT_CHAR :
		                fprintf(stdout,"\n");
		                fprintf(stdout,"============ Exit from Real time monitoring mode ==========\n");
		                fprintf(stdout,"\n");
		                term_stat = MAIN_MENU_STAT; 
		                print_prompt();

	                break;
	            }
	        break;

	        //----------------------------------------------------------
	        // Polling Monitor Mode
	        //----------------------------------------------------------
	        case POLLING_MONITOR_MODE:
	            	switch(cmd) 
	           	{
	                case EXIT_CHAR :
		                fprintf(stdout,"\n");
		                fprintf(stdout,"============ Exit from Polling monitoring mode ============\n");
		                fprintf(stdout,"\n");
		                term_stat = MAIN_MENU_STAT;
		                print_prompt();
	                break;
	            	}
	        break;

	        //----------------------------------------------------------
	        // Debug Monitor Mode
	        //----------------------------------------------------------
	        case DEBUG_MONITOR_MODE:
	            	switch(cmd)
	            	{
	            	case EXIT_CHAR :
		                fprintf(stdout,"\n");
		                fprintf(stdout,"============ Exit from Debug monitoring mode ==========\n");
		                fprintf(stdout,"\n");
		                term_stat = MAIN_MENU_STAT;
		                print_prompt();
	                break;
	            }
	        break;      

	        //----------------------------------------------------------
	        // TEST SYSTEM MENU
	        //----------------------------------------------------------
	        case TEST_SYSTEM_MENU:
	            	switch(cmd) 
	            	{
	            	case EXIT_CHAR :
		                term_stat = MAIN_MENU_STAT;
		                print_main_menu();                      
	                break;

	            	case CR :
	            	case NUL :
		                fprintf(stdout,"\n");
		                print_prompt(); 
	                break;

	            	default :
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	            }
	        break;
	        //----------------------------------------------------------
	        // RESET SYSTEM MENU
	        //----------------------------------------------------------
	        case RESET_SYSTEM:
	            	switch(cmd) 
	            	{
	            	case EXIT_CHAR :
		                term_stat = MAIN_MENU_STAT;
		                print_main_menu();                      
	                break;

	            	case CR :
	            	case NUL :
		                fprintf(stdout,"\n\r");
		                fprintf(stdout,"SYSTEM RESET !!!");
		                //RESET_FUNC();
						forceResetwithWatchdog();
		                term_stat = MAIN_MENU_STAT;                 
	                break;

	            	default :
		                fprintf(stdout,"\n");
		                print_prompt();
	                break;
	            }
	        break;
	        //----------------------------------------------------------
	        // TIME SETTING MENU
	        //----------------------------------------------------------
	        case INPUT_TIME_STAT:
	           	 if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
				char timebuf[255];

		                //fprintf(stdout,"\n\r\n\r input %s !\n\r", user_term_buff);
		                
		                // Year
		                tptr = strtok(user_term_buff, "/");
		                if ( tptr == NULL || strlen(tptr) != 4 ) goto no_save_time;
		                //tmptime.wYear = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wYear);
		                if (ret == FALSE) goto no_save_time;

		                // Month
		                tptr = strtok(NULL, "/");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_time;
		                //tmptime.wMonth = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wMonth);
		                if (ret == FALSE) goto no_save_time;
		                if (tmptime.wMonth > 12 || tmptime.wMonth <= 0) goto no_save_time;

		                // Day                  
		                tptr = strtok(NULL, " ");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_time;
		                //tmptime.wDay = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wDay);
		                if (ret == FALSE) goto no_save_time;
		                if (tmptime.wDay > 31 || tmptime.wDay <= 0) goto no_save_time;

		                // Hour                 
		                tptr = strtok(NULL, ":");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_time;
		                //tmptime.wHour = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wHour);
		                if (ret == FALSE) goto no_save_time;
		                if (tmptime.wHour >= 24) goto no_save_time;

		                // Minute                   
		                tptr = strtok(NULL, ":");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_time;
		                //tmptime.wMinute = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wMinute);
		                if (ret == FALSE) goto no_save_time;
		                if (tmptime.wMinute >= 60) goto no_save_time;

		                // Second
		                tptr = tptr + strlen(tptr) + 1;
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_time;
		                //tmptime.wSecond = atoi(tptr);
		                ret = str2num(tptr, (int *)&tmptime.wSecond);
		                if (ret == FALSE) goto no_save_time;
		                if (tmptime.wSecond >= 60) goto no_save_time;

		                fprintf(stdout,"\nSet Data & Time to RTC!\n");
		                fprintf(stdout,"%d/%d/%d %d:%d:%d\n\r", tmptime.wYear, tmptime.wMonth, \
		                    tmptime.wDay, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);

				//OS Time change
				sprintf(timebuf,"date -s '%d-%d-%d %d:%d:%d'"
								,tmptime.wYear, tmptime.wMonth, tmptime.wDay
								, tmptime.wHour, tmptime.wMinute, tmptime.wSecond);
				system(timebuf);
		                
		                //
				msec_sleep(300);
		                setSysTimeToRTC(&tmptime);				
	                
	            	}

	no_save_time :
	            	//fprintf(stdout,"\n\r\n\r Nosave %s !\n\r", user_term_buff);
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();                       
	        break;

	        //----------------------------------------------------------
	        // ADDRESS SETTING MENU
	        //----------------------------------------------------------
	        case INPUT_ADDR_STAT:               

	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
		                int tmp_id, tmp_addr;

		                //fprintf(stdout,"\n\r %s", user_term_buff );
		                
		                // Group
		                tptr = strtok(user_term_buff, "-");
		                if ( tptr == NULL || strlen(tptr) > 5 || strlen(tptr) == 0 ) goto no_save_addr;
		                tmp_id = atoi(tptr);
		                
		                if (tmp_id > 0x0000ffff) goto no_save_addr;

		                // Address
		                tptr = strtok(NULL, "\0");
		                if ( tptr == NULL || strlen(tptr) > 5 || strlen(tptr) == 0 ) goto no_save_addr;
		                tmp_addr = atoi(tptr);
		                
		                if (tmp_addr > 0x0000ffff) goto no_save_addr;

		                fprintf(stdout,"\n\r\n\r Set Group & Address !\n\r");
		                fprintf(stdout," %05d-%05d\n\r", tmp_id, tmp_addr);

		                gstNetConfig.station_num[0] = ((uint16)tmp_id) >> 8;
		                gstNetConfig.station_num[1] = ((uint16)tmp_id) & 0xFF;
		                gstNetConfig.station_num[2] = ((uint16)tmp_addr) >> 8;
		                gstNetConfig.station_num[3] = ((uint16)tmp_addr) & 0xFF;

		                writeNetConfigToNAND(TRUE);
		                sendNetConfigMsgToMMI(MMI_CFG_IDX_06_STATION_NUM);
	                
	            	}

	no_save_addr :
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();                       
	            break;

	        
	        //----------------------------------------------------------
	        // IP ADDRESS SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_IP_ADDR:
	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                int status;
		                
		                u8 ip_addr[4];

		                status = netIpStr2Bin(user_term_buff, ip_addr);
		                if (status == 0)
		                {
		                    fprintf(stdout,"\n\r New IP Address : %03d.%03d.%03d.%03d", \
		                        ip_addr[0], ip_addr[1], ip_addr[2], ip_addr[3]);

		                    for(i=0; i<4; i++) gstNetConfig.ip[i] = ip_addr[i];

					setInterfaces();	//by capi interface file write
		                    	writeNetConfigToNAND(TRUE);
		                   	 sendNetConfigMsgToMMI(MMI_CFG_IDX_01_IP_ADDR);
		                }
		                else
		                {
		                    fprintf(stdout,"\n\r\n\r ERROR !\n\r");
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }               
	            	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // SUBNET MASK SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_SUBNET_MSK:
	           	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                int status;
		                u8 subn_msk[4];
		        
		                status = netIpStr2Bin(user_term_buff, subn_msk);

		                if (status == 0)
		                {
			                 fprintf(stdout,"\n\r New Subnet Mask : %03d.%03d.%03d.%03d", \
			                        subn_msk[0], subn_msk[1], subn_msk[2], subn_msk[3]);

			                 for(i=0; i<4; i++) gstNetConfig.nmsk[i] = subn_msk[i];

					setInterfaces();	//by capi interface file write
			                writeNetConfigToNAND(TRUE);
			                sendNetConfigMsgToMMI(MMI_CFG_IDX_03_SUBN_MSK);
		                }
		                else
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }               
	            	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // GATEWAY IP ADDRESS SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_GATEWAY_IP:
	           	 if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		               	 int status;
		                u8 gip_addr[4];
		        
		                status = netIpStr2Bin(user_term_buff, gip_addr);

		                if (status == 0)
		                {
		                    	fprintf(stdout,"\n\r New Gateway IP : %03d.%03d.%03d.%03d", \
		                        	gip_addr[0], gip_addr[1], gip_addr[2], gip_addr[3]);

		                   	 for(i=0; i<4; i++) gstNetConfig.gip[i] = gip_addr[i];

					setInterfaces();	//by capi interface file write
		                    	writeNetConfigToNAND(TRUE);
		                    	sendNetConfigMsgToMMI(MMI_CFG_IDX_02_GATEWAY_IP);
		                }
		                else
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }               
	            	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // SERVER IP ADDRESS SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_SERVER_IP:
	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                int status;
		                u8 server_ip[4];
		        
		                status = netIpStr2Bin(user_term_buff, server_ip);

		                if (status == 0)
		                {
		                    fprintf(stdout,"\n\r New VDS Server IP : %03d.%03d.%03d.%03d", \
		                        server_ip[0], server_ip[1], server_ip[2], server_ip[3]);

		                    for(i=0; i<4; i++) gstNetConfig.server_ip[i] = server_ip[i];
		                    writeNetConfigToNAND(TRUE);
		                    sendNetConfigMsgToMMI(MMI_CFG_IDX_04_SERVER_IP);
		                }
		                else
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }               
	            	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // SERVERPORT SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_SERVER_PORT:
	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
		                
		                //tmpv = atoi(user_term_buff);                  

		                ret = str2num(user_term_buff, &tmpv);
		                if (ret == FALSE || tmpv < 0 || tmpv > 65535)
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }
		                else
		                {
		                    fprintf(stdout,"\n\r Sever TCP Port : %d",  tmpv);
		                    gstNetConfig.server_port = tmpv;
		                    writeNetConfigToNAND(TRUE);
		                    sendNetConfigMsgToMMI(MMI_CFG_IDX_05_SERVER_PORT);
		                }               
	            	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;

	        //----------------------------------------------------------
	        // MAC ADDRESS SETTING MENU
	        //----------------------------------------------------------
	        /*case CONFIG_MAC_ADDR :
	           	 if (cmd != EXIT_CHAR && cmd != NUL)
	           	 {
		                BOOL ret;
		                int status;
		                unsigned char mac_addr[6];
		                
		                //tmpv = atoi(user_term_buff);                  
		                fprintf(stdout, " New MAC Address : %s \n ", user_term_buff);

		                status = netMacStr2Bin(user_term_buff, mac_addr);
		                if (status == 0)
		                {
		                    for(i=0; i<6; i++) gstNetConfig.mac[i] = mac_addr[i];

		                    fprintf(stdout,"\n\r New VDS Server IP : %02d:%02d:%02d:%02d:%02d:%02d", \
		                        mac_addr[0], mac_addr[1], mac_addr[2], mac_addr[3], mac_addr[4], mac_addr[5]);
		                    //dumpNetworkConfig();
		                }
		                else
		                {
		                    fprintf(stdout, " ############## Invalid MAC Address !!\n");
		                }
	           	}
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	            

	            
	        break;
	*/
	        //----------------------------------------------------------
	        // SERVER SYNC TIME SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_SYNC:
	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
		                
		                //tmpv = atoi(user_term_buff);
		                ret = str2num(user_term_buff, &tmpv);
		                if (ret == FALSE)
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }
		                
		                if ( (tmpv != gstSysConfig.param_cfg.polling_cycle) && (tmpv <= 250) && (tmpv >= 0) )
		                {
		                    fprintf(stdout,"\n\r New polling cycle : %d",  tmpv);
		                    gstSysConfig.param_cfg.polling_cycle = tmpv;
		                    writeParamToNAND(TRUE);
		                    sendSysConfigMsgToMMI();
		                }
	                                    
	            	}   
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // USED LOOP COUNT SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_MAX_LOOP:
	        
	            /*2011.06.16 by capidra - Single Loop*/         
	            
	            	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
		                int dlane, tm_dual, tm_single;;
		                uint8 j, t1, t2, msk, p_param[10];              
		                    
		                // Group
		                tptr = strtok(user_term_buff, "-");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_num;
		                //tmp_id = atoi(tptr);
		                ret = str2num(tptr, &tm_dual);
		                if (ret == FALSE) goto no_save_num;
		                if (tm_dual > 0x000000ff) goto no_save_num;
		                
		                if (tm_dual %2) goto no_save_num;
		                
		                // Address
		                tptr = strtok(NULL, "\0");
		                if ( tptr == NULL || strlen(tptr) > 2 || strlen(tptr) == 0 ) goto no_save_num;              
		                
		                //tmp_addr = atoi(tptr);
		                ret = str2num(tptr, &tm_single);
		                if (ret == FALSE) goto no_save_num; 
		                if (tm_single > 0x000000ff) goto no_save_num;

		                fprintf(stdout,"\n\r\n\r Set Dual & Single !\n\r");
		                fprintf(stdout," %02d-%02d\n\r", tm_dual, tm_single);

		                //gucActiveDualLoopNum= ((uint8)tm_dual) >> 8;
		                gucActiveDualLoopNum= (uint8)tm_dual ;
		                gucActiveSingleLoopNum= ((uint8)tm_single) & 0xFF;  

		                gucActiveLoopNum = gucActiveDualLoopNum + gucActiveSingleLoopNum*2;
		                
		                for(i=0;i<MAX_LANE_NUM;i++) gucDivDualLoopLane[i] = 0;

		                dlane = gucActiveDualLoopNum/2;
		                for(i=0;i<gucActiveSingleLoopNum;i++) gucDivDualLoopLane[dlane+i] = 1;

		                if ( gucActiveLoopNum > MAX_RIO_LOOP_NUM ) gucActiveLoopNum = MAX_RIO_LOOP_NUM;

		                for (i=0; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;

		                t1 = gucActiveLoopNum / 8;
		                t2 = gucActiveLoopNum % 8;
		        
		                for (i=0; i<t1; i++) p_param[i] = 0xff;
		                for (j=0, msk=1; j<t2; j++, msk<<=1) p_param[i] |= msk;
		                i++;
		                for (; i<MAX_LOOP_NUM/8; i++) p_param[i] = 0;               
		                
		                fprintf(stdout,"\n\r New maximum loop : %d Dual : %d Single : %d",  gucActiveLoopNum, gucActiveDualLoopNum, gucActiveSingleLoopNum);
		                fprintf(stdout,"\n\r PPARAM : %X  : %X  : %X : %X",  p_param[0] , p_param[1] , p_param[2] , p_param[3]);

		                putVdsParameter(IDX_01_LOOP_ENABLE, p_param, TRUE);
		                sendSysConfigMsgToMMI();
	            	}   

	no_save_num :
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	            
	        break;
	        
	        //----------------------------------------------------------
	        // SPEED LOOP DISTANCE SETTING MENU
	        //----------------------------------------------------------
	        case CONFIG_GAP_LOOP :
	        
	           	if (cmd != EXIT_CHAR && cmd != NUL)
	            	{
		                BOOL ret;
		                
		                //tmpv = atoi(user_term_buff);
		                ret = str2num(user_term_buff, &tmpv);
		                if (ret == FALSE)
		                {
		                    term_stat = MAIN_CONFIG_MENU;
		                    print_main_config_menu();
		                    break;
		                }

		                if ( tmpv <= 0 ) tmpv = 45;     // default is 45 DM;
		                else if ( tmpv > 250 ) tmpv = 250;

		                fprintf(stdout,"\n\r New gap of loop : %d",  tmpv);

		                //
		                for (i=0; i<MAX_LANE_NUM; i++)
		                    gstSysConfig.param_cfg.spd_loop_dist[i] = tmpv;

		                pullSpeedLoopDim();

		                // out of defalut parameter.
		                gstSysConfig.is_default_param = _USER_SETTING_PARAM_;
		                // write parameter to flash.
		                writeParamToNAND(TRUE);
		                sendSysConfigMsgToMMI();
	    
	            	}
	            
	            	term_stat = MAIN_CONFIG_MENU;
	            	print_main_config_menu();
	        break;
	        //----------------------------------------------------------
	        // VILUMN RESET SETTING MENU
	        //----------------------------------------------------------
	        case INIT_VOL_STAT :
	            	switch(cmd) 
	            	{
	                case EXIT_CHAR :
	                    	term_stat = MAIN_MENU_STAT;
	                    	print_main_menu();                      
	                break;

	                case NUL :
	                case CR :
		                fprintf(stdout,"\n\r Total volume is ZERO !\n\r");
		                 // inserted by jwank 051107
		                for (i=0; i<MAX_LOOP_NUM; i++) gstReportsOfLoop[i].volume = 0;
		                term_stat = MAIN_MENU_STAT;
		                break;
	            }
	        break;

	        default :
	        break;
	        }
	        
	    }
	    else
	    {
	        switch(term_stat)
	        {
	        /////////////////////////////////////////////////////////////////////
#if 0 // 1 kang
	          case REALTIME_MONITOR_MODE:
	          {
	                for (i=0; i<MAX_LANE_NUM; i++)
	                {
	                    	if (lane_evt_flg[i] == EVENT_CREATED)
	                    	{

	                    		//fprintf(stdout,"\n\r 3 invers_st.gRT_Reverse[%d] = %d \n\r",i, invers_st.gRT_Reverse[i]);

		                        //fprintf(stdout,"\n%04d", cntNumRtData % 10000);
		                        
		                        // 시간.
		                        /*fprintf(stdout,"\n%04d %04d/%02d/%02d %02d:%02d:%02d:%02d", cntNumRtData, \
		                            	sys_time.wYear, sys_time.wMonth, sys_time.wDay, \
		                            	sys_time.wHour, sys_time.wMinute, sys_time.wSecond, (sys_time.wMilliseconds/10)%100);
		                         */                           
		                        if (invers_st.gRT_Reverse[i] == TRUE)
		                        {
		                        	if(gstCurrData.speed[i]<20000)
						{

		                        		fprintf(stdout,"\n%04d %04d/%02d/%02d %02d:%02d:%02d:%02d", cntNumRtData, \
		                            			sys_time.wYear, sys_time.wMonth, sys_time.wDay, \
		                            			sys_time.wHour, sys_time.wMinute, sys_time.wSecond, (sys_time.wMilliseconds/10)%100);
			                            	fprintf(stdout," lane:%02d spd:-%05lu occ1:%05lu occ2:%05lu len:%05lu", \
			                               		 i+1, gstCurrData.speed[i], gstCurrData.occupy[gstLaneLink[i].s_loop], \
			                               		 gstCurrData.occupy[gstLaneLink[i].e_loop], gstCurrData.length[i]);

		                        	} 
		                        }
		                        else
		                        {

		                        	fprintf(stdout,"%04d %04d/%02d/%02d %02d:%02d:%02d:%02d lane:%02d spd:%05lu occ1:%05lu occ2:%05lu len:%05lu\n",
							cntNumRtData, \
		                            		sys_time.wYear, sys_time.wMonth, sys_time.wDay, \
		                            		sys_time.wHour, sys_time.wMinute, sys_time.wSecond, (sys_time.wMilliseconds/10)%100,
		                               		 i+1, gstCurrData.speed[i], gstCurrData.occupy[gstLaneLink[i].s_loop], \
		                                	gstCurrData.occupy[gstLaneLink[i].e_loop], gstCurrData.length[i]);

		                        }                           

		                        cntNumRtData++;
		                        if (cntNumRtData >= 10000) cntNumRtData = 1;

	                    	}
         
	                    	lane_evt_flg[i] = NO_EVENT;
	                        
	                }
	          }
	          break;#endif
#endif
	        /////////////////////////////////////////////////////////////////////

	        /////////////////////////////////////////////////////////////////////
	        case POLLING_MONITOR_MODE :
	        {
	                int j;
	                
	                if ( polling_disp_flg == _POLLING_DATA_RDY_ )
	                {
	                   	 // row 1
	                    	fprintf(stdout,"\n%02d/%02d/%02d %02d:%02d:%02d ", \
	                        	sys_time.wYear%100, sys_time.wMonth, sys_time.wDay, \
	                        	sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

	                    	fprintf(stdout,"%05d-%05d, LOOPS=%02d, LANES=%02d, ",
								(gstNetConfig.station_num[0]<<8) + gstNetConfig.station_num[1], 
								(gstNetConfig.station_num[2]<<8) + gstNetConfig.station_num[3],
	                        	gucActiveLoopNum, gucActiveLoopNum/2);

	                    	fprintf(stdout,"ECODE=");
	                    	//for(j=0; j<2; j++) 
	                        //fprintf(stdout,"[%2x]", gucStuckPerLoop[j]);

	                    	fprintf(stdout,", ICODE=");
	                    	for(j=0; j<3; j++) 
	                        	fprintf(stdout,"[%2x]", 0);

	                    	// row 2
	                    	fprintf(stdout,"\n%02d/%02d/%02d %02d:%02d:%02d ", \
	                        	sys_time.wYear%100, sys_time.wMonth, sys_time.wDay, \
	                        	sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

	                    	fprintf(stdout,"VOL   : ");
	                    	for(j=0; j<gucActiveLoopNum; j++) 
	                        	fprintf(stdout,"%3d ", gstCenterPolling.volume[j]);

	                    	// row 3
	                    	fprintf(stdout,"\n%02d/%02d/%02d %02d:%02d:%02d ", \
	                        	sys_time.wYear%100, sys_time.wMonth, sys_time.wDay, \
	                        	sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

	                    	#if defined(INCREASE_OCCUPY_RATE_PRECISION) // eeee
	                    		fprintf(stdout,"OCC   : ");
	                    		for(j=0; j<gucActiveLoopNum; j++) 
	                        	printf("%3d ", round_for_occupy(gstCenterPolling.occupy[j], 0) );
	                    	#else
	                    	fprintf(stdout,"OCC   : ");
	                    	for(j=0; j<gucActiveLoopNum; j++) 
	                        	fprintf(stdout,"%3d ", gstCenterPolling.occupy[j]);
	                    	#endif

	                   	 // row 4
	                    	fprintf(stdout,"\n%02d/%02d/%02d %02d:%02d:%02d ", \
	                        	sys_time.wYear%100, sys_time.wMonth, sys_time.wDay, \
	                        	sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

	                    	fprintf(stdout,"SPD   : ");
	                    	for(j=0; j<gucActiveLoopNum/2; j++) 
	                        	fprintf(stdout,"%7d ", gstCenterPolling.speed[j]);

	                   	 // row 5
	                    	fprintf(stdout,"\n%02d/%02d/%02d %02d:%02d:%02d ", \
	                        	sys_time.wYear%100, sys_time.wMonth, sys_time.wDay, \
	                        	sys_time.wHour, sys_time.wMinute, sys_time.wSecond);

	                    	fprintf(stdout,"LEN   : ");
	                    	for(j=0; j<gucActiveLoopNum/2; j++) 
	                        	fprintf(stdout,"%7d ", gstCenterPolling.length[j]);

	                    	fprintf(stdout,"\n");

	                    	polling_disp_flg = _POLLING_DATA_EMPTY_;
	                    
	                }
	        }
	        break;
	        /////////////////////////////////////////////////////////////////////
	        
	        }
    	}
    //close_keyboard();
    fflush(stdout);
        
}   
