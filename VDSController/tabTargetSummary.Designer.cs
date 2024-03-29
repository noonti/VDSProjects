﻿using VDSCommon;

namespace VDSController
{
    partial class tabTargetSummary
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
            this.components = new System.ComponentModel.Container();
            this.pnPlayer = new System.Windows.Forms.Panel();
            this.chkExpandPlayer = new System.Windows.Forms.CheckBox();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.rtspPlayer = new RTSPPlayerCtrl.RTSPPlayer();
            this.tabTrafficInfo = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.gbxRight = new DarkUI.Controls.DarkGroupBox();
            this.gbxLeft = new DarkUI.Controls.DarkGroupBox();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.btnSetTime = new DarkUI.Controls.DarkButton();
            this.lbServiceStartTime = new DarkUI.Controls.DarkLabel();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.lbLastCountTime_ = new DarkUI.Controls.DarkLabel();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.darkGroupBox4 = new DarkUI.Controls.DarkGroupBox();
            this.lvTrafficData = new VDSCommon.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.darkGroupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.cbLane = new DarkUI.Controls.DarkComboBox();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.timerSliding = new System.Windows.Forms.Timer(this.components);
            this.ucStartDate = new VDSController.ucDateTimePicker();
            this.ucCenterTrafficData = new VDSController.ucTrafficDataStat();
            this.ucEndTime = new VDSController.ucDateTimePicker();
            this.ucStartTime = new VDSController.ucDateTimePicker();
            this.pnPlayer.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            this.tabTrafficInfo.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panelContainer.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.darkGroupBox4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.darkGroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnPlayer
            // 
            this.pnPlayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnPlayer.Controls.Add(this.darkGroupBox1);
            this.pnPlayer.Controls.Add(this.rtspPlayer);
            this.pnPlayer.Controls.Add(this.chkExpandPlayer);
            this.pnPlayer.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnPlayer.Location = new System.Drawing.Point(0, 0);
            this.pnPlayer.Name = "pnPlayer";
            this.pnPlayer.Size = new System.Drawing.Size(1005, 658);
            this.pnPlayer.TabIndex = 34;
            // 
            // chkExpandPlayer
            // 
            this.chkExpandPlayer.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkExpandPlayer.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkExpandPlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkExpandPlayer.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.chkExpandPlayer.Location = new System.Drawing.Point(978, 0);
            this.chkExpandPlayer.Name = "chkExpandPlayer";
            this.chkExpandPlayer.Size = new System.Drawing.Size(27, 658);
            this.chkExpandPlayer.TabIndex = 0;
            this.chkExpandPlayer.Text = "<";
            this.chkExpandPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkExpandPlayer.UseVisualStyleBackColor = true;
            this.chkExpandPlayer.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkButton2);
            this.darkGroupBox1.Controls.Add(this.darkButton1);
            this.darkGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkGroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(978, 51);
            this.darkGroupBox1.TabIndex = 32;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "실시간 영상";
            // 
            // darkButton2
            // 
            this.darkButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkButton2.Location = new System.Drawing.Point(836, 13);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(126, 32);
            this.darkButton2.TabIndex = 1;
            this.darkButton2.Text = "영상종료";
            // 
            // darkButton1
            // 
            this.darkButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkButton1.Location = new System.Drawing.Point(695, 13);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(126, 32);
            this.darkButton1.TabIndex = 0;
            this.darkButton1.Text = "영상보기";
            // 
            // rtspPlayer
            // 
            this.rtspPlayer.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.rtspPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtspPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rtspPlayer.Location = new System.Drawing.Point(0, 0);
            this.rtspPlayer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtspPlayer.Name = "rtspPlayer";
            this.rtspPlayer.Size = new System.Drawing.Size(978, 658);
            this.rtspPlayer.TabIndex = 31;
            // 
            // tabTrafficInfo
            // 
            this.tabTrafficInfo.Controls.Add(this.tabPage1);
            this.tabTrafficInfo.Controls.Add(this.tabPage3);
            this.tabTrafficInfo.Controls.Add(this.tabPage2);
            this.tabTrafficInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabTrafficInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabTrafficInfo.Location = new System.Drawing.Point(1005, 0);
            this.tabTrafficInfo.Name = "tabTrafficInfo";
            this.tabTrafficInfo.SelectedIndex = 0;
            this.tabTrafficInfo.Size = new System.Drawing.Size(775, 658);
            this.tabTrafficInfo.TabIndex = 36;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.tabPage1.Controls.Add(this.panelContainer);
            this.tabPage1.Controls.Add(this.darkGroupBox2);
            this.tabPage1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(767, 630);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "실시간 차량 검지 정보(그래픽)";
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.panelContainer.Controls.Add(this.gbxRight);
            this.panelContainer.Controls.Add(this.gbxLeft);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panelContainer.Location = new System.Drawing.Point(3, 48);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(761, 579);
            this.panelContainer.TabIndex = 37;
            // 
            // gbxRight
            // 
            this.gbxRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxRight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gbxRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbxRight.Location = new System.Drawing.Point(2, 237);
            this.gbxRight.Name = "gbxRight";
            this.gbxRight.Size = new System.Drawing.Size(749, 122);
            this.gbxRight.TabIndex = 9;
            this.gbxRight.TabStop = false;
            this.gbxRight.Text = "Left->Right";
            // 
            // gbxLeft
            // 
            this.gbxLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxLeft.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gbxLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbxLeft.Location = new System.Drawing.Point(2, 28);
            this.gbxLeft.Name = "gbxLeft";
            this.gbxLeft.Size = new System.Drawing.Size(749, 122);
            this.gbxLeft.TabIndex = 8;
            this.gbxLeft.TabStop = false;
            this.gbxLeft.Text = "Right->Left";
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Controls.Add(this.btnSetTime);
            this.darkGroupBox2.Controls.Add(this.ucStartDate);
            this.darkGroupBox2.Controls.Add(this.lbServiceStartTime);
            this.darkGroupBox2.Controls.Add(this.darkLabel3);
            this.darkGroupBox2.Controls.Add(this.lbLastCountTime_);
            this.darkGroupBox2.Controls.Add(this.darkLabel1);
            this.darkGroupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkGroupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(761, 45);
            this.darkGroupBox2.TabIndex = 35;
            this.darkGroupBox2.TabStop = false;
            // 
            // btnSetTime
            // 
            this.btnSetTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSetTime.Location = new System.Drawing.Point(691, 8);
            this.btnSetTime.Name = "btnSetTime";
            this.btnSetTime.Padding = new System.Windows.Forms.Padding(5);
            this.btnSetTime.Size = new System.Drawing.Size(60, 32);
            this.btnSetTime.TabIndex = 2;
            this.btnSetTime.Text = "SET";
            // 
            // lbServiceStartTime
            // 
            this.lbServiceStartTime.AutoSize = true;
            this.lbServiceStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbServiceStartTime.ForeColor = System.Drawing.Color.Yellow;
            this.lbServiceStartTime.Location = new System.Drawing.Point(146, 15);
            this.lbServiceStartTime.Name = "lbServiceStartTime";
            this.lbServiceStartTime.Size = new System.Drawing.Size(17, 20);
            this.lbServiceStartTime.TabIndex = 3;
            this.lbServiceStartTime.Text = "[]";
            this.lbServiceStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(6, 15);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(105, 20);
            this.darkLabel3.TabIndex = 2;
            this.darkLabel3.Text = "서비스 시작시간: ";
            // 
            // lbLastCountTime_
            // 
            this.lbLastCountTime_.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLastCountTime_.AutoSize = true;
            this.lbLastCountTime_.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbLastCountTime_.ForeColor = System.Drawing.Color.Yellow;
            this.lbLastCountTime_.Location = new System.Drawing.Point(250, 13);
            this.lbLastCountTime_.Name = "lbLastCountTime_";
            this.lbLastCountTime_.Size = new System.Drawing.Size(17, 20);
            this.lbLastCountTime_.TabIndex = 1;
            this.lbLastCountTime_.Text = "[]";
            this.lbLastCountTime_.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbLastCountTime_.Visible = false;
            // 
            // darkLabel1
            // 
            this.darkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(283, 15);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(105, 20);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "카운트 시작시간: ";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.darkGroupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(767, 630);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "실시간 차량 검지정보(리스트)";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // darkGroupBox4
            // 
            this.darkGroupBox4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox4.Controls.Add(this.lvTrafficData);
            this.darkGroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkGroupBox4.Location = new System.Drawing.Point(0, 0);
            this.darkGroupBox4.Name = "darkGroupBox4";
            this.darkGroupBox4.Size = new System.Drawing.Size(767, 630);
            this.darkGroupBox4.TabIndex = 0;
            this.darkGroupBox4.TabStop = false;
            // 
            // lvTrafficData
            // 
            this.lvTrafficData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.lvTrafficData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTrafficData.FullRowSelect = true;
            this.lvTrafficData.GridLines = true;
            this.lvTrafficData.HideSelection = false;
            this.lvTrafficData.Location = new System.Drawing.Point(3, 17);
            this.lvTrafficData.MultiSelect = false;
            this.lvTrafficData.Name = "lvTrafficData";
            this.lvTrafficData.Size = new System.Drawing.Size(761, 610);
            this.lvTrafficData.TabIndex = 1;
            this.lvTrafficData.UseCompatibleStateImageBehavior = false;
            this.lvTrafficData.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "검지 시간";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "차선번호";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "방향";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "길이(cm)";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 70;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "속도(km/h)";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "점유시간(ms)";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "루프1 점유시간(ms)";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "루프2 점유시간(ms)";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "역주행 여부";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "센터 전송 여부";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader10.Width = 100;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.tabPage2.Controls.Add(this.ucCenterTrafficData);
            this.tabPage2.Controls.Add(this.darkGroupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(767, 630);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "센터 전송 차량 검지 정보";
            // 
            // darkGroupBox3
            // 
            this.darkGroupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox3.Controls.Add(this.cbLane);
            this.darkGroupBox3.Controls.Add(this.darkButton3);
            this.darkGroupBox3.Controls.Add(this.darkLabel2);
            this.darkGroupBox3.Controls.Add(this.ucEndTime);
            this.darkGroupBox3.Controls.Add(this.ucStartTime);
            this.darkGroupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkGroupBox3.Location = new System.Drawing.Point(3, 3);
            this.darkGroupBox3.Name = "darkGroupBox3";
            this.darkGroupBox3.Size = new System.Drawing.Size(761, 65);
            this.darkGroupBox3.TabIndex = 0;
            this.darkGroupBox3.TabStop = false;
            this.darkGroupBox3.Text = "조회 조건";
            // 
            // cbLane
            // 
            this.cbLane.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbLane.FormattingEnabled = true;
            this.cbLane.Location = new System.Drawing.Point(19, 29);
            this.cbLane.Name = "cbLane";
            this.cbLane.Size = new System.Drawing.Size(187, 22);
            this.cbLane.TabIndex = 0;
            // 
            // darkButton3
            // 
            this.darkButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton3.Location = new System.Drawing.Point(627, 22);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(128, 32);
            this.darkButton3.TabIndex = 3;
            this.darkButton3.Text = "조회";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(398, 34);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(14, 15);
            this.darkLabel2.TabIndex = 27;
            this.darkLabel2.Text = "~";
            // 
            // timerSliding
            // 
            this.timerSliding.Tick += new System.EventHandler(this.timerSliding_Tick);
            // 
            // ucStartDate
            // 
            this.ucStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ucStartDate.Location = new System.Drawing.Point(417, 7);
            this.ucStartDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucStartDate.Name = "ucStartDate";
            this.ucStartDate.Size = new System.Drawing.Size(255, 34);
            this.ucStartDate.TabIndex = 4;
            // 
            // ucCenterTrafficData
            // 
            this.ucCenterTrafficData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ucCenterTrafficData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCenterTrafficData.Location = new System.Drawing.Point(3, 68);
            this.ucCenterTrafficData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucCenterTrafficData.Name = "ucCenterTrafficData";
            this.ucCenterTrafficData.Size = new System.Drawing.Size(761, 559);
            this.ucCenterTrafficData.TabIndex = 1;
            // 
            // ucEndTime
            // 
            this.ucEndTime.Location = new System.Drawing.Point(418, 27);
            this.ucEndTime.Name = "ucEndTime";
            this.ucEndTime.Size = new System.Drawing.Size(258, 27);
            this.ucEndTime.TabIndex = 2;
            // 
            // ucStartTime
            // 
            this.ucStartTime.Location = new System.Drawing.Point(212, 27);
            this.ucStartTime.Name = "ucStartTime";
            this.ucStartTime.Size = new System.Drawing.Size(258, 27);
            this.ucStartTime.TabIndex = 1;
            // 
            // tabTargetSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabTrafficInfo);
            this.Controls.Add(this.pnPlayer);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "tabTargetSummary";
            this.Size = new System.Drawing.Size(1780, 658);
            this.Load += new System.EventHandler(this.tabTargetSummary_Load);
            this.Resize += new System.EventHandler(this.tabTargetSummary_Resize);
            this.pnPlayer.ResumeLayout(false);
            this.darkGroupBox1.ResumeLayout(false);
            this.tabTrafficInfo.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panelContainer.ResumeLayout(false);
            this.darkGroupBox2.ResumeLayout(false);
            this.darkGroupBox2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.darkGroupBox4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.darkGroupBox3.ResumeLayout(false);
            this.darkGroupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnPlayer;
        private System.Windows.Forms.CheckBox chkExpandPlayer;
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton1;
        private RTSPPlayerCtrl.RTSPPlayer rtspPlayer;
        private System.Windows.Forms.TabControl tabTrafficInfo;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panelContainer;
        private DarkUI.Controls.DarkGroupBox gbxRight;
        private DarkUI.Controls.DarkGroupBox gbxLeft;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkButton btnSetTime;
        private ucDateTimePicker ucStartDate;
        private DarkUI.Controls.DarkLabel lbServiceStartTime;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkLabel lbLastCountTime_;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private System.Windows.Forms.TabPage tabPage3;
        private DarkUI.Controls.DarkGroupBox darkGroupBox4;
        private ListViewEx lvTrafficData;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.TabPage tabPage2;
        private ucTrafficDataStat ucCenterTrafficData;
        private DarkUI.Controls.DarkGroupBox darkGroupBox3;
        private DarkUI.Controls.DarkComboBox cbLane;
        private DarkUI.Controls.DarkButton darkButton3;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private ucDateTimePicker ucEndTime;
        private ucDateTimePicker ucStartTime;
        private System.Windows.Forms.Timer timerSliding;
    }
}
