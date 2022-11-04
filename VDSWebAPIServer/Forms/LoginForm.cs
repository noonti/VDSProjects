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
    public partial class LoginForm : DarkForm
    {
        public String userId;
        public String passwd;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void darkButton1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUSER_ID.Text))
            {
                //MessageBox.Show("아이디를 입력하세요", "입력", MessageBoxButtons.OK);
                Utility.ShowMessageBox("아이디 입력", "아이디를 입력하세요", 1);
                return;
            }



            if (String.IsNullOrEmpty(txtPASSWD.Text))
            {
                //MessageBox.Show("", "입력", MessageBoxButtons.OK);
                Utility.ShowMessageBox("비밀번호 입력", "비밀번호를 입력하세요", 1);
                return;
            }

            userId = txtUSER_ID.Text;
            passwd = txtPASSWD.Text;
            DialogResult = DialogResult.OK;
        }

        private void darkButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtUSER_ID_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    darkButton1_Click(sender, e);
                    break;
                case Keys.Escape:
                    darkButton2_Click(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
}
