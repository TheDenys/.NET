namespace LightIndexerGUI.Forms
{
    partial class Tree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tree));
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.treeStatusStrip = new System.Windows.Forms.StatusStrip();
            this.panButton = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnIndex = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panButton.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFolders
            // 
            this.tvFolders.CheckBoxes = true;
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.Size = new System.Drawing.Size(347, 238);
            this.tvFolders.TabIndex = 0;
            this.tvFolders.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterCheck);
            this.tvFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvFolders_BeforeExpand);
            // 
            // treeStatusStrip
            // 
            this.treeStatusStrip.Location = new System.Drawing.Point(0, 276);
            this.treeStatusStrip.Name = "treeStatusStrip";
            this.treeStatusStrip.Size = new System.Drawing.Size(347, 22);
            this.treeStatusStrip.TabIndex = 1;
            this.treeStatusStrip.Text = "statusStrip1";
            // 
            // panButton
            // 
            this.panButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panButton.Controls.Add(this.btnDelete);
            this.panButton.Controls.Add(this.btnIndex);
            this.panButton.Location = new System.Drawing.Point(0, 245);
            this.panButton.Name = "panButton";
            this.panButton.Size = new System.Drawing.Size(347, 28);
            this.panButton.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDelete.Location = new System.Drawing.Point(197, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnIndex
            // 
            this.btnIndex.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnIndex.Location = new System.Drawing.Point(272, 0);
            this.btnIndex.Name = "btnIndex";
            this.btnIndex.Size = new System.Drawing.Size(75, 28);
            this.btnIndex.TabIndex = 2;
            this.btnIndex.Text = "Index";
            this.btnIndex.UseVisualStyleBackColor = true;
            this.btnIndex.Click += new System.EventHandler(this.btnIndex_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.tvFolders);
            this.panel2.Location = new System.Drawing.Point(0, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(347, 238);
            this.panel2.TabIndex = 3;
            // 
            // Tree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 298);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panButton);
            this.Controls.Add(this.treeStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Tree";
            this.Text = "Tree";
            this.panButton.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.StatusStrip treeStatusStrip;
        private System.Windows.Forms.Panel panButton;
        private System.Windows.Forms.Button btnIndex;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnDelete;
    }
}