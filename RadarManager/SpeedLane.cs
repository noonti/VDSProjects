using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using VDSCommon;
using VDSCommon.DataType;

namespace RadarManageCtrl
{
    public enum RADAR_CONNECT_TYPE
    {
        SERIAL = 1,
        ETHERNET = 2
    }

    public enum RADAR_TYPE
    {
        RADAR_TYPE_DR500 = 0,
        RADAR_TYPE_DR1500 = 1,
        RADAR_TYPE_DR2500 = 2,
        RADAR_TYPE_SS300 = 3,
        RADAR_TYPE_SS300U = 4,
        RADAR_TYPE_PD300 = 5,
        RADAR_TYPE_PD310 = 6,
        RADAR_TYPE_DC300 = 7,
        RADAR_TYPE_DC310 = 8,
        RADAR_TYPE_SPEEDLANE = 9,
        RADAR_TYPE_PD420 = 10,
        RADAR_TYPE_SS400 = 11,
        RADAR_TYPE_SPEEDLANEPRO = 12,
        RADAR_TYPE_SS400U = 13,
        RADAR_TYPE_UNKNOWN = 14,
        MAX_RADAR_TYPE_ENUM = 15
    }

    public class SpeedLane
    {
        public String ipAddress;
        public int port;

        public RadarClient tcpRadar;
        public RadarPacket _prevRadarPacket;
        public RADAR_CONNECT_TYPE connectType;
        public byte[] _lastFilter = null;
        public bool _bStreaming = false;
        public ManualResetEvent threadExitEvent = new ManualResetEvent(false);
        public AddTrafficDataEvent _addRadarData = null;
        public STREAM_CONFIG[] speedLaneStreamConfig = new STREAM_CONFIG[4];
        public Dictionary<String, String> _radarPropertyList = new Dictionary<string, string>();
        public DateTime _radarDateTime = new DateTime();

        private object _requestLock = new object();
        public Queue<RadarRequest> _requestQueue = new Queue<RadarRequest>();
        public Dictionary<String, UInt16> _radarVarValueList = new Dictionary<string, ushort>();

        //public bool _sysinfo ;
        public bool _initialTargetStreaming = false;

        public SpeedLane()
        {

            CreateStreamConfig();
            

        }

        public SpeedLane(String ip, int portNo)
        {
            ipAddress = ip;
            port = portNo;
            connectType = RADAR_CONNECT_TYPE.ETHERNET;
            _prevRadarPacket = null;
            threadExitEvent.Reset();

            CreateStreamConfig();
        }


        public void CreateStreamConfig()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            speedLaneStreamConfig[0] = new STREAM_CONFIG("S1", 3);
            speedLaneStreamConfig[0].bits[0] = PacketDefine.STREAM_LN_SLOW;
            speedLaneStreamConfig[0].bits[1] = PacketDefine.STREAM_VTG;
            speedLaneStreamConfig[0].bits[2] = PacketDefine.STREAM_TG_SUMM;


            speedLaneStreamConfig[1] = new STREAM_CONFIG("", 3);

            speedLaneStreamConfig[2] = new STREAM_CONFIG("SM", 3);
            speedLaneStreamConfig[2].bits[0] = PacketDefine.STREAM_LN_SLOW;
            speedLaneStreamConfig[2].bits[1] = PacketDefine.STREAM_VTG;
            speedLaneStreamConfig[2].bits[2] = PacketDefine.STREAM_TG_SUMM;

            speedLaneStreamConfig[3] = new STREAM_CONFIG("", 3);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }



        public int ConnectToRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            //_initialTargetStreaming = false;
            switch (connectType)
            {
                case RADAR_CONNECT_TYPE.SERIAL:
                    nResult = ConnectToSerialRadar();
                    break;
                case RADAR_CONNECT_TYPE.ETHERNET:
                    nResult = ConnectToTCPRadar();
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int DisConnectToRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (connectType)
            {
                case RADAR_CONNECT_TYPE.SERIAL:
                    nResult = DisConnectToSerialRadar();
                    break;
                case RADAR_CONNECT_TYPE.ETHERNET:
                    nResult = DisConnectToTCPRadar();
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int ConnectToSerialRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;



            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int ConnectToTCPRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                tcpRadar = new RadarClient();
                nResult = tcpRadar.SetAddress(ipAddress, port, CLIENT_TYPE.VDS_CLIENT, RadarConnectCallback, RadarReadCallback, RadarSendCallback);
                //nResult = tcpRadar.SetAddress(ipAddress, port);
                if (nResult > 0)
                {
                    nResult = tcpRadar.StartConnect();
                }
                strLog = String.Format("ip={0} port ={1} connect to radar",ipAddress, port);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        

        public int DisConnectToSerialRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            //TODO 시리얼포트 연결 종료 루틴 


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int DisConnectToTCPRadar()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                if (tcpRadar != null)
                {
                    tcpRadar.Stop();
                    nResult = 1;
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }



        public int SetPacketData(ref byte[] packetData, byte data, ref int packetSize)
        {
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int i = 0;
            switch (data)
            {
                case PacketDefine.MARK_BEGIN:
                    packetData[packetSize++] = PacketDefine.ESC;
                    packetData[packetSize++] = PacketDefine.ESC_MARK_BEGIN;
                    i += 2;
                    break;
                case PacketDefine.ESC:
                    packetData[packetSize++] = PacketDefine.ESC;
                    packetData[packetSize++] = PacketDefine.ESC_ESC;
                    i += 2;
                    break;
                case PacketDefine.MARK_END:
                    packetData[packetSize++] = PacketDefine.ESC;
                    packetData[packetSize++] = PacketDefine.ESC_MARK_END;
                    i += 2;
                    break;
                default:
                    packetData[packetSize++] = data;
                    i++;
                    break;
            }
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return i;
        }

        

        public byte[] MakeRadarPacket(byte[] data, int size, out int packetSize)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            byte[] result = new byte[VDSConfig.PACKET_SIZE]; // MARKB DATA CRC CRC>>8 MARKE
            UInt16 crc = 0; // crc initialize
            packetSize = 0;

            // MARKB + data + crc(하위8bit) + (crc>>8)(상위8bit) + MARKE

            result[packetSize++] = PacketDefine.MARK_BEGIN;
            for (int i = 0; i < size; i++)
            {
                SetPacketData(ref result, data[i], ref packetSize);
                // CRC 계산
                PacketDefine.CalculateCRC(ref crc, data[i]);
            }
            // CRC 하위 8 bit  
            SetPacketData(ref result, (byte)crc, ref packetSize);

            // CRC 상위 8 bit 
            SetPacketData(ref result, (byte)(crc >> 8), ref packetSize);

            result[packetSize++] = PacketDefine.MARK_END;
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        
        private int RadarConnectCallback(SessionContext session , SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if(status == SOCKET_STATUS.CONNECTED)
                {
                    // 연결 후 바로 패킷 스트리밍 요청 위해....
                    _initialTargetStreaming = false;

                    session._socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(RadarReadCallback), session);
                    strLog = String.Format($"레이더 ({ipAddress}:{port})  접속 성공");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    GetSysInfo(null);
                    GetTargetStreamVariable();

                }
                else if(status == SOCKET_STATUS.DISCONNECTED)
                {
                    strLog = String.Format($"레이더 ({ipAddress}:{port})  접속 실패");
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                    Console.WriteLine(strLog);

                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                tcpRadar._status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }



        public void RadarReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

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
                Console.WriteLine("RadarReadCallback {0} byte", bytesRead);
                if (bytesRead > 0)
                {
                    strLog = String.Format("RadarReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    int nResult = ProcessReceivePacket(session.buffer, bytesRead);
                    // Not all data received. Get more.  
                    socket.BeginReceive(session.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(RadarReadCallback), session);

                }
                else
                {
                    if (socket != null && tcpRadar._status != SOCKET_STATUS.DISCONNECTED)
                    {
                        strLog = String.Format("레이더 연결 종료");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        //Console.WriteLine("22Close socket");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        tcpRadar._status = SOCKET_STATUS.DISCONNECTED;
                    }
                }
            }
            catch (Exception ex)
            {
                if (socket != null && tcpRadar._status != SOCKET_STATUS.DISCONNECTED)
                {
                    strLog = String.Format("레이더 연결 종료");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    //Console.WriteLine("22Close socket");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    tcpRadar._status = SOCKET_STATUS.DISCONNECTED;
                }

                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        


        private void RadarSendCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

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
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }



        public int SendToRadar(byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (connectType)
            {
                case RADAR_CONNECT_TYPE.SERIAL:
                    nResult = SendBySerial(data, size);
                    break;
                case RADAR_CONNECT_TYPE.ETHERNET:
                    nResult = SendByEthernet(data, size);
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendBySerial(byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendByEthernet(byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                // Begin sending the data to the remote device.  
                if (tcpRadar._status == SOCKET_STATUS.CONNECTED)
                {
                    tcpRadar._sessionContext._socket.BeginSend(data, 0, size, 0,
                            new AsyncCallback(RadarSendCallback), tcpRadar._sessionContext._socket);
                    //tcpRadar.SendAndReceive()
                    nResult = 1;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int AddRadarData(TrafficDataEvent radarData)
        {
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            if (_addRadarData != null)
            {
                nResult = _addRadarData(radarData);
                strLog = String.Format("radar Data 추가. Quque 갯수: {0}", nResult);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public void SetAddRadarDataDelegate(AddTrafficDataEvent addRadarData)
        {
            _addRadarData = addRadarData;
        }

        public int SetRadarProperty(String[] propertyList)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                _radarPropertyList.Clear();
                foreach(var property in propertyList)
                {
                    var keyValues = property.Split('=');
                    if (keyValues != null && keyValues.Length ==2)//&& !String.IsNullOrEmpty(keyValues[0]))
                    {
                        _radarPropertyList.Add(keyValues[0], keyValues[1]);
                    }
                }
                nResult = _radarPropertyList.Count;
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());


            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
            
        }

        public String GetRadarProperty(String key)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            String Result = String.Empty;

            try
            {
                Result = _radarPropertyList[key];
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return Result;

        }

        public int AddRequestInfo(RadarRequest requestInfo)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            lock(_requestLock)
            {
                _requestQueue.Enqueue(requestInfo);
                nResult = _requestQueue.Count;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public RadarRequest GetRequestInfo()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            RadarRequest Result = null;
            try
            {
                lock (_requestLock)
                {
                    Result = _requestQueue.Dequeue();
                }
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return Result;
        }


        public int GetSysInfo(RadarRequest prevRequest)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                byte[] sendData = new byte[1];
                sendData[0] = PacketDefine.MSG_SYSINFO;

                int msgSize = 0;
                byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                nResult = SendToRadar(sendMsg, msgSize);

                if (nResult>0)
                {
                    RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_SYSINFO,null, prevRequest!=null? prevRequest._callbackFunc: null, prevRequest != null ? prevRequest._callbackParams: null);
                    AddRequestInfo(requestInfo);
                }
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

       

        public int StartLiveCamera()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                byte[] sendData = new byte[2];
                sendData[0] = PacketDefine.MSG_CAMERA_CTL;
                sendData[1] = PacketDefine.CAMERA_CTL_START;
                int msgSize = 0;
                byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);

                nResult = SendToRadar(sendMsg, msgSize);
                if (nResult > 0)
                {
                    Object[] param = new Object[1];
                    param[0] = PacketDefine.CAMERA_CTL_START;
                    RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_CAMERA_CTL, param, null, null);
                    AddRequestInfo(requestInfo);
                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public int StopLiveCamera()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                byte[] sendData = new byte[2];
                sendData[0] = PacketDefine.MSG_CAMERA_CTL;
                sendData[1] = PacketDefine.CAMERA_CTL_STOP;
                int msgSize = 0;
                byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                nResult = SendToRadar(sendMsg, msgSize);
                if (nResult > 0)
                {
                    Object[] param = new Object[1];
                    param[0] = PacketDefine.CAMERA_CTL_STOP;
                    RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_CAMERA_CTL, param, null, null);
                    AddRequestInfo(requestInfo);
                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int GetTargetStreamVariable()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {  // domain, enable, STREAM_TYPE_ENUM
               // ENV_DOMAIN.ENV_VOLATILE, (byte)ENV_BOOL.TRUE, STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM
                STREAM_CONFIG config = speedLaneStreamConfig[(int)STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM];
                nResult = RemoteGetVarByName(ENV_DOMAIN.ENV_VOLATILE, config.var);
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SetTargetSummaryStreaming(byte enable)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {  // domain, enable, STREAM_TYPE_ENUM
               // ENV_DOMAIN.ENV_VOLATILE, (byte)ENV_BOOL.TRUE, STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM
                STREAM_CONFIG config = speedLaneStreamConfig[(int)STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM];
                if(_radarVarValueList.ContainsKey(config.var))
                {
                    UInt16 oldValue = _radarVarValueList[config.var];
                    Int16 bitMask = (Int16)config.bits[(int)STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM];
                    UInt16 temp = (UInt16)(oldValue & bitMask);
                    UInt16 newValue = oldValue;


                    strLog = String.Format($"RemoteSetVarBitByName 변수={config.var} enable={enable}, oldValue ={oldValue}, bitmask={bitMask}, temp={temp}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                    if (enable == 1 && temp ==0)
                    {
                        newValue = (UInt16)(oldValue | (UInt16)bitMask);
                    }
                    else if (enable == 0 && temp != 0)
                    {
                        newValue = (UInt16)(oldValue & (UInt16)~bitMask);
                    }
                    else
                    {
                        strLog = String.Format($"RemoteSetVarBitByName 변수={config.var} 는 서로 같은 상태(old={oldValue} , new={newValue}) 이므로 값 변화 없음 ");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    }
                    if (newValue != oldValue)
                    {
                        strLog = String.Format($"RemoteSetVarBitByName 변수={config.var} 이전값={oldValue}, 새로운값={newValue}. 변수 설정 요청  ");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        nResult = RemoteSetVarByName(ENV_DOMAIN.ENV_VOLATILE, config.var, newValue);
                    }
                    
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"변수({config.var}) 존재하지 않음"));
                }
            }  
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

//        public int GetTargetSummaryInfo()
//        {
//            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
//            int nResult = 0;
//            String strLog;
////            byte[] sendData;
//            try
//            {
//                //TODO 테스트 코드임. 
//                nResult = ControlTargetStream(ENV_DOMAIN.ENV_VOLATILE, (byte)ENV_BOOL.TRUE, STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM);
//                if(nResult>0)
//                {
//                    StartTargetSummaryInfoStreaming();
//                }
//                strLog = String.Format("ControlTargetStream 함수 {0} 리턴", nResult);
//                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
//            }
//            catch (Exception ex)
//            {
//                nResult = 0;
//                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
//            }
//            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
//            return nResult;
//        }

        public int Login(String userId, String passwd)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                _lastFilter = new byte[2];
                _lastFilter[0] = PacketDefine.MSG_STATUS;
                _lastFilter[1] = 0x00;

                byte[] sendData = new byte[VDSConfig.PACKET_SIZE];
                int msgSize = 0;

                sendData[msgSize++] = PacketDefine.MSG_LOGIN;
                sendData[msgSize++] = PacketDefine.LOGIN_VERSION;

                byte[] id = Utility.StringToByte(userId);
                byte[] pass = Utility.StringToByte(passwd);

                int authDataLen = id.Length + 1 + pass.Length + 1;

                byte[] authData = new byte[authDataLen]; // 널문자열 처리
                Array.Copy(id, 0,authData , 0, id.Length);
                Array.Copy(pass, 0, authData, id.Length+1, pass.Length);

                //Array.Copy(authData, 0, id, 0, id.Length);
                //Array.Copy(authData, id.Length+1, pass, 0, pass.Length);

                //XOR 처리...
                SetDataXOR(ref authData, authDataLen, Utility.StringToByte(PacketDefine.XOR_KEY));
                //Array.Copy(sendData, msgSize, authData, 0, authDataLen);
                Array.Copy(authData, 0, sendData, msgSize, authDataLen);
                msgSize += authDataLen;

                byte[] sendMsg = MakeRadarPacket(sendData, msgSize, out msgSize);
                nResult = SendToRadar(sendMsg, msgSize);
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public void SetDataXOR(ref byte [] data,int len, byte[] key)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            int keyLen = key.Length;
            int i, j;
            i = 0;
            j = 0;
            while(i< len)
            {
                data[i++] ^= key[j++];
                if (j >= keyLen)
                    j = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public int PacketProcess(byte [] data, int maxLen)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessRadarPacket(RadarPacket packet)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            RadarRequest requestInfo = null;
            try
            {

                if(packet.data[0] != PacketDefine.MSG_TGSTREAM_SUMM)
                    requestInfo = GetRequestInfo();// _requestQueue.Dequeue();
                switch (packet.data[0])
                {
                    case PacketDefine.MSG_SYSINFO:
                        ProcessGetSysInfo(packet, requestInfo);
                        break;

                    case PacketDefine.MSG_TGSTREAM_SUMM: // Target Summary Info (streasming)
                        ProcessTargetSummaryInfo(packet);
                        break;
                    case PacketDefine.MSG_STATUS:
                        ProcessMsgStatus(packet, requestInfo); // StartLiveCamera, StopLiveCamera, RemoteSetVarByName, SetRadarTime
                        break;
                    case PacketDefine.MSG_VAR:    // RemoteGetVarByName
                        ProcessGetVar(packet, requestInfo);
                        break;
                }
                // 1. 데이터 유형이 Target Summary 인지 확인
                // 2. 요청에 대한 응답인 경우 처리
                // 3. 최근 요청 정보 저장...
                // 4. 

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        //public int ControlTargetStream(ENV_DOMAIN domain, byte enable, STREAM_TYPE_ENUM streamType )
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    int nResult = 0;
        //    try
        //    {
        //        STREAM_CONFIG config = speedLaneStreamConfig[(int)streamType];

        //        if (enable != 0) // enable 
        //        {
        //            nResult = RemoteSetVarBitByName(domain, config.var, (Int16)config.bits[(int)streamType], (Int16)ENV_BOOL.TRUE);
        //        }
        //        else             // disable
        //        {
        //            nResult = RemoteSetVarBitByName(domain, config.var, (Int16)config.bits[(int)streamType], (Int16)ENV_BOOL.FALSE);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
        //    }
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        //    return nResult;
        //}

        //public int RemoteSetVarBitByName(ENV_DOMAIN domain, String var, Int16 bitMask, int doset)
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    int nResult = 0;
        //    String strLog;
        //    UInt16 oldValue = 0;
        //    UInt16 newValue = 0;
        //    UInt16 temp;
        //    try
        //    {

        //        nResult = RemoteGetVarByName(domain, var, out oldValue);
        //        if (nResult > 0)
        //        {
        //            newValue = oldValue;
        //            temp = (UInt16)(oldValue & bitMask);

        //            strLog = String.Format("RemoteSetVarBitByName 변수({0}) 현재 값={1}",var,oldValue);
        //            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


        //            if (doset == 1 && temp == 0) // 
        //            {
        //                newValue = (UInt16)(oldValue | (UInt16)bitMask);
        //            }
        //            else if(doset == 0 && temp != 0)
        //            {
        //                newValue = (UInt16)(oldValue & (UInt16)~bitMask);
        //            }
        //            else
        //            {
        //                strLog = String.Format("RemoteSetVarBitByName 변수={0} 는 서로 같은 상태이므로 값 변화 없음 ", var);
        //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
        //            }
        //            if(newValue!=oldValue)
        //            {
        //                strLog = String.Format("RemoteSetVarBitByName 변수={0} 는 다른 상태이므로 값 변화 있음({1}로 세팅) ", var, newValue);
        //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
        //                nResult = RemoteSetVarByName(domain, var, newValue);
        //                //if (remoteSetVarByName(domain, var, newval) < 0)
        //                //{
        //                //    printf("Could not write the %s variable.\n", var);
        //                //    return FALSE;
        //                //}
        //            }
                            
        //        }
        //        else
        //        {
        //            strLog = String.Format("RemoteGetVarByName return 0 var={0}", var);
        //            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
        //    }
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        //    return nResult;
        // }


        public int RemoteGetVarByName(ENV_DOMAIN domain, String var)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {

                byte[] sendData = new byte[4];
                sendData[0] = PacketDefine.MSG_GET_VARBYNAME;
                sendData[1] = (byte)domain;

                byte[] varName = Utility.StringToByte(var);

                if(varName.Length >=2)
                {
                    sendData[2] = varName[0];
                    sendData[3] = varName[1];

                    int msgSize = 0;
                    byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                    nResult = SendToRadar(sendMsg, msgSize);
                    if(nResult > 0)
                    {
                        Object[] param = new Object[1];
                        param[0] = var;
                        RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_GET_VARBYNAME, param, null, null);
                        AddRequestInfo(requestInfo);
                    }

                    //nResult = tcpRadar.SendAndReceive(sendMsg, msgSize, ref receiveData, out receiveSize, PacketDefine.TIME_OUT, PacketDefine.RETRY_COUNT, null);
                    //if (nResult > 0)
                    //{
                    //    strLog = String.Format("receive {0} bytes from radar", receiveSize);
                    //    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                    //    //if (_prevRadarPacket == null)
                    //    _prevRadarPacket = new RadarPacket();
                    //    _prevRadarPacket.SetRadarPacket(receiveData, receiveSize);
                    //    if (_prevRadarPacket.bCompleted)
                    //    {
                    //        strLog = String.Format("RemoteGetVarByName 전송 및 데이터 수신(Size={0}) 완료, datasize={1}", receiveSize, _prevRadarPacket.dataSize);
                    //        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //        strLog = String.Empty;

                    //        for(int i = 0;i<_prevRadarPacket.dataSize;i++)
                    //        {
                    //            strLog += String.Format(" {0:X2}", _prevRadarPacket.data[i]);
                    //        }

                    //        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //        if (_prevRadarPacket.data[0] == PacketDefine.MSG_VAR)
                    //        {
                    //            byte[] _varName = new byte[2];
                    //            Array.Copy(_prevRadarPacket.data, 2, _varName, 0,2);
                    //            if(var.CompareTo(Utility.ByteToString(_varName)) == 0)
                    //            {
                    //                pval = (ushort) (_prevRadarPacket.data[4] + (_prevRadarPacket.data[5] << 8));
                    //                strLog = String.Format("RemoteGetVarByName 변수명={0} 및 값={1}", var, pval);
                    //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //            }
                    //            else
                    //            {
                    //                strLog = String.Format("RemoteGetVarByName 변수명 불일치 {0} != {1}", var, Utility.ByteToString(_varName));
                    //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    //                nResult = 0;
                    //            }
                    //        }
                    //        else
                    //        {

                    //            strLog = String.Format("RemoteGetVarByName 메시지 아이디 불일치,_prevRadarPacket.data[0]={0}", _prevRadarPacket.data[0]);
                    //            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                    //            nResult = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        strLog = String.Format("RemoteGetVarByName 전송 및 데이터 수신(Size={0}) 및 미완료", receiveSize);
                    //        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //    }
                    //}
                    //else
                    //{
                    //    strLog = String.Format("SendAndReceive 리턴 {0}", nResult);
                    //    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    //}
                }

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int RemoteSetVarByName(ENV_DOMAIN domain, String var, UInt16 pval)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            String strLog;
            try
            {
                byte[] sendData = new byte[6];
                sendData[0] = PacketDefine.MSG_SET_VARBYNAME;
                sendData[1] = (byte)domain;

                byte[] varName = Utility.StringToByte(var);

                if (varName.Length >= 2)
                {
                    sendData[2] = varName[0];
                    sendData[3] = varName[1];

                    byte[] newVal = BitConverter.GetBytes(pval);
                    if (newVal.Length >= 2)
                    {
                        sendData[4] = newVal[0];
                        sendData[5] = newVal[1];

                        int msgSize = 0;
                        byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                        nResult = SendToRadar(sendMsg, msgSize);
                        if (nResult > 0)
                        {
                            Object[] param = new Object[2];
                            param[0] = var;
                            param[1] = pval;
                            RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_SET_VARBYNAME, param, null, null);
                            AddRequestInfo(requestInfo);
                        }
                    }
                    else
                    {
                        strLog = String.Format("BitConverter.GetBytes size error");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int GetRadarTime(ref DateTime date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                nResult = GetSysInfo(null);
                if(nResult>0)
                {
                    //TODO 레이더 시간 가져오는 부분 작업 필요
                    date = _radarDateTime;
                    nResult = 1;
                }
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SetRadarTime(Object callbackFunc, Object workData, DateTime? date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if (date == null)
                return 0;
            try
            {
                byte[] sendData = new byte[8];

                sendData[0] = PacketDefine.MSG_TIME_SET;
                sendData[1] = 0; // dummy
                sendData[2] = (byte)date?.Second;
                sendData[3] = (byte)date?.Minute;
                sendData[4] = (byte)date?.Hour;
                sendData[5] = (byte)date?.Day;
                sendData[6] = (byte)(date?.Month - 1);
                sendData[7] = (byte)(date?.Year - 1900);



                int msgSize = 0;
                byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                nResult = SendToRadar(sendMsg, msgSize);
                if (nResult > 0)
                {
                    Object[] param = new Object[1];
                    param[0] = date;

                    Object[] callbackParam = new object[2];
                    callbackParam[0] = workData;

                    RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_TIME_SET, param, callbackFunc, callbackParam);
                    AddRequestInfo(requestInfo);
                }

    
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
        public int SetRadarTime(DateTime? date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if (date == null)
                return 0;
            try
            {
                byte[] sendData = new byte[8];

                sendData[0] = PacketDefine.MSG_TIME_SET;
                sendData[1] = 0; // dummy
                sendData[2] = (byte) date?.Second;
                sendData[3] = (byte)date?.Minute;
                sendData[4] = (byte)date?.Hour;
                sendData[5] = (byte)date?.Day;
                sendData[6] = (byte)(date?.Month - 1) ;
                sendData[7] = (byte)(date?.Year - 1900);



                int msgSize = 0;
                byte[] sendMsg = MakeRadarPacket(sendData, sendData.Length, out msgSize);
                nResult = SendToRadar(sendMsg, msgSize);
                if (nResult > 0)
                {
                    Object[] param = new Object[1];
                    param[0] = date;
                    RadarRequest requestInfo = new RadarRequest(PacketDefine.MSG_TIME_SET, param, null, null);
                    AddRequestInfo(requestInfo);
                }

                //nResult = tcpRadar.SendAndReceive(sendMsg, msgSize, ref receiveData, out receiveSize, PacketDefine.TIME_OUT, PacketDefine.RETRY_COUNT, null);
                //if (nResult > 0)
                //{
                //    strLog = String.Format("receive {0} bytes from radar", receiveSize);
                //    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                //    _prevRadarPacket = new RadarPacket();
                //    _prevRadarPacket.SetRadarPacket(receiveData, receiveSize);
                //    if (_prevRadarPacket.bCompleted)
                //    {
                //        strLog = String.Format("SetRadarTime 전송 및 데이터 수신(Size={0}) 완료, datasize={1}", receiveSize, _prevRadarPacket.dataSize);
                //        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                //        //strLog = String.Empty;

                //        //for (int i = 0; i < _prevRadarPacket.dataSize; i++)
                //        //{
                //        //    strLog += String.Format(" {0:X2}", _prevRadarPacket.data[i]);
                //        //}

                //        //AddLog(LOG_TYPE.LOG_INFO, strLog);

                //        if (_prevRadarPacket.data[0] == PacketDefine.MSG_STATUS)
                //        {
                //            Int16 status = Convert.ToInt16((_prevRadarPacket.data[2] << 8) + _prevRadarPacket.data[1]);
                //            if (status == 0)
                //            {
                //                strLog = String.Format("SetRadarTime 시간 설정 OK status={0}", status);
                //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //                nResult = 1;

                //            }
                //            else
                //            {
                //                strLog = String.Format("SetRadarTime 시간 설정 실패 status={0}", status);
                //                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //                nResult = 0;
                //            }
                //        }
                //        else
                //        {

                //            strLog = String.Format("SetRadarTime 메시지 아이디 불일치,_prevRadarPacket.data[0]={0}", _prevRadarPacket.data[0]);
                //            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //            nResult = 0;
                //        }
                //    }
                //    else
                //    {
                //        strLog = String.Format("SetRadarTime 전송 및 데이터 수신(Size={0}) 및 미완료", receiveSize);
                //        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //    }
                //}
                //else
                //{
                //    strLog = String.Format("SendAndReceive 리턴 {0}", nResult);
                //    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //}
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int CheckVDSStatus(ref byte[] status, ref byte[] checkTime)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            DateTime date;
            try
            {
                date = DateTime.Now;
                checkTime = Utility.DateToByte(date);
                //TODO 장비 상태 체크 모듈...작성 필요
                var d = Utility.ByteToDate(checkTime);
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessReceivePacket(byte[] data, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            int i = 0;
            String strLog;
            try
            {
                strLog = String.Format($"수신 레이더 데이터 크기:{length}");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                //if (_initialTargetStreaming && length > 60)
                //    Console.WriteLine("break...");
                while(i<length)
                {
                    if (_prevRadarPacket == null)
                    {
                        _prevRadarPacket = new RadarPacket();
                    }
                    i = _prevRadarPacket.SetRadarPacket(data,i, length);
                    if(_prevRadarPacket.bCompleted)
                    {
                        strLog = String.Format($"레이더 패킷 완성");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                        ProcessRadarPacket(_prevRadarPacket);
                        _prevRadarPacket = null;
                    }
                    else if(_prevRadarPacket.state != RECEIVER_STATE.S_FAIL)
                    {
                        strLog = String.Format($"레이더 패킷 미완성. ");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    }
                    else if (_prevRadarPacket.state == RECEIVER_STATE.S_FAIL)
                    {
                        strLog = String.Format($"패킷 오류");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        _prevRadarPacket = null;
                    }
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessGetSysInfo(RadarPacket packet, RadarRequest requestInfo)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            
            try
            {
                /*
                    REV=2.0.137
                    ID=00001b001e2b
                    OPT=0
                    TIME=Thu May 14 23:34:29 2020
                    UPTIME=  6 days 11:47:21
                    TYPE=SPEEDLANEPROB05
                    MOD=0
                    CAP=0xffab92f5
                    ENV=236
                    RNGMAX=384
                    RNGRES=281
                    MPH16=253
                    ERRNO=0
                    CYCLE=2000
                    MK=d84547d8a23b
                    SDSZ=30528
                 */
                
                byte[] sysinfo = new byte[packet.dataSize - 3];
                Array.Copy(packet.data, 1, sysinfo, 0, packet.dataSize - 3);
                String strSysinfo = Utility.ByteToString(sysinfo);
                var propertyList = strSysinfo.Split('\n');

                if (SetRadarProperty(propertyList) > 0)
                {
                    String strDateTime = GetRadarProperty("TIME");
                    _radarDateTime = DateTime.ParseExact(strDateTime, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);

                    strLog = String.Format("SetRadarProperty TIME={0}", _radarDateTime.ToString());
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    if(requestInfo!=null && requestInfo._callbackFunc!=null)
                    {
                        if(requestInfo._callbackFunc is ProcessRadarCallbackDelegate)
                        {
                            ProcessRadarCallbackDelegate processRadarCallback = (ProcessRadarCallbackDelegate)requestInfo._callbackFunc;

                            if(requestInfo._callbackParams.Length ==2) //0: workdata 1: radar time
                            {
                                strLog = String.Format("레이더 시간 정보 콜백함수로 호출 TIME={0}", _radarDateTime.ToString());
                                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                                requestInfo._callbackParams[1] = _radarDateTime;
                                processRadarCallback(requestInfo._callbackParams);
                            }
                            else
                            {
                                strLog = String.Format("콜백 함수로 보낼 파라메터 갯수 오류. 파라메터 갯수={0}", requestInfo._callbackParams.Length);
                                Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);

                            }





                        }
                    }
                }
                else
                {
                    strLog = String.Format("SetRadarProperty 처리 오류");
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int ProcessTargetSummaryInfo(RadarPacket packet)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if (packet.dataSize - 2 - 1 == PacketDefine.TARGET_SUMMARY_INFO_SIZE) // 
                {
                    int idx = 1;
                    TargetSummaryInfo target = new TargetSummaryInfo();
                    target.ID_0 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);
                    idx += 2;
                    target.ID_1 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);

                    idx += 2;
                    target.START_CYCLE_0 = ((packet.data[idx + 3] << 24) + (packet.data[idx + 2] << 16) + (packet.data[idx + 1] << 8) + (packet.data[idx]));
                    idx += 4;

                    target.START_CYCLE_1 = ((packet.data[idx + 3] << 24) + (packet.data[idx + 2] << 16) + (packet.data[idx + 1] << 8) + (packet.data[idx]));
                    idx += 4;

                    target.AGE_0 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);
                    idx += 2;

                    target.AGE_1 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);
                    idx += 2;

                    target.MAG_MAX_0 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);
                    idx += 2;

                    target.MAG_MAX_1 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]);
                    idx += 2;

                    target.LANE = packet.data[idx];
                    idx++;

                    target.TRAVEL_DIRECTION = packet.data[idx];
                    idx++;


                    target.LENGTH_X100 = Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx]); // cm
                    idx += 2;


                    target.SPEED_X100 = (Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx])) / 100f; // km/h
                    idx += 2;

                    target.RANGE_X100 = (Convert.ToInt16((packet.data[idx + 1] << 8) + packet.data[idx])) / 100f; // m 
                    idx += 2;

                    strLog = String.Format("target.ID_0={0}, target.ID_1={1}, target.START_CYCLE_0={2}, target.START_CYCLE_1={2},target.AGE_0={4}, target.AGE_1={5}, target.MAG_MAX_0={6}, target.MAG_MAX_1={7}, target.LANE={8},target.TRAVEL_DIRECTION={9}, target.LENGTH_X100={10}, target.SPEED_X100={11}, target.RANGE_X100={12}", target.ID_0, target.ID_1, target.START_CYCLE_0, target.START_CYCLE_1,
                                                                                 target.AGE_0, target.AGE_1, target.MAG_MAX_0, target.MAG_MAX_1, target.LANE,
                                                                                 target.TRAVEL_DIRECTION, target.LENGTH_X100, target.SPEED_X100, target.RANGE_X100);

                    target.OCCUPY_TIME = Utility.GetOccupyTime(target.SPEED_X100);


                    target.CREATE_DATE = DateTime.Now;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //AddRadarData(target);


                    nResult = 1;
                }
                else
                {
                    strLog = String.Format("TargetSumamryInfo 데이터 사이즈 불일치 targetPacket.dataSize={0}", packet.dataSize);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
                
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessMsgStatus(RadarPacket packet ,RadarRequest request)
        {
            // StartLiveCamera, StopLiveCamera, RemoteSetVarByName, SetRadarTime

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                
                Int16 status = Convert.ToInt16((packet.data[2] << 8) + packet.data[1]);
                if (status == 0)
                {
                    strLog = String.Format("레이더 MsgStatus OK status={0}", status);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
                else
                {
                    strLog = String.Format("레이더 MsgStatus Fail status={0}", status);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
                //TODO 어떤 요청에 대한 응답값인지 확인 하는 과정 필요
                if(request!=null)
                {
                    switch(request._requestType)
                    {
                        case PacketDefine.MSG_CAMERA_CTL:
                            nResult = ProcessRadarCameraCtrl(request, status);
                            break;
                        case PacketDefine.MSG_SET_VARBYNAME: // RemoteSetVarByName
                            nResult = ProcessSetVarbyName(request, status);
                            break;
                        case PacketDefine.MSG_TIME_SET: //SetRadarTime
                            nResult = ProcessSetRadarTime(request, status);
                            break;
                        default:
                            Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"정의되지 않은 요청 유형={request._requestType}"));
                            break;
                    }
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"RequestQueue 리턴 널. 큐사이즈={_requestQueue.Count}"));
                }
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int ProcessGetVar(RadarPacket packet, RadarRequest request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                byte[] _varName = new byte[2];
                Array.Copy(packet.data, 2, _varName, 0, 2);
                if(request!=null)
                {
                    String var = request._parameter[0].ToString();
                    if (var.CompareTo(Utility.ByteToString(_varName)) == 0)
                    {
                        UInt16 pval = (ushort)(packet.data[4] + (packet.data[5] << 8));
                        if (!_radarVarValueList.ContainsKey(var))
                            _radarVarValueList.Add(var, pval);
                        else
                            _radarVarValueList[var] = pval;

                        strLog = String.Format("RemoteGetVarByName 변수명={0} 및 값={1}, _initialTargetStreaming={2}", var, pval, _initialTargetStreaming);
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        

                        // 최초
                        if (_initialTargetStreaming == false) 
                        {
                            STREAM_CONFIG config = speedLaneStreamConfig[(int)STREAM_TYPE_ENUM.STREAM_TARGETS_SUMM];
                            if(config.var.CompareTo(var) == 0)
                            {
                                SetTargetSummaryStreaming((byte)ENV_BOOL.TRUE);
                                _initialTargetStreaming = true;
                            }
                        }
                    }
                    else
                    {
                        strLog = String.Format("RemoteGetVarByName 변수명 불일치 {0} != {1}", var, Utility.ByteToString(_varName));
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                        nResult = 0;
                    }
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"RequestQueue 리턴 널. 큐사이즈={_requestQueue.Count}"));
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessRadarCameraCtrl(RadarRequest request, Int16 status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if (request._parameter.Length != 1)
                {
                    strLog = String.Format($"요청 파라메터 정보 누락. request._parameter.Length={request._parameter.Length} Status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    return 0;
                }

                if (status >=0)
                {
                    switch(request._parameter[0])
                    {
                        case PacketDefine.CAMERA_CTL_START:
                            strLog = String.Format($"카메라 전원 ON 요청 성공 Status={status}");
                            break;
                        case PacketDefine.CAMERA_CTL_STOP:
                            strLog = String.Format($"카메라 전원 OFF 요청 성공 Status={status}");
                            break;
                        default:
                            strLog = String.Format($"알수 없는 카메라 카메라 요청 정보 Status={status}");
                            break;
                    }
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
                else
                {
                    switch (request._parameter[0])
                    {
                        case PacketDefine.CAMERA_CTL_START:
                            strLog = String.Format($"카메라 전원 ON 요청 실패 Status={status}");
                            break;
                        case PacketDefine.CAMERA_CTL_STOP:
                            strLog = String.Format($"카메라 전원 OFF 요청 실패 Status={status}");
                            break;
                        default:
                            strLog = String.Format($"알수 없는 카메라 카메라 요청 정보 Status={status}");
                            break;
                    }
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);

                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessSetVarbyName(RadarRequest request, Int16 status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if (request._parameter.Length != 2)
                {
                    strLog = String.Format($"요청 파라메터 정보 누락. request._parameter.Length={request._parameter.Length} Status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    return 0;
                }

                String var = request._parameter[0].ToString();
                UInt16 value = UInt16.Parse(request._parameter[1].ToString());


                if (status >= 0)
                {
                    _radarVarValueList[var] = value;
                    strLog = String.Format($"RemoteSetVarByName 변수 세팅 ({var}={value}) 성공 status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                }
                else
                {
                    strLog = String.Format($"RemoteSetVarByName 변수 세팅 {var}={value}) 실패 status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);

                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int ProcessSetRadarTime(RadarRequest request, Int16 status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                if (request._parameter.Length != 1)
                {
                    strLog = String.Format($"요청 파라메터 정보 누락. request._parameter.Length={request._parameter.Length} Status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    return 0;
                }

                DateTime date = (DateTime)request._parameter[0];
                if (status >= 0)
                {
                    
                    strLog = String.Format($"레이더 시간 설정({date.ToString(VDSConfig.RADAR_TIME_FORMAT)})  성공 status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    if (request._callbackFunc != null) // callback 함수가 있을 경우 추가 요청
                    {
                        //GetSys를 호출 하여 현재 설정된 시간 정보 알아온다.
                        nResult = GetSysInfo(request);
                        //RadarRequest req = new RadarRequest(PacketDefine.MSG_SYSINFO, null, request._callbackFunc, request._callbackParams);
                    }
                }
                else
                {
                    strLog = String.Format($"레이더 시간 설정({date.ToString(VDSConfig.RADAR_TIME_FORMAT)})  실패 status={status}");
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);

                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
        
    }
}
