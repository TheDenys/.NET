using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using LightIndexer.Indexing;
using LightIndexer.Util;
using LightIndexerGUI.Classes;
using LightIndexerGUI.Classes.Views;
using Microsoft.Experimental.IO;
using PDNUtils.IO;
using log4net;
using Lucene.Net.Documents;
using FIF = LightIndexer.Indexing.FileIndexingFields;
using PDNUtils.Help;
using System.Linq;

namespace LightIndexerGUI.Forms
{
    public partial class Preview : Form, IActivableViewBase
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDataRetriever<Document> dr;

        private readonly PreviewOptions options;

        private Find findForm;//find form instance
        private static readonly string FULL_NAME = FIF.FullName.F2S();
        private static string _FileDoesntExistFormat;

        private Timer timer = new Timer();

        bool exists;
        Tuple<string, Stream> zipEntry;

        internal TextEditorControl TextEditorControl
        {
            get { return textEditorControl; }
        }

        public Preview(IDataRetriever<Document> dr, PreviewOptions options)
        {
            LightIndexerApplicationContext.AddForm(this);
            this.dr = dr;
            this.options = options;

            InitializeComponent();

            timer.Interval = 100;
            timer.Tick += timer_Tick;

            var ta = textEditorControl.ActiveTextAreaControl.TextArea;
            ta.PreviewKeyDown += ta_PreviewKeyDown;
            ta.Caret.PositionChanged += Caret_PositionChanged;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        void ta_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //log.DebugFormat("ta_pkd:{0}", e.KeyCode);

            if (e.KeyCode == Keys.Escape)
            {
                timer.Start();
            }
        }

        private void tbPreview_KeyDown(object sender, KeyEventArgs e)
        {
            //log.DebugFormat("keydown:{0}", e.KeyCode);

            switch (e.KeyCode)
            {
                case Keys.F:
                    if (e.Control)
                    {
                        DisplayFindDialog();
                        e.Handled = true;
                    }
                    break;
                case Keys.F3:
                    e.Handled = true;

                    if (e.Control)
                    {
                        findForm.ClearHighlight();
                    }
                    else
                    {
                        var found = findForm.FindNext(true, e.Shift, options.SearchOptions.SearchString);

                        if (found == null || found.Length == 0)
                        {
                            DisplayFindDialog();
                        }
                    }
                    break;
                case Keys.I:
                    if (e.Control)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        NewSearchInContent();
                    }
                    break;
                case Keys.P:
                    if (e.Control)
                    {
                        e.Handled = true;
                        NewSearchInPath();
                    }
                    break;
                case Keys.W:
                    if (e.Control)
                    {
                        e.Handled = true;
                        Close();
                    }
                    break;
                case Keys.Tab:
                    if (e.Control)
                    {
                        e.Handled = true;
                        LightIndexerApplicationContext.FocusNextForm(this, !e.Shift, false);
                    }
                    break;
            }
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            var c = (Caret)sender;
            DisplayPosition(c.Position.Line, c.Position.Column, c.Offset);
        }

        public void ShowPreview()
        {
            var doc = dr.GetItem(options.DocId);

            if (doc == null)
            {
                log.ErrorFormat("dr.GetItem({0}) returned null", options.DocId);
                return;
            }

            string message = null;
            string filename = doc.GetField(FULL_NAME).StringValue;

            Stream stream = null;

            // if file doesn't exist on disk we inform user
            if (!LongPathFile.Exists(filename))
            {
                // try if it's a zip
                zipEntry = ZipWrapper.GetFileFromZip(filename);

                if (zipEntry != null)
                {
                    exists = true;
                    stream = zipEntry.Item2;
                }
            }
            else
            {
                exists = true;
                stream = LongPathFile.Open(filename, FileMode.Open, FileAccess.Read);
            }

            if (!exists)
            {
                _FileDoesntExistFormat = "File \"{0}\" doesn't exist";
                message = string.Format(_FileDoesntExistFormat, filename);
                log.Error(message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetFormCaption(filename);

            bool error = true;

            try
            {
                textEditorControl.BeginUpdate();

                using (stream)
                {
                    textEditorControl.LoadFile(filename, stream, true, true);
                }

                error = false;
            }
            catch (IOException e)
            {
                log.Error(string.Format("Error when opening '{0}'", filename), e);
                message = e.Message;
            }
            finally
            {
                textEditorControl.EndUpdate();
            }

            if (error)
            {
                string text = string.Format("Can't read file: '{0}'\n{1}", filename, message);
                MessageBox.Show(text, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            findForm = new Find(this);

            if (!string.IsNullOrEmpty(options.SearchOptions.SearchString))
            {
                findForm.FindNext(true, false, options.SearchOptions.SearchString);
            }

            Show();
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            findForm.FindNext(true, true, options.SearchOptions.SearchString);
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            findForm.FindNext(true, false, options.SearchOptions.SearchString);
        }

        private void SetFormCaption(string filename)
        {
            this.Text += string.Format(": {0} - {1}", options.SearchOptions.SearchString, filename);
        }

        private void DisplayFindDialog()
        {
            if (!findForm.Visible) { findForm.Show(this); findForm.CenterFormTo(this); } else { findForm.Focus(); }
        }

        private void DisplayPosition(int line, int column, int offset)
        {
            toolStripStatusLabelCursor.Text = string.Format("L: {0} C: {1} Off: {2}", line + 1, column + 1, offset);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            DisplayFindDialog();
        }

        private void searchInIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSearchInContent();
        }

        private void searchInPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSearchInPath();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var m = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;

            if (m.HasSomethingSelected)
            {
                var sel = m.SelectedText;
                log.DebugFormat("sel:'{0}'", sel);

                try
                {
                    Clipboard.SetText(sel);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("failed to write '{0}' to clipboard", sel), ex);
                }
            }
        }

        private void NewSearchInPath()
        {
            OpenNewSearch(FileIndexingFields.FullNameLowerCase);
        }

        private void NewSearchInContent()
        {
            OpenNewSearch(FileIndexingFields.Content);
        }


        private void OpenNewSearch(FileIndexingFields field)
        {
            string searchString = findForm.GetSelection();
            var searchOptions = new SearchOptions();

            switch (field)
            {
                case FileIndexingFields.FullNameLowerCase:
                    searchOptions.SearchPath = searchString;
                    break;
                case FileIndexingFields.Content:
                    searchOptions.SearchString = searchString;
                    break;
                default:
                    throw new ArgumentException(string.Format("type {0} is not supported", field));
            }

            new LightIndexerForm(searchOptions).Show();
        }

        protected override void OnActivated(EventArgs e)
        {
            IsActive = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            IsActive = false;
            LightIndexerApplicationContext.SetLastForm(this);
        }

        public bool IsActive { get; private set; }

        void IActivableViewBase.Focus()
        {
            Focus();
        }

        public bool IsMainForm
        {
            get { return false; }
        }

        private void Preview_FormClosed(object sender, FormClosedEventArgs e)
        {
            LightIndexerApplicationContext.RemoveForm(this);
        }
    }
}