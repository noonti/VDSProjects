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

namespace VDSManagerCtrl
{
    public class VDSManager : IVDSManager
    { 
        public VDSServer _ctrlServer = new VDSServer();
        public object _sessionLock = new object();
        public List<SessionContext> sessionList = new List<SessionContext>();

        public bool _bProcessing;

        public ManualResetEvent trafficEventThreadExitEvent = new ManualResetEvent(false);
        public ManualResetEvent threadExitEvent = new ManualResetEvent(false);

        public VDSLogger _Logger = new VDSLogger();

        public Control _control = null;
        public FormAddTargetInfoDelegate _formAddTargetInfo = null;

        public Queue<TrafficDataEvent> trafficDataEventQueue = new Queue<TrafficDataEvent>();
        public object _trafficDataEventLock = new object();
        //public RadarManager _radarManager = new RadarManager();

        public bool _bDBProcessing = false;
        public ManualResetEvent dbThreadExitEvent = new ManualResetEvent(false);

        public object _dbLock = new object();
        public Queue<REPORT_INFO> dbUpdateQueue = new Queue<REPORT_INFO>();

        public object _workDataLock = new object();
        public Queue<Object> workDataQueue = new Queue<Object>();


        public object _waitResLock = new object();
        public List<Object> waitResList = new List<Object>();

        //public int _centerPeriod = 30; // 센터 전송 주기 (기본30초)
        //public int _localPeriod = 30;  // 현장 전송 주기 (기본30초)

        public object _centerDataLock = new object();
        public List<Object>[] _centerData = new List<Object>[2];
        public int _activeCenterDataIndex = 0;
        public DateTime _lastResetCenterData;

        public object _localDataLock = new object();
        public List<Object>[] _localData = new List<Object>[2];
        public int _activeLocalDataIndex = 0;
        public DateTime _lastResetLocalData;
        /// <summary>
        /// 센터 Historical 교통 데이터 전송한 최근 데이터 ID
        /// </summary>
        public String _lastTargetId = String.Empty;


        public MAClient maClient; //유지보수 클라이언트 


        public virtual int ProcessWorkData(Object workData)
        {
            return 1;
        }


        public virtual int SetVDSDevice(IVDSDevice device)
        {
            return 1;
        }


        public VDSManager()
        {
            trafficEventThreadExitEvent.Reset();
            threadExitEvent.Reset();
            dbThreadExitEvent.Reset();
            _Logger.SetManagerType(MANAGER_TYPE.VDS_SERVER);
            _bProcessing = false;
            _bDBProcessing = false;
            Utility._addLog = _Logger.AddLog;

            for(int i=0;i<2;i++)
            {
                _centerData[i] = new List<Object>();
                _localData[i] = new List<Object>();
            }
            _activeCenterDataIndex = 0;
            _activeLocalDataIndex = 0;
            //_radarManager.SetAddRadarDataDelegate(AddRadarData);

            _lastTargetId = String.Empty; // 
        }

        
        public int SetFormAddLogDelegate(Control control, FormAddLogDelegate addLogDelegate)
        {
            _Logger._formAddLog = addLogDelegate;
            _Logger._control = control;
            return 1;
        }



        public int SetFormAddTargetInfoDelegate(Control control, FormAddTargetInfoDelegate addTargetInfoDelegate)
        {
            _formAddTargetInfo = addTargetInfoDelegate;
            if (_control == null)
                _control = control;
            return 1;
        }


        public int AddTrafficDataEvent(TrafficDataEvent trafficDataEvent)
        {
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            lock (_trafficDataEventLock)
            {
                trafficDataEventQueue.Enqueue(trafficDataEvent);
                nResult = trafficDataEventQueue.Count;
            }
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int AddDBUpdateQueue(REPORT_INFO report)
        {
            int nResult = 0;
            lock (_dbLock)
            {
                dbUpdateQueue.Enqueue(report);
                nResult = dbUpdateQueue.Count;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, $"DB 업데이트 큐에 추가. 사이즈={nResult}");
            return nResult;
        }


        public int DeleteAllSessionContext()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            try
            {

                lock (_sessionLock)
                {
                    foreach (var session in sessionList)
                    {
                        session._socket.Shutdown(SocketShutdown.Both);
                        session._socket.Close();
                    }
                    sessionList.Clear();
                }
            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        //public int GetRadarInfo()
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
        //    return _radarManager.GetSysInfo();
        //}


        public void StartProcessTrafficDataEventThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            new Thread(() =>
            {
                try
                {
                    _bProcessing = true;
                    while (_bProcessing)
                    {
                        ProcessTrafficDataEventQueue();
                        Thread.Sleep(100);
                    }
                    trafficEventThreadExitEvent.Set();

                }
                catch (Exception ex)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                }
            }
          ).Start();
          Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }


        public void StartProcessWorkDataThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            new Thread(() =>
            {
                try
                {
                    _bProcessing = true;
                    while (_bProcessing)
                    {
                        ProcessWorkDataQueue();
                        ProcessWaitResList();
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
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public int StartDBUpdateThread()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (VDSConfig.USE_DB_THREAD)
            {
                new Thread(() =>
                {
                    try
                    {
                        _bDBProcessing = true;
                        while (_bProcessing)
                        {
                            ProcessDBUpdateQueue();
                            Thread.Sleep(100);
                        }
                        dbThreadExitEvent.Set();

                    }
                    catch (Exception ex)
                    {
                        Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
                    }
                }
                ).Start();
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        //public int SetTargetSummaryStreaming(bool enable)
        //{
        //    return _radarManager.SetTargetSummaryStreaming(enable);
        //}


        //public int SetRadarTime(DateTime date)
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
        //    DateTime deviceTime = new DateTime();
        //    return 1;
        //    //return _radarManager.SetRadarTime(date, ref deviceTime);
        //    //return _radarManager.SetRadarTime(new ProcessRadarCallbackDelegate(ProcessRadarCallback), (Object)workData, Utility.ByteToDate(request.setTime));
        //}


        public int ProcessTrafficDataEventQueue()
        {
            int nResult = 0;
            TrafficDataEvent trafficDataEvent;
            while (trafficDataEventQueue.Count > 0)
            {
                lock (_trafficDataEventLock)
                {
                    trafficDataEvent = trafficDataEventQueue.Dequeue();
                }

                ProcessTrafficDataEvent(trafficDataEvent);
            }
            return nResult;
        }

        public int ProcessTrafficDataEvent(TrafficDataEvent trafficDataEvent)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            SP_RESULT spResult;
            try
            {
                strLog = String.Format($"실시간 검지 정보 ID={trafficDataEvent.id},LANE={trafficDataEvent.lane}, DIRECTION={trafficDataEvent.direction}, LENGTH={trafficDataEvent.length}, SPEED={trafficDataEvent.speed}"
                        + $" CLASS = {trafficDataEvent.vehicle_class}, OCCUPY_TIME ={trafficDataEvent.occupyTime }, LOOP1={trafficDataEvent.loop1OccupyTime},LOOP2={trafficDataEvent.loop2OccupyTime} "
                        + $" REVERSEYN={trafficDataEvent.reverseRunYN }, VEHICLE_GAP ={trafficDataEvent.vehicleGap}, DETECT_TIME = {trafficDataEvent.detectTime} ");

                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                TrafficDataOperation trafficDB = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
                TRAFFIC_DATA trafficData = new TRAFFIC_DATA()
                {
                    ID = trafficDataEvent.id,
                    VDS_TYPE = trafficDataEvent.vds_type,
                    CONTROLLER_ID =  VDSConfig.controllerConfig.ControllerId,
                    LANE = trafficDataEvent.lane,
                    DIRECTION = trafficDataEvent.direction,
                    LENGTH = trafficDataEvent.length,
                    SPEED = trafficDataEvent.speed,
                    VEHICLE_CLASS = trafficDataEvent.vehicle_class,
                    OCCUPY_TIME = trafficDataEvent.occupyTime,
                    LOOP1_OCCUPY_TIME = trafficDataEvent.loop1OccupyTime,
                    LOOP2_OCCUPY_TIME = trafficDataEvent.loop2OccupyTime,
                    REVERSE_RUN_YN = trafficDataEvent.reverseRunYN,
                    VEHICLE_GAP = trafficDataEvent.vehicleGap,
                    DETECT_TIME = trafficDataEvent.detectTime,
                    REPORT_YN = trafficDataEvent.reportYN,

                };
                trafficDB.AddTrafficData(trafficData, out spResult);
                if (!spResult.IS_SUCCESS)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                }
                else
                {
                    nResult = 1;
                }

                // MA Server 에 전송...
                SendTrafficDataToMAServer(trafficDataEvent);

                nResult = SendTrafficData(trafficDataEvent);
                AddTrafficDataEventToForm(trafficDataEvent);
            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        

        public int AddTrafficDataEventToForm(TrafficDataEvent trafficDataEvent)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (_control != null && _formAddTargetInfo != null)
            {
                _control.BeginInvoke(_formAddTargetInfo, new object[] { trafficDataEvent });
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }

        public int ProcessDBUpdateQueue()
        {
            int nResult = 0;
            REPORT_INFO report;
            TrafficDataOperation trafficDB = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            while (dbUpdateQueue.Count > 0)
            {
                lock (_dbLock)
                {
                    report = dbUpdateQueue.Dequeue();
                }

                ProcessReportInfo(report, trafficDB);
            }
            return nResult;
        }


        public int ProcessReportInfo(REPORT_INFO report, TrafficDataOperation db)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            SP_RESULT spResult;
            try
            {
                if (report.ids.Count == 0)
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"안산센터 통보 여부 업데이트할 Id List 사이즈가 0 입니다"));
                    return 0;
                }

                db.UpdateTrafficDataReportYN(Utility.MakeCSVData(report.ids), report.REPORT_YN, out spResult);
                if (!spResult.IS_SUCCESS)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                }
                else
                {
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"안산센터 통보 {spResult.RESULT_COUNT} 개 업데이트 성공"));
                    nResult = 1;
                }

            }
            catch (Exception ex)
            {
                nResult = 0;
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int AddSessionContext(SessionContext session)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            lock (_sessionLock)
            {
                sessionList.Add(session);
                nResult = sessionList.Count;
                //Console.WriteLine("session count={0}", nResult);
                strLog = String.Format("접속 클라이언트 추가. Size={0}", nResult);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int DeleteSessionContext(SessionContext session)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;
            try
            {
                session._socket.Shutdown(SocketShutdown.Both);
                session._socket.Close();

            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            lock (_sessionLock)
            {

                sessionList.Remove(session);
                nResult = sessionList.Count;
                strLog = String.Format("접속 클라이언트 삭제. Size={0}", nResult);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

       

        public int UpdateTargetReportYN(List<String> idList, String reportYN)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            String strLog;

            if (idList.Count == 0)
                return 0;
            try
            {
                TargetSummary db = new TargetSummary(VDSConfig.VDS_DB_CONN);
                db.UpdateTargetReportYN(Utility.MakeCSVData(idList), reportYN, out SP_RESULT spResult);
                if (!spResult.IS_SUCCESS)
                {
                    Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                }
                else
                    nResult = spResult.RESULT_COUNT;

                strLog = String.Format("{0}개 타겟 ID를 {1}로 업데이트", spResult.RESULT_COUNT, reportYN);
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);

                strLog = String.Format("UpdateTargetReportYN 종료");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);


            }
            catch (Exception ex)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, ex.Message.ToString() + "\n" + ex.StackTrace.ToString());
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        public int AddWorkData(Object workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            lock (_workDataLock)
            {
                workDataQueue.Enqueue(workData);
                nResult = workDataQueue.Count;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;

        }

        public int AddWaitResList(Object workData)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리"));
            int nResult = 0;
            lock (_waitResLock)
            {
                waitResList.Add(workData);
                nResult = waitResList.Count;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"응답 대기 리스트 추가. 대기리스트 사이즈={nResult}"));

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }

        public int ProcessWorkDataQueue()
        {
            int nResult = 0;
            Object workData;
            while (workDataQueue.Count > 0)
            {
                lock (_workDataLock)
                {
                    workData = workDataQueue.Dequeue();
                }

                ProcessWorkData(workData);
            }
            return nResult;
        }

        


        public virtual int ProcessWaitResList()
        {
            return 1;
        }

        public virtual int StartManager()
        {
            return 1;
        }

        public virtual int StopManager()
        {
            return 1;
        }

        public virtual int SendTrafficData(TrafficDataEvent target)
        {
            return 1;
        }

        public virtual int AddTrafficDataStat(TrafficDataEvent target)
        {
            return 1;
        }


        
        

        public int AddCenterData(Object data)
        {
            int result = 0;
            lock (_centerDataLock)
            {
                _centerData[_activeCenterDataIndex].Add(data);
                result = _centerData[_activeCenterDataIndex].Count();
            }
            AddTrafficDataStat(data as TrafficDataEvent);
            return result;
        }

        public int AddLocalData(Object data)
        {
            int result = 0;
            lock (_localDataLock)
            {
                _localData[_activeLocalDataIndex].Add(data);
                result = _localData[_activeLocalDataIndex].Count();


            }
            return result;
        }

        public int PrepareCenterData()
        {
            int result = 0;
            lock (_centerDataLock)
            {
                Console.WriteLine($"_activeCenterDataIndex={_activeCenterDataIndex} ,_centerData[0].Count()={_centerData[0].Count()}  _centerData[1].Count()={_centerData[1].Count()}");
                _activeCenterDataIndex = (++_activeCenterDataIndex) % 2;
                _centerData[_activeCenterDataIndex].Clear();
                result = _centerData[_activeCenterDataIndex].Count();
                _lastResetCenterData = DateTime.Now;

            }
            return result;

        }

        public int ResetLocalData()
        {
            int result = 0;
            lock (_localDataLock)
            {
                Console.WriteLine($"_activeLocalDataIndex={_activeLocalDataIndex} ,_localData[0].Count()={_localData[0].Count()}  _localData[1].Count()={_localData[1].Count()}");
                _activeLocalDataIndex = (++_activeLocalDataIndex) % 2;
                _localData[_activeLocalDataIndex].Clear();
                result = _localData[_activeLocalDataIndex].Count();
                _lastResetLocalData = DateTime.Now;
            }
            return result;

        }


        public int StartMAClient()
        {
            if(maClient!=null)
            {
                maClient.StopService();
            }else
            {
                maClient = new MAClient();
            }
            return maClient.StartService();
        }

        public int StopMAClient()
        {
            if (maClient != null)
                maClient.StopService();
            return 1;
        }

        public int SendTrafficDataToMAServer(TrafficDataEvent target)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            if (maClient != null)
            {
                maClient.SendTrafficData(target);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료"));
            return nResult;
        }
    }
}
