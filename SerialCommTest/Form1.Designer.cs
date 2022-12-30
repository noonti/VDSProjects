namespace SerialCommTest
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
            this.darkButton14 = new DarkUI.Controls.DarkButton();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPortName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.rdgFan2Off = new System.Windows.Forms.RadioButton();
            this.rdgFan2On = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rdgFan1Off = new System.Windows.Forms.RadioButton();
            this.rdgFan1On = new System.Windows.Forms.RadioButton();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.rdgPower = new System.Windows.Forms.RadioButton();
            this.rdgCamera = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lbFanValue = new System.Windows.Forms.Label();
            this.tbFan = new System.Windows.Forms.TrackBar();
            this.darkButton6 = new DarkUI.Controls.DarkButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lbHeaterValue = new System.Windows.Forms.Label();
            this.tbHeater = new System.Windows.Forms.TrackBar();
            this.darkButton8 = new DarkUI.Controls.DarkButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.rdgHeater2Off = new System.Windows.Forms.RadioButton();
            this.rdgHeater2On = new System.Windows.Forms.RadioButton();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.rdgHeater1Off = new System.Windows.Forms.RadioButton();
            this.rdgHeater1On = new System.Windows.Forms.RadioButton();
            this.darkButton10 = new DarkUI.Controls.DarkButton();
            this.lbResult = new System.Windows.Forms.Label();
            this.lbRequest = new System.Windows.Forms.Label();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.ucRTUStatus = new SerialComManageCtrl.ucRTUStatusBar();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton5 = new DarkUI.Controls.DarkButton();
            this.darkButton7 = new DarkUI.Controls.DarkButton();
            this.darkButton9 = new DarkUI.Controls.DarkButton();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFan)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeater)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.darkButton9);
            this.groupBox1.Controls.Add(this.darkButton14);
            this.groupBox1.Controls.Add(this.txtBaudRate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPortName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(811, 59);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "포트정보";
            // 
            // darkButton14
            // 
            this.darkButton14.Location = new System.Drawing.Point(527, 17);
            this.darkButton14.Name = "darkButton14";
            this.darkButton14.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton14.Size = new System.Drawing.Size(139, 36);
            this.darkButton14.TabIndex = 6;
            this.darkButton14.Text = "시작";
            this.darkButton14.Click += new System.EventHandler(this.darkButton14_Click);
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Location = new System.Drawing.Point(221, 18);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.Size = new System.Drawing.Size(100, 21);
            this.txtBaudRate.TabIndex = 3;
            this.txtBaudRate.Text = "9600";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "BaudRate";
            // 
            // txtPortName
            // 
            this.txtPortName.Location = new System.Drawing.Point(41, 18);
            this.txtPortName.Name = "txtPortName";
            this.txtPortName.Size = new System.Drawing.Size(100, 21);
            this.txtPortName.TabIndex = 1;
            this.txtPortName.Text = "COM1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "포트";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox9);
            this.groupBox4.Controls.Add(this.groupBox8);
            this.groupBox4.Controls.Add(this.darkButton3);
            this.groupBox4.Location = new System.Drawing.Point(14, 135);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(397, 63);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "FAN 동작";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.rdgFan2Off);
            this.groupBox9.Controls.Add(this.rdgFan2On);
            this.groupBox9.Location = new System.Drawing.Point(136, 18);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(125, 39);
            this.groupBox9.TabIndex = 22;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "FAN2";
            // 
            // rdgFan2Off
            // 
            this.rdgFan2Off.AutoSize = true;
            this.rdgFan2Off.Location = new System.Drawing.Point(62, 16);
            this.rdgFan2Off.Name = "rdgFan2Off";
            this.rdgFan2Off.Size = new System.Drawing.Size(47, 16);
            this.rdgFan2Off.TabIndex = 9;
            this.rdgFan2Off.Tag = "1";
            this.rdgFan2Off.Text = "정지";
            this.rdgFan2Off.UseVisualStyleBackColor = true;
            // 
            // rdgFan2On
            // 
            this.rdgFan2On.AutoSize = true;
            this.rdgFan2On.Checked = true;
            this.rdgFan2On.Location = new System.Drawing.Point(9, 16);
            this.rdgFan2On.Name = "rdgFan2On";
            this.rdgFan2On.Size = new System.Drawing.Size(47, 16);
            this.rdgFan2On.TabIndex = 8;
            this.rdgFan2On.TabStop = true;
            this.rdgFan2On.Tag = "0";
            this.rdgFan2On.Text = "구동";
            this.rdgFan2On.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rdgFan1Off);
            this.groupBox8.Controls.Add(this.rdgFan1On);
            this.groupBox8.Location = new System.Drawing.Point(6, 18);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(125, 39);
            this.groupBox8.TabIndex = 21;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "FAN1";
            // 
            // rdgFan1Off
            // 
            this.rdgFan1Off.AutoSize = true;
            this.rdgFan1Off.Location = new System.Drawing.Point(62, 16);
            this.rdgFan1Off.Name = "rdgFan1Off";
            this.rdgFan1Off.Size = new System.Drawing.Size(47, 16);
            this.rdgFan1Off.TabIndex = 9;
            this.rdgFan1Off.Tag = "1";
            this.rdgFan1Off.Text = "정지";
            this.rdgFan1Off.UseVisualStyleBackColor = true;
            // 
            // rdgFan1On
            // 
            this.rdgFan1On.AutoSize = true;
            this.rdgFan1On.Checked = true;
            this.rdgFan1On.Location = new System.Drawing.Point(9, 16);
            this.rdgFan1On.Name = "rdgFan1On";
            this.rdgFan1On.Size = new System.Drawing.Size(47, 16);
            this.rdgFan1On.TabIndex = 8;
            this.rdgFan1On.TabStop = true;
            this.rdgFan1On.Tag = "0";
            this.rdgFan1On.Text = "구동";
            this.rdgFan1On.UseVisualStyleBackColor = true;
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(289, 27);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(103, 25);
            this.darkButton3.TabIndex = 20;
            this.darkButton3.Text = "요청";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click_2);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.darkButton4);
            this.groupBox5.Controls.Add(this.rdgPower);
            this.groupBox5.Controls.Add(this.rdgCamera);
            this.groupBox5.Location = new System.Drawing.Point(12, 77);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(397, 46);
            this.groupBox5.TabIndex = 22;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "RESET";
            // 
            // darkButton4
            // 
            this.darkButton4.Location = new System.Drawing.Point(289, 14);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(103, 25);
            this.darkButton4.TabIndex = 20;
            this.darkButton4.Text = "요청";
            this.darkButton4.Click += new System.EventHandler(this.darkButton4_Click);
            // 
            // rdgPower
            // 
            this.rdgPower.AutoSize = true;
            this.rdgPower.Location = new System.Drawing.Point(180, 18);
            this.rdgPower.Name = "rdgPower";
            this.rdgPower.Size = new System.Drawing.Size(83, 16);
            this.rdgPower.TabIndex = 7;
            this.rdgPower.Tag = "1";
            this.rdgPower.Text = "Power Fail";
            this.rdgPower.UseVisualStyleBackColor = true;
            // 
            // rdgCamera
            // 
            this.rdgCamera.AutoSize = true;
            this.rdgCamera.Checked = true;
            this.rdgCamera.Location = new System.Drawing.Point(11, 18);
            this.rdgCamera.Name = "rdgCamera";
            this.rdgCamera.Size = new System.Drawing.Size(59, 16);
            this.rdgCamera.TabIndex = 6;
            this.rdgCamera.TabStop = true;
            this.rdgCamera.Tag = "0";
            this.rdgCamera.Text = "카메라";
            this.rdgCamera.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lbFanValue);
            this.groupBox6.Controls.Add(this.tbFan);
            this.groupBox6.Controls.Add(this.darkButton6);
            this.groupBox6.Location = new System.Drawing.Point(14, 214);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(397, 48);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "FAN 동작 임계값 설정";
            // 
            // lbFanValue
            // 
            this.lbFanValue.AutoSize = true;
            this.lbFanValue.Location = new System.Drawing.Point(258, 21);
            this.lbFanValue.Name = "lbFanValue";
            this.lbFanValue.Size = new System.Drawing.Size(11, 12);
            this.lbFanValue.TabIndex = 22;
            this.lbFanValue.Text = "0";
            // 
            // tbFan
            // 
            this.tbFan.Location = new System.Drawing.Point(14, 15);
            this.tbFan.Maximum = 127;
            this.tbFan.Minimum = -127;
            this.tbFan.Name = "tbFan";
            this.tbFan.Size = new System.Drawing.Size(238, 45);
            this.tbFan.TabIndex = 21;
            this.tbFan.Scroll += new System.EventHandler(this.tbFan_Scroll);
            // 
            // darkButton6
            // 
            this.darkButton6.Location = new System.Drawing.Point(289, 15);
            this.darkButton6.Name = "darkButton6";
            this.darkButton6.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton6.Size = new System.Drawing.Size(103, 25);
            this.darkButton6.TabIndex = 20;
            this.darkButton6.Text = "설정";
            this.darkButton6.Click += new System.EventHandler(this.darkButton6_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lbHeaterValue);
            this.groupBox7.Controls.Add(this.tbHeater);
            this.groupBox7.Controls.Add(this.darkButton8);
            this.groupBox7.Location = new System.Drawing.Point(426, 214);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(397, 48);
            this.groupBox7.TabIndex = 23;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Heater 동작 임계값 설정";
            // 
            // lbHeaterValue
            // 
            this.lbHeaterValue.AutoSize = true;
            this.lbHeaterValue.Location = new System.Drawing.Point(258, 21);
            this.lbHeaterValue.Name = "lbHeaterValue";
            this.lbHeaterValue.Size = new System.Drawing.Size(11, 12);
            this.lbHeaterValue.TabIndex = 22;
            this.lbHeaterValue.Text = "0";
            // 
            // tbHeater
            // 
            this.tbHeater.Location = new System.Drawing.Point(14, 15);
            this.tbHeater.Maximum = 127;
            this.tbHeater.Minimum = -127;
            this.tbHeater.Name = "tbHeater";
            this.tbHeater.Size = new System.Drawing.Size(238, 45);
            this.tbHeater.TabIndex = 21;
            this.tbHeater.Scroll += new System.EventHandler(this.tbHeater_Scroll);
            // 
            // darkButton8
            // 
            this.darkButton8.Location = new System.Drawing.Point(289, 15);
            this.darkButton8.Name = "darkButton8";
            this.darkButton8.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton8.Size = new System.Drawing.Size(103, 25);
            this.darkButton8.TabIndex = 20;
            this.darkButton8.Text = "설정";
            this.darkButton8.Click += new System.EventHandler(this.darkButton8_Click_1);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox11);
            this.groupBox10.Controls.Add(this.groupBox12);
            this.groupBox10.Controls.Add(this.darkButton10);
            this.groupBox10.Location = new System.Drawing.Point(426, 135);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(397, 63);
            this.groupBox10.TabIndex = 23;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Heater 동작";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.rdgHeater2Off);
            this.groupBox11.Controls.Add(this.rdgHeater2On);
            this.groupBox11.Location = new System.Drawing.Point(136, 18);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(125, 39);
            this.groupBox11.TabIndex = 22;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Heater2";
            // 
            // rdgHeater2Off
            // 
            this.rdgHeater2Off.AutoSize = true;
            this.rdgHeater2Off.Location = new System.Drawing.Point(62, 16);
            this.rdgHeater2Off.Name = "rdgHeater2Off";
            this.rdgHeater2Off.Size = new System.Drawing.Size(47, 16);
            this.rdgHeater2Off.TabIndex = 9;
            this.rdgHeater2Off.Tag = "1";
            this.rdgHeater2Off.Text = "정지";
            this.rdgHeater2Off.UseVisualStyleBackColor = true;
            // 
            // rdgHeater2On
            // 
            this.rdgHeater2On.AutoSize = true;
            this.rdgHeater2On.Checked = true;
            this.rdgHeater2On.Location = new System.Drawing.Point(9, 16);
            this.rdgHeater2On.Name = "rdgHeater2On";
            this.rdgHeater2On.Size = new System.Drawing.Size(47, 16);
            this.rdgHeater2On.TabIndex = 8;
            this.rdgHeater2On.TabStop = true;
            this.rdgHeater2On.Tag = "0";
            this.rdgHeater2On.Text = "구동";
            this.rdgHeater2On.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.rdgHeater1Off);
            this.groupBox12.Controls.Add(this.rdgHeater1On);
            this.groupBox12.Location = new System.Drawing.Point(6, 18);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(125, 39);
            this.groupBox12.TabIndex = 21;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Heater1";
            // 
            // rdgHeater1Off
            // 
            this.rdgHeater1Off.AutoSize = true;
            this.rdgHeater1Off.Location = new System.Drawing.Point(62, 16);
            this.rdgHeater1Off.Name = "rdgHeater1Off";
            this.rdgHeater1Off.Size = new System.Drawing.Size(47, 16);
            this.rdgHeater1Off.TabIndex = 9;
            this.rdgHeater1Off.Tag = "1";
            this.rdgHeater1Off.Text = "정지";
            this.rdgHeater1Off.UseVisualStyleBackColor = true;
            // 
            // rdgHeater1On
            // 
            this.rdgHeater1On.AutoSize = true;
            this.rdgHeater1On.Checked = true;
            this.rdgHeater1On.Location = new System.Drawing.Point(9, 16);
            this.rdgHeater1On.Name = "rdgHeater1On";
            this.rdgHeater1On.Size = new System.Drawing.Size(47, 16);
            this.rdgHeater1On.TabIndex = 8;
            this.rdgHeater1On.TabStop = true;
            this.rdgHeater1On.Tag = "0";
            this.rdgHeater1On.Text = "구동";
            this.rdgHeater1On.UseVisualStyleBackColor = true;
            // 
            // darkButton10
            // 
            this.darkButton10.Location = new System.Drawing.Point(289, 27);
            this.darkButton10.Name = "darkButton10";
            this.darkButton10.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton10.Size = new System.Drawing.Size(103, 25);
            this.darkButton10.TabIndex = 20;
            this.darkButton10.Text = "요청";
            this.darkButton10.Click += new System.EventHandler(this.darkButton10_Click_1);
            // 
            // lbResult
            // 
            this.lbResult.AutoSize = true;
            this.lbResult.Location = new System.Drawing.Point(18, 304);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(17, 12);
            this.lbResult.TabIndex = 24;
            this.lbResult.Text = "[]";
            // 
            // lbRequest
            // 
            this.lbRequest.AutoSize = true;
            this.lbRequest.Location = new System.Drawing.Point(18, 277);
            this.lbRequest.Name = "lbRequest";
            this.lbRequest.Size = new System.Drawing.Size(17, 12);
            this.lbRequest.TabIndex = 25;
            this.lbRequest.Text = "[]";
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(624, 91);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(204, 25);
            this.darkButton1.TabIndex = 21;
            this.darkButton1.Text = "상태 요청";
            this.darkButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click_1);
            // 
            // ucRTUStatus
            // 
            this.ucRTUStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ucRTUStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ucRTUStatus.Location = new System.Drawing.Point(0, 331);
            this.ucRTUStatus.Name = "ucRTUStatus";
            this.ucRTUStatus.Size = new System.Drawing.Size(1169, 54);
            this.ucRTUStatus.TabIndex = 26;
            this.ucRTUStatus.Load += new System.EventHandler(this.ucRTUStatusBar1_Load);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(848, 91);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(204, 25);
            this.darkButton2.TabIndex = 27;
            this.darkButton2.Text = "미완성 패킷1";
            this.darkButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.darkButton2.Visible = false;
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click_2);
            // 
            // darkButton5
            // 
            this.darkButton5.Location = new System.Drawing.Point(848, 122);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton5.Size = new System.Drawing.Size(204, 25);
            this.darkButton5.TabIndex = 28;
            this.darkButton5.Text = "미완성 패킷2";
            this.darkButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.darkButton5.Visible = false;
            this.darkButton5.Click += new System.EventHandler(this.darkButton5_Click_1);
            // 
            // darkButton7
            // 
            this.darkButton7.Location = new System.Drawing.Point(848, 60);
            this.darkButton7.Name = "darkButton7";
            this.darkButton7.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton7.Size = new System.Drawing.Size(204, 25);
            this.darkButton7.TabIndex = 29;
            this.darkButton7.Text = "완성 패킷";
            this.darkButton7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.darkButton7.Visible = false;
            this.darkButton7.Click += new System.EventHandler(this.darkButton7_Click_1);
            // 
            // darkButton9
            // 
            this.darkButton9.Location = new System.Drawing.Point(672, 17);
            this.darkButton9.Name = "darkButton9";
            this.darkButton9.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton9.Size = new System.Drawing.Size(139, 36);
            this.darkButton9.TabIndex = 7;
            this.darkButton9.Text = "종료";
            this.darkButton9.Click += new System.EventHandler(this.darkButton9_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1169, 385);
            this.Controls.Add(this.darkButton7);
            this.Controls.Add(this.darkButton5);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.ucRTUStatus);
            this.Controls.Add(this.lbRequest);
            this.Controls.Add(this.lbResult);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RTU 제어";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFan)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeater)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private DarkUI.Controls.DarkButton darkButton14;
        private System.Windows.Forms.TextBox txtBaudRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPortName;
        private System.Windows.Forms.GroupBox groupBox4;
        private DarkUI.Controls.DarkButton darkButton3;
        private System.Windows.Forms.GroupBox groupBox5;
        private DarkUI.Controls.DarkButton darkButton4;
        private System.Windows.Forms.RadioButton rdgPower;
        private System.Windows.Forms.RadioButton rdgCamera;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lbFanValue;
        private System.Windows.Forms.TrackBar tbFan;
        private DarkUI.Controls.DarkButton darkButton6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lbHeaterValue;
        private System.Windows.Forms.TrackBar tbHeater;
        private DarkUI.Controls.DarkButton darkButton8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton rdgFan2Off;
        private System.Windows.Forms.RadioButton rdgFan2On;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rdgFan1Off;
        private System.Windows.Forms.RadioButton rdgFan1On;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.RadioButton rdgHeater2Off;
        private System.Windows.Forms.RadioButton rdgHeater2On;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.RadioButton rdgHeater1Off;
        private System.Windows.Forms.RadioButton rdgHeater1On;
        private DarkUI.Controls.DarkButton darkButton10;
        private System.Windows.Forms.Label lbResult;
        private System.Windows.Forms.Label lbRequest;
        private SerialComManageCtrl.ucRTUStatusBar ucRTUStatus;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton5;
        private DarkUI.Controls.DarkButton darkButton7;
        private DarkUI.Controls.DarkButton darkButton9;
    }
}

