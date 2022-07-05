namespace VideoVDSController
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
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEventPortNo = new System.Windows.Forms.TextBox();
            this.txtEventIPAddress = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.darkButton7 = new DarkUI.Controls.DarkButton();
            this.darkButton6 = new DarkUI.Controls.DarkButton();
            this.darkButton5 = new DarkUI.Controls.DarkButton();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkButton8 = new DarkUI.Controls.DarkButton();
            this.darkButton9 = new DarkUI.Controls.DarkButton();
            this.darkButton10 = new DarkUI.Controls.DarkButton();
            this.darkButton11 = new DarkUI.Controls.DarkButton();
            this.darkButton12 = new DarkUI.Controls.DarkButton();
            this.darkButton13 = new DarkUI.Controls.DarkButton();
            this.darkButton14 = new DarkUI.Controls.DarkButton();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(324, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "IP Address";
            // 
            // txtEventPortNo
            // 
            this.txtEventPortNo.Location = new System.Drawing.Point(250, 12);
            this.txtEventPortNo.Name = "txtEventPortNo";
            this.txtEventPortNo.Size = new System.Drawing.Size(55, 21);
            this.txtEventPortNo.TabIndex = 6;
            this.txtEventPortNo.Text = "13488";
            // 
            // txtEventIPAddress
            // 
            this.txtEventIPAddress.Location = new System.Drawing.Point(97, 12);
            this.txtEventIPAddress.Name = "txtEventIPAddress";
            this.txtEventIPAddress.Size = new System.Drawing.Size(100, 21);
            this.txtEventIPAddress.TabIndex = 5;
            this.txtEventIPAddress.Text = "223.171.32.245";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(428, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // darkButton7
            // 
            this.darkButton7.Location = new System.Drawing.Point(190, 366);
            this.darkButton7.Name = "darkButton7";
            this.darkButton7.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton7.Size = new System.Drawing.Size(127, 63);
            this.darkButton7.TabIndex = 17;
            this.darkButton7.Text = "VDSHistoricTrafficDataResponse";
            this.darkButton7.Click += new System.EventHandler(this.darkButton7_Click);
            // 
            // darkButton6
            // 
            this.darkButton6.Location = new System.Drawing.Point(45, 366);
            this.darkButton6.Name = "darkButton6";
            this.darkButton6.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton6.Size = new System.Drawing.Size(127, 63);
            this.darkButton6.TabIndex = 16;
            this.darkButton6.Text = "VDSHistoricTrafficDataRequest";
            this.darkButton6.Click += new System.EventHandler(this.darkButton6_Click);
            // 
            // darkButton5
            // 
            this.darkButton5.Location = new System.Drawing.Point(45, 285);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton5.Size = new System.Drawing.Size(127, 63);
            this.darkButton5.TabIndex = 15;
            this.darkButton5.Text = "VDSTrafficDataEvent";
            this.darkButton5.Click += new System.EventHandler(this.darkButton5_Click);
            // 
            // darkButton4
            // 
            this.darkButton4.Location = new System.Drawing.Point(190, 205);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(127, 63);
            this.darkButton4.TabIndex = 14;
            this.darkButton4.Text = "VDSHeartBeatResponse";
            this.darkButton4.Click += new System.EventHandler(this.darkButton4_Click);
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(45, 205);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(127, 63);
            this.darkButton3.TabIndex = 13;
            this.darkButton3.Text = "VDSHeartBeatRequest";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(190, 126);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(127, 63);
            this.darkButton2.TabIndex = 12;
            this.darkButton2.Text = "VDSAuthResponse";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(45, 126);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(127, 63);
            this.darkButton1.TabIndex = 11;
            this.darkButton1.Text = "VDSAuthRequest";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // darkButton8
            // 
            this.darkButton8.Location = new System.Drawing.Point(501, 126);
            this.darkButton8.Name = "darkButton8";
            this.darkButton8.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton8.Size = new System.Drawing.Size(77, 64);
            this.darkButton8.TabIndex = 18;
            this.darkButton8.Text = "SerialPort Open";
            this.darkButton8.Click += new System.EventHandler(this.darkButton8_Click);
            // 
            // darkButton9
            // 
            this.darkButton9.Location = new System.Drawing.Point(695, 126);
            this.darkButton9.Name = "darkButton9";
            this.darkButton9.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton9.Size = new System.Drawing.Size(79, 64);
            this.darkButton9.TabIndex = 19;
            this.darkButton9.Text = "SerialPort Close";
            this.darkButton9.Click += new System.EventHandler(this.darkButton9_Click);
            // 
            // darkButton10
            // 
            this.darkButton10.Location = new System.Drawing.Point(695, 205);
            this.darkButton10.Name = "darkButton10";
            this.darkButton10.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton10.Size = new System.Drawing.Size(79, 64);
            this.darkButton10.TabIndex = 21;
            this.darkButton10.Text = "SerialPort Close";
            // 
            // darkButton11
            // 
            this.darkButton11.Location = new System.Drawing.Point(501, 205);
            this.darkButton11.Name = "darkButton11";
            this.darkButton11.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton11.Size = new System.Drawing.Size(77, 64);
            this.darkButton11.TabIndex = 20;
            this.darkButton11.Text = "SerialPort Open";
            this.darkButton11.Click += new System.EventHandler(this.darkButton11_Click);
            // 
            // darkButton12
            // 
            this.darkButton12.Location = new System.Drawing.Point(602, 126);
            this.darkButton12.Name = "darkButton12";
            this.darkButton12.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton12.Size = new System.Drawing.Size(77, 64);
            this.darkButton12.TabIndex = 22;
            this.darkButton12.Text = "SerialPort Send";
            this.darkButton12.Click += new System.EventHandler(this.darkButton12_Click);
            // 
            // darkButton13
            // 
            this.darkButton13.Location = new System.Drawing.Point(602, 205);
            this.darkButton13.Name = "darkButton13";
            this.darkButton13.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton13.Size = new System.Drawing.Size(77, 64);
            this.darkButton13.TabIndex = 23;
            this.darkButton13.Text = "SerialPort Send";
            this.darkButton13.Click += new System.EventHandler(this.darkButton13_Click);
            // 
            // darkButton14
            // 
            this.darkButton14.Location = new System.Drawing.Point(501, 285);
            this.darkButton14.Name = "darkButton14";
            this.darkButton14.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton14.Size = new System.Drawing.Size(127, 63);
            this.darkButton14.TabIndex = 24;
            this.darkButton14.Text = "RTU 상태 응답";
            this.darkButton14.Click += new System.EventHandler(this.darkButton14_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.darkButton14);
            this.Controls.Add(this.darkButton13);
            this.Controls.Add(this.darkButton12);
            this.Controls.Add(this.darkButton10);
            this.Controls.Add(this.darkButton11);
            this.Controls.Add(this.darkButton9);
            this.Controls.Add(this.darkButton8);
            this.Controls.Add(this.darkButton7);
            this.Controls.Add(this.darkButton6);
            this.Controls.Add(this.darkButton5);
            this.Controls.Add(this.darkButton4);
            this.Controls.Add(this.darkButton3);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEventPortNo);
            this.Controls.Add(this.txtEventIPAddress);
            this.Name = "Form1";
            this.Text = "영상식 VDS 제어기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEventPortNo;
        private System.Windows.Forms.TextBox txtEventIPAddress;
        private System.Windows.Forms.Button button1;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton3;
        private DarkUI.Controls.DarkButton darkButton4;
        private DarkUI.Controls.DarkButton darkButton5;
        private DarkUI.Controls.DarkButton darkButton6;
        private DarkUI.Controls.DarkButton darkButton7;
        private DarkUI.Controls.DarkButton darkButton8;
        private DarkUI.Controls.DarkButton darkButton9;
        private DarkUI.Controls.DarkButton darkButton10;
        private DarkUI.Controls.DarkButton darkButton11;
        private DarkUI.Controls.DarkButton darkButton12;
        private DarkUI.Controls.DarkButton darkButton13;
        private DarkUI.Controls.DarkButton darkButton14;
    }
}

