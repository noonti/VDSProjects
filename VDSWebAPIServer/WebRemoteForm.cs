using CefSharp;
using CefSharp.WinForms;
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

namespace VDSWebAPIServer
{
    public partial class WebRemoteForm : DarkForm
    {
        public VDS_CONTROLLER selectedVDSCtrl;
        public List<WebRemoteForm> remoteFormList = null;

        public WebRemoteForm()
        {
            InitializeComponent();
            ApiUtility.FillVDSGroupsComboBox(GlobalCommonData.vdsGroupsList, cbVDSGroups);
        }

        public void LoadRemoteCtrl()
        {
            String url = String.Empty;

            if(selectedVDSCtrl!=null && !String.IsNullOrEmpty(selectedVDSCtrl.VDS_CONFIG))
            {
                var data = JsonConvert.DeserializeObject<MAVDSConfigRequest>(selectedVDSCtrl.VDS_CONFIG);
                if (data != null)
                {
                    url = String.Format($"http://{data.controllerConfig.RemoteCtrlId}:{data.controllerConfig.RemoteCtrlPasswd}@{data.controllerConfig.IpAddress}:{data.controllerConfig.RemoteCtrlPort}");
                }

                SetVDSContollerInfo(selectedVDSCtrl);
            }
            chBrowser.Load(url);

        }

        public void SetVDSContollerInfo(VDS_CONTROLLER vdsController)
        {

            cbVDSGroups.SelectedIndex = GlobalCommonData.vdsGroupsList.FindIndex(x => x.ID == vdsController.GROUP_ID);
            txtControllerName.Text = vdsController.CONTROLLER_NAME;
            txtControllerId.Text = vdsController.CONTROLLER_ID;
            txtIPAddress.Text = vdsController.IP_ADDRESS;
            SetEnable(false);
        }

        public void SetEnable(bool enabled)
        {
            cbVDSGroups.Enabled = enabled;
            txtControllerName.Enabled = enabled;
            txtControllerId.Enabled = enabled;
            txtIPAddress.Enabled = enabled;
        }


        private void darkGroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void webRemteCtrl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void WebRemoteForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("View Form closed");
            if (remoteFormList != null)
            {
                remoteFormList.Remove(this);
            }
        }
    }
}
