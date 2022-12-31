using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VideoVDSManageCtrl;
using VideoVDSManageCtrl.Protocol;
using VDSDBHandler.DBOperation;
using VDSDBHandler;
using VDSDBHandler.Model;

namespace VideoVDSServerSimulator
{
    public partial class Form1 : Form
    {
        VDSServer _videoVDSServer = new VDSServer();
        List<UnisemVDSClient> _clientList = new List<UnisemVDSClient>();
        private VDSLogger _Logger = new VDSLogger();

        DataFrame _prevDataFrame = null;
        Timer _timer = null;
        TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);

        Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            _Logger.SetManagerType(MANAGER_TYPE.VDS_MONITOR);
            Utility._addLog = _Logger.AddLog;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_videoVDSServer._bListening)
            {
                _videoVDSServer.StopManager();
                _Logger.StopManager();
                btnStart.Text = "Start";
            }
            else
            {
                _Logger.StartManager();
                _videoVDSServer.SetAddress(txtServerIPAddress.Text, int.Parse(txtServerPortNo.Text), CLIENT_TYPE.VIDEO_VDS_CLIENT, AcceptCtrlCallback);
                _videoVDSServer.StartManager();
                btnStart.Text = "Stop";
            }
        }


        public void AcceptCtrlCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                UnisemVDSClient unisemClient = new UnisemVDSClient();
                unisemClient.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                unisemClient._sessionContext._socket = socket;
                _clientList.Add(unisemClient);

                socket.BeginReceive(unisemClient._sessionContext.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(UnisemClientReadCallback), unisemClient);

                //if (socket.Connected)
                {
                    //KICTClient kictClient = new KICTClient();
                    //kictClient._type = _kictEventServer._clientType;
                    //kictClient._socket = socket;
                    //socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                    //    new AsyncCallback(KictEventReadCallback), kictClient);

                    //strLog = String.Format("Remote Client accepted");
                    //Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    //Console.WriteLine(strLog);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            finally
            {
                _videoVDSServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void UnisemClientReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));

            String content = String.Empty;
            String strLog;

            UnisemVDSClient unisemClient = (UnisemVDSClient)ar.AsyncState;
            Socket socket = unisemClient._sessionContext._socket;
            try
            {
                // Read data from the client socket.
                int bytesRead = socket.EndReceive(ar);
                Console.WriteLine("ReadCallback {0} byte", bytesRead);
                if (bytesRead > 0)
                {
                    byte[] packet = new byte[bytesRead];
                    Array.Copy(unisemClient._sessionContext.buffer, 0, packet, 0, bytesRead);
                    ProcessReceivePacket(unisemClient, packet, bytesRead);

                    socket.BeginReceive(unisemClient._sessionContext.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(UnisemClientReadCallback), unisemClient);

                }
                else
                {
                    DeleteUnisemClient(unisemClient);
                    strLog = String.Format($"원격 클라이언트 연결 종료 client count={_clientList.Count}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private int ProcessReceivePacket(UnisemVDSClient unisem,  byte[] packet, int length)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            int i = 0;
            while (i < packet.Length)
            {
                if (_prevDataFrame == null)
                {
                    _prevDataFrame = new DataFrame();
                }

                i += _prevDataFrame.Deserialize(packet, i);
                if (_prevDataFrame.bDataCompleted)
                {
                    // processDataFrame....
                    ProcessDataFrame(unisem, _prevDataFrame);
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

        private int ProcessDataFrame(UnisemVDSClient unisem, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (frame.OpCode)
            {
                case DataFrameDefine.OPCODE_AUTH_VDS:
                    nResult = ProcessAuthVDS(unisem, frame);
                    break;
                case DataFrameDefine.OPCODE_EVENT_TRAFFIC:
                    break;
                case DataFrameDefine.OPCODE_HISTORIC_TRAFFIC:
                    nResult = ProcessHistoricTrafficData(unisem, frame);
                    break;
                case DataFrameDefine.OPCODE_HEARTBEAT:
                    nResult = ProcessHeartBeat(unisem, frame);
                    break;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessAuthVDS(UnisemVDSClient unisem, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSAuthRequest request;
            VDSAuthResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                response = new VDSAuthResponse();


                request = frame.opDataFrame as VDSAuthRequest;


                response.resultCode = 100;
                response.resultMessage = "success";
                response.rtspSourceURL = txtRTSPURL1.Text;
                response.rtspDetectionURL = txtRTSPURL2.Text;

                DataFrame resFrame = new DataFrame();

                resFrame.RequestType = DataFrameDefine.TYPE_RESPONSE;
                resFrame.OpCode = DataFrameDefine.OPCODE_AUTH_VDS;
                resFrame.opDataFrame = response;

                byte[] packet = resFrame.Serialize();
                nResult = Send(unisem._sessionContext, packet);
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        private int ProcessHeartBeat(UnisemVDSClient unisem, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSHeartBeatRequest request;
            VDSHeartBeatResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                unisem.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);
                response = new VDSHeartBeatResponse();


                request = frame.opDataFrame as VDSHeartBeatRequest;


                response.resultCode = 100;
                response.resultMessage = "success";
                response.vdsControllerId = request.vdsControllerId;
                response.heartBeatTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);


                DataFrame resFrame = new DataFrame();

                resFrame.RequestType = DataFrameDefine.TYPE_RESPONSE;
                resFrame.OpCode = DataFrameDefine.OPCODE_HEARTBEAT;
                resFrame.opDataFrame = response;

                byte[] packet = resFrame.Serialize();
                nResult = Send(unisem._sessionContext, packet);
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }


        private int ProcessHistoricTrafficData(UnisemVDSClient unisem, DataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            VDSHistoricTrafficDataRequest request;
            VDSHistoricTrafficDataResponse response;
            if (frame != null && frame.opDataFrame != null)
            {
                
                response = new VDSHistoricTrafficDataResponse();


                request = frame.opDataFrame as VDSHistoricTrafficDataRequest;


                response.resultCode = 100;
                response.resultMessage = "success";
                response.startDateTime = request.startDateTime;
                response.endDateTime = request.endDateTime;
                response.pageIndex = request.pageIndex;
                response.pageSize = request.pageSize;

                response.totalCount = 300;
                for(int i=0;i<VDSConfig.PAGE_SIZE;i++)
                {

                    TrafficData trafficData = new TrafficData()
                    {

                        id = GetTrafficDataEventID(),
                        lane = 1, // 4 차선  상행선: 3,4 차선  하행선: 1,2 차선
                        direction = 1,
                        length = 1200,
                        speed = 100,
                        vehicle_class = 2,
                        occupyTime = 100,
                        loop1OccupyTime = 200,
                        loop2OccupyTime = 300,
                        reverseRunYN = "N",
                        vehicleGap = 12,
                        detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT),

                    };
                    response.trafficDataList.Add(trafficData);
                }

                DataFrame resFrame = new DataFrame();

                resFrame.RequestType = DataFrameDefine.TYPE_RESPONSE;
                resFrame.OpCode = DataFrameDefine.OPCODE_HISTORIC_TRAFFIC;
                resFrame.opDataFrame = response;

                byte[] packet = resFrame.Serialize();
                nResult = Send(unisem._sessionContext, packet);
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }
        private int Send(SessionContext session, byte[] byteData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 1;
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
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartTimer();
            
        }

        private String GetTrafficDataEventID()
        {
            String result = String.Empty;

            result = DateTime.Now.ToString("yyMMddHHmmss_") + Guid.NewGuid().ToString();
            return result;
        }

        private void SendTrafficDataEvent(SessionContext session)
        {
            
            VDSTrafficDataEvent trafficDataEvent = new VDSTrafficDataEvent();
            //byte _lane = (byte)((DateTime.Now.Millisecond % 4)+1);

            byte _lane = (byte)(random.Next(4)+1 );// ((DateTime.Now.Millisecond % 8) + 1);
            byte _direction = 0;
            if (_lane <3)
                _direction = (int)MOVE_DIRECTION.TO_LEFT;
            else
                _direction = (int)MOVE_DIRECTION.TO_RIGHT;

            TrafficData trafficData = new TrafficData()
            {

                id = GetTrafficDataEventID(),
                lane = _lane, // 4 차선  상행선: 3,4 차선  하행선: 1,2 차선
                direction = _direction,
                length = GetRandomLength(),
                speed = GetRandomSpeed(),
                vehicle_class = 2,
                occupyTime = GetRandomOccupyTime(),
                loop1OccupyTime = 200,
                loop2OccupyTime = 300,
                //reverseRunYN = DateTime.Now.Second % 2 == 0? "N":"Y",
                reverseRunYN = "N" ,
                StoppedCarYN = "Y" ,
                vehicleGap = 12,
                detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT),
                detectDistance = -100
               
            };

            trafficDataEvent.trafficDataList.Add(trafficData);

            trafficDataEvent.totalCount = trafficDataEvent.trafficDataList.Count;


            DataFrame data = new DataFrame();
            data.RequestType = DataFrameDefine.TYPE_COMMAND;
            data.OpCode = DataFrameDefine.OPCODE_EVENT_TRAFFIC;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data.TransactionId, 0, id.Length);
            data.opDataFrame = trafficDataEvent;

            byte[] packet = data.Serialize();

            if(Send(session, packet)>0) //
            {
                //db.AddTrafficTestData(new TRAFFIC_DATA()
                //{
                //    ID = trafficData.id,
                //    VDS_TYPE = "VDS_UNISEM",
                //    LANE= trafficData.lane,
                //    DIRECTION = trafficData.direction,
                //    LENGTH = trafficData.length,
                //    SPEED = trafficData.speed,
                //    VEHICLE_CLASS = trafficData.vehicle_class,
                //    OCCUPY_TIME = trafficData.occupyTime,
                //    LOOP1_OCCUPY_TIME = trafficData.loop1OccupyTime,
                //    LOOP2_OCCUPY_TIME = trafficData.loop2OccupyTime,
                //    REVERSE_RUN_YN = trafficData.reverseRunYN,
                //    VEHICLE_GAP = trafficData.vehicleGap,
                //    DETECT_TIME = trafficData.detectTime,
                //    REPORT_YN = "Y"
                   
                //}, out SP_RESULT spResult);
            }
            else
            {
                //Console.WriteLine("전송 실패");
                //db.AddTrafficTestData(new TRAFFIC_DATA()
                //{
                //    ID = trafficData.id,
                //    VDS_TYPE = "VDS_UNISEM",
                //    LANE = trafficData.lane,
                //    DIRECTION = trafficData.direction,
                //    LENGTH = trafficData.length,
                //    SPEED = trafficData.speed,
                //    VEHICLE_CLASS = trafficData.vehicle_class,
                //    OCCUPY_TIME = trafficData.occupyTime,
                //    LOOP1_OCCUPY_TIME = trafficData.loop1OccupyTime,
                //    LOOP2_OCCUPY_TIME = trafficData.loop2OccupyTime,
                //    REVERSE_RUN_YN = trafficData.reverseRunYN,
                //    VEHICLE_GAP = trafficData.vehicleGap,
                //    DETECT_TIME = trafficData.detectTime,
                //    REPORT_YN = "N"

                //}, out SP_RESULT spResult);
            }
        }

        public void StartTimer()
        {
            if (_timer == null)
                _timer = new Timer();

            _timer.Interval = 1000; // 0.5 초마다 타이머 동작
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            List<UnisemVDSClient> delList = new List<UnisemVDSClient>();

            foreach (var unisem in _clientList)
            {
                if (!CheckHeartBeatTime(unisem))
                {
                    delList.Add(unisem);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Heartbeat 시간 초과......."));
                }

            }



            foreach (var unisem in delList)
            {
                DeleteUnisemClient(unisem);

            }

            for (int i=0;i<8;i++)
            {
                foreach (var unisem in _clientList)
                {
                    SendTrafficDataEvent(unisem._sessionContext);
                }

            }

            //Console.WriteLine($"RandomSpped={GetRandomSpeed()}, GetRandomLength={GetRandomLength()} , GetRandomOccupyTime={GetRandomOccupyTime()}");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer = null;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var unisem in _clientList)
            {
                SendTrafficDataEvent(unisem._sessionContext);
            }
        }

        private bool CheckHeartBeatTime(UnisemVDSClient unisem)
        {
            bool result = true;
            if (!String.IsNullOrEmpty(unisem.heartBeatTime))
            {
                DateTime heartbeatTime = DateTime.ParseExact(unisem.heartBeatTime, VDSConfig.RADAR_TIME_FORMAT, null);
                TimeSpan span = DateTime.Now - heartbeatTime;
                if (span.TotalSeconds > 60)
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Heartbeat 시간 초과......."));
                    result = false;
                }
            }
            
                
            return result;
        }

        private void DeleteUnisemClient(UnisemVDSClient unisem)
        {
            if(unisem!=null)
            {
                unisem._sessionContext._socket.Shutdown(SocketShutdown.Both);
                unisem._sessionContext._socket.Close();
                _clientList.Remove(unisem);
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Unisem Client 연결 종료"));
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int totalSIze = 0;
            byte[] sendPacket = new byte[15000];

            for (int i = 0; i < 15; i++)
            {

                VDSTrafficDataEvent trafficDataEvent = new VDSTrafficDataEvent();
                byte _lane = (byte)((DateTime.Now.Second % 4) + 1);
                byte _direction = 0;
                if (_lane > 2) // 상행선
                    _direction = 1;
                else           // 하행선 
                    _direction = 2;

                trafficDataEvent.trafficDataList.Add(
                    new TrafficData()
                    {
                        id = GetTrafficDataEventID(),
                        lane = _lane, // 4 차선  상행선: 3,4 차선  하행선: 1,2 차선
                        direction = _direction,
                        length = GetRandomLength(),
                        speed = GetRandomSpeed(),
                        vehicle_class = 2,
                        occupyTime = GetRandomOccupyTime(),
                        loop1OccupyTime = 200,
                        loop2OccupyTime = 300,
                        reverseRunYN = "N",
                        vehicleGap = 12,
                        detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT),
                    });

                trafficDataEvent.totalCount = trafficDataEvent.trafficDataList.Count;


                DataFrame data = new DataFrame();
                data.RequestType = DataFrameDefine.TYPE_COMMAND;
                data.OpCode = DataFrameDefine.OPCODE_EVENT_TRAFFIC;
                byte[] id = Utility.StringToByte("transactionID");
                Array.Copy(id, 0, data.TransactionId, 0, id.Length);
                data.opDataFrame = trafficDataEvent;

                byte[] packet = data.Serialize();

                //Array.Copy(sendPacket, totalSIze, packet, 0, packet.Length);
                Array.Copy(packet,0, sendPacket, totalSIze, packet.Length);
                totalSIze += packet.Length;

            }

            byte[] sendbyte = new byte[totalSIze];
            Array.Copy(sendPacket,0, sendbyte, 0, totalSIze);

            foreach (var unisem in _clientList)
            {

                Send(unisem._sessionContext, sendbyte);
            }

            
        }

        public int GetRandomSpeed()
        {
            int result = 0;
            Random rand = new Random();
            result = rand.Next(10, 150);

            return result ;
        }

        public int GetRandomLength()
        {
            int result = 0;
            Random rand = new Random();
            result = rand.Next(300, 1550);
            return result ;
        }

        public int GetRandomOccupyTime()
        {
            int result = 0;
            Random rand = new Random();
            result = rand.Next(500, 1000);
            return result;
        }

        private void txtRTSPURL1_Enter(object sender, EventArgs e)
        {
            //Utility.ShowVirtualKeyborad(sender as Control, this);
        }

        private void txtRTSPURL1_Leave(object sender, EventArgs e)
        {
            //Utility.HideVirtualKeyboard();
        }

        private void txtTest_Enter(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            foreach (var unisem in _clientList)
            {
                SendSingleTrafficDataEvent(unisem._sessionContext);
            }
        }

        private void SendSingleTrafficDataEvent(SessionContext session)
        {

            VDSTrafficDataEvent trafficDataEvent = new VDSTrafficDataEvent();
            //byte _lane = (byte)((DateTime.Now.Millisecond % 4)+1);

            byte _lane = (byte)(int.Parse(txtLane.Text));// ((DateTime.Now.Millisecond % 8) + 1);
            byte _direction = (byte)int.Parse(txtDirection.Text);
            

            TrafficData trafficData = new TrafficData()
            {

                id = GetTrafficDataEventID(),
                lane = _lane, // 4 차선  상행선: 3,4 차선  하행선: 1,2 차선
                direction = _direction,
                length = int.Parse(txtLength.Text),
                speed = int.Parse(txtSpeed.Text),
                vehicle_class = 2,
                occupyTime = GetRandomOccupyTime(),
                loop1OccupyTime = 200,
                loop2OccupyTime = 300,
                //reverseRunYN = DateTime.Now.Second % 2 == 0? "N":"Y",
                reverseRunYN = "N",
                StoppedCarYN = "N",
                vehicleGap = 12,
                detectTime = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT),
                detectDistance = -100

            };

            trafficDataEvent.trafficDataList.Add(trafficData);

            trafficDataEvent.totalCount = trafficDataEvent.trafficDataList.Count;


            DataFrame data = new DataFrame();
            data.RequestType = DataFrameDefine.TYPE_COMMAND;
            data.OpCode = DataFrameDefine.OPCODE_EVENT_TRAFFIC;
            byte[] id = Utility.StringToByte("transactionID");
            Array.Copy(id, 0, data.TransactionId, 0, id.Length);
            data.opDataFrame = trafficDataEvent;

            byte[] packet = data.Serialize();

            if (Send(session, packet) > 0) //
            {
                //db.AddTrafficTestData(new TRAFFIC_DATA()
                //{
                //    ID = trafficData.id,
                //    VDS_TYPE = "VDS_UNISEM",
                //    LANE= trafficData.lane,
                //    DIRECTION = trafficData.direction,
                //    LENGTH = trafficData.length,
                //    SPEED = trafficData.speed,
                //    VEHICLE_CLASS = trafficData.vehicle_class,
                //    OCCUPY_TIME = trafficData.occupyTime,
                //    LOOP1_OCCUPY_TIME = trafficData.loop1OccupyTime,
                //    LOOP2_OCCUPY_TIME = trafficData.loop2OccupyTime,
                //    REVERSE_RUN_YN = trafficData.reverseRunYN,
                //    VEHICLE_GAP = trafficData.vehicleGap,
                //    DETECT_TIME = trafficData.detectTime,
                //    REPORT_YN = "Y"

                //}, out SP_RESULT spResult);
            }
            else
            {
                //Console.WriteLine("전송 실패");
                //db.AddTrafficTestData(new TRAFFIC_DATA()
                //{
                //    ID = trafficData.id,
                //    VDS_TYPE = "VDS_UNISEM",
                //    LANE = trafficData.lane,
                //    DIRECTION = trafficData.direction,
                //    LENGTH = trafficData.length,
                //    SPEED = trafficData.speed,
                //    VEHICLE_CLASS = trafficData.vehicle_class,
                //    OCCUPY_TIME = trafficData.occupyTime,
                //    LOOP1_OCCUPY_TIME = trafficData.loop1OccupyTime,
                //    LOOP2_OCCUPY_TIME = trafficData.loop2OccupyTime,
                //    REVERSE_RUN_YN = trafficData.reverseRunYN,
                //    VEHICLE_GAP = trafficData.vehicleGap,
                //    DETECT_TIME = trafficData.detectTime,
                //    REPORT_YN = "N"

                //}, out SP_RESULT spResult);
            }
        }
    }

}
