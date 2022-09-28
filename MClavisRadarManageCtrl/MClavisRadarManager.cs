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
                                //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($" received packet= {Utility.PrintHexaString(packet, packet.Length)}"));
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
                _prevDataFrame = null; // 예외 발생 시 다음 패킷부터는 정상 처리 위해 null 설정
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
                // state 4 , 5  또는 4,6 인것 항상 존재한다. 차량 속도는 State 4 인것으로,,,차량 길이는 State 6
                if (message.object_id > 0 && message.msgType == MCLAVIS_MESSAGE_TYPE.OBJECT_DATA && IsExistMessage(message) == false
                    && (message.State == 6 || message.State== 7 || message.State==9 || message.State==10)
                    )
                 {
                    // state : 6 -->  속도, 길이 산출
                    // state : id:0 , 7 --> 정체 상태(30초), 8: 무차량 상태(30초), lane 값 유효
                    // state : 9 --> 역주행, lane 값 0으로 고정 
                    // state : 10 -->차량 정지. lane 값 0 으로 고정 
                    var trafficData = GetTrafficData(message);
                    AddTrafficDataEvent(trafficData);
                    _lastMClavisMessage[message.Lane_Dir, message.Lane] = message;
                }
            }
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public bool IsExistMessage(MCLAVIS_MESSAGE message)
        {
            bool bResult = false;
            int direction = message.Lane_Dir == 0 ? 0 : 1; // 0: 상행(TO RIGHT) 1: 하행(TO LEFT)
            //int Lane = message.Lane - 1;
            int Lane = message.Lane;
            if (Lane >=0 && Lane <=15)
            {
                if (_lastMClavisMessage[direction, Lane].object_id == message.object_id && _lastMClavisMessage[direction, Lane].State == message.State)
                    bResult = true;
            }
            else
            {
                Console.WriteLine($"direction={direction} Lane={Lane} ");
            }

            return bResult;
        }

        public TrafficDataEvent GetTrafficData(MCLAVIS_MESSAGE message)
        {
          
            TrafficDataEvent result = new TrafficDataEvent();
            result.id = DateTime.Now.ToString("yyMMddHHmmss_") + Guid.NewGuid().ToString();
            result.detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            result.direction = (byte)(message.Lane_Dir == 0 ? (int)MOVE_DIRECTION.TO_RIGHT : (int)MOVE_DIRECTION.TO_LEFT); // 1: 상행 2: 하행
            result.lane = message.Lane;
            result.length = (int)(message.Range_X * 100); // m 단위--> cm 단위로 변환
            result.speed = Math.Abs(message.Velocity_X);  // km/h
            result.vds_type = VDSConfig.GetVDSTypeName();
            result.occupyTime = Utility.GetOccupyTime(Math.Abs(message.Velocity_X), message.Range_X , VDSConfig.controllerConfig.CheckDistance); ; // milisecond 

            result.reverseRunYN = message.State == 9?"Y":"N";
            result.trafficJamYN = message.State == 7 ? "Y" : "N"; // 차량 정체
            result.StoppedCarYN = message.State == 10 ? "Y" : "N"; // 차량 정지 


            // state : 6 -->  속도, 길이 산출
            // state : id:0 , 7 --> 정체 상태(30초), 8: 무차량 상태(30초), lane 값 유효
            // state : 9 --> 역주행, lane 값 0으로 고정 
            // state : 10 -->차량 정지. lane 값 0 으로 고정 

            return result;
        }
    }
}
