using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VDSController
{
    public partial class ViewLiveCamForm : Form
    {
        public MainForm frmMain;
        public ViewLiveCamForm()
        {
            InitializeComponent();
        }

        private void StartLiveCamera()
        {

            if (frmMain != null)
            {
                MessageBox.Show("카메라는 최대 2시간 후 자동 종료 됩니다. ", "안내");
                frmMain.StartLiveCamera();
                wbLiveCam.Refresh();
            }
        }

        private void StopLiveCamera()
        {

            if (frmMain != null)
            {
                frmMain.StopLiveCamera();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartLiveCamera();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopLiveCamera();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StopLiveCamera();
            Close();
        }

        private void ViewLiveCamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("closing...live cam");
            if (frmMain != null)
                frmMain.viewLiveCamForm = null;
        }
    }
}
