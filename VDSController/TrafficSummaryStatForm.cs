using DarkUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSDBHandler;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;

namespace VDSController
{
    public partial class TrafficSummaryStatForm : DarkForm
    {
        public List<int> laneList = new List<int>();

        public TrafficSummaryStatForm()
        {
            InitializeComponent();

            laneList = VDSConfig.ToRIghtLaneGroup.LaneList.Select(x => x.Lane).ToList().Concat(VDSConfig.ToLeftLaneGroup.LaneList.Select(x => x.Lane).ToList()).OrderBy(p=>p).ToList();

            foreach(var lane in laneList)
            {
                cbLane.Items.Add(lane);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }


        public void SetInitialInfo(int lane, String startDate)
        {

            cbLane.SelectedIndex = laneList.IndexOf(lane);
            ucStartTime.SetDateTime(startDate);
            ucEndTime.SetDateTime(DateTime.Now.ToString("yyyyMMddHHmmss"));

            button1_Click(null, null);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            String startDate = ucStartTime.GetDateTimeFormat();
            String endDate = ucEndTime.GetDateTimeFormat();
            int lane = 0;
            if (cbLane.SelectedIndex >= 0)
                lane = laneList[cbLane.SelectedIndex];
            var result = GetTargetSummaryInfoData(startDate, endDate, lane);

            AddTargetSummaryInfoToList(result);
        }

        public IEnumerable<TARGET_SUMMARY_INFO> GetTargetSummaryInfoData(String startDate, String endDate, int lane)
        {
            TargetSummary db = new TargetSummary(VDSConfig.VDS_DB_CONN);
            var result = db.GetTargetSummaryByLaneList(new TARGET_SUMMARY_INFO()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                I_LANE = lane
            }, out SP_RESULT spResult);

            if (!spResult.IS_SUCCESS)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);
                
            }
            return result;
        }

        public void AddTargetSummaryInfoToList(IEnumerable<TARGET_SUMMARY_INFO> targetList)
        {
            lbxTarget.Items.Clear();
            foreach(var targetInfo in targetList)
            {
                String info = String.Format("[{0}]\t ID: T{1:D3}T{2:D3}\t 속도: {3} km/h\t 길이: {4} m\t 점유시간: {5:f3} msec",
                                        targetInfo.CREATE_TIME, targetInfo.ID_0, targetInfo.ID_1, targetInfo.SPEED_X100, targetInfo.LENGTH_X100 / 100,
                                        targetInfo.OCCUPY_TIME);
                lbxTarget.Items.Add(info);

            }
            lbTotalCount.Text = String.Format($"전체 갯수:  {targetList.Count()} 개");

        }

        private void TrafficSummaryStatForm_Load(object sender, EventArgs e)
        {

        }

        public IEnumerable<TARGET_SUMMARY_INFO> GetTrafficStatList(String startDate, String endDate, int lane)
        {
            TargetSummary db = new TargetSummary(VDSConfig.VDS_DB_CONN);
            var result = db.GetTargetSummaryByLaneList(new TARGET_SUMMARY_INFO()
            {
                I_START_DATE = startDate,
                I_END_DATE = endDate,
                I_LANE = lane
            }, out SP_RESULT spResult);

            if (!spResult.IS_SUCCESS)
            {
                Utility.AddLog(LOG_TYPE.LOG_ERROR, spResult.ERROR_MESSAGE);

            }
            return result;
        }
    }
}
