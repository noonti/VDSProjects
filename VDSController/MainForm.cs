using BenchMarkManageCtrl;
using DarkUI.Forms;
using Gma.System.MouseKeyHook;
using KorExManageCtrl;
using KorExManageCtrl.VDSProtocol;
using KorExManageCtrl.VDSProtocol_v2._0;
using MClavisRadarManageCtrl;
using SerialComManageCtrl;
using SerialComManageCtrl.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSAPIModule;
using VDSCommon;
using VDSCommon.API.Model;
using VDSCommon.DataType;
using VDSController.Global;
using VDSDBHandler.DBOperation;
using VDSDBHandler.DBOperation.VDSManage;
using VDSDBHandler.Model;
using VDSManagerCtrl;
using VideoVDSManageCtrl;

namespace VDSController
{
    public partial class MainForm : DarkForm
    {
        VDSManager vdsManager = null;
        SerialComManager serialManager = new SerialComManager();
        WebAPIManager apiManager = null;
        IVDSDevice vdsDevice;
        public ViewLiveCamForm viewLiveCamForm = null;
        Control[] ucControl = new Control[3];
        tabTargetSummary tabTarget;
        String curDate = String.Empty;

        Timer _timer = new Timer();
        bool _initialActiviated = false;

        List<LANE_GROUP> laneGroupList = new List<LANE_GROUP>();

        public MainForm()
        {
            InitializeComponent();

            Hook.GlobalEvents().KeyDown += GlobalHook_KeyDown;

            VDSConfig.ReadConfig();

            GetSpeedCategory();
            GetLengthCategory();
            GetKorexParameter();
            GetLaneGroupList();

            this.Text = VDSConfig.GetVDSControllerName();
           
            InitTabPages();
            InitializeManager();
            StartTimer();
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {      Console.WriteLine($"Key Down " + e.KeyData);    
        }


        public void StartSerialManager()
        {
            serialManager.SetSerialPort(VDSConfig.controllerConfig.RTUPort,VDSConfig.controllerConfig.BaudRate);
            serialManager.SetFormSerialDataFrameDelegate(this, new FormSerialDataFrameDelegate(RTUStatus.ProcessSerialDataFrame));
            serialManager.StartManager();

        }

        public void StopSerialManager()
        {
            serialManager.StopManager();
        }

        public void StartTimer()
        {
            curDate = DateTime.Now.ToString("yyyyMMdd");
            _timer.Interval = 500; // 0.5 초마다 타이머 동작
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            statusTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime now = DateTime.Now;
            if (curDate.CompareTo(now.ToString("yyyyMMdd")) !=0 ) // 날짜가 다르면 카운터 리셋
            {
                //tabTarget.ResetVecycleCount();
                tabTarget.SyncTrafficDataCount();

                curDate = now.ToString("yyyyMMdd");

                // 과거 데이터 삭제(보관주기 이전)
                DeleteExpiredTrafficData();
                // 과거 로그 파일 삭제(보관주기 이전)
                DeleteExpiredLogFile();
            }

        }
        public void InitTabPages()
        {
            int idx = 0;

            tabTarget = ucTargetSummary;// new tabTargetSummary();

            tabTarget.frmMain = this;
            ucControl[idx++] = tabTarget;
            //AddMainTab("실시간", tabTarget);   
            
        }
        
        public void AddMainTab(String tabName, Control tabControl)
        {
            //tabControl.Dock = DockStyle.Fill;
            //Controls.Add(tabControl);
            


            //TabPage tabPage = new TabPage();
            //tabControl.Dock = DockStyle.Fill;
            //tabPage.Controls.Add(tabControl);
            //tabPage.Controls[0].Show();
            //tabPage.Text = tabName;
            //tabMain.Controls.Add(tabPage);
        }

        public int InitializeManager()
        {
            if(vdsManager==null)
            {
                //if (VDSConfig.CENTER_TYPE == (int)REMOTE_CENTER_TYPE.CENTER_KICT)
                switch(VDSConfig.controllerConfig.ProtocolType)
                {
                    case (int)REMOTE_CENTER_TYPE.CENTER_KICT:
                        vdsManager = new BenchMarkManager();
                        break;
                    case (int)REMOTE_CENTER_TYPE.CENTER_KOREX:
                        vdsManager = new KorExManager();
                        break;
                }
                

                switch(VDSConfig.controllerConfig.DeviceType)
                {
                    case 1: // 영상식
                        
                        vdsDevice = new VideoVDSManager();
                        (vdsDevice as VideoVDSManager).SetRtspStreamingUrlDelegate(new SetRtspStreamingUrlDelegate(tabTarget.SetRtspStreamingUrl));
                        vdsManager.SetVDSDevice(vdsDevice);
                        break;
                    case 2: // 레이더식 
                        vdsDevice = new MClavisRadarManager();
                        vdsManager.SetVDSDevice(vdsDevice);
                        break;
                }
#if false

                vdsDevice.SetRtspStreamingUrlDelegate(new SetRtspStreamingUrlDelegate(tabTarget.SetRtspStreamingUrl));
                vdsManager.SetVDSDevice((IVDSDevice) vdsDevice);
#else
                // vdsManager.SetVDSDevice((IVDSDevice)radarDevice);
#endif

                //else if (VDSConfig.CENTER_TYPE == (int)REMOTE_CENTER_TYPE.CENTER_KOREX)
                //vdsManager = new KorExManager();

            }
            vdsManager.SetFormAddTargetInfoDelegate(this, new FormAddTargetInfoDelegate(tabTarget.AddTargetInfo));
            vdsManager.SetFormAddCommuDataDelegate(this, new FormAddCommuDataDelegate(ProcessCommuData));

            if (apiManager == null)
                apiManager = new WebAPIManager();

            GlobalCommonData.GetCommonData();
            return 1;
        }

        //public int AddLog(LOG_TYPE logType, String strLog)

        public async void StartVDSManager()
        {
            int nResult = 0;
            
            var task1 = Task.Run(() => {
                nResult = vdsManager.StartManager();
                if (apiManager != null)
                    apiManager.StartService();
                return nResult;
            });
            nResult = await task1;
            if (nResult > 0)
                tabTarget.ResetVecycleCount();
            tabTarget.StartService();
        }

        public int StopVDSManager()
        {
            int nResult = 0;
            nResult = vdsManager.StopManager();
            tabTarget.StopService();
            if (apiManager != null)
                apiManager.StopService();
            return nResult;
        }

        private void menuStart_Click(object sender, EventArgs e)
        {
            StartVDSManager();
        }

        private void menuStop_Click(object sender, EventArgs e)
        {
            StopVDSManager();
        }

        private void menuQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopVDSManager();
            StopSerialManager();
        }

        private void menuViewCamera_Click(object sender, EventArgs e)
        {
            ViewLiveCam();
        }
        private void ViewLiveCam()
        {
            if(viewLiveCamForm==null)
            {
                viewLiveCamForm = new ViewLiveCamForm();
                viewLiveCamForm.frmMain = this;
            }
            viewLiveCamForm.Show();
        }

        public void StartLiveCamera()
        {
            //vdsManager.StartLiveCamera();
        }

        public void StopLiveCamera()
        {
            //vdsManager.StopLiveCamera();
        }

        private void menuSetTime_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now.Add(new TimeSpan(-1,0,0,0));
            //vdsManager.SetRadarTime(date);
        }

        private void menuStartTargetStreaming_Click(object sender, EventArgs e)
        {
            //vdsManager.SetTargetSummaryStreaming(true);
        }

        private void menuStopTargetStreaming_Click(object sender, EventArgs e)
        {
            //vdsManager.SetTargetSummaryStreaming(false);
        }

        private void menuAddTrafficData_Click(object sender, EventArgs e)
        {

            TargetSummaryInfo target = new TargetSummaryInfo()
            {
                ID = 1,
                ID_0 = 2,
                ID_1 = 3,
                START_CYCLE_0 = 4,
                START_CYCLE_1 = 5,
                AGE_0 = 6,
                AGE_1 = 7,
                MAG_MAX_0 = 8,
                MAG_MAX_1 = 9,
                LANE = 3,
                TRAVEL_DIRECTION = 1,
                LENGTH_X100 = 309,
                SPEED_X100 =30.21,
                RANGE_X100 =1234,
                OCCUPY_TIME =212,
                CREATE_DATE =DateTime.Now
            };
            //vdsManager.AddRadarData(target);
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void menuConfig_Click(object sender, EventArgs e)
        {
            VDSConfigForm configForm = new VDSConfigForm();
            if(configForm.ShowDialog() == DialogResult.OK)
            {
                SaveConfigAndClose();
            }
        }

        private void ucTargetSummary_Load(object sender, EventArgs e)
        {

        }

        private void KoExConfig_Click(object sender, EventArgs e)
        {
            //KorExConfigForm korExConfigForm = new KorExConfigForm();
            //if (korExConfigForm.ShowDialog() == DialogResult.OK)
            //{
            //    VDSConfig.SaveKorExConfig();
            //    MessageBox.Show("도로공사 설정이 변경되었습니다.");
            //}
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if(!_initialActiviated)
            {
                StartSerialManager();
                StartVDSManager();
                _initialActiviated = true;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        public int ProcessCommuData(CommuData commuData)
        {
            int nResult = 0;
            //if(commuData!=null)
            {
                switch(commuData.ProtocolType)
                {
                    case 1: // ITS
                        break;
                    case 2: // KorEx
                        nResult = ProcessKorExCommuData(commuData);
                        break;
                }

            }
            return nResult;
        }

        public int ProcessKorExCommuData(CommuData commuData)
        {
            int nResult = 0;
            switch(commuData.OpCode)
            {
                case  ExDataFrameDefine.OP_SET_TEMPERATURE_COMMAND:
                    nResult = ProcessSetTemperature(commuData);
                    break;
            }

            return nResult;

        }
        public int ProcessSetTemperature(CommuData commuData)
        {
            int nResult = 0;
            KorExManageCtrl.VDSProtocol.WorkData workData = (KorExManageCtrl.VDSProtocol.WorkData)commuData.data;
            //if(workData!=null)
            {
                SetTemperatureRequest request = (SetTemperatureRequest)(workData.frame.opData);
                if(serialManager!=null)
                {
                    serialManager.SetHeaterThresholdRequest(request.heaterTemperature);
                    serialManager.SetFanThresholdRequest(request.fanTemperature);
                }
            }
            return nResult;
        }

        private void menuLaneMng_Click(object sender, EventArgs e)
        {
            LaneManageForm laneManageForm = new LaneManageForm();
            if(laneManageForm.ShowDialog() == DialogResult.OK)
            {
                SaveConfigAndClose();

            }
        }

        private void SaveConfigAndClose()
        {
            VDSConfig.SaveConfig();
            MessageBox.Show("설정이 변경되었습니다. 프로그램을 다시 실행 시 적용됩니다");
            Close();
        }

        private void GetLaneGroupList()
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            laneGroupList = db.GetLaneGroupList(new LANE_GROUP()
            {

            }, out SP_RESULT spResult).ToList();

            GetLaneInfoList(laneGroupList);

            var leftLaneGroup = laneGroupList.Where(x => x.DIRECTION == (int)MOVE_DIRECTION.TO_LEFT).FirstOrDefault();
            if(leftLaneGroup!=null)
            {
                VDSConfig.ToLeftLaneGroup = new LaneGroup()
                {
                    LaneGroupName = leftLaneGroup.LANE_GROUP_NAME,
                    LaneSort = leftLaneGroup.LANE_SORT,
                    Direction = leftLaneGroup.DIRECTION
                };
                foreach(var lane in leftLaneGroup.laneInfoList)
                {
                    LaneInfo laneInfo = new LaneInfo()
                    {
                        LaneName = lane.LANE_NAME,
                        Lane = lane.LANE,
                        Direction = lane.DIRECTION
                    };
                    VDSConfig.ToLeftLaneGroup.AddLaneInfo(laneInfo);
                }

            }

            var rightLaneGroup = laneGroupList.Where(x => x.DIRECTION == (int)MOVE_DIRECTION.TO_RIGHT).FirstOrDefault();
            if (rightLaneGroup != null)
            {
                VDSConfig.ToRIghtLaneGroup = new LaneGroup()
                {
                    LaneGroupName = rightLaneGroup.LANE_GROUP_NAME,
                    LaneSort = rightLaneGroup.LANE_SORT,
                    Direction = rightLaneGroup.DIRECTION
                };
                foreach (var lane in rightLaneGroup.laneInfoList)
                {
                    LaneInfo laneInfo = new LaneInfo()
                    {
                        LaneName = lane.LANE_NAME,
                        Lane = lane.LANE,
                        Direction = lane.DIRECTION
                    };
                    VDSConfig.ToRIghtLaneGroup.AddLaneInfo(laneInfo);
                }

            }
            // VDSConfig.

        }
        private void GetLaneInfoList(List<LANE_GROUP> groupList)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            foreach (var group in groupList)
            {
                group.laneInfoList = db.GetLaneInfoList(new LANE_INFO()
                {
                    LANE_GROUP_ID = group.ID

                }, out SP_RESULT spResult).ToList();
            }
        }

        // 과거 데이터 삭제(보관주기 이전)
        private async void DeleteExpiredTrafficData()
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            int result;
            var task1 = Task.Run(() => {
                db.DeleteExpiredTrafficData(new TRAFFIC_DATA()
                {
                    I_EXPIRE_DAY = VDSConfig.controllerConfig.TrafficDataPeriod
                }, out SP_RESULT spResult);
                return spResult.RESULT_COUNT;
            });
            result = await task1;

            
        }
        // 과거 로그 파일 삭제(보관주기 이전)
        private async void DeleteExpiredLogFile()
        {
            String expireDate = DateTime.Now.AddDays(-1*VDSConfig.controllerConfig.LogFilePeriod).ToString("yyyyMMdd");
            int result;
            var task1 = Task.Run(() => {
                return Utility.DeleteOldFiles(Utility.GetLogPath(), expireDate); ;
            });
            result = await task1;

        }

        private void GetSpeedCategory()
        {
            VDSConfig.speedCategoryList.Clear();
            CommonOperation db = new CommonOperation(VDSConfig.VDS_DB_CONN);
            var speedCategoryList = db.GetSpeedCategoryList(new SPEED_CATEGORY() { }, out SP_RESULT spResult).ToList();
            foreach(var category in speedCategoryList)
            {
                VDSConfig.speedCategoryList.Add(new TrafficCategory()
                {   
                    Id = category.ID,
                    CategoryNo = category.CATEGORY_NO,
                    CategoryUnit = category.SPEED_UNIT,
                    FromValue = (byte)category.FROM_VALUE,
                    ToValue = (byte)category.TO_VALUE,
                    CategoryType = 1,
                });
            }
        }

        private void GetLengthCategory()
        {
            VDSConfig.lengthCategoryList.Clear();

            CommonOperation db = new CommonOperation(VDSConfig.VDS_DB_CONN);
            var lengthCategoryList = db.GetLengthCategoryList(new LENGTH_CATEGORY() { }, out SP_RESULT spResult).ToList();
            foreach (var category in lengthCategoryList)
            {
                VDSConfig.lengthCategoryList.Add(new TrafficCategory()
                {
                    Id = category.ID,
                    CategoryNo = category.CATEGORY_NO,
                    CategoryUnit = category.LENGTH_UNIT,
                    FromValue = (byte)category.FROM_VALUE,
                    ToValue = (byte)category.TO_VALUE,
                    CategoryType = 2,
                });
            }
        }

        private void GetKorexParameter()
        {
            CommonOperation db = new CommonOperation(VDSConfig.VDS_DB_CONN);
            var param = db.GetKorexParameter(new KOREX_PARAMETER(), out SP_RESULT spResult);
            if(param!=null)
            {
                VDSConfig.korexParam.SpeedAccuEnabled = param.SPEED_ACCU_ENABLED ;
                VDSConfig.korexParam.LengthAccuEnabled = param.LENGTH_ACCU_ENABLED;
                VDSConfig.korexParam.speedCalcuEnabled = param.SPEED_CALCU_ENABLED;
                VDSConfig.korexParam.lengthCalcuEnabled = param.LENGTH_CALCU_ENABLED;
                VDSConfig.korexParam.reverseRunEnabled = param.REVERSE_RUN_ENABLED;
                VDSConfig.korexParam.oscillationThreshold = param.OSCILLATION_THRESHOLD;
                VDSConfig.korexParam.autoSyncPeriod = param.AUTO_SYNC_PERIOD;
            }
        }
    }
}
