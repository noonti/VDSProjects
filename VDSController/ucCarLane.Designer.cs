namespace VDSController
{
    partial class ucCarLane
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
            this.pnLane = new System.Windows.Forms.Panel();
            this.pbCenter = new System.Windows.Forms.PictureBox();
            this.pnLane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCenter)).BeginInit();
            this.SuspendLayout();
            // 
            // pnLane
            // 
            this.pnLane.AutoSize = true;
            this.pnLane.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pnLane.Controls.Add(this.pbCenter);
            this.pnLane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLane.Location = new System.Drawing.Point(0, 0);
            this.pnLane.Name = "pnLane";
            this.pnLane.Size = new System.Drawing.Size(101, 59);
            this.pnLane.TabIndex = 5;
            // 
            // pbCenter
            // 
            this.pbCenter.BackColor = System.Drawing.Color.Red;
            this.pbCenter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCenter.Location = new System.Drawing.Point(45, 3);
            this.pbCenter.Name = "pbCenter";
            this.pbCenter.Size = new System.Drawing.Size(2, 50);
            this.pbCenter.TabIndex = 0;
            this.pbCenter.TabStop = false;
            this.pbCenter.Visible = false;
            // 
            // ucCarLane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnLane);
            this.Name = "ucCarLane";
            this.Size = new System.Drawing.Size(101, 59);
            this.Load += new System.EventHandler(this.ucCarLane_Load);
            this.Resize += new System.EventHandler(this.ucCarLane_Resize);
            this.pnLane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbCenter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnLane;
        private System.Windows.Forms.PictureBox pbCenter;
    }
}
