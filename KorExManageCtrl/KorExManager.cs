using KorExManageCtrl.VDSProtocol;
using KorExManageCtrl.VDSProtocol_v2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

using System.Threading.Tasks;
using System.Timers;
using VDSCommon;
using VDSDBHandler;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;
using VDSManagerCtrl;

namespace KorExManageCtrl
{
    public class KorExManager : VDSManager
    {
        public VDSClient _korExCenter = new VDSClient();
        public ExDataFrame _prevDataFrame = null;

        public byte _lastFrameNo = 0x00;
        public DateTime _prevSyncDateTime;
        public DateTime _currentSyncDateTime;
        public DateTime _lastTrafficDataCheck;
        public DateTime _lastSpeedDataCheck;
        public DateTime _lastLengthDataCheck;
        public DateTime _lastAccuTrafficDataCheck;
        public DateTime _connectionDateTime;
        public DateTime _lastCheckSession;
        public KOR_EX_SESSION_STATE sessionState;

        public List<TRAFFIC_DATA_STAT> trafficDataList = new List<TRAFFIC_DATA_STAT>();

        public SpeedData[] speedDataStat ;

        public LengthData[] lengthDataStat;

        public UInt16[] accuTrafficDataStat; 

        public IExOPData[] VDSParam = new IExOPData[24];

        public IVDSDevice _vdsDevice;

        public Timer _period_timer = null;

        public bool _parameterDownloaded = false;
        public bool _selfSync = true;


        public VDSServer _korExServer = new VDSServer();



        public override int SetVDSDevice(IVDSDevice vdsDevice)
        {
            _vdsDevice = vdsDevice;
            _vdsDevice.SetAddTrafficDataEventDelegate(AddTrafficDataEvent);
            return 1;
        }

        public override int StartManager()
        {

            //  
            _prevSyncDateTime = DateTime.Now;
            _currentSyncDateTime = DateTime.Now;

            _lastFrameNo = Utility.GetLocalFrameNo();
            _lastTrafficDataCheck = DateTime.Now;
            _lastSpeedDataCheck = DateTime.Now;
            _lastLengthDataCheck = DateTime.Now;
            _lastAccuTrafficDataCheck = DateTime.Now;
            _lastResetCenterData = DateTime.Now;
            _lastResetLocalData = DateTime.Now;
            _lastCheckSession = DateTime.Now;

            speedDataStat = new SpeedData[16];
            lengthDataStat = new LengthData[16];
            accuTrafficDataStat = new UInt16[16];
            for (int i=0;i<16;i++)
            {
                speedDataStat[i] = new SpeedData();
                lengthDataStat[i] = new LengthData();
                accuTrafficDataStat[i] = 0;
            }
            _parameterDownloaded = false;
            _selfSync = true;

            sessionState = KOR_EX_SESSION_STATE.SESSION_OFFLINE;

            ReadVDSParameter();

            StartTimer();


            _korExServer.SetAddress(VDSConfig.controllerConfig.IpAddress, VDSConfig.korExConfig.centerPort, CLIENT_TYPE.KICT_EVNT_CLIENT, AcceptKorExServerCallback);
            _korExServer.StartManager();


            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            //_korExCenter.SetAddress(VDSConfig.korExConfig.centerAddress, VDSConfig.korExConfig.centerPort, CLIENT_TYPE.KICT_EVNT_CLIENT, KorExClientConnectCallback, KorExReadCallback, SendCallback);
            //_korExCenter.StartConnect();


            StartWorkThread();
            StartDBUpdateThread();

            _Logger.StartManager();

            //_vdsDevice.StartDevice(VDSConfig.CENTER_ADDRESS, VDSConfig.CENTER_PORT);
            _vdsDevice.StartDevice(VDSConfig.controllerConfig.DeviceAddress, VDSConfig.controllerConfig.RemotePort, VDSConfig.controllerConfig.LocalPort);

            Utility.AddLog(LOG_TYPE.LOG_INFO, "*******************도로공사 콘트롤러 시작*******************");
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public override int StopManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            _vdsDevice.StopDevice();
            _ctrlServer.StopManager();
            //_korExCenter.StopCheckConnection();
            DeleteAllSessionContext();
            StopWorkThread();
            StopDBUpdateThread();

            StopTimer();
            Utility.AddLog(LOG_TYPE.LOG_INFO, "******************도로공사 콘트롤러 종료*******************");
            _Logger.StopManager();

            SaveVDSParameter();
            return 1;
        }

        public int StartTimer()
        {
            if (_period_timer == null)
                _period_timer = new Timer();
            _period_timer.Interval = 200;
            _period_timer.Elapsed += _period_timer_Elapsed;
            _period_timer.Start();

            return 1;
        }


        public void AcceptKorExServerCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                //if (socket.Connected)
                {
                    SessionContext vdsClient = new SessionContext();
                    vdsClient._type = _korExServer._clientType;
                    vdsClient._socket = socket;
                    socket.BeginReceive(vdsClient.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(KorExReadCallback), vdsClient);
                    AddSessionContext(vdsClient);
                    _connectionDateTime = DateTime.Now; // 접속 시간  저장
                    strLog = String.Format($"제어기 접속 accepted");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            finally
            {
                _korExServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        private void _period_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ResetLocalPeriodData();
            CheckSessionTime();
        }

        public int StopTimer()
        {
            if (_period_timer != null)
                _period_timer.Stop();
            _period_timer = null;

            return 1;
        }
        public int ReadVDSParameter()
        {
            int nResult = 0;
            for(byte i = 0;i<24;i++)
            {
                switch(i)
                {
                    case 0:
                        VDSParam[i] = ReadLaneConfig() ;// ReadLoopDetectorConfig();
                        break;
                    case 1:
                        VDSParam[i] = ReadSpeedLoopConfig();
                        break;
                    case 2:
                        VDSParam[i] = ReadPollingCycle();
                        break;
                    case 3:
                        VDSParam[i] = ReadVehiclePulseNumber();
                        break;
                    case 4:
                        VDSParam[i] = ReadSpeedCategory();
                        break;
                    case 5:
                        VDSParam[i] = ReadLengthCategory();
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        VDSParam[i] = ReadVDSValue((byte)(i+1));
                        break;
                    case 10:
                        VDSParam[i] = ReadSpeedLoopDimension();
                        break;
                    case 11:
                    case 12:
                    case 13:
                        VDSParam[i] = ReadPollingThreshold((byte)(i + 1));
                        break;
                    case 14:
                        VDSParam[i] = ReadIncidentDetectThreshold();
                        break;
                    case 15:
                        VDSParam[i] = ReadStuckThreshold();
                        break;
                    case 16:
                        VDSParam[i] = ReadOscillationThreshold((byte)(i + 1));
                        break;
                    case 17:
                        break;
                    case 18:
                        break;
                    case 19:
                        VDSParam[i] = ReadAutoSyncPeriod((byte)(i + 1));
                        break;
                    case 20:
                        VDSParam[i] = ReadVDSValue((byte)(i + 1)); // ReadSimulationTemplate();
                        break;
                    case 21:
                        break;

                    case 22:
                        break;
                    case 23:
                        break;
                }
            }

            return nResult;
        }


        public int SaveVDSParameter()
        {
            int nResult = 0;
            for (byte i = 0; i < 24; i++)
            {
                switch (i)
                {
                    case 0:
                        //SaveLoopDetectorConfig((ParamLoopConfig) VDSParam[i]);
                        SaveLaneConfig((ParamLaneConfig) VDSParam[i]);
                        break;
                    case 1:
                        SaveSpeedLoopConfig((SpeedLoopConfig)VDSParam[i]);
                        break;
                    case 2:
                        SavePollingCycle((PollingCycle)VDSParam[i]);
                        break;
                    case 3:
                        SaveVehiclePulseNumber((VehiclePulseNumber)VDSParam[i]);
                        break;
                    case 4:
                        SaveSpeedCategory((SpeedCategory)VDSParam[i]);
                        break;
                    case 5:
                        SaveLengthCategory((LengthCategory)VDSParam[i]);
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        SaveVDSValue((VDSValue)VDSParam[i], i);
                        break;
                    case 10:
                        SaveSpeedLoopDimension((SpeedLoopDimension)VDSParam[i]);
                        break;
                    case 11:
                    case 12:
                    case 13:
                        SavePollingThreshold((PollingThreshold)VDSParam[i],i);
                        break;
                    case 14:
                        SaveIncidentDetectThreshold((IncidentDetectThreshold)VDSParam[i]);
                        break;
                    case 15:
                         SaveStuckThreshold((StuckThreshold)VDSParam[i]);
                        break;
                    case 16:
                        SaveOscillationThreshold((VDSValue)VDSParam[i],i);
                        break;
                    case 17:
                        break;
                    case 18:
                        break;
                    case 19:
                        SaveAutoSyncPeriod((VDSValue)VDSParam[i]);
                        break;
                    case 20:
                        SaveSimulationTemplate((SimulationTemplate) VDSParam[i]);
                        break;
                    case 21:
                        break;

                    case 22:
                        break;
                    case 23:
                        break;
                }
            }

            return nResult;
        }

        public IExOPData ReadLaneConfig()
        {
            IExOPData result = null;
            ParamLaneConfig config = new ParamLaneConfig();
            int configValue = 0;
            int.TryParse(AppConfiguration.GetAppConfig("LANE_CONFIG_0"), out configValue);
            config.laneInfo[0] = (byte)configValue;

            int.TryParse(AppConfiguration.GetAppConfig("LANE_CONFIG_1"), out configValue);
            config.laneInfo[1] = (byte)configValue;
            result = config;
            return result;
        }


        public void SaveLaneConfig(ParamLaneConfig config)
        {
            AppConfiguration.SetAppConfig("LANE_CONFIG_0", config.laneInfo[0].ToString());
            AppConfiguration.SetAppConfig("LANE_CONFIG_1", config.laneInfo[1].ToString());
        }


        public IExOPData ReadLoopDetectorConfig()
        {
            IExOPData result = null;
            ParamLoopConfig loop = new ParamLoopConfig();
            int loopInfo = 0;
            int.TryParse(AppConfiguration.GetAppConfig("LOOP_DETECTOR_CONFIG"), out loopInfo);
            loop.loopInfo = (UInt32)loopInfo;
            result = loop;
            return result;
        }

        public void SaveLoopDetectorConfig(ParamLoopConfig loop)
        {
            AppConfiguration.SetAppConfig("LOOP_DETECTOR_CONFIG", loop.loopInfo.ToString());
        }

        public IExOPData ReadSpeedLoopConfig()
        {
            IExOPData result = null;
            SpeedLoopConfig config = new SpeedLoopConfig();
            for(int i=0;i<32;i++)
            {
                int loopNo = 0;
                int.TryParse(AppConfiguration.GetAppConfig(String.Format($"SPEED_LOOP_CONFIG_{i+1}")), out loopNo);
                config.loopNo[i] = (byte)loopNo;
            }
            result = config;
            return result;
        }

        public void SaveSpeedLoopConfig(SpeedLoopConfig config)
        {
            for (int i = 0; i < 32; i++)
            {
                AppConfiguration.SetAppConfig(String.Format($"SPEED_LOOP_CONFIG_{i + 1}"), config.loopNo[i].ToString());
            }
        }


        public IExOPData ReadPollingCycle()
        {
            IExOPData result = null;

            PollingCycle cycle = new PollingCycle();

            int value = 30;
            int.TryParse(AppConfiguration.GetAppConfig("POLLING_CYCLE"), out value);
            cycle.cycle = (byte)value;

            result = cycle;
            return result;
        }


        public void SavePollingCycle(PollingCycle cycle)
        {
            AppConfiguration.SetAppConfig(String.Format("POLLING_CYCLE"), cycle.cycle.ToString());
        }

        public IExOPData ReadVehiclePulseNumber()
        {
            IExOPData result = null;
            VehiclePulseNumber pulse = new VehiclePulseNumber();

            int value = 6;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_PRESENCE"), out value);

            pulse.presenceNumber = (byte)value; //

            value = 3;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_NO_PRESENCE"), out value);
            pulse.noPresenceNumber = (byte)value;//

            result = pulse;

            return result;
        }

        public void SaveVehiclePulseNumber(VehiclePulseNumber pulse)
        {

            AppConfiguration.SetAppConfig(String.Format("VEHICLE_PRESENCE"), pulse.presenceNumber.ToString());
            AppConfiguration.SetAppConfig(String.Format("VEHICLE_NO_PRESENCE"), pulse.noPresenceNumber.ToString());

        }

        public IExOPData ReadSpeedCategory()
        {
            IExOPData result = null;
            SpeedCategory category = new SpeedCategory();
            for (int i = 0; i < category.category.Length; i++)
            {
                // 스피트 분류 초기값 설정 
                int speed = (i + 1) * 10;
                int.TryParse(AppConfiguration.GetAppConfig(String.Format($"SPEED_CATEGORY_{i + 1}")), out speed);
                category.category[i] = (byte)speed;
            }
            result = category;
            return result;
        }


        public void SaveSpeedCategory(SpeedCategory category)
        {
            for (int i = 0; i < category.category.Length; i++)
            {
                AppConfiguration.SetAppConfig(String.Format($"SPEED_CATEGORY_{i + 1}"), category.category[i].ToString());
            }
        }


        public IExOPData ReadLengthCategory()
        {
            IExOPData result = null;
            LengthCategory category = new LengthCategory();
            for (int i = 0; i < category.category.Length; i++)
            {
                // dm : m 의 1/10 1 dm = 10 cm = 0.1 m 
                //LENGTH_CATEGORY[0] = 80; // 80 dm --> 8 m small
                //LENGTH_CATEGORY[1] = 120; // 120 dm --> 12 m mid 
                //LENGTH_CATEGORY[2] = 150; // 150 dm --> 15 m big
                int length = 0;
                switch (i)
                {
                    case 0:
                        length = 80;
                        break;
                    case 1:
                        length = 120;
                        break;
                    case 2:
                        length = 150;
                        break;
                }
                int.TryParse(AppConfiguration.GetAppConfig(String.Format($"LENGTH_CATEGORY_{i + 1}")), out length);
                category.category[i] = (byte)length;
            }

            result = category;
            return result;
        }


        public void SaveLengthCategory(LengthCategory category)
        {
            for (int i = 0; i < category.category.Length; i++)
            {
                AppConfiguration.SetAppConfig(String.Format($"LENGTH_CATEGORY_{i + 1}"), category.category[i].ToString());
            }
        }

        public IExOPData ReadVDSValue(byte index)
        {
            IExOPData result = null;
            String sectionName = String.Empty;
            VDSValue ability = new VDSValue(index);
            int value = 0;
            switch(index)
            {
                case 6:
                    value = 1;
                    sectionName = String.Format("SPEED_ACCU_ENABLED");
                    break;
                case 7:
                    sectionName = String.Format("LENGTH_ACCU_ENABLED");
                    break;
                case 8:
                    sectionName = String.Format("SPEED_CALCU_ENABLED");
                    break;
                case 9:
                    sectionName = String.Format("LENGTH_CALCU_ENABLED");
                    break;

                case 21:
                    sectionName = String.Format("REVERSE_RUN_ENABLED");
                    break;

            }

            int.TryParse(AppConfiguration.GetAppConfig(sectionName), out value);
            ability.value = (byte)value;
            result = ability;

            return result;
        }


        public void SaveVDSValue(VDSValue ability , int index)
        {
            String sectionName = String.Empty;
            switch (index)
            {
                case 6:
                    sectionName = String.Format("SPEED_ACCU_ENABLED");
                    break;
                case 7:
                    sectionName = String.Format("LENGTH_ACCU_ENABLED");
                    break;
                case 8:
                    sectionName = String.Format("SPEED_CALCU_ENABLED");
                    break;
                case 9:
                    sectionName = String.Format("LENGTH_CALCU_ENABLED");
                    break;
                case 21:
                    sectionName = String.Format("REVERSE_RUN_ENABLED");
                    break;

            }
            AppConfiguration.SetAppConfig(sectionName, ability.value.ToString());
        }

        public IExOPData ReadSpeedLoopDimension()
        {
            IExOPData result = null;

            SpeedLoopDimension dimension = new SpeedLoopDimension();
            for(int i=0;i< dimension.dimension.Length;i++)
            {
                int value = 0;

                if (i < 16)
                    value = 45;
                else
                    value = 18;

                int.TryParse(AppConfiguration.GetAppConfig(String.Format($"LOOP_DIMENSION_{i + 1}")), out value);
                dimension.dimension[i] = (byte)value;
            }

            result = dimension;
            return result;
        }


        public void SaveSpeedLoopDimension(SpeedLoopDimension dimension)
        {
            for (int i = 0; i < dimension.dimension.Length; i++)
            {
                AppConfiguration.SetAppConfig(String.Format($"LOOP_DIMENSION_{i + 1}"), dimension.dimension[i].ToString());
            }
        }

        public IExOPData ReadPollingThreshold(byte index)
        {
            IExOPData result = null;
            String sectionName = String.Empty;
            PollingThreshold threshold = new PollingThreshold(index);

            int value = 0;
            switch(index)
            {
                case 11:
                    value = 25;
                    sectionName = String.Format("TRAFFIC_THRESHOLD");
                    break;
                case 12:
                    value = 175;
                    sectionName = String.Format("SPEED_THRESHOLD");
                    break;
                case 13:
                    value = 250;
                    sectionName = String.Format("LENGTH_THRESHOLD");
                    break;
            }
            int.TryParse(AppConfiguration.GetAppConfig(sectionName), out value);
            threshold.threshold = (byte)value;
            result = threshold;
            return result;
        }


        public void SavePollingThreshold(PollingThreshold threshold, int index)
        {
            String sectionName = String.Empty;
            switch (index)
            {
                case 11:
                    sectionName = String.Format("TRAFFIC_THRESHOLD");
                    break;
                case 12:
                    sectionName = String.Format("SPEED_THRESHOLD");
                    break;
                case 13:
                    sectionName = String.Format("LENGTH_THRESHOLD");
                    break;
            }

            AppConfiguration.SetAppConfig(sectionName, threshold.threshold.ToString());
        }

        public IExOPData ReadIncidentDetectThreshold()
        {
            IExOPData result = null;

            IncidentDetectThreshold threshold = new IncidentDetectThreshold();

            int value = 5;
            int.TryParse(AppConfiguration.GetAppConfig("INCIDENT_EXECUTE_CYCLE"), out value);
            threshold.incidentCycle = (byte)value;

            value = 2;
            int.TryParse(AppConfiguration.GetAppConfig("PERSISTENCE_PERIOD"), out value);
            threshold.period = (byte)value;


            value = 1;
            int.TryParse(AppConfiguration.GetAppConfig("ALGORITHM"), out value);
            threshold.algorithm = (byte)value;


            value = 0;
            int.TryParse(AppConfiguration.GetAppConfig("K_FACTOR_1"), out value);
            threshold.kfactor_1 = (byte)value;

            value = 0;
            int.TryParse(AppConfiguration.GetAppConfig("K_FACTOR_2"), out value);
            threshold.kfactor_2 = (byte)value;

            for(int i=0;i<threshold.threshold.Length;i++)
            {
                value = 100;
                int.TryParse(AppConfiguration.GetAppConfig($"LOOP_THRESHOLD_{i+1}"), out value);
                threshold.threshold[i] = (UInt16)value;
            }


            result = threshold;
            return result;
        }


        public void SaveIncidentDetectThreshold(IncidentDetectThreshold threshold)
        {

            AppConfiguration.SetAppConfig("INCIDENT_EXECUTE_CYCLE", threshold.incidentCycle.ToString());

            AppConfiguration.SetAppConfig("PERSISTENCE_PERIOD", threshold.period.ToString());

            AppConfiguration.SetAppConfig("ALGORITHM", threshold.algorithm.ToString());

            AppConfiguration.SetAppConfig("K_FACTOR_1", threshold.kfactor_1.ToString());

            AppConfiguration.SetAppConfig("K_FACTOR_2", threshold.kfactor_2.ToString());

            for (int i = 0; i < threshold.threshold.Length; i++)
            {
                AppConfiguration.SetAppConfig(String.Format($"LOOP_THRESHOLD_{i + 1}") , threshold.threshold[i].ToString());

            }
        }




        public IExOPData ReadStuckThreshold()
        {
            IExOPData result = null;
            StuckThreshold threshold = new StuckThreshold();

            int value = 120;
            int.TryParse(AppConfiguration.GetAppConfig("HIGH_TRAFFIC_DURATION"), out value);
            threshold.highTrafficDuration = (byte)value;

            value = 114;
            int.TryParse(AppConfiguration.GetAppConfig("HIGH_ON_DURATION"), out value);
            threshold.highOnDuration = (byte)value;

            value = 48;
            int.TryParse(AppConfiguration.GetAppConfig("HIGH_OFF_DURATION"), out value);
            threshold.highOffDuration = (byte)value;


            value = 240;
            int.TryParse(AppConfiguration.GetAppConfig("LOW_TRAFFIC_DURATION"), out value);
            threshold.lowTrafficDuration = (byte)value;

            value = 156;
            int.TryParse(AppConfiguration.GetAppConfig("LOW_ON_DURATION"), out value);
            threshold.lowOnDuration = (byte)value;

            value = 1;
            int.TryParse(AppConfiguration.GetAppConfig("LOW_OFF_DURATION"), out value);
            threshold.lowOffDuration = (byte)value;

            result = threshold;
            return result;
        }

        public void SaveStuckThreshold(StuckThreshold threshold)
        {


            AppConfiguration.SetAppConfig("HIGH_TRAFFIC_DURATION", threshold.highTrafficDuration.ToString());

            AppConfiguration.SetAppConfig("HIGH_ON_DURATION", threshold.highOnDuration.ToString());

            AppConfiguration.SetAppConfig("HIGH_OFF_DURATION", threshold.highOffDuration.ToString());



            AppConfiguration.SetAppConfig("LOW_TRAFFIC_DURATION", threshold.lowTrafficDuration.ToString());

            AppConfiguration.SetAppConfig("LOW_ON_DURATION", threshold.lowOnDuration.ToString());

            AppConfiguration.SetAppConfig("LOW_OFF_DURATION", threshold.lowOffDuration.ToString());

        }

        public IExOPData ReadOscillationThreshold(int index)
        {
            IExOPData result = null;
            VDSValue threshold = new VDSValue((byte)index);

            int value = 0;

            PollingCycle cycle = (PollingCycle)VDSParam[2];

            if (cycle!=null)
            {
                switch(cycle.cycle)
                {
                    case 20:
                        value = 30;
                        break;
                    case 30:
                        value = 50;
                        break;
                    default:
                        value = 50;
                        break;
                }
            }

            int.TryParse(AppConfiguration.GetAppConfig("OSCILLATION_THRESHOLD"), out value);
            threshold.value = (byte)value;
            result = threshold;
            return result;
        }


        public void SaveOscillationThreshold(VDSValue threshold , int index)
        {
            AppConfiguration.SetAppConfig("OSCILLATION_THRESHOLD", threshold.value.ToString());
        }

        public IExOPData ReadAutoSyncPeriod(int index)
        {
            IExOPData result;
            VDSValue threshold = new VDSValue((byte)index);
            int value = 1;
            int.TryParse(AppConfiguration.GetAppConfig("AUTO_SYNC_PERIOD"), out value);
            threshold.value = (byte)value;
            result = threshold;
            return result;
        }

        public void SaveAutoSyncPeriod(VDSValue threshold)
        {
            AppConfiguration.SetAppConfig("AUTO_SYNC_PERIOD", threshold.value.ToString());
        }


        public IExOPData ReadSimulationTemplate()
        {
            IExOPData result = null;

            SimulationTemplate template = new SimulationTemplate();

            int value = 1;
            int.TryParse(AppConfiguration.GetAppConfig("DATA_STREAM_NUMBER_1"), out value);
            template.dataStreamNo1 =(byte) value;

            value = 0;
            int.TryParse(AppConfiguration.GetAppConfig("SIMULATION_ENABLED_1"), out value);
            template.simulationEnabled1 = (byte)value;

            value = 30;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_LENGTH_1"), out value);
            template.vehicleLength1 = (byte)value;


            value = 100;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_SPEED_1"), out value);
            template.speed1 = (byte)value;


            value = 40;
            int.TryParse(AppConfiguration.GetAppConfig("HEADWAY_1"), out value);
            template.headway1 = (byte)value;

            value = 45;
            int.TryParse(AppConfiguration.GetAppConfig("LOOP_DISTANCE_1"), out value);
            template.distance1 = (byte)value;


            value = 18;
            int.TryParse(AppConfiguration.GetAppConfig("LOOP_LENGTH_1"), out value);
            template.loopLength1 = (byte)value;



            value = 1;
            int.TryParse(AppConfiguration.GetAppConfig("DATA_STREAM_NUMBER_2"), out value);
            template.dataStreamNo2 = (byte)value;

            value = 0;
            int.TryParse(AppConfiguration.GetAppConfig("SIMULATION_ENABLED_2"), out value);
            template.simulationEnabled2 = (byte)value;

            value = 30;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_LENGTH_2"), out value);
            template.vehicleLength2 = (byte)value;


            value = 100;
            int.TryParse(AppConfiguration.GetAppConfig("VEHICLE_SPEED_2"), out value);
            template.speed2 = (byte)value;


            value = 40;
            int.TryParse(AppConfiguration.GetAppConfig("HEADWAY_2"), out value);
            template.headway2 = (byte)value;

            value = 45;
            int.TryParse(AppConfiguration.GetAppConfig("LOOP_DISTANCE_2"), out value);
            template.distance2 = (byte)value;


            value = 18;
            int.TryParse(AppConfiguration.GetAppConfig("LOOP_LENGTH_2"), out value);
            template.loopLength2 = (byte)value;



            result = template;

            return result;
        }


        public void SaveSimulationTemplate(SimulationTemplate template)
        {


            AppConfiguration.SetAppConfig("DATA_STREAM_NUMBER_1", template.dataStreamNo1.ToString());

            AppConfiguration.SetAppConfig("SIMULATION_ENABLED_1", template.simulationEnabled1.ToString());

            AppConfiguration.SetAppConfig("VEHICLE_LENGTH_1", template.vehicleLength1.ToString());

            AppConfiguration.SetAppConfig("VEHICLE_SPEED_1", template.speed1.ToString());

            AppConfiguration.SetAppConfig("HEADWAY_1", template.headway1.ToString());

            AppConfiguration.SetAppConfig("LOOP_DISTANCE_1", template.distance1.ToString());

            AppConfiguration.SetAppConfig("LOOP_LENGTH_1", template.loopLength1.ToString());




            AppConfiguration.SetAppConfig("DATA_STREAM_NUMBER_2", template.dataStreamNo2.ToString());

            AppConfiguration.SetAppConfig("SIMULATION_ENABLED_2", template.simulationEnabled2.ToString());

            AppConfiguration.SetAppConfig("VEHICLE_LENGTH_2", template.vehicleLength2.ToString());

            AppConfiguration.SetAppConfig("VEHICLE_SPEED_2", template.speed2.ToString());

            AppConfiguration.SetAppConfig("HEADWAY_2", template.headway2.ToString());

            AppConfiguration.SetAppConfig("LOOP_DISTANCE_2", template.distance2.ToString());

            AppConfiguration.SetAppConfig("LOOP_LENGTH_2", template.loopLength2.ToString());


        }

        private int KorExClientConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                _selfSync = true; // 접속시 sync 명령 받기 전까지는 자체 sync 
                _connectionDateTime = DateTime.Now; // 접속 시간  저장
                _korExCenter._status = SOCKET_STATUS.CONNECTED;
                sessionState = KOR_EX_SESSION_STATE.SESSION_INIT;
                session._socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KorExReadCallback), session);

                strLog = String.Format("제어기-->안산 센터({0}:{1}  접속 성공)", VDSConfig.korExConfig.centerAddress, VDSConfig.korExConfig.centerPort);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                nResult = 1;
            }
            catch (Exception ex)
            {
                _korExCenter._status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private void SendCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            try
            {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.  
                int bytesSent = socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        public void KorExReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            String content = String.Empty;
            String strLog;

            SessionContext session = (SessionContext)ar.AsyncState;
            try
            {
                // Read data from the client socket.
                int bytesRead = session._socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    strLog = String.Format("도로공사 센터--> 제어기 ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    byte[] packet = new byte[bytesRead];
                    Array.Copy(session.buffer, 0, packet, 0, bytesRead);
                    int i = 0;
                    while (i < packet.Length)
                    {
                        if (_prevDataFrame == null)
                        {
                            _prevDataFrame = new ExDataFrame();
                            _prevDataFrame.bRequstFrame = true; // 기본적으로 센터의 request 를 처리하기 때문에 true 로 설정 .
                        }

                        i += _prevDataFrame.Deserialize(packet, i);
                        if (_prevDataFrame.bDataCompleted)
                        {
                            WorkData workData = new WorkData();
                            ExDataFrameDefine.InitWorkData(ref workData);
                            workData.session = session;// _korExCenter._sessionContext;
                            workData.frame = _prevDataFrame;
                            AddWorkData(workData);
                            _prevDataFrame = null;
                        }
                        else
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                        }
                    }
                    _lastCheckSession = DateTime.Now; // 메시지 수신 최종 시간 저장 
                                                      //// Not all data received. Get more.  
                    session._socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KorExReadCallback), session);

                }
                else
                {
                    DeleteSessionContext(session);
                    _selfSync = true;
                }
            }
            catch (Exception ex)
            {
                if (session._type == CLIENT_TYPE.KICT_EVNT_CLIENT)
                {
                    Console.WriteLine("event client error22");
                }
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        public int StartWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            StartMAClient();
            StartProcessTrafficDataEventThread();
            StartProcessWorkDataThread();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int StopWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            if (VDSConfig.USE_DB_THREAD)
            {
                if (_bProcessing)
                {
                    _bProcessing = false;

                    trafficEventThreadExitEvent.WaitOne();
                    threadExitEvent.WaitOne();

                    strLog = String.Format("안산센터 평가 서버 처리 쓰레드 종료 OK");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
            }
            StopMAClient();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int StopDBUpdateThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            if (_bDBProcessing)
            {
                _bDBProcessing = false;
                dbThreadExitEvent.WaitOne();

                strLog = String.Format("DB Update 쓰레드 종료 OK");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;

        }


        public override int ProcessWorkData(Object work)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리-GUID={((WorkData)(work)).guid} "));
            WorkData workData = (WorkData)work;
            int nResult = 0;
            switch (workData.frame.opCode)
            {

                // CSN 인증 요청
                case ExDataFrameDefine.OP_CSN_CHECK_COMMAND:
                    ProcessCSNCheckCommand(workData);
                    break;

                case ExDataFrameDefine.OP_CHECK_SESSION_COMMAND:
                    ProcessCheckSessionCommand(workData);
                    break;

                // 제어기 동기화 
                case ExDataFrameDefine.OP_SYNC_VDS_COMMAND:
                    ProcessSyncVDSCommand(workData);
                    break;

                // 교통 데이터 수집 요청
                case ExDataFrameDefine.OP_TRAFFIC_DATA_COMMAND:
                    ProcessTrafficDataCommand(workData);
                    break;

                case ExDataFrameDefine.OP_SPEED_DATA_COMMAND:
                    ProcessSpeedDataCommand(workData);
                    break;

                case ExDataFrameDefine.OP_VEHICLE_LENGTH_COMMAND:
                    //ProcessVehicleLengthDataCommand(workData);
                    break;
                case ExDataFrameDefine.OP_ACCU_TRAFFIC_COMMAND:
                    ProcessAccuTrafficDataCommand(workData);
                    break;

                case ExDataFrameDefine.OP_VDS_RESET_COMMAND:
                    ProcessResetControllerCommand(workData);
                    break;

                case ExDataFrameDefine.OP_VDS_INIT_COMMAND:
                    ProcessInitControllerCommand(workData);
                    break;

                case ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND:
                    ProcessParamDownloadCommand(workData);
                    break;


                case ExDataFrameDefine.OP_PARAM_UPLOAD_COMMAND:
                    ProcessParamUploadCommand(workData);
                    break;



                case ExDataFrameDefine.OP_ONLINE_STATUS_COMMAND:
                    ProcessOnlineStatusCommand(workData);
                    break;


                case ExDataFrameDefine.OP_MEMORY_STATUS_COMMAND:
                    ProcessMemoryStatusCommand(workData);
                    break;



                case ExDataFrameDefine.OP_MESSAGE_ECHO_COMMAND:
                    ProcessMessageEchoCommand(workData);
                    break;



                case ExDataFrameDefine.OP_SEQ_TRANSFER_COMMAND:
                    ProcessCheckSequenceCommand(workData);
                    break;



                case ExDataFrameDefine.OP_VDS_VERSION_COMMAND:
                    ProcessVDSVersionCommand(workData);
                    break;





                case ExDataFrameDefine.OP_INDIV_TRAFFIC_COMMAND:
                    ProcessIndivTrafficDataCommand(workData); 
                    break;


                // RTC 변경 요청
                case ExDataFrameDefine.OP_CHANGE_RTC_COMMAND:
                    ProcessChangeRTCCommand(workData); 
                    break;

                // 역주행 응답 처리
                case ExDataFrameDefine.OP_REVERSE_RUN_COMMAND:
                    ProcessReverseRunCommand(workData); 
                    break;


                // 상태 정보 요청 처리
                case ExDataFrameDefine.OP_SYSTEM_STATUS_COMMAND:
                    ProcessSystemStatusCommand(workData);
                    break;

                // Fan/Heater 동작 온도 설정 
                case ExDataFrameDefine.OP_SET_TEMPERATURE_COMMAND:
                    ProcessSetTemperatureCommand(workData);
                    break;


                    //case ExDataFrameDefine.OP_TRAFFIC_THRESHOLD_COMMAND:
                    //    ProcessTrafficThresholdCommand(workData);
                    //    break;
                    //case ExDataFrameDefine.OP_HW_STATUS_COMMAND:
                    //    ProcessControllerStatusCommand(workData);
                    //    break;











            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int SetRequestFrame(ref ExDataFrame request,byte opCode, IExOPData dataFrame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            byte[] controllerKind = new byte[2];
            try
            {
                // SENDER IP
                // 제어기 IP 설정
                //2001:0230:abcd:ffff:0000:0000:ffff:1111
                Array.Copy(Utility.IPAddressToKorExFormat(VDSConfig.controllerConfig.IpAddress), 0, request.senderIP, 0, 16);

                // DESTINATION IP
                Array.Copy(Utility.IPAddressToKorExFormat(VDSConfig.korExConfig.centerAddress), 0, request.destinationIP, 0, 16);

                //// CONTROLLER KIND
                //controllerKind = Utility.StringToByte("VD");
                //Array.Copy(controllerKind, 0, request.controllerKind, 0, 2);

                // CSN 
                Utility.GetCSN(ref request.csn);

                // OP CODE
                request.opCode = opCode;

                //(dataFrame as ExRequest).transactionNo = Utility.GetTransactionId();

                request.opData = dataFrame;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int SetACKResponseFrame(ExDataFrame request, ref ExDataFrame response, byte resultCode, byte errorCode)
        {
            ExResponse resultData = new ExResponse();
            resultData.resultCode = resultCode;
            if(resultCode== ExDataFrameDefine.NAK_CSN_ERROR)
            {
                resultData.errorCode = errorCode;
            }
            return SetResponseFrame(request, ref response, resultData);
        }
        public int SetResponseFrame(ExDataFrame request, ref ExDataFrame response, IExOPData opData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                // SENDER IP
                // 제어기 IP 설정
                //2001:0230:abcd:ffff:0000:0000:ffff:1111
                Array.Copy(Utility.IPAddressToKorExFormat(VDSConfig.controllerConfig.IpAddress), 0, response.senderIP, 0, 16);

                // DESTINATION IP
                Array.Copy(request.senderIP, 0, response.destinationIP, 0, 16);

                // CSN 
                Utility.GetCSN(ref response.csn);

                // OP CODE
                response.opCode = request.opCode;
                response.opData = opData;

                // TRANSACTION NUMBER 
                //if(request.opData !=null)  
                //{
                //    if(response.opData !=null)
                //    {
                //        Array.Copy((request.opData as ExRequest).transactionNo, 0, (response.opData as ExResponse).transactionNo,0,8);
                //        (response.opData as ExResponse).resultCode = resultCode;
                //        (response.opData as ExResponse).status = GetVDSStatus(request);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int SetResponseFrame(ExDataFrame request, ref ExDataFrame response, byte resultCode, byte errorCode)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                // SENDER IP
                // 제어기 IP 설정
                //2001:0230:abcd:ffff:0000:0000:ffff:1111
                Array.Copy(Utility.IPAddressToKorExFormat(VDSConfig.controllerConfig.IpAddress), 0, response.senderIP, 0, 16);
                
                // DESTINATION IP
                Array.Copy(request.senderIP, 0, response.destinationIP,0, 16);


                // CSN 
                Utility.GetCSN(ref response.csn);



                // OP CODE
                response.opCode = request.opCode;

                // TRANSACTION NUMBER 
                //if(request.opData !=null)  
                //{
                //    if(response.opData !=null)
                //    {
                //        Array.Copy((request.opData as ExRequest).transactionNo, 0, (response.opData as ExResponse).transactionNo,0,8);
                //        (response.opData as ExResponse).resultCode = resultCode;
                //        (response.opData as ExResponse).status = GetVDSStatus(request);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int Send(SessionContext session, byte[] byteData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 1;
            String strLog;
            try
            {
                // Begin sending the data to the remote device.  
                session._socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), session._socket);
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

                DeleteSessionContext(session);
                nResult = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        
        public int ProcessCSNCheckCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);
                //ExResponse resultData = new ExResponse();
                //resultData.resultCode = ExDataFrameDefine.ACK_NORMAL;
                //SetResponseFrame(workData.frame, ref response, resultData);
                byte[] data = response.Serialize();
                Send(workData.session, data);

                sessionState = KOR_EX_SESSION_STATE.SESSION_ONLINE;
                _korExCenter._sessionContext = workData.session;
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessSyncVDSCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                // Sync Message 받았을 때는 응답메시지는 보내지 않는다.
                // 수집버퍼--> 저장 버퍼로 데이터 전환
                // 수집버퍼는 다시 초기화 후 재개 
                // 저장 버퍼에서 교통량 계산하여 저장버퍼에 저장한다. 
                _lastFrameNo = (workData.frame.opData as ControllerSyncRequest).frameNo;
                _selfSync = false;
                PrepareCenterData();
                _currentSyncDateTime = DateTime.Now;
                //curDate = DateTime.Now;//.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //TargetSummary targetDB = new TargetSummary(VDSConfig.VDS_DB_CONN);
                //TRAFFIC_DATA_STAT stat = new TRAFFIC_DATA_STAT();

                //stat.I_START_DATE = _lastTrafficDataCheck.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //stat.I_END_DATE = curDate.ToString(VDSConfig.RADAR_TIME_FORMAT);

                //// 현재 시간 저장 
                //_lastTrafficDataCheck = curDate;

                //SP_RESULT spResult;
                //trafficDataList = targetDB.GetTrafficDataStat(stat, out spResult).ToList();
                //if (!spResult.IS_SUCCESS)
                //{
                //    Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                //}
                //else
                //{
                //    nResult = 1;
                //}
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }


        public int ProcessTrafficDataCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            TimeSpan ts;
            try
            {
                ExDataFrame response = new ExDataFrame();
                TrafficDataExResponse trafficData = GetTrafficDataExResponse();
                SetResponseFrame(workData.frame, ref response, trafficData);
                byte[] data = response.Serialize();
                Send(workData.session, data);

                //ExDataFrame response = new ExDataFrame();
                //TrafficDataResponse trafficData = new TrafficDataResponse();

                //trafficData.frameNo = _lastFrameNo;

                //// 루프 장애 정보 루프당 2 bit . 32개까지. 즉 64 비트 --> 8 bytes 
                //// 레이더식 방식이기에 루프 장애 정보는 편의상 정상으로 설정
                //for (int i = 0; i < 8; i++)
                //    trafficData.detector.errorInfo[i] = ExDataFrameDefine.LOOP_NORMAL;

                //// 루프 유고 정보 루프당 1 bit. 32 bit --> 4 bytes
                //for (int i = 0; i < 4; i++)
                //    trafficData.detector.accidentInfo[i] = ExDataFrameDefine.INCIDENT_NORMAL;

                //// 루프 갯수 = 차선수 * 2 (쌍루프로 설정)
                //int laneCount = VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count;
                //trafficData.detector.sensorCount = (byte)(VDSConfig.controllerConfig.DeviceType == 2 ? VDSConfig.korExConfig.sensorCount : 1); //  (laneCount * 2);
                //trafficData.detector.laneCount = (byte)laneCount;

                //DetectInfo detectInfo = new DetectInfo();
                //// _activeCenterDataIndex : 현재 저장되는 버퍼 인덱스. 따라서 반대 인덱스가 저장되어있는 인덱스
                //ts = _currentSyncDateTime - _prevSyncDateTime;
                //GetTrafficDataSummary(_centerData[(_activeCenterDataIndex + 1) % 2], out detectInfo.trafficCount, out detectInfo.occupyTime, ts);
                //trafficData.detector.detectInfoList.Add(detectInfo);
                //for (int i=0;i<laneCount;i++)
                //{

                //    var trafficStatData = GetTrafficDataByLane(i + 1, _centerData[(_activeCenterDataIndex + 1) % 2]);
                //    //var trafficStatData = trafficDataList.Where(x => x.LANE == (i + 1)).FirstOrDefault();
                //    LaneInfo lane = new LaneInfo();

                //    if (trafficStatData != null)
                //    {
                //        lane.averageLength = (byte)trafficStatData.AVG_LENGTH;
                //        lane.averageSpeed = (byte)trafficStatData.AVG_SPEED;
                //    }
                //    Console.WriteLine($"Lane:{i+1} 평균 길이:{lane.averageLength}, 평균속도:{lane.averageSpeed} ");
                //    trafficData.detector.laneInfoList.Add(lane);
                //}

                //response.opData = trafficData;
                //response._totalLength = 13 + 8 + 4 + 1 + 3* trafficData.detector.detectInfoList.Count + 1 + 2* trafficData.detector.laneInfoList.Count;
                ////SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL);

                //byte[] data = response.Serialize();
                //Send(workData.session, data);

                _prevSyncDateTime = _currentSyncDateTime; // 현재 동기화 시간 저장

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }


        public int ProcessSpeedDataCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SpeedDataExResponse speedData = GetSpeedDataExResponse();
                SetResponseFrame(workData.frame, ref response, speedData);
                byte[] data = response.Serialize();
                Send(workData.session, data);


                //SpeedDataResponse speedData = new SpeedDataResponse();
                //speedData.laneNo = (workData.frame.opData as SpeedDataRequest).laneNo;

                //if(speedData.laneNo >=1 && speedData.laneNo<=16)
                //{
                //    for (int i = 0; i < 12; i++)
                //    {
                //        speedData.speedData.speedCategory[i] = speedDataStat[speedData.laneNo - 1].speedCategory[i];
                //        speedDataStat[speedData.laneNo - 1].speedCategory[i] = 0;// 리셋
                //    }
                //}
                
                //response.opData = speedData;
                //response._totalLength = 12 + 1 + 24;
                ////SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                //byte[] data = response.Serialize();
                //Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessVehicleLengthDataCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                VehicleLengthDataResponse lengthData = new VehicleLengthDataResponse();
                lengthData.laneNo = (workData.frame.opData as VehicleLengthDataRequest).laneNo;

                if (lengthData.laneNo >= 1 && lengthData.laneNo <= 16)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        lengthData.lengthData.lengthCategory[i] = lengthDataStat[lengthData.laneNo - 1].lengthCategory[i];
                        lengthDataStat[lengthData.laneNo - 1].lengthCategory[i] = 0; // 리셋 
                    }
                }


                //curDate = DateTime.Now;
                //TargetSummary targetDB = new TargetSummary(VDSConfig.VDS_DB_CONN);
                //LENGTH_DATA_STAT stat = new LENGTH_DATA_STAT();

                //stat.I_START_DATE = _lastLengthDataCheck.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //stat.I_END_DATE = curDate.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //stat.LANE = lengthData.laneNo;
                //SP_RESULT spResult;
                //var lengthDataResult = targetDB.GetLengthDataStat(stat, VDSConfig.LENGTH_CATEGORY, out spResult);
                //// 현재 시간 저장 
                //_lastLengthDataCheck = curDate;

                //if (lengthDataResult != null)
                //{
                //    lengthData.lengthData.lengthCategory[0] = (ushort)(lengthDataResult.CATEGORY_1);
                //    lengthData.lengthData.lengthCategory[1] = (ushort)(lengthDataResult.CATEGORY_2);
                //    lengthData.lengthData.lengthCategory[2] = (ushort)(lengthDataResult.CATEGORY_12);
                   
                //}

                response.opData = lengthData;
                response._totalLength = 12 + 1 + 6;
                //SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessAccuTrafficDataCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                AccuTrafficDataResponse accuTrafficData = new AccuTrafficDataResponse();


                for (int i = 0;i<16;i++)
                {
                    accuTrafficData.volumData[i] =  accuTrafficDataStat[i];
                    accuTrafficData.volumData[i+1] = accuTrafficDataStat[i];
                    accuTrafficDataStat[i] = 0; // reset 
                }

                SetResponseFrame(workData.frame, ref response, accuTrafficData);
                byte[] data = response.Serialize();
                Send(workData.session, data);


                //curDate = DateTime.Now;
                //TargetSummary targetDB = new TargetSummary(VDSConfig.VDS_DB_CONN);

                //ACCU_TRAFFIC_DATA_STAT stat = new ACCU_TRAFFIC_DATA_STAT();

                //stat.I_START_DATE = _lastAccuTrafficDataCheck.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //stat.I_END_DATE = curDate.ToString(VDSConfig.RADAR_TIME_FORMAT);
                //SP_RESULT spResult;
                //var accuTrafficDataResult = targetDB.GetAccuTrafficDataStat(stat, VDSConfig.LENGTH_CATEGORY, out spResult);
                ////// 현재 시간 저장 
                //_lastAccuTrafficDataCheck = curDate;

                //if (accuTrafficDataResult != null)
                //{
                //    int i = 0;
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_1);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_2);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_3);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_4);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_5);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_6);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_7);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_8);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_9);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_10);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_11);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_12);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_13);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_14);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_15);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_16);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_17);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_18);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_19);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_20);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_21);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_22);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_23);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_24);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_25);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_26);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_27);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_28);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_29);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_30);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_31);
                //    accuTrafficData.volumData[i++] = (ushort)(accuTrafficDataResult.LOOP_32);


                //}

                //response.opData = accuTrafficData;
                //response._totalLength = 12 + 64;
                //SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                //byte[] data = response.Serialize();
                //Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }


        public int ProcessTrafficThresholdCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SetErrorThresholdResponse thresholdData = new SetErrorThresholdResponse();
                thresholdData.threshold = (workData.frame.opData as SetErrorThresholdRequest).threshold;


                response.opData = thresholdData;
                response._totalLength = 12 + 1 ;
                //SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessControllerStatusCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                ControllerStatusResponse statusData = new ControllerStatusResponse();

                statusData.powerSupplyCount = (byte) VDSConfig.korExConfig.powerSupplyCount;
                statusData.powerSupplyStatus = VDSRackStatus.GetPowerSupplyStatus() ;

                statusData.boardCount = (byte)VDSConfig.korExConfig.boardCount;
                statusData.boardStatus = VDSRackStatus.GetBoardStatus();
                
                response.opData = statusData;
                response._totalLength = 12 + 5;
                //SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessResetControllerCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                //ResetControllerResponse resetData = new ResetControllerResponse();
                SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);

                //response.opData = resetData;
                //response._totalLength = 12  ;
                //SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessInitControllerCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

                //ExDataFrame response = new ExDataFrame();
                //InitControllerResponse initData = new InitControllerResponse();

                ////_radarManager
                //response.opData = initData;
                //response._totalLength = 12;
                ////SetResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL_RESULT);

                //byte[] data = response.Serialize();
                //Send(workData.session, data);
                //nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessParamDownloadCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            DateTime curDate;
            try
            {
                ExDataFrame response = new ExDataFrame();
                //ParamDownloadResponse paramResponse = new ParamDownloadResponse();

                //response.opData = paramResponse;

                //(response.opData as ExResponse).resultCode = ProcessParamDownloadIndex((ParamDownloadRequest)workData.frame.opData);
                byte errorCode = ExDataFrameDefine.NAK_NO_ERROR;
                byte resultCode = ProcessParamDownloadIndex((ParamDownloadRequest)workData.frame.opData, out errorCode);

                SetACKResponseFrame(workData.frame, ref response, resultCode, errorCode);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessParamUploadCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {

                ParamUploadRequest request = (ParamUploadRequest)workData.frame.opData;
                if(request!=null)
                {
                    ExDataFrame response = new ExDataFrame();
                    ParamUploadResponse paramResponse = new ParamUploadResponse();
                    paramResponse.paramIndex = request.paramIndex;
                    IExOPData param = ProcessParamUploadIndex((ParamUploadRequest)workData.frame.opData);
                    paramResponse.param = param;


                    SetResponseFrame(workData.frame, ref response, paramResponse);
                    byte[] data = response.Serialize();
                    Send(workData.session, data);
                    nResult = 1;
                }
                
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }
        

        public byte ProcessParamDownloadIndex(ParamDownloadRequest request, out byte errorCode)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = 0;
            errorCode = ExDataFrameDefine.NAK_NO_ERROR;
            switch (request.paramIndex)
            {
                case 1: // Loop Detector Configuration : 루프 검지기 지정 4bytes
                    //result = ProcessLoopDetectorConfig((ParamLoopConfig)(request.param), request.paramIndex-1);
                    result = ProcessLaneConfig((ParamLaneConfig)(request.param), request.paramIndex - 1);
                    break;
                case 2: // Speed Loop   Configuration : 속도 검지 루프 지정
                    //result = ProcessSpeedLoopConfig((SpeedLoopConfig)(request.param), request.paramIndex - 1);
                    break;

                case 3: // Polling Cycle : 수집 주기
                    result = ProcessPollingCycle((PollingCycle)(request.param), request.paramIndex - 1);
                    break;

                case 4: // Vehicle   Definition : 차량 감응시간 정의
                    //result = ProcessVehiclePulseNumberCommand((VehiclePulseNumber)(request.param), request.paramIndex - 1);
                    break;

                case 5: // Speed   Categories : 차량 속도 구분
                    result = ProcessSpeedCategoryCommand((SpeedCategory)(request.param), request.paramIndex - 1);
                    break;

                case 6: // Vehicle Length   Categories : 차량 길이 구분
                    result = ProcessLengthCategoryCommand((LengthCategory)(request.param), request.paramIndex - 1);
                    break;
                case 7: // Speed Category   Accumulation Disable/Enable : 속도 별 누적 치 계산
                case 8: // Vehicle Length   Accumulation Disable/Enable : 차량길이 별 누적 치 계산
                case 9: // Speed   Calculation Disable/Enable : 속도 계산 가능 여부
                case 10: // Vehicle Length   Calculation Disable/Enable : 차량길이 계산 가능 여부
                    result = ProcessVDSAbilityCommand((VDSValue)(request.param), request.paramIndex - 1);
                    break;
                //case 11: // Speed Loop   Dimensions : 속도 Loop 간격 및 길이
                //    result = ProcessSpeedLoopDimensionCommand((SpeedLoopDimension)(request.param), request.paramIndex - 1);
                //    break;

                //case 12: // Upper Volume   Limit : 교통량 상한치
                //case 13: // Upper Speed   Limit : 차량 속도 상한치
                //case 14: // Upper Vehicle   Length Limit : 차량길이 상한치
                //    result = ProcessPollingThresholdCommand((PollingThreshold)(request.param), request.paramIndex - 1);
                //    break;
                //case 15: // Incident   Detection Threshold : 유고정의를 위한 임계치

                //    result = ProcessIncidentDetectThresholdCommand((IncidentDetectThreshold)(request.param), request.paramIndex - 1);
                //    break;
                //case 16: // Loop Detector   Stuck ON / OFF : Stuck   ON/OFF 임계치

                //    result = ProcessStuckThresholdCommand((StuckThreshold)(request.param), request.paramIndex - 1);
                //    break;
                case 17: // Loop Detector   Oscillation Threshold : Oscillation   임계치
                    result = ProcessOscillationThresholdCommand((VDSValue)(request.param), request.paramIndex - 1);
                    break;

                case 20: // Auto   Re-Synchronization Waiting Period
                    result = ProcessAutoSyncPeriodCommand((VDSValue)(request.param), request.paramIndex - 1);
                    break;

                case 21: // 역주행 사용 여부
                    //result = ProcessSimulationTemplateCommand((SimulationTemplate)(request.param), request.paramIndex - 1);
                    result = ProcessVDSAbilityCommand((VDSValue)(request.param), request.paramIndex - 1);

                    break;

                default:  // 18,19,22,23,24 : reserved 
                    break;

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        

        public int ProcessOnlineStatusCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                CheckOnlineStatusResponse onlineStatusRespose = new CheckOnlineStatusResponse();

                TimeSpan ts = DateTime.Now - _connectionDateTime;
                onlineStatusRespose.passedTime = (UInt32)ts.TotalSeconds;

                SetResponseFrame(workData.frame, ref response, onlineStatusRespose);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessMemoryStatusCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);
                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessMessageEchoCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                EchoMessageRequest echoRequest =(EchoMessageRequest)(workData.frame.opData );
                EchoMessageResponse echoResponse = new EchoMessageResponse();
                echoResponse.echoMessage = new byte[echoRequest.echoMessage.Length];
                Array.Copy(echoRequest.echoMessage, 0, echoResponse.echoMessage, 0, echoRequest.echoMessage.Length);

                SetResponseFrame(workData.frame, ref response, echoResponse);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessCheckSequenceCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                CheckSeqNoRequest seqRequest = (CheckSeqNoRequest)(workData.frame.opData);
                CheckSeqNoResponse seqResponse = new CheckSeqNoResponse();
                seqResponse.seqList = new byte[seqRequest.counter];
                for (byte i =0;i< seqRequest.counter;i++)
                {
                    if(seqRequest.baseNumber + i <255)
                        seqResponse.seqList[i] =(byte)(seqRequest.baseNumber + i +1);
                    else
                        seqResponse.seqList[i] = 254;
                }

                //response.opData = seqResponse;
                SetResponseFrame(workData.frame, ref response, seqResponse);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessVDSVersionCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                VDSVersionResponse versionResponse = new VDSVersionResponse();

                versionResponse.version = GetKorExVDSVersion();

                SetResponseFrame(workData.frame, ref response, versionResponse);

                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int ProcessIndivTrafficDataCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            TimeSpan ts;
            try
            {


                ExDataFrame response = new ExDataFrame();
                IndivTrafficDataResponse trafficData = new IndivTrafficDataResponse();
                trafficData.timeFrameNo = _lastFrameNo;
                foreach (var trafficEventObject in _centerData[(_activeCenterDataIndex + 1) % 2])
                {
                    TrafficDataEvent trafficEvent = trafficEventObject as TrafficDataEvent;
                    IndivTrafficData traffic = new IndivTrafficData();
                    traffic.lane = (byte) GetKorExLaneNo(trafficEvent);
                    ts = DateTime.Now - _currentSyncDateTime;
                    traffic.passTime = (byte) ts.TotalSeconds;
                    traffic.speed = (byte) trafficEvent.speed;
                    traffic.occupyTime = (UInt16) trafficEvent.occupyTime;
                    Utility.VEHICLE_LENGTH_CATEGORY lengthCategory = Utility.GetVehicleLengthCategory(trafficEvent.length / 100); // cm--> m 로
                    switch (lengthCategory)
                    {
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL:
                            traffic.category = 1;
                            break;
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE:
                            traffic.category = 2;
                            break;
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE:
                            traffic.category = 3;
                            break;
                    }
                    trafficData.trafficDataList.Add(traffic);
                }
                //response.opData = trafficData;
                SetResponseFrame(workData.frame, ref response, trafficData);
                byte[] data = response.Serialize();
                Send(workData.session, data);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }


        public int ProcessChangeRTCCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {
                ExDataFrame response = new ExDataFrame();
                RealTimeClock rtcInfo = (RealTimeClock)(workData.frame.opData);
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" RTC 변경 요청 시간 정보: {Utility.BCDtoString(rtcInfo.timeinfo)}"));
                nResult = Utility.SetOsTime(rtcInfo.timeinfo);
                if (nResult > 0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" RTC 변경 성공"));
                    SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" RTC 변경 실패"));
                    SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.NAK_ERROR, ExDataFrameDefine.NAK_INTERNAL_ERROR);
                }
                
                byte[] data = response.Serialize();
                Send(workData.session, data);
                nResult = 1;

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }


        public int ProcessReverseRunCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            TimeSpan ts;
            try
            {
                //Console.WriteLine("역주행 응답 메시지 처리 완료");
                

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int ProcessSystemStatusCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            TimeSpan ts;
            try
            {
                ExDataFrame response = new ExDataFrame();
                SystemStatus systemStatus = GetVDSRackStatusInKorEx();
                SetResponseFrame(workData.frame, ref response, systemStatus);
                byte[] data = response.Serialize();
                Send(workData.session, data);
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }


        public int ProcessSetTemperatureCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            TimeSpan ts;
            try
            {
                ExDataFrame response = new ExDataFrame();


                SetTemperatureRequest request = (SetTemperatureRequest)(workData.frame.opData);
                if(request!=null)
                {
                    // 팬/온도 설정 모듈 전송
                    CommuData commuData = Utility.GetCommuData(2, workData.session, workData.frame.opCode, workData);
                    nResult = AddCommuDataToForm(commuData);
                    SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);
                }
                else
                {
                    SetACKResponseFrame(workData.frame, ref response, ExDataFrameDefine.ACK_NORMAL, ExDataFrameDefine.NAK_NO_ERROR);
                }
                byte[] data = response.Serialize();
                Send(workData.session, data);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int ProcessCheckSessionCommand(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            try
            {


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;

        }

        public int GetParamLength(int paramIndex)
        {
            int result = 0;
            switch (paramIndex)
            {
                case 1: // Loop Detector Configuration : 루프 검지기 지정 4bytes
                    result = 4; 
                    break;
                case 2: // Speed Loop   Configuration : 속도 검지 루프 지정
                    result = 32;
                    break;

                case 3: // Polling Cycle : 수집 주기
                    result = 1;
                    break;

                case 4: // Vehicle   Definition : 차량 감응시간 정의
                    result = 2;
                    break;

                case 5: // Speed   Categories : 차량 속도 구분
                    result = 12;
                    break;

                case 6: // Vehicle Length   Categories : 차량 길이 구분
                    result = 3;
                    break;
                case 7: // Speed Category   Accumulation Disable/Enable : 속도 별 누적 치 계산
                case 8: // Vehicle Length   Accumulation Disable/Enable : 차량길이 별 누적 치 계산
                case 9: // Speed   Calculation Disable/Enable : 속도 계산 가능 여부
                case 10: // Vehicle Length   Calculation Disable/Enable : 차량길이 계산 가능 여부
                    result = 1;
                    break;
                case 11: // Speed Loop   Dimensions : 속도 Loop 간격 및 길이
                    result = 32;
                    break;

                case 12: // Upper Volume   Limit : 교통량 상한치
                case 13: // Upper Speed   Limit : 차량 속도 상한치
                case 14: // Upper Vehicle   Length Limit : 차량길이 상한치
                    result = 1;
                    break;
                case 15: // Incident   Detection Threshold : 유고정의를 위한 임계치

                    result = 71;
                    break;
                case 16: // Loop Detector   Stuck ON / OFF : Stuck   ON/OFF 임계치

                    result = 6;
                    break;
                case 17: // Loop Detector   Oscillation Threshold : Oscillation   임계치
                    result = 1;
                    break;

                case 20: // Auto   Re-Synchronization Waiting Period
                    result = 1;
                    break;

                case 21: // Simulation   Templates
                    result = 14;

                    break;

                default:  // 18,19,22,23,24 : reserved 
                    break;

            }
            return result;

        }

        public IExOPData ProcessParamUploadIndex(ParamUploadRequest request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            IExOPData result = null;
            if (request.paramIndex>=1 && request.paramIndex<25)
                result = VDSParam[request.paramIndex - 1]; 

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessLaneConfig(ParamLaneConfig config, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    (VDSParam[index] as ParamLaneConfig).laneInfo = config.laneInfo;
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessLoopDetectorConfig(ParamLoopConfig config, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if(index >= 0 && index < VDSParam.Length && VDSParam[index]!=null)
                {
                    (VDSParam[index] as ParamLoopConfig).loopInfo = config.loopInfo;
                }
                
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessSpeedLoopConfig(SpeedLoopConfig config, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if(config.loopNo.Length == 32)
                {
                    if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                    {
                        Array.Copy(config.loopNo, (VDSParam[index] as SpeedLoopConfig).loopNo, 32);
                    }

                    
                }
                else
                {
                    result = ExDataFrameDefine.NAK_DATALEN_ERROR;
                }
                
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessPollingCycle(PollingCycle polling, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    (VDSParam[index] as PollingCycle).cycle = polling.cycle;
                }

                
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessVehiclePulseNumberCommand(VehiclePulseNumber pulse, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    (VDSParam[index] as VehiclePulseNumber).presenceNumber = pulse.presenceNumber;
                    (VDSParam[index] as VehiclePulseNumber).noPresenceNumber = pulse.noPresenceNumber;
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessSpeedCategoryCommand(SpeedCategory category, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    Array.Copy(category.category, 0, (VDSParam[index] as SpeedCategory).category, 0, 12);
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessLengthCategoryCommand(LengthCategory category, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    Array.Copy(category.category, 0, (VDSParam[index] as LengthCategory).category, 0, 3);
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessVDSAbilityCommand(VDSValue ability, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {

                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    (VDSParam[index] as VDSValue).value = ability.value;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessSpeedLoopDimensionCommand(SpeedLoopDimension dimension, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    Array.Copy(dimension.dimension,0, (VDSParam[index] as SpeedLoopDimension).dimension,0,32);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessPollingThresholdCommand(PollingThreshold pollingThreshold, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {
                    (VDSParam[index] as PollingThreshold).threshold = pollingThreshold.threshold;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessIncidentDetectThresholdCommand(IncidentDetectThreshold incident, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {

                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {

                    (VDSParam[index] as IncidentDetectThreshold).incidentCycle = incident.incidentCycle;
                    (VDSParam[index] as IncidentDetectThreshold).period = incident.period;
                    (VDSParam[index] as IncidentDetectThreshold).algorithm = incident.algorithm;
                    (VDSParam[index] as IncidentDetectThreshold).kfactor_1 = incident.kfactor_1;
                    (VDSParam[index] as IncidentDetectThreshold).kfactor_2 = incident.kfactor_2;
                    Array.Copy(incident.threshold,0, (VDSParam[index] as IncidentDetectThreshold).threshold,0,32);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessStuckThresholdCommand(StuckThreshold stuck, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {

                    (VDSParam[index] as StuckThreshold).highTrafficDuration = stuck.highTrafficDuration;
                    (VDSParam[index] as StuckThreshold).highOnDuration = stuck.highOnDuration;
                    (VDSParam[index] as StuckThreshold).highOffDuration = stuck.highOffDuration;
                    (VDSParam[index] as StuckThreshold).lowTrafficDuration = stuck.lowTrafficDuration;
                    (VDSParam[index] as StuckThreshold).lowOnDuration = stuck.lowOnDuration;
                    (VDSParam[index] as StuckThreshold).lowOffDuration = stuck.lowOffDuration;
                    
                }


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessOscillationThresholdCommand(VDSValue oscilation, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {

//                    (VDSParam[index] as VDSValue).paramIndex = oscilation.paramIndex;
                    (VDSParam[index] as VDSValue).value = oscilation.value;
                }


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }

        public byte ProcessAutoSyncPeriodCommand(VDSValue period, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {

//                    (VDSParam[index] as VDSValue).paramIndex = period.paramIndex;
                    (VDSParam[index] as VDSValue).value = period.value;
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public byte ProcessSimulationTemplateCommand(SimulationTemplate template, int index)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte result = ExDataFrameDefine.ACK_NORMAL;
            try
            {
                if (index >= 0 && index < VDSParam.Length && VDSParam[index] != null)
                {

                    (VDSParam[index] as SimulationTemplate).dataStreamNo1 = template.dataStreamNo1;
                    (VDSParam[index] as SimulationTemplate).simulationEnabled1 = template.simulationEnabled1;
                    (VDSParam[index] as SimulationTemplate).vehicleLength1 = template.vehicleLength1;
                    (VDSParam[index] as SimulationTemplate).speed1 = template.speed1;
                    (VDSParam[index] as SimulationTemplate).headway1 = template.headway1;
                    (VDSParam[index] as SimulationTemplate).distance1 = template.distance1;
                    (VDSParam[index] as SimulationTemplate).loopLength1 = template.loopLength1;

                    (VDSParam[index] as SimulationTemplate).dataStreamNo2 = template.dataStreamNo2;
                    (VDSParam[index] as SimulationTemplate).simulationEnabled2 = template.simulationEnabled2;
                    (VDSParam[index] as SimulationTemplate).vehicleLength2 = template.vehicleLength2;
                    (VDSParam[index] as SimulationTemplate).speed2 = template.speed2;
                    (VDSParam[index] as SimulationTemplate).headway2 = template.headway2;
                    (VDSParam[index] as SimulationTemplate).distance2 = template.distance2;
                    (VDSParam[index] as SimulationTemplate).loopLength2 = template.loopLength2;



                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }
       
        public byte[] GetVDSStatus(ExDataFrame request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            byte[] result = new byte[2] ;
            try
            {
                //avogadro result = Utility.toBigEndianInt16(VDSRackStatus.GetKorExControllerStatus(request._csn, _parameterDownloaded, _selfSync));
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return result;
        }


        public override int SendTrafficData(TrafficDataEvent trafficDataEvent)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                // 도로공사 관련 처리 추가
                //Console.WriteLine("KorExManager---> sendTrafficData......");
                AddCenterData(trafficDataEvent);
                AddLocalData(trafficDataEvent);
                
                if(trafficDataEvent.reverseRunYN =="Y") // 역주행일 경우 센터에 통지 
                {
                    SendReverseRunNotify(trafficDataEvent);
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public void ResetCenterPeriodData()
        {
            DateTime curDate = DateTime.Now;
            TimeSpan expireTime = new TimeSpan(0, 0, 0, VDSConfig.korExConfig.centerPollingPeriod,0); // VDSConfig.CENTER_POLLILNG_PERIOD 초 후에 초기화
            DateTime checkDate = _lastResetCenterData.Add(expireTime);
            if(curDate > checkDate)
            {
                 PrepareCenterData();
            }
        }

        public void ResetLocalPeriodData()
        {
            DateTime curDate = DateTime.Now;
            TimeSpan expireTime = new TimeSpan(0, 0, 0, VDSConfig.korExConfig.localPollingPeriod, 0); // VDSConfig.LOCAL_POLLING_PERIOD 초 후에 초기화
            DateTime checkDate = _lastResetLocalData.Add(expireTime);
            if (curDate > checkDate)
            {
                ResetLocalData();
                Console.WriteLine($"self sync ={_selfSync} , _lastFrameNo ={_lastFrameNo}");
                if(_selfSync)
                    _lastFrameNo = Utility.GetLocalFrameNo();
            }

        }

        public void CheckSessionTime()
        {
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            DateTime curDate = DateTime.Now;
            TimeSpan expireTime = new TimeSpan(0, 0, 0, VDSConfig.korExConfig.checkSessionTime, 0); // VDSConfig.CENTER_POLLILNG_PERIOD 초 후에 초기화
            DateTime checkDate = _lastCheckSession.Add(expireTime);
            if (curDate > checkDate) // 5분 초과되었을 경우 통신 유효성 확인 메시지 전송
            {
                Console.WriteLine($"_lastCheckSession = {_lastCheckSession.ToString(VDSConfig.RADAR_TIME_FORMAT)}, VDSConfig.CHECK_SESSION_TIME= {VDSConfig.korExConfig.checkSessionTime}, checkDate = {checkDate.ToString(VDSConfig.RADAR_TIME_FORMAT)} , curDate = {curDate.ToString(VDSConfig.RADAR_TIME_FORMAT)}");
                RequestCheckSession();
                
                
            }
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public int RequestCheckSession()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if(sessionState == KOR_EX_SESSION_STATE.SESSION_ONLINE)
            {
                ExDataFrame frame = new ExDataFrame();
                //IExOPData dataFrame = new CheckSessionRequest();
                SetRequestFrame(ref frame, ExDataFrameDefine.OP_CHECK_SESSION_COMMAND, null);
                byte[] data = frame.Serialize();
                Send(_korExCenter._sessionContext, data);
            }
            
            _lastCheckSession = DateTime.Now;
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int GetTrafficDataSummary(List<Object> dataList, out byte trafficCount, out byte[] occupyTime, TimeSpan ts)
        {
            TrafficDataEvent trafficData;
            int totalTrafficCount = 0;
            int totalOccupyTime = 0;
            double ratio = 0;
            foreach(var data in dataList)
            {
                trafficData = (data as TrafficDataEvent);
                if(trafficData!=null)
                {
                    totalTrafficCount++;
                    totalOccupyTime += trafficData.occupyTime;
                }
            }
            trafficCount = (byte)(totalTrafficCount > 255 ? 255 : totalTrafficCount);
            ratio = totalOccupyTime / ts.TotalSeconds;
            occupyTime = Utility.SplitNumber(ratio);
            return 1;
        }

        public TRAFFIC_DATA_STAT GetTrafficDataByLane(int lane, List<Object> dataList)
        {
            TRAFFIC_DATA_STAT trafficDataStat = new TRAFFIC_DATA_STAT();
            TrafficDataEvent trafficData;
            int totalTrafficCount = 0;
            int totalLength = 0;
            double totalSpeed = 0;
            foreach (var data in dataList)
            {
                trafficData = (data as TrafficDataEvent);
                if (trafficData != null) // && )  // 1: 상행(TO RIGHT) 2: 하행(TO LEFT)
                {
                    if(GetKorExLaneNo(trafficData) == lane)
                    {
                        totalTrafficCount++;
                        totalSpeed += trafficData.speed;
                        totalLength += trafficData.length/10; // cm--> dm 단위.
                    }
                }
            }

            if(totalTrafficCount>0)
            {
                trafficDataStat.AVG_SPEED = totalSpeed / totalTrafficCount;
                trafficDataStat.AVG_LENGTH = totalLength / totalTrafficCount;
            }
            return trafficDataStat;
        }


        public int GetKorExLaneNo(TrafficDataEvent trafficDataEvent)
        {
            int result = 0;
            switch(trafficDataEvent.direction)
            {
                case 1: // 상행(TO RIGHT)
                    result = VDSConfig.ToLeftLaneGroup.LaneList.Count - trafficDataEvent.lane + 1;
                    break;
                case 2: // 하행(TO LEFT)
                    result = VDSConfig.ToLeftLaneGroup.LaneList.Count + trafficDataEvent.lane;
                    break;
            }
            return result;
        }

        public override int AddTrafficDataStat(TrafficDataEvent trafficDataEvent)
        {

            int result = 1;
            int laneNo = 0;
            int category = 0;
            // 각종 Speed , length 관련 데이터 누적
            lock (_centerDataLock)
            {
                laneNo = GetKorExLaneNo(trafficDataEvent);
                if(laneNo>0)
                {
                    category = Utility.GetSpeedCategory(trafficDataEvent.speed); // km/h
                    if(category>=1 && category<=12)
                    {
                        speedDataStat[laneNo - 1].speedCategory[category-1]++;
                    }
                    Utility.VEHICLE_LENGTH_CATEGORY lengthCategory = Utility.GetVehicleLengthCategory(trafficDataEvent.length/100); // cm--> m 로
                    switch(lengthCategory)
                    {
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL:
                            lengthDataStat[laneNo - 1].lengthCategory[0]++;
                            break;
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE:
                            lengthDataStat[laneNo - 1].lengthCategory[1]++;
                            break;
                        case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE:
                            lengthDataStat[laneNo - 1].lengthCategory[2]++;
                            break;
                    }
                    accuTrafficDataStat[laneNo - 1]++;
                }
            }
            return result;
        }

        public byte[] GetKorExVDSVersion()
        {
            byte[] result = new byte[4];
            int index = 0;
            result[index++] = (byte)((VDSConfig.korExConfig.versionNo <<4) + VDSConfig.korExConfig.releaseNo);
            result[index++] = (byte)VDSConfig.korExConfig.releaseYear;
            result[index++] = (byte)VDSConfig.korExConfig.releaseMonth;
            result[index++] = (byte)VDSConfig.korExConfig.releaseDay;
            return result;
        }

        public int SendReverseRunNotify(TrafficDataEvent trafficDataEvent)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 0;
            try
            {
                ReverseRunRequest reverseRun = new ReverseRunRequest();
                reverseRun.contraflow.laneCount = (byte)((VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count) );
                int laneNo = GetKorExLaneNo(trafficDataEvent);

                for (int i = 0; i < reverseRun.contraflow.laneCount; i++) // 역주행 1
                {
                    if (laneNo == (i + 1))
                    {
                        reverseRun.contraflow.reverseRun[i] = 1;
                    }
                    else                                                     // 정상 0 
                    {
                        reverseRun.contraflow.reverseRun[i] = 0;
                    }
                }
                ExDataFrame frame = new ExDataFrame();
                SetRequestFrame(ref frame, ExDataFrameDefine.OP_REVERSE_RUN_COMMAND, reverseRun);
                byte[] data = frame.Serialize();
                Send(_korExCenter._sessionContext, data);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public byte[] GetErrorInfo()
        {
            byte[] result = new byte[4];

            return result;
        }

        public TrafficDataExResponse GetTrafficDataExResponse()
        {
            TrafficDataExResponse result = new TrafficDataExResponse();
            TrafficDataEvent trafficData;

            result.frameNo = _lastFrameNo;
            result.errorInfo = GetErrorInfo();
            result.laneCount = (byte)(VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count);
            // 초기화
            for (int i = 0; i < result.laneCount; i++)
            {
                LaneInfoEx lane = new LaneInfoEx();
                result.laneInfoList.Add(lane);
            }

            // 대형, 중형, 소형 교통량 산출
            // 평균 속도 산출
            // 평균 점유율 산출
            // 평균 차량길이 산출
            List<Object> dataList = _centerData[(_activeCenterDataIndex + 1) % 2];// 저장된 센터 데이터
            
            foreach (var data in dataList)
            {
                trafficData = (data as TrafficDataEvent);
                if (trafficData != null)
                {
                    if(trafficData.lane-1 < result.laneInfoList.Count)
                    {
                        Utility.VEHICLE_LENGTH_CATEGORY lengthCategory = Utility.GetVehicleLengthCategory(trafficData.length / 100);
                        switch (lengthCategory)
                        {
                            case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_SMALL:
                                result.laneInfoList[trafficData.lane - 1].smallTrafficCount++;
                                break;
                            case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_MIDDLE:
                                result.laneInfoList[trafficData.lane - 1].middleTrafficCount++;
                                break;
                            case Utility.VEHICLE_LENGTH_CATEGORY.CATEGORY_LARGE:
                                result.laneInfoList[trafficData.lane - 1].largeTrafficCount++;
                                break;
                        }

                        result.laneInfoList[trafficData.lane - 1].totalLength += (trafficData.length / 10);// dm 기준...10cm = 1 dm. 1 m = 10 dm 
                        result.laneInfoList[trafficData.lane - 1].totalSpeed += trafficData.speed;
                        result.laneInfoList[trafficData.lane - 1].totalOccupyTime += trafficData.occupyTime;
                    }
                }
            }
            for (int i = 0; i < result.laneCount; i++)
            {
                LaneInfoEx lane = result.laneInfoList[i];
                if (lane.smallTrafficCount + lane.middleTrafficCount + lane.largeTrafficCount > 0)
                {
                    lane.speed = (byte)(lane.totalSpeed / (lane.smallTrafficCount + lane.middleTrafficCount + lane.largeTrafficCount));
                    lane.occupyRatio =(UInt16)((lane.totalOccupyTime / (lane.smallTrafficCount + lane.middleTrafficCount + lane.largeTrafficCount))*1000);
                    lane.carLength = (byte)(lane.totalLength / (lane.smallTrafficCount + lane.middleTrafficCount + lane.largeTrafficCount));
                }
                else
                {
                    lane.speed = 0;
                    lane.occupyRatio = 0;
                    lane.carLength = 0;

                }
            }
            return result;
        }

        public SpeedDataExResponse GetSpeedDataExResponse()
        {
            SpeedDataExResponse result = new SpeedDataExResponse();
            result.laneCount = (byte)(VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count);
            for(int i=0;i< result.laneCount;i++)
            {
                SpeedData speedData = new SpeedData();
                for (int j = 0; j < 12; j++)
                {
                    speedData.speedCategory[j] = speedDataStat[i].speedCategory[j];
                    speedDataStat[i].speedCategory[j] = 0;// 리셋
                }
                result.speedDataList.Add(speedData);
            }
            return result;
        }

        public SystemStatus GetVDSRackStatusInKorEx()
        {
            SystemStatus status = new SystemStatus();
            status.longPowerFail = VDSRackStatus.IsLongPowerFail;
            status.shortPowerFail = VDSRackStatus.IsShortPowerFail;
            status.defaultParameter = 1;
            status.frontStatus = VDSRackStatus.IsFrontDoorOpen;
            status.rearStatus = VDSRackStatus.IsRearDoorOpen;
            status.heaterStatus = VDSRackStatus.IsHeaterOn;
            status.videoStatus = 0;
            status.IsReset = VDSRackStatus.IsReset;
            status.temperature = (byte)VDSRackStatus.Temperature;
            status.inputVoltage = 220;
            status.outputVoltage = 220;

            return status;

        }
    }
}
