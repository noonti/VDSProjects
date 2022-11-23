namespace MClavisRadarController
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRemotePort = new System.Windows.Forms.TextBox();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.txtLocalPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbxLog = new System.Windows.Forms.ListBox();
            this.txtLastPacket = new System.Windows.Forms.TextBox();
            this.rdgServer = new System.Windows.Forms.RadioButton();
            this.rdgClient = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1605, 52);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 46);
            this.button1.TabIndex = 16;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(1454, 54);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(139, 46);
            this.btnStart.TabIndex = 15;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(438, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 24);
            this.label2.TabIndex = 14;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 24);
            this.label1.TabIndex = 13;
            this.label1.Text = "Radar Address";
            // 
            // txtRemotePort
            // 
            this.txtRemotePort.Location = new System.Drawing.Point(500, 54);
            this.txtRemotePort.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtRemotePort.Name = "txtRemotePort";
            this.txtRemotePort.Size = new System.Drawing.Size(99, 35);
            this.txtRemotePort.TabIndex = 12;
            this.txtRemotePort.Text = "45555";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.AcceptsReturn = true;
            this.txtServerAddress.Location = new System.Drawing.Point(215, 54);
            this.txtServerAddress.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(182, 35);
            this.txtServerAddress.TabIndex = 11;
            this.txtServerAddress.Text = "192.168.0.15";
            // 
            // txtLocalPort
            // 
            this.txtLocalPort.Location = new System.Drawing.Point(761, 54);
            this.txtLocalPort.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtLocalPort.Name = "txtLocalPort";
            this.txtLocalPort.Size = new System.Drawing.Size(99, 35);
            this.txtLocalPort.TabIndex = 18;
            this.txtLocalPort.Text = "45175";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(635, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 24);
            this.label4.TabIndex = 19;
            this.label4.Text = "Local Port";
            // 
            // lbxLog
            // 
            this.lbxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxLog.FormattingEnabled = true;
            this.lbxLog.ItemHeight = 24;
            this.lbxLog.Location = new System.Drawing.Point(41, 262);
            this.lbxLog.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lbxLog.Name = "lbxLog";
            this.lbxLog.Size = new System.Drawing.Size(1700, 580);
            this.lbxLog.TabIndex = 20;
            // 
            // txtLastPacket
            // 
            this.txtLastPacket.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastPacket.Location = new System.Drawing.Point(41, 208);
            this.txtLastPacket.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtLastPacket.Name = "txtLastPacket";
            this.txtLastPacket.Size = new System.Drawing.Size(1700, 35);
            this.txtLastPacket.TabIndex = 22;
            // 
            // rdgServer
            // 
            this.rdgServer.AutoSize = true;
            this.rdgServer.Checked = true;
            this.rdgServer.Location = new System.Drawing.Point(914, 58);
            this.rdgServer.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdgServer.Name = "rdgServer";
            this.rdgServer.Size = new System.Drawing.Size(137, 28);
            this.rdgServer.TabIndex = 23;
            this.rdgServer.TabStop = true;
            this.rdgServer.Text = "서버모드";
            this.rdgServer.UseVisualStyleBackColor = true;
            // 
            // rdgClient
            // 
            this.rdgClient.AutoSize = true;
            this.rdgClient.Location = new System.Drawing.Point(1079, 56);
            this.rdgClient.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rdgClient.Name = "rdgClient";
            this.rdgClient.Size = new System.Drawing.Size(137, 28);
            this.rdgClient.TabIndex = 24;
            this.rdgClient.Text = "서버모드";
            this.rdgClient.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(41, 150);
            this.button2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(199, 46);
            this.button2.TabIndex = 25;
            this.button2.Text = "Start Manager";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(290, 150);
            this.button3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(199, 46);
            this.button3.TabIndex = 26;
            this.button3.Text = "Stop Manager";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(600, 150);
            this.button4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(446, 46);
            this.button4.TabIndex = 27;
            this.button4.Text = "Add Inverse";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1079, 150);
            this.button5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(446, 46);
            this.button5.TabIndex = 28;
            this.button5.Text = "Find sequence";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1766, 850);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.rdgClient);
            this.Controls.Add(this.rdgServer);
            this.Controls.Add(this.txtLastPacket);
            this.Controls.Add(this.lbxLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLocalPort);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRemotePort);
            this.Controls.Add(this.txtServerAddress);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRemotePort;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtLocalPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbxLog;
        private System.Windows.Forms.TextBox txtLastPacket;
        private System.Windows.Forms.RadioButton rdgServer;
        private System.Windows.Forms.RadioButton rdgClient;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}

