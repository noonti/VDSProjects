using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDSCommon.Config;
using VDSController;

namespace VDSCommon
{

    public static class VDSConfig
    {

        public static ControllerConfig controllerConfig { get; set; }
        public static KictConfig kictConfig { get; set; }
        public static KorExConfig korExConfig {get;set;}
        
        public static String VDS_DB_CONN = "";
        public static String MA_DB_CONN = ""; // 유지보수 DB

        /// <summary>
        /// 접속 종료 시 재접속 시도 간격
        /// </summary>
        public static int RECONNECT_INTERVAL = 10;// 재접속 시도 간격(초)

        

        /// <summary>
        /// DB 수정 시 별도 쓰레드 사용 여부
        /// </summary>
        public static bool USE_DB_THREAD = true;

        public static String RADAR_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.ff";


        public const int PACKET_SIZE = 4096*2;
        public const int PAGE_SIZE = 20;



        public static LaneGroup ToLeftLaneGroup = new LaneGroup();
        public static LaneGroup ToRIghtLaneGroup = new LaneGroup();

        public static byte[] SPEED_CATEGORY = new byte[12];

        public static byte[] LENGTH_CATEGORY = new byte[3];


        public static byte[] LOOP_NO = new byte[32];

        
        public static int ReadConfig()
        {
            int nResult = 0;
            try
            {
                ReadControllerConfig();
                ReadKictConfig();
                ReadKorExConfig();


                //ReadLaneGroup(ref ToLeftLaneGroup, "Left");
                //ReadLaneGroup(ref ToRIghtLaneGroup, "Right");


                // 스피드 분류 초기값 설정 
                for (int i = 0; i < 12; i++)
                    SPEED_CATEGORY[i] =(byte)( (i + 1) * 10);

                // dm : m 의 1/10 1 dm = 10 cm = 0.1 m 
                LENGTH_CATEGORY[0] = 80 ; // 80 dm --> 8 m small
                LENGTH_CATEGORY[1] = 120; // 120 dm --> 12 m mid 
                LENGTH_CATEGORY[2] = 150; // 150 dm --> 15 m big


                // LOOP NO 초기값 설정
                for (int i = 0; i < 16; i++)
                {
                    LOOP_NO[i] = (byte)i;
                    LOOP_NO[i+1] = (byte)(i+1);
                }

                nResult = 1;
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }
        public static void ReadControllerConfig()
        {
            int value = 0;
            if (controllerConfig == null)
                controllerConfig = new ControllerConfig();
            // controller config start 
            if (!int.TryParse(AppConfiguration.GetAppConfig("CENTER_TYPE"), out value))
                value = (int)REMOTE_CENTER_TYPE.CENTER_KICT;

            controllerConfig.ProtocolType = value;

            controllerConfig.IpAddress = AppConfiguration.GetAppConfig("IP_ADDRESS");
            if (String.IsNullOrEmpty(controllerConfig.IpAddress))
                controllerConfig.IpAddress = "127.0.0.1";

            controllerConfig.ControllerId = AppConfiguration.GetAppConfig("VDS_ID");
            if (String.IsNullOrEmpty(controllerConfig.ControllerId))
                controllerConfig.ControllerId = "avogadro";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("API_PORT"), out value))
                value = 8089;
            controllerConfig.ApiPort = value;

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("REMOTE_CTRL_PORT"), out value))
                value = 8090;
            controllerConfig.RemoteCtrlPort = value;


            controllerConfig.RemoteCtrlId = AppConfiguration.GetAppConfig("REMOTE_CTRL_ID");
            if (String.IsNullOrEmpty(controllerConfig.RemoteCtrlId))
                controllerConfig.RemoteCtrlId = "admin";

            controllerConfig.RemoteCtrlPasswd = AppConfiguration.GetAppConfig("REMOTE_CTRL_PASSWD");
            if (String.IsNullOrEmpty(controllerConfig.RemoteCtrlPasswd))
                controllerConfig.RemoteCtrlPasswd = "admin";


            controllerConfig.RTUPort = AppConfiguration.GetAppConfig("RTU_PORT");
            if (String.IsNullOrEmpty(controllerConfig.RTUPort))
                controllerConfig.RTUPort = "COM11";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("BAUD_RATE"), out value))
                value = 115200;
            controllerConfig.BaudRate = value;



            controllerConfig.DBAddress = AppConfiguration.GetAppConfig("VDS_DB_ADDRESS");
            if (String.IsNullOrEmpty(controllerConfig.DBAddress))
                controllerConfig.DBAddress = "127.0.0.1";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("VDS_DB_PORT"), out value))
                value = 3306;
            controllerConfig.DBPort = value;


            controllerConfig.DBName = AppConfiguration.GetAppConfig("VDS_DB_NAME");
            if (String.IsNullOrEmpty(controllerConfig.DBName))
                controllerConfig.DBName = "vdsdb";



            controllerConfig.DBUser = AppConfiguration.GetAppConfig("VDS_DB_USER");
            if (String.IsNullOrEmpty(controllerConfig.DBUser))
                controllerConfig.DBUser = "VDS";


            controllerConfig.DBPasswd = AppConfiguration.GetAppConfig("VDS_DB_PASSWD");
            if (String.IsNullOrEmpty(controllerConfig.DBPasswd))
                controllerConfig.DBPasswd = "1234";


            VDS_DB_CONN = String.Format($"Server={controllerConfig.DBAddress};Port={controllerConfig.DBPort};Database={controllerConfig.DBName};Uid={controllerConfig.DBUser};Pwd={controllerConfig.DBPasswd};SSL Mode=None");



            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("VDS_TYPE"), out value))
                value = 1;

            controllerConfig.DeviceType = value;


            controllerConfig.DeviceAddress = AppConfiguration.GetAppConfig("VDS_DEVICE_ADDRESS");
            if (String.IsNullOrEmpty(controllerConfig.DeviceAddress))
                controllerConfig.DeviceAddress = "127.0.0.1";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("VDS_DEVICE_PORT"), out value))
                value = 4555;

            controllerConfig.RemotePort = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("VDS_LOCAL_PORT"), out value))
                value = 45175;
            controllerConfig.LocalPort = value;

            double distance = 0;
            if (!double.TryParse(AppConfiguration.GetAppConfig("CHECK_DISTANCE"), out distance))
                distance = 3;
            controllerConfig.CheckDistance = distance;


            controllerConfig.StreamingURL = AppConfiguration.GetAppConfig("RTSP_STREAMING_URL");
            if (String.IsNullOrEmpty(controllerConfig.StreamingURL))
                controllerConfig.StreamingURL = "";



            controllerConfig.MAServerAddress = AppConfiguration.GetAppConfig("MA_SERVER_ADDRESS");
            if (String.IsNullOrEmpty(controllerConfig.MAServerAddress))
                controllerConfig.MAServerAddress = "127.0.0.1";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("MA_SERVER_PORT"), out value))
                value = 1234;

            controllerConfig.MAServerPort = value;

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("MA_API_PORT"), out value))
                value = 8088;
            controllerConfig.MAServerAPIPort = value;


            controllerConfig.MAServerUrl = String.Format($"http://{controllerConfig.MAServerAddress}:{controllerConfig.MAServerAPIPort}/api/");


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("VDS_USE_ANIMATION"), out value))
                value = 1;

            controllerConfig.UseAnimation = value;


            if (!int.TryParse(AppConfiguration.GetAppConfig("TRAFFIC_DATA_PERIOD"), out value))
                value = 30;
            controllerConfig.TrafficDataPeriod = value;

            if (!int.TryParse(AppConfiguration.GetAppConfig("LOG_FILE_PERIOD"), out value))
                value = 30;
            controllerConfig.LogFilePeriod = value;
        }

        public static void ReadKictConfig()
        {
            // kict config start 
            if (kictConfig == null)
                kictConfig = new KictConfig();

            int value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("CTRL_PORT"), out value))
                value = 12000;

            kictConfig.ctrlPort = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("CALIB_PORT"), out value))
                value = 11000;
            kictConfig.calibPort = value;


            // 수집서버 
            kictConfig.centerAddress = AppConfiguration.GetAppConfig("CENTER_ADDRESS");
            if (String.IsNullOrEmpty(kictConfig.centerAddress))
                kictConfig.centerAddress = "";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("CENTER_PORT"), out value))
                value = 10000;

            kictConfig.centerPort = value;

        }

        public static void ReadKorExConfig()
        {
            if (korExConfig == null)
                korExConfig = new KorExConfig();
            // korExConfig start
            int value = 0;

            // 수집서버 
            korExConfig.centerAddress = AppConfiguration.GetAppConfig("KOR_EX_CENTER_ADDRESS");
            if (String.IsNullOrEmpty(korExConfig.centerAddress))
                korExConfig.centerAddress = "";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_CENTER_PORT"), out value))
                value = 10000;

            korExConfig.centerPort = value;


            if (!int.TryParse(AppConfiguration.GetAppConfig("CENTER_POLLILNG_PERIOD"), out value))
                value = 30;
            korExConfig.centerPollingPeriod = value;



            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("LOCAL_POLLING_PERIOD"), out value))
                value = 30;

            korExConfig.localPollingPeriod = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("CHECK_SESSION_TIME"), out value))
                value = 300;

            korExConfig.checkSessionTime = value;



            korExConfig.vdsType = AppConfiguration.GetAppConfig("VDS_KIND");
            if (String.IsNullOrEmpty(korExConfig.vdsType))
                korExConfig.vdsType = "VD";

            korExConfig.vdsGroup = AppConfiguration.GetAppConfig("VDS_GROUP");
            if (String.IsNullOrEmpty(korExConfig.vdsGroup))
                korExConfig.vdsGroup = "1";

            korExConfig.vdsNo = AppConfiguration.GetAppConfig("VDS_NO");
            if (String.IsNullOrEmpty(korExConfig.vdsNo))
                korExConfig.vdsNo = "0000000001";

            korExConfig.csn = Utility.GetCSN(korExConfig.vdsType, korExConfig.vdsGroup, korExConfig.vdsNo);

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("SENSOR_COUNT"), out value))
                value = 1;
            korExConfig.sensorCount = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("POWER_SUPPLY_COUNT"), out value))
                value = 1;

            korExConfig.powerSupplyCount = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("BOARD_COUNT"), out value))
                value = 1;
            korExConfig.boardCount = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_VERSION_NO"), out value))
                value = 1;
            korExConfig.versionNo = value;

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_RELEASE_NO"), out value))
                value = 2;
            korExConfig.releaseNo = value;

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_RELEASE_YEAR"), out value))
                value = 21;
            korExConfig.releaseYear = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_RELEASE_MONTH"), out value))
                value = 12;

            korExConfig.releaseMonth = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("KOR_EX_RELEASE_DAY"), out value))
                value = 4;
            korExConfig.releaseDay = value;

            // korExConfig end 

        }
        //public static void ReadLaneGroup(ref LaneGroup laneGroup, String laneName)
        //{
        //    NameValueCollection groupNVC;
        //    String laneGroupName = String.Format("LaneGroup/{0}LaneGroup", laneName);
        //    int laneCount = 0;
            
        //    //"LaneGroup/LeftLaneGroup" , "LaneGroup/RightLaneGroup"
            
        //    groupNVC = (NameValueCollection)ConfigurationManager.GetSection(laneGroupName);
        //    if(groupNVC != null && groupNVC.Keys.Count > 0)
        //    {
        //        //< add key = "LaneGroupName" value = "좌측방향" />
        //        // < add key = "LaneSort" value = "1" />
        //        //    < add key = "Direction" value = "1" />
        //        //       < add key = "LaneCount" value = "0" />
        //        laneGroup.LaneGroupName = groupNVC.GetValues("LaneGroupName")[0].ToString() ;
        //        laneGroup.LaneSort = int.Parse(groupNVC.GetValues("LaneSort")[0].ToString());
        //        laneGroup.Direction = int.Parse(groupNVC.GetValues("Direction")[0].ToString());
        //        laneCount = int.Parse(groupNVC.GetValues("LaneCount")[0].ToString());
        //        if(laneCount>0)
        //        {
        //            for(int i =0;i< laneCount;i++)
        //            {
        //                String laneSection = String.Format("{0}Lane/Lane_{1}", laneName, i+1);
        //                LaneInfo lane = ReadLaneInfo(laneSection);
        //                if(lane!=null)
        //                    laneGroup.AddLaneInfo(lane);
        //            }
        //        }
        //    }
        //}

        //public static LaneInfo ReadLaneInfo(String laneSection)
        //{
        //    LaneInfo lane = null;
        //    NameValueCollection nvc;
        //    nvc = (NameValueCollection)ConfigurationManager.GetSection(laneSection);
        //    if (nvc != null && nvc.Keys.Count > 0)
        //    {
        //        lane = new LaneInfo();
        //        lane.LaneName = nvc.GetValues("LaneName")[0].ToString();
        //        lane.Lane = int.Parse(nvc.GetValues("lane")[0].ToString());
        //        lane.Direction = int.Parse(nvc.GetValues("Direction")[0].ToString());

        //    }
        //    return lane;
        //}

        

        public static int SaveConfig()
        {
            int nResult = 0;
            try
            {
                SaveControllerConfig();
                SaveKictConfig();
                SaveKorExConfig();



               

                //AppConfiguration.SetAppConfig("CENTER_TYPE", CENTER_TYPE.ToString());

                //AppConfiguration.SetAppConfig("IP_ADDRESS",IPADDRESS);
                //AppConfiguration.SetAppConfig("CTRL_PORT", CTRL_PORT.ToString());
                //AppConfiguration.SetAppConfig("CALIB_PORT", CALIB_PORT.ToString());
                //AppConfiguration.SetAppConfig("API_PORT", API_PORT.ToString());
                //AppConfiguration.SetAppConfig("VDS_ID", VDS_ID);


                //AppConfiguration.SetAppConfig("RTU_PORT", RTU_PORT);
                //AppConfiguration.SetAppConfig("BAUD_RATE", BAUD_RATE.ToString());


                //AppConfiguration.SetAppConfig("VDS_DB_ADDRESS", VDS_DB_ADDRESS);
                //AppConfiguration.SetAppConfig("VDS_DB_PORT", VDS_DB_PORT.ToString());
                //AppConfiguration.SetAppConfig("VDS_DB_NAME", VDS_DB_NAME);
                //AppConfiguration.SetAppConfig("VDS_DB_USER", VDS_DB_USER);
                //AppConfiguration.SetAppConfig("VDS_DB_PASSWD", VDS_DB_PASSWD);



                //AppConfiguration.SetAppConfig("CENTER_ADDRESS", CENTER_ADDRESS);
                //AppConfiguration.SetAppConfig("CENTER_PORT", CENTER_PORT.ToString());



                //AppConfiguration.SetAppConfig("VDS_TYPE", VDS_TYPE.ToString());
                //AppConfiguration.SetAppConfig("VDS_DEVICE_ADDRESS", VDS_DEVICE_ADDRESS);
                //AppConfiguration.SetAppConfig("VDS_DEVICE_PORT", VDS_DEVICE_PORT.ToString());
                //AppConfiguration.SetAppConfig("VDS_LOCAL_PORT", VDS_LOCAL_PORT.ToString());

                //AppConfiguration.SetAppConfig("CHECK_DISTANCE", CHECK_DISTANCE.ToString());
                //AppConfiguration.SetAppConfig("RTSP_STREAMING_URL", RTSP_STREAMING_URL);

                //AppConfiguration.SetAppConfig("VDS_USE_ANIMATION", VDS_USE_ANIMATION.ToString());


                //AppConfiguration.SetAppConfig("SENSOR_COUNT", SENSOR_COUNT.ToString());

                //AppConfiguration.SetAppConfig("POWER_SUPPLY_COUNT", POWER_SUPPLY_COUNT.ToString());
                //AppConfiguration.SetAppConfig("BOARD_COUNT", BOARD_COUNT.ToString());

                //AppConfiguration.SetAppConfig("KOR_EX_VERSION_NO", KOR_EX_VERSION_NO.ToString());
                //AppConfiguration.SetAppConfig("KOR_EX_RELEASE_NO", KOR_EX_RELEASE_NO.ToString());

                //AppConfiguration.SetAppConfig("KOR_EX_RELEASE_YEAR", KOR_EX_RELEASE_YEAR.ToString());
                //AppConfiguration.SetAppConfig("KOR_EX_RELEASE_MONTH", KOR_EX_RELEASE_MONTH.ToString());
                //AppConfiguration.SetAppConfig("KOR_EX_RELEASE_DAY", KOR_EX_RELEASE_DAY.ToString());


                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public static int SaveControllerConfig()
        {
            int nResult = 0;
            try
            {
                AppConfiguration.SetAppConfig("CENTER_TYPE", controllerConfig.ProtocolType.ToString());

                AppConfiguration.SetAppConfig("IP_ADDRESS", controllerConfig.IpAddress);
                AppConfiguration.SetAppConfig("VDS_ID", controllerConfig.ControllerId);
                AppConfiguration.SetAppConfig("API_PORT", controllerConfig.ApiPort.ToString());

                AppConfiguration.SetAppConfig("REMOTE_CTRL_PORT", controllerConfig.RemoteCtrlPort.ToString());
                AppConfiguration.SetAppConfig("REMOTE_CTRL_ID", controllerConfig.RemoteCtrlId);
                AppConfiguration.SetAppConfig("REMOTE_CTRL_PASSWD", controllerConfig.RemoteCtrlPasswd);


                AppConfiguration.SetAppConfig("RTU_PORT", controllerConfig.RTUPort);
                AppConfiguration.SetAppConfig("BAUD_RATE", controllerConfig.BaudRate.ToString());


                AppConfiguration.SetAppConfig("VDS_DB_ADDRESS", controllerConfig.DBAddress);
                AppConfiguration.SetAppConfig("VDS_DB_PORT", controllerConfig.DBPort.ToString());

                AppConfiguration.SetAppConfig("VDS_DB_NAME", controllerConfig.DBName);
                AppConfiguration.SetAppConfig("VDS_DB_USER", controllerConfig.DBUser);

                AppConfiguration.SetAppConfig("VDS_DB_PASSWD", controllerConfig.DBPasswd);

                AppConfiguration.SetAppConfig("VDS_TYPE", controllerConfig.DeviceType.ToString());

                AppConfiguration.SetAppConfig("VDS_DEVICE_ADDRESS", controllerConfig.DeviceAddress);

                AppConfiguration.SetAppConfig("VDS_DEVICE_PORT", controllerConfig.RemotePort.ToString());

                AppConfiguration.SetAppConfig("VDS_LOCAL_PORT", controllerConfig.LocalPort.ToString());

                AppConfiguration.SetAppConfig("CHECK_DISTANCE", controllerConfig.CheckDistance.ToString());

                AppConfiguration.SetAppConfig("RTSP_STREAMING_URL", controllerConfig.StreamingURL);

                AppConfiguration.SetAppConfig("MA_SERVER_ADDRESS", controllerConfig.MAServerAddress);

                AppConfiguration.SetAppConfig("MA_SERVER_PORT", controllerConfig.MAServerPort.ToString());

                AppConfiguration.SetAppConfig("MA_API_PORT", controllerConfig.MAServerAPIPort.ToString());


                AppConfiguration.SetAppConfig("VDS_USE_ANIMATION", controllerConfig.UseAnimation.ToString());

                AppConfiguration.SetAppConfig("TRAFFIC_DATA_PERIOD", controllerConfig.TrafficDataPeriod.ToString());
                AppConfiguration.SetAppConfig("LOG_FILE_PERIOD", controllerConfig.LogFilePeriod.ToString());


                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }

        public static int SaveKictConfig()
        {
            int nResult = 0;
            try
            {
                AppConfiguration.SetAppConfig("CTRL_PORT", kictConfig.ctrlPort.ToString());
                AppConfiguration.SetAppConfig("CALIB_PORT", kictConfig.calibPort.ToString());
                AppConfiguration.SetAppConfig("CENTER_ADDRESS", kictConfig.centerAddress);
                AppConfiguration.SetAppConfig("CENTER_PORT", kictConfig.centerPort.ToString());

                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }


        public static int SaveKorExConfig()
        {
            int nResult = 0;
            try
            {

                AppConfiguration.SetAppConfig("VDS_KIND", korExConfig.vdsType);
                AppConfiguration.SetAppConfig("VDS_GROUP", korExConfig.vdsGroup);
                AppConfiguration.SetAppConfig("VDS_NO", korExConfig.vdsNo);

              
                AppConfiguration.SetAppConfig("KOR_EX_CENTER_ADDRESS", korExConfig.centerAddress);
                AppConfiguration.SetAppConfig("KOR_EX_CENTER_PORT", korExConfig.centerPort.ToString());


                AppConfiguration.SetAppConfig("CENTER_POLLILNG_PERIOD", korExConfig.centerPollingPeriod.ToString());
                AppConfiguration.SetAppConfig("LOCAL_POLLING_PERIOD", korExConfig.localPollingPeriod.ToString());
                AppConfiguration.SetAppConfig("CHECK_SESSION_TIME", korExConfig.checkSessionTime.ToString());

                AppConfiguration.SetAppConfig("SENSOR_COUNT", korExConfig.sensorCount.ToString());
                AppConfiguration.SetAppConfig("POWER_SUPPLY_COUNT", korExConfig.powerSupplyCount.ToString());
                AppConfiguration.SetAppConfig("BOARD_COUNT", korExConfig.boardCount.ToString());

                AppConfiguration.SetAppConfig("KOR_EX_VERSION_NO", korExConfig.versionNo.ToString());
                AppConfiguration.SetAppConfig("KOR_EX_RELEASE_NO", korExConfig.releaseNo.ToString());

                AppConfiguration.SetAppConfig("KOR_EX_RELEASE_YEAR", korExConfig.releaseYear.ToString());
                AppConfiguration.SetAppConfig("KOR_EX_RELEASE_MONTH", korExConfig.releaseMonth.ToString());
                AppConfiguration.SetAppConfig("KOR_EX_RELEASE_DAY", korExConfig.releaseDay.ToString());



                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return nResult;
        }


        public static String GetVDSTypeName()
        {
            String result = String.Empty;
            switch(controllerConfig.DeviceType)
            {
                case 1:
                    result = "VIDEO_UNISEM";
                    break;
                case 2:
                    result = "MClavisRadar";
                    break;

            }
            return result;
        }

        public static String GetVDSControllerName()
        {
            String result = String.Empty;

            switch (controllerConfig.DeviceType)
            {
                case 1:
                    result = "VDS Controller - 영상식";
                    break;
                case 2:
                    result = "VDS Controller - 레이더식";
                    break;
                case 3:
                    result = "VDS Controller - 하이브리드";
                    break;
            }

            switch(controllerConfig.ProtocolType)
            {
                case 1:
                    result += "(한국건설기술연구원)";
                    break;
                case 2:
                    result += "(도로공사)";
                    break;

            }
            return result;
        }
    }
}
