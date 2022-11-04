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

namespace VDSWebAPIServer.Forms
{
    public partial class AdminConfigForm : DarkForm
    {
        public AdminConfigForm()
        {
            InitializeComponent();
            LoadAdminConfig();
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            if (SetAdminConfig() == 0)
            {
                MessageBox.Show("입력값을 확인하세요");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void LoadAdminConfig()
        {
            txtIPAddress.Text = AdminConfig.ADMIN_ADDRESS;
            txtPort.Text = AdminConfig.ADMIN_PORT.ToString();
            txtAPIPort.Text = AdminConfig.ADMIN_API_PORT.ToString();

            // Database 정보
            txtDBAddress.Text = AdminConfig.DB_ADDRESS;
            txtDBPort.Text = AdminConfig.DB_PORT.ToString();
            txtDBName.Text = AdminConfig.DB_NAME;
            txtDBUserId.Text = AdminConfig.DB_USER;
            txtDBPasswd.Text = AdminConfig.DB_PASSWD;

        }


        private int SetAdminConfig()
        {
            int nResult = 0;
            try
            {
                AdminConfig.ADMIN_ADDRESS = txtIPAddress.Text;
                AdminConfig.ADMIN_PORT  = int.Parse(txtPort.Text);
                AdminConfig.ADMIN_API_PORT = int.Parse(txtAPIPort.Text);

                // Database 정보
                AdminConfig.DB_ADDRESS = txtDBAddress.Text;
                AdminConfig.DB_PORT = int.Parse(txtDBPort.Text);
                AdminConfig.DB_NAME = txtDBName.Text;
                AdminConfig.DB_USER = txtDBUserId.Text;
                AdminConfig.DB_PASSWD = txtDBPasswd.Text;
                nResult = 1;

            }
            catch(Exception ex)
            {
                nResult = 0;
                Console.WriteLine(ex.StackTrace.ToString());
            }
            return nResult;
        }
    }
}
