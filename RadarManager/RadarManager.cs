using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDSCommon;
using VDSCommon.DataType;

namespace RadarManageCtrl
{
    public class RadarManager : IVDSManager
    {
        SpeedLane speedLane;//= new SpeedLane();
        public AddTrafficDataEvent _addRadarData = null;

        public bool _isService = false;

        public int StartManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            String strLog;
                
            int nResult = 0;
            StopManager();
            speedLane = new SpeedLane(VDSConfig.controllerConfig.DeviceAddress, VDSConfig.controllerConfig.RemotePort);

            //speedLane.SetAddLogDelegate(_addLog);
            speedLane.SetAddRadarDataDelegate(_addRadarData);


            nResult = speedLane.ConnectToRadar();
            //TODO 아래 코드 최적화 필요(현재 테스트코드)
            //speedLane.GetSysInfo();
            //speedLane.GetTargetSummaryStreamingInfo();
            //speedLane.SetTargetSummaryStreaming((byte)ENV_BOOL.TRUE);

            _isService = true;

            //if (speedLane.GetTargetSummaryInfo() > 0)
            //    _isService = true;
            //else
            //    _isService = false;

            strLog = String.Format("RadarManager 서비스 시작");
            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int StopManager()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            String strLog;
            if (speedLane!=null)
            {
                speedLane.DisConnectToRadar();
                speedLane = null;
                
            }
            _isService = false;
            strLog = String.Format("RadarManager 서비스 중지");
            Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return 1;
        }




        public void SetAddRadarDataDelegate(AddTrafficDataEvent addRadarData)
        {
            _addRadarData = addRadarData;
        }

      
        public int AddRadarData(TrafficDataEvent radarData)
        {
           // AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            String strLog;
            if (_addRadarData != null)
            {
                nResult = _addRadarData(radarData);
                strLog = String.Format("Radar 데이터 추가");
                Utility.AddLog(LOG_TYPE.LOG_INFO, strLog);
            }
            //AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int GetSysInfo()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            return speedLane.GetSysInfo(null);
        }

        //public int GetTargetSummaryInfo()
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    return speedLane.GetTargetSummaryInfo();
        //}

        //public int StopTargetSummaryInfoStreaming()
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    return speedLane.StopTargetSummaryInfoStreaming();
        //}

        public int StartLiveCamera()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            return speedLane.StartLiveCamera();
        }

        public int StopLiveCamera()
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            return speedLane.StopLiveCamera();
        }

        //public int SetRadarTime(DateTime? date, ref DateTime deviceTime)
        //{
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
        //    int nResult = 0;
        //    nResult = speedLane.SetRadarTime(date);
        //    if(nResult > 0)
        //    {
        //        nResult = speedLane.GetRadarTime(ref deviceTime);
        //    }
        //    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
        //    return nResult;
        //}

        public int SetRadarTime(Object callbackFunc, Object workData, DateTime? date)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            nResult = speedLane.SetRadarTime(callbackFunc, workData,date);
            //nResult = speedLane.SetRadarTime(date);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int CheckVDSStatus(ref byte[] status, ref byte[] checkTime)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            nResult = speedLane.CheckVDSStatus(ref status, ref checkTime);
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name+":"+ MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        public int SetTargetSummaryStreaming(bool enable)
        {
            
            return speedLane.SetTargetSummaryStreaming((byte)(enable==true?ENV_BOOL.TRUE:ENV_BOOL.FALSE));
        }

        //public int SendTrafficData(TargetSummaryInfo target)
        //{
        //    return 1;
        //}

    }
}