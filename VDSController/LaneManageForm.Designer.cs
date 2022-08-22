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
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.txtLaneGroupName = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.cbLeftSortKind = new System.Windows.Forms.ComboBox();
            this.lvTrafficStat = new VDSCommon.ListViewEx();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkButton3 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.darkButton5 = new DarkUI.Controls.DarkButton();
            this.darkButton6 = new DarkUI.Controls.DarkButton();
            this.listViewEx1 = new VDSCommon.ListViewEx();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.darkTextBox1 = new DarkUI.Controls.DarkTextBox();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.darkButton7 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox1.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.darkButton3);
            this.darkGroupBox1.Controls.Add(this.darkButton2);
            this.darkGroupBox1.Controls.Add(this.darkButton1);
            this.darkGroupBox1.Controls.Add(this.lvTrafficStat);
            this.darkGroupBox1.Controls.Add(this.cbLeftSortKind);
            this.darkGroupBox1.Controls.Add(this.darkLabel2);
            this.darkGroupBox1.Controls.Add(this.txtLaneGroupName);
            this.darkGroupBox1.Controls.Add(this.darkLabel1);
            this.darkGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(779, 207);
            this.darkGroupBox1.TabIndex = 0;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "왼쪽 방향";
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
            // txtLaneGroupName
            // 
            this.txtLaneGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtLaneGroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLaneGroupName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLaneGroupName.Location = new System.Drawing.Point(86, 25);
            this.txtLaneGroupName.Name = "txtLaneGroupName";
            this.txtLaneGroupName.Size = new System.Drawing.Size(185, 21);
            this.txtLaneGroupName.TabIndex = 1;
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
            // lvTrafficStat
            // 
            this.lvTrafficStat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader1});
            this.lvTrafficStat.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvTrafficStat.FullRowSelect = true;
            this.lvTrafficStat.GridLines = true;
            this.lvTrafficStat.HideSelection = false;
            this.lvTrafficStat.Location = new System.Drawing.Point(29, 52);
            this.lvTrafficStat.MultiSelect = false;
            this.lvTrafficStat.Name = "lvTrafficStat";
            this.lvTrafficStat.Size = new System.Drawing.Size(604, 149);
            this.lvTrafficStat.TabIndex = 5;
            this.lvTrafficStat.UseCompatibleStateImageBehavior = false;
            this.lvTrafficStat.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "행선지명";
            this.columnHeader11.Width = 150;
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
            // columnHeader1
            // 
            this.columnHeader1.Text = "방향";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 150;
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(639, 52);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(127, 33);
            this.darkButton1.TabIndex = 6;
            this.darkButton1.Text = "추가";
            // 
            // darkButton2
            // 
            this.darkButton2.Location = new System.Drawing.Point(639, 110);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(127, 33);
            this.darkButton2.TabIndex = 7;
            this.darkButton2.Text = "수정";
            // 
            // darkButton3
            // 
            this.darkButton3.Location = new System.Drawing.Point(639, 168);
            this.darkButton3.Name = "darkButton3";
            this.darkButton3.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton3.Size = new System.Drawing.Size(127, 33);
            this.darkButton3.TabIndex = 8;
            this.darkButton3.Text = "삭제";
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox2.Controls.Add(this.darkButton4);
            this.darkGroupBox2.Controls.Add(this.darkButton5);
            this.darkGroupBox2.Controls.Add(this.darkButton6);
            this.darkGroupBox2.Controls.Add(this.listViewEx1);
            this.darkGroupBox2.Controls.Add(this.comboBox1);
            this.darkGroupBox2.Controls.Add(this.darkLabel3);
            this.darkGroupBox2.Controls.Add(this.darkTextBox1);
            this.darkGroupBox2.Controls.Add(this.darkLabel4);
            this.darkGroupBox2.Location = new System.Drawing.Point(12, 225);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(779, 207);
            this.darkGroupBox2.TabIndex = 9;
            this.darkGroupBox2.TabStop = false;
            this.darkGroupBox2.Text = "오른쪽 방향";
            // 
            // darkButton4
            // 
            this.darkButton4.Location = new System.Drawing.Point(639, 168);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(127, 33);
            this.darkButton4.TabIndex = 8;
            this.darkButton4.Text = "삭제";
            // 
            // darkButton5
            // 
            this.darkButton5.Location = new System.Drawing.Point(639, 110);
            this.darkButton5.Name = "darkButton5";
            this.darkButton5.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton5.Size = new System.Drawing.Size(127, 33);
            this.darkButton5.TabIndex = 7;
            this.darkButton5.Text = "수정";
            // 
            // darkButton6
            // 
            this.darkButton6.Location = new System.Drawing.Point(639, 52);
            this.darkButton6.Name = "darkButton6";
            this.darkButton6.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton6.Size = new System.Drawing.Size(127, 33);
            this.darkButton6.TabIndex = 6;
            this.darkButton6.Text = "추가";
            // 
            // listViewEx1
            // 
            this.listViewEx1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewEx1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewEx1.FullRowSelect = true;
            this.listViewEx1.GridLines = true;
            this.listViewEx1.HideSelection = false;
            this.listViewEx1.Location = new System.Drawing.Point(29, 52);
            this.listViewEx1.MultiSelect = false;
            this.listViewEx1.Name = "listViewEx1";
            this.listViewEx1.Size = new System.Drawing.Size(604, 149);
            this.listViewEx1.TabIndex = 5;
            this.listViewEx1.UseCompatibleStateImageBehavior = false;
            this.listViewEx1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "행선지명";
            this.columnHeader2.Width = 150;
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
            // columnHeader5
            // 
            this.columnHeader5.Text = "방향";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 150;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "오름차순",
            "내림차순"});
            this.comboBox1.Location = new System.Drawing.Point(339, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 1;
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
            // darkTextBox1
            // 
            this.darkTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.darkTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkTextBox1.Location = new System.Drawing.Point(86, 25);
            this.darkTextBox1.Name = "darkTextBox1";
            this.darkTextBox1.Size = new System.Drawing.Size(185, 21);
            this.darkTextBox1.TabIndex = 1;
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
            // darkButton7
            // 
            this.darkButton7.Location = new System.Drawing.Point(335, 436);
            this.darkButton7.Name = "darkButton7";
            this.darkButton7.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton7.Size = new System.Drawing.Size(127, 33);
            this.darkButton7.TabIndex = 9;
            this.darkButton7.Text = "닫기";
            // 
            // LaneManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 475);
            this.Controls.Add(this.darkButton7);
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
        private VDSCommon.ListViewEx lvTrafficStat;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ComboBox cbLeftSortKind;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtLaneGroupName;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton darkButton3;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkButton darkButton4;
        private DarkUI.Controls.DarkButton darkButton5;
        private DarkUI.Controls.DarkButton darkButton6;
        private VDSCommon.ListViewEx listViewEx1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ComboBox comboBox1;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox darkTextBox1;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkButton darkButton7;
    }
}