using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class VDSClient
    {
        public String _address { get; set; }
        public int _port { get; set; }

        /// <summary>
        /// 재시도 횟수 (-1 무한대)
        /// </summary>
        public int _retryCount { get; set; }

        /// <summary>
        /// 재접속 시도 간격(초)
        /// </summary>
        public double _retryInterval { get; set; }
        public DateTime _lastConnectTime { get; set; }


        public bool _bChecking { get; set; }
        public SOCKET_STATUS _status { get; set; }
        public SessionContext _sessionContext = null;
        //public Socket _clientSocket = null;

        public String heartBeatTime { get; set; }
        //public DateTime? lastHeartBeatTime;


        public AsyncCallback _connectCallback = null;
        public AsyncCallback _receiveCallback = null;
        public AsyncCallback _sendCallback = null;

        public ConnectCallback _timedConnectCallback = null;
        public ManualResetEvent checkExitEvent = new ManualResetEvent(false);


        public VDSClient()
        {
            _retryCount = -1;
            _retryInterval = VDSConfig.RECONNECT_INTERVAL; // 10초마다 재접속 시도
            _status = SOCKET_STATUS.DISCONNECTED;
            _bChecking = false;
            _sessionContext = new SessionContext();
            checkExitEvent.Reset();

        }

        public int SetRetryCount(int count)
        {
            _retryCount = count; 
            return _retryCount;
        }

        public double SetRetryInterval(double time)
        {
            _retryInterval = time;
            return _retryInterval;
        }

        public int SetAddress(String address, int port, CLIENT_TYPE clientType, ConnectCallback connectCallback, AsyncCallback receiveCallback, AsyncCallback sendCallback)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            _address = address;
            _port = port;
            _sessionContext._type = clientType;
            _timedConnectCallback = connectCallback;
            _receiveCallback = receiveCallback;
            _sendCallback = sendCallback;

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"아이피:{address} 포트:{port} Client_TYPE:{clientType} 설정"));
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int Connect(String address, int port, int timeout)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(_address);// ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                _sessionContext._socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);


                if (Utility.Connect(ref _sessionContext._socket, address, port, timeout))
                {
                    _status = SOCKET_STATUS.CONNECTED;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, $"접속 {_address}:{_port} 성공");

                }
                else
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, $"접속 {_address}:{_port} 실패 . connect time out({timeout}초)");

                }
                if (_timedConnectCallback != null)
                    _timedConnectCallback(_sessionContext, _status);

                // 최종 접속 시도 시간 저장
                _lastConnectTime = DateTime.Now;
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


        public int Connect(String address, int port)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(_address);// ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                
                // Create a TCP/IP socket.  
                _sessionContext._socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // 최종 접속 시도 시간 저장
                _lastConnectTime = DateTime.Now;

                // Connect to the remote endpoint.  
                _status = SOCKET_STATUS.CONNECTING;

                if (_connectCallback!=null)
                {
                    _sessionContext._socket.BeginConnect(remoteEP,
                        new AsyncCallback(_connectCallback), _sessionContext._socket);
                    nResult = 1;
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

        public void Send(byte[] byteData)
        {
            try
            {

                // Convert the string data to byte data using ASCII encoding.  
                //byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.  
                _sessionContext._socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(_sendCallback), _sessionContext._socket);
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
        }


        public int StartConnect()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            
            int nResult = Connect(_address, _port,5);
            if(nResult>0)
            {
               StartCheckConnection();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int StartCheckConnection()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            new Thread(() =>
            {
                DateTime nextTryTime ;
                try
                {
                    _bChecking = true;
                    while (_bChecking)
                    {
                        try
                        {
                            nextTryTime = _lastConnectTime.AddSeconds(_retryInterval);
                            switch (_status)
                            {
                                case SOCKET_STATUS.DISCONNECTED:
                                    //Console.WriteLine("_lastConnectTime={0},nextTryTime={1} ", _lastConnectTime, nextTryTime);
                                    // retry connection
                                    if (nextTryTime < DateTime.Now)
                                    {

                                        if (_retryCount == -1 || _retryCount > 0) // -1 인경우 무제한으로 재접속 시도
                                        {
                                            Utility.AddLog(LOG_TYPE.LOG_INFO, $"재접속 시도수={_retryCount} (-1: 무제한 재접속) {_address}:{_port} 접속 ");
                                            // 하나 줄인다.
                                            if (_retryCount > 0)
                                                _retryCount--;
                                            Connect(_address, _port, 5);
                                        }
                                    }
                                    break;

                                case SOCKET_STATUS.CONNECTING:
                                    // no action
                                    Console.WriteLine($"SOCKET_STATUS.CONNECTING...{_address}:{_port}");
                                    break;

                                case SOCKET_STATUS.CONNECTED:
                                case SOCKET_STATUS.AUTHORIZED:
                                case SOCKET_STATUS.UNAUTHORIZE:


                                    //if (!Utility.IsConnected(_sessionContext._socket)) // not connected 
                                    if (!CheckConnection()) // not connected 
                                    {
                                        Utility.AddLog(LOG_TYPE.LOG_INFO, $"{_address}:{_port} 접속 종료");
                                        _status = SOCKET_STATUS.DISCONNECTED;
                                    }
                                    break;
                            }
                            Thread.Sleep(1000);
                        }
                        catch(Exception ex)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                            _status = SOCKET_STATUS.DISCONNECTED;
                        }
                    }
                    checkExitEvent.Set();
                }
                catch (Exception ex)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                }
            }
           ).Start();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public bool CheckConnection()
        {
            bool bResult = true;
            if(!String.IsNullOrEmpty(heartBeatTime))
            {
                DateTime heartBeatTime = DateTime.ParseExact(this.heartBeatTime, VDSConfig.RADAR_TIME_FORMAT, null);
                TimeSpan timeSpan = DateTime.Now - heartBeatTime;

                //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"CheckConnection..HeartBeatTime={this.heartBeatTime} {timeSpan.TotalSeconds} seconds before "));


                if (timeSpan.TotalSeconds > 60)
                    bResult = false;
            }
            else
            {
                bResult = Utility.IsConnected(_sessionContext._socket);
            }
            return bResult;
        }

        public int StopCheckConnection()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            if (_bChecking)
            {
                _bChecking = false;
                checkExitEvent.WaitOne();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int Stop()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                StopCheckConnection();
                if(_sessionContext!=null && _sessionContext._socket != null)
                {
                    _sessionContext._socket.Shutdown(SocketShutdown.Both);
                    _sessionContext._socket.Close();
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
            }  
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;

        }
    }
}
