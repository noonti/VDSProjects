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

namespace VDSController
{
    public partial class ucTargetSummaryInfo : UserControl
    {
        Color panelOriginalColor;
        DateTime _displayTime = DateTime.Now;
        Timer _currentTimer = null;

        public int _moveInterval = 100; // 

        //public bool DoCount = false;
        public Int64 VecycleCount { get; set; }
        public TrafficLane lane;
        public DateTime resetTime = DateTime.Now;



        public ucTargetSummaryInfo()
        {
            InitializeComponent();

            
            _currentTimer = new Timer();
            _currentTimer.Interval = 200; // 
            _currentTimer.Tick += new EventHandler(Timer_Tick);
            _currentTimer.Start();


            //carLane.StartTimer();

            panelOriginalColor = splitLane.Panel1.BackColor;
            VecycleCount = 0;

        }

        public void SetLaneName(String laneName)
        {
            lbLane.Text = laneName;
           

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var nowDate = DateTime.Now;
            if (_displayTime != null && nowDate > _displayTime)
            {
                //Console.WriteLine("hide...");
                splitLane.Panel1.BackColor = panelOriginalColor;
            }
            
        }


       
        public int AddTargetInfo(TrafficDataEvent trafficDataEvent)
        {
      
            //String info = String.Format(" 시간      : {0} 차선:{4}  속도      : {1} km/h 길이      : {2} cm 점유시간 : {3:f3} msec",
            //                            trafficDataEvent.detectTime, trafficDataEvent.speed , trafficDataEvent.length ,
            //                            trafficDataEvent.occupyTime, trafficDataEvent.lane);

            //Console.WriteLine(info);

            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500); //5초 후에 초기화
            _displayTime = DateTime.Now.Add(duration);

            splitLane.Panel1.BackColor = Color.Blue;
            VecycleCount++;
            SetVecycleCount(VecycleCount);
            segmentCount.Value = VecycleCount.ToString();


            AddRealTimeTrafficDataList(trafficDataEvent);

            return 1;
        }

        private void AddRealTimeTrafficDataList(TrafficDataEvent trafficDataEvent)
        {
            if (lvTrafficData.Items.Count > 1000)
                lvTrafficData.Items.RemoveAt(lvTrafficData.Items.Count - 1);

            ListViewItem item;
           
            item = new ListViewItem(trafficDataEvent.detectTime); // 

            item.SubItems.Add(trafficDataEvent.lane.ToString());
            //item.SubItems.Add(trafficDataEvent.direction == 1 ? "상행선" : "하행선");
            item.SubItems.Add(Utility.GetLaneGroupName(trafficDataEvent.direction));
            item.SubItems.Add(trafficDataEvent.length.ToString());
            item.SubItems.Add(trafficDataEvent.speed.ToString());
            item.SubItems.Add(trafficDataEvent.occupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.loop1OccupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.loop2OccupyTime.ToString());
            item.SubItems.Add(trafficDataEvent.reverseRunYN);
            item.SubItems.Add(trafficDataEvent.reportYN != null ? trafficDataEvent.reportYN : "N");
            lvTrafficData.Items.Insert(0, item);

        }


        public void SetVecycleCount(long count)
        {
            VecycleCount = count;
            segmentCount.Value = VecycleCount.ToString();
        }
        public void ResetVecycleCount(DateTime time)
        {
            resetTime = time;
            VecycleCount = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TrafficSummaryStatForm statForm = new TrafficSummaryStatForm();
            //statForm.SetInitialInfo(lane.lane, resetTime.ToString("yyyyMMddHHmmss"));
            //statForm.ShowDialog();
        }

        private void segmentCount_Load(object sender, EventArgs e)
        {

        }
    }
}
