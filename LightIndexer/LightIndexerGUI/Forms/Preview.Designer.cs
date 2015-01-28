namespace LightIndexerGUI.Forms
{
    partial class Preview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
            this.statusStripBottom = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelCursor = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripTop = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFind = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStripPreview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.searchInIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchInPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textEditorControl = new ICSharpCode.TextEditor.TextEditorControl();
            this.statusStripBottom.SuspendLayout();
            this.menuStripTop.SuspendLayout();
            this.toolStripTop.SuspendLayout();
            this.contextMenuStripPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripBottom
            // 
            this.statusStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelCursor});
            this.statusStripBottom.Location = new System.Drawing.Point(0, 400);
            this.statusStripBottom.Name = "statusStripBottom";
            this.statusStripBottom.Size = new System.Drawing.Size(886, 22);
            this.statusStripBottom.TabIndex = 0;
            this.statusStripBottom.Text = "statusStrip1";
            // 
            // toolStripStatusLabelCursor
            // 
            this.toolStripStatusLabelCursor.Name = "toolStripStatusLabelCursor";
            this.toolStripStatusLabelCursor.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStripTop
            // 
            this.menuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStripTop.Location = new System.Drawing.Point(0, 0);
            this.menuStripTop.Name = "menuStripTop";
            this.menuStripTop.Size = new System.Drawing.Size(886, 24);
            this.menuStripTop.TabIndex = 1;
            this.menuStripTop.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripTop
            // 
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrevious,
            this.toolStripButtonNext,
            this.toolStripButtonFind});
            this.toolStripTop.Location = new System.Drawing.Point(0, 24);
            this.toolStripTop.Name = "toolStripTop";
            this.toolStripTop.Size = new System.Drawing.Size(886, 25);
            this.toolStripTop.TabIndex = 2;
            this.toolStripTop.Text = "toolStrip1";
            // 
            // toolStripButtonPrevious
            // 
            this.toolStripButtonPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrevious.Image = global::LightIndexerGUI.Properties.Resources.arrowLeft;
            this.toolStripButtonPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevious.Name = "toolStripButtonPrevious";
            this.toolStripButtonPrevious.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrevious.Text = "toolStripButton1";
            this.toolStripButtonPrevious.ToolTipText = "Previous (Shift+F3)";
            this.toolStripButtonPrevious.Click += new System.EventHandler(this.toolStripButtonPrevious_Click);
            // 
            // toolStripButtonNext
            // 
            this.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNext.Image = global::LightIndexerGUI.Properties.Resources.arrowRight;
            this.toolStripButtonNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNext.Name = "toolStripButtonNext";
            this.toolStripButtonNext.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNext.Text = "toolStripButton2";
            this.toolStripButtonNext.ToolTipText = "Next (F3)";
            this.toolStripButtonNext.Click += new System.EventHandler(this.toolStripButtonNext_Click);
            // 
            // toolStripButtonFind
            // 
            this.toolStripButtonFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFind.Image = global::LightIndexerGUI.Properties.Resources.find;
            this.toolStripButtonFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFind.Name = "toolStripButtonFind";
            this.toolStripButtonFind.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFind.Text = "Find (Ctrl+F)";
            this.toolStripButtonFind.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // contextMenuStripPreview
            // 
            this.contextMenuStripPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchInIndexToolStripMenuItem,
            this.searchInPathToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.contextMenuStripPreview.Name = "contextMenuStripPreview";
            this.contextMenuStripPreview.Size = new System.Drawing.Size(154, 70);
            // 
            // searchInIndexToolStripMenuItem
            // 
            this.searchInIndexToolStripMenuItem.Name = "searchInIndexToolStripMenuItem";
            this.searchInIndexToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.searchInIndexToolStripMenuItem.Text = "Search In Index";
            this.searchInIndexToolStripMenuItem.Click += new System.EventHandler(this.searchInIndexToolStripMenuItem_Click);
            // 
            // searchInPathToolStripMenuItem
            // 
            this.searchInPathToolStripMenuItem.Name = "searchInPathToolStripMenuItem";
            this.searchInPathToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.searchInPathToolStripMenuItem.Text = "Search In Path";
            this.searchInPathToolStripMenuItem.Click += new System.EventHandler(this.searchInPathToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // textEditorControl
            // 
            this.textEditorControl.ContextMenuStrip = this.contextMenuStripPreview;
            this.textEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorControl.IsReadOnly = false;
            this.textEditorControl.Location = new System.Drawing.Point(0, 49);
            this.textEditorControl.Name = "textEditorControl";
            this.textEditorControl.Size = new System.Drawing.Size(886, 351);
            this.textEditorControl.TabIndex = 4;
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 422);
            this.Controls.Add(this.textEditorControl);
            this.Controls.Add(this.toolStripTop);
            this.Controls.Add(this.statusStripBottom);
            this.Controls.Add(this.menuStripTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripTop;
            this.Name = "Preview";
            this.Text = "Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Preview_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPreview_KeyDown);
            this.statusStripBottom.ResumeLayout(false);
            this.statusStripBottom.PerformLayout();
            this.menuStripTop.ResumeLayout(false);
            this.menuStripTop.PerformLayout();
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.contextMenuStripPreview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripBottom;
        private System.Windows.Forms.MenuStrip menuStripTop;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevious;
        private System.Windows.Forms.ToolStripButton toolStripButtonNext;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCursor;
        private System.Windows.Forms.ToolStripButton toolStripButtonFind;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPreview;
        private System.Windows.Forms.ToolStripMenuItem searchInIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchInPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private ICSharpCode.TextEditor.TextEditorControl textEditorControl;
    }
}