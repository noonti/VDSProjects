using VDSCommon;

namespace VDSWebAPIServer
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ucVDSGroupTreeView = new VDSWebAPIServer.ucTreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton5 = new DarkUI.Controls.DarkButton();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.lvVDSControl = new VDSCommon.ListViewEx();
            this.GrupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VDSID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VDSName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VDSTYPE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.USEYN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PROTOCOL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IPADDRESS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ONLINE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FrontGate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RearGate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Fan = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Heater = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Temperature = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.darkGroupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton6 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            this.동작ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.서비스시작ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.서비스중지ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.환경설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUser = new System.Windows.Forms.ToolStripMenuItem();
            this.도움말ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            this.darkGroupBox3.SuspendLayout();
            this.darkMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ucVDSGroupTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(2176, 1223);
            this.splitContainer1.SplitterDistance = 209;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // ucVDSGroupTreeView
            // 
            this.ucVDSGroupTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucVDSGroupTreeView.Location = new System.Drawing.Point(0, 0);
            this.ucVDSGroupTreeView.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ucVDSGroupTreeView.Name = "ucVDSGroupTreeView";
            this.ucVDSGroupTreeView.Size = new System.Drawing.Size(209, 1223);
            this.ucVDSGroupTreeView.TabIndex = 2;
            this.ucVDSGroupTreeView.Load += new System.EventHandler(this.ucVDSGroupTreeView_Load);
            this.ucVDSGroupTreeView.Click += new System.EventHandler(this.ucVDSGroupTreeView_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.darkGroupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvVDSControl);
            this.splitContainer2.Panel2.Controls.Add(this.darkGroupBox3);
            this.splitContainer2.Panel2.Controls.Add(this.darkGroupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(1962, 1223);
            this.splitContainer2.SplitterDistance = 102;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkButton2);
            this.darkGroupBox1.Controls.Add(this.darkButton5);
            this.darkGroupBox1.Controls.Add(this.darkButton4);
            this.darkGroupBox1.Controls.Add(this.darkButton3);
            this.darkGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.darkGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.darkGroupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox1.Size = new System.Drawing.Size(1962, 102);
            this.darkGroupBox1.TabIndex = 5;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "제어기 관리";
            // 
            // darkButton2
            // 
            this.darkButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton2.Location = new System.Drawing.Point(1769, 52);
            this.darkButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton2.Size = new System.Drawing.Size(178, 49);
            this.darkButton2.TabIndex = 10;
            this.darkButton2.Text = "새로고침";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton5
            // 
            this.darkButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton5.Location = new System.Drawing.Point(1517, 52);
            this.darkButton5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton5.Size = new System.Drawing.Size(178, 49);
            this.darkButton5.TabIndex = 9;
            this.darkButton5.Text = "제어기 삭제";
            this.darkButton5.Click += new System.EventHandler(this.darkButton5_Click);
            // 
            // darkButton4
            // 
            this.darkButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton4.Location = new System.Drawing.Point(1137, 52);
            this.darkButton4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton4.Size = new System.Drawing.Size(178, 49);
            this.darkButton4.TabIndex = 8;
            this.darkButton4.Text = "제어기 추가";
            this.darkButton4.Click += new System.EventHandler(this.darkButton4_Click);
            // 
            // darkButton3
            // 
            this.darkButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton3.Location = new System.Drawing.Point(1327, 52);
            this.darkButton3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton3.Size = new System.Drawing.Size(178, 49);
            this.darkButton3.TabIndex = 7;
            this.darkButton3.Text = "제어기 수정";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // lvVDSControl
            // 
            this.lvVDSControl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GrupName,
            this.VDSID,
            this.VDSName,
            this.VDSTYPE,
            this.USEYN,
            this.PROTOCOL,
            this.IPADDRESS,
            this.ONLINE,
            this.FrontGate,
            this.RearGate,
            this.Fan,
            this.Heater,
            this.Temperature});
            this.lvVDSControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvVDSControl.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lvVDSControl.FullRowSelect = true;
            this.lvVDSControl.GridLines = true;
            this.lvVDSControl.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvVDSControl.HideSelection = false;
            this.lvVDSControl.Location = new System.Drawing.Point(0, 0);
            this.lvVDSControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvVDSControl.Name = "lvVDSControl";
            this.lvVDSControl.Size = new System.Drawing.Size(1733, 1057);
            this.lvVDSControl.TabIndex = 5;
            this.lvVDSControl.UseCompatibleStateImageBehavior = false;
            this.lvVDSControl.View = System.Windows.Forms.View.Details;
            this.lvVDSControl.DoubleClick += new System.EventHandler(this.lvVDSControl_DoubleClick);
            // 
            // GrupName
            // 
            this.GrupName.Text = "GROUP 명";
            this.GrupName.Width = 100;
            // 
            // VDSID
            // 
            this.VDSID.Text = "VDS ID";
            this.VDSID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VDSID.Width = 100;
            // 
            // VDSName
            // 
            this.VDSName.Text = "제어기명";
            this.VDSName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VDSName.Width = 100;
            // 
            // VDSTYPE
            // 
            this.VDSTYPE.Text = "제어기 유형";
            this.VDSTYPE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VDSTYPE.Width = 100;
            // 
            // USEYN
            // 
            this.USEYN.Text = "사용여부";
            this.USEYN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.USEYN.Width = 100;
            // 
            // PROTOCOL
            // 
            this.PROTOCOL.Text = "프로토콜";
            this.PROTOCOL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PROTOCOL.Width = 100;
            // 
            // IPADDRESS
            // 
            this.IPADDRESS.Text = "IP Address";
            this.IPADDRESS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IPADDRESS.Width = 100;
            // 
            // ONLINE
            // 
            this.ONLINE.Text = "통신상태";
            this.ONLINE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ONLINE.Width = 100;
            // 
            // FrontGate
            // 
            this.FrontGate.Text = "앞문열림";
            this.FrontGate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FrontGate.Width = 100;
            // 
            // RearGate
            // 
            this.RearGate.Text = "뒷문열림";
            this.RearGate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RearGate.Width = 100;
            // 
            // Fan
            // 
            this.Fan.Text = "Fan";
            this.Fan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Fan.Width = 100;
            // 
            // Heater
            // 
            this.Heater.Text = "Heater";
            this.Heater.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Heater.Width = 100;
            // 
            // Temperature
            // 
            this.Temperature.Text = "함체온도";
            this.Temperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Temperature.Width = 100;
            // 
            // darkGroupBox3
            // 
            this.darkGroupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox3.Controls.Add(this.darkButton6);
            this.darkGroupBox3.Controls.Add(this.darkButton1);
            this.darkGroupBox3.Dock = System.Windows.Forms.DockStyle.Right;
            this.darkGroupBox3.Location = new System.Drawing.Point(1733, 0);
            this.darkGroupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox3.Name = "darkGroupBox3";
            this.darkGroupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox3.Size = new System.Drawing.Size(229, 1057);
            this.darkGroupBox3.TabIndex = 4;
            this.darkGroupBox3.TabStop = false;
            // 
            // darkButton6
            // 
            this.darkButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton6.Location = new System.Drawing.Point(25, 182);
            this.darkButton6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton6.Name = "darkButton6";
            this.darkButton6.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton6.Size = new System.Drawing.Size(178, 49);
            this.darkButton6.TabIndex = 14;
            this.darkButton6.Text = "원격설정";
            this.darkButton6.Click += new System.EventHandler(this.darkButton6_Click_1);
            // 
            // darkButton1
            // 
            this.darkButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.darkButton1.Location = new System.Drawing.Point(25, 108);
            this.darkButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.darkButton1.Size = new System.Drawing.Size(178, 49);
            this.darkButton1.TabIndex = 13;
            this.darkButton1.Text = "영상보기";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click_1);
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkGroupBox2.Location = new System.Drawing.Point(0, 1057);
            this.darkGroupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.darkGroupBox2.Size = new System.Drawing.Size(1962, 59);
            this.darkGroupBox2.TabIndex = 2;
            this.darkGroupBox2.TabStop = false;
            // 
            // darkMenuStrip1
            // 
            this.darkMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkMenuStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.darkMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.동작ToolStripMenuItem,
            this.환경설정ToolStripMenuItem,
            this.도움말ToolStripMenuItem});
            this.darkMenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkMenuStrip1.Name = "darkMenuStrip1";
            this.darkMenuStrip1.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
            this.darkMenuStrip1.Size = new System.Drawing.Size(2176, 28);
            this.darkMenuStrip1.TabIndex = 3;
            this.darkMenuStrip1.Text = "darkMenuStrip1";
            // 
            // 동작ToolStripMenuItem
            // 
            this.동작ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.동작ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.서비스시작ToolStripMenuItem,
            this.서비스중지ToolStripMenuItem});
            this.동작ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.동작ToolStripMenuItem.Name = "동작ToolStripMenuItem";
            this.동작ToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.동작ToolStripMenuItem.Text = "동작";
            // 
            // 서비스시작ToolStripMenuItem
            // 
            this.서비스시작ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.서비스시작ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.서비스시작ToolStripMenuItem.Name = "서비스시작ToolStripMenuItem";
            this.서비스시작ToolStripMenuItem.Size = new System.Drawing.Size(172, 26);
            this.서비스시작ToolStripMenuItem.Text = "서비스 시작";
            // 
            // 서비스중지ToolStripMenuItem
            // 
            this.서비스중지ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.서비스중지ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.서비스중지ToolStripMenuItem.Name = "서비스중지ToolStripMenuItem";
            this.서비스중지ToolStripMenuItem.Size = new System.Drawing.Size(172, 26);
            this.서비스중지ToolStripMenuItem.Text = "서비스 중지";
            // 
            // 환경설정ToolStripMenuItem
            // 
            this.환경설정ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.환경설정ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem1,
            this.MenuUser});
            this.환경설정ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.환경설정ToolStripMenuItem.Name = "환경설정ToolStripMenuItem";
            this.환경설정ToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.환경설정ToolStripMenuItem.Text = "환경설정";
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(172, 26);
            this.ToolStripMenuItem1.Text = "환경설정";
            this.ToolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // MenuUser
            // 
            this.MenuUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.MenuUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.MenuUser.Name = "MenuUser";
            this.MenuUser.Size = new System.Drawing.Size(172, 26);
            this.MenuUser.Text = "사용자 관리";
            this.MenuUser.Click += new System.EventHandler(this.MenuUser_Click);
            // 
            // 도움말ToolStripMenuItem
            // 
            this.도움말ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.도움말ToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.도움말ToolStripMenuItem.Name = "도움말ToolStripMenuItem";
            this.도움말ToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.도움말ToolStripMenuItem.Text = "도움말";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2176, 1251);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.darkMenuStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VDS 유지보수 프로그램";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox3.ResumeLayout(false);
            this.darkMenuStrip1.ResumeLayout(false);
            this.darkMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ucTreeView ucVDSGroupTreeView;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 동작ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 서비스시작ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 서비스중지ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 환경설정ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 도움말ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkButton darkButton5;
        private DarkUI.Controls.DarkButton darkButton4;
        private DarkUI.Controls.DarkButton darkButton3;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkButton darkButton2;
        private System.Windows.Forms.ToolStripMenuItem MenuUser;
        private DarkUI.Controls.DarkGroupBox darkGroupBox3;
        private DarkUI.Controls.DarkButton darkButton6;
        private DarkUI.Controls.DarkButton darkButton1;
        private ListViewEx lvVDSControl;
        private System.Windows.Forms.ColumnHeader GrupName;
        private System.Windows.Forms.ColumnHeader VDSID;
        private System.Windows.Forms.ColumnHeader VDSName;
        private System.Windows.Forms.ColumnHeader VDSTYPE;
        private System.Windows.Forms.ColumnHeader USEYN;
        private System.Windows.Forms.ColumnHeader PROTOCOL;
        private System.Windows.Forms.ColumnHeader IPADDRESS;
        private System.Windows.Forms.ColumnHeader ONLINE;
        private System.Windows.Forms.ColumnHeader FrontGate;
        private System.Windows.Forms.ColumnHeader RearGate;
        private System.Windows.Forms.ColumnHeader Fan;
        private System.Windows.Forms.ColumnHeader Heater;
        private System.Windows.Forms.ColumnHeader Temperature;
    }
}

