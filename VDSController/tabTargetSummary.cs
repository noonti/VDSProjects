using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.DataType;
using VDSCommon;
using System.Threading;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;
using VDSDBHandler;
using VDSCommon.API.Model;

namespace VDSController
{
    public partial class tabTargetSummary : UserControl
    {

        // 8차선 까지 지원
        PictureBox[] pbxLeft = new PictureBox[8];
        // 8차선 까지 지원
        PictureBox[] pbxRight = new PictureBox[8];

        List<ucTargetSummaryInfo> ucTargetSummaryInfo = new List<ucTargetSummaryInfo>();
        public MainForm frmMain;

        public String rtspStreamingUrl = String.Empty;
        public StartRTSPStreamingDelegate _startRTSPStreaming = null;


        

        public List<int> laneList = new List<int>();

        //슬라이딩 메뉴의 최대, 최소 폭 크기
        int MAX_SLIDING_WIDTH = 1005;
        int MIN_SLIDING_WIDTH = 30;
        //슬라이딩 메뉴가 보이는/접히는 속도 조절
        const int STEP_SLIDING = 10;
        //최초 슬라이딩 메뉴 크기


        public tabTargetSummary()
        {
            InitializeComponent();
             _startRTSPStreaming = StartRTSPStreaming;

            
        }

        public void StartService()
        {

            lbServiceStartTime.Text = String.Format($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]");
           
        }

        public void StopService()
        {
            lbServiceStartTime.Text = String.Format("[중지상태]");
            StopRTSPStreaming();
            
        }

        

        private void InitializeLaneInfo()
        {
            
            gbxLeft.Text = VDSConfig.ToLeftLaneGroup.LaneGroupName;
            
            var leftLaneList = VDSConfig.ToLeftLaneGroup.LaneSort == 1 ? VDSConfig.ToLeftLaneGroup.LaneList.OrderBy(x => x.Lane).ToList()
                                                          : VDSConfig.ToLeftLaneGroup.LaneList.OrderByDescending(x => x.Lane).ToList();
            SetUcTrgetInfo(leftLaneList, gbxLeft, pbxLeft);
            gbxRight.Text = VDSConfig.ToRIghtLaneGroup.LaneGroupName;

            var rightLaneList = VDSConfig.ToRIghtLaneGroup.LaneSort == 1 ? VDSConfig.ToRIghtLaneGroup.LaneList.OrderBy(x => x.Lane).ToList()
                                                          : VDSConfig.ToRIghtLaneGroup.LaneList.OrderByDescending(x => x.Lane).ToList();
            SetUcTrgetInfo(rightLaneList, gbxRight, pbxRight);


            laneList = VDSConfig.ToRIghtLaneGroup.LaneList.Select(x => x.Lane).ToList().Concat(VDSConfig.ToLeftLaneGroup.LaneList.Select(x => x.Lane).ToList()).OrderBy(p => p).ToList();

            foreach (var data in laneList)
            {
                cbLane.Items.Add(data);
            }
            cbLane.Items.Insert(0, "전체");
        }

        private void SetUcTrgetInfo(List<LaneInfo>laneList, GroupBox gbx, PictureBox[] pbxList)
        {
            ucTargetSummaryInfo ucTargetInfo;
            for (int i = 0; i < laneList.Count; i++)
            {
                pbxList[i] = new PictureBox();
                pbxList[i].Parent = gbx;

                pbxList[i].Anchor = AnchorStyles.Left | AnchorStyles.Right;

                ucTargetInfo = new ucTargetSummaryInfo();
                ucTargetInfo.Dock = DockStyle.Fill;
                ucTargetInfo.lane = new TrafficLane();
                ucTargetInfo.lane.lane = laneList[i].Lane;
                ucTargetInfo.lane.laneName = laneList[i].LaneName;
                ucTargetInfo.lane.travel_direction = laneList[i].Direction; // 
                
                ucTargetInfo.SetLaneName(laneList[i].LaneName);
                pbxList[i].Controls.Add(ucTargetInfo);
                ucTargetSummaryInfo.Add(ucTargetInfo);
                gbx.Controls.Add(pbxList[i]);

            }
        }

        public int AddTargetInfo(TrafficDataEvent trafficDataEvent)
        {


            foreach(var targetSummary in ucTargetSummaryInfo)
            {
                if(targetSummary!=null && targetSummary.lane.travel_direction == trafficDataEvent.direction && targetSummary.lane.lane == trafficDataEvent.lane)
                {
                    targetSummary.AddTargetInfo(trafficDataEvent);
                }
            }

            AddRealTimeTrafficDataList(trafficDataEvent);
         
            return 1;
        }

        public void InitializeRTSPPlayer()
        {
            rtspPlayer.SetRTSPLogDelegate(this, new FormAddLogDelegate(AddRTSPLog));
            if(VDSConfig.controllerConfig!=null)
            {
                if (VDSConfig.controllerConfig.DeviceType == 1)
                    chkExpandPlayer.Checked = true;
                else
                    chkExpandPlayer.Checked = false;   
            }
            else
                chkExpandPlayer.Checked = false;   
            ExpandPnPlayer(chkExpandPlayer.Checked);
        }
        
        public int AddRTSPLog(LOG_TYPE logType, String log)
        {
            Console.WriteLine(String.Format(" AddRTSPLog : {0}",log));

            //if (lbxVideoLog.Items.Count > 1000)
            //    lbxVideoLog.Items.RemoveAt(lbxVideoLog.Items.Count - 1);
           
            //lbxVideoLog.Items.Insert(0, log);


            return 1;
        }

        
        public int SetRtspStreamingUrl(String[] url)
        {
            rtspStreamingUrl = url[0];

            BeginInvoke(_startRTSPStreaming, null);
            return 1;
        }
        public int StartRTSPStreaming()
        {
            rtspStreamingUrl = VDSConfig.controllerConfig.StreamingURL;
            //String url = String.Format($"rtsp://{VDSConfig.CCTV_ID}:{VDSConfig.CCTV_PWD}@{VDSConfig.CCTV_ADDRESS}");
            if (!String.IsNullOrWhiteSpace(rtspStreamingUrl))
                rtspPlayer.StartStreaming(true, rtspStreamingUrl);
            return 1;
        }

        public int StopRTSPStreaming()
        {
            rtspPlayer.StopStreaming();
            return 1;
        }

        



        public void ResetVecycleCount()
        {
            DateTime curDate = DateTime.Now;
            foreach (var summary in ucTargetSummaryInfo)
            {
                if (summary != null)
                {
                    summary.ResetVecycleCount(curDate);// (doCount);
                }
                    
            }
            //if(doCount)
            //    lbLastCountTime_.Text = "[" + curDate.ToString("yyyy-MM-dd HH:mm:ss") + "]";
        }

        private void tabTargetSummary_Resize(object sender, EventArgs e)
        {
            //MAX_SLIDING_WIDTH = pnPlayer.Width;
            ResizeContainer(panelContainer.Width, panelContainer.Height);
        }


        
        private void ResizeContainer(int width, int height)
        {
            int margin = 10;

            Console.WriteLine("ResizeContainer {0}x{1}",width, height);
            if (VDSConfig.ToLeftLaneGroup.LaneList.Count == 0) // gbxRight 가 전체 차지 한다.
            {
                gbxRight.Left = margin;
                gbxRight.Top = margin;
                gbxRight.Height = height - margin * 2;
                gbxRight.Width = width - margin * 2;

                ResizeControl(gbxRight, pbxRight, VDSConfig.ToRIghtLaneGroup.LaneList.Count);
                gbxRight.Visible = true;
                gbxLeft.Visible = false;
            }
            else if (VDSConfig.ToRIghtLaneGroup.LaneList.Count == 0)
            {
                gbxLeft.Left = margin;
                gbxLeft.Top =  margin;
                gbxLeft.Height = height - margin * 2;
                gbxLeft.Width = width - margin * 2;


                ResizeControl(gbxLeft, pbxLeft, VDSConfig.ToLeftLaneGroup.LaneList.Count);
                gbxRight.Visible = false;
                gbxLeft.Visible = true;
            }
            else
            {
                gbxLeft.Left = margin;
                gbxLeft.Top =  margin;
                gbxLeft.Height = height * VDSConfig.ToLeftLaneGroup.LaneList.Count / (VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count) - margin;
                gbxLeft.Width = width - margin * 2;


                ResizeControl(gbxLeft, pbxLeft, VDSConfig.ToLeftLaneGroup.LaneList.Count);

                gbxRight.Left = margin;
                gbxRight.Top = gbxLeft.Top + gbxLeft.Height + margin;
                gbxRight.Height = height * VDSConfig.ToRIghtLaneGroup.LaneList.Count / (VDSConfig.ToLeftLaneGroup.LaneList.Count + VDSConfig.ToRIghtLaneGroup.LaneList.Count) - margin;
                gbxRight.Width = width - margin * 2;


                ResizeControl(gbxRight, pbxRight, VDSConfig.ToRIghtLaneGroup.LaneList.Count);

                gbxRight.Visible = true;
                gbxLeft.Visible = true;
            }
        }


        //private void ResizeContainer(int width, int height)
        //{
        //    int count1 = 0;
        //    int count2 = 0;
        //    int margin = 10;
        //    count1 = 0;
        //    count2 = 4;

        //    gbxLeft.Height = height * count1 / (count1 + count2) - margin;
        //    gbxLeft.Top = margin;
        //    //ResizeControl(gbxLeft, pbxLeft, count1);

        //    gbxRight.Height = height * count2 / (count1 + count2) - margin;
        //    gbxRight.Top = gbxLeft.Top + gbxLeft.Height + margin;
        //    //ResizeControl(gbxRight, pbxRight, count2);


        //}

        private void ResizeControl(GroupBox grpBox, PictureBox[] picList, int count)
        {

            int margin = 10;

            if (count == 0)
                return;

            // 실제 그릴 영역
            int left = margin;
            int top = margin;
            int width = grpBox.Width - margin * 2;
            int height = (grpBox.Height - margin * 2) / count;


            // control 사이즈
            int pbxWidth = width;
            int pbxHeight = height - margin / 2;
            int startY = margin * 2;

            for (int i = 0; i < count; i++)
            {
                if (picList[i] != null)
                {
//                    picList[i].BackColor = Color.Yellow;
                    picList[i].Location = new Point(left, startY);
                    picList[i].Width = pbxWidth;
                    picList[i].Height = pbxHeight;
                    startY += height;

                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StartRTSPStreaming();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            StopRTSPStreaming();
        }

        private void tabTargetSummary_Load(object sender, EventArgs e)
        {
            InitializeLaneInfo();
            InitializeRTSPPlayer();
            //StartCarMovingTimer();

            ResizeContainer(panelContainer.Width, panelContainer.Height);

        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            SyncTrafficDataCount();
        }

        public void SyncTrafficDataCount()
        {
            //String startDate = ucStartDate.GetDateTimeFormat();
            //String endDate = DateTime.Now.ToString(VDSConfig.RADAR_TIME_FORMAT);

            //TargetSummary targetDB = new TargetSummary(VDSConfig.VDS_DB_CONN);
            //TRAFFIC_DATA_STAT stat = new TRAFFIC_DATA_STAT();

            //stat.I_START_DATE = startDate;
            //stat.I_END_DATE = endDate;

            //SP_RESULT spResult;
            //var trafficDataList = targetDB.GetTrafficDataStat(stat, out spResult).ToList();
            //if(trafficDataList!=null)
            //{
            //    foreach(var trafficData in trafficDataList)
            //    {
            //        var summary = ucTargetSummaryInfo.Where(x => x.lane.lane == trafficData.LANE).FirstOrDefault();
            //        if (summary != null)
            //            summary.SetVecycleCount(trafficData.TRAFFIC_COUNT);
            //    }
            //}
        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            String startDate = ucStartTime.GetDateTimeFormat();
            String endDate = ucEndTime.GetDateTimeFormat();
            int lane = 0;
            if (cbLane.SelectedIndex > 0)
                lane = laneList[cbLane.SelectedIndex - 1];




            TRAFFIC_DATA data = new TRAFFIC_DATA()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                //I_REPORT_YN = "Y",
                LANE = lane,

            };
            ucCenterTrafficData.SearchTrafficData(data);
        }

        private void ucCenterTrafficData_Load(object sender, EventArgs e)
        {

        }

        private void ucStartTime_Load(object sender, EventArgs e)
        {

        }


        private void AddRealTimeTrafficDataList(TrafficDataEvent trafficDataEvent)
        {
            if (lvTrafficData.Items.Count > 1000)
                lvTrafficData.Items.RemoveAt(lvTrafficData.Items.Count - 1);
            
            ListViewItem item;

            item = new ListViewItem(trafficDataEvent.detectTime); // 

            item.SubItems.Add(trafficDataEvent.lane.ToString());
            //item.SubItems.Add(trafficDataEvent.direction == 1 ? "상행선" : "하행선"); // 1: TO Right  2: TO Left
            item.SubItems.Add(Utility.GetLaneGroupName(trafficDataEvent.direction));
            item.SubItems.Add(trafficDataEvent.length.ToString());
            item.SubItems.Add(trafficDataEvent.speed.ToString());
            item.SubItems.Add(trafficDataEvent.occupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.loop1OccupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.loop2OccupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.reverseRunYN);
            item.SubItems.Add(trafficDataEvent.reportYN!=null? trafficDataEvent.reportYN : "N");
            lvTrafficData.Items.Insert(0,item);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ExpandPnPlayer(chkExpandPlayer.Checked);

        }

        private void timerSliding_Tick(object sender, EventArgs e)
        {
            
            //if (chkExpandPlayer.Checked == true)
            //{
            //    //슬라이딩 메뉴를 숨기는 동작
            //    _posSliding -= STEP_SLIDING;
            //    if (_posSliding <= MIN_SLIDING_WIDTH)
            //        timerSliding.Stop();
            //}
            //else
            //{
            //    //슬라이딩 메뉴를 보이는 동작
            //    _posSliding += STEP_SLIDING;
            //    if (_posSliding >= MAX_SLIDING_WIDTH)
            //        timerSliding.Stop();
            //}

            //pnPlayer.Width = _posSliding;
        }

        private void ExpandPnPlayer(bool expanded)
        {
            if(expanded) // 펼치기
            {
                chkExpandPlayer.Text = "<";
                pnPlayer.Width = MAX_SLIDING_WIDTH;

            }
            else         // 접기
            {
                //슬라이딩 메뉴가 접혔을 때, 메뉴 버튼의 표시
                chkExpandPlayer.Text = ">";
                pnPlayer.Width = MIN_SLIDING_WIDTH;
            }
            
        }

        private void darkButton4_Click(object sender, EventArgs e)
        {
            String startDate = ucTrafficStartTime.GetDateTimeFormat();
            String endDate = ucTrafficEndTime.GetDateTimeFormat();
            int lane = 0;
            if (cbTrafficLane.SelectedIndex > 0)
                lane = laneList[cbLane.SelectedIndex - 1];


            TRAFFIC_STAT data = new TRAFFIC_STAT()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                I_LANE = lane,

            };
            SearchTrafficStat(data);

        }


        public async void SearchTrafficStat(TRAFFIC_STAT data)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            SP_RESULT spResult;
            var task1 = Task.Run(() =>
            {
                return db.GetTrafficStatList(data, out spResult).ToList();
            });

            var trafficDataList = await task1;
            AddTrafficStatResult(trafficDataList);
        }

        public void AddTrafficStatResult(List<dynamic> trafficStatList)
        {

            lvTrafficStat.Items.Clear();
            ListViewItem item;
            foreach(var trafficStat in trafficStatList)
            {
                item = new ListViewItem(trafficStat.DETECT_DATE.ToString()); // 
                item.SubItems.Add(trafficStat.FRAME_NO.ToString());
                //item.SubItems.Add(trafficStat.lane_count.ToString());
                //item.SubItems.Add(trafficStat.report_yn.ToString());
                item.SubItems.Add(trafficStat.LANE.ToString());
                item.SubItems.Add(trafficStat.LARGE_COUNT.ToString());
                item.SubItems.Add(trafficStat.MIDDLE_COUNT.ToString());
                item.SubItems.Add(trafficStat.SMALL_COUNT.ToString());
                //item.SubItems.Add(trafficStat.speed.ToString());
                item.SubItems.Add(trafficStat.OCCUPY.ToString());
                item.SubItems.Add(trafficStat.CAR_LENGTH.ToString());
                lvTrafficStat.Items.Add(item);

            }
        }

        public async void SearchSpeedStat(SPEED_STAT data)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            //trafficDataList.Clear();

            SP_RESULT spResult;
            var task1 = Task.Run(() => {
                return db.GetSpeedStatList(data, out spResult).ToList();
            });

            var speedDataList = await task1;
            AddSpeedStatResult(speedDataList);
        }

        public void AddSpeedStatResult(List<dynamic> speedStatList)
        {

            lvSpeedStat.Items.Clear();
            ListViewItem item;
            foreach (var speedStat in speedStatList)
            {
                item = new ListViewItem(speedStat.DETECT_DATE.ToString()); // 
                item.SubItems.Add(speedStat.LANE.ToString());

                item.SubItems.Add(speedStat.CATEGORY_1_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_2_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_3_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_4_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_5_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_6_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_7_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_8_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_9_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_10_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_11_COUNT.ToString());
                item.SubItems.Add(speedStat.CATEGORY_12_COUNT.ToString());
                lvSpeedStat.Items.Add(item);

            }
        }

        private void darkButton5_Click(object sender, EventArgs e)
        {
            String startDate = ucSpeedStartTime.GetDateTimeFormat();
            String endDate = ucSpeedEndTime.GetDateTimeFormat();
            int lane = 0;
            if (cbSpeedLane.SelectedIndex > 0)
                lane = laneList[cbLane.SelectedIndex - 1];


            SPEED_STAT data = new SPEED_STAT()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                I_LANE = lane,

            };
            SearchSpeedStat(data);

        }
    }
}
