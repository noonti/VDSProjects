namespace VDSController
{
    partial class VDSConfigForm
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
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.ucConfig = new VDSCommon.ucVDSConfig();
            this.SuspendLayout();
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(480, 617);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(75, 28);
            this.darkButton1.TabIndex = 7;
            this.darkButton1.Text = "확인";
            this.darkButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(590, 617);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(75, 28);
            this.darkButton2.TabIndex = 8;
            this.darkButton2.Text = "취소";
            this.darkButton2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ucConfig
            // 
            this.ucConfig.Location = new System.Drawing.Point(-8, -4);
            this.ucConfig.Name = "ucConfig";
            this.ucConfig.Size = new System.Drawing.Size(1144, 614);
            this.ucConfig.TabIndex = 9;
            this.ucConfig.Load += new System.EventHandler(this.ucConfig_Load);
            // 
            // VDSConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 655);
            this.Controls.Add(this.ucConfig);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VDSConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "설정";
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.VDSConfigForm_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkButton darkButton2;
        private VDSCommon.ucVDSConfig ucConfig;
        //private VDSCommon.ucVDSConfig ucVDSConfig1;
    }
}