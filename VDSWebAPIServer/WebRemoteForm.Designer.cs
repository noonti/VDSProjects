namespace VDSWebAPIServer
{
    partial class WebRemoteForm
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
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.chBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.txtControllerId = new DarkUI.Controls.DarkTextBox();
            this.txtIPAddress = new DarkUI.Controls.DarkTextBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.txtControllerName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.cbVDSGroups = new DarkUI.Controls.DarkComboBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.darkGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkLabel4);
            this.darkGroupBox1.Controls.Add(this.txtControllerId);
            this.darkGroupBox1.Controls.Add(this.txtIPAddress);
            this.darkGroupBox1.Controls.Add(this.darkLabel3);
            this.darkGroupBox1.Controls.Add(this.txtControllerName);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Controls.Add(this.cbVDSGroups);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(1884, 53);
            this.darkGroupBox1.TabIndex = 0;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "제어기 정보";
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkGroupBox2.Location = new System.Drawing.Point(0, 920);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(1884, 41);
            this.darkGroupBox2.TabIndex = 1;
            this.darkGroupBox2.TabStop = false;
            // 
            // chBrowser
            // 
            this.chBrowser.ActivateBrowserOnCreation = false;
            this.chBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chBrowser.Location = new System.Drawing.Point(0, 53);
            this.chBrowser.Name = "chBrowser";
            this.chBrowser.Size = new System.Drawing.Size(1884, 867);
            this.chBrowser.TabIndex = 2;
            // 
            // txtControllerId
            // 
            this.txtControllerId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtControllerId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtControllerId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtControllerId.Location = new System.Drawing.Point(682, 20);
            this.txtControllerId.Name = "txtControllerId";
            this.txtControllerId.Size = new System.Drawing.Size(202, 21);
            this.txtControllerId.TabIndex = 14;
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtIPAddress.Location = new System.Drawing.Point(962, 20);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(151, 21);
            this.txtIPAddress.TabIndex = 16;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(885, 24);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(67, 12);
            this.darkLabel3.TabIndex = 17;
            this.darkLabel3.Text = "IP Address";
            // 
            // txtControllerName
            // 
            this.txtControllerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtControllerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtControllerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtControllerName.Location = new System.Drawing.Point(323, 21);
            this.txtControllerName.Name = "txtControllerName";
            this.txtControllerName.Size = new System.Drawing.Size(331, 21);
            this.txtControllerName.TabIndex = 13;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(266, 25);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(53, 12);
            this.darkLabel2.TabIndex = 15;
            this.darkLabel2.Text = "제어기명";
            // 
            // cbVDSGroups
            // 
            this.cbVDSGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbVDSGroups.FormattingEnabled = true;
            this.cbVDSGroups.Location = new System.Drawing.Point(92, 20);
            this.cbVDSGroups.Name = "cbVDSGroups";
            this.cbVDSGroups.Size = new System.Drawing.Size(165, 22);
            this.cbVDSGroups.TabIndex = 11;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(30, 25);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(57, 12);
            this.darkLabel1.TabIndex = 12;
            this.darkLabel1.Text = "소속 그룹";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(660, 25);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(16, 12);
            this.darkLabel4.TabIndex = 21;
            this.darkLabel4.Text = "ID";
            // 
            // WebRemoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1884, 961);
            this.Controls.Add(this.chBrowser);
            this.Controls.Add(this.darkGroupBox2);
            this.Controls.Add(this.darkGroupBox1);
            this.Name = "WebRemoteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "원격 설정";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WebRemoteForm_FormClosed);
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private CefSharp.WinForms.ChromiumWebBrowser chBrowser;
        private DarkUI.Controls.DarkTextBox txtControllerId;
        private DarkUI.Controls.DarkTextBox txtIPAddress;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox txtControllerName;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkComboBox cbVDSGroups;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel4;
    }
}