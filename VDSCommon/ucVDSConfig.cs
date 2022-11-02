using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.Config;
using VDSCommon.API.Model;

namespace VDSCommon
{
    public partial class ucVDSConfig : UserControl
    {
        ControllerConfig controllerConfig;
        KictConfig kictConfig;
        KorExConfig korExConfig;

        public List<VDS_TYPE> vdsTypeList;
        //public List<VDS_GROUPS> vdsGroupList;
        public List<KorexOffice> korexOfficeList;

        //public Form parentForm = null;
        public ucVDSConfig()
        {
            InitializeComponent();
            //LoadVDSConfig();
        }




        public void LoadVDSConfig(ControllerConfig contConfig, KictConfig kiConfig, KorExConfig exConfig)
        {
            controllerConfig = contConfig;
            kictConfig = kiConfig;
            korExConfig = exConfig;


           
            //if (vdsGroupList != null)
            //    Utility.FillVDSGroupsComboBox(vdsGroupList, cbVDSGroup);
            if (korexOfficeList != null)
                Utility.FillKorofficeComboBox(korexOfficeList, cbVDSGroup);


            if (vdsTypeList != null)
                Utility.FillVDSTypeComboBox(vdsTypeList, cbVDSType);
            

            LoadControllerConfig();
            LoadKictConfig();
            LoadKorExConfig();

            //switch(controllerConfig.ProtocolType)
            //{
            //    case 1: // 건기연
            //        tabConfing.TabPages[2];
            //        break;
            //    case 2: // 도로공사
            //        break;
            //}
                
        }

        public void LoadControllerConfig()
        {
            if(controllerConfig!=null)
            {
                rdgKICT.Checked = controllerConfig.ProtocolType == 1 ? true : false;
                rdgKorEx.Checked = controllerConfig.ProtocolType == 2 ? true : false;


                txtIPAddress.Text = controllerConfig.IpAddress;
                txtVDSID.Text = controllerConfig.ControllerId;
                txtAPIPort.Text = controllerConfig.ApiPort.ToString();

                txtRemoteCtrlPort.Text = controllerConfig.RemoteCtrlPort.ToString();
                txtRemoteCtrlId.Text = controllerConfig.RemoteCtrlId;
                txtRemoteCtrlPasswd.Text = controllerConfig.RemoteCtrlPasswd;

                // serial port
                txtRTUPort.Text = controllerConfig.RTUPort;
                txtBaudRate.Text = controllerConfig.BaudRate.ToString();


                // Database 정보
                txtDBAddress.Text = controllerConfig.DBAddress;
                txtDBPort.Text = controllerConfig.DBPort.ToString();
                txtDBName.Text = controllerConfig.DBName;
                txtDBUserId.Text = controllerConfig.DBUser;
                txtDBPasswd.Text = controllerConfig.DBPasswd;



                // 검지장치
                rdgVideo.Checked = controllerConfig.DeviceType == 1 ? true : false;
                rdgRadar.Checked = controllerConfig.DeviceType == 2 ? true : false;



                txtVDSDeviceAddress.Text = controllerConfig.DeviceAddress;
                txtVDSDevicePort.Text = controllerConfig.RemotePort.ToString();
                txtLocalPort.Text = controllerConfig.LocalPort.ToString();
                txtCheckDistance.Text = controllerConfig.CheckDistance.ToString();
                txtStreamingURL.Text = controllerConfig.StreamingURL;

                txtMAServerAddress.Text = controllerConfig.MAServerAddress;
                txtMAServerPort.Text = controllerConfig.MAServerPort.ToString();
                txtMAApiPort.Text = controllerConfig.MAServerAPIPort.ToString();

                

                chkAnimation.Checked = controllerConfig.UseAnimation == 1 ? true : false;

                txtTrafficDataPeriod.Text = controllerConfig.TrafficDataPeriod.ToString();
                txtLogFilePeriod.Text = controllerConfig.LogFilePeriod.ToString();

            }
            
        }

        public void LoadKictConfig()
        {
            if(kictConfig!=null)
            {
                txtControlPort.Text = kictConfig.ctrlPort.ToString();
                txtCalibrationPort.Text = kictConfig.calibPort.ToString();
                txtCenterAddress.Text = kictConfig.centerAddress;
                txtCenterPort.Text = kictConfig.centerPort.ToString();
            }
            

    }

        public void LoadKorExConfig()
        {
            String vdsType = String.Empty;
            String groupCode = String.Empty;
            String vdsNo = String.Empty ;
            if(korExConfig!=null)
            {
                txtKorExCenterAddress.Text = korExConfig.centerAddress;
                txtKorExCenterPort.Text = korExConfig.centerPort.ToString();

                //Utility.GetCSN(korExConfig.csn, ref vdsType, ref groupCode, ref vdsNo);

                cbVDSType.SelectedIndex = vdsTypeList.FindIndex(x => x.VDS_TYPE_CODE == korExConfig.vdsType);
                cbVDSGroup.SelectedIndex = korexOfficeList.FindIndex(x => x.OfficeCode == korExConfig.vdsGroup);
                txtVDSNo.Text = korExConfig.vdsNo;
                txtSiteName.Text = korExConfig.siteName;
                //vdsNo = Utility.BCDToByte(vdsNo);
                //groupCode = (UInt16)((korExConfig.csn & 0xFFFF0000) >> 16);
                //serialNo = (UInt16)((korExConfig.csn & 0x0000FFFF));

                //txtCSN_1.Text = groupCode.ToString();
                //txtCSN_2.Text = serialNo.ToString();

                txtCenterPeriod.Text = korExConfig.centerPollingPeriod.ToString();
                txtLocalPeriod.Text = korExConfig.localPollingPeriod.ToString();
                txtSessionCheckTime.Text = korExConfig.checkSessionTime.ToString();

                txtPowerSupplyCnt.Text = korExConfig.powerSupplyCount.ToString();
                txtBoardCnt.Text = korExConfig.boardCount.ToString();

                txtVersion.Text = korExConfig.versionNo.ToString();
                txtRelease.Text = korExConfig.releaseNo.ToString();

                txtReleaseYear.Text = korExConfig.releaseYear.ToString();
                txtReleaseMonth.Text = korExConfig.releaseMonth.ToString();
                txtReleaesDay.Text = korExConfig.releaseDay.ToString();
            }
            

        }


        public int SetVDSConfig()
        {
            int nResult = 0;

            nResult = SetControllerConfig();
            if(nResult>0)
            {
                nResult = SetKictConfig();
                if(nResult > 0)
                {
                    nResult = SetKorExConfig();
                }
            }
            return nResult;
        }

        public int SetControllerConfig()
        {
            int nResult = 0;
            try
            {
                controllerConfig.ProtocolType = rdgKICT.Checked ? 1 : 2;

                controllerConfig.IpAddress = txtIPAddress.Text;
                controllerConfig.ControllerId = txtVDSID.Text;
                controllerConfig.ApiPort = int.Parse(txtAPIPort.Text);


                controllerConfig.RemoteCtrlPort = int.Parse(txtRemoteCtrlPort.Text);
                controllerConfig.RemoteCtrlId = txtRemoteCtrlId.Text;
                controllerConfig.RemoteCtrlPasswd = txtRemoteCtrlPasswd.Text;



                // serial port
                controllerConfig.RTUPort = txtRTUPort.Text;
                controllerConfig.BaudRate = int.Parse(txtBaudRate.Text);



                // Database 정보
                controllerConfig.DBAddress = txtDBAddress.Text;
                controllerConfig.DBPort = int.Parse(txtDBPort.Text);
                controllerConfig.DBName = txtDBName.Text;
                controllerConfig.DBUser = txtDBUserId.Text;
                controllerConfig.DBPasswd = txtDBPasswd.Text;



                // 검지장치
                controllerConfig.DeviceType = rdgVideo.Checked ? 1 : 2;


                controllerConfig.DeviceAddress = txtVDSDeviceAddress.Text;
                controllerConfig.RemotePort = int.Parse(txtVDSDevicePort.Text);
                controllerConfig.LocalPort = int.Parse(txtLocalPort.Text);
                controllerConfig.CheckDistance = double.Parse(txtCheckDistance.Text);
                controllerConfig.StreamingURL = txtStreamingURL.Text;


                controllerConfig.MAServerAddress = txtMAServerAddress.Text;
                controllerConfig.MAServerPort = int.Parse(txtMAServerPort.Text);
                controllerConfig.MAServerAPIPort = int.Parse(txtMAApiPort.Text);



                controllerConfig.UseAnimation = chkAnimation.Checked ? 1 : 0;

                controllerConfig.TrafficDataPeriod = int.Parse(txtTrafficDataPeriod.Text);
                controllerConfig.LogFilePeriod = int.Parse(txtLogFilePeriod.Text);

                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
            }



            return nResult;
        }

        public int SetKictConfig()
        {
            int nResult = 0;
            try
            {
                kictConfig.ctrlPort = int.Parse(txtControlPort.Text);
                kictConfig.calibPort = int.Parse(txtCalibrationPort.Text);
                kictConfig.centerAddress = txtCenterAddress.Text;
                kictConfig.centerPort = int.Parse(txtCenterPort.Text);
                nResult = 1;
            }
            catch(Exception ex)
            {
                nResult = 0;
            }
            


            return nResult;
        }

        public int SetKorExConfig()
        {
            int nResult = 0;
            try
            {
                korExConfig.centerAddress = txtKorExCenterAddress.Text;
                korExConfig.centerPort = int.Parse(txtKorExCenterPort.Text);

                if(cbVDSType.SelectedIndex>=0)
                    korExConfig.vdsType = vdsTypeList[cbVDSType.SelectedIndex].VDS_TYPE_CODE;
                if(cbVDSGroup.SelectedIndex>=0)
                    korExConfig.vdsGroup = korexOfficeList[cbVDSGroup.SelectedIndex].OfficeCode;
                korExConfig.vdsNo = txtVDSNo.Text;
                korExConfig.siteName = txtSiteName.Text;


                korExConfig.csn = Utility.GetCSN(korExConfig.vdsType, korExConfig.vdsGroup, korExConfig.vdsNo);

                korExConfig.centerPollingPeriod = int.Parse(txtCenterPeriod.Text);
                korExConfig.localPollingPeriod = int.Parse(txtLocalPeriod.Text);
                korExConfig.checkSessionTime = int.Parse(txtSessionCheckTime.Text);

                korExConfig.powerSupplyCount = int.Parse(txtPowerSupplyCnt.Text);
                korExConfig.boardCount = int.Parse(txtBoardCnt.Text);

                korExConfig.versionNo = int.Parse(txtVersion.Text);
                korExConfig.releaseNo = int.Parse(txtRelease.Text);

                korExConfig.releaseYear = int.Parse(txtReleaseYear.Text);
                korExConfig.releaseMonth = int.Parse(txtReleaseMonth.Text);
                korExConfig.releaseDay = int.Parse(txtReleaesDay.Text);


                nResult = 1;
            }
            catch (Exception ex)
            {
                nResult = 0;
            }
            return nResult;
        }

        private void txtRemotePasswd_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtIPAddress_Enter(object sender, EventArgs e)
        {
            //Utility.ShowVirtualKeyborad(sender as Control, ParentForm);

        }

        private void txtIPAddress_Leave(object sender, EventArgs e)
        {
            //Utility.HideVirtualKeyboard();
        }

        private void tabConfing_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"KeyDown Event 발생: keycode = {e.KeyCode}"));
            if(e.Control || e.Shift)
            {
                Utility.AddLog(LOG_TYPE.LOG_INFO, String.Format($"Cotrol 또는 Shift key 눌림"));
            }
        }

        public void SetVDSTypeList(List<VDS_TYPE> vdsList)
        {
            vdsTypeList = vdsList;
        }

        

        public void SetKorexOfficeList(List<KorexOffice> officeList)
        {
            korexOfficeList = officeList;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                    }

        private void cbVDSGroup_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
