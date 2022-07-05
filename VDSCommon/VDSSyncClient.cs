using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VDSCommon
{
    public class VDSSyncClient
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


        public VDSSyncClient()
        {
            _retryCount = -1;
            _retryInterval = VDSConfig.RECONNECT_INTERVAL; // 10초마다 재접속 시도
            _status = SOCKET_STATUS.DISCONNECTED;
            _bChecking = false;
            _sessionContext = new SessionContext();
        }

        public int Connect(String address, int port, int timeout)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                _address = address;
                _port = port;
                IPAddress ipAddress = IPAddress.Parse(_address);// ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);

                // Create a TCP/IP socket.  
                _sessionContext._socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);


                if (Utility.Connect(ref _sessionContext._socket, address, port, timeout))
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, $"접속 {_address}:{_port} 성공");
                    _status = SOCKET_STATUS.CONNECTED;
                }
                else
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
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


        public int ReceivePacket(ref byte[] receiveData, out int receiveSize, double timeout)
        {
            int nResult = 0;
            receiveSize = 0;
            try
            {
                ArrayList socketList = new ArrayList();
                socketList.Add(_sessionContext._socket);
                Socket.Select(socketList, null, null, (int)(timeout * 1000000));
                foreach (Socket socket in socketList)
                {
                    if (socket == _sessionContext._socket)
                    {
                        receiveSize = _sessionContext._socket.Receive(receiveData);
                        nResult = receiveSize;
                        if (receiveSize > 0)
                            break;
                    }
                }
            }
            catch (SocketException ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

                if (ex.SocketErrorCode == SocketError.ConnectionRefused ||
                    ex.SocketErrorCode == SocketError.ConnectionReset ||
                    ex.SocketErrorCode == SocketError.ConnectionAborted
                    )
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
            }
            return nResult;
        }


        public int SendPacket(byte[] sendData, int sendSize)
        {
            int nResult = 0;
            String strLog;
            try
            {

                nResult = _sessionContext._socket.Send(sendData, sendSize, SocketFlags.None);
                strLog = String.Format("SendPacket 함수 sendSize = {0}, nResult = {1}", sendSize, nResult);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            catch (SocketException ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.StackTrace.ToString());
                if (ex.SocketErrorCode == SocketError.ConnectionRefused ||
                    ex.SocketErrorCode == SocketError.ConnectionReset ||
                    ex.SocketErrorCode == SocketError.ConnectionAborted
                    )
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return nResult;
        }

        public int Close()
        {
            int nResult = 0;
            try
            {
                _sessionContext._socket.Shutdown(SocketShutdown.Both);
                _sessionContext._socket.Close();
                _status = SOCKET_STATUS.DISCONNECTED;
            }
            catch (SocketException ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.StackTrace.ToString());
                if (ex.SocketErrorCode == SocketError.ConnectionRefused ||
                    ex.SocketErrorCode == SocketError.ConnectionReset ||
                    ex.SocketErrorCode == SocketError.ConnectionAborted
                    )
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            return nResult;
        }

        public int SendAndReceive(byte[] sendData, int sendSize, ref byte[] receiveData, out int receiveSize, double timeout)
        {
            int nResult = 0;
            receiveSize = 0;
            if (_status == SOCKET_STATUS.DISCONNECTED)
                return 0;
            receiveData = new byte[VDSConfig.PACKET_SIZE];
            try
            {
                nResult = SendPacket(sendData, sendSize);// _socket.Send(sendData, sendSize, SocketFlags.None);
                if (nResult > 0)
                {
                    nResult = ReceivePacket(ref receiveData, out receiveSize, timeout);
                }
                nResult = receiveSize;
            }
            catch (SocketException ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());


                if (ex.SocketErrorCode == SocketError.ConnectionRefused ||
                    ex.SocketErrorCode == SocketError.ConnectionReset ||
                    ex.SocketErrorCode == SocketError.ConnectionAborted
                    )
                {
                    _status = SOCKET_STATUS.DISCONNECTED;
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.StackTrace.ToString());

            }
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
    }
}
