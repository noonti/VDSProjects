namespace YSESimulator
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
            this.txtPortNo = new System.Windows.Forms.TextBox();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.button25 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.button28 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.button30 = new System.Windows.Forms.Button();
            this.button31 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lbxReceive = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbxSend = new System.Windows.Forms.ListBox();
            this.txtPollingTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPolling = new System.Windows.Forms.Button();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.dtTime = new System.Windows.Forms.DateTimePicker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(299, 11);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Connect";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(205, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "IP Address";
            // 
            // txtPortNo
            // 
            this.txtPortNo.Location = new System.Drawing.Point(238, 12);
            this.txtPortNo.Name = "txtPortNo";
            this.txtPortNo.Size = new System.Drawing.Size(55, 21);
            this.txtPortNo.TabIndex = 6;
            this.txtPortNo.Text = "5000";
            this.txtPortNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(85, 12);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(100, 21);
            this.txtIPAddress.TabIndex = 5;
            this.txtIPAddress.Text = "192.168.0.11";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(14, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "제어 동기화";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(148, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "검지기 데이터 요구";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(282, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(128, 23);
            this.button4.TabIndex = 14;
            this.button4.Text = "검지기 리셋";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(660, 58);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "동시전송";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(6, 20);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(149, 23);
            this.button5.TabIndex = 15;
            this.button5.Text = "검지기 지정(idx=1)";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button13);
            this.groupBox1.Controls.Add(this.button14);
            this.groupBox1.Controls.Add(this.button12);
            this.groupBox1.Controls.Add(this.button11);
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Location = new System.Drawing.Point(12, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 84);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "파라메터 다운로드";
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(622, 49);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(149, 23);
            this.button13.TabIndex = 24;
            this.button13.Text = "Auto ReSync(idx=20)";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(468, 49);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(149, 23);
            this.button14.TabIndex = 23;
            this.button14.Text = "OSC 임계치(idx=17)";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(314, 49);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(149, 23);
            this.button12.TabIndex = 22;
            this.button12.Text = "길이계산가능(idx=10)";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(161, 49);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(149, 23);
            this.button11.TabIndex = 21;
            this.button11.Text = "속도계산가능(idx=9)";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(6, 49);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(149, 23);
            this.button10.TabIndex = 20;
            this.button10.Text = "길이별 누적치(idx=8)";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(623, 20);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(149, 23);
            this.button9.TabIndex = 19;
            this.button9.Text = "속도별 누적치(idx=7)";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(468, 20);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(149, 23);
            this.button8.TabIndex = 18;
            this.button8.Text = "length Category(idx=6)";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(314, 20);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(149, 23);
            this.button7.TabIndex = 17;
            this.button7.Text = "Speed Category(idx=5)";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(160, 20);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(149, 23);
            this.button6.TabIndex = 16;
            this.button6.Text = "Polling Cycle(idx=3)";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button15);
            this.groupBox2.Controls.Add(this.button16);
            this.groupBox2.Controls.Add(this.button17);
            this.groupBox2.Controls.Add(this.button18);
            this.groupBox2.Controls.Add(this.button19);
            this.groupBox2.Controls.Add(this.button20);
            this.groupBox2.Controls.Add(this.button21);
            this.groupBox2.Controls.Add(this.button22);
            this.groupBox2.Controls.Add(this.button23);
            this.groupBox2.Controls.Add(this.button24);
            this.groupBox2.Location = new System.Drawing.Point(14, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 84);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "파라메터 업로드";
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(622, 49);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(149, 23);
            this.button15.TabIndex = 24;
            this.button15.Tag = "20";
            this.button15.Text = "Auto ReSync(idx=20)";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button24_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(468, 49);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(149, 23);
            this.button16.TabIndex = 23;
            this.button16.Tag = "17";
            this.button16.Text = "OSC 임계치(idx=17)";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button24_Click);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(314, 49);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(149, 23);
            this.button17.TabIndex = 22;
            this.button17.Tag = "10";
            this.button17.Text = "길이계산가능(idx=10)";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button24_Click);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(161, 49);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(149, 23);
            this.button18.TabIndex = 21;
            this.button18.Tag = "9";
            this.button18.Text = "속도계산가능(idx=9)";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button24_Click);
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(6, 49);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(149, 23);
            this.button19.TabIndex = 20;
            this.button19.Tag = "8";
            this.button19.Text = "길이별 누적치(idx=8)";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button24_Click);
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(622, 20);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(149, 23);
            this.button20.TabIndex = 19;
            this.button20.Tag = "7";
            this.button20.Text = "속도별 누적치(idx=7)";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button24_Click);
            // 
            // button21
            // 
            this.button21.Location = new System.Drawing.Point(468, 20);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(149, 23);
            this.button21.TabIndex = 18;
            this.button21.Tag = "6";
            this.button21.Text = "length Category(idx=6)";
            this.button21.UseVisualStyleBackColor = true;
            this.button21.Click += new System.EventHandler(this.button24_Click);
            // 
            // button22
            // 
            this.button22.Location = new System.Drawing.Point(314, 20);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(149, 23);
            this.button22.TabIndex = 17;
            this.button22.Tag = "5";
            this.button22.Text = "Speed Category(idx=5)";
            this.button22.UseVisualStyleBackColor = true;
            this.button22.Click += new System.EventHandler(this.button24_Click);
            // 
            // button23
            // 
            this.button23.Location = new System.Drawing.Point(161, 20);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(149, 23);
            this.button23.TabIndex = 16;
            this.button23.Tag = "3";
            this.button23.Text = "Polling Cycle(idx=3)";
            this.button23.UseVisualStyleBackColor = true;
            this.button23.Click += new System.EventHandler(this.button24_Click);
            // 
            // button24
            // 
            this.button24.Location = new System.Drawing.Point(6, 20);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(149, 23);
            this.button24.TabIndex = 15;
            this.button24.Tag = "1";
            this.button24.Text = "검지기 지정(idx=1)";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // button25
            // 
            this.button25.Location = new System.Drawing.Point(12, 267);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(128, 23);
            this.button25.TabIndex = 26;
            this.button25.Text = "Online Status";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.button25_Click);
            // 
            // button26
            // 
            this.button26.Location = new System.Drawing.Point(148, 267);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(128, 23);
            this.button26.TabIndex = 27;
            this.button26.Text = "Memory Check";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // button27
            // 
            this.button27.Location = new System.Drawing.Point(282, 267);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(128, 23);
            this.button27.TabIndex = 28;
            this.button27.Text = "Echo Message";
            this.button27.UseVisualStyleBackColor = true;
            this.button27.Click += new System.EventHandler(this.button27_Click);
            // 
            // button28
            // 
            this.button28.Location = new System.Drawing.Point(416, 267);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(128, 23);
            this.button28.TabIndex = 29;
            this.button28.Text = "Seq No Check";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click);
            // 
            // button29
            // 
            this.button29.Location = new System.Drawing.Point(550, 267);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(128, 23);
            this.button29.TabIndex = 30;
            this.button29.Text = "VER Check";
            this.button29.UseVisualStyleBackColor = true;
            this.button29.Click += new System.EventHandler(this.button29_Click);
            // 
            // button30
            // 
            this.button30.Location = new System.Drawing.Point(416, 296);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(128, 23);
            this.button30.TabIndex = 31;
            this.button30.Text = "온도/전압 체크";
            this.button30.UseVisualStyleBackColor = true;
            this.button30.Click += new System.EventHandler(this.button30_Click);
            // 
            // button31
            // 
            this.button31.Location = new System.Drawing.Point(194, 293);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(128, 23);
            this.button31.TabIndex = 32;
            this.button31.Text = "RTC 설정";
            this.button31.UseVisualStyleBackColor = true;
            this.button31.Click += new System.EventHandler(this.button31_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(14, 325);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(756, 371);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "패킷정보";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbxReceive);
            this.groupBox5.Location = new System.Drawing.Point(399, 20);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(357, 342);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "수신 정보";
            // 
            // lbxReceive
            // 
            this.lbxReceive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxReceive.FormattingEnabled = true;
            this.lbxReceive.HorizontalScrollbar = true;
            this.lbxReceive.ItemHeight = 12;
            this.lbxReceive.Location = new System.Drawing.Point(3, 17);
            this.lbxReceive.Name = "lbxReceive";
            this.lbxReceive.ScrollAlwaysVisible = true;
            this.lbxReceive.Size = new System.Drawing.Size(351, 322);
            this.lbxReceive.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbxSend);
            this.groupBox4.Location = new System.Drawing.Point(12, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(373, 342);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "송신 정보";
            // 
            // lbxSend
            // 
            this.lbxSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxSend.FormattingEnabled = true;
            this.lbxSend.HorizontalScrollbar = true;
            this.lbxSend.ItemHeight = 12;
            this.lbxSend.Location = new System.Drawing.Point(3, 17);
            this.lbxSend.Name = "lbxSend";
            this.lbxSend.ScrollAlwaysVisible = true;
            this.lbxSend.Size = new System.Drawing.Size(367, 322);
            this.lbxSend.TabIndex = 0;
            // 
            // txtPollingTime
            // 
            this.txtPollingTime.Location = new System.Drawing.Point(515, 13);
            this.txtPollingTime.Name = "txtPollingTime";
            this.txtPollingTime.Size = new System.Drawing.Size(51, 21);
            this.txtPollingTime.TabIndex = 34;
            this.txtPollingTime.Text = "30";
            this.txtPollingTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(430, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 12);
            this.label3.TabIndex = 35;
            this.label3.Text = "폴링 주기(초)";
            // 
            // btnPolling
            // 
            this.btnPolling.Location = new System.Drawing.Point(572, 13);
            this.btnPolling.Name = "btnPolling";
            this.btnPolling.Size = new System.Drawing.Size(75, 23);
            this.btnPolling.TabIndex = 36;
            this.btnPolling.Text = "폴링 시작";
            this.btnPolling.UseVisualStyleBackColor = true;
            this.btnPolling.Click += new System.EventHandler(this.button32_Click);
            // 
            // dtDate
            // 
            this.dtDate.CustomFormat = "yyyy-MM-dd";
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDate.Location = new System.Drawing.Point(14, 295);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(100, 21);
            this.dtDate.TabIndex = 38;
            // 
            // dtTime
            // 
            this.dtTime.CustomFormat = "HH:mm:ss";
            this.dtTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTime.Location = new System.Drawing.Point(117, 295);
            this.dtTime.Name = "dtTime";
            this.dtTime.ShowUpDown = true;
            this.dtTime.Size = new System.Drawing.Size(71, 21);
            this.dtTime.TabIndex = 37;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 708);
            this.Controls.Add(this.dtDate);
            this.Controls.Add(this.dtTime);
            this.Controls.Add(this.btnPolling);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPollingTime);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button31);
            this.Controls.Add(this.button30);
            this.Controls.Add(this.button29);
            this.Controls.Add(this.button28);
            this.Controls.Add(this.button27);
            this.Controls.Add(this.button26);
            this.Controls.Add(this.button25);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPortNo);
            this.Controls.Add(this.txtIPAddress);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPortNo;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button button22;
        private System.Windows.Forms.Button button23;
        private System.Windows.Forms.Button button24;
        private System.Windows.Forms.Button button25;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.Button button27;
        private System.Windows.Forms.Button button28;
        private System.Windows.Forms.Button button29;
        private System.Windows.Forms.Button button30;
        private System.Windows.Forms.Button button31;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListBox lbxReceive;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lbxSend;
        private System.Windows.Forms.TextBox txtPollingTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPolling;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.DateTimePicker dtTime;
    }
}

