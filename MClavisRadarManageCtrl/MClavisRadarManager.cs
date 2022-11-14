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
using VDSDBHandler;
using VDSDBHandler.DBOperation;

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

        List<MCLAVIS_MESSAGE>[] _inverseMClavisMessage = new List<MCLAVIS_MESSAGE>[64]; 

        public MClavisRadarManager()
        {
            socketMsgThreadExitEvent.Reset();
            for (int i = 0; i < 2; i++)
                for(int j =0;j<16;j++)
                    _lastMClavisMessage[i,j] = new MCLAVIS_MESSAGE();

            for (int i = 0; i < 64; i++)
                _inverseMClavisMessage[i] = new List<MCLAVIS_MESSAGE>();
        }


        public int AddTrafficDataEvent(TrafficDataEvent trafficDataEvent)
        {
            int nResult = 0;
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
                _prevDataFrame = null; // 예외 발생 시 다음 패킷부터는 정상 처리 위해 null 설정
                nResult = 0;
            }
            
            // Not all data received. Get more.  
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public int ProcessDataFrame(MClavisDataFrame dataFrame)
        {
            int result = 1;
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            foreach(var message in dataFrame.messageList)
            {

                // state 4 , 5  또는 4,6 인것 항상 존재한다. 차량 속도는 State 4 인것으로,,,차량 길이는 State 6
                if (message.msgType == MCLAVIS_MESSAGE_TYPE.OBJECT_DATA && IsExistMessage(message) == false  // message.object_id > 0 && 
                    && (message.State == 6 || message.State== 7 || message.State==9 || message.State==10)
                    )
                 {
                    // state : 6 -->  속도, 길이 산출
                    // state : id:0 , 7 --> 정체 상태(30초), 8: 무차량 상태(30초), lane 값 유효
                    // state : 9 --> 역주행, lane 값 0으로 고정 
                    // state : 10 -->차량 정지. lane 값 0 으로 고정 
                    switch(message.State)
                    {
                        case 6: // 차량 검지
                            result = ProcessRadarVehicleDetect(message);
                            break;
                        case 7: // 정체 상태(30초)
                            break;
                        case 8: // 무차량 상태(30초)
                            break;
                        case 9: // 역주행 id 별로 상태 업데이트 한다. 
                            ProcessRadarInverseDetect(message);
                            break;
                        case 10: // 차량 정지 
                            ProcessRadarVehicleStop(message);
                            break;
                        case 11: // 기타 일반
                            break;
                    }
                    
                }
                // message 정보 출력(테스트용) 검지 정보 DB 기록(테스트용)
                if (message.State >= 7 && message.State <= 10) // 7: 정체, 8: 차량없음 9: 역주행 10: 정지 11:기타 의 경우 
                {
                    // object data 정보 기록
                    AddMClavisMessage(message);
                }
            }
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
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
            result.detectTime = message.DETECT_TIME;// DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
            result.direction = (byte)(message.Lane_Dir == 0 ? (int)MOVE_DIRECTION.TO_RIGHT : (int)MOVE_DIRECTION.TO_LEFT); // 1: 상행 2: 하행
            result.lane = message.Lane;
            result.length = (int)(message.Range_X * 100); // m 단위--> cm 단위로 변환
            result.speed = Math.Abs(message.Velocity_X);  // km/h
            result.vds_type = VDSConfig.GetVDSTypeName();
            result.occupyTime = Utility.GetOccupyTime(Math.Abs(message.Velocity_X), message.Range_X, VDSConfig.controllerConfig.CheckDistance); ; // milisecond 

            result.reverseRunYN = message.State == 9 ? "Y" : "N";
            result.trafficJamYN = message.State == 7 ? "Y" : "N"; // 차량 정체
            result.StoppedCarYN = message.State == 10 ? "Y" : "N"; // 차량 정지 


            // state : 6 -->  속도, 길이 산출
            // state : id:0 , 7 --> 정체 상태(30초), 8: 무차량 상태(30초), lane 값 유효
            // state : 9 --> 역주행, lane 값 0으로 고정 
            // state : 10 -->차량 정지. lane 값 0 으로 고정 


            return result;
        }


        public int AddMClavisMessage(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            TrafficDataOperation trafficDB = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"**** MCLAVIS OBJECT 정보 시작****"));
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"**** ID={message.object_id} ,  state={message.State}, Direction={message.Lane_Dir}, Lane ={message.Lane} message.Velocity_Y={message.Velocity_Y}, message.Velocity_X = {message.Velocity_X} message.Range_Y={message.Range_Y} message.Range_X={message.Range_X} ****"));
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"**** MCLAVIS OBJECT 정보 종료****"));

            trafficDB.AddRadarObjectData(new RADAR_OBJECT_DATA()
            {
                DETECT_TIME = message.DETECT_TIME,//DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT),
                ID = message.object_id,
                STATE = message.State,
                DIRECTION = message.Lane_Dir,
                LANE = message.Lane,
                YY = message.Velocity_Y,
                XX = message.Velocity_X,
                Y = message.Range_Y,
                X = message.Range_X
            }, out SP_RESULT spResult);

            if (spResult.RESULT_CODE.CompareTo("500") == 0)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"{spResult.ERROR_MESSAGE}"));
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return spResult.RESULT_COUNT;
        }

        public int ProcessRadarVehicleDetect(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 1;
            var trafficData = GetTrafficData(message);
            AddTrafficDataEvent(trafficData);
            _lastMClavisMessage[message.Lane_Dir, message.Lane] = message;

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public int ProcessRadarInverseDetect(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 1;
            MCLAVIS_INVERSE_PHASE invsersePhase = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
            // id  로 최종 역주행 정보 가져와서 역주행 정보 업데이트 한다. 
            // 역주행 판단 기준(시간 , 거리)를 충족하면 역주행 DB 에 추가 하고 기타 작업 한다. 
            if (message.object_id>=1 && message.object_id<=63)
            {
                _inverseMClavisMessage[message.object_id].Add(message);
                // 역주행 판단 기준 체크
                // 1. 역주행이 진행중인 경우 
                //    - 최종 역주행 정보 수신 후 특정시간(5초)가 경과하지 않은 경우 
                //    - 최종 역주행 정보 수신 후 이동거리가 특정 거리 이내인 경우 
                // 2. 역주행이 완료된 경우
                //    2.1 역주행인 경우
                //     - 역주행 이동 거리가 특정거리 이상
                //        역주행 정보 DB Insert
                //    2.2 역주행이 아닌 경우
                //     - 특정 시간 경과 후에도 이동거리가 특정거리 이내인 경우 
                //    2.3 역주행 트래킹 정보 clear
                invsersePhase = GetInverseRunPhase(_inverseMClavisMessage[message.object_id]);
                switch (invsersePhase)
                {
                    case MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS: // 역주행 진행중:

                        break;
                    case MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE: // 역주행 완료:
                         // 역주행 정보 Insert 
                        break;
                    case MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE:  // 역주행 아님:
                        break;
                }
                if(invsersePhase == MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE || invsersePhase == MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE)
                    _inverseMClavisMessage[message.object_id].Clear();
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"역주행 정보 오류(Track Id = {message.object_id})"));
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public int ProcessRadarVehicleStop(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 1;

            // id  로 최종 정지 정보 가져와서 역주행 정보 업데이트 한다. 
            // 역주행 판단 기준(시간 , 거리)를 충족하면 역주행 DB 에 추가 하고 기타 작업 한다. 


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }


        public MCLAVIS_INVERSE_PHASE GetInverseRunPhase(List<MCLAVIS_MESSAGE> invserseMessageList)
        {
            MCLAVIS_INVERSE_PHASE result = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
            MCLAVIS_MESSAGE firstMessage = invserseMessageList.First();
            MCLAVIS_MESSAGE lastMessage = invserseMessageList.Last();
            DateTime currentDate = DateTime.Now;
            TimeSpan ts = DateTime.Now - DateTime.ParseExact(lastMessage.DETECT_TIME,VDSConfig.RADAR_TIME_FORMAT, null);
            double passedSeconds = ts.TotalSeconds;
            double passedDistance = Math.Abs(lastMessage.Range_X - firstMessage.Range_X);
            
            // 역주행 유예시간 이내 이고 이동거리가 임계치 이내인경우  : 진행중 
            // 역주행 유예시간 이내 이고 이동거리가 임계치 이상인경우  : 완료
            // 역주행 유예시간 이상 이고 이동거리가 임계치 이내인 경우 : 역주행 아님 
            // 역주행 유예시간 이상 이고 이동거리가 임계치 이상인 경우 : 역주행 아님


            if (passedSeconds <= VDSConfig.korExConfig.invserseTime) // 역주행 판단 시간 이내인 경우
            {
                if(passedDistance < VDSConfig.korExConfig.inverseDistance) // 이동거리가 임계치 이내인 경우 (역주행 진행중)
                    result = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
                else 
                    result = MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE;       // 이동거리가 임계치 이상인 경우 (역주행)

            }
            else                                                    // 역주행 판단 시간 이상인 경우 
            {
                result = MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE;
            }
            return result;

        }

    }
}
