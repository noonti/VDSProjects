using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.DataType;

namespace VDSCommon
{

    public enum MANAGER_TYPE
    {
        VDS_RADAR = 1, // RADAR 제어기
        VDS_MONITOR = 2, // VDS 모니터
        VDS_CLIENT = 3,
        VDS_SERVER = 4 ,
        VDS_ADMIN = 5
    }


    public enum LOG_TYPE
    {
        LOG_INFO = 1,
        LOG_WARN = 2,
        LOG_ERROR = 3
    }
    
    public class VDSLogger : IVDSManager
    {

        public MANAGER_TYPE _mangerType { get; set; }
        public String _managerName { get; set; }
        public String _path { get; set; }
        public LOG_TYPE _logType { get; set; }
        public bool _bLogging { get; set; }
        public ManualResetEvent threadExitEvent = new ManualResetEvent(false);
        Queue<VDSLog> _queue = new Queue<VDSLog>();

        public FormAddLogDelegate _formAddLog = null;
        public Control _control = null;
        // lock object 
        private object _lockQueue = new object();
        public VDSLogger()
        {
            _bLogging = false;
            threadExitEvent.Reset();
        }

        public int SetManagerType(MANAGER_TYPE managerType)
        {
            _mangerType = managerType;
            switch (_mangerType)
            {
                case MANAGER_TYPE.VDS_RADAR:
                    _managerName = "VDS_RADAR";
                    break;

                case MANAGER_TYPE.VDS_MONITOR:
                    _managerName = "VDS_MONITOR";
                    break;

                case MANAGER_TYPE.VDS_CLIENT:
                    _managerName = "VDS_CLIENT";
                    break;

                case MANAGER_TYPE.VDS_SERVER:
                    _managerName = "VDS_SERVER";
                    break;
                case MANAGER_TYPE.VDS_ADMIN:
                    _managerName = "VDS_ADMIN";
                    break;
            }

            _path = Utility.GetLogPath();
            if(!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }

            _path = System.IO.Path.Combine(Utility.GetApplicationPath(), "TrafficEvent");
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }

            return 1;
        }

        public int AddLog(LOG_TYPE type, String _log)
        {
            int nResult = 0;
            VDSLog log = new VDSLog();
            try
            {
                log._logType = type;
                log._log = _log;
                lock (_lockQueue)
                {
                    _queue.Enqueue(log);
                    nResult = _queue.Count;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return nResult;
        }

        public String GetLogFilePath()
        {
            String result = System.IO.Path.Combine(Utility.GetLogPath(), _managerName + ".log");
            return result;
        }

        public int ProcessLog()
        {
            //log 파일 open write close 
            VDSLog log;
            String buf;
            int nResult = 0;
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetLogFilePath(), true))
                {

                    while (_queue.Count() > 0)
                    {
                        lock (_lockQueue)
                        {
                            log = _queue.Dequeue();
                        }
                        buf = String.Format("{0}\t[{1}]\t{2}",log._logTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), log._logType,log._log);
                        if (_formAddLog != null)
                        {
                           _control.BeginInvoke( _formAddLog, new object[] { log._logType, buf });
                        }
                        //Console.WriteLine(buf);
                        file.WriteLine(buf);
                        nResult++;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return nResult;
        }


        public int StartManager()
        {
            String logDate = String.Empty;
            String curDate = String.Empty;
            if(_bLogging) // 아직 logging 인경우 stop 시킨다
            {
                StopManager();

            }

            logDate = DateTime.Now.ToString("yyyyMMdd");
            new Thread(() =>
                {
                    _bLogging = true;
                    String logFileName;
                    while (_bLogging)
                    {
                        curDate = DateTime.Now.ToString("yyyyMMdd");
                        if(logDate.CompareTo(curDate) != 0) // 현재 날짜와 로깅 한 날짜가 다를 경우..
                        {
                            // 현재 로깅 파일 백업 
                            logFileName = GetLogFilePath();
                            if (System.IO.File.Exists(logFileName))
                            {
                                String destFileName = System.IO.Path.Combine(Utility.GetLogPath(), _managerName + String.Format("_{0}.log", logDate));
                                if(!System.IO.File.Exists(destFileName)) // 기존 파일 없으면 Move logFileName
                                    System.IO.File.Move(logFileName, destFileName);
                                else                                    // 기존 파일 있으면 overwrite
                                    System.IO.File.Copy(logFileName, destFileName,true);
                            }
                            logDate = curDate;
                        }
                        // 날짜 체크하여 로그 파일 재생성 여부 판단 
                        ProcessLog();
                        Thread.Sleep(100);
                    }
                    threadExitEvent.Set();
                    Console.WriteLine("log....threadExitEvent.Set();");
                }
            ).Start();
            return 1;
        }

        public int StopManager()
        {
            if(_bLogging)
            {
                _bLogging = false;
                threadExitEvent.WaitOne();
                // 남아있는 로그 정리 
                ProcessLog();
            }
            
            return 1;
        }
        //public int SendTrafficData(TargetSummaryInfo target)
        //{
        //    return 1;
        //}
    }
}
