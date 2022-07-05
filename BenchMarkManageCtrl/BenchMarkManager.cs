using RadarManageCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.API.Model;
using VDSCommon.DataType;
using VDSDBHandler;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;
using VDSManagerCtrl;

namespace BenchMarkManageCtrl
{
    public class BenchMarkManager : VDSManager //, IVDSManager
    {

        

        public delegate int KictConnectCallback(SessionContext session, SOCKET_STATUS status);


        public VDSServer _calibServer = new VDSServer();

        /// <summary>
        /// 제어기-->안산 센터 연결 클라이언트
        /// </summary>
        public VDSClient _kictEventClient = new VDSClient();

        /// <summary>
        /// 센터에 Historical 교통 데이터 전송 여부
        /// </summary>
        public bool _historicalDataReport = false;

        public IVDSDevice _vdsDevice;

        

        public BenchMarkManager()
        {

        }

        public override int SetVDSDevice(IVDSDevice vdsDevice)
        {
            _vdsDevice = vdsDevice;
            _vdsDevice.SetAddTrafficDataEventDelegate(AddTrafficDataEvent);
            return 1;
        }

        public override int StartManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                
                _ctrlServer.SetAddress(VDSConfig.controllerConfig.IpAddress, VDSConfig.kictConfig.ctrlPort, CLIENT_TYPE.KICT_CTRL_CLIENT, AcceptCtrlCallback);
                _ctrlServer.StartManager();


                _calibServer.SetAddress(VDSConfig.controllerConfig.IpAddress, VDSConfig.kictConfig.calibPort, CLIENT_TYPE.KICT_CTRL_CLIENT, AcceptCalibCallback);
                _calibServer.StartManager();


                _kictEventClient.SetAddress(VDSConfig.kictConfig.centerAddress, VDSConfig.kictConfig.centerPort, CLIENT_TYPE.KICT_EVNT_CLIENT, KictEventClientConnectCallback, KictReadCallback, SendCallback);
                _kictEventClient.StartConnect();
                _vdsDevice.StartDevice(VDSConfig.controllerConfig.DeviceAddress, VDSConfig.controllerConfig.RemotePort, VDSConfig.controllerConfig.LocalPort);
                StartWorkThread();
                StartDBUpdateThread();
                _Logger.StartManager();

            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, "*******************안산센터 평가 서버 콘트롤러 시작*******************");
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return 1;
        }

        public override int StopManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            try
            {
                //_radarManager.StopManager();
                _vdsDevice.StopDevice();
                _ctrlServer.StopManager();
                _calibServer.StopManager();

                _kictEventClient.StopCheckConnection();
                DeleteAllSessionContext();
                StopWorkThread();
                StopDBUpdateThread();

                Utility.AddLog(LOG_TYPE.LOG_INFO, "*******************안산센터 평가 서버 콘트롤러 종료*******************");
                _Logger.StopManager();
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.StackTrace.ToString());
            }
            
            return 1;
        }

        public int StartWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            StartMAClient();
            StartProcessTrafficDataEventThread();
            StartProcessWorkDataThread();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        

        

        public int StopWorkThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            if (VDSConfig.USE_DB_THREAD)
            {
                if (_bProcessing)
                {
                    _bProcessing = false;

                    trafficEventThreadExitEvent.WaitOne();
                    threadExitEvent.WaitOne();

                    strLog = String.Format("안산센터 평가 서버 처리 쓰레드 종료 OK");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
            }
            StopMAClient();
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int StopDBUpdateThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            if (_bDBProcessing)
            {
                _bDBProcessing = false;
                dbThreadExitEvent.WaitOne();

                strLog = String.Format("DB Update 쓰레드 종료 OK");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
            
        }


        

        public override int ProcessWorkData(Object work)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리-GUID={((WorkData)(work)).guid} "));
            WorkData workData = (WorkData)work;
            int nResult = 0;
            switch (workData.session._type)
            {

                case CLIENT_TYPE.KICT_EVNT_CLIENT:
                    nResult = ProcessKICTEventClientData(workData);
                    break;

                case CLIENT_TYPE.KICT_CTRL_CLIENT:
                    nResult = ProcessKICTCtrlClientData(workData);
                    break;

                case CLIENT_TYPE.KICT_CLIB_CLIENT:
                    nResult = ProcessKICTCLIBClientData(workData);
                    break;

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessKICTEventClientData(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리- GUID={workData.guid} "));

            int nResult = 0;
            String strLog;
            

            DataFrame dataFrame = workData.frame;
            if(dataFrame.opDataFrame is TrafficResponse)
            {
                TrafficResponse res = (TrafficResponse)dataFrame.opDataFrame;

                strLog = String.Format($"교통 데이터 응답 Lane={res.lane} Direction={res.direction} checkTime={Utility.ByteToDate(res.checkTime)?.ToString("yyyy-MM-dd HH:mm:ss") } OK");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                nResult = RemoveWaitResList(workData);
                strLog = String.Format($"교통 데이터 요청 정보 {nResult}개 삭제 OK");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            else
            {
                strLog = String.Format("교통 데이터 응답 오류");
                Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessKICTCtrlClientData(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));

            int nResult = 0;
            DataFrame dataFrame = workData.frame;
            if (dataFrame.OpCode != DataFrameDefine.OP_FRAME_INITIAL_REQUEST &&
                dataFrame.OpCode != DataFrameDefine.OP_FRAME_RE_REQUEST)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"정보 요청 코드({dataFrame.OpCode}) 오류(전송{DataFrameDefine.OP_FRAME_INITIAL_REQUEST}/재전송{DataFrameDefine.OP_FRAME_RE_REQUEST} 요청 코드 아님)"));
                return nResult;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"정보 요청  OPCode={dataFrame.OpCode}: {Utility.PrintOPCodeName(dataFrame.OpCode)}"));
            //// Request 의 경우 응답 처리 
            if (dataFrame.opDataFrame is HistoricalTrafficRequest ) //Historical 데이터 요청
            {
                nResult = ProcessHistoricalTrafficRequest(workData);
            }
            else if (dataFrame.opDataFrame is VDSStartRequest)     // 차량검지 시작/정지 요청
            {
                nResult = ProcessVDSStartRequest(workData);
            }
            else if (dataFrame.opDataFrame is EchoBackRequest)   // echoback 요청
            {
                nResult = ProcessEchoBackRequest(workData);
            }
            else if (dataFrame.opDataFrame is VDSStatusRequest)    // 검지기 장비 상태 요청
            {
                nResult = ProcessVDSStatusRequest(workData);
            }
            else if (dataFrame.opDataFrame is VDSSetTimeRequest) // 검지기 시각 설정 요청
            {
                nResult = ProcessVDSSetTimeRequest(workData);
            }
            else
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"알수 없는 데이터 프레임 요청)"));
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        
        public int ProcessHistoricalTrafficRequest(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));

            int nResult = 0;
            String strLog;
            try
            {
                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                HistoricalTrafficRequest request = (HistoricalTrafficRequest)workData.frame.opDataFrame;
                HistoricalTrafficResponse response = new HistoricalTrafficResponse();
                GetHistoricalTrafficDataList(ref response.trafficDataList);

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Historical Traffic Data {response.trafficDataList.Count}건 조회 완료. 적용 ID={_lastTargetId} "));

                dataFrame.opDataFrame = response;
                byte[] data = dataFrame.Serialize();
                nResult = Send(workData.session, data);
                if(nResult > 0)
                {
                    if (response.trafficDataList.Count > 0)
                    {
                        strLog = String.Format($"VDSConfig.USE_DB_THREAD 값={VDSConfig.USE_DB_THREAD}");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                        //TODO REPORT_YN 을 Y 로 업데이트 한다. 
                        if (VDSConfig.USE_DB_THREAD)
                        {
                            var idList = from target in response.trafficDataList
                                         select target.id;
                            //UpdateTargetReportYN(idList.ToList(), "Y");
                            REPORT_INFO report = new REPORT_INFO();
                            report.ids = idList.ToList();
                            report.REPORT_YN = "Y";
                            AddDBUpdateQueue(report);

                        }
                        _lastTargetId = response.trafficDataList.Max(x => x.id);

                        strLog = String.Format("ProcessHistoricalTrafficRequest 최종 처리 ID={0} 세팅", _lastTargetId);
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    }
                    if(response.trafficDataList.Count < 255) // 0일수도 있다. 
                    {
                        _lastTargetId = String.Empty;// reset 한다. 
                        _historicalDataReport = true;
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Historical Traffic Data 전송 완료.최종 처리  ID={_lastTargetId}로 초기화"));
                    }
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                }
                

                strLog = String.Format("{0}\t 종료", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessVDSStartRequest(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));

            int nResult = 0;
            String strLog;
            byte[] data = null;
            try
            {
                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                VDSStartRequest request = (VDSStartRequest)workData.frame.opDataFrame;
                VDSStartResponse response = new VDSStartResponse();

                //VDSFlag :   0x01 : 시작, 0x02 : 정지
                strLog = String.Format($"검지기 시작/중지 요청.  VDSFlag={request.VDSFlag}");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                switch (request.VDSFlag)
                {
                    case 0x01: // 시작
                        if (!_vdsDevice.isService()) // 서비스가 아닐때만 시작
                        {
                            _vdsDevice.StartDevice(VDSConfig.controllerConfig.DeviceAddress, VDSConfig.controllerConfig.RemotePort, VDSConfig.controllerConfig.LocalPort);
                            strLog = String.Format("검지기 시작 요청 처리");
                            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        }
                        else
                        {
                            strLog = String.Format("검지기 서비스 중으로 시작 요청 처리 안함");
                            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        }
                        break;
                    case 0x02: // 정지
                        if (_vdsDevice.isService()) // 서비스 중일 때만 정지
                        {
                            _vdsDevice.StopDevice();
                            strLog = String.Format("검지기 중지 요청 처리");
                            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        }
                        else
                        {
                            strLog = String.Format("검지기 서비스 중지 중으로 중지 요청 처리 안함");
                            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        }
                        break;
                    default:
                        strLog = String.Format($"알수 없는 VDSFlag 값({request.VDSFlag})");
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                        break;
                }
                response.VDSFlag = request.VDSFlag;
                dataFrame.opDataFrame = response;
                data = dataFrame.Serialize();
                nResult = Send(workData.session, data);
                if(nResult <=0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                }

                strLog = String.Format("{0}\t 종료", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessEchoBackRequest(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;
            String strLog;
            try
            {
                strLog = String.Format("{0}\t 시작", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                EchoBackRequest request = (EchoBackRequest)workData.frame.opDataFrame;
                EchoBackResponse  response = new EchoBackResponse();
                Array.Copy(request.EchoData, 0, response.EchoData, 0, request.EchoData.Length);
                dataFrame.opDataFrame = response;

                byte[] data = dataFrame.Serialize();
                nResult = Send(workData.session, data);

                if (nResult <= 0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                }

                strLog = String.Format("{0}\t 종료", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                //int j = 0;
                //float i = 1 / j;
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessVDSStatusRequest(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;
            String strLog;
            try
            {
                strLog = String.Format("{0}\t 시작", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                VDSStatusRequest request = (VDSStatusRequest)workData.frame.opDataFrame;
                VDSStatusResponse response = new VDSStatusResponse();
                //TODO 응답 데이터 처리 로직 구현 
                nResult = CheckVDSStatus(ref response.VDSStatus, ref response.checkTime);

                //response.VDSStatus 

                dataFrame.opDataFrame = response;
                byte[] data = dataFrame.Serialize();
                nResult = Send(workData.session, data);
                if (nResult <= 0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                }

                strLog = String.Format("{0}\t 종료", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessVDSSetTimeRequest(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;
            String strLog;
            try
            {
                #region callback 버젼
                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                VDSSetTimeRequest request = (VDSSetTimeRequest)workData.frame.opDataFrame;
                VDSSetTimeResponse response = new VDSSetTimeResponse();



                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"검지기 시간({Utility.ByteToDate(request.setTime)?.ToString(VDSConfig.RADAR_TIME_FORMAT)}) 설정 요청"));

                // callback delegate 설정...
                nResult = _vdsDevice.SetDeviceTime(new ProcessRadarCallbackDelegate(ProcessRadarCallback), (Object)workData, Utility.ByteToDate(request.setTime));

                response.setTime = Utility.DateToByte(DateTime.Now);

                #endregion

                //                

                dataFrame.opDataFrame = response;
                byte[] data = dataFrame.Serialize();
                nResult = Send(workData.session, data);

                strLog = String.Format("{0}\t 종료", MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                Console.WriteLine("avogadrol...fsd.fsdfsdfsadfasdf");

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int ProcessKICTCLIBClientData(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        
        public void AcceptCtrlCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                //if (socket.Connected)
                {
                    KICTClient kictClient = new KICTClient();
                    kictClient._type = _ctrlServer._clientType;
                    kictClient._socket = socket;
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(KictReadCallback), kictClient);
                    AddSessionContext(kictClient);

                    strLog = String.Format($"안산 센터--> 제어기(Control)로 접속 추가. 사이즈={sessionList.Count}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            finally
            {
                _ctrlServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void AcceptCalibCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            try
            {
                // Create the state object.  
                Socket serverSocket = (Socket)ar.AsyncState;
                Socket socket = serverSocket.EndAccept(ar);
                //if (socket.Connected)
                {
                    KICTClient kictClient = new KICTClient();
                    kictClient._type = _ctrlServer._clientType;
                    kictClient._socket = socket;
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                        new AsyncCallback(KictReadCallback), kictClient);
                    AddSessionContext(kictClient);

                    strLog = String.Format($"안산 센터--> 제어기(Calibration)로 접속 추가. 사이즈={sessionList.Count}");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            finally
            {
                _calibServer.SetAcceptProcessEvent();
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }
        

        public void KictReadCallback(IAsyncResult ar)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

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
                if (bytesRead > 0)
                {
                    strLog = String.Format("안산 센터--> 제어기 ReadCallback {0} 바이트 데이터 수신",bytesRead);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                    //if(kictClient._prevDataFrame == null)
                    //{
                    //    kictClient._prevDataFrame = new DataFrame();
                    //}
                    byte[] packet = new byte[bytesRead];
                    Array.Copy(kictClient.buffer, 0, packet, 0, bytesRead);
                    int i = 0;
                    while(i<packet.Length)
                    {
                        if (kictClient._prevDataFrame == null)
                        {
                            kictClient._prevDataFrame = new DataFrame();
                        }

                        i += kictClient._prevDataFrame.Deserialize(packet,i);
                        if(kictClient._prevDataFrame.bDataCompleted)
                        {
                            WorkData workData = new WorkData();
                            DataFrameDefine.InitWorkData(ref workData);
                            workData.session = kictClient;
                            workData.frame = kictClient._prevDataFrame;
                            AddWorkData(workData);

                            kictClient._prevDataFrame = null;
                        }
                        else
                        {
                            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"DataFrame 패킷 미완성 i={i}, packet.Length={packet.Length}"));
                        }
                    }
                    // Not all data received. Get more.  
                    socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KictReadCallback), kictClient);

                }
                else
                {
                    if (socket != null && kictClient._type!= CLIENT_TYPE.KICT_EVNT_CLIENT)
                    {
                        strLog = String.Format("안산센터-> 제어기 연결 종료");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        //Console.WriteLine("22Close socket");
                        //socket.Shutdown(SocketShutdown.Both);
                        //socket.Close();
                        DeleteSessionContext(kictClient);
                    }
                    else if(socket != null && kictClient._type == CLIENT_TYPE.KICT_EVNT_CLIENT)
                    {
                        Console.WriteLine("event client error");
                    }
                }
            }
            catch (Exception ex)
            {
                if (socket != null && kictClient._type != CLIENT_TYPE.KICT_EVNT_CLIENT)
                {
                    strLog = String.Format("원격 클라이언트 연결 종료");
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                    //Console.WriteLine("22Close socket");
                    //socket.Shutdown(SocketShutdown.Both);
                    //socket.Close();
                    DeleteSessionContext(kictClient);
                }
                else if (kictClient._type == CLIENT_TYPE.KICT_EVNT_CLIENT)
                {
                    Console.WriteLine("event client error");
                }
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private int Send(SessionContext session, byte[] byteData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 1;
            String strLog;
            try
            {
                // Begin sending the data to the remote device.  
                session._socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), session._socket);
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

                DeleteSessionContext(session);
                nResult = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private void SendCallback(IAsyncResult ar)
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


        #region VDSClient 처리 callback

        private int KictEventClientConnectCallback(SessionContext session, SOCKET_STATUS status)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                KICTClient kictClient = new KICTClient();
                kictClient._type = session._type;
                kictClient._socket = session._socket;
                kictClient._socket.BeginReceive(kictClient.buffer, 0, SessionContext.BufferSize, 0,
                    new AsyncCallback(KictReadCallback), kictClient);
                
                //_kictEventClient._sessionContext = kictClient;
                //_kictEventClient._status = SOCKET_STATUS.CONNECTED;

                strLog = String.Format("제어기-->안산 센터({0}:{1}  접속 성공)", VDSConfig.kictConfig.centerAddress, VDSConfig.kictConfig.centerPort);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                nResult = 1;
            }
            catch (Exception ex)
            {
                _kictEventClient._status = SOCKET_STATUS.DISCONNECTED;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }



        #endregion


        

        


        
        //public int GetTargetSummaryInfo()
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    return _radarManager.GetTargetSummaryInfo();
        //}

        //public int StopTargetSummaryInfoStreaming()
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    return _radarManager.StopTargetSummaryInfoStreaming();
        //}


        


        

        public int CheckVDSStatus(ref byte[] status, ref byte[] checkTime)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                nResult = _vdsDevice.CheckVDSStatus(ref status, ref checkTime);
            }
            catch(Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public int GetHistoricalTrafficDataList(ref List<TrafficData> trafficList )
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
                var targetList = db.GetTrafficDataList(new TRAFFIC_DATA()
                {
                    I_START_DATE = String.Empty ,
                    I_END_DATE = String.Empty ,
                    I_REPORT_YN = "N" ,
                    I_LIMIT_COUNT = 255
                }, out SP_RESULT spResult);


                if (!spResult.IS_SUCCESS)
                {

                    Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                }
                else
                {
                    nResult = targetList.Count();
                    strLog = String.Format("GetHistoricalTrafficDataList {0} 타겟 조회 완료", nResult);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }


                if (targetList.Count() > 0)
                {
                    
                    var trafficData = from target in targetList
                                      select new TrafficData()
                                      {
                                          id = target.ID,
                                          checkTime = Utility.DateToByte(target.DETECT_TIME), //Utility.DateToByte(target.REG_DATE),
                                          lane = (byte)target.LANE,
                                          direction = (byte) target.DIRECTION,
                                          velocity = target.SPEED,
                                          occupyTime = (UInt16) target.OCCUPY_TIME,
                                          carLength = (UInt16)target.LENGTH

                                      };
                    trafficList = trafficData.ToList();
                }
                strLog = String.Format("GetHistoricalTrafficDataList 종료");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        

        public override int SendTrafficData(TrafficDataEvent trafficDataEvent)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            DataFrame dataFrame ;
            TrafficRequest trafficDataRequest ;
            try
            {
                // 안산 센터에 전송...
                if (_kictEventClient._status == SOCKET_STATUS.CONNECTED)
                {
                    dataFrame = new DataFrame();
                    trafficDataRequest = new TrafficRequest();
                
                    dataFrame.OpCode = DataFrameDefine.OP_FRAME_INITIAL_REQUEST;
                    trafficDataRequest.trafficData = new TrafficData()
                    {

                        id = trafficDataEvent.id,
                        checkTime = Utility.DateToByte(trafficDataEvent.detectTime),
                        lane = trafficDataEvent.lane,
                        direction = trafficDataEvent.direction,
                        velocity = trafficDataEvent.speed, // km/h
                        occupyTime = (UInt16)trafficDataEvent.occupyTime,
                        carLength = (UInt16)trafficDataEvent.length
                    };
                    dataFrame.opDataFrame = trafficDataRequest;
                    WorkData workData = new WorkData();
                    DataFrameDefine.InitWorkData(ref workData);

                    workData.session = _kictEventClient._sessionContext;
                    workData.frame = dataFrame;

                    // WorkData Send 
                    nResult = SendWorkData(ref workData);
                    if (nResult > 0)
                    {
                        nResult = AddWaitResList(workData);
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"안산센터로 실시간 교통 데이터 전송 성공. 응답 대기 리스트에 추가.nResult={nResult}"));
                    }
                    else
                    {
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                        //avogadro _kictEventClient._status = SOCKET_STATUS.DISCONNECTED;
                    }
                    
                }
                else
                {
                    strLog = String.Format("센터에 연결되어 있지 않음({0}:{1})", _kictEventClient._address, _kictEventClient._port);
                    Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }



            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SendWorkData(ref WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            String strLog;
            int nResult = 0;
            
            try
            {
                byte[] data = workData.frame.Serialize();
                nResult = Send(workData.session, data);

                SetRetryInfo(ref workData);

                strLog = String.Format($"전송 대기 횟수: {workData.sleepCount} 전송 횟수:{workData.sendCount} 다음 전송 시간:{workData.nextSendTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        

        public override int ProcessWaitResList()
        {
            int nResult = 0;
            List<WorkData> delList = new List<WorkData>();
            if (waitResList.Count == 0)
                return 0;
            DateTime current = DateTime.Now;
            lock (_waitResLock)
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"응답 대기 리스트 처리. 대기리스트 사이즈={waitResList.Count}"));

                for (int i = 0;i<waitResList.Count;i++)
                {
                    var workData = (WorkData)waitResList[i];

                    //Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"응답 대기 리스트 전송 예정시간={workData.nextSendTime.ToString(VDSConfig.RADAR_TIME_FORMAT)}, 현재 시간={current.ToString(VDSConfig.RADAR_TIME_FORMAT)}"));
                    
                    if (workData.nextSendTime < current && workData.sleepCount ==0)
                    {
                    
                       Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"전송 예정시간 초과로 재전송.."));
                       SendWorkData(ref workData);
                    }
                    else if(workData.sleepCount > 0)
                    {
                        delList.Add(workData);
                    }
                    waitResList[i] = workData;
                }

                if(delList.Count > 0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"응답 대기 리스트 처리. 재전송 횟수 초과 정보 삭제(갯수={delList.Count}"));

                    foreach (var item in delList)
                    {
                        waitResList.Remove(item);
                    }

                }
            }
            return nResult;
        }

        public int SetRetryInfo(ref WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;
            TimeSpan duration;

            if (workData.frame.OpCode == DataFrameDefine.OP_FRAME_INITIAL_REQUEST)
                workData.frame.OpCode = DataFrameDefine.OP_FRAME_RE_REQUEST;

            workData.lastSendTime = DateTime.Now;
            workData.sendCount++;
            if (workData.sendCount < VDSConfig.kictConfig.retryCount) //  1
            {
                duration = new TimeSpan(0, 0, 0, VDSConfig.kictConfig.responseTimeout  , 0); //3초 후에 재전송
                workData.nextSendTime = workData.lastSendTime.Add(duration);

            }
            else  //Next Sleep 5분 후에..
            {
                duration = new TimeSpan(0, 0, 0, VDSConfig.kictConfig.nextRetryTime, 0); //5분후에 재시도
                workData.nextSendTime = workData.lastSendTime.Add(duration);
                workData.sleepCount++;
                workData.sendCount = 0;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        public int RemoveWaitResList(WorkData workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            String strLog;
            List<WorkData> delList = new List<WorkData>();
            int nResult = 0;
            DateTime current = DateTime.Now;
            
            byte[] data = workData.frame.opDataFrame.Serialize();
            lock (_waitResLock)
            {
                for(int i = 0;i<waitResList.Count;i++)
                {
                    var target = (WorkData)waitResList[i];

                    if(target.frame.opDataFrame is TrafficRequest && workData.frame.opDataFrame is TrafficResponse)
                    {
                        TrafficRequest req = (TrafficRequest)target.frame.opDataFrame;
                        TrafficResponse res = (TrafficResponse)workData.frame.opDataFrame;

                        strLog = String.Format($"요청데이터({i+1}/{waitResList.Count}) Lane={req.trafficData.lane} Direction={req.trafficData.direction} checkTime={Utility.ByteToDate(req.trafficData.checkTime)?.ToString("yyyy-MM-dd HH:mm:ss.ff") }" );
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                        strLog = String.Format($"응답데이터 Lane={res.lane} Direction={res.direction} checkTime={Utility.ByteToDate(res.checkTime)?.ToString("yyyy-MM-dd HH:mm:ss.ff") }");
                        Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                        if (req.trafficData.lane == res.lane && 
                           req.trafficData.direction == res.direction
                            && Utility.ByteToDate(req.trafficData.checkTime) == Utility.ByteToDate(res.checkTime))
                        {
                            delList.Add(target);
                            nResult++;
                            strLog = String.Format($"응답데이터 Lane={res.lane} Direction={res.direction} checkTime={Utility.ByteToDate(res.checkTime)?.ToString("yyyy-MM-dd HH:mm:ss") } 업데이트 리스트({delList.Count} 개) 추가 ");
                            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                        }
                    }
                    else
                    {
                        strLog = String.Format($"교통 데이터 요청/응답 메시지 아님");
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, strLog);
                    }
                }
                strLog = String.Format($"요청 데이터에 대한 응답{delList.Count}개 처리");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
                if (delList.Count>0)
                {
                    foreach (var item in delList)
                    {
                        if(VDSConfig.USE_DB_THREAD)
                        {
                            REPORT_INFO report = new REPORT_INFO();
                            report.ids = new List<String>();
                            report.ids.Add( (item.frame.opDataFrame as TrafficRequest).trafficData.id);
                            report.REPORT_YN = "Y";
                            AddDBUpdateQueue(report);
                        }
                        waitResList.Remove(item);
                    }
                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }

        

        public int ProcessRadarCallback(Object [] _params)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {
                if(_params[0] is WorkData)
                {
                    WorkData workData = (WorkData)_params[0];
                    if (workData.frame.opDataFrame is VDSSetTimeRequest) 
                    {
                        DateTime radarDate = (DateTime)_params[1];
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"레이더에 설정된 시간={radarDate.ToString(VDSConfig.RADAR_TIME_FORMAT)}"));
                        nResult = SetRadarTimeResponse(workData,radarDate);
                    }
                    else
                    {
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"처리할 수 있는 데이터 프레임(VDSSetTimeRequest) 아님"));
                    }
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"처리할수 없는 Object로 WorkData 타입이 아님"));
                }

                
            }
            catch(Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult; 
        }

        public int SetRadarTimeResponse(WorkData workData, DateTime radarDate)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 - GUID={workData.guid} "));
            int nResult = 0;
            try
            {
                DataFrame dataFrame = new DataFrame(workData.frame.OpCode);
                VDSSetTimeRequest request = (VDSSetTimeRequest)workData.frame.opDataFrame;
                VDSSetTimeResponse response = new VDSSetTimeResponse();
                //TODO 응답 데이터 처리 로직 구현 
                response.setTime = Utility.DateToByte(radarDate);

                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"레이더 설정된 시간={radarDate.ToString(VDSConfig.RADAR_TIME_FORMAT)} 응답 "));

                dataFrame.opDataFrame = response;
                byte[] data = dataFrame.Serialize();
                nResult = Send(workData.session, data);
                if(nResult <=0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, String.Format($"안산 센터 연결 오류로 데이터 전송 실패"));
                }

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 - GUID={workData.guid} "));
            return nResult;
        }
    }
}
