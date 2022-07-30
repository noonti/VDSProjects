using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using VDSCommon;

namespace VDSCtrlService
{
    /// <summary>
    /// 서비스 등록 :  sc create VDSCtrlService binPath="D:\avogadro\Projects\VDS\SOURCE\2.GIT\VDSProjects\VDSCtrlService\bin\Release\VDSCtrlService.exe"
    /// 서비스 삭제 :  sc.exe delete SensorSyncSVC 
    /// </summary>
    /// 
    public partial class VDSCtrlService : ServiceBase
    {
        EventLog eventLog;
        System.Timers.Timer timer = null;

        String eventLogSource = "VDS 제어 서비스";
        String logName = "VDSCtrlService";


        public VDSCtrlService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists(eventLogSource))
            {
                EventLog.CreateEventSource(eventLogSource, logName);
            }
            eventLog.Source = eventLogSource;
        }

        protected override void OnStart(string[] args)
        {
            AddEventLog(EventLogEntryType.Information, "VDSCtrlService Service Started");
            if (timer == null)
                timer = new System.Timers.Timer();


            timer.Interval = 1000*10 ; // 
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            String ProcessName = ConfigurationManager.AppSettings["PROCESS_NAME"];
            AddEventLog(EventLogEntryType.Information, "VDSCtrlService Service Stopped");
            if (timer != null)
                timer.Stop();
            timer = null;
            KillProcess(ProcessName);
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            WatchProcess();

        }

        private int WatchProcess()
        {
            String ProcessName = ConfigurationManager.AppSettings["PROCESS_NAME"];
            String ProcessPath = ConfigurationManager.AppSettings["PROCESS_PATH"];


            Process[] processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length == 0)
            {

                AddEventLog(EventLogEntryType.Information, $"WatchProcess..{ProcessName}.not found in {ProcessPath}");
                LaunchProcess(ProcessPath, ProcessName);
            }
            return 1;
        }

        public void AddEventLog(EventLogEntryType evtType, String message)
        {
            eventLog.WriteEntry(message, evtType);
        }

        public void LaunchProcess(String processPath, String processName)
        {
            String path = String.Format($"{processPath}\\{processName}.exe");
            Utility.LaunchProcess(path);
        }

        public void KillProcess(String processName)
        {
            foreach(Process process in Process.GetProcesses())
            {
                if(process.ProcessName.StartsWith(processName))
                {
                    process.Kill();
                }
            }
        }
    }
}
