namespace VDSCommon
{
    partial class VDSMessageBoxForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnYes = new DarkUI.Controls.DarkButton();
            this.lbMessage = new DarkUI.Controls.DarkLabel();
            this.btnNo = new DarkUI.Controls.DarkButton();
            this.btnOk = new DarkUI.Controls.DarkButton();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(83, 60);
            this.btnYes.Name = "btnYes";
            this.btnYes.Padding = new System.Windows.Forms.Padding(5);
            this.btnYes.Size = new System.Drawing.Size(103, 28);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "예";
            this.btnYes.Visible = false;
            this.btnYes.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // lbMessage
            // 
            this.lbMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbMessage.Location = new System.Drawing.Point(6, 22);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(365, 23);
            this.lbMessage.TabIndex = 1;
            this.lbMessage.Text = "삭제하시겠습니까?";
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(192, 60);
            this.btnNo.Name = "btnNo";
            this.btnNo.Padding = new System.Windows.Forms.Padding(5);
            this.btnNo.Size = new System.Drawing.Size(103, 28);
            this.btnNo.TabIndex = 2;
            this.btnNo.Text = "아니오";
            this.btnNo.Visible = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(142, 60);
            this.btnOk.Name = "btnOk";
            this.btnOk.Padding = new System.Windows.Forms.Padding(5);
            this.btnOk.Size = new System.Drawing.Size(103, 28);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "예";
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // VDSMessageBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 101);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VDSMessageBoxForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "메시지 박스";
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkButton btnYes;
        private DarkUI.Controls.DarkLabel lbMessage;
        private DarkUI.Controls.DarkButton btnNo;
        private DarkUI.Controls.DarkButton btnOk;
    }
}