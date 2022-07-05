using BenchMarkManageCtrl;
using RadarManageCtrl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.DataType;
using VDSDBHandler;
using VDSDBHandler.DBOperation;

namespace VDSSimulator
{

    public delegate int FormAddWorkDataDelegate(WorkData workData);

    public delegate void DisplayDataDelegate(DataFrame request, DataFrame resposne);

    public partial class Form1 : Form
    {
        VDSServer _kictEventServer = new VDSServer();
        private VDSLogger _Logger = new VDSLogger();


        Color panelOriginalColor;
        DateTime _KictEventDisplayTime = DateTime.Now;
        System.Windows.Forms.Timer _currentTimer = null;
        FormAddWorkDataDelegate _formAddWorkDataDelegate = null;
        DisplayDataDelegate _displayDataDelegate = null;

        Thread [] threads;
        Thread trafficDataThread;
        bool bAutoSend = false;
        bool bSendHistoricalData = false;

        bool bSyncTime = false;
        DateTime _lastSyncTime = DateTime.Now;

        StreamWriter _carDetectInfo;
        

        public Form1()
        {
            InitializeComponent();
            _carDetectInfo = null;
            //threadExitEvent.Reset();
            _Logger.SetManagerType(MANAGER_TYPE.VDS_MONITOR);
            Utility._addLog = _Logger.AddLog;

            _formAddWorkDataDelegate = new FormAddWorkDataDelegate(AddWorkData);
            _displayDataDelegate = new DisplayDataDelegate(DisplayData);
            _currentTimer = new System.Windows.Forms.Timer();
            _currentTimer.Interval = 100; // 1초마다 체크
            _currentTimer.Tick += new EventHandler(Timer_Tick);
            _currentTimer.Start();

            panelOriginalColor = splitLane.Panel1.BackColor;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var nowDate = DateTime.Now;
            if (_KictEventDisplayTime != null && nowDate > _KictEventDisplayTime)
            {
                //Console.WriteLine("hide...");
                splitLane.Panel1.BackColor = panelOriginalColor;
            }
            if(bSyncTime)
                ProcessAutoTimeSync();
            

        }

        public int AddWorkData(WorkData workData)
        {
            try
            {
            
                if (lbxTarget.Items.Count > 1000)
                    lbxTarget.Items.RemoveAt(lbxTarget.Items.Count - 1);

                TrafficRequest request = (TrafficRequest) workData.frame.opDataFrame;

                String info = String.Format("[{0}]\t 차선: {1}\t 속도: {2} km/h\t 길이: {3} cm\t 점유시간 {4} ",
                                            Utility.ByteToDate(request.trafficData.checkTime)?.ToString(VDSConfig.RADAR_TIME_FORMAT),request.trafficData.lane, request.trafficData.velocity, request.trafficData.carLength,request.trafficData.occupyTime);
                if (_carDetectInfo != null)
                    _carDetectInfo.WriteLine(info);

                lbxTarget.Items.Insert(0, info);

                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500); //5초 후에 초기화
                _KictEventDisplayTime = DateTime.Now.Add(duration);

                splitLane.Panel1.BackColor = Color.Blue;
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            return 1;
        }


        private void button1_Click_2(object sender, EventArgs e)
        {

            if (_kictEventServer._bListening)
            {

                if (_carDetectInfo != null)
                    _carDetectInfo.Close();
                _carDetectInfo = null;

                _kictEventServer.StopManager();
                _Logger.StopManager();
                btnStart.Text = "Start";
            }
            else
            {


                String destFileName = System.IO.Path.Combine(Utility.GetLogPath(), "laneInfo.log");
                _carDetectInfo = File.CreateText(destFileName);
                
                _Logger.StartManager();
                _kictEventServer.SetAddress(txtEventIPAddress.Text, int.Parse(txtEventPortNo.Text), CLIENT_TYPE.KICT_EVNT_CLIENT, AcceptCtrlCallback);
                _kictEventServer.StartManager();
                btnStart.Text = "Stop";
            }

        }

        public void AcceptCtrlCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                //if (socket.Connected)
                {
                    KICTClient kictClient = new KICTClient();
                    kictClient._type = _kictEventServer._clientType;
                    kictClient._socket = socket;
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(KictEventReadCallback), kictClient);

                    strLog = String.Format("Remote Client accepted");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    Console.WriteLine(strLog);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            finally
            {
                _kictEventServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void KictEventReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));

            String content = String.Empty;
            String strLog;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            KICTClient kictClient = (KICTClient)ar.AsyncState;
            Socket socket = kictClient._socket;
            try
            {
                // Read data from the client socket.
                int bytesRead = socket.EndReceive(ar);
                Console.WriteLine("ReadCallback {0} byte", bytesRead);
                if (bytesRead > 0)
                {
                    strLog = String.Format("ReadCallback {0} 바이트 데이터 수신", bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    byte[] packet = new byte[bytesRead];
                    Array.Copy(kictClient.buffer, 0, packet, 0, bytesRead);

                    strLog = Utility.PrintHexaString(packet, bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"PrintHexa:{strLog}"));

                    int i = 0;
                    while (i < packet.Length)
                    {
                        if (kictClient._prevDataFrame == null)
                        {
                            kictClient._prevDataFrame = new DataFrame();
                        }

                        i += kictClient._prevDataFrame.Deserialize(packet,i);
                        if (kictClient._prevDataFrame.bDataCompleted)
                        {
                            WorkData workData = new WorkData();
                            DataFrameDefine.InitWorkData(ref workData);
                            workData.session = kictClient;
                            workData.frame = kictClient._prevDataFrame;

                            ProcessTrafficEventData(workData);

                            kictClient._prevDataFrame = null;
                        }
                    }
                    // Not all data received. Get more.  
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KictEventReadCallback), kictClient);

                }
                else
                {
                    if (socket != null && kictClient._type != CLIENT_TYPE.KICT_EVNT_CLIENT)
                    {
                        strLog = String.Format("원격 클라이언트 연결 종료111");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    else
                    {
                        strLog = String.Format("원격 클라이언트 연결 종료222");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        //Console.WriteLine("22Close socket");
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }
        public int ProcessTrafficEventData(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                
                TrafficRequest request = (TrafficRequest)workData.frame.opDataFrame;
                TrafficResponse response = new TrafficResponse();
                response.lane = request.trafficData.lane;
                response.direction = request.trafficData.direction;
                Array.Copy(request.trafficData.checkTime,0, response.checkTime, 0, 8);
                response.status = 1;
                dataFrame.opDataFrame = response;
                byte[] data = dataFrame.Serialize();
                
                SendByEthernet((KICTClient)workData.session, data, data.Length);
                //AddTargetInfo(workData);
                BeginInvoke(_formAddWorkDataDelegate, new object[] { workData });

            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendByEthernet(KICTClient kictClient, byte[] data, int size)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                kictClient._socket.BeginSend(data, 0, size, 0,
                            new AsyncCallback(KictEventSendCallback), kictClient._socket);
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void KictEventSendCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));

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
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        
        public VDSSyncClient GetSyncClient()
        {
            VDSSyncClient Result;
            try
            {
                Result = new VDSSyncClient();
                if (!(Result.Connect(txtVDSAddress.Text, int.Parse(txtVDSPort.Text), 5) > 0 && Result._status == SOCKET_STATUS.CONNECTED))
                {
                    Result = null;
                }
                LingerOption lingerOpts = new LingerOption(false, 0);
                Result._sessionContext._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger,(object)lingerOpts);


            }
            catch (Exception ex)
            {
                Result = null;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return Result;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SendHistoricalData();
            if (bSendHistoricalData) // 중지
            {
                bSendHistoricalData = false;
                trafficDataThread.Join();
                btnTrafficData.Text = "Historical Data 시작";

            }
            else         // 시작
            {
                bSendHistoricalData = true;
                trafficDataThread = new Thread(SendHistoricalDataThread);
                trafficDataThread.Start();
                btnTrafficData.Text = "Historical Data 중지";
                
            }
        }

        public int SendHistoricalData()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            //VDSSyncClient client = GetSyncClient();
            DataFrame reqFrame;
            DataFrame resFrame;

            try
            {
                //if(client!=null)
                //{
                    reqFrame = new DataFrame();
                    reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                    HistoricalTrafficRequest request = new HistoricalTrafficRequest();
                    reqFrame.opDataFrame = request;
                    if(SendData(ref reqFrame, out resFrame)>0)
                    {
                        BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                        //DisplayData(reqFrame, resFrame);
                    }

                  //  client.Close();
                //}
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendData(ref DataFrame request, out DataFrame response)
        {
            int nResult = 0;
            byte[] data = request.Serialize();
            byte[] receiveData = null;
            int receiveCount = 0;
            response = null;
            VDSSyncClient client; ;

            try
            {
                for (int i = 0; i < 1; i++)
                {
                    client = GetSyncClient();
                    if (i > 0)
                    {
                        request.OpCode = DataFrameDefine.OP_FRAME_RE_REQUEST;
                        data = request.Serialize();
                        Console.WriteLine("retry.....");
                    }
                    if (client.SendAndReceive(data, data.Length, ref receiveData, out receiveCount, 5) > 0)
                    {
                        byte[] packet = new byte[receiveCount];
                        Array.Copy(receiveData, 0, packet, 0, receiveCount);
                        response = new DataFrame();
                        response.Deserialize(packet,0);
                        nResult = receiveCount;
                        Console.WriteLine("receive...data ok");
                        client.Close();
                        break;
                    }
                   
                    client.Close();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return nResult;
        }

        //public int SendData(VDSSyncClient client, ref DataFrame request, out DataFrame response)
        //{
        //    int nResult = 0;
        //    byte[] data = request.Serialize();
        //    byte[] receiveData = null;
        //    int receiveCount = 0;
        //    response = null;

        //    try
        //    {
        //        for(int i = 0;i<VDSConfig.RETRY_COUNT;i++)
        //        {
        //            if(i>0)
        //            {
        //                request.OpCode = DataFrameDefine.OP_FRAME_RE_REQUEST;
        //                data = request.Serialize();
        //                Console.WriteLine("retry.....");
        //            }
        //            if (client.SendAndReceive(data, data.Length, ref receiveData, out receiveCount, 0.1) > 0)
        //            {
        //                byte[] packet = new byte[receiveCount];
        //                Array.Copy(receiveData, 0, packet, 0, receiveCount);
        //                response = new DataFrame();
        //                response.Deserialize(packet);
        //                nResult = receiveCount;
        //                Console.WriteLine("receive...data ok");
        //                break;
        //            }

        //        }

        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //    return nResult;
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            SendEchoBackData();
        }

        public int SendEchoBackData()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            DataFrame reqFrame;
            DataFrame resFrame;

            try
            {
                reqFrame = new DataFrame();
                reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                EchoBackRequest request = new EchoBackRequest();

                for (int i = 0; i < 100; i++)
                    request.EchoData[i] = (byte)i;
                reqFrame.opDataFrame = request;
                if (SendData(ref reqFrame, out resFrame) > 0)
                {
                    BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                    //DisplayData(reqFrame, resFrame);
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendVDSStatusData()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            DataFrame reqFrame;
            DataFrame resFrame;

            try
            {
                reqFrame = new DataFrame();
                reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                VDSStatusRequest request  = new VDSStatusRequest();
                    
                reqFrame.opDataFrame = request;
                if (SendData(ref reqFrame, out resFrame) > 0)
                {
                    BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                    //DisplayData(reqFrame, resFrame);
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendVDSStatusData();
        }

        public int SendVDSStartStopData(byte flag)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            DataFrame reqFrame;
            DataFrame resFrame;

            try
            {
                reqFrame = new DataFrame();
                reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                VDSStartRequest request = new VDSStartRequest();
                request.VDSFlag = flag;
                reqFrame.opDataFrame = request;
                if (SendData(ref reqFrame, out resFrame) > 0)
                {
                    BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                    //DisplayData(reqFrame, resFrame);
                }
                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendVDSStartStopData(0x01);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendVDSStartStopData(0x02);
        }


        public int SendVDSSetTimeData(DateTime date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            DataFrame reqFrame;
            DataFrame resFrame;

            try
            {
                reqFrame = new DataFrame();
                reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                VDSSetTimeRequest request = new VDSSetTimeRequest();
                request.setTime = Utility.DateToByte(date);
                //Utility.ByteToDate(request.setTime);
                reqFrame.opDataFrame = request;
                if (SendData(ref reqFrame, out resFrame) > 0)
                {
                    BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                    //DisplayData(reqFrame, resFrame);
                }

                nResult = 1;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (chkSysTime.Checked)
                date = DateTime.Now;
            else
            {
                String time = String.Format("{0} {1}", dtPicker.Value.ToString("yyyy-MM-dd"), txtTime.Text);
                date = DateTime.Parse(time);
                Console.WriteLine("time = {0}", time);
            }
                
            SendVDSSetTimeData(date);
        }

        public void DisplayData(DataFrame request, DataFrame resposne)
        {
            DisplayDataFrame(request, lbxSend);
            DisplayDataFrame(resposne, lbxReceive);
        }

        public void DisplayDataFrame(DataFrame frame, ListBox list)
        {
            list.Items.Clear();
            String value;
            String temp;
            if(frame!=null)
            {
                value = String.Format($"시간: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")} ");
                list.Items.Add(value);


                value = String.Format("버젼: 0x{0:X2}", frame.Version);
                list.Items.Add(value);

                value = String.Format($"Addr: {Utility.ByteToString(frame.Addr)}({Utility.PrintHexaString(frame.Addr, frame.Addr.Length)}) ");
                list.Items.Add(value);

                temp = String.Format("0x{0:X2}", frame.OpCode);
                value = String.Format($"OP Code:{temp} ({Utility.PrintOPCodeName(frame.OpCode)}) ");
                list.Items.Add(value);

                value = String.Format($"Data Size: {frame.DataLength}");
                list.Items.Add(value);

                if (frame.opDataFrame is HistoricalTrafficRequest)
                    DisplayHistoricalRequest((HistoricalTrafficRequest)frame.opDataFrame, list);
                else if (frame.opDataFrame is HistoricalTrafficResponse)
                    DisplayHistoricalResponse((HistoricalTrafficResponse)frame.opDataFrame, list);

                else if(frame.opDataFrame is EchoBackRequest)
                    DisplayEchoBackRequest((EchoBackRequest)frame.opDataFrame, list);
                else if (frame.opDataFrame is EchoBackResponse)
                    DisplayEchoBackResponse((EchoBackResponse)frame.opDataFrame, list);

                else if (frame.opDataFrame is VDSStatusRequest)
                    DisplayVDSStatusRequest((VDSStatusRequest)frame.opDataFrame, list);
                else if (frame.opDataFrame is VDSStatusResponse)
                    DisplayVDSStatusResponse((VDSStatusResponse)frame.opDataFrame, list);

                else if (frame.opDataFrame is VDSStartRequest)
                    DisplayVDSStartRequest((VDSStartRequest)frame.opDataFrame, list);
                else if (frame.opDataFrame is VDSStartResponse)
                    DisplayVDSStartResponse((VDSStartResponse)frame.opDataFrame, list);

                else if (frame.opDataFrame is VDSSetTimeRequest)
                    DisplayVDSSetTimeRequest((VDSSetTimeRequest)frame.opDataFrame, list);
                else if (frame.opDataFrame is VDSSetTimeResponse)
                    DisplayVDSSetTimeResponse((VDSSetTimeResponse)frame.opDataFrame, list);

            }

        }

        public void DisplayHistoricalRequest(HistoricalTrafficRequest request, ListBox list)
        {
            String value;

            value = String.Format("****** Historical 교통 데이터 요청 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", request._OPCode);
            list.Items.Add(value);
        }

        public void DisplayHistoricalResponse(HistoricalTrafficResponse response, ListBox list)
        {
            String value;

            value = String.Format("****** Historical 교통 데이터 응답 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", response._OPCode);
            list.Items.Add(value);

            value = String.Format("데이터 건수: 0x{0:X2}", response.TrafficDataCount);
            list.Items.Add(value);

            foreach( var traffic in response.trafficDataList)
            {
                DateTime? checkTime = Utility.ByteToDate(traffic.checkTime);
                if (checkTime != null)
                    Console.WriteLine($"traffic data check time={checkTime?.ToString("yyy-MM-dd HH:mm:ss.ff")}");

            }

            //if (response.TrafficDataCount == 255)
            //{
            //    //Thread.Sleep(1000);
            //    SendHistoricalData();

            //}
        }

        public void DisplayEchoBackRequest(EchoBackRequest request, ListBox list)
        {
            String value;

            value = String.Format("****** EchoBack 요청 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", request._OPCode);
            list.Items.Add(value);
        }

        public void DisplayEchoBackResponse(EchoBackResponse response, ListBox list)
        {
            String value;

            value = String.Format("****** EchoBack 응답 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", response._OPCode);
            list.Items.Add(value);

        }


        public void DisplayVDSStatusRequest(VDSStatusRequest request, ListBox list)
        {
            String value;

            value = String.Format("****** VDSStatus 요청 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", request._OPCode);
            list.Items.Add(value);
        }

        public void DisplayVDSStatusResponse(VDSStatusResponse response, ListBox list)
        {
            String value;

            value = String.Format("****** VDSStatus 응답 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", response._OPCode);
            list.Items.Add(value);

            value = String.Format($"전송시각: {Utility.ByteToDate(response.checkTime)?.ToString("yyyy-MM-dd HH:mm:ss.ff")} ( {Utility.PrintHexaString(response.checkTime,8)} )");
            list.Items.Add(value);

            value = String.Format($"장비상태: ({Utility.PrintHexaString(response.VDSStatus, 2)})");
            list.Items.Add(value);

        }

        public void DisplayVDSStartRequest(VDSStartRequest request, ListBox list)
        {
            String value;

            value = String.Format("****** VDSStart 요청 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", request._OPCode);
            list.Items.Add(value);

            String temp = String.Empty;
            if (request.VDSFlag == 0x01)
                temp = "차량 검지 시작";
            else if (request.VDSFlag == 0x02)
                temp = "차량 검지 중지";

            value = String.Format("차량 검지 플래그: 0x{0:X2} {1} ", request.VDSFlag,temp);
            list.Items.Add(value);

            
        }

        public void DisplayVDSStartResponse(VDSStartResponse response, ListBox list)
        {
            String value;

            value = String.Format("****** VDSStart 응답 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", response._OPCode);
            list.Items.Add(value);

            String temp = String.Empty;
            if (response.VDSFlag == 0x01)
                temp = "차량 검지 시작";
            else if (response.VDSFlag == 0x02)
                temp = "차량 검지 중지";

            value = String.Format("차량 검지 플래그: 0x{0:X2} {1} ", response.VDSFlag, temp);
            list.Items.Add(value);

        }

        public void DisplayVDSSetTimeRequest(VDSSetTimeRequest request, ListBox list)
        {
            String value;

            value = String.Format("****** VDSSetTime 요청 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", request._OPCode);
            list.Items.Add(value);

            value = String.Format($"설정 시각: {Utility.ByteToDate(request.setTime)?.ToString("yyyy-MM-dd HH:mm:ss.ff")} ( {Utility.PrintHexaString(request.setTime, 8)} )");
            list.Items.Add(value);


        }

        public void DisplayVDSSetTimeResponse(VDSSetTimeResponse response, ListBox list)
        {
            String value;

            value = String.Format("****** VDSSetTime 응답 *******");
            list.Items.Add(value);
            value = String.Format("데이터 OP: 0x{0:X2}", response._OPCode);
            list.Items.Add(value);

            value = String.Format($"전송 시각: {Utility.ByteToDate(response.setTime)?.ToString("yyyy-MM-dd HH:mm:ss.ff")} ( {Utility.PrintHexaString(response.setTime, 8)} )");
            list.Items.Add(value);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //SetSendThread();
            bSyncTime = !bSyncTime;
            if (bSyncTime)
                btnAutoSync.Text = "자동 시간 동기화 중지";
            else
                btnAutoSync.Text = "자동 시간 동기화 시작";
        }


        public void SetSendThread()
        {
            if(bAutoSend) // 중지
            {
                bAutoSend = false;

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
                btnAutoSync.Text = "자동 전송 시작";
            }
            else         // 시작
            {
                bAutoSend = true;
                threads = new Thread[1];
                for(int i=0;i<1;i++)
                {
                    threads[i] = new Thread(new ParameterizedThreadStart(ThreadFunction));
                    threads[i].Start(i);
                }
                



                btnAutoSync.Text = "자동 전송 중지";

                
            }
        }

        public void SendHistoricalDataThread()
        {
            DataFrame reqFrame;
            DataFrame resFrame;
            while (bSendHistoricalData)
            {

                reqFrame = new DataFrame();
                reqFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                HistoricalTrafficRequest request = new HistoricalTrafficRequest();
                reqFrame.opDataFrame = request;
                if (SendData(ref reqFrame, out resFrame) > 0)
                {
                    if(resFrame!=null)
                    {
                        BeginInvoke(_displayDataDelegate, new object[] { reqFrame, resFrame });
                        var trafficResponse = (HistoricalTrafficResponse)resFrame.opDataFrame;
                        if (trafficResponse.TrafficDataCount < 255)
                        {
                            bSendHistoricalData = false;
                        }
                    }

                }

                Thread.Sleep(1000);
            }

            

        }

        public void ThreadFunction(object i)
        {
            int idx = (int)i;
            idx = 1;
            while(bAutoSend)
            {
                Console.WriteLine("thread func {0}",idx);
                switch(idx)
                {
                    case 0: // Historical
                        SendHistoricalData();
                        //SendVDSStatusData();
                        break;
                    case 1:
                        SendEchoBackData();
                        break;
                    case 2:
                        SendVDSStatusData();
                        break;
                    case 3:
                        SendVDSStartStopData(0x01);
                        break;
                    case 4:
                        SendVDSStartStopData(0x02);
                        break;
                    case 5:
                        DateTime date = DateTime.Now;
                        SendVDSSetTimeData(date);
                        break;
                }

                Thread.Sleep(500);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_kictEventServer._bListening)
            {
                _kictEventServer.StopManager();
                _Logger.StopManager();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SendHistoricalData();

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            DataFrame _prevDataFrame = null;
            try
            {
                // Read data from the client socket.
                int i = 0;
                int bytesRead = 30;
                byte[] packet = new byte[bytesRead];
                packet[i++] = 0x01;
                packet[i++] = 0x31;
                packet[i++] = 0x32;
                packet[i++] = 0x33;
                packet[i++] = 0x34;
                packet[i++] = 0x35;
                packet[i++] = 0x36;
                packet[i++] = 0x37;
                packet[i++] = 0x38;
                packet[i++] = 0xA1;
                packet[i++] = 0x00;
                packet[i++] = 0x12;
                packet[i++] = 0xB0;
                packet[i++] = 0x01;
                packet[i++] = 0x01;
                packet[i++] = 0x20;
                packet[i++] = 0x21;
                packet[i++] = 0x07;
                packet[i++] = 0x02;
                packet[i++] = 0x23;
                packet[i++] = 0x47;
                packet[i++] = 0x34;
                packet[i++] = 0xA0;
                packet[i++] = 0x15;
                packet[i++] = 0x41;
                packet[i++] = 0x02;
                packet[i++] = 0xEC;
                packet[i++] = 0x00;
                packet[i++] = 0x1F;
                packet[i++] = 0x00;
                if (bytesRead > 0)
                {

                    i = 0;
                    while (i < packet.Length)
                    {
                        if (_prevDataFrame == null)
                        {
                            _prevDataFrame = new DataFrame();
                        }

                        i += _prevDataFrame.Deserialize(packet, i);
                        if (_prevDataFrame.bDataCompleted)
                        {
                            WorkData workData = new WorkData();
                            DataFrameDefine.InitWorkData(ref workData);
                            workData.session = null;
                            workData.frame = _prevDataFrame;

                            ProcessTrafficEventData(workData);

                            _prevDataFrame = null;
                        }
                    }
                    

                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
        }

        private void ProcessAutoTimeSync()
        {
            var nowDate = DateTime.Now;
            int hour = int.Parse(txtPeriod.Text);

            // TimeSpan duration = new TimeSpan(0, 0, hour, 0, 0); //5초 후에 초기화
            TimeSpan duration = new TimeSpan(0, hour, 0, 0, 0); //5초 후에 초기화
            var nextSyncTime = _lastSyncTime.Add(duration); // 최종 동기화 이후 보낼 시간 설정 
            if(nowDate > nextSyncTime)
            {
                _lastSyncTime = nowDate;
                SendVDSSetTimeData(nowDate);
            }
        }
    }
}
