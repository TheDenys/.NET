using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LightIndexer.Indexing;
using PDNUtils.IO;
using log4net;
using Lucene.Net.Documents;

namespace LightIndexerGUI.Classes
{
    public class MenuActionHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IDataRetriever<Document> dr;
        protected readonly int docId;

        public MenuActionHandler(IDataRetriever<Document> dr, int docId)
        {
            this.dr = dr;
            this.docId = docId;
        }

        public void HandleAction(MenuActions action)
        {
            string fieldValue;

            switch (action)
            {
                // TODO: comb this nudles
                case MenuActions.Open:
                    fieldValue = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.FullName);
                    fieldValue = fieldValue.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    Start(fieldValue);
                    break;
                case MenuActions.OpenPath:
                    //fieldValue = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.Path);
                    fieldValue = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.FullName);
                    fieldValue = fieldValue.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    //fieldValue = Path.GetDirectoryName(fieldValue);
                    Start("explorer", string.Format("/n,/select,\"{0}\"", fieldValue));
                    break;
                case MenuActions.Copy:
                    WriteFileToClipboard(docId);
                    break;
                case MenuActions.CopyPath:
                    WriteFieldValueToClipboard(docId, FileIndexingFields.Path);
                    break;
                case MenuActions.CopyFileName:
                    WriteFieldValueToClipboard(docId, FileIndexingFields.NameWithExtension);
                    break;
                case MenuActions.CopyFileNameWithoutExtension:
                    WriteFieldValueToClipboard(docId, FileIndexingFields.NameWithoutExtension);
                    break;
                case MenuActions.CopyFullName:
                    WriteFieldValueToClipboard(docId, FileIndexingFields.FullName);
                    break;
                case MenuActions.Properties:
                    fieldValue = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.FullName);
                    PDNUtils.IO.LongPath.ShowProperties(fieldValue);
                    break;

                default:
                    throw new NotSupportedException(string.Format("action '{0}' is not supported", action));
            }
        }

        protected void Start(string path)
        {
            log.DebugFormat("start({0})", path);
            bool showError = true;

            try
            {
                Process.Start(path);
                showError = false;
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }

            if (showError)
            {
                MessageBox.Show("error");
            }
            //LongPath.StartProcess(path);
            //LongPath.ShellExecute(path);
            //LongPath.GetShortPath(path);
        }

        protected void Start(string program, string arguments)
        {
            log.DebugFormat("start(({0}),({1}))",program,arguments);
            bool showError = true;
            
            try
            {
                Process.Start(program, arguments);
                showError = false;
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }

            if (showError)
            {
                MessageBox.Show("error");
            }
            //LongPath.StartProcess(path);
            //LongPath.ShellExecute(path);
            //LongPath.GetShortPath(path);
        }

        protected void WriteFieldValueToClipboard(int docId, FileIndexingFields field)
        {
            var fieldValue = IndexingFacade.GetFieldValue(dr, docId, field);
            WriteTextToClipboard(fieldValue);
        }

        protected void WriteTextToClipboard(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception e)
            {
                log.Error(string.Format("failed to write '{0}' to clipboard", text), e);
            }
        }

        protected void WriteFileToClipboard(int docId)
        {
            var fieldValue = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.FullName);
            var sc = new StringCollection();
            sc.Add(fieldValue);
            WriteFilesToClipboard(sc);
        }

        protected void WriteFilesToClipboard(StringCollection filePaths)
        {
            try
            {
                Clipboard.SetFileDropList(filePaths);
            }
            catch (Exception e)
            {
                log.Error(string.Format("failed to write '{0}' to clipboard", filePaths), e);
            }
        }

    }
}