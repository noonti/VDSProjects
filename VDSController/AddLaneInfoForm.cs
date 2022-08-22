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
using VDSDBHandler.Model;

namespace VDSController
{
    public partial class AddLaneInfoForm : DarkForm
    {
        public String laneName = String.Empty;
        public int lane = 0;
        public AddLaneInfoForm()
        {
            InitializeComponent();
        }

        public void SetLaneInfo(LANE_INFO laneInfo)
        {
            laneName = laneInfo.LANE_NAME;
            lane = laneInfo.LANE - 1;
        }
        private void darkButton10_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtLaneName.Text))
            {
                MessageBox.Show("입력오류", "차선명을 입력하세요");
                return;
            }

            laneName = txtLaneName.Text;

            if (cbLane.SelectedIndex == -1)
            {
                MessageBox.Show("입력오류", "차선을 선택하세요");
                return;
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
            cbLane.SelectedIndex = lane ;
        }
    
    }
}
