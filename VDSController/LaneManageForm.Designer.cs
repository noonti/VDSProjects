namespace VDSController
{
    partial class LaneManageForm
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
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.cbLeftSortKind = new System.Windows.Forms.ComboBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtLeftLaneGroupName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkButton8 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.darkButton5 = new DarkUI.Controls.DarkButton();
            this.darkButton6 = new DarkUI.Controls.DarkButton();
            this.darkButton9 = new DarkUI.Controls.DarkButton();
            this.cbRightSortKind = new System.Windows.Forms.ComboBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.txtRightLaneGroupName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.lvRightLane = new VDSCommon.ListViewEx();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvLeftLane = new VDSCommon.ListViewEx();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.darkButton10 = new DarkUI.Controls.DarkButton();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.darkGroupBox1.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkButton8);
            this.darkGroupBox1.Controls.Add(this.darkButton3);
            this.darkGroupBox1.Controls.Add(this.darkButton2);
            this.darkGroupBox1.Controls.Add(this.darkButton1);
            this.darkGroupBox1.Controls.Add(this.lvLeftLane);
            this.darkGroupBox1.Controls.Add(this.cbLeftSortKind);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Controls.Add(this.txtLeftLaneGroupName);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(779, 207);
            this.darkGroupBox1.TabIndex = 0;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "왼쪽 방향";
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(639, 132);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(127, 26);
            this.darkButton3.TabIndex = 8;
            this.darkButton3.Tag = "2";
            this.darkButton3.Text = "삭제";
            this.darkButton3.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(639, 100);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(127, 26);
            this.darkButton2.TabIndex = 7;
            this.darkButton2.Tag = "2";
            this.darkButton2.Text = "수정";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(639, 68);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(127, 26);
            this.darkButton1.TabIndex = 6;
            this.darkButton1.Tag = "2";
            this.darkButton1.Text = "추가";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click_1);
            // 
            // cbLeftSortKind
            // 
            this.cbLeftSortKind.FormattingEnabled = true;
            this.cbLeftSortKind.Items.AddRange(new object[] {
            "오름차순",
            "내림차순"});
            this.cbLeftSortKind.Location = new System.Drawing.Point(339, 26);
            this.cbLeftSortKind.Name = "cbLeftSortKind";
            this.cbLeftSortKind.Size = new System.Drawing.Size(121, 20);
            this.cbLeftSortKind.TabIndex = 1;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(280, 29);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(53, 12);
            this.darkLabel2.TabIndex = 2;
            this.darkLabel2.Text = "정렬방식";
            // 
            // txtLeftLaneGroupName
            // 
            this.txtLeftLaneGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLeftLaneGroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLeftLaneGroupName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLeftLaneGroupName.Location = new System.Drawing.Point(86, 25);
            this.txtLeftLaneGroupName.Name = "txtLeftLaneGroupName";
            this.txtLeftLaneGroupName.Size = new System.Drawing.Size(185, 21);
            this.txtLeftLaneGroupName.TabIndex = 1;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(27, 29);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(53, 12);
            this.darkLabel1.TabIndex = 0;
            this.darkLabel1.Text = "행선지명";
            // 
            // darkButton8
            // 
            this.darkButton8.Location = new System.Drawing.Point(639, 23);
            this.darkButton8.Name = "darkButton8";
            this.darkButton8.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton8.Size = new System.Drawing.Size(127, 26);
            this.darkButton8.TabIndex = 9;
            this.darkButton8.Tag = "2";
            this.darkButton8.Text = "저장";
            this.darkButton8.Click += new System.EventHandler(this.darkButton8_Click);
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Controls.Add(this.darkButton4);
            this.darkGroupBox2.Controls.Add(this.darkButton5);
            this.darkGroupBox2.Controls.Add(this.darkButton6);
            this.darkGroupBox2.Controls.Add(this.darkButton9);
            this.darkGroupBox2.Controls.Add(this.lvRightLane);
            this.darkGroupBox2.Controls.Add(this.cbRightSortKind);
            this.darkGroupBox2.Controls.Add(this.darkLabel3);
            this.darkGroupBox2.Controls.Add(this.txtRightLaneGroupName);
            this.darkGroupBox2.Controls.Add(this.darkLabel4);
            this.darkGroupBox2.Location = new System.Drawing.Point(12, 224);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(779, 207);
            this.darkGroupBox2.TabIndex = 10;
            this.darkGroupBox2.TabStop = false;
            this.darkGroupBox2.Text = "오른쪽 방향";
            // 
            // darkButton4
            // 
            this.darkButton4.Location = new System.Drawing.Point(639, 23);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(127, 26);
            this.darkButton4.TabIndex = 9;
            this.darkButton4.Tag = "1";
            this.darkButton4.Text = "저장";
            this.darkButton4.Click += new System.EventHandler(this.darkButton4_Click);
            // 
            // darkButton5
            // 
            this.darkButton5.Location = new System.Drawing.Point(639, 132);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton5.Size = new System.Drawing.Size(127, 26);
            this.darkButton5.TabIndex = 8;
            this.darkButton5.Tag = "1";
            this.darkButton5.Text = "삭제";
            this.darkButton5.Click += new System.EventHandler(this.darkButton3_Click);
            // 
            // darkButton6
            // 
            this.darkButton6.Location = new System.Drawing.Point(639, 100);
            this.darkButton6.Name = "darkButton6";
            this.darkButton6.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton6.Size = new System.Drawing.Size(127, 26);
            this.darkButton6.TabIndex = 7;
            this.darkButton6.Tag = "1";
            this.darkButton6.Text = "수정";
            this.darkButton6.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkButton9
            // 
            this.darkButton9.Location = new System.Drawing.Point(639, 68);
            this.darkButton9.Name = "darkButton9";
            this.darkButton9.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton9.Size = new System.Drawing.Size(127, 26);
            this.darkButton9.TabIndex = 6;
            this.darkButton9.Tag = "1";
            this.darkButton9.Text = "추가";
            this.darkButton9.Click += new System.EventHandler(this.darkButton1_Click_1);
            // 
            // cbRightSortKind
            // 
            this.cbRightSortKind.FormattingEnabled = true;
            this.cbRightSortKind.Items.AddRange(new object[] {
            "오름차순",
            "내림차순"});
            this.cbRightSortKind.Location = new System.Drawing.Point(339, 26);
            this.cbRightSortKind.Name = "cbRightSortKind";
            this.cbRightSortKind.Size = new System.Drawing.Size(121, 20);
            this.cbRightSortKind.TabIndex = 1;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(280, 29);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(53, 12);
            this.darkLabel3.TabIndex = 2;
            this.darkLabel3.Text = "정렬방식";
            // 
            // txtRightLaneGroupName
            // 
            this.txtRightLaneGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtRightLaneGroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRightLaneGroupName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtRightLaneGroupName.Location = new System.Drawing.Point(86, 25);
            this.txtRightLaneGroupName.Name = "txtRightLaneGroupName";
            this.txtRightLaneGroupName.Size = new System.Drawing.Size(185, 21);
            this.txtRightLaneGroupName.TabIndex = 1;
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(27, 29);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(53, 12);
            this.darkLabel4.TabIndex = 0;
            this.darkLabel4.Text = "행선지명";
            // 
            // lvRightLane
            // 
            this.lvRightLane.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader2});
            this.lvRightLane.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvRightLane.FullRowSelect = true;
            this.lvRightLane.GridLines = true;
            this.lvRightLane.HideSelection = false;
            this.lvRightLane.Location = new System.Drawing.Point(29, 54);
            this.lvRightLane.MultiSelect = false;
            this.lvRightLane.Name = "lvRightLane";
            this.lvRightLane.Size = new System.Drawing.Size(604, 147);
            this.lvRightLane.TabIndex = 5;
            this.lvRightLane.UseCompatibleStateImageBehavior = false;
            this.lvRightLane.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "차선명";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "실차선번호";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 150;
            // 
            // lvLeftLane
            // 
            this.lvLeftLane.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader1});
            this.lvLeftLane.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvLeftLane.FullRowSelect = true;
            this.lvLeftLane.GridLines = true;
            this.lvLeftLane.HideSelection = false;
            this.lvLeftLane.Location = new System.Drawing.Point(29, 54);
            this.lvLeftLane.MultiSelect = false;
            this.lvLeftLane.Name = "lvLeftLane";
            this.lvLeftLane.Size = new System.Drawing.Size(604, 147);
            this.lvLeftLane.TabIndex = 5;
            this.lvLeftLane.UseCompatibleStateImageBehavior = false;
            this.lvLeftLane.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "차선명";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader12.Width = 150;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "실차선번호";
            this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader13.Width = 150;
            // 
            // darkButton10
            // 
            this.darkButton10.Location = new System.Drawing.Point(333, 436);
            this.darkButton10.Name = "darkButton10";
            this.darkButton10.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton10.Size = new System.Drawing.Size(127, 33);
            this.darkButton10.TabIndex = 11;
            this.darkButton10.Text = "확인";
            this.darkButton10.Click += new System.EventHandler(this.darkButton10_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "방향";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "방향";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 150;
            // 
            // LaneManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 475);
            this.Controls.Add(this.darkButton10);
            this.Controls.Add(this.darkGroupBox2);
            this.Controls.Add(this.darkGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LaneManageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "차선 관리";
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.darkGroupBox2.ResumeLayout(false);
            this.darkGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private VDSCommon.ListViewEx lvLeftLane;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ComboBox cbLeftSortKind;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtLeftLaneGroupName;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton darkButton3;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkButton darkButton8;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkButton darkButton4;
        private DarkUI.Controls.DarkButton darkButton5;
        private DarkUI.Controls.DarkButton darkButton6;
        private DarkUI.Controls.DarkButton darkButton9;
        private VDSCommon.ListViewEx lvRightLane;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ComboBox cbRightSortKind;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox txtRightLaneGroupName;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkButton darkButton10;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}