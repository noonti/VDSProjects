namespace VDSWebAPIServer.Forms
{
    partial class AdminConfigForm
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
            this.darkGroupBox5 = new DarkUI.Controls.DarkGroupBox();
            this.txtDBPasswd = new DarkUI.Controls.DarkTextBox();
            this.darkLabel21 = new DarkUI.Controls.DarkLabel();
            this.txtDBUserId = new DarkUI.Controls.DarkTextBox();
            this.darkLabel20 = new DarkUI.Controls.DarkLabel();
            this.txtDBName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel19 = new DarkUI.Controls.DarkLabel();
            this.txtDBPort = new DarkUI.Controls.DarkTextBox();
            this.darkLabel17 = new DarkUI.Controls.DarkLabel();
            this.txtDBAddress = new DarkUI.Controls.DarkTextBox();
            this.darkLabel18 = new DarkUI.Controls.DarkLabel();
            this.darkGroupBox8 = new DarkUI.Controls.DarkGroupBox();
            this.txtPort = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtAPIPort = new DarkUI.Controls.DarkTextBox();
            this.darkLabel14 = new DarkUI.Controls.DarkLabel();
            this.txtIPAddress = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox5.SuspendLayout();
            this.darkGroupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox5
            // 
            this.darkGroupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox5.Controls.Add(this.txtDBPasswd);
            this.darkGroupBox5.Controls.Add(this.darkLabel21);
            this.darkGroupBox5.Controls.Add(this.txtDBUserId);
            this.darkGroupBox5.Controls.Add(this.darkLabel20);
            this.darkGroupBox5.Controls.Add(this.txtDBName);
            this.darkGroupBox5.Controls.Add(this.darkLabel19);
            this.darkGroupBox5.Controls.Add(this.txtDBPort);
            this.darkGroupBox5.Controls.Add(this.darkLabel17);
            this.darkGroupBox5.Controls.Add(this.txtDBAddress);
            this.darkGroupBox5.Controls.Add(this.darkLabel18);
            this.darkGroupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkGroupBox5.Location = new System.Drawing.Point(12, 85);
            this.darkGroupBox5.Name = "darkGroupBox5";
            this.darkGroupBox5.Size = new System.Drawing.Size(536, 87);
            this.darkGroupBox5.TabIndex = 12;
            this.darkGroupBox5.TabStop = false;
            this.darkGroupBox5.Text = "Database 정보";
            // 
            // txtDBPasswd
            // 
            this.txtDBPasswd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDBPasswd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBPasswd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDBPasswd.Location = new System.Drawing.Point(423, 49);
            this.txtDBPasswd.Name = "txtDBPasswd";
            this.txtDBPasswd.Size = new System.Drawing.Size(102, 21);
            this.txtDBPasswd.TabIndex = 4;
            this.txtDBPasswd.Text = "1234";
            this.txtDBPasswd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel21
            // 
            this.darkLabel21.AutoSize = true;
            this.darkLabel21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel21.Location = new System.Drawing.Point(357, 52);
            this.darkLabel21.Name = "darkLabel21";
            this.darkLabel21.Size = new System.Drawing.Size(61, 15);
            this.darkLabel21.TabIndex = 8;
            this.darkLabel21.Text = "Password";
            // 
            // txtDBUserId
            // 
            this.txtDBUserId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDBUserId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBUserId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDBUserId.Location = new System.Drawing.Point(251, 49);
            this.txtDBUserId.Name = "txtDBUserId";
            this.txtDBUserId.Size = new System.Drawing.Size(102, 21);
            this.txtDBUserId.TabIndex = 3;
            this.txtDBUserId.Text = "vds";
            this.txtDBUserId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel20
            // 
            this.darkLabel20.AutoSize = true;
            this.darkLabel20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel20.Location = new System.Drawing.Point(200, 52);
            this.darkLabel20.Name = "darkLabel20";
            this.darkLabel20.Size = new System.Drawing.Size(48, 15);
            this.darkLabel20.TabIndex = 6;
            this.darkLabel20.Text = "User ID";
            // 
            // txtDBName
            // 
            this.txtDBName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDBName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDBName.Location = new System.Drawing.Point(80, 49);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(75, 21);
            this.txtDBName.TabIndex = 2;
            this.txtDBName.Text = "vdsdb";
            this.txtDBName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel19
            // 
            this.darkLabel19.AutoSize = true;
            this.darkLabel19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel19.Location = new System.Drawing.Point(38, 52);
            this.darkLabel19.Name = "darkLabel19";
            this.darkLabel19.Size = new System.Drawing.Size(39, 15);
            this.darkLabel19.TabIndex = 4;
            this.darkLabel19.Text = "DB 명";
            // 
            // txtDBPort
            // 
            this.txtDBPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDBPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDBPort.Location = new System.Drawing.Point(340, 22);
            this.txtDBPort.Name = "txtDBPort";
            this.txtDBPort.Size = new System.Drawing.Size(185, 21);
            this.txtDBPort.TabIndex = 1;
            this.txtDBPort.Text = "3306";
            this.txtDBPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDBPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel17
            // 
            this.darkLabel17.AutoSize = true;
            this.darkLabel17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel17.Location = new System.Drawing.Point(278, 25);
            this.darkLabel17.Name = "darkLabel17";
            this.darkLabel17.Size = new System.Drawing.Size(58, 15);
            this.darkLabel17.TabIndex = 2;
            this.darkLabel17.Text = "접속 포트";
            // 
            // txtDBAddress
            // 
            this.txtDBAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDBAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDBAddress.Location = new System.Drawing.Point(80, 22);
            this.txtDBAddress.Name = "txtDBAddress";
            this.txtDBAddress.Size = new System.Drawing.Size(185, 21);
            this.txtDBAddress.TabIndex = 0;
            this.txtDBAddress.Text = "127.0.0.1";
            this.txtDBAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel18
            // 
            this.darkLabel18.AutoSize = true;
            this.darkLabel18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel18.Location = new System.Drawing.Point(32, 25);
            this.darkLabel18.Name = "darkLabel18";
            this.darkLabel18.Size = new System.Drawing.Size(45, 15);
            this.darkLabel18.TabIndex = 0;
            this.darkLabel18.Text = "IP 주소";
            // 
            // darkGroupBox8
            // 
            this.darkGroupBox8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox8.Controls.Add(this.txtPort);
            this.darkGroupBox8.Controls.Add(this.darkLabel2);
            this.darkGroupBox8.Controls.Add(this.txtAPIPort);
            this.darkGroupBox8.Controls.Add(this.darkLabel14);
            this.darkGroupBox8.Controls.Add(this.txtIPAddress);
            this.darkGroupBox8.Controls.Add(this.darkLabel1);
            this.darkGroupBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkGroupBox8.Location = new System.Drawing.Point(12, 12);
            this.darkGroupBox8.Name = "darkGroupBox8";
            this.darkGroupBox8.Size = new System.Drawing.Size(536, 67);
            this.darkGroupBox8.TabIndex = 11;
            this.darkGroupBox8.TabStop = false;
            this.darkGroupBox8.Text = "VDS 제어프로그램 설정";
            // 
            // txtPort
            // 
            this.txtPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtPort.Location = new System.Drawing.Point(250, 22);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(102, 21);
            this.txtPort.TabIndex = 9;
            this.txtPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(217, 25);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(29, 15);
            this.darkLabel2.TabIndex = 10;
            this.darkLabel2.Text = "Port";
            // 
            // txtAPIPort
            // 
            this.txtAPIPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtAPIPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAPIPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtAPIPort.Location = new System.Drawing.Point(423, 22);
            this.txtAPIPort.Name = "txtAPIPort";
            this.txtAPIPort.Size = new System.Drawing.Size(102, 21);
            this.txtAPIPort.TabIndex = 7;
            this.txtAPIPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel14
            // 
            this.darkLabel14.AutoSize = true;
            this.darkLabel14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel14.Location = new System.Drawing.Point(367, 25);
            this.darkLabel14.Name = "darkLabel14";
            this.darkLabel14.Size = new System.Drawing.Size(57, 15);
            this.darkLabel14.TabIndex = 8;
            this.darkLabel14.Text = "Web Port";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtIPAddress.Location = new System.Drawing.Point(80, 22);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(128, 21);
            this.txtIPAddress.TabIndex = 0;
            this.txtIPAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPAddress_KeyDown);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(32, 25);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(45, 15);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "IP 주소";
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(297, 181);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(75, 28);
            this.darkButton2.TabIndex = 14;
            this.darkButton2.Text = "취소";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(187, 181);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 28);
            this.darkButton1.TabIndex = 13;
            this.darkButton1.Text = "확인";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // AdminConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 221);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.darkGroupBox5);
            this.Controls.Add(this.darkGroupBox8);
            this.Name = "AdminConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "환경설정";
            this.darkGroupBox5.ResumeLayout(false);
            this.darkGroupBox5.PerformLayout();
            this.darkGroupBox8.ResumeLayout(false);
            this.darkGroupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox darkGroupBox5;
        private DarkUI.Controls.DarkTextBox txtDBPasswd;
        private DarkUI.Controls.DarkLabel darkLabel21;
        private DarkUI.Controls.DarkTextBox txtDBUserId;
        private DarkUI.Controls.DarkLabel darkLabel20;
        private DarkUI.Controls.DarkTextBox txtDBName;
        private DarkUI.Controls.DarkLabel darkLabel19;
        private DarkUI.Controls.DarkTextBox txtDBPort;
        private DarkUI.Controls.DarkLabel darkLabel17;
        private DarkUI.Controls.DarkTextBox txtDBAddress;
        private DarkUI.Controls.DarkLabel darkLabel18;
        private DarkUI.Controls.DarkGroupBox darkGroupBox8;
        private DarkUI.Controls.DarkTextBox txtAPIPort;
        private DarkUI.Controls.DarkLabel darkLabel14;
        private DarkUI.Controls.DarkTextBox txtIPAddress;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkTextBox txtPort;
        private DarkUI.Controls.DarkLabel darkLabel2;
    }
}