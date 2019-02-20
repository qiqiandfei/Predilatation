namespace Predilatation
{
    partial class ChooseFunForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseFunForm));
            this.BtnEstimate = new System.Windows.Forms.Button();
            this.BtnDilatation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnEstimate
            // 
            this.BtnEstimate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnEstimate.Location = new System.Drawing.Point(150, 174);
            this.BtnEstimate.Name = "BtnEstimate";
            this.BtnEstimate.Size = new System.Drawing.Size(100, 50);
            this.BtnEstimate.TabIndex = 0;
            this.BtnEstimate.Text = "扩容预估";
            this.BtnEstimate.UseVisualStyleBackColor = true;
            this.BtnEstimate.Click += new System.EventHandler(this.BtnEstimate_Click);
            // 
            // BtnDilatation
            // 
            this.BtnDilatation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnDilatation.Location = new System.Drawing.Point(369, 174);
            this.BtnDilatation.Name = "BtnDilatation";
            this.BtnDilatation.Size = new System.Drawing.Size(100, 50);
            this.BtnDilatation.TabIndex = 0;
            this.BtnDilatation.Text = "扩容计算";
            this.BtnDilatation.UseVisualStyleBackColor = true;
            this.BtnDilatation.Click += new System.EventHandler(this.BtnDilatation_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(148, 375);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Copyright © 2018 大唐移动通信设备有限公司";
            // 
            // ChooseFunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(618, 399);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnDilatation);
            this.Controls.Add(this.BtnEstimate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChooseFunForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络容量扩容演算工具_V2.2.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnEstimate;
        private System.Windows.Forms.Button BtnDilatation;
        private System.Windows.Forms.Label label1;
    }
}