﻿namespace VideoVDSServerSimulator
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
            this.txtServerPortNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRTSPURL1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServerIPAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRTSPURL2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDirection = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLane = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.chkReverse = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(324, 20);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 14;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "Port";
            // 
            // txtServerPortNo
            // 
            this.txtServerPortNo.Location = new System.Drawing.Point(263, 20);
            this.txtServerPortNo.Name = "txtServerPortNo";
            this.txtServerPortNo.Size = new System.Drawing.Size(55, 21);
            this.txtServerPortNo.TabIndex = 11;
            this.txtServerPortNo.Text = "4693";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "rtsp url";
            // 
            // txtRTSPURL1
            // 
            this.txtRTSPURL1.Location = new System.Drawing.Point(104, 55);
            this.txtRTSPURL1.Name = "txtRTSPURL1";
            this.txtRTSPURL1.Size = new System.Drawing.Size(214, 21);
            this.txtRTSPURL1.TabIndex = 15;
            this.txtRTSPURL1.Text = "rtsp://218.36.126.200:8554/sample";
            this.txtRTSPURL1.Enter += new System.EventHandler(this.txtRTSPURL1_Enter);
            this.txtRTSPURL1.Leave += new System.EventHandler(this.txtRTSPURL1_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "IP Address";
            // 
            // txtServerIPAddress
            // 
            this.txtServerIPAddress.Location = new System.Drawing.Point(104, 19);
            this.txtServerIPAddress.Name = "txtServerIPAddress";
            this.txtServerIPAddress.Size = new System.Drawing.Size(100, 21);
            this.txtServerIPAddress.TabIndex = 17;
            this.txtServerIPAddress.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "rtsp url";
            // 
            // txtRTSPURL2
            // 
            this.txtRTSPURL2.Location = new System.Drawing.Point(104, 82);
            this.txtRTSPURL2.Name = "txtRTSPURL2";
            this.txtRTSPURL2.Size = new System.Drawing.Size(214, 21);
            this.txtRTSPURL2.TabIndex = 19;
            this.txtRTSPURL2.Text = "rtsp://218.36.126.200:8554/sample";
            this.txtRTSPURL2.Enter += new System.EventHandler(this.txtRTSPURL1_Enter);
            this.txtRTSPURL2.Leave += new System.EventHandler(this.txtRTSPURL1_Leave);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(405, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(174, 23);
            this.button1.TabIndex = 21;
            this.button1.Text = "Traffic Event 전송 시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(612, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(174, 23);
            this.button2.TabIndex = 22;
            this.button2.Text = "Traffic Event 전송  중지";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(405, 58);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(174, 23);
            this.button3.TabIndex = 23;
            this.button3.Text = "Traffic Event 전송(단일)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(612, 48);
            this.button4.Name = "button4";
            this.button4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button4.Size = new System.Drawing.Size(174, 23);
            this.button4.TabIndex = 24;
            this.button4.Text = "메시지 전송 테스트";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 26;
            this.label5.Text = "방향";
            // 
            // txtDirection
            // 
            this.txtDirection.Location = new System.Drawing.Point(104, 121);
            this.txtDirection.Name = "txtDirection";
            this.txtDirection.Size = new System.Drawing.Size(55, 21);
            this.txtDirection.TabIndex = 25;
            this.txtDirection.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(176, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 28;
            this.label6.Text = "차선";
            // 
            // txtLane
            // 
            this.txtLane.Location = new System.Drawing.Point(211, 121);
            this.txtLane.Name = "txtLane";
            this.txtLane.Size = new System.Drawing.Size(55, 21);
            this.txtLane.TabIndex = 27;
            this.txtLane.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(277, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 30;
            this.label7.Text = "속도";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(312, 121);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(55, 21);
            this.txtSpeed.TabIndex = 29;
            this.txtSpeed.Text = "60";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(415, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 32;
            this.label8.Text = "길이";
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(450, 121);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(55, 21);
            this.txtLength.TabIndex = 31;
            this.txtLength.Text = "450";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(511, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 12);
            this.label9.TabIndex = 33;
            this.label9.Text = "cm";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(373, 125);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 34;
            this.label10.Text = "km/h";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(636, 119);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(174, 23);
            this.button5.TabIndex = 35;
            this.button5.Text = "단건 전송";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // chkReverse
            // 
            this.chkReverse.AutoSize = true;
            this.chkReverse.Location = new System.Drawing.Point(541, 125);
            this.chkReverse.Name = "chkReverse";
            this.chkReverse.Size = new System.Drawing.Size(60, 16);
            this.chkReverse.TabIndex = 36;
            this.chkReverse.Text = "역주행";
            this.chkReverse.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 189);
            this.Controls.Add(this.chkReverse);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSpeed);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLane);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDirection);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRTSPURL2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtServerIPAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRTSPURL1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServerPortNo);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "영상식 VDS 서버 테스트";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerPortNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRTSPURL1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtServerIPAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRTSPURL2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDirection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLane;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox chkReverse;
    }
}

