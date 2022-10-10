using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.API.Model;
using VDSCommon.Protocol.admin;
using VDSDBHandler.DBOperation.VDSManage;
using VDSManagerCtrl;


namespace AdminManageCtrl
{
    public class VDSAdminManager : VDSManager
    {
        VDSServer _adminServer = new VDSServer();

        Queue<SOCKET_MSG> socketMsgQueue = new Queue<SOCKET_MSG>();
        private object _lockQueue = new object();
        public ManualResetEvent socketMsgThreadExitEvent = new ManualResetEvent(false);
        bool _bSocketMsgProcessing = false;
        //MADataFrame _prevDataFrame = null;
        public AddMADataEvent _addMADataEvent = null;


        VDSControllerOperation vdsOp = new VDSControllerOperation();

        public override int StartManager()
        {
            StartAdminServer();
            return 1;
        }

        public override int StopManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            StopAdminServer();

            return 1;
        }

        public int StartAdminServer()
        {
            _adminServer.StopManager();
            _adminServer.SetAddress(AdminConfig.ADMIN_ADDRESS, AdminConfig.ADMIN_PORT, CLIENT_TYPE.VDS_CLIENT, AcceptCallback);
            
            _adminServer.StartManager();
            StartWorkThread();
            return 1;
        }

        public int StopAdminServer()
        {
            
            _adminServer.StopManager();
            StopWorkThread();
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
            String strLog;
            _bSocketMsgProcessing = false;

            socketMsgThreadExitEvent.WaitOne();

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public void AcceptCallback(IAsyncResult ar)
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
                    vdsClient._type = _adminServer._clientType;
                    vdsClient._socket = socket;
                    socket.BeginReceive(vdsClient.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(AdminReadCallback), vdsClient);
                    AddSessionContext(vdsClient);

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
                _adminServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void AdminReadCallback(IAsyncResult ar)
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
                    strLog = String.Format("제어기 --> 유지보수 서버 ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    AddSocketMsg(session, session.buffer, bytesRead);
                    socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(AdminReadCallback), session);
                }
                else
                {
                    // Main form 에 세선 연결 종료 Event 전송
                    PostMASessionDisConnectEvent(session); 
                    DeleteSessionContext(session);
                    
                }
            }
            catch (Exception ex)
            {
                PostMASessionDisConnectEvent(session);
                DeleteSessionContext(session);
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
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

        private int ProcessReceivePacket(SessionContext session, byte[] packet, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            int i = 0;
            while (i < packet.Length)
            {
                if (session._prevFrame == null) //_prevDataFrame
                {
                    session._prevFrame = new MADataFrame();
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"미완성 패킷 후속 처리 "));
                }

                i = (session._prevFrame as MADataFrame).Deserialize(packet, i);
                if ((session._prevFrame as MADataFrame).bDataCompleted)
                {
                    // processDataFrame....
                    ProcessDataFrame(session, (session._prevFrame as MADataFrame));
                    session._prevFrame   = null;
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
                    nResult = ProcessEventTrafficData(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_HISTORIC_TRAFFIC:
                    //nResult = ProcessHistoricTrafficData(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_HEARTBEAT:
                    nResult = ProcessHeartBeat(session, frame);
                    break;

                case MADataFrameDefine.OPCODE_VDS_CONFIG:
                    nResult = ProcessVDSConfig(session, frame);
                    break;

            }

            PostMADataEvent(session, frame);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessAuthVDS(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAAuthRequest request;
            MAAuthResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = new MAAuthResponse();
                request = frame.opDataFrame as MAAuthRequest;

                var controller = vdsOp.CheckVDSController(new VDS_CONTROLLER()
                {
                    CONTROLLER_ID = request.vdsControllerId,
                }, out SP_RESULT spResult);

                response.resultCode = Utility.GetResultCode(spResult.RESULT_CODE);
                response.resultMessage = spResult.ERROR_MESSAGE;

                if (response.resultCode == 100) // Status --> AUTHORIZED 로 변경.
                {

                }

                nResult = SendResponse(frame.OpCode, sessionContext, response);
            }
            else
            {

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        private int ProcessHeartBeat(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAHeartBeatRequest request;
            MAHeartBeatResponse response;

            if (frame != null && frame.opDataFrame != null)
            {
                response = new MAHeartBeatResponse();
                request = frame.opDataFrame as MAHeartBeatRequest;

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"vdsController id({request.vdsControllerId}) receive heartbeat and rackStatus= {request.rackStatus} "));

                response.resultCode = 100;
                response.resultMessage = "success";

                UpdateLastHeartBeatTime(request);

                nResult = SendResponse(frame.OpCode, sessionContext, response);

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        private int ProcessVDSConfig(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAVDSConfigRequest request;
            MAResponse response;

            if (frame != null && frame.opDataFrame != null)
            {
                response = new MAResponse();
                request = frame.opDataFrame as MAVDSConfigRequest;

                if(request!=null) // Config 정보 업데이트 
                {
                    vdsOp.UpdateVDSControllerConfig(new VDS_CONTROLLER()
                    {
                        CONTROLLER_ID = request.vdsControllerId,
                        VDS_CONFIG = JsonConvert.SerializeObject(request),
                    }, out SP_RESULT spResult);

                    response.resultCode = Utility.GetResultCode(spResult.RESULT_CODE);
                    response.resultMessage = spResult.ERROR_MESSAGE;

                }
                else
                {
                    response.resultCode = 500;
                    response.resultMessage = "request 객체가 NUL";
                }
                nResult = SendResponse(frame.OpCode, sessionContext, response);

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private int ProcessEventTrafficData(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MATrafficDataEvent request;
            MAResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = new MAResponse();
                request = frame.opDataFrame as MATrafficDataEvent;

                response.resultCode = 100;
                response.resultMessage = "success";

                nResult = SendResponse(frame.OpCode, sessionContext, response);


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

        public void SetMADataEventDelegate( AddMADataEvent addMADataEvent)
        {
            _addMADataEvent = addMADataEvent;
        }

        private void PostMADataEvent(SessionContext session, MADataFrame frame)
        {
            if(_addMADataEvent!=null)
            {
                _addMADataEvent(session, frame);
            }
        }

        private int  SendResponse(byte opCode , SessionContext session, IOpData response)
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


        public int RequestStartTrafficDataSend(SessionContext session, String vdsId)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAControlVDSRequest request = new MAControlVDSRequest();
            request.vdsControllerId = vdsId;
            request.commandName = MADataFrameDefine.COMMAND_TRAFFIC_SEND;
            request.operation = MADataFrameDefine.OPERATION_START;
            nResult = SendRequest(session, MADataFrameDefine.OPCODE_CONTROL_SERVICE, request);

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int RequestStopTrafficDataSend(SessionContext session, String vdsId)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAControlVDSRequest request = new MAControlVDSRequest();
            request.vdsControllerId = vdsId;
            request.commandName = MADataFrameDefine.COMMAND_TRAFFIC_SEND;
            request.operation = MADataFrameDefine.OPERATION_STOP;
            nResult = SendRequest(session, MADataFrameDefine.OPCODE_CONTROL_SERVICE, request);

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


        private int UpdateLastHeartBeatTime(MAHeartBeatRequest request)
        {
            int nResult = 0;
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            VDSControllerOperation db = new VDSControllerOperation(AdminConfig.GetDBConn());
            
            db.UpdateVDSHeartBeatTime(new VDS_CONTROLLER()
            {
                CONTROLLER_ID = request.vdsControllerId,
            }, out SP_RESULT spResult);

            nResult = spResult.RESULT_COUNT;
            if(spResult.RESULT_CODE.CompareTo("500")==0)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"{spResult.ERROR_MESSAGE}"));
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void PostMASessionDisConnectEvent(SessionContext session)
        {
            MADataFrame frame = new MADataFrame();
            frame.OpCode = MADataFrameDefine.OPCODE_VDS_DISCONNECT;
            PostMADataEvent(session, frame);

        }
    }
    
}
