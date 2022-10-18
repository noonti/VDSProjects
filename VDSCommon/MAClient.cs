using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using VDSCommon.Protocol.admin;

namespace VDSCommon
{
    public class MAClient : VDSClient
    {


        Queue<SOCKET_MSG> socketMsgQueue = new Queue<SOCKET_MSG>();
        private object _lockQueue = new object();
        bool _bSocketMsgProcessing = false;

        public bool _bTrafficDataEventSend = false; 

        public ManualResetEvent socketMsgThreadExitEvent = new ManualResetEvent(false);

        MADataFrame _prevDataFrame = null;
        System.Timers.Timer _timer = null;

        public MAClient()
        {
            socketMsgThreadExitEvent.Reset();
        }

        public int StartService()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            _bTrafficDataEventSend = false;

            SetAddress(VDSConfig.controllerConfig.MAServerAddress, VDSConfig.controllerConfig.MAServerPort, CLIENT_TYPE.VDS_CLIENT, MAConnectCallback, MAReadCallback, SendCallback);
            StartConnect();

            StartProcessSocketMsgThread();
            StartHeartBeatTimer();

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int StopService()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            StopCheckConnection();
            StopProcessSocketMsgThread();
            StopHeartBeatTimer();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }


        public int StopProcessSocketMsgThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            _bSocketMsgProcessing = false;

            socketMsgThreadExitEvent.WaitOne();

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        private int MAConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                _sessionContext = session;
                _sessionContext._socket.BeginReceive(_sessionContext.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(MAReadCallback), _sessionContext);

                
                SendAuthVDSRequest(_sessionContext, VDSConfig.controllerConfig.ControllerId);

                strLog = String.Format("제어기-->MA 서버 ({0}:{1}  접속 성공)", VDSConfig.controllerConfig.MAServerAddress, VDSConfig.controllerConfig.MAServerPort);

                heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
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


        public void MAReadCallback(IAsyncResult ar)
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
                    strLog = String.Format("MA Server --> 제어기 ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    AddSocketMsg(session, session.buffer, bytesRead);

                    socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(MAReadCallback), session);

                }
                else
                {

                    Console.WriteLine("event client error");

                    CloseMAClient();

                }
            }
            catch (Exception ex)
            {
                CloseMAClient();
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
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
            String strLog;
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

        private void CloseMAClient()
        {
            DeleteSessionContext(_sessionContext);
            _bTrafficDataEventSend = false;
            _status = SOCKET_STATUS.DISCONNECTED;

        }

        private int SendAuthVDSRequest(SessionContext session, String vdsId)
        {
            int nResult = 0;
            MAAuthRequest request = new MAAuthRequest();
            request.vdsControllerId = vdsId;
            nResult = SendRequest(session, MADataFrameDefine.OPCODE_AUTH_VDS, request);
            return nResult;

        }

        private int SendVDSConfigRequest(SessionContext session, String vdsId)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if(_status == SOCKET_STATUS.AUTHORIZED)
            {
                MAVDSConfigRequest request = new MAVDSConfigRequest();
                request.vdsControllerId = vdsId;
                request.controllerConfig = VDSConfig.controllerConfig;
                request.kictConfig = VDSConfig.kictConfig;
                request.korExConfig = VDSConfig.korExConfig;

                nResult = SendRequest(session, MADataFrameDefine.OPCODE_VDS_CONFIG, request);
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" _status={_status} . 인증되지 않은 상태에서 설정 정보 전송 시도"));
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int Send(SessionContext session, byte[] byteData)
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
                    if (socketMsgQueue.Count > 0)
                        Result = socketMsgQueue.Dequeue();
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return Result;
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
                        if (socketMsg != null)
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


        private int ProcessReceivePacket(SessionContext session, byte[] packet, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            int i = 0;
            while (i < packet.Length)
            {
                if (_prevDataFrame == null)
                {
                    _prevDataFrame = new MADataFrame();
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


        private int ProcessDataFrame(SessionContext session, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (frame.OpCode)
            {
                case MADataFrameDefine.OPCODE_AUTH_VDS:
                    nResult = ProcessAuthVDS(session, frame);
                   
                    break;
                case MADataFrameDefine.OPCODE_EVENT_TRAFFIC:
                    
                    break;
                case MADataFrameDefine.OPCODE_HISTORIC_TRAFFIC:
                    
                    break;
                case MADataFrameDefine.OPCODE_HEARTBEAT:
                    nResult = ProcessHeartBeat(session, frame);
                    break;

                case MADataFrameDefine.OPCODE_CONTROL_SERVICE:
                    nResult = ProcessControlService(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_VDS_CONFIG:
                    nResult = ProcessVDSConfig(session, frame);
                    break;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessAuthVDS(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAAuthResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                //_status = SOCKET_STATUS.UNAUTHORIZE;

                response = new MAAuthResponse();
                response = frame.opDataFrame as MAAuthResponse;

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"VDS Controller 인증 응답 resultCode = {response.resultCode} , resultMessage={response.resultMessage} "));
                switch(response.resultCode)
                {
                    case 100:
                        _status = SOCKET_STATUS.AUTHORIZED;
                        break;
                    case 500:
                        _status = SOCKET_STATUS.UNAUTHORIZE; // 권한이 없기 때문에...close ...
                        CloseMAClient();
                        break;
                }
            }

            // 자신의 Config 정보 전송 
            SendVDSConfigRequest(_sessionContext, VDSConfig.controllerConfig.ControllerId);


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public void StartHeartBeatTimer()
        {
            heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

            if (_timer == null)
                _timer = new System.Timers.Timer();

            _timer.Interval = 1000 * 10; // 10 초마다 타이머 동작
            _timer.Elapsed += OnHeartBeatCheck;
            _timer.Start();
        }

        private void OnHeartBeatCheck(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("OnHeartBeatCheck...");
            TimeSpan timeSpan;
            DateTime heartBeatTime;
            if (_status == SOCKET_STATUS.AUTHORIZED)
            {
                if (!String.IsNullOrEmpty(this.heartBeatTime))
                {
                    heartBeatTime = DateTime.ParseExact(this.heartBeatTime, VDSConfig.RADAR_TIME_FORMAT, null);
                    timeSpan = DateTime.Now - heartBeatTime;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, $"OnHeartBeatCheck...send heartbeat {timeSpan.TotalSeconds} 초");
                    if (timeSpan.TotalSeconds < 60)
                        SendHeartBeatRequest(_sessionContext, VDSConfig.controllerConfig.ControllerId);
                    else
                    {
                        Console.WriteLine($"OnHeartBeatCheck...시간 초과...{timeSpan.TotalSeconds} 초");
                        CloseMAClient();
                    }
                }

            }


        }

        public void StopHeartBeatTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();

            }
            _timer = null;
        }


        private int SendHeartBeatRequest(SessionContext session, String vdsId)
        {
            int nResult = 0;
            MAHeartBeatRequest request = new MAHeartBeatRequest();

            request.vdsControllerId = vdsId;
            request.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            request.rackStatus = VDSRackStatus.GetRackStatus();
            nResult = SendRequest(session, MADataFrameDefine.OPCODE_HEARTBEAT, request);
            return nResult;

        }

        private int ProcessHeartBeat(SessionContext session, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAHeartBeatResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = frame.opDataFrame as MAHeartBeatResponse;
                if (response.resultCode == 100)
                {
                    heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
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

        private int ProcessControlService(SessionContext session, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAControlVDSRequest request;
            MAControlVDSResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                request = frame.opDataFrame as MAControlVDSRequest;

                switch(request.commandName)
                {
                    case MADataFrameDefine.COMMAND_TRAFFIC_SEND:
                        nResult = ProcessTrafficSendCommand(session, request);

                        break;
                    case MADataFrameDefine.COMMAND_VDS_SERVICE:
                        nResult = ProcessVDSServiceCommand(session, request);
                        break;
                }
                response = new MAControlVDSResponse();
                response.resultCode = nResult==1?100:500;
                response.resultMessage = nResult == 1 ? "SUCCESS" : "FAILED";
                nResult = SendResponse(frame.OpCode, session, response);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessTrafficSendCommand(SessionContext session, MAControlVDSRequest request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            
            switch(request.operation)
            {
                case MADataFrameDefine.OPERATION_START:
                    _bTrafficDataEventSend = true;
                    break;
                case MADataFrameDefine.OPERATION_STOP:
                    _bTrafficDataEventSend = false;
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private int ProcessVDSServiceCommand(SessionContext session, MAControlVDSRequest request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
           
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private int ProcessVDSConfig(SessionContext session, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                //_status = SOCKET_STATUS.UNAUTHORIZE;
                response = new MAResponse();
                response = frame.opDataFrame as MAResponse;
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"VDS Controller 인증 응답 resultCode = {response.resultCode} , resultMessage={response.resultMessage} "));
                nResult = 1;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
        


        public int SendTrafficData(TrafficDataEvent trafficData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if(_status == SOCKET_STATUS.AUTHORIZED && _bTrafficDataEventSend ==true)
            {
                MATrafficDataEvent request = new MATrafficDataEvent();
                request.vdsControllerId = VDSConfig.controllerConfig.ControllerId;
                TrafficData data = new TrafficData();
                data.SetTrafficDataEvent(trafficData);
                request.trafficDataList.Add(data);
                nResult = SendRequest(_sessionContext, MADataFrameDefine.OPCODE_EVENT_TRAFFIC, request);
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Status = {_status} , 실시간 검지데이터 전송(_bTrafficDataEventSend)={_bTrafficDataEventSend}"));
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int SendRequest(SessionContext session, byte opCode, IOpData request)
        {
            int nResult = 0;
            MADataFrame frame = new MADataFrame();
            frame.RequestType = MADataFrameDefine.TYPE_REQUEST;
            frame.OpCode = opCode;
            frame.opDataFrame = request;
            byte[] packet = frame.Serialize();
            nResult = Send(session, packet);
            return nResult;
        }

        private int SendResponse(byte opCode, SessionContext session, IOpData response)
        {
            int nResult = 0;
            MADataFrame resFrame = new MADataFrame();
            resFrame.RequestType = MADataFrameDefine.TYPE_RESPONSE;
            resFrame.OpCode = opCode;// MADataFrameDefine.OPCODE_AUTH_VDS;
            resFrame.opDataFrame = response;

            byte[] packet = resFrame.Serialize();
            nResult = Send(session, packet);

            return nResult;
        }
    }
}
