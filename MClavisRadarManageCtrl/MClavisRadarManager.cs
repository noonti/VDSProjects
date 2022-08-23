using MClavisRadarManageCtrl.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;

namespace MClavisRadarManageCtrl
{
    
    public class MClavisRadarManager  : IVDSManager, IVDSDevice
    {
        private IPEndPoint remoteEP;
        //private IPEndPoint localEP;
        private UdpClient mclavisClient;

        ArrayList socketList = new ArrayList();
        public ManualResetEvent socketMsgThreadExitEvent = new ManualResetEvent(false);
        bool _bSocketMsgProcessing = false;


        Control _control = null;
        FormAddUDPData _formAddUDPData = null;

        public AddTrafficDataEvent _addTrafficDataEvent = null;
        MClavisDataFrame  _prevDataFrame = null;

        MCLAVIS_MESSAGE[,] _lastMClavisMessage = new MCLAVIS_MESSAGE[2,16]; // 편도 16차선까지 ...

        public MClavisRadarManager()
        {
            socketMsgThreadExitEvent.Reset();
            for (int i = 0; i < 2; i++)
                for(int j =0;j<16;j++)
                    _lastMClavisMessage[i,j] = new MCLAVIS_MESSAGE();
        }


        public int AddTrafficDataEvent(TrafficDataEvent trafficDataEvent)
        {
            int nResult = 0;
            String strLog;
            if (_addTrafficDataEvent != null && trafficDataEvent != null)
            {
                trafficDataEvent.vds_type = VDSConfig.GetVDSTypeName(); ;
                nResult = _addTrafficDataEvent(trafficDataEvent);
            }
            return nResult;
        }



        public int CheckVDSStatus(ref byte[] status, ref byte[] checkTime)
        {
            throw new NotImplementedException();
        }

        public bool isService()
        {
            return _bSocketMsgProcessing;
        }

        public int SetAddTrafficDataEventDelegate(AddTrafficDataEvent addTrafficDataEvent)
        {
            _addTrafficDataEvent = addTrafficDataEvent;
            return 1;
        }

        public int SetDeviceTime(object callbackFunc, object workData, DateTime? date)
        {
            throw new NotImplementedException();
        }

        public int StartDevice(string address, int port, int localPort = 0) // server address, port (udp)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (mclavisClient != null)
            {
                mclavisClient.Close();
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"mclavis remote address={address} port={port} local port={localPort}"));
            socketList.Clear();

            mclavisClient = new UdpClient(localPort);
            mclavisClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            remoteEP = new IPEndPoint(IPAddress.Parse(address), port);
            socketList.Add(mclavisClient.Client);
            StartWorkThread();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));


            return 1;
            
        }

        

        public int StopDevice()
        {
            StopWorkThread();
            return 1;
        }

        public int StartManager()
        {
            return 1;
        }

        public int StopManager()
        {
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

        public int SetFormAddTargetInfoDelegate(Control control, FormAddUDPData formAddUDPData)
        {
            _formAddUDPData = formAddUDPData;
            if (_control == null)
                _control = control;
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

                        ArrayList checkList = new ArrayList(socketList);
                        Socket.Select(checkList, null, null, 10);
                        foreach (var sock in checkList)
                        {
                            if(sock == mclavisClient.Client)
                            {
                                byte[] packet = mclavisClient.Receive(ref remoteEP);
                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" received packet= {Utility.PrintHexaString(packet, packet.Length)}"));
                                ProcessReceivePacket(packet);
                            }

                        }
                    }
                    socketMsgThreadExitEvent.Set();
                }
                catch (Exception ex)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                }
                finally
                {
                    mclavisClient.Close();
                    mclavisClient = null;
                }
            }
          ).Start();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        public int ProcessReceivePacket(byte[] packet)
        {
           // Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            int i = 0;
            try
            {
                while (i < packet.Length)
                {
                    if (_prevDataFrame == null)
                    {
                        _prevDataFrame = new MClavisDataFrame();
                    }
                    else
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"미완성 패킷 후속 처리 "));
                    }

                    i = _prevDataFrame.Deserialize(packet, i);
                    ProcessDataFrame(_prevDataFrame);
                    _prevDataFrame = null;
                }

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                nResult = 0;
            }
            
            // Not all data received. Get more.  
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public int ProcessDataFrame(MClavisDataFrame dataFrame)
        {
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            foreach(var message in dataFrame.messageList)
            {
                if(message.object_id > 0 && message.msgType == MCLAVIS_MESSAGE_TYPE.OBJECT_DATA &&  (message.State == 6 || message.State == 9)
                    && IsExistMessage(message)==false
                    ) // state 4 , 5  또는 4,6 인것 항상 존재한다. 차량 속도는 State 4 인것으로,,,차량 길이는 State 6
                {

                    var trafficData = GetTrafficData(message);
                    AddTrafficDataEvent(trafficData);
                    //String data = dataFrame.GetMClavisMessageInfo(message);
                    //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"message = {data}"));
                    //if (_control != null && _formAddUDPData != null)
                    //    _control.BeginInvoke(_formAddUDPData, new object[] { data });
                    _lastMClavisMessage[message.Lane_Dir, message.Lane - 1] = message;
                }
                
            }

            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public bool IsExistMessage(MCLAVIS_MESSAGE message)
        {
            bool bResult = false;
            int direction = message.Lane_Dir == 0 ? 0 : 1; // 0: 상행(TO RIGHT) 1: 하행(TO LEFT)
            int Lane = message.Lane - 1;

            if (_lastMClavisMessage[direction, Lane].object_id == message.object_id && _lastMClavisMessage[direction, Lane].State == message.State)
                bResult = true;
            
            return bResult;
        }

        public TrafficDataEvent GetTrafficData(MCLAVIS_MESSAGE message)
        {
          
            TrafficDataEvent result = new TrafficDataEvent();
            result.id = DateTime.Now.ToString("yyMMddHHmmss_") + Guid.NewGuid().ToString();
            result.detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            result.direction = (byte)(message.Lane_Dir == 0 ? (int)MOVE_DIRECTION.TO_RIGHT : (int)MOVE_DIRECTION.TO_LEFT); // 1: 상행 2: 하행
            result.lane = message.Lane;
            result.length = (int)(message.Range_X * 100);
            result.speed = Math.Abs(message.Velocity_X);
            result.vds_type = VDSConfig.GetVDSTypeName();
            result.occupyTime = Utility.GetOccupyTime(Math.Abs(message.Velocity_X), message.Range_X , VDSConfig.controllerConfig.CheckDistance); ;
            result.reverseRunYN = message.State == 9?"Y":"N";
            
            return result;
        }
    }
}
