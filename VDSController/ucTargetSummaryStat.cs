using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSDBHandler.DBOperation;
using VDSDBHandler;
using VDSCommon;

namespace VDSController
{
    public partial class ucTargetSummaryStat : UserControl
    {
        Timer _searchTimer = null;
        public ucTargetSummaryStat()
        {
            InitializeComponent();
            InitializeDate();
        }

        private void rdgSelect_Click(object sender, EventArgs e)
        {
            int tag = 0;
            tag = int.Parse((sender as RadioButton).Tag.ToString());
            SetSearchDate(tag);
        }
           

        private void button1_Click(object sender, EventArgs e)
        {
            GetTargetSumaryStat();
        }

        private void InitializeDate()
        {
                    // 오늘로 부터 1일 전부터 현재까지 시간 설정
            DateTime curDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;

            startDate = curDate - new TimeSpan(0, 24, 0, 0);
            endDate = curDate;
            ucStartTime.SetDateTime(startDate.ToString("yyyyMMddHHmmss"));
            ucEndTime.SetDateTime(endDate.ToString("yyyyMMddHHmmss"));
        }

        private void SetSearchDate(int tag)
        {
            DateTime curDate = DateTime.Now;
            DateTime startDate;
            DateTime endDate;
            if(tag == 0)
            {
                StopTimer();
            }
            else
            {
                        // 현재 시간으로 부터 5분 또는 60분 이전 으로 설정
                startDate = curDate - new TimeSpan(0, 0, tag / 60, 0);
                endDate = curDate;
                ucStartTime.SetDateTime(startDate.ToString("yyyyMMddHHmmss"));
                ucEndTime.SetDateTime(endDate.ToString("yyyyMMddHHmmss"));
            }
        }

        private void StartTimer(int second)
        {
            StopTimer();

            if (_searchTimer == null)
                _searchTimer = new Timer();
            _searchTimer.Interval = second * 1000;
            _searchTimer.Tick += Timer_Tick;
            _searchTimer.Start();

        }
        private void StopTimer()
        {
            if (_searchTimer != null)
            {
                _searchTimer.Stop();
                _searchTimer = null;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!rdgSelect.Checked && !rdgManual.Checked)  // 날짜 선택이 아닌 경우 즉 5분, 1시간 단위 조회이고 수동이 아닌 경우에만
            {
                int tag = 0;
                
                if (rdg5Minute.Checked)
                    tag = int.Parse(rdg5Minute.Tag.ToString());
                else if(rdg60Minute.Checked)
                    tag = int.Parse(rdg60Minute.Tag.ToString());

                if (tag > 0)
                {
                    SetSearchDate(tag);
                    GetTargetSumaryStat();
                }
            }
        }

        private void GetTargetSumaryStat()
        {
            TextBox txtBox = null;
            String strStartDate, strEndDate;
            ResetStatInfo();
            strStartDate = ucStartTime.GetDateTime();
            strEndDate = ucEndTime.GetDateTime();
            TargetSummary targetDB = new TargetSummary(VDSConfig.VDS_DB_CONN);
            var result = targetDB.GetTargetSummaryStat(new VDSDBHandler.Model.TARGET_SUMMARY_INFO()
            {
                I_START_DATE = strStartDate,
                I_END_DATE = strEndDate

            }, out SP_RESULT spResult);
            if (!spResult.IS_SUCCESS)
            {
                Console.WriteLine(spResult.ERROR_MESSAGE);
            }
            else
            {
                foreach(var stat in result)
                {
                    switch(stat.LANE)
                    {
                        case 1:
                            txtBox = txtLane1;
                            break;
                        case 2:
                            txtBox = txtLane2;
                            break;
                        case 3:
                            txtBox = txtLane3;
                            break;
                        case 4:
                            txtBox = txtLane4;
                            break;

                        case 5:
                            txtBox = txtLane5;
                            break;

                        case 6:
                            txtBox = txtLane6;
                            break;
                    }
                    if (txtBox != null)
                        txtBox.Text = stat.COUNT.ToString();
                }
            }
            DateTime startDate = DateTime.ParseExact(strStartDate, "yyyyMMddHHmmss", null);
            DateTime endDate = DateTime.ParseExact(strEndDate, "yyyyMMddHHmmss", null);
            lbLastCheckDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + String.Format($" [{startDate.ToString("yyyy-MM-dd HH:mm:ss")} - {endDate.ToString("yyyy-MM-dd HH:mm:ss")}] ") ;
        }

        private void rdgManual_Click(object sender, EventArgs e)
        {
            int tag = 0;
            tag = int.Parse((sender as RadioButton).Tag.ToString());
            if (tag == 0)
                StopTimer();
            else
            {
                StartTimer(tag);
            }
        }

        private void ResetStatInfo()
        {
            txtLane1.Text = "0";
            txtLane2.Text = "0";
            txtLane3.Text = "0";
            txtLane4.Text = "0";
            txtLane5.Text = "0";
            txtLane6.Text = "0";


        }
    }
}
