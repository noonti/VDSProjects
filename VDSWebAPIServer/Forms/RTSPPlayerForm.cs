using DarkUI.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDSCommon.API.Model;
using VDSCommon.Protocol.admin;
using VDSWebAPIServer.Common;

namespace VDSWebAPIServer.Forms
{
    public partial class RTSPPlayerForm : DarkForm
    {
        public VDS_CONTROLLER selectedVDSCtrl;

        public List<RTSPPlayerForm> playerFormList = null;


        public RTSPPlayerForm()
        {
            InitializeComponent();
            
        }

        public void LoadPlayer()
        {
            ApiUtility.FillVDSGroupsComboBox(GlobalCommonData.vdsGroupsList, cbVDSGroups);
            SetVDSContollerInfo(selectedVDSCtrl);

        }

        private void darkButton3_Click(object sender, EventArgs e)
        {
            StartPlay();
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            StopPlay();
        }

        public void StartPlay()
        {
            String url = String.Empty;
            if(selectedVDSCtrl!=null && !String.IsNullOrEmpty(selectedVDSCtrl.VDS_CONFIG))
            {
                var data = JsonConvert.DeserializeObject<MAVDSConfigRequest>(selectedVDSCtrl.VDS_CONFIG);
                if (data != null)
                {
                    url = data.controllerConfig.StreamingURL;
                }
               
            }
            if (!String.IsNullOrEmpty(url))
            {
                rtspStreamingPlayer.StartStreaming(true, url);
            }
            else
            {
                MessageBox.Show("영상보기에 실패하였습니다", "오류");
            }

        }

        public void StopPlay()
        {
            rtspStreamingPlayer.StopStreaming();

        }

        private void RTSPPlayerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("View Form closed");
            if (playerFormList != null)
            {
                rtspStreamingPlayer.StopStreaming();
                playerFormList.Remove(this);
            }
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void SetVDSContollerInfo(VDS_CONTROLLER vdsController)
        {
            if(vdsController!=null)
            {
                cbVDSGroups.SelectedIndex = GlobalCommonData.vdsGroupsList.FindIndex(x => x.ID == vdsController.GROUP_ID);
                Console.WriteLine($"selected index={cbVDSGroups.SelectedIndex}");
                txtControllerName.Text = vdsController.CONTROLLER_NAME;
                txtControllerId.Text = vdsController.CONTROLLER_ID;
                txtIPAddress.Text = vdsController.IP_ADDRESS;
                SetEnable(false);
            }
            
        }

        public void SetEnable(bool enabled)
        {
            //cbVDSGroups.Enabled = enabled;
            txtControllerName.Enabled = enabled;
            txtControllerId.Enabled = enabled;
            txtIPAddress.Enabled = enabled;
        }
    }
}
