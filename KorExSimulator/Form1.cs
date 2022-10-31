using KorExManageCtrl;
using KorExManageCtrl.VDSProtocol;
using KorExManageCtrl.VDSProtocol_v2._0;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace KorExSimulator
{
   
   public delegate void DisplayDataDelegate(ExDataFrameExt frame, ListBox listBox);


    public partial class Form1 : Form
    {
        VDSServer _kictEventServer = new VDSServer();
        VDSClient _korExClient = new VDSClient();
        //private VDSLogger _Logger = new VDSLogger();
        SessionContext _sessionContext;
        SOCKET_STATUS _status;
        DisplayDataDelegate _displayDataDelegate = null;

        ExDataFrame _prevDataFrame = null;
        Timer syncTimer;
        public Form1()
        {
            InitializeComponent();
            _displayDataDelegate = new DisplayDataDelegate(DisplayData);

            //Utility._addLog = _Logger.AddLog;
        }

        private void button1_Click(object sender, EventArgs e)
        {



        }

        private void button8_Click(object sender, EventArgs e)
        {

            if(true)
            {
                ConnectToController();
            }
            else
            {
                //if (_kictEventServer._bListening)
                //{
                //    _kictEventServer.StopManager();
                //    //  _Logger.StopManager();
                //    btnStart.Text = "Start";
                //}
                //else
                //{
                //    // _Logger.StartManager();
                //    _kictEventServer.SetAddress(txtAddress.Text, int.Parse(txtPort.Text), CLIENT_TYPE.KICT_EVNT_CLIENT, AcceptCtrlCallback);
                //    _kictEventServer.StartManager();
                //    btnStart.Text = "Stop";
                //}
            }
            
        }

        //public void AcceptCtrlCallback(IAsyncResult ar)
        //{
        //    // Get the socket that handles the client request.  
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
        //    String strLog;
        //    try
        //    {
        //        // Create the state object.  
        //        Socket serverSocket = (Socket)ar.AsyncState;
        //        Socket socket = serverSocket.EndAccept(ar);
        //        //if (socket.Connected)
        //        {
        //            kictClient = new SessionContext();
        //            kictClient._type = _kictEventServer._clientType;
        //            kictClient._socket = socket;
        //            socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
        //                new AsyncCallback(KorExReadCallback), kictClient);

        //            strLog = String.Format("Remote Client accepted");
        //            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);



        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
        //    }
        //    finally
        //    {
        //        _kictEventServer.SetAcceptProcessEvent();
        //    }
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        //}


        private int KorExConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                _sessionContext = session;
                _sessionContext._socket.BeginReceive(_sessionContext.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KorExReadCallback), _sessionContext);

                //strLog = String.Format("제어기 ({0}:{1}  접속 성공)", VDSConfig.controllerConfig.MAServerAddress, VDSConfig.controllerConfig.MAServerPort);

                //Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                nResult = 1;
            }
            catch (Exception ex)
            {
                _status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public void KorExReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));

            String content = String.Empty;
            String strLog;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            SessionContext kictClient = (SessionContext)ar.AsyncState;
            Socket socket = kictClient._socket;
            try
            {
                // Read data from the client socket.
                int bytesRead = socket.EndReceive(ar);
                Console.WriteLine("ReadCallback {0} byte", bytesRead);
                if (bytesRead > 0)
                {
                    strLog = String.Format("ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                    byte[] packet = new byte[bytesRead];
                    Array.Copy(kictClient.buffer, 0, packet, 0, bytesRead);
                    int i = 0;
                    while (i < packet.Length)
                    {
                        if (_prevDataFrame == null)
                        {
                            _prevDataFrame = new ExDataFrame();
                        }

                        i += _prevDataFrame.Deserialize(packet, i);
                        if (_prevDataFrame.bDataCompleted)
                        {
                            ExDataFrameExt exFrame = new ExDataFrameExt();
                            exFrame.frame = _prevDataFrame;

                            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxReceive });
                            _prevDataFrame = null;

                        }
                        else
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                            //_prevDataFrame = null;
                        }
                    }

                    // Not all data received. Get more.  
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KorExReadCallback), kictClient);

                }
                else
                {
                    if (socket != null && kictClient._type == CLIENT_TYPE.KICT_EVNT_CLIENT)
                    {
                        strLog = String.Format("원격 클라이언트 연결 종료");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        //Console.WriteLine("22Close socket");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        kictClient = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_CSN_CHECK_COMMAND,null);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

            //StartSyncTimer();
        }

        public void StartSyncTimer()
        {
            StopSyncTimer();

            if (syncTimer == null)
                syncTimer = new Timer();
            syncTimer.Interval = int.Parse(txtPeriod.Text) * 1000; // 동기화 주기
            syncTimer.Tick += SyncTimer_Tick;
            syncTimer.Start();
        }

        public void StopSyncTimer()
        {
            if (syncTimer != null)
            {
                syncTimer.Stop();
            }
            syncTimer = null;
        }


        private void SyncTimer_Tick(object sender, EventArgs e)
        {

            SendSyncCommand();

        }

        public int SendByEthernet(SessionContext kictClient, byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                kictClient._socket.BeginSend(data, 0, size, 0,
                            new AsyncCallback(KictEventSendCallback), kictClient._socket);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void KictEventSendCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));

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
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] ip1 = Utility.IPAddressToKorExFormat("192.168.0.1");
            byte[] ip2 = Utility.IPAddressToKorExFormat("2001:0230:abcd:ffff:0000:0000:ffff:1111");

        }


        public void DisplayData(ExDataFrameExt frame, ListBox list)
        {
            DisplayDataFrame(frame, list);
        }

        public void DisplayDataFrame(ExDataFrameExt frame, ListBox list)
        {
            list.Items.Clear();
            String value;
            String temp;
            if (frame != null)
            {
                value = String.Format($"시간: {frame.txTime.ToString("yyyy-MM-dd HH:mm:ss.ff")} ");
                list.Items.Add(value);


                value = String.Format($"SenderIP:{Utility.PrintHexaString(frame.frame.senderIP, frame.frame.senderIP.Length)}");
                list.Items.Add(value);

                value = String.Format($"DestinationIP:{Utility.PrintHexaString(frame.frame.destinationIP, frame.frame.destinationIP.Length)}");
                list.Items.Add(value);

                value = String.Format($"CSN:{Utility.ByteToString(frame.frame.csn)}");
                list.Items.Add(value);

                value = String.Format($"Total Length:{frame.frame._totalLength}");
                list.Items.Add(value);

                value = String.Format($"OPCODE:{Utility.PrintHexaString(new byte[1] { frame.frame.opCode }, 1)}");
                list.Items.Add(value);
                if(list == lbxReceive)
                {
                    if(frame.frame.opData is ExResponse)
                    {
                        ExResponse response = (ExResponse)frame.frame.opData;
                        //value = String.Format($"status:{response.status[0]} {response.status[1]}");
                        //list.Items.Add(value);
                    }
                    
                    switch (frame.frame.opCode)
                    {
                        case ExDataFrameDefine.OP_TRAFFIC_DATA_COMMAND:

                            DisplayTrafficDataInfo(frame.frame, list);
                            break;
                        case ExDataFrameDefine.OP_SPEED_DATA_COMMAND:
                            DisplaySpeedDataInfo(frame.frame, list);
                            break;
                        case ExDataFrameDefine.OP_VEHICLE_LENGTH_COMMAND:
                            DisplayLengthDataInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_ACCU_TRAFFIC_COMMAND:
                            DisplayAccuDataInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_VDS_RESET_COMMAND:
                        case ExDataFrameDefine.OP_VDS_INIT_COMMAND:
                        case ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND:
                        case ExDataFrameDefine.OP_MEMORY_STATUS_COMMAND:
                        case ExDataFrameDefine.OP_CHANGE_RTC_COMMAND:
                        case ExDataFrameDefine.OP_SET_TEMPERATURE_COMMAND:
                            DisplayAckResponse(frame.frame, list);
                            break;


                        case ExDataFrameDefine.OP_PARAM_UPLOAD_COMMAND:
                            DisplayParamUploadResponse(frame.frame, list);
                            break;




                        case ExDataFrameDefine.OP_HW_STATUS_COMMAND:
                            DisplayHWSataus(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_TRAFFIC_THRESHOLD_COMMAND:
                            DisplayTrafficThresholdInfo(frame.frame, list);
                            break;
                        case ExDataFrameDefine.OP_ONLINE_STATUS_COMMAND:
                            DisplayOnlineStatusInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_MESSAGE_ECHO_COMMAND:
                            DisplayMessageEchoInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_SEQ_TRANSFER_COMMAND:
                            DisplayCheckSeqInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_VDS_VERSION_COMMAND:
                            DisplayVDSVersionInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_INDIV_TRAFFIC_COMMAND:
                            DisplayIndivTrafficInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_REVERSE_RUN_COMMAND:
                            DisplayReverseRunInfo(frame.frame, list);
                            break;

                        case ExDataFrameDefine.OP_SYSTEM_STATUS_COMMAND:
                            DisplaySystemStatus(frame.frame, list);
                            break;

                    }
                }
                if (frame.frame.opCode == ExDataFrameDefine.OP_CHECK_SESSION_COMMAND) //||
                    //frame.frame.opCode == ExDataFrameDefine.OP_REVERSE_RUN_COMMAND
                    //)
                {
                    SendCheckSessionResponse(frame.frame);
                }
            }
        }

        
        private void DisplayHWSataus(ExDataFrame frame, ListBox list)
        {
            ControllerStatusResponse ctrlStatus = (ControllerStatusResponse)frame.opData;
            String value;
            int count = 0;
            if (ctrlStatus != null)
            {
                value = String.Format($"powerSupplyCount:{ctrlStatus.powerSupplyCount}, status={ctrlStatus.powerSupplyStatus}");
                list.Items.Add(value);


                value = String.Format($"boardCount:{ctrlStatus.boardCount}, status={ctrlStatus.boardStatus}");
                list.Items.Add(value);

            }

        }

        
        private void DisplayOnlineStatusInfo(ExDataFrame frame, ListBox list)
        {
            CheckOnlineStatusResponse statusResponse = (CheckOnlineStatusResponse)frame.opData;
            String value;
            int count = 0;
            if (statusResponse != null)
            {

                value = String.Format($"연결시간:{statusResponse.passedTime}");
                list.Items.Add(value);
            }

        }

        private void DisplayMessageEchoInfo(ExDataFrame frame, ListBox list)
        {
            EchoMessageResponse echoResponse = (EchoMessageResponse)frame.opData;
            String value;
            int count = 0;
            if (echoResponse != null)
            {
               
                value = String.Format($"echo message:{Utility.ByteToString(echoResponse.echoMessage)}");
                list.Items.Add(value);
            }

        }

        private void DisplayCheckSeqInfo(ExDataFrame frame, ListBox list)
        {
            CheckSeqNoResponse seqResponse = (CheckSeqNoResponse)frame.opData;
            String value;
            int count = 0;
            if (seqResponse != null)
            {
                for(int i=0;i< seqResponse.seqList.Length;i++)
                {
                    value = String.Format($"seq {i+1}:{seqResponse.seqList[i]}");
                    list.Items.Add(value);
                }

                
            }

        }

        private void DisplayVDSVersionInfo(ExDataFrame frame, ListBox list)
        {
            VDSVersionResponse versionResponse = (VDSVersionResponse)frame.opData;
            String value;
            int count = 0;
            if (versionResponse != null)
            {

                for (int i = 0; i < versionResponse.version.Length; i++)
                {
                    value = String.Format($"version {i + 1}:{versionResponse.version[i]}");
                    list.Items.Add(value);
                }


            }

        }

        private void DisplayIndivTrafficInfo(ExDataFrame frame, ListBox list)
        {
            IndivTrafficDataResponse indivResponse = (IndivTrafficDataResponse)frame.opData;
            String value;
            int count = 0;
            if (indivResponse != null)
            {
                value = String.Format($"timeFrameNo:{indivResponse.timeFrameNo}");
                list.Items.Add(value);

                value = String.Format($"totalCount:{indivResponse.totalCount}");
                list.Items.Add(value);

        
                for (int i = 0; i < indivResponse.trafficDataList.Count; i++)
                {
                    IndivTrafficData data = indivResponse.trafficDataList[i];

                    value = String.Format($"traffic Data {i + 1}: lane={data.lane} , passTime={data.passTime} , speed = {data.speed}, occupyTime={data.occupyTime}, category={data.category}");
                    list.Items.Add(value);
                }


            }

        }

        private void DisplayReverseRunInfo(ExDataFrame frame, ListBox list)
        {
            ReverseRunRequest reverseRun = (ReverseRunRequest)frame.opData;
            String value;
            int count = 0;
            if (reverseRun != null)
            {

                value = String.Format($"lane 갯수:{reverseRun.contraflow.laneCount}");
                list.Items.Add(value);


                for (int i = 0; i < reverseRun.contraflow.laneCount; i++)
                {
                    value = String.Format($"Lane {i + 1} 역주행 정보: {reverseRun.contraflow.reverseRun[i]}");
                    list.Items.Add(value);
                }
                SendReverseRunResponse(frame);

            }

        }


        private void DisplaySystemStatus(ExDataFrame frame, ListBox list)
        {
            SystemStatus status = (SystemStatus)frame.opData;
            String value;
            if (status != null)
            {

                value = String.Format($"longPowerFail : {status.longPowerFail}");
                list.Items.Add(value);

                value = String.Format($"shortPowerFail : {status.shortPowerFail}");
                list.Items.Add(value);

                value = String.Format($"defaultParameter : {status.defaultParameter}");
                list.Items.Add(value);

                value = String.Format($"frontStatus : {status.frontStatus}");
                list.Items.Add(value);

                value = String.Format($"rearStatus : {status.rearStatus}");
                list.Items.Add(value);

                value = String.Format($"fanStatus : {status.fanStatus}");
                list.Items.Add(value);

                value = String.Format($"heaterStatus : {status.heaterStatus}");
                list.Items.Add(value);

                value = String.Format($"videoStatus : {status.videoStatus}");
                list.Items.Add(value);

                value = String.Format($"IsReset : {status.IsReset}");
                list.Items.Add(value);

                value = String.Format($"temperature : {status.temperature}");
                list.Items.Add(value);

                value = String.Format($"inputVoltage : {status.inputVoltage}");
                list.Items.Add(value);

                value = String.Format($"outputVoltage : {status.outputVoltage}");
                list.Items.Add(value);

            }

        }


        

        private void SendReverseRunResponse(ExDataFrame frame)
        {
            ExDataFrame response = new ExDataFrame();
            ReverseRunResponse reverseRun = new ReverseRunResponse();
            //Array.Copy(BitConverter.GetBytes(VDSConfig.CSN), 0, csnData.csn, 0, 4);
            Utility.GetCSN(ref response.csn);
            response.opData = reverseRun;
            //response._totalLength = 9;
            SetResponseFrame(frame, ref response, ExDataFrameDefine.ACK_NORMAL);

            byte[] data = response.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);
        }

        private void DisplayTrafficThresholdInfo(ExDataFrame frame, ListBox list)
        {
            SetErrorThresholdResponse thresholdResponse = (SetErrorThresholdResponse)frame.opData;
            String value;
            int count = 0;
            if (thresholdResponse != null)
            {
                value = String.Format($"resultCode:{thresholdResponse.resultCode}");
                list.Items.Add(value);


                value = String.Format($"threshold:{thresholdResponse.threshold}");
                list.Items.Add(value);
            }

        }

        private void DisplaySpeedDataInfo(ExDataFrame frame, ListBox list)
        {
            SpeedDataExResponse speedDataEx = (SpeedDataExResponse)frame.opData;
            String value;
            int count = 0;
            int laneNo = 0;
            if (speedDataEx != null)
            {
                value = String.Format($"lane Count: {speedDataEx.laneCount}");
                list.Items.Add(value);

                foreach(var speedData in speedDataEx.speedDataList)
                {
                    value = String.Format($" {laneNo + 1} 차선 :");
                    for (int i = 0; i < 12; i++)
                    {
                        value += String.Format($" {i + 1} Category Count: {speedData.speedCategory[i]}");
                        count += speedData.speedCategory[i];
                    }
                    list.Items.Add(value);
                    laneNo++;
                }
                value = String.Format($"전체갯수:{count}");
                list.Items.Add(value);

            }

        }
        

        private void DisplayAccuDataInfo(ExDataFrame frame, ListBox list)
        {
            AccuTrafficDataResponse accuData = (AccuTrafficDataResponse)frame.opData;
            String value;
            int count = 0;
            if (accuData != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    value = String.Format($"{i + 1} volume: {accuData.volumData[i]} ({accuData.volumData[i]})");
                    list.Items.Add(value);
                    count += accuData.volumData[i];
                }
                value = String.Format($"전체갯수:{count}");
                list.Items.Add(value);
            }

        }


        private void DisplayAckResponse(ExDataFrame frame, ListBox list)
        {
            ExResponse response = (ExResponse)frame.opData;
            String value;
            if (response != null)
            {
                value = String.Format($"응답코드:{response.resultCode}");
                list.Items.Add(value);
            }

        }

        private void DisplayParamUploadResponse(ExDataFrame frame, ListBox list)
        {
            ParamUploadResponse response = (ParamUploadResponse)frame.opData;
            String value;
            if (response != null)
            {
                switch(response.paramIndex)
                {
                    case 1: // 차로
                        ParamLaneConfig config = (ParamLaneConfig)response.param;
                        for(int i=0;i<config.laneInfo.Length;i++)
                        {
                            value = String.Format($"lane config[{i}] :{config.laneInfo[i]}");
                            list.Items.Add(value);
                        }
                        break;
                    case 3: // 수집주기
                        PollingCycle cycle = (PollingCycle)response.param;
                        if(cycle!=null)
                        {
                            value = String.Format($"polling cycle :{cycle.cycle}");
                            list.Items.Add(value);
                        }
                        break;
                    case 5: // 차량속도 구분
                        SpeedCategory category = (SpeedCategory)response.param;
                        for(int i=0;i<category.category.Length;i++)
                        {
                            value = String.Format($"{i} category :{category.category[i]}");
                            list.Items.Add(value);
                        }
                        break;
                    case 6: // 차량 길이 구분
                        LengthCategory lenCategory = (LengthCategory)response.param;
                        for (int i = 0; i < lenCategory.category.Length; i++)
                        {
                            value = String.Format($"{i} category :{lenCategory.category[i]}");
                            list.Items.Add(value);
                        }
                        break;

                    case 7:// 속도별 누적치 

                    case 8: // 길이별 누적치
                    case 9: // 속도 계산 가능 여부
                    case 10: // 차량길이 계산 가능 여부
                    case 17: // oscillation threshold
                    case 20: // 자동 동기화 대기시간
                    case 21:// 역주행 사용여부
                            VDSValue ability = (VDSValue)response.param;
                            value = String.Format($"value :{ability.value}");
                            list.Items.Add(value);
                        break;
                }
            }

        }

        

        private void DisplayLengthDataInfo(ExDataFrame frame, ListBox list)
        {
            VehicleLengthDataResponse lengthData = (VehicleLengthDataResponse)frame.opData;
            String value;
            int count = 0;
            if (lengthData != null)
            {
                value = String.Format($"resultCode:{lengthData.resultCode}");
                list.Items.Add(value);


                value = String.Format($"laneNo:{lengthData.laneNo}");
                list.Items.Add(value);

                for (int i = 0; i < 3; i++)
                {
                    value = String.Format($"{i + 1} Category Count: {lengthData.lengthData.lengthCategory[i]}");
                    list.Items.Add(value);
                    count += lengthData.lengthData.lengthCategory[i];
                }
                value = String.Format($"전체갯수:{count}");
                list.Items.Add(value);
            }

        }

        private void DisplayTrafficDataInfo(ExDataFrame frame, ListBox list)
        {
            TrafficDataExResponse trafficData = (TrafficDataExResponse)frame.opData;
            String value;
            if (trafficData != null)
            {
                value = String.Format($"frameNo:{trafficData.frameNo}");
                list.Items.Add(value);

                value = String.Format($"***** Detector Info *****");
                list.Items.Add(value);


                value = String.Format($"장애정보:{Utility.PrintHexaString(trafficData.errorInfo, trafficData.errorInfo.Length)}");
                list.Items.Add(value);


                value = String.Format($"차선수:{trafficData.laneCount}");
                list.Items.Add(value);
                int i = 0;
                foreach (var lane in trafficData.laneInfoList)
                {
                    value = String.Format($"{i + 1}차선 대분류 차량 수:{lane.largeTrafficCount} ,중분류 차량 수:{lane.middleTrafficCount} 소분류 차량 수:{lane.smallTrafficCount}, Speed: {lane.speed}, occupy: {lane.occupyRatio}, car length: {lane.carLength}");
                    list.Items.Add(value);
                    i++;
                }

            }

        }

        private void SendCheckSessionResponse(ExDataFrame frame)
        {
            ExResponse resultData = new ExResponse();
            resultData.resultCode = ExDataFrameDefine.ACK_NORMAL;
            ExDataFrame response = MakeKorExRequest(ExDataFrameDefine.OP_CHECK_SESSION_COMMAND, resultData);

            byte[] data = response.Serialize();
            //SendByEthernet(_sessionContext, data, data.Length);
        }

        public int SetResponseFrame(ExDataFrame request, ref ExDataFrame response, byte resultCode)
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
                //Array.Copy(BitConverter.GetBytes(VDSConfig.CSN), 0, response.csn, 0, 4);
                Utility.GetCSN(ref response.csn);

                //response._csn = VDSConfig.kictConfig.csn;

                // OP CODE
                response.opCode = request.opCode;

                // TRANSACTION NUMBER 
                //if (request.opData != null)
                //{
                //    if (response.opData != null)
                //    {
                //        Array.Copy((request.opData as ExRequest).transactionNo, 0, (response.opData as ExResponse).transactionNo, 0, 8);
                //        (response.opData as ExResponse).resultCode = resultCode;
                //       // (response.opData as ExResponse).status = GetVDSStatus();
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

        public void SendSyncCommand()
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);

            frame.csn = Utility.StringToByte("VD11234567890");

            frame.opCode = ExDataFrameDefine.OP_SYNC_VDS_COMMAND;

            ControllerSyncRequest request = new ControllerSyncRequest();

            request.frameNo = GetFrameNo();

            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

            //SendTrafficDataCommand();
        }

        private void SendTrafficDataCommand()
        {

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_TRAFFIC_DATA_COMMAND, null);
            byte[] data = frame.Serialize();

            SendByEthernet(_sessionContext, data, data.Length);
            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            SendSyncCommand();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendTrafficDataCommand();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_ACCU_TRAFFIC_COMMAND, null);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }


        private void button5_Click(object sender, EventArgs e)
        {

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_SPEED_DATA_COMMAND, null);
            byte[] data = frame.Serialize();

            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_VEHICLE_LENGTH_COMMAND;

            VehicleLengthDataRequest request = new VehicleLengthDataRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.laneNo = (byte)int.Parse(txtLane.Text);

            //val = Utility.StringToByte("12345678");
            //Array.Copy(val, 0, request.transactionNo, 0, 8);

            frame._totalLength = 10;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

        }

        private void button7_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_TRAFFIC_THRESHOLD_COMMAND;

            SetErrorThresholdRequest request = new SetErrorThresholdRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.threshold = 0x01;

            
            frame._totalLength = 10;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_HW_STATUS_COMMAND;

            ControllerStatusRequest request = new ControllerStatusRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;

            frame._totalLength = 9;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_VDS_RESET_COMMAND, null);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button10_Click(object sender, EventArgs e)
        {

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_VDS_INIT_COMMAND, null);
            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        

        private void button11_Click(object sender, EventArgs e)
        {
            ParamLaneConfig param = new ParamLaneConfig();
            param.laneInfo[0] = 0xFF; //8차선 사용(11111111)
            param.laneInfo[1] = 0x07; //3차선 사용(00000111)

            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x01;
            request.param = param;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 0x02;

            SpeedLoopConfig config = new SpeedLoopConfig();

            for(int i=0;i<32;i++)
                config.loopNo[i] = (byte)(i+16);

            request.param = config;

            frame._totalLength = 10 + 32;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

        }

        private void button13_Click(object sender, EventArgs e)
        {
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x03;
            PollingCycle polling = new PollingCycle();
            polling.cycle = 45;
            request.param = polling;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button14_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();

            
            request.paramIndex = 0x04;

            VehiclePulseNumber pulse = new VehiclePulseNumber();
            pulse.presenceNumber = 12;
            pulse.noPresenceNumber = 34;
            request.param = pulse;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button15_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();

            
            request.paramIndex = 0x05;

            SpeedCategory category = new SpeedCategory();

            for (int i = 0; i < 12; i++)
                category.category[i] = (byte)(i * 5 + 1);

            request.param = category;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button16_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x06;
            LengthCategory category = new LengthCategory();
            category.category[0] = 47;
            category.category[1] = 135;
            category.category[2] = 0;

            request.param = category;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button17_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x07;
            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 2;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button18_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x08;

            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 3;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button19_Click(object sender, EventArgs e)
        {
            
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x09;
            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 4;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button20_Click(object sender, EventArgs e)
        {
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 0x0A;

            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 5;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 11;

            SpeedLoopDimension dimension = new SpeedLoopDimension();

            for (int i = 0; i < 32; i++)
                dimension.dimension[i] = (byte)(i + 16);

            request.param = dimension;

            frame._totalLength = 10 + 32;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

        }

        private void button22_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 12;

            PollingThreshold ability = new PollingThreshold(request.paramIndex);
            ability.threshold = 12;

            request.param = ability;

            frame._totalLength = 10 + 1;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button23_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 13;

            PollingThreshold ability = new PollingThreshold(request.paramIndex);
            ability.threshold = 14;

            request.param = ability;

            frame._totalLength = 10 + 1;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button24_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 14;

            PollingThreshold ability = new PollingThreshold(request.paramIndex);
            ability.threshold = 15;

            request.param = ability;

            frame._totalLength = 10 + 1;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });

        }

        private void button25_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 15;

            IncidentDetectThreshold threshold = new IncidentDetectThreshold();

            threshold.incidentCycle = 5;
            threshold.period = 6;
            threshold.algorithm = 4; // 1: 점유율, 2: 속도, 3: 교통량, 4: 점유율 및 교통량 
            threshold.kfactor_1 = 50;
            threshold.kfactor_2 = 60;

            for (int i = 0; i < 32; i++)
                threshold.threshold[i] = (byte)(i + 16);

            request.param = threshold;

            frame._totalLength = 10 + 71;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button26_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND;

            ParamDownloadRequest request = new ParamDownloadRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;


            request.paramIndex = 16;

            StuckThreshold threshold = new StuckThreshold();

            threshold.highTrafficDuration = 10;
            threshold.highOnDuration = 20;
            threshold.highOffDuration = 30;

            threshold.lowTrafficDuration = 40;
            threshold.lowOnDuration = 50;
            threshold.lowOffDuration = 60;
            request.param = threshold;

            frame._totalLength = 10 + 6;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button27_Click(object sender, EventArgs e)
        {
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 17;
            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 30;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);


            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button28_Click(object sender, EventArgs e)
        {
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 20;
            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 40;
            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button29_Click(object sender, EventArgs e)
        {
            ParamDownloadRequest request = new ParamDownloadRequest();
            request.paramIndex = 21;

            VDSValue ability = new VDSValue(request.paramIndex);
            ability.value = 7;


            request.param = ability;

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_DOWNLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button30_Click(object sender, EventArgs e)
        {
            ParamUploadRequest request = new ParamUploadRequest();
            
            
            request.paramIndex = (byte)(int.Parse(cbParam.SelectedItem.ToString()));

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_PARAM_UPLOAD_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        public byte GetFrameNo()
        {
            byte result = 0;
            int totalSecond = DateTime.Now.Minute * 60 + DateTime.Now.Second;
            result = (byte)(totalSecond / int.Parse(txtPeriod.Text));
            return result;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_ONLINE_STATUS_COMMAND, null);
            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button32_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);

            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);


            frame.csn = Utility.StringToByte("VD123456");

            frame.opCode = ExDataFrameDefine.OP_MEMORY_STATUS_COMMAND;

            CheckOnlineStatusRequest request = new CheckOnlineStatusRequest();

            //request.transactionNo[0] = 0x01;
            //request.transactionNo[1] = 0x02;
            //request.transactionNo[2] = 0x03;
            //request.transactionNo[3] = 0x04;
            //request.transactionNo[4] = 0x05;
            //request.transactionNo[5] = 0x06;
            //request.transactionNo[6] = 0x07;
            //request.transactionNo[7] = 0x08;




            //val = Utility.StringToByte("12345678");
            //Array.Copy(val, 0, request.transactionNo, 0, 8);

            frame._totalLength = 9;
            frame.opData = request;



            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button33_Click(object sender, EventArgs e)
        {
            EchoMessageRequest request = new EchoMessageRequest();
            request.echoMessage = Utility.StringToByte("avogadro echo test....");

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_MESSAGE_ECHO_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button34_Click(object sender, EventArgs e)
        {
            CheckSeqNoRequest request = new CheckSeqNoRequest();
            request.baseNumber = 101;
            request.counter = 21;
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_SEQ_TRANSFER_COMMAND, request);

            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button35_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_VDS_VERSION_COMMAND, null);
            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button36_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_INDIV_TRAFFIC_COMMAND, null);
            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }


        public void ConnectToController()
        {
            _korExClient.SetAddress(txtAddress.Text, int.Parse(txtPort.Text), CLIENT_TYPE.VDS_CLIENT, KorExConnectCallback, KorExReadCallback, SendCallback);
            _korExClient.Connect(txtAddress.Text, int.Parse(txtPort.Text), 5);
            //_korExClient.StartConnect();
        }


        private ExDataFrame MakeKorExRequest(byte opcode, IExOPData opData)
        {
            ExDataFrame frame = new ExDataFrame();
            byte[] val = Utility.StringToByte("avogadro12345678");
            Array.Copy(val, 0, frame.senderIP, 0, 16);
            val = Utility.StringToByte("12345678avogadro");
            Array.Copy(val, 0, frame.destinationIP, 0, 16);
            frame.csn = Utility.GetCSN("VD", "1", "1234567890");
            frame.opCode = opcode;
            frame.opData = opData;

            return frame;

        }

        private void button37_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (chkSysTime.Checked)
                date = DateTime.Now;
            else
            {
                String time = String.Format("{0} {1}", dtPicker.Value.ToString("yyyy-MM-dd"), txtTime.Text);
                date = DateTime.Parse(time);
                Console.WriteLine("time = {0}", time);
            }
            RealTimeClock rtc = new RealTimeClock();
            //yyyymmddhhmmss --> bcd 코드 변환
            rtc.timeinfo = Utility.stringToBCD(date.ToString("yyyyMMddHHmmss"));
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_CHANGE_RTC_COMMAND, rtc);
            byte[] data = frame.Serialize();
            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });


        }

        private void button38_Click(object sender, EventArgs e)
        {
            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_SYSTEM_STATUS_COMMAND, null);
            byte[] data = frame.Serialize();

            SendByEthernet(_sessionContext, data, data.Length);
            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });
        }

        private void button39_Click(object sender, EventArgs e)
        {
            SetTemperatureRequest request = new SetTemperatureRequest();
            request.fanTemperature = Utility.GetThresholdToByte(int.Parse(txtFan.Text));
            request.heaterTemperature = Utility.GetThresholdToByte(int.Parse(txtHeater.Text));

            ExDataFrame frame = MakeKorExRequest(ExDataFrameDefine.OP_SET_TEMPERATURE_COMMAND, request);

            byte[] data = frame.Serialize();

            SendByEthernet(_sessionContext, data, data.Length);

            ExDataFrameExt exFrame = new ExDataFrameExt();
            exFrame.frame = frame;

            BeginInvoke(_displayDataDelegate, new object[] { exFrame, lbxSend });


        }

        private void button40_Click(object sender, EventArgs e)
        {
            Utility.LaunchProcess(String.Format(@"C:\Program Files\WindowsApps\Microsoft.WindowsNotepad_11.2205.11.0_arm64__8wekyb3d8bbwe\Notepad\notepad.exe"));
        }
    }
    public class ExDataFrameExt
    {
        public DateTime txTime;
        public ExDataFrame frame;

        public ExDataFrameExt()
        {
            txTime = DateTime.Now;
        }
    }

   
    
}
