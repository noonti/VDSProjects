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
using System.Timers;
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

        private object _inverseMessageLock = new object();
        List<MCLAVIS_MESSAGE>[] _inverseMessage_0 = new List<MCLAVIS_MESSAGE>[64]; // 0: 다가옴 (상행)
        List<MCLAVIS_MESSAGE>[] _inverseMessage_1 = new List<MCLAVIS_MESSAGE>[64]; // 1: 멀어짐(하행)

        List<MCLAVIS_MESSAGE>[] _stopMessage_0 = new List<MCLAVIS_MESSAGE>[64]; // 0: 다가옴 (상행)
        List<MCLAVIS_MESSAGE>[] _stopMessage_1 = new List<MCLAVIS_MESSAGE>[64]; // 1: 멀어짐(하행)

        private System.Timers.Timer _timer = null;

        public MClavisRadarManager()
        {
            socketMsgThreadExitEvent.Reset();
            for (int i = 0; i < 2; i++)
                for(int j =0;j<16;j++)
                    _lastMClavisMessage[i,j] = new MCLAVIS_MESSAGE();

            for (int i = 0; i < 64; i++)
            {
                _inverseMessage_0[i] = new List<MCLAVIS_MESSAGE>();
                _inverseMessage_1[i] = new List<MCLAVIS_MESSAGE>();

                _stopMessage_0[i] = new List<MCLAVIS_MESSAGE>();
                _stopMessage_1[i] = new List<MCLAVIS_MESSAGE>();
                
            }
                
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
            StartTimer();
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
            StopTimer();
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

        public int StartTimer()
        {
            StopTimer();
            
            _timer = new System.Timers.Timer();
            _timer.Interval = 500;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
            return 1;
        }

        public int StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
                
            return 1;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //ProcessInverseMessageList();
            //ProcessStopMessageList();
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
            result.detectDistance = message.Range_X; // 검지 거리
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
            //MCLAVIS_INVERSE_PHASE invsersePhase = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
            // id  로 최종 역주행 정보 가져와서 역주행 정보 업데이트 한다. 
            // 역주행 판단 기준(시간 , 거리)를 충족하면 역주행 DB 에 추가 하고 기타 작업 한다. 
            if (message.object_id>=1 && message.object_id<=63)
            {

                AddInverseMClavisMessage(message);
                
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
                //invsersePhase = GetInverseRunPhase(_inverseMClavisMessage[message.object_id]);
                //switch (invsersePhase)
                //{
                //    case MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS: // 역주행 진행중:

                //        break;
                //    case MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE: // 역주행 완료:
                //         // 역주행 정보 Insert 
                //        break;
                //    case MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE:  // 역주행 아님:
                //        break;
                //}
                //if(invsersePhase == MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE || invsersePhase == MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE)
                //    _inverseMClavisMessage[message.object_id].Clear();
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"역주행 정보 오류(object Id = {message.object_id}) 가 1과 63 범위 밖" ));
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public int ProcessRadarVehicleStop(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 1;
            if (message.object_id >= 1 && message.object_id <= 63)
            {
                AddStopMClavisMessage(message);
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"역주행 정보 오류(Track Id = {message.object_id})"));
            }


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public int ProcessInverseMessageList()
        {
            int result = 0;
            for (int i = 1; i < 63; i++)
            {
                ProcessInverseMessage(_inverseMessage_0[i], 0);
                ProcessInverseMessage(_inverseMessage_1[i], 1);
            }
            return result;
        }

        public int ProcessStopMessageList()
        {
            int result = 0;
            for (int i = 1; i < 63; i++)
            {
                ProcessStopMessage(_stopMessage_0[i], 0);
                ProcessStopMessage(_stopMessage_1[i], 1);
            }
            return result;
        }

        private int ProcessInverseMessage(List<MCLAVIS_MESSAGE> messageList, int direction)
        {
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = 0;
            if (messageList.Count == 0)
                return 0;

            DateTime currentDate = DateTime.Now;
            // direction :  0: 다가옴 1: 멀어짐
            lock (_inverseMessageLock)
            {
                MCLAVIS_MESSAGE firstMessage = messageList.First();
                MCLAVIS_MESSAGE lastMessage = messageList.Last();
                TimeSpan ts = DateTime.Now - DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                double passedMiliSeconds = ts.TotalMilliseconds;


                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 역주행 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second "));

#if false     // 2022.11.21 역주행 시 처리 
                // 최종 역주행 정보 수신 후 경과 시간이 5초 초과 한 경우 역주행 수집 종료로 간주
                if (passedMiliSeconds < VDSConfig.korExConfig.inverseCheckTime *1000) // 역주행 진행으로 간주
                {
                    // 아무일도 하지 않는다. 
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 역주행 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second 로 {VDSConfig.korExConfig.inverseCheckTime}초 이내. 역주행 아직 진행중...."));
                }
                else               // 역주행 종료로 간주
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"최종 수신 역주행 시간 이후{VDSConfig.korExConfig.inverseCheckTime}초 경과로 역주행 종료로 간주 "));
                    double distance = Math.Abs(lastMessage.Range_X - firstMessage.Range_X);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m "));
                    //if(distance >= VDSConfig.korExConfig.inverseDistance) // 일정 거리 이상인 역주행 발생
                    if (distance >= VDSConfig.korExConfig.inverseCheckDistance) // 일정 거리 이상인 역주행 발생
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m 로 일정거리({VDSConfig.korExConfig.inverseCheckDistance} m) 이상이므로 역주행 정보 추가"));
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정로 리스트 시작 ******"));
                        foreach (var message in messageList)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                        }
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정보 리스트 종료 ******"));
                        var trafficData = GetTrafficData(firstMessage);
                        AddTrafficDataEvent(trafficData);

                    }
                    else // 역주행 발생하였으나 일정거리 이하로 무시함
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m 로 최소 역주행거리({VDSConfig.korExConfig.inverseCheckDistance}m) 미만이므로 역주행으로 판단 안함"));
                    }
                    messageList.Clear();
                }
           
#else
                double distance = Math.Abs(lastMessage.Range_X - firstMessage.Range_X);
                TimeSpan inverseTs = DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null) - DateTime.ParseExact(firstMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                double duration = inverseTs.TotalMilliseconds;


                // 역주행 주행 거리가 일정거리 이상일 경우 처리 
                // 1. 역주행 정보 수신 후 경과 시간이 특정 시간 초과한 경우 
                if (passedMiliSeconds < VDSConfig.korExConfig.inverseCheckTime * 1000) // 유효 시간 초과 하지 않은 경우 
                {
                    //if (firstMessage.processOutbreakYN == "N" && distance >= VDSConfig.korExConfig.inverseCheckDistance)
                    if (firstMessage.processOutbreakYN == "N" && duration >= VDSConfig.korExConfig.inverseMinTime * 1000)
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m 로 일정거리({VDSConfig.korExConfig.inverseCheckDistance} m) 이상이므로 역주행 정보 추가"));
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정로 리스트 시작 ******"));
                        foreach (var message in messageList)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                        }
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정보 리스트 종료 ******"));
                        var trafficData = GetTrafficData(firstMessage);
                        AddTrafficDataEvent(trafficData);
                        firstMessage.processOutbreakYN = "Y"; // 처리완료
                    }
                    else
                    {
                        // 아무일도 하지 않는다. 
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 역주행 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second 로 {VDSConfig.korExConfig.inverseCheckTime}초 . 처리여부firstMessage.processOutbreakYN ={firstMessage.processOutbreakYN} . 역주행 아직 진행중...."));
                    }
                }
                else               // 유효 시간 초과로 역주행 종료로 처리함
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"최종 수신 역주행 시간 이후{VDSConfig.korExConfig.inverseCheckTime}초 경과로 역주행 종료로 간주 "));
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m "));

                    //if (firstMessage.processOutbreakYN == "N" && distance >= VDSConfig.korExConfig.inverseCheckDistance) // 역주행 유효 주행거리 이상이고 아직 처리되지 않은 메시지의 경우 역주행 정보 추가
                    if (firstMessage.processOutbreakYN == "N" && duration >= VDSConfig.korExConfig.inverseMinTime * 1000)
                        {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 총 거리 = {distance} m 로 일정거리({VDSConfig.korExConfig.inverseCheckDistance} m). 처리여부={firstMessage.processOutbreakYN} 이므로 역주행 정보 추가"));
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정로 리스트 시작 ******"));
                        foreach (var message in messageList)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                        }
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 역주행 정보 리스트 종료 ******"));
                        var trafficData = GetTrafficData(firstMessage);
                        AddTrafficDataEvent(trafficData);
                        firstMessage.processOutbreakYN = "Y"; // 처리완료

                    }
                    else // 역주행 발생하였으나 이미 역주행 정보 처리 했거나 유효거리 이하로 무시함
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"firstMessage.object_id = {firstMessage.object_id} .역추행 처리 여부={firstMessage.processOutbreakYN}, 역주행 총 거리 = {distance} m 로 역주행 정보 추가 안함"));
                    }
                    messageList.Clear();
                }

#endif
            }
                //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
                return result;
        }

        private int ProcessStopMessage(List<MCLAVIS_MESSAGE> messageList, int direction)
        {
            //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            // Stop 메시지 처리 

            int result = 0;
            if (messageList.Count == 0)
                return 0;

            DateTime currentDate = DateTime.Now;
            // direction :  0: 다가옴 1: 멀어짐
            lock (_inverseMessageLock)
            {
                MCLAVIS_MESSAGE firstMessage = messageList.First();
                MCLAVIS_MESSAGE lastMessage = messageList.Last();
                TimeSpan ts = DateTime.Now - DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                double passedMiliSeconds = ts.TotalMilliseconds;
#if false   // 2022.11.21 수정

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 정지 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second "));
                // 최종 정지 정보 수신 후 경과 시간이 1초 초과 한 경우 정지 종료로 간주
                if (passedMiliSeconds < VDSConfig.korExConfig.stopCheckTime * 1000) // 정지 진행중으로 간주
                {
                    // 아무일도 하지 않는다. 
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 정지 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second 로 {VDSConfig.korExConfig.stopCheckTime}초 이내. 정지 아직 진행중...."));
                }
                else               // 정지 종료로 간주
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"최종 수신된 정지 시간 이후 {VDSConfig.korExConfig.stopCheckTime}초 경과로 정지 종료로 간주 "));

                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 시작 ******"));
                    foreach (var message in messageList)
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                    }
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 종료 ******"));

                    var trafficData = GetTrafficData(firstMessage);
                    AddTrafficDataEvent(trafficData);
                    messageList.Clear();
                }
#else
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 정지 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second "));
                // 최종 정지 정보 수신 후 경과 시간이 1초 초과 한 경우 정지 종료로 간주
                if (passedMiliSeconds < VDSConfig.korExConfig.stopCheckTime * 1000) // 정지 진행중으로 간주
                {

                    TimeSpan stopTs = DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null) - DateTime.ParseExact(firstMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                    double duration = stopTs.TotalMilliseconds;
                    if(firstMessage.processOutbreakYN == "N" && duration >= VDSConfig.korExConfig.stopMinTime * 1000)
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 시작 ******"));
                        foreach (var message in messageList)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                        }
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 종료 ******"));

                        var trafficData = GetTrafficData(firstMessage);
                        AddTrafficDataEvent(trafficData);

                        firstMessage.processOutbreakYN = "Y"; // 처리완료
                    }
                    else //
                    {
                        // 아무일도 하지 않는다. 
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재시간:{DateTime.Now}, 최종 저장 정지 시간:{lastMessage.DETECT_TIME} 경과 시간={passedMiliSeconds} mili second 로 {VDSConfig.korExConfig.stopCheckTime}초 이내. 정지 아직 진행중...."));
                    }

                    
                }
                else               // 정지 종료로 간주
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"최종 수신된 정지 시간 이후 {VDSConfig.korExConfig.stopCheckTime}초 경과로 정지 종료로 간주 "));
                    TimeSpan stopTs = DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null) - DateTime.ParseExact(firstMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                    double duration = stopTs.TotalMilliseconds;
                    if (firstMessage.processOutbreakYN == "N" && duration >= VDSConfig.korExConfig.stopMinTime * 1000)
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 시작 ******"));
                        foreach (var message in messageList)
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"detect time= {message.DETECT_TIME} , object id={message.object_id}, state = {message.State}, Dir={message.Lane_Dir}, Lane={message.Lane}, Range_X={message.Range_X} , Range_Y={message.Range_Y}"));
                        }
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"****** 정지 정보 리스트 종료 ******"));

                        var trafficData = GetTrafficData(firstMessage);
                        AddTrafficDataEvent(trafficData);

                        firstMessage.processOutbreakYN = "Y"; // 처리완료
                    }
                    messageList.Clear();
                }
#endif

            }
           // Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        //public MCLAVIS_INVERSE_PHASE GetInverseRunPhase(List<MCLAVIS_MESSAGE> invserseMessageList)
        //{
        //    MCLAVIS_INVERSE_PHASE result = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
        //    MCLAVIS_MESSAGE firstMessage = invserseMessageList.First();
        //    MCLAVIS_MESSAGE lastMessage = invserseMessageList.Last();


        //    DateTime currentDate = DateTime.Now;
        //    TimeSpan ts = DateTime.Now - DateTime.ParseExact(lastMessage.DETECT_TIME,VDSConfig.RADAR_TIME_FORMAT, null);
        //    double passedSeconds = ts.TotalSeconds;
        //    double passedDistance = Math.Abs(lastMessage.Range_X - firstMessage.Range_X);
            
        //    // 역주행 유예시간 이내 이고 이동거리가 임계치 이내인경우  : 진행중 
        //    // 역주행 유예시간 이내 이고 이동거리가 임계치 이상인경우  : 완료
        //    // 역주행 유예시간 이상 이고 이동거리가 임계치 이내인 경우 : 역주행 아님 
        //    // 역주행 유예시간 이상 이고 이동거리가 임계치 이상인 경우 : 역주행 아님


        //    if (passedSeconds <= VDSConfig.korExConfig.invserseTime) // 역주행 판단 시간 이내인 경우
        //    {
        //        if(passedDistance < VDSConfig.korExConfig.inverseDistance) // 이동거리가 임계치 이내인 경우 (역주행 진행중)
        //            result = MCLAVIS_INVERSE_PHASE.INVERSE_PROGRESS;
        //        else 
        //            result = MCLAVIS_INVERSE_PHASE.INVERSE_COMPLETE;       // 이동거리가 임계치 이상인 경우 (역주행)

        //    }
        //    else                                                    // 역주행 판단 시간 이상인 경우 
        //    {
        //        result = MCLAVIS_INVERSE_PHASE.INVSERSE_EXPIRE;
        //    }
        //    return result;

        //}

        public int AddInverseMClavisMessage(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            List<MCLAVIS_MESSAGE>[] messageList = message.Lane_Dir == 0 ? _inverseMessage_0 : _inverseMessage_1;
            int index = GetInverseMClavisMessageId(messageList, message);
            lock (_inverseMessageLock)
            {
                if (index >= 0)
                {
                    messageList[index].Add(message);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"역주행 id={message.object_id}는 역주행 정보 index={index} 추가. 역주행 리스트 갯수 messageList[index] = {messageList[index].Count}"));
                }
                    
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return index;
        }


        public int AddStopMClavisMessage(MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            List<MCLAVIS_MESSAGE>[] messageList = message.Lane_Dir == 0 ? _stopMessage_0 : _stopMessage_1;
            int index = GetStopMClavisMessageId(messageList, message);
            lock (_inverseMessageLock)
            {
                if (index >= 0)
                {
                    messageList[index].Add(message);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"정지 정보(message.object_id={message.object_id}는 index={index} 에 정지 정보 추가. 정지 리스트 갯수 messageList[{index}] = {messageList[index].Count}"));
                }
                    
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return index;
        }

        public int GetInverseMClavisMessageId(List<MCLAVIS_MESSAGE>[] messageList, MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = -1;
            int i = 1;
            MCLAVIS_MESSAGE lastMessage;
            int direction = message.Lane_Dir; // 0: 다가옴 1: 멀어짐. 역주행은 0: 거리가 멀어짐. 1: 거리가 가까워짐
            if (messageList[message.object_id].Count > 0) // 이미 역주행 정보가 할당 되었을 경우 해당 Id 에 추가한다. 
            {
                result = message.object_id;
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"message.object_id={message.object_id} 역주행 리스트 갯수 messageList[{message.object_id}].Count = {messageList[message.object_id].Count} 이므로 이미 할당된 역주행 index({message.object_id}) 리턴"));
            }
                
            else
            {
                for (i = 1; i < 63; i++)
                {
                    if (messageList[i].Count > 0)
                    {
                        lastMessage = messageList[i].Last();
                        if(lastMessage.DETECT_TIME.CompareTo(message.DETECT_TIME) < 0) // 추가되는 역주행정보가 마지막 역주행 정보보다 최신일 경우만 체크
                        {
                            TimeSpan ts = DateTime.ParseExact(message.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null) - DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 역주행 시간={message.DETECT_TIME}, 최종 역주행 시간={lastMessage.DETECT_TIME} . 경과 시간 = {ts.TotalMilliseconds} mili second. 동일 역주행 판단 기준 시간={VDSConfig.korExConfig.inverseGapTime} 초"));
                            if (ts.TotalMilliseconds <VDSConfig.korExConfig.inverseGapTime * 1000) // 저장된 최종 역주행 정보와의 차이가 1초 이내일 경우만 체크 
                            {

                                // 거리 차이의 평균 구하고
                                // 그 평균과의 오차가 10% 이내인 경우 동일한 역주행으로 취급한다. 
                                double diffAverageDistance = GetInverseAverageDistanceDiff(messageList[i]); 
                                diffAverageDistance = diffAverageDistance * 1.1; // // 10% 인정 오차 추가
                                switch (direction)
                                {
                                    case 0: // 다가옴. 역주행은 거리가 증가함
                                        if (Math.Abs(lastMessage.Range_X) < Math.Abs(message.Range_X)
                                            && Math.Abs(message.Range_X - lastMessage.Range_X) < diffAverageDistance // 평균 거리차이 보다 작을 경우 동일 역주행으로 판단
                                            )
                                        {
                                            result = i;
                                            
                                        }
                                        else
                                        {
                                            if (Math.Abs(lastMessage.Range_X) >= Math.Abs(message.Range_X))
                                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 오브젝트 Id={message.object_id}, 최종 수신 오브젝트 id={lastMessage.object_id}   방향(direction) = {direction} , Math.Abs(lastMessage.Range_X)={Math.Abs(lastMessage.Range_X)} , Math.Abs(message.Range_X)={Math.Abs(message.Range_X)} 거리차이={Math.Abs(message.Range_X - lastMessage.Range_X)}. 상행 역주행은 거리 증가해야 하나 거리 감소"));
                                            else
                                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 오브젝트 Id={message.object_id}, 최종 수신 오브젝트 id={lastMessage.object_id}   방향(direction) = {direction} , Math.Abs(lastMessage.Range_X)={Math.Abs(lastMessage.Range_X)} , Math.Abs(message.Range_X)={Math.Abs(message.Range_X)} 거리차이={Math.Abs(message.Range_X - lastMessage.Range_X)} 거리차이 평균값({diffAverageDistance}) 이상"));

                                        }
                                            
                                        break;
                                    case 1: // 멀어짐. 역주행은 거리가 감소함
                                        if (Math.Abs(lastMessage.Range_X) > Math.Abs(message.Range_X)
                                            && Math.Abs(message.Range_X - lastMessage.Range_X) < diffAverageDistance  // 평균 거리차이 보다 작을 경우 동일 역주행으로 판단
                                            )
                                        {
                                            result = i;
                                        }
                                        else
                                        {
                                            if(Math.Abs(lastMessage.Range_X) <= Math.Abs(message.Range_X))
                                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 오브젝트 Id={message.object_id}, 최종 수신 오브젝트 id={lastMessage.object_id}   방향(direction) = {direction} , Math.Abs(lastMessage.Range_X)={Math.Abs(lastMessage.Range_X)} , Math.Abs(message.Range_X)={Math.Abs(message.Range_X)} 거리차이={Math.Abs(message.Range_X - lastMessage.Range_X)}. 하행 역주행은 거리 감소해야 하나 거리 증가"));
                                            else
                                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 오브젝트 Id={message.object_id}, 최종 수신 오브젝트 id={lastMessage.object_id}   방향(direction) = {direction} , Math.Abs(lastMessage.Range_X)={Math.Abs(lastMessage.Range_X)} , Math.Abs(message.Range_X)={Math.Abs(message.Range_X)} 거리차이={Math.Abs(message.Range_X - lastMessage.Range_X)} 거리차이 평균값({diffAverageDistance}) 이상"));
                                        }
                                            
                                        break;
                                }
                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"방향(direction) = {direction} , Math.Abs(lastMessage.Range_X)={Math.Abs(lastMessage.Range_X)} , Math.Abs(message.Range_X)={Math.Abs(message.Range_X)} result={result}"));
                            }
                        }
                    }
                    if (result > 0)
                        break;
                }
                if (result == -1) // 편입할 정보를 찾지 못할 경우 신규 역주행 정보 할당 
                {
                    result = message.object_id;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"편입할 역주행 정보 찾지 못함. 해당 아이디({result})에 할당"));
                }
                    
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }

        public double GetInverseAverageDistanceDiff(List<MCLAVIS_MESSAGE> messageList)
        {
            double result = 0;
            double sum = 0;
            double prevDistance = 0;
            int count = 0;
            foreach(var message in messageList)
            {
                sum += (Math.Abs(message.Range_X-prevDistance));
                prevDistance = message.Range_X;
                count++;
            }

            if(count>0)
                result = sum / count;
            return result;
        }

        public int GetStopMClavisMessageId(List<MCLAVIS_MESSAGE>[] messageList, MCLAVIS_MESSAGE message)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int result = -1;
            int i = 1;
            MCLAVIS_MESSAGE lastMessage;
            int direction = message.Lane_Dir; // 0: 다가옴 1: 멀어짐. 역주행은 0: 거리가 멀어짐. 1: 거리가 가까워짐
            if (messageList[message.object_id].Count > 0) // 이미 정지 정보가 할당 되었을 경우 해당 Id 에 추가한다. 
            {
                result = message.object_id;
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"정지 리스트 갯수 messageList[message.object_id].Count = {messageList[message.object_id].Count} 이므로 이미 할당된 정지 id 리턴"));
            }
            else
            {
                for (i = 1; i < 63; i++)
                {
                    if (messageList[i].Count > 0)
                    {
                        lastMessage = messageList[i].Last();
                        if (lastMessage.DETECT_TIME.CompareTo(message.DETECT_TIME) < 0) // 추가되는 정지 정보가 마지막 정지 정보보다 최신일 경우만 체크
                        {
                            TimeSpan ts = DateTime.ParseExact(message.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null) - DateTime.ParseExact(lastMessage.DETECT_TIME, VDSConfig.RADAR_TIME_FORMAT, null);
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 정지 시간={message.DETECT_TIME}, 최종 정지 시간={lastMessage.DETECT_TIME} . 경과 시간 = {ts.TotalMilliseconds} mili second , 동일 정지 정보 판단 기준 시기={VDSConfig.korExConfig.stopGapTime} 초"));
                            if (ts.TotalMilliseconds < VDSConfig.korExConfig.stopGapTime *1000) // 저장된 최종 정지 정보와의 차이가 1초 이내일 경우만 체크 
                            {
                                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"현재 정지 거리={message.Range_X}, 최종 정지 시간={lastMessage.Range_X} . 차이 = {Math.Abs(message.Range_X - lastMessage.Range_X)} m "));
                                if (Math.Abs(message.Range_X - lastMessage.Range_X) < VDSConfig.korExConfig.stopGapDistance) // 1미터 이내 차이일 경우 같은 정지로 간주한다. 
                                {
                                    result = i;
                                    break;
                                }
                               
                            }
                        }
                    }
                    if (result > 0)
                        break;
                }
                if (result == -1) // 편입할 정보를 찾지 못할 경우 신규 정지 정보 할당 
                {
                    result = message.object_id;
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"편입할 정지 정보 찾지 못함. 해당 아이디({result})에 할당"));
                }
                    
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return result;
        }
    }
}
