namespace CCTVManageCtrl
{
    partial class ucAlarmManageCtrl
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
            this.CCTVPlayWindow = new System.Windows.Forms.PictureBox();
            this.lbxAlarm = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkPreview = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CCTVPlayWindow)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.CCTVPlayWindow);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbxAlarm);
            this.splitContainer1.Size = new System.Drawing.Size(1109, 765);
            this.splitContainer1.SplitterDistance = 641;
            this.splitContainer1.TabIndex = 0;
            // 
            // CCTVPlayWindow
            // 
            this.CCTVPlayWindow.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CCTVPlayWindow.Location = new System.Drawing.Point(1, 118);
            this.CCTVPlayWindow.Name = "CCTVPlayWindow";
            this.CCTVPlayWindow.Size = new System.Drawing.Size(640, 480);
            this.CCTVPlayWindow.TabIndex = 0;
            this.CCTVPlayWindow.TabStop = false;
            // 
            // lbxAlarm
            // 
            this.lbxAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxAlarm.FormattingEnabled = true;
            this.lbxAlarm.ItemHeight = 12;
            this.lbxAlarm.Location = new System.Drawing.Point(0, 0);
            this.lbxAlarm.Name = "lbxAlarm";
            this.lbxAlarm.Size = new System.Drawing.Size(464, 765);
            this.lbxAlarm.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.chkPreview);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1109, 35);
            this.panel1.TabIndex = 1;
            // 
            // chkPreview
            // 
            this.chkPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPreview.AutoSize = true;
            this.chkPreview.Location = new System.Drawing.Point(868, 10);
            this.chkPreview.Name = "chkPreview";
            this.chkPreview.Size = new System.Drawing.Size(69, 16);
            this.chkPreview.TabIndex = 0;
            this.chkPreview.Text = "Preview";
            this.chkPreview.UseVisualStyleBackColor = true;
            this.chkPreview.Click += new System.EventHandler(this.chkPreview_Click);
            // 
            // ucAlarmManageCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ucAlarmManageCtrl";
            this.Size = new System.Drawing.Size(1109, 765);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CCTVPlayWindow)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox CCTVPlayWindow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbxAlarm;
        private System.Windows.Forms.CheckBox chkPreview;
    }
}
