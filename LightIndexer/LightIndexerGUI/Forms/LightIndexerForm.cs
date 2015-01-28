using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Forms;
using LightIndexer.Indexing;
using LightIndexerGUI.Classes;
using LightIndexerGUI.Classes.Help;
using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Presenters;
using LightIndexerGUI.Classes.Views;
using log4net;
using FIF = LightIndexer.Indexing.FileIndexingFields;
using System.Linq;

namespace LightIndexerGUI.Forms
{

    internal sealed partial class LightIndexerForm : Form, ILightIndexerView
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Font BasicFont, VirtualFont;

        LightIndexerModel.FormState ILightIndexerView.CurrentFormState { get; set; }

        IWaitView waitForm;

        LightIndexerPresenter presenter;

        Point dragStartPoint;

        Color _nonSelectedForeColor;

        Color _nonSelectedForeColorArchive;

        Color _selectedForeColor;

        const int dragMinDelta = 25;

        // explicitly implemented methods are accessible only via ILightIndexerView
        // so to avoid writing ((this) as ILightIndexerView).Memeber everywhere i defined _this
        ILightIndexerView _this;

        internal LightIndexerForm()
            : this(null)
        {
        }

        internal LightIndexerForm(SearchOptions searchOptions)
        {
            _this = ((this) as ILightIndexerView);
            this.presenter = new LightIndexerPresenter(this);
            LightIndexerApplicationContext.AddForm(this);
            InitializeComponent();
            InitGui();
            presenter.Init(searchOptions);
            _this.UpdateFileToolstripMenu();
        }

        private void InitGui()
        {
            BasicFont = dgvFiles.Font;
            VirtualFont = new Font(BasicFont.FontFamily, BasicFont.Size, FontStyle.Italic);
            _this.SetStatusLabels(new LightIndexerPresenter.SearchStatusContainer(-1, -1, false));
            _this.CurrentFormState = LightIndexerModel.FormState.Ready;

            //datagrid colors
            _nonSelectedForeColor = dgvFiles.AlternatingRowsDefaultCellStyle.ForeColor;
            _nonSelectedForeColorArchive = Color.Green;
            _selectedForeColor = Color.Red;

            //reactive init
            var observablePathKeyDowns = Observable.FromEventPattern(tbPath, "KeyDown");
            observablePathKeyDowns.Subscribe(e => tbKeyDownHandler(e.Sender, e.EventArgs as KeyEventArgs));

            var observableContentKeyDowns = Observable.FromEventPattern(tbSearch, "KeyDown");
            observableContentKeyDowns.Subscribe(e => tbKeyDownHandler(e.Sender, e.EventArgs as KeyEventArgs));

            waitForm = new WaitForm();
        }

        protected override void OnActivated(EventArgs e)
        {
            isActive = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            isActive = false;
            LightIndexerApplicationContext.SetLastForm(this);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            presenter.OnShown();
        }

        void ILightIndexerView.SetStatusLabels(LightIndexerPresenter.SearchStatusContainer container)
        {
            toolStripStatusLabelWaiting.Visible = container.Waiting;

            if (container.Waiting)
            {
                toolStripStatusLabelTotal.Text = string.Empty;
                toolStripStatusLabelItems.Text = string.Empty;
                toolStripStatusLabelTime.Text = string.Empty;
            }
            else
            {
                toolStripStatusLabelTotal.Text = string.Format("Total: {0}", IndexingFacade.Count);

                if (container.RowCount != -1)
                {
                    toolStripStatusLabelItems.Text = string.Format("Items: {0}", container.RowCount);
                }

                if (container.ElapsedMilliseconds != -1)
                {
                    toolStripStatusLabelTime.Text = string.Format("Time: {0}ms", container.ElapsedMilliseconds);
                }
            }
        }

        private void CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            var propName = (sender as DataGridView).Columns[columnIndex].DataPropertyName;

            object value;
            Font f;
            bool isPacked;
            var hasValue = presenter.GetValue(rowIndex, propName, BasicFont, VirtualFont, out value, out f, out isPacked);

            if (hasValue)
            {
                e.Value = value;
                dgvFiles.Rows[rowIndex].Cells[columnIndex].Style.Font = f;
                dgvFiles[columnIndex, rowIndex].Style.ForeColor = presenter.IsMarked(rowIndex) ? _selectedForeColor : isPacked ? _nonSelectedForeColorArchive : _nonSelectedForeColor;
            }
        }

        private void tbKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (_this.CurrentFormState != LightIndexerModel.FormState.Ready)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    presenter.Search();
                    break;
                case Keys.Down:
                    e.Handled = true;
                    if (dgvFiles.Rows.Count > 0)
                    {
                        dgvFiles.Select();
                    }
                    break;
            }
        }

        private void tbEnter(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void dgvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (!e.Control)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        tbPath.Focus();
                    }
                    break;
                case Keys.Enter:
                    {
                        if (e.Alt)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                        }
                        else
                        {
                            e.Handled = true;
                            ShowPreview();
                        }
                    }
                    break;
                case Keys.Space:
                    e.Handled = true;
                    presenter.MarkCurrent();
                    break;
                case Keys.Insert:
                    e.Handled = true;
                    presenter.MarkCurrentAndGoToNext();
                    break;
                case Keys.F3:
                    e.Handled = true;
                    ShowPreview();
                    break;
                case Keys.F4:
                    if (!e.Alt && !e.Control && !e.Shift)
                    {
                        e.Handled = true;
                        Edit();
                    }
                    break;
            }
        }

        private void dgvFiles_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (e.Control)
                    {
                        LightIndexerApplicationContext.FocusNextForm(this, !e.Shift, false);
                    }
                    break;
                case Keys.Enter:
                    if (e.Alt)
                    {
                        var docId = dgvFiles.CurrentCell.RowIndex;
                        var handler = new MenuActionHandler(presenter.dr, docId);
                        handler.HandleAction(MenuActions.Properties);
                    }
                    break;
            }
        }

        int ILightIndexerView.GetCurrentId()
        {
            var docId = dgvFiles.CurrentCell.RowIndex;
            return docId;
        }

        void ILightIndexerView.Mark(int id, bool mark)
        {
            dgvFiles[0, id].Style.ForeColor = mark ? _selectedForeColor : presenter.IsPacked(id) ? _nonSelectedForeColorArchive : _nonSelectedForeColor;
        }

        void ILightIndexerView.SetActiveCell(int id)
        {
            if (id < dgvFiles.Rows.Count)
            {
                dgvFiles.CurrentCell = dgvFiles[0, id];
            }
        }

        internal void ShowPreview()
        {
            if (dgvFiles.CurrentCell != null)
            {
                var docId = dgvFiles.CurrentCell.RowIndex;
                var preview = new Preview(presenter.dr, new PreviewOptions { DocId = docId, SearchOptions = presenter.SearchOptions });
                preview.ShowPreview();
            }
        }

        internal void Edit()
        {
            if (dgvFiles.CurrentCell != null)
            {
                var docId = dgvFiles.CurrentCell.RowIndex;

                if (presenter.IsPacked(docId))
                {
                    MessageBox.Show("Can't edit files in archive.");
                    return;
                }

                var path = IndexingFacade.GetFieldValue(presenter.dr, docId, FileIndexingFields.FullName);
                StartEditor.Start(path);
            }
        }

        private void toolStripMenuItemIndex_Click(object sender, EventArgs e)
        {
            new Tree().ShowDialog();
        }

        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewSearch();
        }

        private void markAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkAll(true);
        }

        private void unmarkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkAll(false);
        }


        private void deleteMarkedFromIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMarkedFromIndex();
        }

        private void ShowNewSearch()
        {
            new LightIndexerForm().Show();
        }

        private void MarkAll(bool mark)
        {
            presenter.MarkAll(mark);
        }

        private void DeleteMarkedFromIndex()
        {
            presenter.DeleteMarkedFromIndex();
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // options menu
            MessageBox.Show("to be done");
        }

        private void LightIndexerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LightIndexerApplicationContext.RemoveForm(this);
        }

        private void wholeWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wholeWordToolStripMenuItem.Checked = !wholeWordToolStripMenuItem.Checked;
            wildCardToolStripMenuItem.Checked = false;
            tbPath.Focus();
        }

        private void wildCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wildCardToolStripMenuItem.Checked = !wildCardToolStripMenuItem.Checked;
            wholeWordToolStripMenuItem.Checked = false;
            tbPath.Focus();
        }

        private void contextMenuStripGrid_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dgvFiles.CurrentCell == null)
            {
                e.Cancel = true;
            }
        }

        private void searchEnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.Search();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem menuItem = sender as ToolStripDropDownItem;

            if (menuItem == null)
            {
                throw new ArgumentNullException("sender");
            }

            var docId = dgvFiles.CurrentCell.RowIndex;
            var handler = new MenuActionHandler(presenter.dr, docId);
            var action = MenuActions.Copy;

            switch (menuItem.Name)
            {
                case "openToolStripMenuItem":
                case "openToolStripMenuItem1":
                    action = MenuActions.Open;
                    break;
                case "openPathToolStripMenuItem":
                case "openContainingFolderToolStripMenuItem":
                    action = MenuActions.OpenPath;
                    break;
                case "copyToolStripMenuItem":
                case "toolStripMenuItemCopy":
                    action = MenuActions.Copy;
                    break;
                case "copyPathToolStripMenuItem":
                case "copyPathToolStripMenuItem1":
                    action = MenuActions.CopyPath;
                    break;
                case "copyFileNameToolStripMenuItem":
                case "copyFileNameToolStripMenuItem1":
                    action = MenuActions.CopyFileName;
                    break;
                case "copyFileNameWithoutExtensionToolStripMenuItem":
                case "copyFileNameWithoutExtensionToolStripMenuItem1":
                    action = MenuActions.CopyFileNameWithoutExtension;
                    break;
                case "copyFullNameToolStripMenuItem":
                case "copyFullNameToolStripMenuItem1":
                    action = MenuActions.CopyFullName;
                    break;
                case "propertiesToolStripMenuItem":
                case "propertiesToolStripMenuItem1":
                    action = MenuActions.Properties;
                    break;

                default:
                    throw new NotSupportedException(string.Format("context menu '{0}'({1}) is not supported", menuItem.Text,
                                                                  menuItem.Name));
            }

            handler.HandleAction(action);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }

        private void LightIndexerForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (e.Control)
                    {
                        e.Handled = true;
                        LightIndexerApplicationContext.FocusNextForm(this, !e.Shift, false);
                    }
                    break;
                case Keys.Escape:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    Close();
                    break;
                case Keys.W:
                    if (e.Control)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        Close();
                    }
                    break;
                case Keys.N:
                    if (e.Control)
                    {
                        e.Handled = true;
                        ShowNewSearch();
                    }
                    break;
            }
        }

        void ILightIndexerView.UpdateFileToolstripMenu()
        {
            var dd = toolStripMenuItemFile.DropDownItems;

            foreach (ToolStripItem item in dd)
            {
                if (item != toolStripMenuItemIndex && item != toolStripMenuItemNewSearch)
                {
                    item.Enabled = presenter.dr != null && presenter.dr.Count > 0;
                }
            }
        }


        private const int WM_QUERYENDSESSION = 0x11;
        private static bool systemShutdown = false;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                log.Info("User Session end.");
                systemShutdown = true;
            }

            base.WndProc(ref m);
        }

        private void LightIndexerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // when user ends session we don't ask him silly question if he want's to exit the app
            if (systemShutdown)
            {
                return;
            }

            if (LightIndexerApplicationContext.IsLastForm)
            {
                var choice = MessageBox.Show(this, "Do you want to exit the application?", string.Format("Exit {0}", LightIndexerApplicationContext.ApplicationName),
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (choice == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void dgvFiles_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            bool dragAndDropClick = e.Clicks == 1 && e.Button == MouseButtons.Left && rowIndex >= 0 && dgvFiles.CurrentCell != null
                && (columnIndex == 0 || columnIndex == 1);

            if (dragAndDropClick)
            {
                Point p = new Point(e.X, e.Y);

                switch (columnIndex)
                {
                    case 0: PushDragAndDropData(DataFormats.FileDrop, p);
                        break;
                    case 1: PushDragAndDropData(DataFormats.Text, p);
                        break;
                }
            }
            else
            {
                bool rightSingleClick = e.Clicks == 1 && e.Button == MouseButtons.Right && rowIndex >= 0 && dgvFiles.CurrentCell != null;

                if (rightSingleClick)
                {
                    dgvFiles.CurrentCell = dgvFiles[0, rowIndex];
                }
            }
        }

        private void PushDragAndDropData(string dataFormat, Point startPoint)
        {
            SetDragStartPoint(startPoint);

            DataGridViewSelectedCellCollection selectedCells = dgvFiles.SelectedCells;
            IList<string> filePaths = new List<string>(selectedCells.Count);

            foreach (DataGridViewCell selectedCell in selectedCells)
            {
                var docId = selectedCell.RowIndex;
                var fileName = IndexingFacade.GetFieldValue(presenter.dr, docId, FileIndexingFields.FullName);
                filePaths.Add(fileName);
            }

            var data = new DataObject();

            if (dataFormat == DataFormats.FileDrop)
            {
                StringCollection sc = new StringCollection();
                sc.AddRange(filePaths.ToArray());
                data.SetFileDropList(sc);
            }
            else
            {
                data.SetText(string.Join(Environment.NewLine, filePaths));
            }

            DoDragDrop(data, DragDropEffects.Copy);
        }

        private void SetDragStartPoint(Point point)
        {
            point = Cursor.Position;
            dragStartPoint = point;
            //log.DebugFormat("set start point:{0}", point);
        }

        private void dgvFiles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ShowPreview();
            }
        }

        // MVP

        string ILightIndexerView.PathText
        {
            get { return tbPath.Text; }
            set { tbPath.Text = value; }
        }

        string ILightIndexerView.SearchText
        {
            get { return tbSearch.Text; }
            set { tbSearch.Text = value; }
        }

        bool ILightIndexerView.WholeWord
        {
            get { return wholeWordToolStripMenuItem.Checked; }
            set { wholeWordToolStripMenuItem.Checked = value; }
        }

        int ILightIndexerView.Slop
        {
            get { return Int32.Parse(toolStripTextBoxSlop.Text, NumberStyles.Integer); }
            set { toolStripTextBoxSlop.Text = value.ToString(); }
        }

        bool ILightIndexerView.WildCard
        {
            get { return wildCardToolStripMenuItem.Checked; }
            set { wildCardToolStripMenuItem.Checked = value; }
        }

        string ILightIndexerView.Caption
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        void ILightIndexerView.ShowWait()
        {
            waitForm.ShowView(this);
        }

        void ILightIndexerView.SetPathFocus()
        {
            tbPath.Focus();
        }

        void ILightIndexerView.HideWait()
        {
            waitForm.HideView();
        }

        void ILightIndexerView.ShowResults(int rowCount, Stopwatch sw)
        {
            dgvFiles.RowCount = 0;
            dgvFiles.RowCount = rowCount;
            _this.SetStatusLabels(new LightIndexerPresenter.SearchStatusContainer(rowCount, sw.ElapsedMilliseconds, false));

            //sets selected cell if there are results
            if (rowCount > 0)
            {
                dgvFiles.CurrentCell = dgvFiles.Rows[0].Cells[0];
                dgvFiles.Focus();
            }
        }

        void IActivableViewBase.Focus()
        {
            this.Focus();
        }

        bool IActivableViewBase.IsMainForm
        {
            get { return true; }
        }

        private bool isActive;
        bool IActivableViewBase.IsActive { get { return isActive; } }

        private void toolStripMenuItemFile_Click(object sender, EventArgs e)
        {
            log.Debug("click toolstrip");
        }

        private void toolStripMenuItemFile_VisibleChanged(object sender, EventArgs e)
        {
            log.Debug("visible toolstrip");
        }

        private void tbSearchAndPath_DragEnter(object sender, DragEventArgs e)
        {
            Point startPoint = dragStartPoint;
            Point currentPoint = new Point(e.X, e.Y);

            if (!isFarEnoughToDrop(startPoint, currentPoint))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            bool acceptDragData = e.Data.GetDataPresent(DataFormats.FileDrop) ||
                e.Data.GetDataPresent(DataFormats.StringFormat) ||
                e.Data.GetDataPresent(DataFormats.Html) ||
                e.Data.GetDataPresent(DataFormats.Riff) ||
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent(DataFormats.UnicodeText);
            DragDropEffects dragDropEffects = acceptDragData ? DragDropEffects.Copy : DragDropEffects.None;
            e.Effect = dragDropEffects;
        }

        private bool isFarEnoughToDrop(Point p1, Point p2)
        {
            double d = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            bool farEnough = d > dragMinDelta;
            return farEnough;
        }

        private void dgvFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = GetDroppedData(e.Data);
            string message = string.Join(Environment.NewLine, paths);
            this.Activate();
            DialogResult confirm = MessageBox.Show("Do you want to add these paths to the index?" + Environment.NewLine + message,
                                                   "Adding to index", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes && paths != null && paths.Length != 0)
            {
                //call adding to index routine
                //open form with progressbar for indexing
                var dialogResult = new IndexProgress(paths).ShowDialog();

                //if (dialogResult == DialogResult.OK)
                //{
                //    MessageBox.Show("Indexing finished successfully!");
                //}
                //else
                //{
                //    MessageBox.Show("Indexing was interrupted.");
                //}
            }
        }

        private void tbSearch_DragDrop(object sender, DragEventArgs e)
        {
            SetTextBoxValue(e.Data, sender as TextBox, false, ' ');
            ((Control)sender).Focus();
        }

        private void lblContent_DragDrop(object sender, DragEventArgs e)
        {
            SetTextBoxValue(e.Data, tbSearch, true, ' ');
            tbSearch.Focus();
        }

        private void tbPath_DragDrop(object sender, DragEventArgs e)
        {
            SetTextBoxValue(e.Data, sender as TextBox, false, Path.PathSeparator);
            ((Control)sender).Focus();
        }


        private void lblPath_DragDrop(object sender, DragEventArgs e)
        {
            SetTextBoxValue(e.Data, tbPath, true, Path.PathSeparator);
            tbPath.Focus();
        }

        private void SetTextBoxValue(IDataObject dataObject, TextBox textBox, bool append, char separator)
        {
            string[] droppedData = GetDroppedData(dataObject);

            if (droppedData == null)
            {
                return;
            }

            string res = string.Join(separator.ToString(CultureInfo.InvariantCulture), droppedData);

            if (append)
            {
                string tbOldText = textBox.Text;
                textBox.Text = !string.IsNullOrWhiteSpace(tbOldText) ? tbOldText + separator + res : res;
            }
            else
            {
                textBox.Text = res;
            }
        }

        private string[] GetDroppedData(IDataObject dataObject)
        {
            string[] formats = dataObject.GetFormats();

            foreach (var format in formats)
            {
                if (format == DataFormats.StringFormat)
                {
                    return new string[] { dataObject.GetData(DataFormats.StringFormat) as string };
                }
                else if (format == DataFormats.Text)
                {
                    return new string[] { dataObject.GetData(DataFormats.Text) as string };
                }
                else if (format == DataFormats.UnicodeText)
                {
                    return new string[] { dataObject.GetData(DataFormats.UnicodeText) as string };
                }
                else if (format == DataFormats.FileDrop)
                {
                    return dataObject.GetData(DataFormats.FileDrop) as string[];
                }
            }

            return null;
        }

        private void dgvFiles_DragLeave(object sender, EventArgs e)
        {
        }

        private void dgvFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void dgvFiles_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            var rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            bool shift = (Control.ModifierKeys & Keys.Shift) != Keys.None;
            bool dragAndDropClick = !shift && e.Button == MouseButtons.Left && rowIndex >= 0 && dgvFiles.CurrentCell != null
                && (columnIndex == 0 || columnIndex == 1);// && !leaveDrag;

            if (dragAndDropClick)
            {
                Point p = new Point(e.X, e.Y);

                switch (columnIndex)
                {
                    case 0: PushDragAndDropData(DataFormats.FileDrop, p);
                        break;
                    case 1: PushDragAndDropData(DataFormats.Text, p);
                        break;
                }
            }
        }

    }
}
