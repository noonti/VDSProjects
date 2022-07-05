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
            carLane._interval = _moveInterval;
            carLane._lane = lane.lane;
            carLane.SetCarDirection(lane.travel_direction);

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


        public void MoveCar()
        {
            carLane.MoveCar();
        }
        public int AddTargetInfo(TrafficDataEvent trafficDataEvent)
        {
            //if (lbxTarget.Items.Count > 1000)
            //    lbxTarget.Items.RemoveAt(lbxTarget.Items.Count - 1);

            String info = String.Format(" 시간      : {0} \n 속도      : {1} km/h\n 길이      : {2} cm\n 점유시간 : {3:f3} msec",
                                        trafficDataEvent.detectTime, trafficDataEvent.speed , trafficDataEvent.length ,
                                        trafficDataEvent.occupyTime);
            //lbxTarget.Items.Insert(0, info);

            lbTarget.Text = info;

            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500); //5초 후에 초기화
            _displayTime = DateTime.Now.Add(duration);

            splitLane.Panel1.BackColor = Color.Blue;
            VecycleCount++;
            SetVecycleCount(VecycleCount);
            segmentCount.Value = VecycleCount.ToString();


            if(VDSConfig.controllerConfig.UseAnimation==1)
                carLane.AddCar(trafficDataEvent); // avogadro addcar

            return 1;
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
            TrafficSummaryStatForm statForm = new TrafficSummaryStatForm();
            statForm.SetInitialInfo(lane.lane, resetTime.ToString("yyyyMMddHHmmss"));
            statForm.ShowDialog();
        }

        private void segmentCount_Load(object sender, EventArgs e)
        {

        }
    }
}
