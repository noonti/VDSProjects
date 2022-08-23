using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon;
using VDSDBHandler.DBOperation;
using VDSDBHandler.Model;
using VDSDBHandler;
using VDSCommon.API.Model;

namespace VDSController
{
    public partial class ucTrafficDataStat : UserControl
    {
        List<TRAFFIC_DATA> trafficDataList = new List<TRAFFIC_DATA>();
        public ucTrafficDataStat()
        {
            InitializeComponent();
        }

        public async void SearchTrafficData(TRAFFIC_DATA data)
        {
            TrafficDataOperation db = new TrafficDataOperation(VDSConfig.VDS_DB_CONN);
            trafficDataList.Clear();

            SP_RESULT spResult;
            var task1 = Task.Run(() =>{
                return db.GetTrafficDataList(data, out spResult).ToList();
            }) ;

            trafficDataList = await task1;
            AddTrafficDataResult(trafficDataList);
        }

        public void AddTrafficDataResult(IEnumerable<TRAFFIC_DATA> trafficDataList)
        {
            lvTrafficData.Items.Clear();
            foreach (var trafficData in trafficDataList)
            {
                AddTrafficDataToList(trafficData);
            }
            lbTotalCount.Text = String.Format($"전체 갯수:  {trafficDataList.Count()} 개");

        }

        public void AddTrafficDataToList(TRAFFIC_DATA data)
        {
            ListViewItem item;
            // 제목, 콘텐츠 유형,  조회수, 별점수, 의뢰인, 코치, 등록일
            item = new ListViewItem(data.DETECT_TIME); // 

            item.SubItems.Add(data.LANE.ToString());
            //item.SubItems.Add(data.DIRECTION == 1?"상행선":"하행선");
            item.SubItems.Add(Utility.GetLaneGroupName(data.DIRECTION));
            item.SubItems.Add(data.LENGTH.ToString());
            item.SubItems.Add(data.SPEED.ToString());
            item.SubItems.Add(data.OCCUPY_TIME.ToString());
            item.SubItems.Add(data.LOOP1_OCCUPY_TIME.ToString());
            item.SubItems.Add(data.LOOP2_OCCUPY_TIME.ToString());
            item.SubItems.Add(data.REVERSE_RUN_YN);
            item.SubItems.Add(data.REPORT_YN);
            lvTrafficData.Items.Add(item);
        }

        public bool SaveTrafficDataToCSV(String fileName)
        {
            bool result = false;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                file.WriteLine("ID, 차선, 방향, 길이, 속도, 차량종류, 점유시간, 점유시간1, 점유시간2, 역주행여부, 차두거리,검지시간, 등록일");

                foreach(var trafficData in trafficDataList)
                {
                    file.WriteLine($"{trafficData.ID}, {trafficData.LANE}, {trafficData.DIRECTION}, {trafficData.LENGTH}, {trafficData.SPEED}, {trafficData.VEHICLE_CLASS}, {trafficData.OCCUPY_TIME}, {trafficData.LOOP1_OCCUPY_TIME}, {trafficData.LOOP2_OCCUPY_TIME}, {trafficData.REVERSE_RUN_YN}, {trafficData.VEHICLE_GAP},{trafficData.DETECT_TIME}, {trafficData.REG_DATE}");
                }
            }
            return result;
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            String fileName = String.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "저장경로 및 파일명을 입력하세요";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "CSV file(*.csv)|*.csv";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                SaveTrafficDataToCSV(fileName);
                MessageBox.Show(fileName + "에 저장하였습니다", "저장");
            }
            
        }
    }
}
