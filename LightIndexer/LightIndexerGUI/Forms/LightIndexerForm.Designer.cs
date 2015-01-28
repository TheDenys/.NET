namespace LightIndexerGUI.Forms
{
    partial class LightIndexerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LightIndexerForm));
            this.tbPath = new System.Windows.Forms.TextBox();
            this.dgvFiles = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExtension = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileNameWithoutExtensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFullNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripBottom = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelTotal = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelItems = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelWaiting = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripTop = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNewSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.markAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmarkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMarkedFromIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPathToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileNameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileNameWithoutExtensionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFullNameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItemSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.wholeWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wildCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxSlop = new System.Windows.Forms.ToolStripTextBox();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxExtensionFilter = new System.Windows.Forms.ToolStripTextBox();
            this.searchEnterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblContent = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.tbSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            this.contextMenuStripGrid.SuspendLayout();
            this.statusStripBottom.SuspendLayout();
            this.menuStripTop.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.AllowDrop = true;
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPath.Location = new System.Drawing.Point(54, 0);
            this.tbPath.Margin = new System.Windows.Forms.Padding(1);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(1076, 21);
            this.tbPath.TabIndex = 1;
            this.tbPath.WordWrap = false;
            this.tbPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbPath_DragDrop);
            this.tbPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSearchAndPath_DragEnter);
            this.tbPath.Enter += new System.EventHandler(this.tbEnter);
            // 
            // dgvFiles
            // 
            this.dgvFiles.AllowDrop = true;
            this.dgvFiles.AllowUserToAddRows = false;
            this.dgvFiles.AllowUserToDeleteRows = false;
            this.dgvFiles.AllowUserToResizeRows = false;
            this.dgvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFiles.BackgroundColor = System.Drawing.Color.White;
            this.dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.colPath,
            this.colExtension,
            this.colFileSize});
            this.dgvFiles.ContextMenuStrip = this.contextMenuStripGrid;
            this.dgvFiles.GridColor = System.Drawing.Color.White;
            this.dgvFiles.Location = new System.Drawing.Point(0, 74);
            this.dgvFiles.Name = "dgvFiles";
            this.dgvFiles.ReadOnly = true;
            this.dgvFiles.RowHeadersVisible = false;
            this.dgvFiles.Size = new System.Drawing.Size(1130, 270);
            this.dgvFiles.TabIndex = 3;
            this.dgvFiles.VirtualMode = true;
            this.dgvFiles.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFiles_CellDoubleClick);
            this.dgvFiles.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFiles_CellMouseDown);
            this.dgvFiles.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFiles_CellMouseMove);
            this.dgvFiles.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.CellValueNeeded);
            this.dgvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvFiles_DragDrop);
            this.dgvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvFiles_DragEnter);
            this.dgvFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.tbSearchAndPath_DragEnter);
            this.dgvFiles.DragLeave += new System.EventHandler(this.dgvFiles_DragLeave);
            this.dgvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFiles_KeyDown);
            this.dgvFiles.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dgvFiles_PreviewKeyDown);
            // 
            // FileName
            // 
            this.FileName.DataPropertyName = "FileName";
            this.FileName.FillWeight = 25F;
            this.FileName.HeaderText = "Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            // 
            // colPath
            // 
            this.colPath.DataPropertyName = "FullPath";
            this.colPath.FillWeight = 40F;
            this.colPath.HeaderText = "Path";
            this.colPath.Name = "colPath";
            this.colPath.ReadOnly = true;
            // 
            // colExtension
            // 
            this.colExtension.DataPropertyName = "Extension";
            this.colExtension.FillWeight = 5F;
            this.colExtension.HeaderText = "Extension";
            this.colExtension.Name = "colExtension";
            this.colExtension.ReadOnly = true;
            // 
            // colFileSize
            // 
            this.colFileSize.DataPropertyName = "FileSize";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.colFileSize.DefaultCellStyle = dataGridViewCellStyle1;
            this.colFileSize.FillWeight = 10F;
            this.colFileSize.HeaderText = "Size";
            this.colFileSize.Name = "colFileSize";
            this.colFileSize.ReadOnly = true;
            // 
            // contextMenuStripGrid
            // 
            this.contextMenuStripGrid.DropShadowEnabled = false;
            this.contextMenuStripGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openContainingFolderToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItemCopy,
            this.copyPathToolStripMenuItem,
            this.copyFileNameToolStripMenuItem,
            this.copyFileNameWithoutExtensionToolStripMenuItem,
            this.copyFullNameToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStripGrid.Name = "contextMenuStrip1";
            this.contextMenuStripGrid.ShowImageMargin = false;
            this.contextMenuStripGrid.Size = new System.Drawing.Size(215, 186);
            this.contextMenuStripGrid.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripGrid_Opening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // openContainingFolderToolStripMenuItem
            // 
            this.openContainingFolderToolStripMenuItem.Name = "openContainingFolderToolStripMenuItem";
            this.openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.openContainingFolderToolStripMenuItem.Text = "Open Path";
            this.openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // toolStripMenuItemCopy
            // 
            this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
            this.toolStripMenuItemCopy.Size = new System.Drawing.Size(214, 22);
            this.toolStripMenuItemCopy.Text = "Copy";
            this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyPathToolStripMenuItem
            // 
            this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            this.copyPathToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyPathToolStripMenuItem.Text = "Copy Path";
            this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFileNameToolStripMenuItem
            // 
            this.copyFileNameToolStripMenuItem.Name = "copyFileNameToolStripMenuItem";
            this.copyFileNameToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyFileNameToolStripMenuItem.Text = "Copy File Name";
            this.copyFileNameToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFileNameWithoutExtensionToolStripMenuItem
            // 
            this.copyFileNameWithoutExtensionToolStripMenuItem.Name = "copyFileNameWithoutExtensionToolStripMenuItem";
            this.copyFileNameWithoutExtensionToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyFileNameWithoutExtensionToolStripMenuItem.Text = "Copy File Name Without Extension";
            this.copyFileNameWithoutExtensionToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFullNameToolStripMenuItem
            // 
            this.copyFullNameToolStripMenuItem.Name = "copyFullNameToolStripMenuItem";
            this.copyFullNameToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyFullNameToolStripMenuItem.Text = "Copy Full Name";
            this.copyFullNameToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // statusStripBottom
            // 
            this.statusStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTotal,
            this.toolStripStatusLabelItems,
            this.toolStripStatusLabelTime,
            this.toolStripStatusLabelWaiting});
            this.statusStripBottom.Location = new System.Drawing.Point(0, 347);
            this.statusStripBottom.Name = "statusStripBottom";
            this.statusStripBottom.Size = new System.Drawing.Size(1130, 22);
            this.statusStripBottom.TabIndex = 3;
            this.statusStripBottom.Text = "statusStripBottom";
            // 
            // toolStripStatusLabelTotal
            // 
            this.toolStripStatusLabelTotal.Name = "toolStripStatusLabelTotal";
            this.toolStripStatusLabelTotal.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelItems
            // 
            this.toolStripStatusLabelItems.Name = "toolStripStatusLabelItems";
            this.toolStripStatusLabelItems.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelTime
            // 
            this.toolStripStatusLabelTime.Name = "toolStripStatusLabelTime";
            this.toolStripStatusLabelTime.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelWaiting
            // 
            this.toolStripStatusLabelWaiting.Name = "toolStripStatusLabelWaiting";
            this.toolStripStatusLabelWaiting.Size = new System.Drawing.Size(66, 17);
            this.toolStripStatusLabelWaiting.Text = "Searching...";
            this.toolStripStatusLabelWaiting.Visible = false;
            // 
            // menuStripTop
            // 
            this.menuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile,
            this.optionsToolStripMenuItemSearch,
            this.optionsToolStripMenuItemOptions,
            this.toolStripMenuItem1});
            this.menuStripTop.Location = new System.Drawing.Point(0, 0);
            this.menuStripTop.Name = "menuStripTop";
            this.menuStripTop.Size = new System.Drawing.Size(1130, 24);
            this.menuStripTop.TabIndex = 4;
            this.menuStripTop.Text = "menuStrip";
            // 
            // toolStripMenuItemFile
            // 
            this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNewSearch,
            this.toolStripMenuItemIndex,
            this.markAllToolStripMenuItem,
            this.unmarkAllToolStripMenuItem,
            this.deleteMarkedFromIndexToolStripMenuItem,
            this.toolStripSeparator1,
            this.openToolStripMenuItem1,
            this.openPathToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyToolStripMenuItem,
            this.copyPathToolStripMenuItem1,
            this.copyFileNameToolStripMenuItem1,
            this.copyFileNameWithoutExtensionToolStripMenuItem1,
            this.copyFullNameToolStripMenuItem1,
            this.propertiesToolStripMenuItem1});
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            this.toolStripMenuItemFile.Size = new System.Drawing.Size(35, 20);
            this.toolStripMenuItemFile.Text = "File";
            this.toolStripMenuItemFile.Click += new System.EventHandler(this.toolStripMenuItemFile_Click);
            this.toolStripMenuItemFile.VisibleChanged += new System.EventHandler(this.toolStripMenuItemFile_VisibleChanged);
            // 
            // toolStripMenuItemNewSearch
            // 
            this.toolStripMenuItemNewSearch.Name = "toolStripMenuItemNewSearch";
            this.toolStripMenuItemNewSearch.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemNewSearch.Text = "New Search (Ctrl + N)";
            this.toolStripMenuItemNewSearch.Click += new System.EventHandler(this.newSearchToolStripMenuItem_Click);
            // 
            // toolStripMenuItemIndex
            // 
            this.toolStripMenuItemIndex.Name = "toolStripMenuItemIndex";
            this.toolStripMenuItemIndex.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemIndex.Text = "Index...";
            this.toolStripMenuItemIndex.Click += new System.EventHandler(this.toolStripMenuItemIndex_Click);
            // 
            // markAllToolStripMenuItem
            // 
            this.markAllToolStripMenuItem.Name = "markAllToolStripMenuItem";
            this.markAllToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.markAllToolStripMenuItem.Text = "Mark All";
            this.markAllToolStripMenuItem.Click += new System.EventHandler(this.markAllToolStripMenuItem_Click);
            // 
            // unmarkAllToolStripMenuItem
            // 
            this.unmarkAllToolStripMenuItem.Name = "unmarkAllToolStripMenuItem";
            this.unmarkAllToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.unmarkAllToolStripMenuItem.Text = "Unmark All";
            this.unmarkAllToolStripMenuItem.Click += new System.EventHandler(this.unmarkAllToolStripMenuItem_Click);
            // 
            // deleteMarkedFromIndexToolStripMenuItem
            // 
            this.deleteMarkedFromIndexToolStripMenuItem.Name = "deleteMarkedFromIndexToolStripMenuItem";
            this.deleteMarkedFromIndexToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.deleteMarkedFromIndexToolStripMenuItem.Text = "Delete Marked From Index";
            this.deleteMarkedFromIndexToolStripMenuItem.Click += new System.EventHandler(this.deleteMarkedFromIndexToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(236, 6);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // openPathToolStripMenuItem
            // 
            this.openPathToolStripMenuItem.Name = "openPathToolStripMenuItem";
            this.openPathToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.openPathToolStripMenuItem.Text = "Open Path";
            this.openPathToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(236, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyPathToolStripMenuItem1
            // 
            this.copyPathToolStripMenuItem1.Name = "copyPathToolStripMenuItem1";
            this.copyPathToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.copyPathToolStripMenuItem1.Text = "Copy Path";
            this.copyPathToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFileNameToolStripMenuItem1
            // 
            this.copyFileNameToolStripMenuItem1.Name = "copyFileNameToolStripMenuItem1";
            this.copyFileNameToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.copyFileNameToolStripMenuItem1.Text = "Copy File Name";
            this.copyFileNameToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFileNameWithoutExtensionToolStripMenuItem1
            // 
            this.copyFileNameWithoutExtensionToolStripMenuItem1.Name = "copyFileNameWithoutExtensionToolStripMenuItem1";
            this.copyFileNameWithoutExtensionToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.copyFileNameWithoutExtensionToolStripMenuItem1.Text = "Copy File Name Without Extension";
            this.copyFileNameWithoutExtensionToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // copyFullNameToolStripMenuItem1
            // 
            this.copyFullNameToolStripMenuItem1.Name = "copyFullNameToolStripMenuItem1";
            this.copyFullNameToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.copyFullNameToolStripMenuItem1.Text = "Copy Full Name";
            this.copyFullNameToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem1
            // 
            this.propertiesToolStripMenuItem1.Name = "propertiesToolStripMenuItem1";
            this.propertiesToolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.propertiesToolStripMenuItem1.Text = "Properties";
            this.propertiesToolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItemSearch
            // 
            this.optionsToolStripMenuItemSearch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wholeWordToolStripMenuItem,
            this.wildCardToolStripMenuItem,
            this.slopToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.searchEnterToolStripMenuItem});
            this.optionsToolStripMenuItemSearch.Name = "optionsToolStripMenuItemSearch";
            this.optionsToolStripMenuItemSearch.Size = new System.Drawing.Size(52, 20);
            this.optionsToolStripMenuItemSearch.Text = "Search";
            // 
            // wholeWordToolStripMenuItem
            // 
            this.wholeWordToolStripMenuItem.Name = "wholeWordToolStripMenuItem";
            this.wholeWordToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.wholeWordToolStripMenuItem.Text = "Match Whole Word";
            this.wholeWordToolStripMenuItem.Click += new System.EventHandler(this.wholeWordToolStripMenuItem_Click);
            // 
            // wildCardToolStripMenuItem
            // 
            this.wildCardToolStripMenuItem.Name = "wildCardToolStripMenuItem";
            this.wildCardToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.wildCardToolStripMenuItem.Text = "Wildcard";
            this.wildCardToolStripMenuItem.Click += new System.EventHandler(this.wildCardToolStripMenuItem_Click);
            // 
            // slopToolStripMenuItem
            // 
            this.slopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxSlop});
            this.slopToolStripMenuItem.Name = "slopToolStripMenuItem";
            this.slopToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.slopToolStripMenuItem.Text = "Slop";
            // 
            // toolStripTextBoxSlop
            // 
            this.toolStripTextBoxSlop.AutoCompleteCustomSource.AddRange(new string[] {
            "0",
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
            "15"});
            this.toolStripTextBoxSlop.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripTextBoxSlop.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.toolStripTextBoxSlop.Name = "toolStripTextBoxSlop";
            this.toolStripTextBoxSlop.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBoxSlop.Text = "0";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extensionToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            this.filterToolStripMenuItem.Visible = false;
            // 
            // extensionToolStripMenuItem
            // 
            this.extensionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxExtensionFilter});
            this.extensionToolStripMenuItem.Name = "extensionToolStripMenuItem";
            this.extensionToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.extensionToolStripMenuItem.Text = "Extension";
            // 
            // toolStripTextBoxExtensionFilter
            // 
            this.toolStripTextBoxExtensionFilter.AutoCompleteCustomSource.AddRange(new string[] {
            "asp",
            "aspx",
            "ascx",
            "asmx",
            "cs",
            "cfg",
            "config",
            "OrionMap"});
            this.toolStripTextBoxExtensionFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripTextBoxExtensionFilter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.toolStripTextBoxExtensionFilter.Name = "toolStripTextBoxExtensionFilter";
            this.toolStripTextBoxExtensionFilter.Size = new System.Drawing.Size(100, 21);
            // 
            // searchEnterToolStripMenuItem
            // 
            this.searchEnterToolStripMenuItem.Name = "searchEnterToolStripMenuItem";
            this.searchEnterToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.searchEnterToolStripMenuItem.Text = "Search (Enter)";
            this.searchEnterToolStripMenuItem.Click += new System.EventHandler(this.searchEnterToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItemOptions
            // 
            this.optionsToolStripMenuItemOptions.Name = "optionsToolStripMenuItemOptions";
            this.optionsToolStripMenuItemOptions.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItemOptions.Text = "Options";
            this.optionsToolStripMenuItemOptions.Visible = false;
            this.optionsToolStripMenuItemOptions.Click += new System.EventHandler(this.optionsToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
            this.toolStripMenuItem1.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblContent);
            this.panel1.Controls.Add(this.lblPath);
            this.panel1.Controls.Add(this.tbSearch);
            this.panel1.Controls.Add(this.tbPath);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1130, 43);
            this.panel1.TabIndex = 5;
            // 
            // lblContent
            // 
            this.lblContent.AllowDrop = true;
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(3, 26);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(47, 13);
            this.lblContent.TabIndex = 4;
            this.lblContent.Text = "Content:";
            this.lblContent.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblContent_DragDrop);
            this.lblContent.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSearchAndPath_DragEnter);
            // 
            // lblPath
            // 
            this.lblPath.AllowDrop = true;
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(3, 4);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(32, 13);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "Path:";
            this.lblPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblPath_DragDrop);
            this.lblPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSearchAndPath_DragEnter);
            // 
            // tbSearch
            // 
            this.tbSearch.AllowDrop = true;
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSearch.Location = new System.Drawing.Point(54, 22);
            this.tbSearch.Margin = new System.Windows.Forms.Padding(1);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(1076, 21);
            this.tbSearch.TabIndex = 2;
            this.tbSearch.WordWrap = false;
            this.tbSearch.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSearch_DragDrop);
            this.tbSearch.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSearchAndPath_DragEnter);
            this.tbSearch.Enter += new System.EventHandler(this.tbEnter);
            // 
            // LightIndexerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 369);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStripBottom);
            this.Controls.Add(this.menuStripTop);
            this.Controls.Add(this.dgvFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripTop;
            this.Name = "LightIndexerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LightIndexerForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LightIndexerForm_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LightIndexerForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            this.contextMenuStripGrid.ResumeLayout(false);
            this.statusStripBottom.ResumeLayout(false);
            this.statusStripBottom.PerformLayout();
            this.menuStripTop.ResumeLayout(false);
            this.menuStripTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.DataGridView dgvFiles;
        private System.Windows.Forms.StatusStrip statusStripBottom;
        private System.Windows.Forms.MenuStrip menuStripTop;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemIndex;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTotal;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelItems;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGrid;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNewSearch;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTime;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItemSearch;
        private System.Windows.Forms.ToolStripMenuItem wholeWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItemOptions;
        private System.Windows.Forms.ToolStripMenuItem slopToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSlop;
        private System.Windows.Forms.ToolStripMenuItem wildCardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxExtensionFilter;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openContainingFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFileNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFileNameWithoutExtensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyFileNameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyFileNameWithoutExtensionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem searchEnterToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExtension;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileSize;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyFullNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFullNameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteMarkedFromIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelWaiting;
        private System.Windows.Forms.ToolStripMenuItem markAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unmarkAllToolStripMenuItem;
    }
}

