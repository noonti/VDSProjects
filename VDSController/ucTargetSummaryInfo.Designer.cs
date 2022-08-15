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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lbLane = new System.Windows.Forms.Label();
            this.segmentCount = new DmitryBrant.CustomControls.SevenSegmentArray();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).BeginInit();
            this.splitLane.Panel1.SuspendLayout();
            this.splitLane.Panel2.SuspendLayout();
            this.splitLane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitLane
            // 
            this.splitLane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitLane.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.splitLane.IsSplitterFixed = true;
            this.splitLane.Location = new System.Drawing.Point(0, 0);
            this.splitLane.Name = "splitLane";
            // 
            // splitLane.Panel1
            // 
            this.splitLane.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitLane.Panel2
            // 
            this.splitLane.Panel2.Controls.Add(this.lvTrafficData);
            this.splitLane.Size = new System.Drawing.Size(959, 104);
            this.splitLane.SplitterDistance = 190;
            this.splitLane.SplitterWidth = 1;
            this.splitLane.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lbLane);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.segmentCount);
            this.splitContainer2.Size = new System.Drawing.Size(190, 104);
            this.splitContainer2.SplitterDistance = 57;
            this.splitContainer2.TabIndex = 1;
            // 
            // lbLane
            // 
            this.lbLane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLane.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbLane.Location = new System.Drawing.Point(0, 0);
            this.lbLane.Name = "lbLane";
            this.lbLane.Size = new System.Drawing.Size(190, 57);
            this.lbLane.TabIndex = 1;
            this.lbLane.Text = "label1";
            this.lbLane.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // segmentCount
            // 
            this.segmentCount.ArrayCount = 5;
            this.segmentCount.BackColor = System.Drawing.SystemColors.Control;
            this.segmentCount.ColorBackground = System.Drawing.Color.Black;
            this.segmentCount.ColorDark = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.segmentCount.ColorLight = System.Drawing.Color.Yellow;
            this.segmentCount.DecimalShow = false;
            this.segmentCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentCount.ElementPadding = new System.Windows.Forms.Padding(2, 4, 1, 4);
            this.segmentCount.ElementWidth = 10;
            this.segmentCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.segmentCount.ItalicFactor = -0.1F;
            this.segmentCount.Location = new System.Drawing.Point(0, 0);
            this.segmentCount.Name = "segmentCount";
            this.segmentCount.Size = new System.Drawing.Size(190, 43);
            this.segmentCount.TabIndex = 12;
            this.segmentCount.TabStop = false;
            this.segmentCount.Value = "0";
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
            this.lvTrafficData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvTrafficData.FullRowSelect = true;
            this.lvTrafficData.GridLines = true;
            this.lvTrafficData.HideSelection = false;
            this.lvTrafficData.Location = new System.Drawing.Point(0, 0);
            this.lvTrafficData.MultiSelect = false;
            this.lvTrafficData.Name = "lvTrafficData";
            this.lvTrafficData.Size = new System.Drawing.Size(768, 104);
            this.lvTrafficData.TabIndex = 2;
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
            // ucTargetSummaryInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitLane);
            this.Name = "ucTargetSummaryInfo";
            this.Size = new System.Drawing.Size(959, 104);
            this.splitLane.Panel1.ResumeLayout(false);
            this.splitLane.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).EndInit();
            this.splitLane.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitLane;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label lbLane;
        private DmitryBrant.CustomControls.SevenSegmentArray segmentCount;
        private VDSCommon.ListViewEx lvTrafficData;
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
    }
}
