using AdminManageCtrl;
using DarkUI.Forms;
using Newtonsoft.Json;
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
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.API.Model;
using VDSCommon.Config;
using VDSCommon.Protocol.admin;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;
using VDSWebAPIServer.Forms;
using static System.Windows.Forms.ListViewItem;

namespace VDSWebAPIServer
{

    public delegate void DisplayVDSController(String vdsControllerId);

    public partial class MainForm : DarkForm
    {
        HttpSelfHostConfiguration config;
        HttpSelfHostServer server;


        USER_INFO loginUser = null;
        VDSAdminManager adminManager = new VDSAdminManager();

        //VDSControllerOperation vdsControllerOp = new VDSControllerOperation();
        VDS_CONTROLLER searchCondition = new VDS_CONTROLLER();


        List<VDS_CONTROLLER> vdsControllerList = new List<VDS_CONTROLLER>();
        List<VDSViewForm> viewFormList = new List<VDSViewForm>();
        List<WebRemoteForm> remoteFormList = new List<WebRemoteForm>();
        List<RTSPPlayerForm> playerFormList = new List<RTSPPlayerForm>();

        DisplayVDSController _displayVDSController = null;

        Timer timer = null;

        int currentPage = 1;
        int totalPage = 1;

        bool firstActivated = false;

        VDSLogger _Logger = new VDSLogger();
        public MainForm()
        {
            InitializeComponent();
     
            ReadMAConfig();
            _displayVDSController = new DisplayVDSController(UpdateVDSControllerList);
            StartHTTPService();
            GlobalCommonData.GetCommonData();
            

            _Logger.SetManagerType(MANAGER_TYPE.VDS_ADMIN);
            Utility._addLog = _Logger.AddLog;

            GetAllVDSController();

            ucVDSGroupTreeView.SetChangeVDSGroupsEventDelegate(ChangeVDSGroups);
            
        }

        private void StartHTTPService()
        {
            config = new HttpSelfHostConfiguration($"http://{AdminConfig.ADMIN_ADDRESS}:{AdminConfig.ADMIN_API_PORT}");
            config.Routes.MapHttpRoute(
            name: "ControllerAndActionOnly",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { },
            constraints: new { action = @"^[a-zA-Z]+([\s][a-zA-Z]+)*$" });

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            StartService();
        }

        private int StartService()
        {
            _Logger.StartManager();
            StarTimer();
            adminManager.SetMADataEventDelegate(ProcessMADataEvent);
            adminManager.StartManager();



            return 1;
        }

        private int StopService()
        {
            adminManager.StopManager();
            _Logger.StopManager();
            server.CloseAsync().Wait();
            StopTimer();
            return 1;
        }

        private int StarTimer()
        {
            if(timer==null)
                timer = new Timer();
            timer.Interval = 1000; // 1초마다 체크
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
            return 1;
        }

        private int StopTimer()
        {
            if(timer!=null)
            {
                timer.Stop();
                timer = null;
            }
            return 1;
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            CheckHeartBeat();


        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            VDSAddForm addForm = new VDSAddForm();
            addForm.Text = "제어기 추가";
            addForm.operatonMode = "ADD";
            VDSControllerOperation vdsControllerOp = new VDSControllerOperation(VDSConfig.MA_DB_CONN);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (addForm._vdsCtrl != null)
                {
                    var targetVDS = SearchVDSController(addForm._vdsCtrl.CONTROLLER_ID);
                    if(targetVDS==null)
                    {
                        var added = vdsControllerOp.GetVDSController(addForm._vdsCtrl, out SP_RESULT spResult);
                        if(spResult.RESULT_COUNT >0)
                        {
                            vdsControllerList.Add(added);
                            AddVDSControllerToListView(lvVDSControl, added);
                        }
                    }
                }
            }
        }

        private void darkButton6_Click(object sender, EventArgs e)
        {

        }

        private void darkButton1_Click(object sender, EventArgs e)
        {

        }

        private void searchVDSController(int page, int pageSize)
        {
            for(int i=0;i< lvVDSControl.Items.Count;i++)
            {
                var items = (VDSListItem)lvVDSControl.Items[i];
                items.DisposeControl();
            }

            lvVDSControl.Items.Clear();
            SetVDSControllerToListView(vdsControllerList);
            

        }


        private void SetVDSControllerToListView(List<VDS_CONTROLLER> vdsControllerList)
        {
           // lvVDSControl.Items.Clear();

            foreach (var vdsController in vdsControllerList)
            {
                AddVDSControllerToListView(lvVDSControl, vdsController);
            }
        }

        private void AddVDSControllerToListView(ListView lvView, VDS_CONTROLLER vdsController)
        {
            VDSListItem item;
            item = new VDSListItem(vdsController, lvView); 
            item.Tag = vdsController;
            lvView.Items.Add(item);
            item.SetControl();
            UpdateVDSControllerList(vdsController.CONTROLLER_ID);


        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            var items = lvVDSControl.SelectedItems;
            VDSControllerOperation vdsControllerOp = new VDSControllerOperation(VDSConfig.MA_DB_CONN);
            foreach (ListViewItem item in items)
            {
                //Console.WriteLine($"선택한 아이템 :{item.Text}");
                VDS_CONTROLLER vdsController = (VDS_CONTROLLER)item.Tag;
                if (vdsController != null)
                {
                    VDSAddForm addForm = new VDSAddForm();
                    addForm.Text = "제어기 수정";
                    addForm.selectedVDSCtrl = vdsController;
                    addForm.operatonMode = "UPDATE";
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        if (addForm._vdsCtrl != null)
                        {
                            var targetVDS = SearchVDSController(addForm._vdsCtrl.CONTROLLER_ID);
                            if (targetVDS != null)
                            {
                                var updated = vdsControllerOp.GetVDSController(addForm._vdsCtrl, out SP_RESULT spResult);
                                if (spResult.RESULT_COUNT > 0)
                                {
                                    targetVDS.SetVDSController(updated);
                                    UpdateVDSControllerList(targetVDS.CONTROLLER_ID);
                                }
                               
                            }

                        }

                    }

                }
                else
                {
                    MessageBox.Show("수정할 제어기를 선택하세요", "오류", MessageBoxButtons.OK);
                }
            }
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            //searchVDSController(currentPage, GlobalCommonData.PAGE_SIZE);
            GetAllVDSController();
            searchVDSController(currentPage, GlobalCommonData.PAGE_SIZE);
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            VDSControllerOperation vdsControllerOp = new VDSControllerOperation(VDSConfig.MA_DB_CONN);
            if (lvVDSControl.SelectedIndices.Count > 0 && MessageBox.Show("삭제하시겠습니까?", "삭제", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var items = lvVDSControl.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    VDS_CONTROLLER vdsController = (VDS_CONTROLLER)item.Tag;
                    if (vdsController != null)
                    {
                        vdsControllerOp.DeleteVDSController(vdsController, out SP_RESULT spResult);

                        if (spResult.RESULT_CODE.CompareTo("500") == 0)
                        {
                            MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                            return;
                        }
                        else
                        {
                            var targetVDS = SearchVDSController(vdsController.CONTROLLER_ID);
                            if(targetVDS!=null)
                            {
                                VDSListItem listItem = SearchVDSListItem(vdsController.CONTROLLER_ID);
                                if(listItem!=null)
                                {
                                    lvVDSControl.Items.Remove(listItem);
                                }
                                vdsControllerList.Remove(targetVDS);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("삭제할 제어기를 선택하세요", "오류", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopService();
        }

        public int ProcessMADataEvent(SessionContext session, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            switch (frame.OpCode)
            {
                case MADataFrameDefine.OPCODE_AUTH_VDS:
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($".... MADataFrameDefine.OPCODE_AUTH_VDS"));
                    ProcessAuthVDS(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_EVENT_TRAFFIC:
                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($".... MADataFrameDefine.OPCODE_EVENT_TRAFFIC"));
                    ProcessEventTrafficData(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_HISTORIC_TRAFFIC:

                    Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($".... MADataFrameDefine.OPCODE_HISTORIC_TRAFFIC"));
                    break;
                case MADataFrameDefine.OPCODE_HEARTBEAT:
                    ProcessHeartBeat(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_VDS_CONFIG:
                    ProcessVDSConfig(session, frame);
                    break;
                case MADataFrameDefine.OPCODE_VDS_DISCONNECT:
                    ProcessVDSDisConnect(session, frame);
                    break;
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessAuthVDS(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MAAuthRequest request;
            if (frame != null && frame.opDataFrame != null)
            {
                request = frame.opDataFrame as MAAuthRequest;

                // vds controller id 로 찾아서 auth 값 변경한다. 
                SetVDSControllerStatus(sessionContext, request.vdsControllerId, (int)SOCKET_STATUS.AUTHORIZED);
                PostUpdateVDSController(request.vdsControllerId);

            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }

        private int ProcessEventTrafficData(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));

            int nResult = 0;
            MATrafficDataEvent request;
            if (frame != null && frame.opDataFrame != null)
            {
                request = frame.opDataFrame as MATrafficDataEvent;
                var viewForm = SearchVDSViewForm(request.vdsControllerId);
                if(viewForm!=null)
                {
                    viewForm.AddEventTrafficData(request.trafficDataList);
                }
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

            return nResult;
        }

        private int ProcessHeartBeat(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            MAHeartBeatRequest request;
            MAHeartBeatResponse response;

            if (frame != null && frame.opDataFrame != null)
            {
                request = frame.opDataFrame as MAHeartBeatRequest;
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"vdsController id({request.vdsControllerId}) receive heartbeat and rackStatus= {request.rackStatus} "));
                SetVDSControllerStatus(sessionContext, request.vdsControllerId, (int)SOCKET_STATUS.AUTHORIZED);
                // rack staus update ..
                SetVDSRackStatus(request.vdsControllerId, request.rackStatus);
                PostUpdateVDSController(request.vdsControllerId);
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private int ProcessVDSConfig(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;
            MAVDSConfigRequest request;
            

            if (frame != null && frame.opDataFrame != null)
            {
                request = frame.opDataFrame as MAVDSConfigRequest;
                var vdsController = SearchVDSController(request.vdsControllerId);
                if(vdsController!=null)
                {
                    vdsController.VDS_CONFIG = JsonConvert.SerializeObject(request);
                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }

        private int ProcessVDSDisConnect(SessionContext sessionContext, MADataFrame frame)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            if (sessionContext!=null)
            {
                var vdsController = vdsControllerList.Where(x=>x.sessionContext == sessionContext).FirstOrDefault() ;
                if(vdsController!=null)
                {
                    vdsController.LAST_HEARTBEAT_TIME = String.Empty;
                    vdsController.STATUS = (int)SOCKET_STATUS.DISCONNECTED;

                    PostUpdateVDSController(vdsController.CONTROLLER_ID);
                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if(!firstActivated)
            {
                searchVDSController(currentPage, GlobalCommonData.PAGE_SIZE);
                firstActivated = true;
            }
        }

        private void ucVDSGroupTreeView_Load(object sender, EventArgs e)
        {

        }

        private int SetVDSControllerStatus(MAHeartBeatRequest request)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            int nResult = 0;

            // request.vdsControllerId  로 search 


            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
            return nResult;
        }


        private void GetAllVDSController()
        {

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            VDSControllerOperation vdsControllerOp = new VDSControllerOperation(VDSConfig.MA_DB_CONN);

            vdsControllerList = vdsControllerOp.GetVDSControllerList(new VDS_CONTROLLER()
            {
                PAGE_SIZE = 999999,
                CURRENT_PAGE = 1
            }, out SP_RESULT spResult).ToList();
            if (spResult.RESULT_CODE.CompareTo("500") == 0)
            {
                MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                return;
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private VDS_CONTROLLER SearchVDSController(String vdsControllerId)
        {
            VDS_CONTROLLER result = null;
            // vds controller id 로 찾는다
            var vdsController = vdsControllerList.Where(x => x.CONTROLLER_ID.CompareTo(vdsControllerId) == 0);
            if (vdsController.Count() > 0)
                result = vdsController.First();
            return result;
        }

        private void SetVDSControllerStatus(SessionContext sessionContext, String vdsControllerId, int status)
        {
            var vdsController = SearchVDSController(vdsControllerId);
            if (vdsController != null)
            {
                vdsController.STATUS = status;
                vdsController.LAST_HEARTBEAT_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vdsController.sessionContext = sessionContext;

            }
        }


        private void SetVDSRackStatus(String vdsControllerId, RackStatus rackStatus)
        {
            var vdsController = SearchVDSController(vdsControllerId);
            if (vdsController != null)
            {
                vdsController.rackStatus = rackStatus;
            }
        }

        private void PostUpdateVDSController(String vdsControllerId)
        {
            BeginInvoke(_displayVDSController, new object[] { vdsControllerId });
        }

        private void UpdateVDSControllerList(String vdsControllerId)
        {
            var vdsController = SearchVDSController(vdsControllerId);
            if (vdsController != null)
            {
                VDSListItem listItem = SearchVDSListItem(vdsControllerId);
                if(listItem!=null)
                    listItem.UpdateVDSControllerInfo(vdsController);
                //for (int i=0;i<lvVDSControl.Items.Count;i++)
                //{
                //    var vds = (VDS_CONTROLLER) lvVDSControl.Items[i].Tag;
                //    if(vds.CONTROLLER_ID.CompareTo(vdsControllerId) == 0)
                //    {
                //        VDSListItem listItem = (VDSListItem)lvVDSControl.Items[i];
                //        listItem.UpdateVDSControllerInfo(vdsController);
                //        break;
                //    }
                //}
            }
        }

        private VDSListItem SearchVDSListItem(String vdsControllerId)
        {
            VDSListItem result = null;
            for (int i = 0; i < lvVDSControl.Items.Count; i++)
            {
                var vds = (VDS_CONTROLLER)lvVDSControl.Items[i].Tag;
                if (vds.CONTROLLER_ID.CompareTo(vdsControllerId) == 0)
                {
                    result = (VDSListItem)lvVDSControl.Items[i];
                    break;
                }
            }
            return result;
        }

        private void CheckHeartBeat()
        {
            TimeSpan timeSpan;
            DateTime heartBeatTime;

            for (int i = 0;i<vdsControllerList.Count;i++)
            {
                var vdsController = vdsControllerList[i];

                if(vdsController.STATUS == (int)SOCKET_STATUS.AUTHORIZED && !String.IsNullOrEmpty(vdsController.LAST_HEARTBEAT_TIME))
                {
                    heartBeatTime = DateTime.ParseExact(vdsController.LAST_HEARTBEAT_TIME, "yyyy-MM-dd HH:mm:ss", null);
                    timeSpan = DateTime.Now - heartBeatTime;

                    if (timeSpan.TotalSeconds > 60)
                    {
                        adminManager.DeleteSessionContext(vdsController.sessionContext);

                        vdsController.LAST_HEARTBEAT_TIME = String.Empty;
                        vdsController.STATUS = (int)SOCKET_STATUS.DISCONNECTED;

                        PostUpdateVDSController(vdsController.CONTROLLER_ID);
                    }
                }
            }
        }


        private VDSViewForm SearchVDSViewForm(String vdsControllerId)
        {
            VDSViewForm result = null;

            for(int i = 0;i< viewFormList.Count;i++)
            {
                if(viewFormList[i].selectedVDSCtrl.CONTROLLER_ID.CompareTo(vdsControllerId)==0)
                {
                    result = viewFormList[i];
                    break;
                }
            }
            return result;
        }


        private WebRemoteForm SearchRemoteForm(String vdsControllerId)
        {
            WebRemoteForm result = null;

            for (int i = 0; i < remoteFormList.Count; i++)
            {
                if (remoteFormList[i].selectedVDSCtrl.CONTROLLER_ID.CompareTo(vdsControllerId) == 0)
                {
                    result = remoteFormList[i];
                    break;
                }
            }
            return result;
        }

        private RTSPPlayerForm SearchPlayerForm(String vdsControllerId)
        {
            RTSPPlayerForm result = null;
            for (int i = 0; i < playerFormList.Count; i++)
            {
                if (playerFormList[i].selectedVDSCtrl.CONTROLLER_ID.CompareTo(vdsControllerId) == 0)
                {
                    result = playerFormList[i];
                    break;
                }
            }
            return result;
        }

        
        private void lvVDSControl_DoubleClick(object sender, EventArgs e)
        {
            var items = lvVDSControl.SelectedItems;
            VDSViewForm vdsViewForm;
            foreach (ListViewItem item in items)
            {
                VDS_CONTROLLER vdsController = (VDS_CONTROLLER)item.Tag;
                if (vdsController != null)
                {
                    vdsViewForm = SearchVDSViewForm(vdsController.CONTROLLER_ID);
                    if (vdsViewForm == null)
                    {
                        vdsViewForm = new VDSViewForm();
                        vdsViewForm.Text = "제어기 정보";
                        vdsViewForm.selectedVDSCtrl = vdsController;
                        vdsViewForm.viewFormList = viewFormList;
                        vdsViewForm.mainForm = this;
                        vdsViewForm.Show();
                        viewFormList.Add(vdsViewForm);
                    }
                    else
                    {
                        vdsViewForm.BringToFront();
                    }
                }
            }
        }

        public void RequestStartTrafficDataSend(String vdsCtrollerId)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (!String.IsNullOrEmpty(vdsCtrollerId))
            {
                var vdsContoller = SearchVDSController(vdsCtrollerId);
                if (vdsContoller != null)
                {
                    if (vdsContoller.STATUS == (int)SOCKET_STATUS.AUTHORIZED)
                        adminManager.RequestStartTrafficDataSend(vdsContoller.sessionContext, vdsContoller.CONTROLLER_ID);
                    else
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"vdsCtrollerId{vdsCtrollerId} STATUS ={vdsContoller.STATUS} 로 AUTHORIZE 상태가 아니어서 검지 데이터 전송 시작 요청 실패 "));
                    }
                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        public void RequestStopTrafficDataSend(String vdsCtrollerId)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (!String.IsNullOrEmpty(vdsCtrollerId))
            {
                var vdsContoller = SearchVDSController(vdsCtrollerId);
                if (vdsContoller != null)
                {
                    if (vdsContoller.STATUS == (int)SOCKET_STATUS.AUTHORIZED)
                        adminManager.RequestStopTrafficDataSend(vdsContoller.sessionContext, vdsContoller.CONTROLLER_ID);
                    else
                    {
                        Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"vdsCtrollerId{vdsCtrollerId} STATUS ={vdsContoller.STATUS} 로 AUTHORIZE 상태가 아니어서 검지 데이터 전송 중지 요청 실패 "));
                    }
                }
            }
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));
        }

        private void ReadMAConfig()
        {
            int value = 0;
           

            AdminConfig.ADMIN_ADDRESS = AppConfiguration.GetAppConfig("MA_ADDRESS");
            if (String.IsNullOrEmpty(AdminConfig.ADMIN_ADDRESS))
                AdminConfig.ADMIN_ADDRESS = "127.0.0.1";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("MA_PORT"), out value))
                value = 1234;

            AdminConfig.ADMIN_PORT = value;


            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("MA_API_PORT"), out value))
                value = 8088;
            AdminConfig.ADMIN_API_PORT = value;


            


            AdminConfig.DB_ADDRESS = AppConfiguration.GetAppConfig("MA_DB_ADDRESS");
            if (String.IsNullOrEmpty(AdminConfig.DB_ADDRESS))
                AdminConfig.DB_ADDRESS = "127.0.0.1";

            value = 0;
            if (!int.TryParse(AppConfiguration.GetAppConfig("MA_DB_PORT"), out value))
                value = 3306;
            AdminConfig.DB_PORT = value;


            AdminConfig.DB_NAME = AppConfiguration.GetAppConfig("MA_DB_NAME");
            if (String.IsNullOrEmpty(AdminConfig.DB_NAME))
                AdminConfig.DB_NAME = "vdsmanage";



            AdminConfig.DB_USER = AppConfiguration.GetAppConfig("MA_DB_USER");
            if (String.IsNullOrEmpty(AdminConfig.DB_USER))
                AdminConfig.DB_USER = "VDS";


            AdminConfig.DB_PASSWD = AppConfiguration.GetAppConfig("MA_DB_PASSWD");
            if (String.IsNullOrEmpty(AdminConfig.DB_PASSWD))
                AdminConfig.DB_PASSWD = "1234";


            VDSConfig.MA_DB_CONN = String.Format($"Server={AdminConfig.DB_ADDRESS};Port={AdminConfig.DB_PORT};Database={AdminConfig.DB_NAME};Uid={AdminConfig.DB_USER};Pwd={AdminConfig.DB_PASSWD};SSL Mode=None");



        }

        private int SaveMAConfig()
        {
            AppConfiguration.SetAppConfig("MA_ADDRESS", AdminConfig.ADMIN_ADDRESS);
            AppConfiguration.SetAppConfig("MA_PORT", AdminConfig.ADMIN_PORT.ToString());
            AppConfiguration.SetAppConfig("MA_API_PORT", AdminConfig.ADMIN_API_PORT.ToString());

            AppConfiguration.SetAppConfig("MA_DB_ADDRESS", AdminConfig.DB_ADDRESS);
            AppConfiguration.SetAppConfig("MA_DB_PORT", AdminConfig.DB_PORT.ToString());

            AppConfiguration.SetAppConfig("MA_DB_NAME", AdminConfig.DB_NAME);
            AppConfiguration.SetAppConfig("MA_DB_USER", AdminConfig.DB_USER);

            AppConfiguration.SetAppConfig("MA_DB_PASSWD", AdminConfig.DB_PASSWD);



            return 1;
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AdminConfigForm configForm = new AdminConfigForm();
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                SaveMAConfig();
                MessageBox.Show("설정이 변경되었습니다. 프로그램을 다시 실행 시 적용됩니다");
                Close();
            }
        }

        private void darkButton6_Click_1(object sender, EventArgs e)
        {
            if (lvVDSControl.SelectedIndices.Count > 0 )
            {
                var items = lvVDSControl.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    VDS_CONTROLLER vdsController = (VDS_CONTROLLER)item.Tag;
                    if (vdsController != null)
                    {
                        WebRemoteForm webForm = SearchRemoteForm(vdsController.CONTROLLER_ID);
                        if (webForm == null)
                        {
                            webForm = new WebRemoteForm();
                            webForm.selectedVDSCtrl = vdsController;
                            webForm.remoteFormList = remoteFormList;
                            webForm.Show();
                            remoteFormList.Add(webForm);
                            webForm.LoadRemoteCtrl();
                        }
                        else
                        {
                            webForm.BringToFront();
                        }
                    }
                    else
                    {
                        MessageBox.Show("원격연결할 제어기를 선택하세요", "오류", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void darkButton1_Click_1(object sender, EventArgs e)
        {
            if (lvVDSControl.SelectedIndices.Count > 0)
            {
                var items = lvVDSControl.SelectedItems;
                foreach (ListViewItem item in items)
                {
                    VDS_CONTROLLER vdsController = (VDS_CONTROLLER)item.Tag;
                    if (vdsController != null)
                    {
                        RTSPPlayerForm playerForm = SearchPlayerForm(vdsController.CONTROLLER_ID);
                        if (playerForm == null)
                        {
                            playerForm = new RTSPPlayerForm();
                            playerForm.selectedVDSCtrl = vdsController;
                            playerForm.playerFormList = playerFormList;
                            playerForm.LoadPlayer();
                            playerForm.Show();
                            playerFormList.Add(playerForm);
                            playerForm.StartPlay();
                        }
                        else
                        {
                            playerForm.BringToFront();
                        }
                    }
                    else
                    {
                        MessageBox.Show("제어기를 선택하세요", "오류", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void ucVDSGroupTreeView_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoginForm logForm = new LoginForm();
            if (logForm.ShowDialog() == DialogResult.OK)
            {
                UserInfoOperation userOp = new UserInfoOperation(VDSConfig.MA_DB_CONN);
                userOp.CheckLogin(new USER_INFO()
                {
                    USER_ID = logForm.userId,
                    PASSWD = logForm.passwd

                }, out SP_RESULT spResult);

                if (spResult.RESULT_COUNT > 0)
                {
                    loginUser = userOp.GetUserInfo(new USER_INFO()
                    {
                        USER_ID = logForm.userId,
                    }, out spResult);
                }
                else
                {
                    MessageBox.Show("관리자 아이디 또는 비밀번호를 확인하세요", "로그인 실패", MessageBoxButtons.OK);
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void ChangeVDSGroups(VDS_GROUPS vdsGroup)
        {
            if(vdsGroup!=null)
            {
                lvVDSControl.Items.Clear();
                var vdsList = vdsControllerList.Where(x => x.GROUP_ID == vdsGroup.ID).ToList();
                SetVDSControllerToListView(vdsList);
            }
        }

        private void MenuUser_Click(object sender, EventArgs e)
        {
            if(loginUser!=null && loginUser.USER_TYPE > 1)
            {
                UserManageForm userManageForm = new UserManageForm();
                userManageForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("사용자 관리 권한이 없습니다", "권한오류");
            }

        }
    }
}
