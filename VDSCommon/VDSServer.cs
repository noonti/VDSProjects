using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VDSCommon.DataType;

namespace VDSCommon
{
    
    public class VDSServer : IVDSManager
    {
        public String _address { get; set; }
        public int _port { get; set; }
        public bool _bListening { get; set; }
        public CLIENT_TYPE _clientType { get; set; }
        public ManualResetEvent threadExitEvent = new ManualResetEvent(false);
        public ManualResetEvent acceptProcessEvent = new ManualResetEvent(false);
        public AsyncCallback _acceptCallback = null;


        Socket _serverSocket = null;


        public VDSServer()
        {
            _port = 10000;
            _bListening = false;
            threadExitEvent.Reset();
            acceptProcessEvent.Reset();
        }

        public int SetAddress(String address, int port,CLIENT_TYPE clientType, AsyncCallback acceptCallback)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            _address = address;
            _port = port;
            _acceptCallback = acceptCallback;
            _clientType = clientType;
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"아이피:{address} 포트:{port} Client_TYPE:{clientType} 설정"));

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }


        public int StartManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, string.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            String strLog;
            if (_bListening) // 아직 logging 인경우 stop 시킨다
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"VDSServer 이미 리스닝 중. 중지"));

                StopManager();

            }
            //if(_serverSocket!=null)
            //{
            //    _serverSocket.Disconnect(true);
            //}
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPAddress ipAddress = IPAddress.Parse(_address);// ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);
            // Create a TCP/IP socket.  
            _serverSocket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(localEndPoint);
            _serverSocket.Listen(100);

            strLog = String.Format("{0}:{1} 리스닝 시작", _address, _port);
            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            new Thread(() =>
            {
                try
                {
                    _bListening = true;
                    while (_bListening)
                    {
                        acceptProcessEvent.Reset();
                        if (_acceptCallback != null)
                        {
                            _serverSocket.BeginAccept(
                                new AsyncCallback(_acceptCallback),
                                _serverSocket);
                        }

                        acceptProcessEvent.WaitOne();
                        Thread.Sleep(100);
                    }
                    threadExitEvent.Set();

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

        public int StopManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            try
            {


                if (_bListening)
                {
                    _bListening = false;

                    strLog = String.Format("리스닝 종료 이벤트 SET");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    SetAcceptProcessEvent();
                    threadExitEvent.WaitOne();

                    strLog = String.Format("리스닝 종료 OK");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    if (_serverSocket != null)
                    {
                        //_serverSocket.Shutdown(SocketShutdown.Both);
                        _serverSocket.Close();
                        _serverSocket.Dispose();
                        _acceptCallback = null;
                        _serverSocket = null;

                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"서버 소켓 close 및 dispose"));

                    }
                }
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public void SetAcceptProcessEvent()
        {
            acceptProcessEvent.Set();
        }

        //public int SendTrafficData(TargetSummaryInfo target)
        //{
        //    return 1;
        //}
    }
}
