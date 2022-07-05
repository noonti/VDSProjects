namespace VDSController
{
    partial class TrafficSummaryStatForm
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
            this.lbxTarget = new System.Windows.Forms.ListBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.lbTotalCount = new DarkUI.Controls.DarkLabel();
            this.cbLane = new DarkUI.Controls.DarkComboBox();
            this.ucEndTime = new VDSController.ucDateTimePicker();
            this.ucStartTime = new VDSController.ucDateTimePicker();
            this.SuspendLayout();
            // 
            // lbxTarget
            // 
            this.lbxTarget.FormattingEnabled = true;
            this.lbxTarget.ItemHeight = 19;
            this.lbxTarget.Location = new System.Drawing.Point(13, 65);
            this.lbxTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbxTarget.Name = "lbxTarget";
            this.lbxTarget.ScrollAlwaysVisible = true;
            this.lbxTarget.Size = new System.Drawing.Size(983, 783);
            this.lbxTarget.TabIndex = 14;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(465, 27);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(19, 19);
            this.darkLabel1.TabIndex = 19;
            this.darkLabel1.Text = "~";
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(754, 18);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(107, 37);
            this.darkButton1.TabIndex = 20;
            this.darkButton1.Text = "조회";
            this.darkButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(884, 18);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(107, 37);
            this.darkButton2.TabIndex = 21;
            this.darkButton2.Text = "닫기";
            this.darkButton2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lbTotalCount
            // 
            this.lbTotalCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lbTotalCount.Location = new System.Drawing.Point(19, 853);
            this.lbTotalCount.Name = "lbTotalCount";
            this.lbTotalCount.Size = new System.Drawing.Size(977, 37);
            this.lbTotalCount.TabIndex = 22;
            this.lbTotalCount.Text = "전체 갯수:  0";
            this.lbTotalCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbLane
            // 
            this.cbLane.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbLane.FormattingEnabled = true;
            this.cbLane.Location = new System.Drawing.Point(13, 24);
            this.cbLane.Name = "cbLane";
            this.cbLane.Size = new System.Drawing.Size(187, 27);
            this.cbLane.TabIndex = 24;
            // 
            // ucEndTime
            // 
            this.ucEndTime.Location = new System.Drawing.Point(490, 19);
            this.ucEndTime.Name = "ucEndTime";
            this.ucEndTime.Size = new System.Drawing.Size(258, 27);
            this.ucEndTime.TabIndex = 12;
            // 
            // ucStartTime
            // 
            this.ucStartTime.Location = new System.Drawing.Point(206, 19);
            this.ucStartTime.Name = "ucStartTime";
            this.ucStartTime.Size = new System.Drawing.Size(258, 27);
            this.ucStartTime.TabIndex = 10;
            // 
            // TrafficSummaryStatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 891);
            this.Controls.Add(this.cbLane);
            this.Controls.Add(this.lbTotalCount);
            this.Controls.Add(this.darkButton2);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.darkLabel1);
            this.Controls.Add(this.lbxTarget);
            this.Controls.Add(this.ucEndTime);
            this.Controls.Add(this.ucStartTime);
            this.Font = new System.Drawing.Font("나눔고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrafficSummaryStatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "차량 통계 화면";
            this.Load += new System.EventHandler(this.TrafficSummaryStatForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ucDateTimePicker ucEndTime;
        private ucDateTimePicker ucStartTime;
        private System.Windows.Forms.ListBox lbxTarget;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkLabel lbTotalCount;
        private DarkUI.Controls.DarkComboBox cbLane;
    }
}