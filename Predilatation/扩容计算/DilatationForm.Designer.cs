namespace Predilatation
{
    partial class DilatationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DilatationForm));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelDataResource = new System.Windows.Forms.Button();
            this.btnSelEstCells = new System.Windows.Forms.Button();
            this.txtDataResourcePath = new System.Windows.Forms.TextBox();
            this.txtEstCellsPath = new System.Windows.Forms.TextBox();
            this.cmbChkRule = new System.Windows.Forms.ComboBox();
            this.cmbCalType = new System.Windows.Forms.ComboBox();
            this.cmbEstType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.labTip = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(18, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "预估方式：";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btnSelDataResource);
            this.groupBox1.Controls.Add(this.btnSelEstCells);
            this.groupBox1.Controls.Add(this.txtDataResourcePath);
            this.groupBox1.Controls.Add(this.txtEstCellsPath);
            this.groupBox1.Controls.Add(this.cmbChkRule);
            this.groupBox1.Controls.Add(this.cmbCalType);
            this.groupBox1.Controls.Add(this.cmbEstType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1047, 174);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "扩容数据源选择";
            // 
            // btnSelDataResource
            // 
            this.btnSelDataResource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelDataResource.ForeColor = System.Drawing.Color.Black;
            this.btnSelDataResource.Location = new System.Drawing.Point(960, 130);
            this.btnSelDataResource.Name = "btnSelDataResource";
            this.btnSelDataResource.Size = new System.Drawing.Size(75, 28);
            this.btnSelDataResource.TabIndex = 3;
            this.btnSelDataResource.Text = "选择";
            this.btnSelDataResource.UseVisualStyleBackColor = true;
            this.btnSelDataResource.Click += new System.EventHandler(this.btnSelDataSource_Click);
            // 
            // btnSelEstCells
            // 
            this.btnSelEstCells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelEstCells.ForeColor = System.Drawing.Color.Black;
            this.btnSelEstCells.Location = new System.Drawing.Point(960, 87);
            this.btnSelEstCells.Name = "btnSelEstCells";
            this.btnSelEstCells.Size = new System.Drawing.Size(75, 28);
            this.btnSelEstCells.TabIndex = 3;
            this.btnSelEstCells.Text = "选择";
            this.btnSelEstCells.UseVisualStyleBackColor = true;
            this.btnSelEstCells.Click += new System.EventHandler(this.btnSelEstCells_Click);
            // 
            // txtDataResourcePath
            // 
            this.txtDataResourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataResourcePath.Enabled = false;
            this.txtDataResourcePath.Location = new System.Drawing.Point(106, 132);
            this.txtDataResourcePath.Name = "txtDataResourcePath";
            this.txtDataResourcePath.ReadOnly = true;
            this.txtDataResourcePath.Size = new System.Drawing.Size(837, 25);
            this.txtDataResourcePath.TabIndex = 2;
            // 
            // txtEstCellsPath
            // 
            this.txtEstCellsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEstCellsPath.Enabled = false;
            this.txtEstCellsPath.Location = new System.Drawing.Point(106, 89);
            this.txtEstCellsPath.Name = "txtEstCellsPath";
            this.txtEstCellsPath.ReadOnly = true;
            this.txtEstCellsPath.Size = new System.Drawing.Size(837, 25);
            this.txtEstCellsPath.TabIndex = 2;
            // 
            // cmbChkRule
            // 
            this.cmbChkRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChkRule.FormattingEnabled = true;
            this.cmbChkRule.Items.AddRange(new object[] {
            "无",
            "集团高负荷（2018上）",
            "集团高负荷（2018下）",
            "我司高负荷"});
            this.cmbChkRule.Location = new System.Drawing.Point(616, 40);
            this.cmbChkRule.Name = "cmbChkRule";
            this.cmbChkRule.Size = new System.Drawing.Size(207, 23);
            this.cmbChkRule.TabIndex = 1;
            this.cmbChkRule.SelectedIndexChanged += new System.EventHandler(this.cmbChkRule_SelectedIndexChanged);
            // 
            // cmbCalType
            // 
            this.cmbCalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCalType.FormattingEnabled = true;
            this.cmbCalType.Items.AddRange(new object[] {
            "无",
            "流量自忙时",
            "利用率自忙时"});
            this.cmbCalType.Location = new System.Drawing.Point(335, 40);
            this.cmbCalType.Name = "cmbCalType";
            this.cmbCalType.Size = new System.Drawing.Size(176, 23);
            this.cmbCalType.TabIndex = 1;
            this.cmbCalType.SelectedIndexChanged += new System.EventHandler(this.cmbCalType_SelectedIndexChanged);
            // 
            // cmbEstType
            // 
            this.cmbEstType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstType.FormattingEnabled = true;
            this.cmbEstType.Items.AddRange(new object[] {
            "无",
            "日常",
            "节假日",
            "给定增量"});
            this.cmbEstType.Location = new System.Drawing.Point(106, 40);
            this.cmbEstType.Name = "cmbEstType";
            this.cmbEstType.Size = new System.Drawing.Size(121, 23);
            this.cmbEstType.TabIndex = 1;
            this.cmbEstType.SelectedIndexChanged += new System.EventHandler(this.cmbEstType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(528, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "判断规则：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(247, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "计算方式：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(18, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "数据源：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(18, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "扩容小区：";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Excel2007(*.xlsx)|*.xlsx|Excel2003(*.xls)|*.xls";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCalculate.Location = new System.Drawing.Point(485, 328);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(100, 28);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "扩容计算";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // labTip
            // 
            this.labTip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labTip.BackColor = System.Drawing.Color.Transparent;
            this.labTip.ForeColor = System.Drawing.Color.White;
            this.labTip.Location = new System.Drawing.Point(12, 199);
            this.labTip.Name = "labTip";
            this.labTip.Size = new System.Drawing.Size(1047, 72);
            this.labTip.TabIndex = 0;
            this.labTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 285);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1047, 23);
            this.progressBar.TabIndex = 3;
            // 
            // DilatationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1071, 386);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labTip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DilatationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络容量扩容演算工具_V2.2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DilatationForm_FormClosing);
            this.Load += new System.EventHandler(this.DilatationForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbChkRule;
        private System.Windows.Forms.ComboBox cmbCalType;
        private System.Windows.Forms.ComboBox cmbEstType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelEstCells;
        private System.Windows.Forms.TextBox txtEstCellsPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtDataResourcePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSelDataResource;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Label labTip;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}