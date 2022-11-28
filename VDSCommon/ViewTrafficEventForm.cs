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

namespace VDSCommon
{
    public partial class ViewTrafficEventForm : DarkForm
    {
        public String imgFileName = String.Empty;
        public ViewTrafficEventForm()
        {
            InitializeComponent();
        }

        public ViewTrafficEventForm(String fileName)
        {
            InitializeComponent();
            imgFileName = fileName;
        }

        private void darkButton10_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ViewTrafficEventForm_Load(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(imgFileName) && System.IO.File.Exists(imgFileName) )
            {
                pbTrafficEvent.Image = Image.FromFile(imgFileName);
            }
            else
            {
                Utility.ShowMessageBox("파일없음", "저장된 파일이 없습니다", 1);
            }
        }
    }
}
