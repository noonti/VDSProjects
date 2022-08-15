namespace VDSController
{
    partial class ucTargetSummaryInfo
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
            this.splitLane = new System.Windows.Forms.SplitContainer();
            this.lbLane = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.segmentCount = new DmitryBrant.CustomControls.SevenSegmentArray();
            this.lbCaption = new System.Windows.Forms.Label();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).BeginInit();
            this.splitLane.Panel1.SuspendLayout();
            this.splitLane.Panel2.SuspendLayout();
            this.splitLane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitLane
            // 
            this.splitLane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitLane.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitLane.IsSplitterFixed = true;
            this.splitLane.Location = new System.Drawing.Point(0, 0);
            this.splitLane.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.splitLane.Name = "splitLane";
            // 
            // splitLane.Panel1
            // 
            this.splitLane.Panel1.Controls.Add(this.lbLane);
            // 
            // splitLane.Panel2
            // 
            this.splitLane.Panel2.Controls.Add(this.splitContainer1);
            this.splitLane.Size = new System.Drawing.Size(1507, 182);
            this.splitLane.SplitterDistance = 70;
            this.splitLane.SplitterWidth = 2;
            this.splitLane.TabIndex = 1;
            // 
            // lbLane
            // 
            this.lbLane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLane.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbLane.Location = new System.Drawing.Point(0, 0);
            this.lbLane.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbLane.Name = "lbLane";
            this.lbLane.Size = new System.Drawing.Size(70, 182);
            this.lbLane.TabIndex = 0;
            this.lbLane.Text = "label1";
            this.lbLane.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1435, 182);
            this.splitContainer1.SplitterDistance = 1281;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.Color.Black;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.segmentCount);
            this.splitContainer3.Panel1.Controls.Add(this.lbCaption);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.darkButton1);
            this.splitContainer3.Size = new System.Drawing.Size(148, 182);
            this.splitContainer3.SplitterDistance = 126;
            this.splitContainer3.SplitterWidth = 7;
            this.splitContainer3.TabIndex = 3;
            // 
            // segmentCount
            // 
            this.segmentCount.ArrayCount = 5;
            this.segmentCount.BackColor = System.Drawing.SystemColors.Control;
            this.segmentCount.ColorBackground = System.Drawing.Color.Black;
            this.segmentCount.ColorDark = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.segmentCount.ColorLight = System.Drawing.Color.Yellow;
            this.segmentCount.DecimalShow = false;
            this.segmentCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.segmentCount.ElementPadding = new System.Windows.Forms.Padding(2, 4, 1, 4);
            this.segmentCount.ElementWidth = 10;
            this.segmentCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.segmentCount.ItalicFactor = -0.1F;
            this.segmentCount.Location = new System.Drawing.Point(0, 63);
            this.segmentCount.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.segmentCount.Name = "segmentCount";
            this.segmentCount.Size = new System.Drawing.Size(148, 63);
            this.segmentCount.TabIndex = 11;
            this.segmentCount.TabStop = false;
            this.segmentCount.Value = "0";
            this.segmentCount.Load += new System.EventHandler(this.segmentCount_Load);
            // 
            // lbCaption
            // 
            this.lbCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCaption.ForeColor = System.Drawing.Color.Yellow;
            this.lbCaption.Location = new System.Drawing.Point(0, 0);
            this.lbCaption.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbCaption.Name = "lbCaption";
            this.lbCaption.Size = new System.Drawing.Size(148, 35);
            this.lbCaption.TabIndex = 10;
            this.lbCaption.Text = "실시간 카운트";
            this.lbCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // darkButton1
            // 
            this.darkButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.darkButton1.Location = new System.Drawing.Point(0, -9);
            this.darkButton1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.darkButton1.Size = new System.Drawing.Size(148, 58);
            this.darkButton1.TabIndex = 0;
            this.darkButton1.Text = "상세보기";
            this.darkButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ucTargetSummaryInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitLane);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ucTargetSummaryInfo";
            this.Size = new System.Drawing.Size(1507, 182);
            this.splitLane.Panel1.ResumeLayout(false);
            this.splitLane.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).EndInit();
            this.splitLane.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitLane;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private DmitryBrant.CustomControls.SevenSegmentArray segmentCount;
        private System.Windows.Forms.Label lbCaption;
        private System.Windows.Forms.Label lbLane;
        private DarkUI.Controls.DarkButton darkButton1;
    }
}
