using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using VDSCommon;
using VideoVDSManageCtrl.Protocol;

namespace VideoVDSManageCtrl
{
    public class VideoVDSManager : IVDSManager , IVDSDevice
    {
        UnisemVDSClient _unisemClient = new UnisemVDSClient();
        DataFrame _prevDataFrame = null;
        System.Timers.Timer _timer = null;



        String deviceAddress = String.Empty;
        int devicePort { get; set; }

        Queue<SOCKET_MSG> socketMsgQueue = new Queue<SOCKET_MSG>();
        private object _lockQueue = new object();
        public ManualResetEvent socketMsgThreadExitEvent = new ManualResetEvent(false);
        bool _bSocketMsgProcessing = false;

        public AddTrafficDataEvent _addTrafficDataEvent = null;
        public SetRtspStreamingUrlDelegate _setRtspUrlDelegate = null;


        public VideoVDSManager()
        {
            socketMsgThreadExitEvent.Reset();
        }

        public int AddSocketMsg(SessionContext session, byte[] packet, int size)
        {
            int nResult = 0;
            SOCKET_MSG socketMsg = new SOCKET_MSG();
            try
            {
                socketMsg.session = session;
                socketMsg.packet = new byte[size];
                socketMsg.size = size;
                Array.Copy(packet, socketMsg.packet, size);
                lock (_lockQueue)
                {
                    socketMsgQueue.Enqueue(socketMsg);
                    nResult = socketMsgQueue.Count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return nResult;
        }

        public SOCKET_MSG? GetSocketMsg()
        {
            SOCKET_MSG? Result = null;
            try
            {
                lock (_lockQueue)
                {
                    if(socketMsgQueue.Count> 0)
                        Result = socketMsgQueue.Dequeue();
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
        }

        public int StartManager()
        {
            
            return 1;
        }

        public int StopManager()
        {
            

            return 1;
        }


        private int VideoClientConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {

                InitUniSemClient(ref _unisemClient);

                _unisemClient._sessionContext = session;
                _unisemClient._sessionContext._socket.BeginReceive(_unisemClient._sessionContext.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(VideoClientReadCallback), _unisemClient._sessionContext);

                

                SendAuthVDSRequest(_unisemClient._sessionContext, VDSConfig.controllerConfig.ControllerId);

                //_unisemClient._status = SOCKET_STATUS.CONNECTED;
                //1. vds 정보 전송
                //2. heartbeat 전송 쓰레드 시작 
                strLog = String.Format("제어기-->영상VDS ({0}:{1}  접속 성공)", VDSConfig.controllerConfig.DeviceAddress, VDSConfig.controllerConfig.RemotePort);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                nResult = 1;
            }
            catch (Exception ex)
            {
                _unisemClient._status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public void InitUniSemClient(ref UnisemVDSClient unisemClient)
        {
            if (unisemClient == null)
                unisemClient = new UnisemVDSClient();

            unisemClient.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            unisemClient.vdsType = VDSConfig.GetVDSTypeName();
            _prevDataFrame = null;


        }

        public void VideoClientReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            String content = String.Empty;
            String strLog;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            SessionContext session = (SessionContext)ar.AsyncState;
            Socket socket = session._socket;
            try
            {
                // Read data from the client socket.
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    strLog = String.Format("영상VDS --> 제어기 ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    AddSocketMsg(session, session.buffer, bytesRead);

                    //byte[] packet = new byte[bytesRead];
                    //Array.Copy(session.buffer, 0, packet, 0, bytesRead);
                    //ProcessReceivePacket(session, packet, bytesRead);

                    socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(VideoClientReadCallback), session);

                }
                else
                {

                    Console.WriteLine("event client error");
                    //
                    //1. heartbeat 전송 쓰레드 중지 

                    CloseUnisemClient();


                }
            }
            catch (Exception ex)
            {
                CloseUnisemClient();
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        private void CloseUnisemClient()
        {
            if(_unisemClient!=null)
            {
                DeleteSessionContext(_unisemClient._sessionContext);
                //StopHeartBeatTimer();
                _unisemClient._status = SOCKET_STATUS.DISCONNECTED;
            }
        }

        private int Send(SessionContext session, byte[] byteData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 1;
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

        public int DeleteSessionContext(SessionContext session)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                session._socket.Shutdown(SocketShutdown.Both);
                session._socket.Close();

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }


        private int ProcessReceivePacket(SessionContext session, byte[] packet, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            int i = 0;
            while (i < packet.Length)
            {
                if (_prevDataFrame == null)
                {
                    _prevDataFrame = new DataFrame();
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"미완성 패킷 후속 처리 "));
                }

                i = _prevDataFrame.Deserialize(packet, i);
                if (_prevDataFrame.bDataCompleted)
                {
                    // processDataFrame....
                    ProcessDataFrame(session, _prevDataFrame);
                    _prevDataFrame = null;
                    nResult++;
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                }
            }
            // Not all data received. Get more.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessDataFrame(SessionContext session, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (frame.OpCode)
            {
                case DataFrameDefine.OPCODE_AUTH_VDS:
                    nResult = ProcessAuthVDS(session, frame);
                    break;
                case DataFrameDefine.OPCODE_EVENT_TRAFFIC:
                    nResult = ProcessEventTrafficData(session, frame);
                    break;
                case DataFrameDefine.OPCODE_HISTORIC_TRAFFIC:
                    nResult = ProcessHistoricTrafficData(session, frame);
                    break;
                case DataFrameDefine.OPCODE_HEARTBEAT:
                    nResult = ProcessHeartBeat(session, frame);
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessAuthVDS(SessionContext session, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSAuthResponse response;
            if(frame!=null && frame.opDataFrame!=null)
            {
                response = frame.opDataFrame as VDSAuthResponse;
                if(response.resultCode == 100)
                {
                    _unisemClient.rtspSourceURL = response.rtspSourceURL;
                    _unisemClient.rtspDetectionURL = response.rtspDetectionURL;
                    _unisemClient.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                    _unisemClient.vdsType = VDSConfig.GetVDSTypeName();
                    _unisemClient._status = SOCKET_STATUS.AUTHORIZED;
                    String[] rtspURLS = new String[2];
                    rtspURLS[0] = _unisemClient.rtspSourceURL;
                    rtspURLS[1] = _unisemClient.rtspDetectionURL;

                    SetRtspStreamingUrl(rtspURLS);


                    // 미수신 차량검지 정보 요청
                    
                    String startTime = DateTime.Now.Add(new TimeSpan(0, 0, -1, 0)).ToString(VDSConfig.RADAR_TIME_FORMAT);
                    String endTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                    SendHistoricalTrafficDataRequest(session, startTime, endTime, 1);



                }
                else
                {
                    _unisemClient._status = SOCKET_STATUS.UNAUTHORIZE;
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, response.resultMessage);
                }
                
                nResult = 1;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }

        
        private int ProcessEventTrafficData(SessionContext session, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSTrafficDataEvent trafficEvent;
            if (frame != null && frame.opDataFrame != null)
            {
                trafficEvent = frame.opDataFrame as VDSTrafficDataEvent;
                foreach(var trafficData in trafficEvent.trafficDataList)
                    nResult = AddTrafficDataEvent(trafficData);
                //var trafficData = trafficEvent.trafficDataList.FirstOrDefault();
                
                //Console.WriteLine($"ProcessEventTrafficData.. id={trafficData.id} , lane={trafficData.lane}, detectTime = {trafficData.detectTime}  ");
                //nResult = 1;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }

        private int ProcessHistoricTrafficData(SessionContext session, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSHistoricTrafficDataResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = frame.opDataFrame as VDSHistoricTrafficDataResponse;
                if (response.resultCode == 100)
                {
                    //_unisemClient.heartBeatTime = response.heartBeatTime;
                    //_unisemClient.lastHeartBeatTime = DateTime.Now;
                    //
                    int totalPage = Utility.GetTotalPageCount(response.totalCount, response.pageSize);
                    if(response.pageIndex < totalPage)
                    {
                        response.pageIndex++;
                        SendHistoricalTrafficDataRequest(session, response.startDateTime, response.endDateTime, response.pageIndex);
                    }
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, response.resultMessage);
                }

                nResult = 1;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        private int ProcessHeartBeat(SessionContext session, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSHeartBeatResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = frame.opDataFrame as VDSHeartBeatResponse;
                if (response.resultCode == 100)
                {
                    //_unisemClient.heartBeatTime = response.heartBeatTime;
                    _unisemClient.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, response.resultMessage);
                }

                nResult = 1;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }
        

        private int SendAuthVDSRequest(SessionContext session, String vdsId)
        {
            int nResult = 0;
            VDSAuthRequest request = new VDSAuthRequest();
            DataFrame frame = new DataFrame();
            frame.RequestType = DataFrameDefine.TYPE_REQUEST;
            frame.OpCode = DataFrameDefine.OPCODE_AUTH_VDS;

            request.vdsControllerId = vdsId;
            frame.opDataFrame = request;

            byte[] packet = frame.Serialize();

            nResult = Send(session, packet);

            return nResult;

        }


        private int SendHeartBeatRequest(SessionContext session, String vdsId)
        {
            int nResult = 0;
            VDSHeartBeatRequest request = new VDSHeartBeatRequest();
            DataFrame frame = new DataFrame();
            frame.RequestType = DataFrameDefine.TYPE_REQUEST;
            frame.OpCode = DataFrameDefine.OPCODE_HEARTBEAT;

            request.vdsControllerId = vdsId;
            request.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

            frame.opDataFrame = request;

            byte[] packet = frame.Serialize();

            nResult = Send(session, packet);

            return nResult;

        }

        private int SendHistoricalTrafficDataRequest(SessionContext session, String startTime, String endTime, int pageIndex)
        {
            int nResult = 0;
            VDSHistoricTrafficDataRequest request = new VDSHistoricTrafficDataRequest();
            request = new VDSHistoricTrafficDataRequest();
            request.startDateTime = startTime;
            request.endDateTime = endTime;
            request.pageIndex = pageIndex;
            request.pageSize = VDSConfig.PAGE_SIZE;
            
            DataFrame frame = new DataFrame();
            frame.RequestType = DataFrameDefine.TYPE_REQUEST;
            frame.OpCode = DataFrameDefine.OPCODE_HISTORIC_TRAFFIC;
            frame.opDataFrame = request;
            byte[] packet = frame.Serialize();
            nResult = Send(session, packet);
            return nResult;
        }

        public void StartHeartBeatTimer()
        {
            if (_timer == null)
                _timer = new System.Timers.Timer();

            _timer.Interval = 1000*10; // 10 초마다 타이머 동작
            _timer.Elapsed += OnHeartBeatCheck;
            _timer.Start();
        }

        private void OnHeartBeatCheck(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("OnHeartBeatCheck...");
            TimeSpan timeSpan;
            DateTime heartBeatTime;
            if(_unisemClient!=null && _unisemClient._status == SOCKET_STATUS.AUTHORIZED)
            {
                if(!String.IsNullOrEmpty(_unisemClient.heartBeatTime))
                {
                    heartBeatTime = DateTime.ParseExact(_unisemClient.heartBeatTime, VDSConfig.RADAR_TIME_FORMAT, null);
                    timeSpan = DateTime.Now - heartBeatTime;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, $"OnHeartBeatCheck...send heartbeat {timeSpan.TotalSeconds} 초");
                    if (timeSpan.TotalSeconds < 60)
                        SendHeartBeatRequest(_unisemClient._sessionContext, _unisemClient.vdsControllerId);
                    else
                    {
                        Console.WriteLine($"OnHeartBeatCheck...시간 초과...{timeSpan.TotalSeconds} 초");
                        DeleteSessionContext(_unisemClient._sessionContext);
                    }
                }
                
            }
            

        }

        public void StopHeartBeatTimer()
        {
            if(_timer!=null)
            {
                _timer.Stop();

            }
            _timer = null;
        }

        public int StartDevice(String address, int port, int localPort = 0)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            // 처리 쓰레드 시작 
            StartWorkThread();
            deviceAddress = address;
            devicePort = port;
            //_unisemClient.SetAddress(VDSConfig.VDS_DEVICE_ADDRESS, VDSConfig.VDS_DEVICE_PORT, CLIENT_TYPE.VIDEO_VDS_CLIENT, VideoClientConnectCallback, VideoClientReadCallback, SendCallback);
            _unisemClient.vdsControllerId = VDSConfig.controllerConfig.ControllerId;
            _unisemClient.SetAddress(deviceAddress, devicePort, CLIENT_TYPE.VIDEO_VDS_CLIENT, VideoClientConnectCallback, VideoClientReadCallback, SendCallback);
            _unisemClient.StartConnect();
            StartHeartBeatTimer();

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;

        }

        public int StopDevice()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            StopHeartBeatTimer();
            _unisemClient.StopCheckConnection();

            // 처리 쓰레드 종료 
            StopWorkThread();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }


        public int StartWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            StartProcessSocketMsgThread();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }





        public int StopWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            _bSocketMsgProcessing = false;

            socketMsgThreadExitEvent.WaitOne();

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }


        public void StartProcessSocketMsgThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            new Thread(() =>
            {
                try
                {
                    _bSocketMsgProcessing = true;
                    while (_bSocketMsgProcessing)
                    {
                        SOCKET_MSG? socketMsg = GetSocketMsg();
                        if(socketMsg != null)
                        {
                            ProcessReceivePacket(socketMsg?.session, socketMsg?.packet, (int)socketMsg?.size);
                        }
                        Thread.Sleep(100);
                    }
                    socketMsgThreadExitEvent.Set();

                }
                catch (Exception ex)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                }
            }
          ).Start();
          Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public bool isService()
        {
            return _unisemClient._status == SOCKET_STATUS.AUTHORIZED;
        }

        public int SetDeviceTime(object callbackFunc, object workData, DateTime? date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if(date.HasValue)
            {
                DateTime osTime = date.Value;
                nResult = Utility.SetOsTime(osTime);
            }
            
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int CheckVDSStatus(ref byte[] status, ref byte[] checkTime)
        {
            return 1;
        }

        public int SetAddTrafficDataEventDelegate(AddTrafficDataEvent addTrafficDataEvent)
        {
            _addTrafficDataEvent = addTrafficDataEvent;
            return 1;
        }

        
        public int AddTrafficDataEvent(TrafficDataEvent trafficDataEvent)
        {
            int nResult = 0;
            if (_addTrafficDataEvent != null && trafficDataEvent!=null)
            {
                trafficDataEvent.vds_type = VDSConfig.GetVDSTypeName();
                nResult = _addTrafficDataEvent(trafficDataEvent);
            }
            return nResult;
        }

        public int SetRtspStreamingUrlDelegate(SetRtspStreamingUrlDelegate setRtspUrlDelegate)
        {
            _setRtspUrlDelegate = setRtspUrlDelegate;
            return 1;
        }

       
        public int SetRtspStreamingUrl(String[] urls)
        {
            int Result = 0;
            if(_setRtspUrlDelegate != null)
            {
                
                Result = _setRtspUrlDelegate(urls);
                 
            }
            return Result;
        }
    }
}
