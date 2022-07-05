namespace VDSCommon.VitualKeyboard
{
    partial class ucKeyButton
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
            this.pbButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbButton)).BeginInit();
            this.SuspendLayout();
            // 
            // pbButton
            // 
            this.pbButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbButton.Location = new System.Drawing.Point(0, 0);
            this.pbButton.Name = "pbButton";
            this.pbButton.Size = new System.Drawing.Size(46, 45);
            this.pbButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbButton.TabIndex = 1;
            this.pbButton.TabStop = false;
            // 
            // ucKeyButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbButton);
            this.Name = "ucKeyButton";
            this.Size = new System.Drawing.Size(46, 45);
            this.Load += new System.EventHandler(this.ucKeyButton_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbButton;
    }
}
