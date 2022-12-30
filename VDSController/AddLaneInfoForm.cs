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
using VDSDBHandler.Model;

namespace VDSController
{
    public partial class AddLaneInfoForm : DarkForm
    {
        public String laneName = String.Empty;
        public int lane = 0;
        public int korExLane = 0;
        public AddLaneInfoForm()
        {
            InitializeComponent();
        }

        public void SetLaneInfo(LANE_INFO laneInfo)
        {
            laneName = laneInfo.LANE_NAME;
            lane = laneInfo.LANE;
            korExLane = laneInfo.KOREX_LANE;

        }
        private void darkButton10_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtLaneName.Text))
            {
                Utility.ShowMessageBox("입력오류", "차선명을 입력하세요", 1);
                return;
            }

            laneName = txtLaneName.Text;

            if (cbLane.SelectedIndex == -1)
            {
                Utility.ShowMessageBox("입력오류", "차선을 선택하세요", 1);
                return;
            }

            if(VDSConfig.controllerConfig.ProtocolType == 2) // 도로공사용
            {
                if (cbKorExLane.SelectedIndex == -1)
                {
                    Utility.ShowMessageBox("입력오류", "도로공사 차선을 선택하세요", 1);
                    return;
                }
                korExLane = int.Parse(cbKorExLane.Text);
            }
            lane = int.Parse(cbLane.Text);
            DialogResult = DialogResult.OK;
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void AddLaneInfoForm_Activated(object sender, EventArgs e)
        {
            txtLaneName.Text = laneName;
            cbLane.SelectedIndex = lane - 1;
            cbKorExLane.SelectedIndex = korExLane - 1;
        }
    
    }
}
