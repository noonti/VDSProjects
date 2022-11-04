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
using VDSCommon.API.Model;
using VDSDBHandler.DBOperation.VDSManage;
using VDSWebAPIServer.Common;

namespace VDSWebAPIServer.Forms
{
    public partial class VDSAddForm : DarkForm
    {
        public String operatonMode = String.Empty;

        public VDS_CONTROLLER selectedVDSCtrl = null;
        public VDS_CONTROLLER _vdsCtrl = null;
        CommonOperation commonOP = new CommonOperation();
        VDSControllerOperation vdsCtrlOp = new VDSControllerOperation();

        bool bInitalSet = false;

        public VDSAddForm()
        {
            InitializeComponent();
            ApiUtility.FillVDSGroupsComboBox(GlobalCommonData.vdsGroupsList,cbVDSGroups );
        }

       

        private VDS_CONTROLLER CheckForm()
        {
            VDS_CONTROLLER result = null;
            if (cbVDSGroups.SelectedIndex<0)
            {
                MessageBox.Show("소속그룹을 선택하세요", "입력 확인");
                return null;
            }

            if (String.IsNullOrEmpty(txtControllerName.Text))
            {
                MessageBox.Show("제어기명을 입력하세요", "입력 확인");
                return null;
            }

            if (String.IsNullOrEmpty(txtControllerId.Text))
            {
                MessageBox.Show("ID를 입력하세요", "입력 확인");
                return null;
            }

            if (String.IsNullOrEmpty(txtIPAddress.Text))
            {
                MessageBox.Show("IP Address를 입력하세요", "입력 확인");
                return null;
            }

            result = new VDS_CONTROLLER();

            result.GROUP_ID = GlobalCommonData.vdsGroupsList[cbVDSGroups.SelectedIndex].ID;
            result.CONTROLLER_NAME = txtControllerName.Text;
            result.CONTROLLER_ID = txtControllerId.Text;
            result.IP_ADDRESS = txtIPAddress.Text;
            result.PROTOCOL = rdgProtocol1.Checked ? 1 : 2;

            if (rdgVDSType1.Checked)
                result.VDS_TYPE = 1;
            else if (rdgVDSType2.Checked)
                result.VDS_TYPE = 2;
            else
                result.VDS_TYPE = 3;
            result.USE_YN = rdgUseY.Checked ? "Y" : "N";

            return result;
        }


        private bool AddVDSController(VDS_CONTROLLER vdsCtrl)
        {
            bool result = true;

            vdsCtrlOp.AddVDSController(ref vdsCtrl, out SP_RESULT spResult);
            if (spResult.RESULT_CODE.CompareTo("500") == 0)
            {
                MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                result = false;
            }
            else
            {
                MessageBox.Show("제어기 추가에 성공하였습니다", "저장", MessageBoxButtons.OK);
                result = true;
            }
            return result;
        }


        private bool UpdateVDSController(VDS_CONTROLLER vdsCtrl)
        {
            bool result = true;
            vdsCtrlOp.UpdateVDSController(vdsCtrl, out SP_RESULT spResult);

            if (spResult.RESULT_CODE.CompareTo("500") == 0)
            {
                MessageBox.Show(spResult.ERROR_MESSAGE, "오류", MessageBoxButtons.OK);
                result = false;
            }
            else
            {
                MessageBox.Show("제어기 수정에 성공하였습니다", "저장", MessageBoxButtons.OK);
                result = true;
            }
            return result;
        }


        private void darkButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void VDSAddForm_Activated(object sender, EventArgs e)
        {
            if(selectedVDSCtrl!=null && !bInitalSet)
            {
                SetVDSContollerInfo(selectedVDSCtrl);
            }
            bInitalSet = true;
        }
        
        public void SetVDSContollerInfo(VDS_CONTROLLER vdsController)
        {

            cbVDSGroups.SelectedIndex = GlobalCommonData.vdsGroupsList.FindIndex(x => x.ID == vdsController.GROUP_ID);
            txtControllerName.Text = vdsController.CONTROLLER_NAME;
            txtControllerId.Text = vdsController.CONTROLLER_ID;
            txtIPAddress.Text = vdsController.IP_ADDRESS;

            switch(vdsController.PROTOCOL)
            {
                case 1:
                    rdgProtocol1.Checked = true;
                    break;
                case 2:
                    rdgProtocol2.Checked = true;
                    break;
            }

            switch(vdsController.VDS_TYPE)
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
            SetEnable(operatonMode.CompareTo("VIEW") == 0 ? false : true);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            _vdsCtrl = CheckForm();
            bool bResult = false;
            if (_vdsCtrl != null)
            {
                switch (operatonMode)
                {
                    case "ADD":
                        bResult = AddVDSController(_vdsCtrl);
                        break;
                    case "UPDATE":
                        _vdsCtrl.ID = selectedVDSCtrl.ID;
                        bResult = UpdateVDSController(_vdsCtrl);
                        break;
                }
            }
            else
                return;

            DialogResult = DialogResult.OK;
        }

        private void darkButton1_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rdgProtocol2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btnSave_Click(sender, e);
                    break;
                case Keys.Escape:
                    darkButton1_Click_1(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
}
