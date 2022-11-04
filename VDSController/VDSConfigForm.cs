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
using VDSController.Global;

namespace VDSController
{
    public partial class VDSConfigForm : DarkForm
    {
        public VDSConfigForm()
        {
            InitializeComponent();
            //ucConfig.SetVDSGrupList(GlobalCommonData.vdsGroupsList);
            ucConfig.SetVDSTypeList(GlobalCommonData.vdsTypeList);
            ucConfig.SetKorexOfficeList(GlobalCommonData.korexOfficeList);
            ucConfig.LoadVDSConfig(VDSConfig.controllerConfig, VDSConfig.kictConfig, VDSConfig.korExConfig);
 
        }

        //private void LoadVDSConfig()
        //{


        //    rdgKICT.Checked = VDSConfig.CENTER_TYPE == 1 ? true : false;
        //    rdgKorEx.Checked = VDSConfig.CENTER_TYPE == 2 ? true : false;


        //    txtIPAddress.Text = VDSConfig.IPADDRESS;
        //    txtControlPort.Text = VDSConfig.CTRL_PORT.ToString();
        //    txtCalibrationPort.Text = VDSConfig.CALIB_PORT.ToString();
        //    txtVDSID.Text = VDSConfig.VDS_ID;
        //    txtAPIPort.Text = VDSConfig.API_PORT.ToString();

        //    // Database 정보
        //    txtDBAddress.Text = VDSConfig.VDS_DB_ADDRESS;
        //    txtDBPort.Text = VDSConfig.VDS_DB_PORT.ToString();
        //    txtDBName.Text = VDSConfig.VDS_DB_NAME;
        //    txtDBUserId.Text = VDSConfig.VDS_DB_USER;
        //    txtDBPasswd.Text = VDSConfig.VDS_DB_PASSWD;


        //    // 센터
        //    txtCenterAddress.Text = VDSConfig.CENTER_ADDRESS;
        //    txtCenterPort.Text = VDSConfig.CENTER_PORT.ToString();

        //    // 검지장치
        //    rdgVideo.Checked = VDSConfig.VDS_TYPE == 1 ? true : false;
        //    rdgRadar.Checked = VDSConfig.VDS_TYPE == 2 ? true : false;

        //    txtSensorCount.Text = VDSConfig.SENSOR_COUNT.ToString();

        //    txtVDSDeviceAddress.Text = VDSConfig.VDS_DEVICE_ADDRESS;
        //    txtVDSDevicePort.Text = VDSConfig.VDS_DEVICE_PORT.ToString();
        //    txtLocalPort.Text = VDSConfig.VDS_LOCAL_PORT.ToString();
        //    txtCheckDistance.Text = VDSConfig.CHECK_DISTANCE.ToString();
        //    txtStreamingURL.Text = VDSConfig.RTSP_STREAMING_URL;

        //    chkAnimation.Checked = VDSConfig.VDS_USE_ANIMATION ==1?true:false;

        //}


        //private bool SetVDSConfig()
        //{
        //    bool result = false;
        //    try
        //    {

        //        VDSConfig.CENTER_TYPE = rdgKICT.Checked ? 1 : 2;

        //        //VDS 제어 프로그램 설정
        //        VDSConfig.IPADDRESS = txtIPAddress.Text;
        //        VDSConfig.CTRL_PORT = int.Parse(txtControlPort.Text);
        //        VDSConfig.CALIB_PORT = int.Parse(txtCalibrationPort.Text);
        //        VDSConfig.VDS_ID = txtVDSID.Text;
        //        VDSConfig.API_PORT = int.Parse(txtAPIPort.Text);


        //        VDSConfig.RTU_PORT = txtRTUPort.Text;
        //        VDSConfig.BAUD_RATE = int.Parse(txtBaudRate.Text);



        //        // Database 정보
        //        VDSConfig.VDS_DB_ADDRESS = txtDBAddress.Text;
        //        VDSConfig.VDS_DB_PORT = int.Parse(txtDBPort.Text);
        //        VDSConfig.VDS_DB_NAME = txtDBName.Text;
        //        VDSConfig.VDS_DB_USER = txtDBUserId.Text;
        //        VDSConfig.VDS_DB_PASSWD = txtDBPasswd.Text;


        //        // 안산센터
        //        VDSConfig.CENTER_ADDRESS = txtCenterAddress.Text;
        //        VDSConfig.CENTER_PORT = int.Parse(txtCenterPort.Text);

        //        // 검지장치
        //        VDSConfig.VDS_TYPE = rdgVideo.Checked ? 1 : 2;

        //        VDSConfig.SENSOR_COUNT = int.Parse(txtSensorCount.Text);


        //        VDSConfig.VDS_DEVICE_ADDRESS = txtVDSDeviceAddress.Text;
        //        VDSConfig.VDS_DEVICE_PORT = int.Parse(txtVDSDevicePort.Text);
        //        VDSConfig.VDS_LOCAL_PORT = int.Parse(txtLocalPort.Text);
        //        VDSConfig.CHECK_DISTANCE = double.Parse(txtCheckDistance.Text);
        //        VDSConfig.RTSP_STREAMING_URL = txtStreamingURL.Text;

        //        VDSConfig.VDS_USE_ANIMATION = chkAnimation.Checked ? 1 : 0;

        //        result = true;
        //    }
        //    catch(Exception ex)
        //    {
                
        //    }

        //    return result;
        //}


        private void button1_Click(object sender, EventArgs e)
        {

            if(ucConfig.SetVDSConfig()==0)
            {
                Utility.ShowMessageBox("입력 오류", "입력값을 확인하세요", 1);
                return;
            }
            DialogResult = DialogResult.OK; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtIPAddress_Enter(object sender, EventArgs e)
        {
            //Utility.ShowVirtualKeyborad(sender as Control, this);
        }

        private void txtIPAddress_Leave(object sender, EventArgs e)
        {
            //Utility.HideVirtualKeyboard();
        }

        private void ucConfig_Load(object sender, EventArgs e)
        {

        }

        private void VDSConfigForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }
    }
}
