﻿namespace Predilatation
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labBefore = new System.Windows.Forms.Label();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.txtPath_before = new System.Windows.Forms.TextBox();
            this.btnSelFiles_before = new System.Windows.Forms.Button();
            this.Start = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.radFlow = new System.Windows.Forms.RadioButton();
            this.radUtilizaerate = new System.Windows.Forms.RadioButton();
            this.labTip = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.radCrtDB = new System.Windows.Forms.RadioButton();
            this.radAlreadyDB = new System.Windows.Forms.RadioButton();
            this.radPanel2 = new System.Windows.Forms.Panel();
            this.radPanel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstbox_BD = new System.Windows.Forms.ListBox();
            this.lstbox_BU = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtERAB_B = new System.Windows.Forms.TextBox();
            this.cmbERAB_B = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lstbox_SD = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lstbox_SU = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbERAB_S = new System.Windows.Forms.ComboBox();
            this.txtERAB_S = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstbox_MD = new System.Windows.Forms.ListBox();
            this.lstbox_MU = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtERAB_MS = new System.Windows.Forms.TextBox();
            this.cmbERAB_MS = new System.Windows.Forms.ComboBox();
            this.txtERAB_ME = new System.Windows.Forms.TextBox();
            this.cmbERAB_ME = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radDTMobile = new System.Windows.Forms.RadioButton();
            this.radGroupSHY = new System.Windows.Forms.RadioButton();
            this.radGroupFHY = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radGivenRate = new System.Windows.Forms.RadioButton();
            this.radHoliday = new System.Windows.Forms.RadioButton();
            this.radDaily = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radCurNet = new System.Windows.Forms.RadioButton();
            this.radAll = new System.Windows.Forms.RadioButton();
            this.radAfterN = new System.Windows.Forms.RadioButton();
            this.radBeforeN = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtPath_CurNet = new System.Windows.Forms.TextBox();
            this.txtPath_after = new System.Windows.Forms.TextBox();
            this.btnSelCurNet = new System.Windows.Forms.Button();
            this.btnSelFiles_after = new System.Windows.Forms.Button();
            this.labCurNet = new System.Windows.Forms.Label();
            this.labAfter = new System.Windows.Forms.Label();
            this.radPanel2.SuspendLayout();
            this.radPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labBefore
            // 
            this.labBefore.AutoSize = true;
            this.labBefore.Location = new System.Drawing.Point(18, 114);
            this.labBefore.Name = "labBefore";
            this.labBefore.Size = new System.Drawing.Size(90, 15);
            this.labBefore.TabIndex = 0;
            this.labBefore.Text = "前N天指标：";
            // 
            // txtPath_before
            // 
            this.txtPath_before.Enabled = false;
            this.txtPath_before.Location = new System.Drawing.Point(136, 109);
            this.txtPath_before.Name = "txtPath_before";
            this.txtPath_before.ReadOnly = true;
            this.txtPath_before.Size = new System.Drawing.Size(920, 25);
            this.txtPath_before.TabIndex = 1;
            // 
            // btnSelFiles_before
            // 
            this.btnSelFiles_before.BackColor = System.Drawing.Color.White;
            this.btnSelFiles_before.ForeColor = System.Drawing.Color.Black;
            this.btnSelFiles_before.Location = new System.Drawing.Point(1064, 106);
            this.btnSelFiles_before.Name = "btnSelFiles_before";
            this.btnSelFiles_before.Size = new System.Drawing.Size(75, 30);
            this.btnSelFiles_before.TabIndex = 2;
            this.btnSelFiles_before.Text = "选择";
            this.btnSelFiles_before.UseVisualStyleBackColor = false;
            this.btnSelFiles_before.Click += new System.EventHandler(this.btnSelFiles_before_Click);
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(554, 759);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 30);
            this.Start.TabIndex = 3;
            this.Start.Text = "开始";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(798, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "自忙时标准：";
            // 
            // radFlow
            // 
            this.radFlow.AutoSize = true;
            this.radFlow.Location = new System.Drawing.Point(9, 6);
            this.radFlow.Name = "radFlow";
            this.radFlow.Size = new System.Drawing.Size(58, 19);
            this.radFlow.TabIndex = 6;
            this.radFlow.TabStop = true;
            this.radFlow.Text = "流量";
            this.radFlow.UseVisualStyleBackColor = true;
            // 
            // radUtilizaerate
            // 
            this.radUtilizaerate.AutoSize = true;
            this.radUtilizaerate.Location = new System.Drawing.Point(91, 6);
            this.radUtilizaerate.Name = "radUtilizaerate";
            this.radUtilizaerate.Size = new System.Drawing.Size(73, 19);
            this.radUtilizaerate.TabIndex = 6;
            this.radUtilizaerate.TabStop = true;
            this.radUtilizaerate.Text = "利用率";
            this.radUtilizaerate.UseVisualStyleBackColor = true;
            // 
            // labTip
            // 
            this.labTip.BackColor = System.Drawing.Color.Transparent;
            this.labTip.ForeColor = System.Drawing.Color.White;
            this.labTip.Location = new System.Drawing.Point(12, 696);
            this.labTip.Name = "labTip";
            this.labTip.Size = new System.Drawing.Size(1158, 23);
            this.labTip.TabIndex = 4;
            this.labTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 726);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1158, 23);
            this.progressBar.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "执行方式：";
            // 
            // radCrtDB
            // 
            this.radCrtDB.AutoSize = true;
            this.radCrtDB.Location = new System.Drawing.Point(10, 6);
            this.radCrtDB.Name = "radCrtDB";
            this.radCrtDB.Size = new System.Drawing.Size(103, 19);
            this.radCrtDB.TabIndex = 9;
            this.radCrtDB.TabStop = true;
            this.radCrtDB.Text = "生成数据库";
            this.radCrtDB.UseVisualStyleBackColor = true;
            this.radCrtDB.CheckedChanged += new System.EventHandler(this.radCrtDB_CheckedChanged);
            // 
            // radAlreadyDB
            // 
            this.radAlreadyDB.AutoSize = true;
            this.radAlreadyDB.Location = new System.Drawing.Point(145, 6);
            this.radAlreadyDB.Name = "radAlreadyDB";
            this.radAlreadyDB.Size = new System.Drawing.Size(103, 19);
            this.radAlreadyDB.TabIndex = 9;
            this.radAlreadyDB.TabStop = true;
            this.radAlreadyDB.Text = "已有数据库";
            this.radAlreadyDB.UseVisualStyleBackColor = true;
            // 
            // radPanel2
            // 
            this.radPanel2.Controls.Add(this.radFlow);
            this.radPanel2.Controls.Add(this.radUtilizaerate);
            this.radPanel2.Location = new System.Drawing.Point(901, 28);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(172, 31);
            this.radPanel2.TabIndex = 10;
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radCrtDB);
            this.radPanel1.Controls.Add(this.radAlreadyDB);
            this.radPanel1.Location = new System.Drawing.Point(136, 26);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(255, 31);
            this.radPanel1.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1158, 442);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "扩容标准";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lstbox_BD);
            this.groupBox3.Controls.Add(this.lstbox_BU);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtERAB_B);
            this.groupBox3.Controls.Add(this.cmbERAB_B);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(6, 61);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1146, 120);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "大包小区";
            // 
            // lstbox_BD
            // 
            this.lstbox_BD.FormattingEnabled = true;
            this.lstbox_BD.HorizontalScrollbar = true;
            this.lstbox_BD.ItemHeight = 15;
            this.lstbox_BD.Location = new System.Drawing.Point(870, 17);
            this.lstbox_BD.Name = "lstbox_BD";
            this.lstbox_BD.Size = new System.Drawing.Size(270, 94);
            this.lstbox_BD.TabIndex = 4;
            this.lstbox_BD.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_BD_MouseClick);
            this.lstbox_BD.DoubleClick += new System.EventHandler(this.lstbox_BD_DoubleClick);
            // 
            // lstbox_BU
            // 
            this.lstbox_BU.FormattingEnabled = true;
            this.lstbox_BU.HorizontalScrollbar = true;
            this.lstbox_BU.ItemHeight = 15;
            this.lstbox_BU.Location = new System.Drawing.Point(467, 17);
            this.lstbox_BU.Name = "lstbox_BU";
            this.lstbox_BU.Size = new System.Drawing.Size(270, 94);
            this.lstbox_BU.TabIndex = 4;
            this.lstbox_BU.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_BU_MouseClick);
            this.lstbox_BU.DoubleClick += new System.EventHandler(this.lstbox_BU_DoubleClick);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(751, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 15);
            this.label10.TabIndex = 3;
            this.label10.Text = "下行标准(且)：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(348, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 15);
            this.label7.TabIndex = 3;
            this.label7.Text = "上行标准(且)：";
            // 
            // txtERAB_B
            // 
            this.txtERAB_B.Location = new System.Drawing.Point(230, 52);
            this.txtERAB_B.Name = "txtERAB_B";
            this.txtERAB_B.Size = new System.Drawing.Size(50, 25);
            this.txtERAB_B.TabIndex = 2;
            // 
            // cmbERAB_B
            // 
            this.cmbERAB_B.FormattingEnabled = true;
            this.cmbERAB_B.Items.AddRange(new object[] {
            ">=",
            ">"});
            this.cmbERAB_B.Location = new System.Drawing.Point(174, 53);
            this.cmbERAB_B.Name = "cmbERAB_B";
            this.cmbERAB_B.Size = new System.Drawing.Size(50, 23);
            this.cmbERAB_B.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "E-RAB流量(KB)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lstbox_SD);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.lstbox_SU);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.cmbERAB_S);
            this.groupBox4.Controls.Add(this.txtERAB_S);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(6, 313);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1146, 120);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "小包小区";
            // 
            // lstbox_SD
            // 
            this.lstbox_SD.FormattingEnabled = true;
            this.lstbox_SD.HorizontalScrollbar = true;
            this.lstbox_SD.ItemHeight = 15;
            this.lstbox_SD.Location = new System.Drawing.Point(870, 16);
            this.lstbox_SD.Name = "lstbox_SD";
            this.lstbox_SD.Size = new System.Drawing.Size(270, 94);
            this.lstbox_SD.TabIndex = 4;
            this.lstbox_SD.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_SD_MouseClick);
            this.lstbox_SD.DoubleClick += new System.EventHandler(this.lstbox_SD_DoubleClick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(751, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(113, 15);
            this.label12.TabIndex = 3;
            this.label12.Text = "下行标准(且)：";
            // 
            // lstbox_SU
            // 
            this.lstbox_SU.FormattingEnabled = true;
            this.lstbox_SU.HorizontalScrollbar = true;
            this.lstbox_SU.ItemHeight = 15;
            this.lstbox_SU.Location = new System.Drawing.Point(467, 16);
            this.lstbox_SU.Name = "lstbox_SU";
            this.lstbox_SU.Size = new System.Drawing.Size(270, 94);
            this.lstbox_SU.TabIndex = 4;
            this.lstbox_SU.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_SU_MouseClick);
            this.lstbox_SU.DoubleClick += new System.EventHandler(this.lstbox_SU_DoubleClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(348, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "上行标准(且)：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(59, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "E-RAB流量(KB)";
            // 
            // cmbERAB_S
            // 
            this.cmbERAB_S.FormattingEnabled = true;
            this.cmbERAB_S.Items.AddRange(new object[] {
            "<",
            "<="});
            this.cmbERAB_S.Location = new System.Drawing.Point(174, 52);
            this.cmbERAB_S.Name = "cmbERAB_S";
            this.cmbERAB_S.Size = new System.Drawing.Size(50, 23);
            this.cmbERAB_S.TabIndex = 1;
            // 
            // txtERAB_S
            // 
            this.txtERAB_S.Location = new System.Drawing.Point(230, 51);
            this.txtERAB_S.Name = "txtERAB_S";
            this.txtERAB_S.Size = new System.Drawing.Size(50, 25);
            this.txtERAB_S.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstbox_MD);
            this.groupBox2.Controls.Add(this.lstbox_MU);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtERAB_MS);
            this.groupBox2.Controls.Add(this.cmbERAB_MS);
            this.groupBox2.Controls.Add(this.txtERAB_ME);
            this.groupBox2.Controls.Add(this.cmbERAB_ME);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(6, 187);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1146, 120);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "中包小区";
            // 
            // lstbox_MD
            // 
            this.lstbox_MD.FormattingEnabled = true;
            this.lstbox_MD.HorizontalScrollbar = true;
            this.lstbox_MD.ItemHeight = 15;
            this.lstbox_MD.Location = new System.Drawing.Point(870, 17);
            this.lstbox_MD.Name = "lstbox_MD";
            this.lstbox_MD.Size = new System.Drawing.Size(270, 94);
            this.lstbox_MD.TabIndex = 4;
            this.lstbox_MD.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_MD_MouseClick);
            this.lstbox_MD.DoubleClick += new System.EventHandler(this.lstbox_MD_DoubleClick);
            // 
            // lstbox_MU
            // 
            this.lstbox_MU.FormattingEnabled = true;
            this.lstbox_MU.HorizontalScrollbar = true;
            this.lstbox_MU.ItemHeight = 15;
            this.lstbox_MU.Location = new System.Drawing.Point(467, 17);
            this.lstbox_MU.Name = "lstbox_MU";
            this.lstbox_MU.Size = new System.Drawing.Size(270, 94);
            this.lstbox_MU.TabIndex = 4;
            this.lstbox_MU.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstbox_MU_MouseClick);
            this.lstbox_MU.DoubleClick += new System.EventHandler(this.lstbox_MU_DoubleClick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(751, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 15);
            this.label11.TabIndex = 3;
            this.label11.Text = "下行标准(且)：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(348, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "上行标准(且)：";
            // 
            // txtERAB_MS
            // 
            this.txtERAB_MS.Location = new System.Drawing.Point(6, 52);
            this.txtERAB_MS.Name = "txtERAB_MS";
            this.txtERAB_MS.Size = new System.Drawing.Size(50, 25);
            this.txtERAB_MS.TabIndex = 2;
            // 
            // cmbERAB_MS
            // 
            this.cmbERAB_MS.FormattingEnabled = true;
            this.cmbERAB_MS.Items.AddRange(new object[] {
            "<=",
            "<"});
            this.cmbERAB_MS.Location = new System.Drawing.Point(62, 53);
            this.cmbERAB_MS.Name = "cmbERAB_MS";
            this.cmbERAB_MS.Size = new System.Drawing.Size(50, 23);
            this.cmbERAB_MS.TabIndex = 1;
            // 
            // txtERAB_ME
            // 
            this.txtERAB_ME.Location = new System.Drawing.Point(280, 52);
            this.txtERAB_ME.Name = "txtERAB_ME";
            this.txtERAB_ME.Size = new System.Drawing.Size(50, 25);
            this.txtERAB_ME.TabIndex = 2;
            // 
            // cmbERAB_ME
            // 
            this.cmbERAB_ME.FormattingEnabled = true;
            this.cmbERAB_ME.Items.AddRange(new object[] {
            "<",
            "<="});
            this.cmbERAB_ME.Location = new System.Drawing.Point(224, 53);
            this.cmbERAB_ME.Name = "cmbERAB_ME";
            this.cmbERAB_ME.Size = new System.Drawing.Size(50, 23);
            this.cmbERAB_ME.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(118, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "E-RAB流量(KB)";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radDTMobile);
            this.panel1.Controls.Add(this.radGroupSHY);
            this.panel1.Controls.Add(this.radGroupFHY);
            this.panel1.Location = new System.Drawing.Point(6, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1146, 35);
            this.panel1.TabIndex = 0;
            // 
            // radDTMobile
            // 
            this.radDTMobile.AutoSize = true;
            this.radDTMobile.Location = new System.Drawing.Point(548, 9);
            this.radDTMobile.Name = "radDTMobile";
            this.radDTMobile.Size = new System.Drawing.Size(133, 19);
            this.radDTMobile.TabIndex = 0;
            this.radDTMobile.TabStop = true;
            this.radDTMobile.Text = "我司高负荷标准";
            this.radDTMobile.UseVisualStyleBackColor = true;
            this.radDTMobile.CheckedChanged += new System.EventHandler(this.radDTMobile_CheckedChanged);
            // 
            // radGroupSHY
            // 
            this.radGroupSHY.AutoSize = true;
            this.radGroupSHY.Location = new System.Drawing.Point(279, 9);
            this.radGroupSHY.Name = "radGroupSHY";
            this.radGroupSHY.Size = new System.Drawing.Size(196, 19);
            this.radGroupSHY.TabIndex = 0;
            this.radGroupSHY.TabStop = true;
            this.radGroupSHY.Text = "集团高负荷标准(2018下)";
            this.radGroupSHY.UseVisualStyleBackColor = true;
            this.radGroupSHY.CheckedChanged += new System.EventHandler(this.radGroupSHY_CheckedChanged);
            // 
            // radGroupFHY
            // 
            this.radGroupFHY.AutoSize = true;
            this.radGroupFHY.Location = new System.Drawing.Point(10, 9);
            this.radGroupFHY.Name = "radGroupFHY";
            this.radGroupFHY.Size = new System.Drawing.Size(196, 19);
            this.radGroupFHY.TabIndex = 0;
            this.radGroupFHY.TabStop = true;
            this.radGroupFHY.Text = "集团高负荷标准(2018上)";
            this.radGroupFHY.UseVisualStyleBackColor = true;
            this.radGroupFHY.CheckedChanged += new System.EventHandler(this.radGroupFHY_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.panel3);
            this.groupBox5.Controls.Add(this.panel2);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.radPanel1);
            this.groupBox5.Controls.Add(this.radPanel2);
            this.groupBox5.Controls.Add(this.txtPath_CurNet);
            this.groupBox5.Controls.Add(this.txtPath_after);
            this.groupBox5.Controls.Add(this.txtPath_before);
            this.groupBox5.Controls.Add(this.btnSelCurNet);
            this.groupBox5.Controls.Add(this.btnSelFiles_after);
            this.groupBox5.Controls.Add(this.btnSelFiles_before);
            this.groupBox5.Controls.Add(this.labCurNet);
            this.groupBox5.Controls.Add(this.labAfter);
            this.groupBox5.Controls.Add(this.labBefore);
            this.groupBox5.ForeColor = System.Drawing.Color.White;
            this.groupBox5.Location = new System.Drawing.Point(12, 460);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1158, 228);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "执行方式";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.radGivenRate);
            this.panel3.Controls.Add(this.radHoliday);
            this.panel3.Controls.Add(this.radDaily);
            this.panel3.Location = new System.Drawing.Point(136, 66);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(877, 31);
            this.panel3.TabIndex = 12;
            // 
            // radGivenRate
            // 
            this.radGivenRate.AutoSize = true;
            this.radGivenRate.Location = new System.Drawing.Point(613, 9);
            this.radGivenRate.Name = "radGivenRate";
            this.radGivenRate.Size = new System.Drawing.Size(133, 19);
            this.radGivenRate.TabIndex = 0;
            this.radGivenRate.TabStop = true;
            this.radGivenRate.Text = "给定增长率预估";
            this.radGivenRate.UseVisualStyleBackColor = true;
            // 
            // radHoliday
            // 
            this.radHoliday.AutoSize = true;
            this.radHoliday.Location = new System.Drawing.Point(319, 6);
            this.radHoliday.Name = "radHoliday";
            this.radHoliday.Size = new System.Drawing.Size(163, 19);
            this.radHoliday.TabIndex = 0;
            this.radHoliday.TabStop = true;
            this.radHoliday.Text = "节假日保障容量预估";
            this.radHoliday.UseVisualStyleBackColor = true;
            this.radHoliday.CheckedChanged += new System.EventHandler(this.radHoliday_CheckedChanged);
            // 
            // radDaily
            // 
            this.radDaily.AutoSize = true;
            this.radDaily.Location = new System.Drawing.Point(10, 6);
            this.radDaily.Name = "radDaily";
            this.radDaily.Size = new System.Drawing.Size(178, 19);
            this.radDaily.TabIndex = 0;
            this.radDaily.TabStop = true;
            this.radDaily.Text = "日常小区容量增长预估";
            this.radDaily.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.radCurNet);
            this.panel2.Controls.Add(this.radAll);
            this.panel2.Controls.Add(this.radAfterN);
            this.panel2.Controls.Add(this.radBeforeN);
            this.panel2.Location = new System.Drawing.Point(495, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(297, 31);
            this.panel2.TabIndex = 11;
            // 
            // radCurNet
            // 
            this.radCurNet.AutoSize = true;
            this.radCurNet.Location = new System.Drawing.Point(160, 6);
            this.radCurNet.Name = "radCurNet";
            this.radCurNet.Size = new System.Drawing.Size(58, 19);
            this.radCurNet.TabIndex = 1;
            this.radCurNet.TabStop = true;
            this.radCurNet.Text = "现网";
            this.radCurNet.UseVisualStyleBackColor = true;
            // 
            // radAll
            // 
            this.radAll.AutoSize = true;
            this.radAll.Location = new System.Drawing.Point(230, 6);
            this.radAll.Name = "radAll";
            this.radAll.Size = new System.Drawing.Size(58, 19);
            this.radAll.TabIndex = 0;
            this.radAll.TabStop = true;
            this.radAll.Text = "全部";
            this.radAll.UseVisualStyleBackColor = true;
            // 
            // radAfterN
            // 
            this.radAfterN.AutoSize = true;
            this.radAfterN.Location = new System.Drawing.Point(82, 6);
            this.radAfterN.Name = "radAfterN";
            this.radAfterN.Size = new System.Drawing.Size(66, 19);
            this.radAfterN.TabIndex = 0;
            this.radAfterN.TabStop = true;
            this.radAfterN.Text = "后N天";
            this.radAfterN.UseVisualStyleBackColor = true;
            // 
            // radBeforeN
            // 
            this.radBeforeN.AutoSize = true;
            this.radBeforeN.Location = new System.Drawing.Point(4, 6);
            this.radBeforeN.Name = "radBeforeN";
            this.radBeforeN.Size = new System.Drawing.Size(66, 19);
            this.radBeforeN.TabIndex = 0;
            this.radBeforeN.TabStop = true;
            this.radBeforeN.Text = "前N天";
            this.radBeforeN.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(82, 15);
            this.label15.TabIndex = 8;
            this.label15.Text = "预估方式：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(411, 34);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 15);
            this.label14.TabIndex = 5;
            this.label14.Text = "计算范围：";
            // 
            // txtPath_CurNet
            // 
            this.txtPath_CurNet.Location = new System.Drawing.Point(136, 189);
            this.txtPath_CurNet.Name = "txtPath_CurNet";
            this.txtPath_CurNet.ReadOnly = true;
            this.txtPath_CurNet.Size = new System.Drawing.Size(920, 25);
            this.txtPath_CurNet.TabIndex = 1;
            // 
            // txtPath_after
            // 
            this.txtPath_after.Enabled = false;
            this.txtPath_after.Location = new System.Drawing.Point(136, 149);
            this.txtPath_after.Name = "txtPath_after";
            this.txtPath_after.ReadOnly = true;
            this.txtPath_after.Size = new System.Drawing.Size(920, 25);
            this.txtPath_after.TabIndex = 1;
            // 
            // btnSelCurNet
            // 
            this.btnSelCurNet.BackColor = System.Drawing.Color.White;
            this.btnSelCurNet.ForeColor = System.Drawing.Color.Black;
            this.btnSelCurNet.Location = new System.Drawing.Point(1064, 186);
            this.btnSelCurNet.Name = "btnSelCurNet";
            this.btnSelCurNet.Size = new System.Drawing.Size(75, 30);
            this.btnSelCurNet.TabIndex = 2;
            this.btnSelCurNet.Text = "选择";
            this.btnSelCurNet.UseVisualStyleBackColor = false;
            this.btnSelCurNet.Click += new System.EventHandler(this.btnSelCurNet_Click);
            // 
            // btnSelFiles_after
            // 
            this.btnSelFiles_after.BackColor = System.Drawing.Color.White;
            this.btnSelFiles_after.ForeColor = System.Drawing.Color.Black;
            this.btnSelFiles_after.Location = new System.Drawing.Point(1064, 146);
            this.btnSelFiles_after.Name = "btnSelFiles_after";
            this.btnSelFiles_after.Size = new System.Drawing.Size(75, 30);
            this.btnSelFiles_after.TabIndex = 2;
            this.btnSelFiles_after.Text = "选择";
            this.btnSelFiles_after.UseVisualStyleBackColor = false;
            this.btnSelFiles_after.Click += new System.EventHandler(this.btnSelFiles_after_Click);
            // 
            // labCurNet
            // 
            this.labCurNet.AutoSize = true;
            this.labCurNet.Location = new System.Drawing.Point(18, 194);
            this.labCurNet.Name = "labCurNet";
            this.labCurNet.Size = new System.Drawing.Size(84, 15);
            this.labCurNet.TabIndex = 0;
            this.labCurNet.Text = "现网7*24：";
            // 
            // labAfter
            // 
            this.labAfter.AutoSize = true;
            this.labAfter.Location = new System.Drawing.Point(18, 154);
            this.labAfter.Name = "labAfter";
            this.labAfter.Size = new System.Drawing.Size(90, 15);
            this.labAfter.TabIndex = 0;
            this.labAfter.Text = "后N天指标：";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1182, 798);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labTip);
            this.Controls.Add(this.Start);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "预扩容工具_Demo";
            this.radPanel2.ResumeLayout(false);
            this.radPanel2.PerformLayout();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labBefore;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.TextBox txtPath_before;
        private System.Windows.Forms.Button btnSelFiles_before;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radFlow;
        private System.Windows.Forms.RadioButton radUtilizaerate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radCrtDB;
        private System.Windows.Forms.RadioButton radAlreadyDB;
        private System.Windows.Forms.Panel radPanel2;
        private System.Windows.Forms.Panel radPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.RadioButton radDTMobile;
        public System.Windows.Forms.RadioButton radGroupSHY;
        public System.Windows.Forms.RadioButton radGroupFHY;
        public System.Windows.Forms.TextBox txtERAB_B;
        public System.Windows.Forms.ComboBox cmbERAB_B;
        public System.Windows.Forms.ComboBox cmbERAB_S;
        public System.Windows.Forms.TextBox txtERAB_S;
        public System.Windows.Forms.TextBox txtERAB_MS;
        public System.Windows.Forms.ComboBox cmbERAB_MS;
        public System.Windows.Forms.TextBox txtERAB_ME;
        public System.Windows.Forms.ComboBox cmbERAB_ME;
        public System.Windows.Forms.ListBox lstbox_BD;
        public System.Windows.Forms.ListBox lstbox_SD;
        public System.Windows.Forms.ListBox lstbox_SU;
        public System.Windows.Forms.ListBox lstbox_MD;
        public System.Windows.Forms.ListBox lstbox_MU;
        public System.Windows.Forms.ListBox lstbox_BU;
        public System.Windows.Forms.ProgressBar progressBar;
        public System.Windows.Forms.Label labTip;
        private System.Windows.Forms.TextBox txtPath_after;
        private System.Windows.Forms.Button btnSelFiles_after;
        private System.Windows.Forms.Label labAfter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radAll;
        private System.Windows.Forms.RadioButton radAfterN;
        private System.Windows.Forms.RadioButton radBeforeN;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radGivenRate;
        private System.Windows.Forms.RadioButton radHoliday;
        private System.Windows.Forms.RadioButton radDaily;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtPath_CurNet;
        private System.Windows.Forms.Button btnSelCurNet;
        private System.Windows.Forms.Label labCurNet;
        private System.Windows.Forms.RadioButton radCurNet;
    }
}

