namespace VDSCommon
{
    partial class ViewTrafficEventForm
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
            this.darkButton10 = new DarkUI.Controls.DarkButton();
            this.pbTrafficEvent = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbTrafficEvent)).BeginInit();
            this.SuspendLayout();
            // 
            // darkButton10
            // 
            this.darkButton10.Location = new System.Drawing.Point(339, 514);
            this.darkButton10.Name = "darkButton10";
            this.darkButton10.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton10.Size = new System.Drawing.Size(127, 33);
            this.darkButton10.TabIndex = 13;
            this.darkButton10.Text = "확인";
            this.darkButton10.Click += new System.EventHandler(this.darkButton10_Click);
            // 
            // pbTrafficEvent
            // 
            this.pbTrafficEvent.Location = new System.Drawing.Point(12, 12);
            this.pbTrafficEvent.Name = "pbTrafficEvent";
            this.pbTrafficEvent.Size = new System.Drawing.Size(786, 486);
            this.pbTrafficEvent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbTrafficEvent.TabIndex = 0;
            this.pbTrafficEvent.TabStop = false;
            // 
            // ViewTrafficEventForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 559);
            this.Controls.Add(this.darkButton10);
            this.Controls.Add(this.pbTrafficEvent);
            this.Name = "ViewTrafficEventForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "돌발 영상 보기";
            this.Load += new System.EventHandler(this.ViewTrafficEventForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbTrafficEvent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbTrafficEvent;
        private DarkUI.Controls.DarkButton darkButton10;
    }
}