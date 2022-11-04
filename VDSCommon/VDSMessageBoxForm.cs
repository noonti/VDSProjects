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
    public partial class VDSMessageBoxForm : DarkForm
    {
        String Title { get; set; }
        String Message { get; set; }

        int Mode = 1; 

        public VDSMessageBoxForm()
        {
            InitializeComponent();
        }

        public void SetMessage(String title, String message, int mode)
        {
            Title = title;
            Message = message;
            Mode = mode;
            this.Text = Title;
            lbMessage.Text = Message;
            switch(Mode)
            {
                case 1:
                    btnYes.Visible = false;
                    btnNo.Visible = false;
                    btnOk.Visible = true;
                    break;
                case 2:
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    btnOk.Visible = false;
                    break;
            }


        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
