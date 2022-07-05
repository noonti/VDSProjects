namespace VDSController
{
    partial class ucTargetSummaryStat
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdg5Second = new System.Windows.Forms.RadioButton();
            this.rdg1Second = new System.Windows.Forms.RadioButton();
            this.rdgManual = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLane6 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLane5 = new System.Windows.Forms.TextBox();
            this.lbLastCheckDate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLane4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLane3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLane2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLane1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ucEndTime = new VDSController.ucDateTimePicker();
            this.ucStartTime = new VDSController.ucDateTimePicker();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.rdgSelect = new DarkUI.Controls.DarkRadioButton();
            this.rdg5Minute = new DarkUI.Controls.DarkRadioButton();
            this.rdg60Minute = new DarkUI.Controls.DarkRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.darkGroupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.ucEndTime);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.ucStartTime);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane6);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane5);
            this.splitContainer1.Panel2.Controls.Add(this.lbLastCheckDate);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane4);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane3);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane2);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.txtLane1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(957, 127);
            this.splitContainer1.SplitterDistance = 51;
            this.splitContainer1.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(872, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "조회";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdg5Second);
            this.groupBox2.Controls.Add(this.rdg1Second);
            this.groupBox2.Controls.Add(this.rdgManual);
            this.groupBox2.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(692, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 39);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "갱신 주기";
            // 
            // rdg5Second
            // 
            this.rdg5Second.AutoSize = true;
            this.rdg5Second.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdg5Second.Location = new System.Drawing.Point(119, 14);
            this.rdg5Second.Name = "rdg5Second";
            this.rdg5Second.Size = new System.Drawing.Size(50, 18);
            this.rdg5Second.TabIndex = 2;
            this.rdg5Second.Tag = "30";
            this.rdg5Second.Text = "30초";
            this.rdg5Second.UseVisualStyleBackColor = true;
            this.rdg5Second.Click += new System.EventHandler(this.rdgManual_Click);
            // 
            // rdg1Second
            // 
            this.rdg1Second.AutoSize = true;
            this.rdg1Second.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdg1Second.Location = new System.Drawing.Point(72, 14);
            this.rdg1Second.Name = "rdg1Second";
            this.rdg1Second.Size = new System.Drawing.Size(50, 18);
            this.rdg1Second.TabIndex = 1;
            this.rdg1Second.Tag = "10";
            this.rdg1Second.Text = "10초";
            this.rdg1Second.UseVisualStyleBackColor = true;
            this.rdg1Second.Click += new System.EventHandler(this.rdgManual_Click);
            // 
            // rdgManual
            // 
            this.rdgManual.AutoSize = true;
            this.rdgManual.Checked = true;
            this.rdgManual.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdgManual.Location = new System.Drawing.Point(19, 14);
            this.rdgManual.Name = "rdgManual";
            this.rdgManual.Size = new System.Drawing.Size(47, 18);
            this.rdgManual.TabIndex = 0;
            this.rdgManual.TabStop = true;
            this.rdgManual.Tag = "0";
            this.rdgManual.Text = "수동";
            this.rdgManual.UseVisualStyleBackColor = true;
            this.rdgManual.Click += new System.EventHandler(this.rdgManual_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(490, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "~";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(802, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 14);
            this.label8.TabIndex = 13;
            this.label8.Text = "6 차선";
            // 
            // txtLane6
            // 
            this.txtLane6.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane6.Location = new System.Drawing.Point(847, 31);
            this.txtLane6.Name = "txtLane6";
            this.txtLane6.ReadOnly = true;
            this.txtLane6.Size = new System.Drawing.Size(100, 21);
            this.txtLane6.TabIndex = 12;
            this.txtLane6.Text = "0";
            this.txtLane6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(644, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 14);
            this.label7.TabIndex = 11;
            this.label7.Text = "5 차선";
            // 
            // txtLane5
            // 
            this.txtLane5.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane5.Location = new System.Drawing.Point(689, 31);
            this.txtLane5.Name = "txtLane5";
            this.txtLane5.ReadOnly = true;
            this.txtLane5.Size = new System.Drawing.Size(100, 21);
            this.txtLane5.TabIndex = 10;
            this.txtLane5.Text = "0";
            this.txtLane5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lbLastCheckDate
            // 
            this.lbLastCheckDate.AutoSize = true;
            this.lbLastCheckDate.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbLastCheckDate.Location = new System.Drawing.Point(105, 11);
            this.lbLastCheckDate.Name = "lbLastCheckDate";
            this.lbLastCheckDate.Size = new System.Drawing.Size(10, 14);
            this.lbLastCheckDate.TabIndex = 9;
            this.lbLastCheckDate.Text = " ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(483, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 14);
            this.label6.TabIndex = 8;
            this.label6.Text = "4 차선";
            // 
            // txtLane4
            // 
            this.txtLane4.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane4.Location = new System.Drawing.Point(528, 31);
            this.txtLane4.Name = "txtLane4";
            this.txtLane4.ReadOnly = true;
            this.txtLane4.Size = new System.Drawing.Size(100, 21);
            this.txtLane4.TabIndex = 7;
            this.txtLane4.Text = "0";
            this.txtLane4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(325, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 14);
            this.label5.TabIndex = 6;
            this.label5.Text = "3 차선";
            // 
            // txtLane3
            // 
            this.txtLane3.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane3.Location = new System.Drawing.Point(370, 31);
            this.txtLane3.Name = "txtLane3";
            this.txtLane3.ReadOnly = true;
            this.txtLane3.Size = new System.Drawing.Size(100, 21);
            this.txtLane3.TabIndex = 5;
            this.txtLane3.Text = "0";
            this.txtLane3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(170, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 14);
            this.label4.TabIndex = 4;
            this.label4.Text = "2 차선";
            // 
            // txtLane2
            // 
            this.txtLane2.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane2.Location = new System.Drawing.Point(215, 31);
            this.txtLane2.Name = "txtLane2";
            this.txtLane2.ReadOnly = true;
            this.txtLane2.Size = new System.Drawing.Size(100, 21);
            this.txtLane2.TabIndex = 3;
            this.txtLane2.Text = "0";
            this.txtLane2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(10, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "1 차선";
            // 
            // txtLane1
            // 
            this.txtLane1.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtLane1.Location = new System.Drawing.Point(55, 31);
            this.txtLane1.Name = "txtLane1";
            this.txtLane1.ReadOnly = true;
            this.txtLane1.Size = new System.Drawing.Size(100, 21);
            this.txtLane1.TabIndex = 1;
            this.txtLane1.Text = "0";
            this.txtLane1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(10, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "최종 갱신 시간:";
            // 
            // ucEndTime
            // 
            this.ucEndTime.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucEndTime.Location = new System.Drawing.Point(507, 14);
            this.ucEndTime.Name = "ucEndTime";
            this.ucEndTime.Size = new System.Drawing.Size(179, 27);
            this.ucEndTime.TabIndex = 6;
            // 
            // ucStartTime
            // 
            this.ucStartTime.Font = new System.Drawing.Font("나눔고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucStartTime.Location = new System.Drawing.Point(305, 14);
            this.ucStartTime.Name = "ucStartTime";
            this.ucStartTime.Size = new System.Drawing.Size(179, 27);
            this.ucStartTime.TabIndex = 4;
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.rdg60Minute);
            this.darkGroupBox1.Controls.Add(this.rdg5Minute);
            this.darkGroupBox1.Controls.Add(this.rdgSelect);
            this.darkGroupBox1.Location = new System.Drawing.Point(3, 3);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(298, 43);
            this.darkGroupBox1.TabIndex = 10;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "날짜 구분";
            // 
            // rdgSelect
            // 
            this.rdgSelect.AutoSize = true;
            this.rdgSelect.Checked = true;
            this.rdgSelect.Location = new System.Drawing.Point(10, 17);
            this.rdgSelect.Name = "rdgSelect";
            this.rdgSelect.Size = new System.Drawing.Size(72, 18);
            this.rdgSelect.TabIndex = 0;
            this.rdgSelect.TabStop = true;
            this.rdgSelect.Text = "날짜 선택";
            // 
            // rdg5Minute
            // 
            this.rdg5Minute.AutoSize = true;
            this.rdg5Minute.Location = new System.Drawing.Point(105, 17);
            this.rdg5Minute.Name = "rdg5Minute";
            this.rdg5Minute.Size = new System.Drawing.Size(65, 18);
            this.rdg5Minute.TabIndex = 1;
            this.rdg5Minute.TabStop = true;
            this.rdg5Minute.Text = "5분자료";
            // 
            // rdg60Minute
            // 
            this.rdg60Minute.AutoSize = true;
            this.rdg60Minute.Location = new System.Drawing.Point(189, 17);
            this.rdg60Minute.Name = "rdg60Minute";
            this.rdg60Minute.Size = new System.Drawing.Size(76, 18);
            this.rdg60Minute.TabIndex = 2;
            this.rdg60Minute.TabStop = true;
            this.rdg60Minute.Text = "1시간자료";
            // 
            // ucTargetSummaryStat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.Controls.Add(this.splitContainer1);
            this.Name = "ucTargetSummaryStat";
            this.Size = new System.Drawing.Size(957, 127);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdg5Second;
        private System.Windows.Forms.RadioButton rdg1Second;
        private System.Windows.Forms.RadioButton rdgManual;
        private ucDateTimePicker ucEndTime;
        private System.Windows.Forms.Label label1;
        private ucDateTimePicker ucStartTime;
        private System.Windows.Forms.Label lbLastCheckDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLane4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLane3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLane2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLane1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLane6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLane5;
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkRadioButton rdg60Minute;
        private DarkUI.Controls.DarkRadioButton rdg5Minute;
        private DarkUI.Controls.DarkRadioButton rdgSelect;
    }
}
