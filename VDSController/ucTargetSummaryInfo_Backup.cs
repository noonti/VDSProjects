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
    public partial class ucTargetSummaryInfo_Backup : UserControl
    {
        Color panelOriginalColor;
        DateTime _displayTime = DateTime.Now;
        Timer _currentTimer = null;

        public bool DoCount = false;
        public Int64 VecycleCount { get; set; }
        public TrafficLane lane;




        public ucTargetSummaryInfo_Backup()
        {
            InitializeComponent();

            _currentTimer = new Timer();
            _currentTimer.Interval = 100; // 1초마다 체크
            _currentTimer.Tick += new EventHandler(Timer_Tick);
            _currentTimer.Start();

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

        public int AddTargetInfo(TargetSummaryInfo targetInfo)
        {
            if (lbxTarget.Items.Count > 1000)
                lbxTarget.Items.RemoveAt(lbxTarget.Items.Count - 1);

            String info = String.Format("[{0}]\t 방항:{7} \t ID: T{1:D3}T{2:D3}\t 속도: {3} km/h\t 길이: {4} m\t Range: {5} m\t 점유시간: {6:f3} msec",
                                        targetInfo.CREATE_DATE.ToString(VDSConfig.RADAR_TIME_FORMAT), targetInfo.ID_0, targetInfo.ID_1, targetInfo.SPEED_X100 , targetInfo.LENGTH_X100/100 ,
                                        targetInfo.RANGE_X100 , targetInfo.OCCUPY_TIME, targetInfo.TRAVEL_DIRECTION == lane.travel_direction?"순방향":"역방향");
            lbxTarget.Items.Insert(0, info);

            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500); //5초 후에 초기화
            _displayTime = DateTime.Now.Add(duration);

            splitLane.Panel1.BackColor = Color.Blue;
            if(DoCount)
            {
                VecycleCount++;

            }
            lbVecycleCount.Text = VecycleCount.ToString();

            return 1;
        }

        public void SetVecycleCount(bool doCount)
        {
            DoCount = doCount;
            if (DoCount)
                VecycleCount = 0;
        }
        public void ResetVecycleCount()
        {
            VecycleCount = 0;
        }
    }
}
