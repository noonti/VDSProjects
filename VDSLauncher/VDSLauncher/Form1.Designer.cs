namespace VDSSimulator
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitLane = new System.Windows.Forms.SplitContainer();
            this.lbxTarget = new System.Windows.Forms.ListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEventPortNo = new System.Windows.Forms.TextBox();
            this.txtEventIPAddress = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.chkSysTime = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbxSend = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbxReceive = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVDSPort = new System.Windows.Forms.TextBox();
            this.txtVDSAddress = new System.Windows.Forms.TextBox();
            this.btnAutoSync = new System.Windows.Forms.Button();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnTrafficData = new System.Windows.Forms.Button();
            this.txtTime = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).BeginInit();
            this.splitLane.Panel2.SuspendLayout();
            this.splitLane.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.splitLane);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtEventPortNo);
            this.groupBox1.Controls.Add(this.txtEventIPAddress);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(759, 275);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Traffic Event Server";
            // 
            // splitLane
            // 
            this.splitLane.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitLane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitLane.IsSplitterFixed = true;
            this.splitLane.Location = new System.Drawing.Point(3, 47);
            this.splitLane.Name = "splitLane";
            // 
            // splitLane.Panel2
            // 
            this.splitLane.Panel2.Controls.Add(this.lbxTarget);
            this.splitLane.Size = new System.Drawing.Size(753, 225);
            this.splitLane.SplitterDistance = 52;
            this.splitLane.SplitterWidth = 1;
            this.splitLane.TabIndex = 5;
            // 
            // lbxTarget
            // 
            this.lbxTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxTarget.FormattingEnabled = true;
            this.lbxTarget.ItemHeight = 12;
            this.lbxTarget.Location = new System.Drawing.Point(0, 0);
            this.lbxTarget.Name = "lbxTarget";
            this.lbxTarget.Size = new System.Drawing.Size(700, 225);
            this.lbxTarget.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(310, 18);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Address";
            // 
            // txtEventPortNo
            // 
            this.txtEventPortNo.Location = new System.Drawing.Point(236, 18);
            this.txtEventPortNo.Name = "txtEventPortNo";
            this.txtEventPortNo.Size = new System.Drawing.Size(55, 21);
            this.txtEventPortNo.TabIndex = 1;
            this.txtEventPortNo.Text = "10000";
            // 
            // txtEventIPAddress
            // 
            this.txtEventIPAddress.Location = new System.Drawing.Point(83, 18);
            this.txtEventIPAddress.Name = "txtEventIPAddress";
            this.txtEventIPAddress.Size = new System.Drawing.Size(100, 21);
            this.txtEventIPAddress.TabIndex = 0;
            this.txtEventIPAddress.Text = "192.168.0.10";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtTime);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtPeriod);
            this.groupBox2.Controls.Add(this.chkSysTime);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.splitContainer1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtVDSPort);
            this.groupBox2.Controls.Add(this.txtVDSAddress);
            this.groupBox2.Controls.Add(this.btnAutoSync);
            this.groupBox2.Controls.Add(this.dtPicker);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.btnTrafficData);
            this.groupBox2.Location = new System.Drawing.Point(15, 293);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(756, 490);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "VDS 제어";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(504, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "시간마다 동기화";
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(448, 73);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(55, 21);
            this.txtPeriod.TabIndex = 6;
            this.txtPeriod.Text = "1";
            this.txtPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chkSysTime
            // 
            this.chkSysTime.AutoSize = true;
            this.chkSysTime.Checked = true;
            this.chkSysTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSysTime.Location = new System.Drawing.Point(243, 78);
            this.chkSysTime.Name = "chkSysTime";
            this.chkSysTime.Size = new System.Drawing.Size(88, 16);
            this.chkSysTime.TabIndex = 17;
            this.chkSysTime.Text = "시스템 시간";
            this.chkSysTime.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Historical Traffic Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(9, 103);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Size = new System.Drawing.Size(734, 381);
            this.splitContainer1.SplitterDistance = 373;
            this.splitContainer1.TabIndex = 15;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbxSend);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(373, 381);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "송신 정보";
            // 
            // lbxSend
            // 
            this.lbxSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxSend.FormattingEnabled = true;
            this.lbxSend.ItemHeight = 12;
            this.lbxSend.Location = new System.Drawing.Point(3, 17);
            this.lbxSend.Name = "lbxSend";
            this.lbxSend.ScrollAlwaysVisible = true;
            this.lbxSend.Size = new System.Drawing.Size(367, 361);
            this.lbxSend.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbxReceive);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(357, 381);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "수신 정보";
            // 
            // lbxReceive
            // 
            this.lbxReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxReceive.FormattingEnabled = true;
            this.lbxReceive.ItemHeight = 12;
            this.lbxReceive.Location = new System.Drawing.Point(3, 17);
            this.lbxReceive.Name = "lbxReceive";
            this.lbxReceive.ScrollAlwaysVisible = true;
            this.lbxReceive.Size = new System.Drawing.Size(351, 361);
            this.lbxReceive.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(200, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "IP Address";
            // 
            // txtVDSPort
            // 
            this.txtVDSPort.Location = new System.Drawing.Point(233, 18);
            this.txtVDSPort.Name = "txtVDSPort";
            this.txtVDSPort.Size = new System.Drawing.Size(55, 21);
            this.txtVDSPort.TabIndex = 12;
            this.txtVDSPort.Text = "12000";
            // 
            // txtVDSAddress
            // 
            this.txtVDSAddress.Location = new System.Drawing.Point(80, 18);
            this.txtVDSAddress.Name = "txtVDSAddress";
            this.txtVDSAddress.Size = new System.Drawing.Size(100, 21);
            this.txtVDSAddress.TabIndex = 11;
            this.txtVDSAddress.Text = "192.168.0.11";
            // 
            // btnAutoSync
            // 
            this.btnAutoSync.Location = new System.Drawing.Point(603, 73);
            this.btnAutoSync.Name = "btnAutoSync";
            this.btnAutoSync.Size = new System.Drawing.Size(147, 23);
            this.btnAutoSync.TabIndex = 8;
            this.btnAutoSync.Text = "자동 시간 동기화 시작";
            this.btnAutoSync.UseVisualStyleBackColor = true;
            this.btnAutoSync.Click += new System.EventHandler(this.button7_Click);
            // 
            // dtPicker
            // 
            this.dtPicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtPicker.Location = new System.Drawing.Point(9, 75);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(152, 21);
            this.dtPicker.TabIndex = 6;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(468, 46);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(94, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "검지기 정지";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(368, 46);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(94, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "검지기 시작";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(337, 75);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(105, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "검지기 시각 설정";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(268, 46);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(94, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "장비 상태";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(168, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Echo Back ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnTrafficData
            // 
            this.btnTrafficData.Location = new System.Drawing.Point(568, 46);
            this.btnTrafficData.Name = "btnTrafficData";
            this.btnTrafficData.Size = new System.Drawing.Size(129, 23);
            this.btnTrafficData.TabIndex = 0;
            this.btnTrafficData.Text = "Historical Data 시작";
            this.btnTrafficData.UseVisualStyleBackColor = true;
            this.btnTrafficData.Visible = false;
            this.btnTrafficData.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(164, 75);
            this.txtTime.Mask = "00:00:00.00";
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(73, 21);
            this.txtTime.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 786);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "건기연 프로토콜 VDS 제어 시뮬레이터";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitLane.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).EndInit();
            this.splitLane.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEventPortNo;
        private System.Windows.Forms.TextBox txtEventIPAddress;
        private System.Windows.Forms.SplitContainer splitLane;
        private System.Windows.Forms.ListBox lbxTarget;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnTrafficData;
        private System.Windows.Forms.Button btnAutoSync;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVDSPort;
        private System.Windows.Forms.TextBox txtVDSAddress;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbxSend;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lbxReceive;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkSysTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.MaskedTextBox txtTime;
    }
}

