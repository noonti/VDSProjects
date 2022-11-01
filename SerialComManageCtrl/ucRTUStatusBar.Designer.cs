namespace SerialComManageCtrl
{
    partial class ucRTUStatusBar
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbHumidity = new System.Windows.Forms.Label();
            this.lbHeaterThreshold = new System.Windows.Forms.Label();
            this.lbFanThreshold = new System.Windows.Forms.Label();
            this.lbTemperature = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ledAVR = new VDSCommon.ucLEDLight();
            this.ledHeater = new VDSCommon.ucLEDLight();
            this.ledFan = new VDSCommon.ucLEDLight();
            this.ledRearGate = new VDSCommon.ucLEDLight();
            this.ledFrontGate = new VDSCommon.ucLEDLight();
            this.ucBlinkLight = new VDSCommon.ucLEDLight();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1200, 62);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RTU 정보";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 17);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.ledAVR);
            this.splitContainer1.Panel1.Controls.Add(this.ledHeater);
            this.splitContainer1.Panel1.Controls.Add(this.ledFan);
            this.splitContainer1.Panel1.Controls.Add(this.ledRearGate);
            this.splitContainer1.Panel1.Controls.Add(this.ledFrontGate);
            this.splitContainer1.Panel1.Controls.Add(this.ucBlinkLight);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbHumidity);
            this.splitContainer1.Panel2.Controls.Add(this.lbHeaterThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.lbFanThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.lbTemperature);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Size = new System.Drawing.Size(1194, 42);
            this.splitContainer1.SplitterDistance = 627;
            this.splitContainer1.TabIndex = 1;
            // 
            // lbHumidity
            // 
            this.lbHumidity.ForeColor = System.Drawing.SystemColors.Window;
            this.lbHumidity.Location = new System.Drawing.Point(495, 6);
            this.lbHumidity.Name = "lbHumidity";
            this.lbHumidity.Size = new System.Drawing.Size(53, 12);
            this.lbHumidity.TabIndex = 23;
            this.lbHumidity.Text = " ";
            this.lbHumidity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHeaterThreshold
            // 
            this.lbHeaterThreshold.ForeColor = System.Drawing.SystemColors.Window;
            this.lbHeaterThreshold.Location = new System.Drawing.Point(367, 6);
            this.lbHeaterThreshold.Name = "lbHeaterThreshold";
            this.lbHeaterThreshold.Size = new System.Drawing.Size(53, 12);
            this.lbHeaterThreshold.TabIndex = 22;
            this.lbHeaterThreshold.Text = " ";
            this.lbHeaterThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbFanThreshold
            // 
            this.lbFanThreshold.ForeColor = System.Drawing.SystemColors.Window;
            this.lbFanThreshold.Location = new System.Drawing.Point(227, 6);
            this.lbFanThreshold.Name = "lbFanThreshold";
            this.lbFanThreshold.Size = new System.Drawing.Size(53, 12);
            this.lbFanThreshold.TabIndex = 19;
            this.lbFanThreshold.Text = " ";
            this.lbFanThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTemperature
            // 
            this.lbTemperature.ForeColor = System.Drawing.SystemColors.Window;
            this.lbTemperature.Location = new System.Drawing.Point(71, 6);
            this.lbTemperature.Name = "lbTemperature";
            this.lbTemperature.Size = new System.Drawing.Size(53, 12);
            this.lbTemperature.TabIndex = 18;
            this.lbTemperature.Text = " ";
            this.lbTemperature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.Window;
            this.label8.Location = new System.Drawing.Point(425, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 17;
            this.label8.Text = "습도계측값";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.Window;
            this.label7.Location = new System.Drawing.Point(285, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "Heater임계값";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.Window;
            this.label4.Location = new System.Drawing.Point(129, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "FAN동작임계값 ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(13, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "함체온도";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 44;
            this.label1.Text = "통신상태";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(101, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 45;
            this.label2.Text = "앞문열림";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.Window;
            this.label5.Location = new System.Drawing.Point(202, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 46;
            this.label5.Text = "뒷문열림";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.Window;
            this.label6.Location = new System.Drawing.Point(298, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 12);
            this.label6.TabIndex = 47;
            this.label6.Text = "Fan";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.Window;
            this.label9.Location = new System.Drawing.Point(361, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 48;
            this.label9.Text = "Heater";
            // 
            // ledAVR
            // 
            this.ledAVR.Location = new System.Drawing.Point(556, 1);
            this.ledAVR.Name = "ledAVR";
            this.ledAVR.Size = new System.Drawing.Size(20, 23);
            this.ledAVR.TabIndex = 43;
            this.ledAVR.Visible = false;
            // 
            // ledHeater
            // 
            this.ledHeater.Location = new System.Drawing.Point(407, 1);
            this.ledHeater.Name = "ledHeater";
            this.ledHeater.Size = new System.Drawing.Size(20, 23);
            this.ledHeater.TabIndex = 42;
            // 
            // ledFan
            // 
            this.ledFan.Location = new System.Drawing.Point(326, 1);
            this.ledFan.Name = "ledFan";
            this.ledFan.Size = new System.Drawing.Size(20, 23);
            this.ledFan.TabIndex = 41;
            // 
            // ledRearGate
            // 
            this.ledRearGate.Location = new System.Drawing.Point(257, 1);
            this.ledRearGate.Name = "ledRearGate";
            this.ledRearGate.Size = new System.Drawing.Size(20, 23);
            this.ledRearGate.TabIndex = 40;
            // 
            // ledFrontGate
            // 
            this.ledFrontGate.Location = new System.Drawing.Point(156, 1);
            this.ledFrontGate.Name = "ledFrontGate";
            this.ledFrontGate.Size = new System.Drawing.Size(20, 23);
            this.ledFrontGate.TabIndex = 39;
            // 
            // ucBlinkLight
            // 
            this.ucBlinkLight.Location = new System.Drawing.Point(62, 1);
            this.ucBlinkLight.Name = "ucBlinkLight";
            this.ucBlinkLight.Size = new System.Drawing.Size(20, 23);
            this.ucBlinkLight.TabIndex = 38;
            // 
            // ucRTUStatusBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.Controls.Add(this.groupBox1);
            this.Name = "ucRTUStatusBar";
            this.Size = new System.Drawing.Size(1200, 62);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private VDSCommon.ucLEDLight ledAVR;
        private VDSCommon.ucLEDLight ledHeater;
        private VDSCommon.ucLEDLight ledFan;
        private VDSCommon.ucLEDLight ledRearGate;
        private VDSCommon.ucLEDLight ledFrontGate;
        private VDSCommon.ucLEDLight ucBlinkLight;
        private System.Windows.Forms.Label lbHumidity;
        private System.Windows.Forms.Label lbHeaterThreshold;
        private System.Windows.Forms.Label lbFanThreshold;
        private System.Windows.Forms.Label lbTemperature;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
