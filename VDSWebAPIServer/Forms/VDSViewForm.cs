using DarkUI.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSCommon.API.APIResponse;
using VDSCommon.API.Model;
using VDSCommon.Config;
using VDSCommon.Protocol.admin;
using VDSWebAPIServer.Common;




namespace VDSWebAPIServer.Forms
{
    public partial class VDSViewForm : DarkForm
    {
        public VDS_CONTROLLER selectedVDSCtrl;
        bool bInitalSet = false;

        public List<VDSViewForm> viewFormList = null;
        public List<int> laneList = new List<int>();

        public MainForm mainForm = null;



        protected virtual bool DoubleBuffered { get; set; }

        public ControllerConfig controllerConfig ;
        public KictConfig kictConfig ;
        public KorExConfig korExConfig ;


        int currentPage = 1;
        int totalPage = 1;
        TRAFFIC_DATA searchData = null;


        public VDSViewForm()
        {
            InitializeComponent();
            ApiUtility.FillVDSGroupsComboBox(GlobalCommonData.vdsGroupsList, cbVDSGroups);
        }

        private void VDSViewForm_Activated(object sender, EventArgs e)
        {
            if (selectedVDSCtrl != null && !bInitalSet)
            {
                InitializeLaneInfo();
                SetVDSContollerInfo(selectedVDSCtrl);
                if(mainForm!=null)
                {
                    mainForm.RequestStartTrafficDataSend(selectedVDSCtrl.CONTROLLER_ID);
                }
            }
            bInitalSet = true;
        }

        public void SetVDSContollerInfo(VDS_CONTROLLER vdsController)
        {

            cbVDSGroups.SelectedIndex = GlobalCommonData.vdsGroupsList.FindIndex(x => x.ID == vdsController.GROUP_ID);
            txtControllerName.Text = vdsController.CONTROLLER_NAME;
            txtControllerId.Text = vdsController.CONTROLLER_ID;
            txtIPAddress.Text = vdsController.IP_ADDRESS;

            switch (vdsController.PROTOCOL)
            {
                case 1:
                    rdgProtocol1.Checked = true;
                    break;
                case 2:
                    rdgProtocol2.Checked = true;
                    break;
            }

            switch (vdsController.VDS_TYPE)
            {
                case 1:
                    rdgVDSType1.Checked = true;
                    break;
                case 2:
                    rdgVDSType2.Checked = true;
                    break;
                case 3:
                    rdgVDSType3.Checked = true;
                    break;

            }

            if (vdsController.USE_YN == "Y")
                rdgUseY.Checked = true;
            else
                rdgUseN.Checked = true;
            SetEnable(false);

            if(!String.IsNullOrEmpty(vdsController.VDS_CONFIG))
            {
                var data = JsonConvert.DeserializeObject<MAVDSConfigRequest>(vdsController.VDS_CONFIG);
                if (data != null)
                {
                    controllerConfig = data.controllerConfig;
                    kictConfig = data.kictConfig;
                    korExConfig = data.korExConfig;

                    ucConfig.LoadVDSConfig(controllerConfig, kictConfig, korExConfig);

                }
            }
        }

        public void SetEnable(bool enabled)
        {
            cbVDSGroups.Enabled = enabled;
            txtControllerName.Enabled = enabled;
            txtControllerId.Enabled = enabled;
            txtIPAddress.Enabled = enabled;

            rdgProtocol1.Enabled = enabled;
            rdgProtocol2.Enabled = enabled;

            rdgVDSType1.Enabled = enabled;
            rdgVDSType2.Enabled = enabled;
            rdgVDSType3.Enabled = enabled;
            rdgUseY.Enabled = enabled;
            rdgUseN.Enabled = enabled;
            btnSave.Enabled = enabled;
        }

        private void VDSViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("View Form closed");
            if(viewFormList!=null)
            {
                viewFormList.Remove(this);
            }
            if (mainForm != null)
            {
                mainForm.RequestStopTrafficDataSend(selectedVDSCtrl.CONTROLLER_ID);
            }
        }

        public void AddEventTrafficData(List<TrafficData> trafficDataList)
        {
            for (int i = 0; i < trafficDataList.Count; i++)
            {
                if(lvTrafficData.InvokeRequired)
                {
                    lvTrafficData.Invoke(new MethodInvoker(delegate { AddRealTimeTrafficDataList(trafficDataList[i]); }));
                }
                else 
                {
                    AddRealTimeTrafficDataList(trafficDataList[i]);
                }
                
            }

        }

        private void AddRealTimeTrafficDataList(TrafficData trafficData)
        {
           

            if (lvTrafficData.Items.Count > 1000)
                lvTrafficData.Items.RemoveAt(lvTrafficData.Items.Count - 1);

            ListViewItem item;
            // 제목, 콘텐츠 유형,  조회수, 별점수, 의뢰인, 코치, 등록일
            item = new ListViewItem(trafficData.detectTime); // 

            item.SubItems.Add(trafficData.lane.ToString());
            item.SubItems.Add(trafficData.direction == 1 ? "상행선" : "하행선");
            item.SubItems.Add(trafficData.length.ToString());
            item.SubItems.Add(trafficData.speed.ToString());
            item.SubItems.Add(trafficData.occupyTime.ToString());
            item.SubItems.Add(trafficData.loop1OccupyTime.ToString());
            item.SubItems.Add(trafficData.loop2OccupyTime.ToString());
            item.SubItems.Add(trafficData.reverseRunYN);
            item.SubItems.Add(trafficData.reportYN != null ? trafficData.reportYN : "N");
            lvTrafficData.Items.Insert(0, item);

        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void darkButton1_Click_1(object sender, EventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.RequestStartTrafficDataSend(selectedVDSCtrl.CONTROLLER_ID);
            }
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.RequestStopTrafficDataSend(selectedVDSCtrl.CONTROLLER_ID);
            }
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            String startDate = ucStartTime.GetDateTimeFormat();
            String endDate = ucEndTime.GetDateTimeFormat();
            int lane = 0;
            if (cbLane.SelectedIndex > 0)
                lane = laneList[cbLane.SelectedIndex - 1];



            currentPage = 1;

            searchData = new TRAFFIC_DATA()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                LANE = lane,
                I_PAGE_NO = currentPage,
                I_PAGE_SIZE = GlobalCommonData.PAGE_SIZE,
             };

            lvTrafficDataStat.Items.Clear();
            SearchTrafficData(currentPage, GlobalCommonData.PAGE_SIZE);
        }


        private void InitializeLaneInfo()
        {
            laneList = VDSConfig.ToRIghtLaneGroup.LaneList.Select(x => x.Lane).ToList().Concat(VDSConfig.ToLeftLaneGroup.LaneList.Select(x => x.Lane).ToList()).OrderBy(p => p).ToList();

            foreach (var data in laneList)
            {
                cbLane.Items.Add(data);
            }
            cbLane.Items.Insert(0, "전체");
        }

        private void ucStartTime_Load(object sender, EventArgs e)
        {

        }

        public async void SearchTrafficData(int page, int pageSize) //  TRAFFIC_DATA data)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 처리 "));
            if (controllerConfig == null)
            {
                MessageBox.Show("VDS 설정 정보가 없습니다", "오류");
                return;
            }
                
            String url =  String.Format($"http://{controllerConfig.IpAddress}:{controllerConfig.ApiPort}/api/Traffic/GetTrafficDataListByPage");
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"VDS ID={selectedVDSCtrl.ID} 조회 API(url={url} ") );

            String response = String.Empty;
            HttpStatusCode statusCode;
            TrafficDataResponse trafficDataResponse;

            searchData.I_PAGE_NO = page;
            searchData.I_PAGE_SIZE = pageSize;

            var task1 = Task.Run(() => {
                return Utility.RequestWebAPI(url, searchData, out response);
            });

            statusCode = await task1;
            if(statusCode == HttpStatusCode.OK)
            {
                trafficDataResponse = JsonConvert.DeserializeObject<TrafficDataResponse>(response);

                totalPage = ApiUtility.GetTotalPageCount(trafficDataResponse.RESULT_COUNT, GlobalCommonData.PAGE_SIZE);
                lbTotalCount.Text = String.Format($"조회결과수: {trafficDataResponse.RESULT_COUNT}");
                lbPageInfo.Text = String.Format($"페이지: {page}/{totalPage}");

                AddTrafficDataStatResult(trafficDataResponse.resultList);

            }
            else
            {
                lbPageInfo.Text = String.Format($"0/0");
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"VDS ID={selectedVDSCtrl.ID} 조회 API(url={url} 실패. http code={statusCode}"));
                MessageBox.Show("조회 요청에 실패하였습니다. VDS 제어기 연결 상태를 확인하세요", "오류");
            }

            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"{MethodBase.GetCurrentMethod().ReflectedType.Name + ":" + MethodBase.GetCurrentMethod().Name} 종료 "));

        }

        public void AddTrafficDataStatResult(IEnumerable<TRAFFIC_DATA> trafficDataList)
        {
            //lvTrafficDataStat.Items.Clear();
            foreach (var trafficData in trafficDataList)
            {
                AddTrafficDataStatToList(trafficData);
            }
            //lbTotalCount.Text = String.Format($"전체 갯수:  {trafficDataList.Count()} 개");

        }


        public void AddTrafficDataStatToList(TRAFFIC_DATA data)
        {
            ListViewItem item;
            // 제목, 콘텐츠 유형,  조회수, 별점수, 의뢰인, 코치, 등록일
            item = new ListViewItem(data.DETECT_TIME); // 

            item.SubItems.Add(data.LANE.ToString());
            item.SubItems.Add(data.DIRECTION == 1 ? "상행선" : "하행선");
            item.SubItems.Add(data.LENGTH.ToString());
            item.SubItems.Add(data.SPEED.ToString());
            item.SubItems.Add(data.OCCUPY_TIME.ToString());
            item.SubItems.Add(data.LOOP1_OCCUPY_TIME.ToString());
            item.SubItems.Add(data.LOOP2_OCCUPY_TIME.ToString());
            item.SubItems.Add(data.REVERSE_RUN_YN);
            item.SubItems.Add(data.REPORT_YN);
            item.Tag = data;
            lvTrafficDataStat.Items.Add(item);
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            if (lvTrafficDataStat.Items.Count ==0)
            {
                MessageBox.Show("조회 결과가 없습니다", "정보");
                return;
            }

            String fileName = String.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "저장경로 및 파일명을 입력하세요";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "CSV file(*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                SaveTrafficDataToCSV(fileName);
                MessageBox.Show(fileName + "에 저장하였습니다", "저장");
            }
        }

        public bool SaveTrafficDataToCSV(String fileName)
        {
            bool result = false;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                file.WriteLine("ID, 차선, 방향, 길이, 속도, 차량종류, 점유시간, 점유시간1, 점유시간2, 역주행여부, 차두거리,검지시간, 등록일");

                for(int i = 0;i< lvTrafficDataStat.Items.Count;i++)
                {
                    var trafficData = (TRAFFIC_DATA)lvTrafficDataStat.Items[i].Tag;
                    if(trafficData!=null)
                    {
                        file.WriteLine($"{trafficData.ID}, {trafficData.LANE}, {trafficData.DIRECTION}, {trafficData.LENGTH}, {trafficData.SPEED}, {trafficData.VEHICLE_CLASS}, {trafficData.OCCUPY_TIME}, {trafficData.LOOP1_OCCUPY_TIME}, {trafficData.LOOP2_OCCUPY_TIME}, {trafficData.REVERSE_RUN_YN}, {trafficData.VEHICLE_GAP},{trafficData.DETECT_TIME}, {trafficData.REG_DATE}");
                    }

                }
            }
            return result;
        }

        private void darkButton5_Click_1(object sender, EventArgs e)
        {
            if (currentPage < totalPage)
                currentPage++;
            else
            {
                MessageBox.Show("마지막 페이지입니다", "정보", MessageBoxButtons.OK);
                return;
            }
            SearchTrafficData(currentPage, GlobalCommonData.PAGE_SIZE);
        }
    }
}
