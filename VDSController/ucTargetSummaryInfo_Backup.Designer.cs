namespace VDSController
{
    partial class ucTargetSummaryInfo_Backup
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
            this.lbxTarget = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbVecycleCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).BeginInit();
            this.splitLane.Panel1.SuspendLayout();
            this.splitLane.Panel2.SuspendLayout();
            this.splitLane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitLane
            // 
            this.splitLane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitLane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitLane.IsSplitterFixed = true;
            this.splitLane.Location = new System.Drawing.Point(0, 0);
            this.splitLane.Name = "splitLane";
            // 
            // splitLane.Panel1
            // 
            this.splitLane.Panel1.Controls.Add(this.lbLane);
            // 
            // splitLane.Panel2
            // 
            this.splitLane.Panel2.Controls.Add(this.splitContainer1);
            this.splitLane.Size = new System.Drawing.Size(961, 56);
            this.splitLane.SplitterDistance = 70;
            this.splitLane.SplitterWidth = 1;
            this.splitLane.TabIndex = 1;
            // 
            // lbLane
            // 
            this.lbLane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLane.AutoSize = true;
            this.lbLane.Location = new System.Drawing.Point(22, 22);
            this.lbLane.Name = "lbLane";
            this.lbLane.Size = new System.Drawing.Size(29, 12);
            this.lbLane.TabIndex = 0;
            this.lbLane.Text = "차로";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbxTarget);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(890, 56);
            this.splitContainer1.SplitterDistance = 780;
            this.splitContainer1.TabIndex = 2;
            // 
            // lbxTarget
            // 
            this.lbxTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxTarget.FormattingEnabled = true;
            this.lbxTarget.ItemHeight = 12;
            this.lbxTarget.Location = new System.Drawing.Point(0, 0);
            this.lbxTarget.Name = "lbxTarget";
            this.lbxTarget.ScrollAlwaysVisible = true;
            this.lbxTarget.Size = new System.Drawing.Size(780, 56);
            this.lbxTarget.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbVecycleCount);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(106, 56);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "실시간 카운트";
            // 
            // lbVecycleCount
            // 
            this.lbVecycleCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbVecycleCount.Location = new System.Drawing.Point(3, 17);
            this.lbVecycleCount.Name = "lbVecycleCount";
            this.lbVecycleCount.Size = new System.Drawing.Size(100, 36);
            this.lbVecycleCount.TabIndex = 0;
            this.lbVecycleCount.Text = "0";
            this.lbVecycleCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucTargetSummaryInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitLane);
            this.Name = "ucTargetSummaryInfo";
            this.Size = new System.Drawing.Size(961, 56);
            this.splitLane.Panel1.ResumeLayout(false);
            this.splitLane.Panel1.PerformLayout();
            this.splitLane.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLane)).EndInit();
            this.splitLane.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitLane;
        private System.Windows.Forms.Label lbLane;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbxTarget;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbVecycleCount;
    }
}
