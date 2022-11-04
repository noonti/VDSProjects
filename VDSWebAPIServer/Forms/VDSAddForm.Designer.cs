namespace VDSWebAPIServer.Forms
{
    partial class VDSAddForm
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
            this.txtControllerId = new DarkUI.Controls.DarkTextBox();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdgUseN = new DarkUI.Controls.DarkRadioButton();
            this.rdgUseY = new DarkUI.Controls.DarkRadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdgVDSType3 = new DarkUI.Controls.DarkRadioButton();
            this.rdgVDSType2 = new DarkUI.Controls.DarkRadioButton();
            this.rdgVDSType1 = new DarkUI.Controls.DarkRadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdgProtocol2 = new DarkUI.Controls.DarkRadioButton();
            this.rdgProtocol1 = new DarkUI.Controls.DarkRadioButton();
            this.txtIPAddress = new DarkUI.Controls.DarkTextBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.txtControllerName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.cbVDSGroups = new DarkUI.Controls.DarkComboBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.txtControllerId);
            this.darkGroupBox1.Controls.Add(this.darkLabel4);
            this.darkGroupBox1.Controls.Add(this.groupBox3);
            this.darkGroupBox1.Controls.Add(this.groupBox2);
            this.darkGroupBox1.Controls.Add(this.groupBox1);
            this.darkGroupBox1.Controls.Add(this.txtIPAddress);
            this.darkGroupBox1.Controls.Add(this.darkLabel3);
            this.darkGroupBox1.Controls.Add(this.txtControllerName);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Controls.Add(this.cbVDSGroups);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Location = new System.Drawing.Point(8, 8);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(909, 133);
            this.darkGroupBox1.TabIndex = 2;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "제어기 기본정보";
            // 
            // txtControllerId
            // 
            this.txtControllerId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtControllerId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtControllerId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtControllerId.Location = new System.Drawing.Point(548, 28);
            this.txtControllerId.Name = "txtControllerId";
            this.txtControllerId.Size = new System.Drawing.Size(120, 21);
            this.txtControllerId.TabIndex = 2;
            this.txtControllerId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(526, 32);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(16, 12);
            this.darkLabel4.TabIndex = 12;
            this.darkLabel4.Text = "ID";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdgUseN);
            this.groupBox3.Controls.Add(this.rdgUseY);
            this.groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox3.Location = new System.Drawing.Point(667, 65);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(225, 53);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "사용여부";
            // 
            // rdgUseN
            // 
            this.rdgUseN.AutoSize = true;
            this.rdgUseN.Location = new System.Drawing.Point(131, 22);
            this.rdgUseN.Name = "rdgUseN";
            this.rdgUseN.Size = new System.Drawing.Size(71, 16);
            this.rdgUseN.TabIndex = 1;
            this.rdgUseN.Text = "사용안함";
            // 
            // rdgUseY
            // 
            this.rdgUseY.AutoSize = true;
            this.rdgUseY.Checked = true;
            this.rdgUseY.Location = new System.Drawing.Point(37, 22);
            this.rdgUseY.Name = "rdgUseY";
            this.rdgUseY.Size = new System.Drawing.Size(47, 16);
            this.rdgUseY.TabIndex = 0;
            this.rdgUseY.TabStop = true;
            this.rdgUseY.Text = "사용";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdgVDSType3);
            this.groupBox2.Controls.Add(this.rdgVDSType2);
            this.groupBox2.Controls.Add(this.rdgVDSType1);
            this.groupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox2.Location = new System.Drawing.Point(306, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 53);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "제어기 유형";
            // 
            // rdgVDSType3
            // 
            this.rdgVDSType3.AutoSize = true;
            this.rdgVDSType3.Location = new System.Drawing.Point(242, 22);
            this.rdgVDSType3.Name = "rdgVDSType3";
            this.rdgVDSType3.Size = new System.Drawing.Size(83, 16);
            this.rdgVDSType3.TabIndex = 2;
            this.rdgVDSType3.Text = "하이브리드";
            this.rdgVDSType3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // rdgVDSType2
            // 
            this.rdgVDSType2.AutoSize = true;
            this.rdgVDSType2.Checked = true;
            this.rdgVDSType2.Location = new System.Drawing.Point(128, 22);
            this.rdgVDSType2.Name = "rdgVDSType2";
            this.rdgVDSType2.Size = new System.Drawing.Size(71, 16);
            this.rdgVDSType2.TabIndex = 1;
            this.rdgVDSType2.TabStop = true;
            this.rdgVDSType2.Text = "레이더식";
            // 
            // rdgVDSType1
            // 
            this.rdgVDSType1.AutoSize = true;
            this.rdgVDSType1.Location = new System.Drawing.Point(19, 22);
            this.rdgVDSType1.Name = "rdgVDSType1";
            this.rdgVDSType1.Size = new System.Drawing.Size(59, 16);
            this.rdgVDSType1.TabIndex = 0;
            this.rdgVDSType1.Text = "영상식";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdgProtocol2);
            this.groupBox1.Controls.Add(this.rdgProtocol1);
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(73, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 53);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "프로토콜";
            // 
            // rdgProtocol2
            // 
            this.rdgProtocol2.AutoSize = true;
            this.rdgProtocol2.Checked = true;
            this.rdgProtocol2.Location = new System.Drawing.Point(137, 22);
            this.rdgProtocol2.Name = "rdgProtocol2";
            this.rdgProtocol2.Size = new System.Drawing.Size(71, 16);
            this.rdgProtocol2.TabIndex = 1;
            this.rdgProtocol2.TabStop = true;
            this.rdgProtocol2.Text = "도로공사";
            this.rdgProtocol2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // rdgProtocol1
            // 
            this.rdgProtocol1.AutoSize = true;
            this.rdgProtocol1.Location = new System.Drawing.Point(7, 22);
            this.rdgProtocol1.Name = "rdgProtocol1";
            this.rdgProtocol1.Size = new System.Drawing.Size(82, 16);
            this.rdgProtocol1.TabIndex = 0;
            this.rdgProtocol1.Text = "건기연 ITS";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtIPAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtIPAddress.Location = new System.Drawing.Point(752, 28);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(140, 21);
            this.txtIPAddress.TabIndex = 3;
            this.txtIPAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(675, 32);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(67, 12);
            this.darkLabel3.TabIndex = 4;
            this.darkLabel3.Text = "IP Address";
            // 
            // txtControllerName
            // 
            this.txtControllerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtControllerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtControllerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtControllerName.Location = new System.Drawing.Point(306, 28);
            this.txtControllerName.Name = "txtControllerName";
            this.txtControllerName.Size = new System.Drawing.Size(212, 21);
            this.txtControllerName.TabIndex = 1;
            this.txtControllerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(249, 32);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(53, 12);
            this.darkLabel2.TabIndex = 2;
            this.darkLabel2.Text = "제어기명";
            // 
            // cbVDSGroups
            // 
            this.cbVDSGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbVDSGroups.FormattingEnabled = true;
            this.cbVDSGroups.Location = new System.Drawing.Point(73, 27);
            this.cbVDSGroups.Name = "cbVDSGroups";
            this.cbVDSGroups.Size = new System.Drawing.Size(165, 22);
            this.cbVDSGroups.TabIndex = 0;
            this.cbVDSGroups.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rdgProtocol2_KeyDown);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(11, 32);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(57, 12);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "소속 그룹";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(363, 147);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 31);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "확인";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(491, 147);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 31);
            this.darkButton1.TabIndex = 14;
            this.darkButton1.Text = "취소";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click_1);
            // 
            // VDSAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 187);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.darkGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "VDSAddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "제어기 상세 정보";
            this.Activated += new System.EventHandler(this.VDSAddForm_Activated);
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkTextBox txtControllerId;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private System.Windows.Forms.GroupBox groupBox3;
        private DarkUI.Controls.DarkRadioButton rdgUseN;
        private DarkUI.Controls.DarkRadioButton rdgUseY;
        private System.Windows.Forms.GroupBox groupBox2;
        private DarkUI.Controls.DarkRadioButton rdgVDSType3;
        private DarkUI.Controls.DarkRadioButton rdgVDSType2;
        private DarkUI.Controls.DarkRadioButton rdgVDSType1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DarkUI.Controls.DarkRadioButton rdgProtocol2;
        private DarkUI.Controls.DarkRadioButton rdgProtocol1;
        private DarkUI.Controls.DarkTextBox txtIPAddress;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox txtControllerName;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkComboBox cbVDSGroups;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton btnSave;
        private DarkUI.Controls.DarkButton darkButton1;
    }
}