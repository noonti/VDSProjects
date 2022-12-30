namespace VDSController
{
    partial class AddLaneInfoForm
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
            this.txtLaneName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.cbLane = new System.Windows.Forms.ComboBox();
            this.darkButton10 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.cbKorExLane = new System.Windows.Forms.ComboBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.SuspendLayout();
            // 
            // txtLaneName
            // 
            this.txtLaneName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLaneName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLaneName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLaneName.Location = new System.Drawing.Point(66, 26);
            this.txtLaneName.Name = "txtLaneName";
            this.txtLaneName.Size = new System.Drawing.Size(112, 21);
            this.txtLaneName.TabIndex = 3;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(19, 30);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(41, 12);
            this.darkLabel1.TabIndex = 2;
            this.darkLabel1.Text = "차선명";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(187, 30);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(29, 12);
            this.darkLabel2.TabIndex = 4;
            this.darkLabel2.Text = "차선";
            // 
            // cbLane
            // 
            this.cbLane.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLane.FormattingEnabled = true;
            this.cbLane.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbLane.Location = new System.Drawing.Point(222, 26);
            this.cbLane.Name = "cbLane";
            this.cbLane.Size = new System.Drawing.Size(92, 20);
            this.cbLane.TabIndex = 5;
            // 
            // darkButton10
            // 
            this.darkButton10.Location = new System.Drawing.Point(119, 73);
            this.darkButton10.Name = "darkButton10";
            this.darkButton10.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton10.Size = new System.Drawing.Size(127, 33);
            this.darkButton10.TabIndex = 12;
            this.darkButton10.Text = "확인";
            this.darkButton10.Click += new System.EventHandler(this.darkButton10_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(266, 73);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(127, 33);
            this.darkButton1.TabIndex = 13;
            this.darkButton1.Text = "취소";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // cbKorExLane
            // 
            this.cbKorExLane.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKorExLane.FormattingEnabled = true;
            this.cbKorExLane.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cbKorExLane.Location = new System.Drawing.Point(408, 26);
            this.cbKorExLane.Name = "cbKorExLane";
            this.cbKorExLane.Size = new System.Drawing.Size(92, 20);
            this.cbKorExLane.TabIndex = 15;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(324, 30);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(81, 12);
            this.darkLabel3.TabIndex = 14;
            this.darkLabel3.Text = "도로공사 차선";
            // 
            // AddLaneInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 118);
            this.Controls.Add(this.cbKorExLane);
            this.Controls.Add(this.darkLabel3);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.darkButton10);
            this.Controls.Add(this.cbLane);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.txtLaneName);
            this.Controls.Add(this.darkLabel1);
            this.Name = "AddLaneInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "차선 정보 설정";
            this.Activated += new System.EventHandler(this.AddLaneInfoForm_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkTextBox txtLaneName;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private System.Windows.Forms.ComboBox cbLane;
        private DarkUI.Controls.DarkButton darkButton10;
        private DarkUI.Controls.DarkButton darkButton1;
        private System.Windows.Forms.ComboBox cbKorExLane;
        private DarkUI.Controls.DarkLabel darkLabel3;
    }
}