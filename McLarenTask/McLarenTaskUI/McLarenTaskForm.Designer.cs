namespace McLarenTaskUI
{
    partial class McLarenTaskForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(McLarenTaskForm));
            this.tbInput = new System.Windows.Forms.TextBox();
            this.lblInput = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblTop = new System.Windows.Forms.Label();
            this.numUpDownTop = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTop)).BeginInit();
            this.SuspendLayout();
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.Location = new System.Drawing.Point(71, 13);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(605, 47);
            this.tbInput.TabIndex = 1;
            this.tbInput.Text = "sqrrqabccbatudefggfedvwhijkllkjihxymnnmzpop";
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(12, 13);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(43, 17);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input:";
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Location = new System.Drawing.Point(535, 66);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(140, 32);
            this.btnFind.TabIndex = 0;
            this.btnFind.Text = "Find palindromes";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbResult
            // 
            this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResult.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbResult.Location = new System.Drawing.Point(71, 104);
            this.tbResult.MaxLength = 0;
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResult.Size = new System.Drawing.Size(605, 341);
            this.tbResult.TabIndex = 2;
            this.tbResult.WordWrap = false;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(13, 104);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(52, 17);
            this.lblResult.TabIndex = 4;
            this.lblResult.Text = "Result:";
            // 
            // lblTop
            // 
            this.lblTop.AutoSize = true;
            this.lblTop.Location = new System.Drawing.Point(15, 66);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(37, 17);
            this.lblTop.TabIndex = 5;
            this.lblTop.Text = "Top:";
            // 
            // numUpDownTop
            // 
            this.numUpDownTop.Location = new System.Drawing.Point(71, 67);
            this.numUpDownTop.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDownTop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownTop.Name = "numUpDownTop";
            this.numUpDownTop.Size = new System.Drawing.Size(120, 22);
            this.numUpDownTop.TabIndex = 6;
            this.numUpDownTop.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // McLarenTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 457);
            this.Controls.Add(this.numUpDownTop);
            this.Controls.Add(this.lblTop);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.tbInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "McLarenTaskForm";
            this.Text = "McLarenTask";
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.NumericUpDown numUpDownTop;
    }
}

