namespace VDSController
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.txtPASSWD = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtUSER_ID = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox1.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.txtPASSWD);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Controls.Add(this.txtUSER_ID);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(350, 96);
            this.darkGroupBox1.TabIndex = 4;
            this.darkGroupBox1.TabStop = false;
            // 
            // txtPASSWD
            // 
            this.txtPASSWD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtPASSWD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPASSWD.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtPASSWD.Location = new System.Drawing.Point(110, 55);
            this.txtPASSWD.Name = "txtPASSWD";
            this.txtPASSWD.PasswordChar = '*';
            this.txtPASSWD.Size = new System.Drawing.Size(177, 21);
            this.txtPASSWD.TabIndex = 1;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(46, 59);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(53, 12);
            this.darkLabel2.TabIndex = 2;
            this.darkLabel2.Text = "비밀번호";
            // 
            // txtUSER_ID
            // 
            this.txtUSER_ID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtUSER_ID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUSER_ID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtUSER_ID.Location = new System.Drawing.Point(110, 28);
            this.txtUSER_ID.Name = "txtUSER_ID";
            this.txtUSER_ID.Size = new System.Drawing.Size(177, 21);
            this.txtUSER_ID.TabIndex = 0;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(58, 32);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(41, 12);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "아이디";
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Controls.Add(this.darkButton2);
            this.darkGroupBox2.Controls.Add(this.darkButton1);
            this.darkGroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkGroupBox2.Location = new System.Drawing.Point(0, 96);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(350, 42);
            this.darkGroupBox2.TabIndex = 5;
            this.darkGroupBox2.TabStop = false;
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(187, 5);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(75, 31);
            this.darkButton2.TabIndex = 1;
            this.darkButton2.Text = "취소";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(91, 5);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 31);
            this.darkButton1.TabIndex = 0;
            this.darkButton1.Text = "확인";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 138);
            this.Controls.Add(this.darkGroupBox1);
            this.Controls.Add(this.darkGroupBox2);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "로그인";
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.darkGroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkTextBox txtPASSWD;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtUSER_ID;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton1;
    }
}