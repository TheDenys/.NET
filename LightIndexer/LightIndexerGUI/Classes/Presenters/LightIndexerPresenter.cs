using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using LightIndexer.Config;
using LightIndexer.Indexing;
using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Views;
using Lucene.Net.Documents;
using Microsoft.Experimental.IO;

namespace LightIndexerGUI.Classes.Presenters
{
    internal class LightIndexerPresenter
    {
        private ILightIndexerView view;

        private LightIndexerModel model;

        internal SearchOptions SearchOptions { get; set; }

        internal IDataRetriever<Document> dr;

        private static readonly string FULL_NAME = FileIndexingFields.FullName.F2S();

        private static readonly string FLAGS = FileIndexingFields.Flags.F2S();

        private static readonly string EXTENSION = FileIndexingFields.Extension.F2S();

        private static readonly string ARCHIVE_ENTRY = ((int)EntryFlags.ArchiveEntry).ToString();

        internal LightIndexerPresenter(ILightIndexerView lightIndexerView)
        {
            this.view = lightIndexerView;
            this.model = new LightIndexerModel();
        }

        internal void Init(SearchOptions searchOptions)
        {
            if (searchOptions != null)
            {
                SearchOptions = searchOptions;
                view.PathText = searchOptions.SearchPath;
                view.SearchText = searchOptions.SearchString;
                view.WholeWord = searchOptions.MatchWholeWord;
                view.Slop = searchOptions.Slop;
                view.WildCard = searchOptions.Wildcard;
            }

            SetCaption();
            view.CurrentFormState = LightIndexerModel.FormState.Ready;
        }

        internal void Search()
        {
            if ((string.IsNullOrWhiteSpace(view.PathText) && string.IsNullOrWhiteSpace(view.SearchText)) ||
                view.CurrentFormState != LightIndexerModel.FormState.Ready)
            {
                return;
            }

            model.InvalidateMark();
            view.CurrentFormState = LightIndexerModel.FormState.Busy;

            SearchOptions = model.BuildSearchOptions(
                view.PathText, view.SearchText, view.WholeWord, view.Slop, view.WildCard
            );

            SetCaption();

            view.SetStatusLabels(new LightIndexerPresenter.SearchStatusContainer(-1, -1, true));
            //view.ShowWait();

            var sw = Stopwatch.StartNew();

            Observable.ToAsync<SearchOptions, IDataRetriever<Document>>(DataRetriverFactory.GetDataRetriever)(SearchOptions).ObserveOn(SynchronizationContext.Current).Subscribe(
                (result) =>
                {
                    dr = result;
                    var rowCount = dr.Count;
                    view.ShowResults(rowCount, sw);
                    //view.HideWait();
                    view.SetStatusLabels(new LightIndexerPresenter.SearchStatusContainer(-1, -1, false));
                    view.UpdateFileToolstripMenu();
                    view.CurrentFormState = LightIndexerModel.FormState.Ready;
                    SetCaption();
                },//onnext
                //(ex) => { },//onerror
                () =>
                {
                    //waitForm.Hide();
                }//oncompleted
            );
        }

        internal void DeleteMarkedFromIndex()
        {
            view.CurrentFormState = LightIndexerModel.FormState.Busy;

            string name = FileIndexingFields.FullName.F2S();
            ISet<string> paths = new HashSet<string>();

            foreach (var markedId in model.GetMarkedIds())
            {
                var d = dr.GetItem(markedId);

                if (d != null)
                {
                    string path = d.GetField(name).StringValue.ToLowerInvariant();
                    paths.Add(path);
                }
            }

            //Configurator.GetDefaultIndexManager().DeleteItems(paths);
            view.CurrentFormState = LightIndexerModel.FormState.Busy;

            Observable.ToAsync(() => Configurator.GetDefaultIndexManager().DeleteItems(paths))().ObserveOn(
                SynchronizationContext.Current).Subscribe((unit) =>
                                                              {
                                                                  view.SetStatusLabels(new LightIndexerPresenter.SearchStatusContainer(-1, -1, false));
                                                                  view.UpdateFileToolstripMenu();
                                                                  view.CurrentFormState = LightIndexerModel.FormState.Ready;

                                                                  SetCaption();
                                                              });
        }

        internal void OnShown()
        {
            if (SearchOptions == null)
            {
                view.SetPathFocus();
            }
            else
            {
                Search();
            }
        }

        internal void SetCaption()
        {
            string text = SearchOptions != null ? SearchOptions.SearchString : null;
            text = !string.IsNullOrWhiteSpace(text) ? string.Format("{0} - ", text) : null;
            view.Caption = string.Format("{0}{1} {2}", text, LightIndexerApplicationContext.FullApplicationName, view.CurrentFormState == LightIndexerModel.FormState.Busy ? "(Busy)" : string.Empty);
        }

        internal bool IsMarked(int id)
        {
            return model.IsMarked(id);
        }

        internal int MarkCurrent()
        {
            var id = view.GetCurrentId();
            bool mark = !model.IsMarked(id);
            view.Mark(id, mark);

            if (mark)
            {
                model.MarkItem(id);
            }
            else
            {
                model.UnmarkItem(id);
            }

            return id;
        }

        internal void MarkCurrentAndGoToNext()
        {
            var id = MarkCurrent();
            view.SetActiveCell(++id);
        }

        internal void MarkAll(bool mark)
        {
            for (int i = 0; i < dr.Count; i++)
            {
                view.Mark(i, mark);

                if (mark)
                {
                    model.MarkItem(i);
                }
                else
                {
                    model.UnmarkItem(i);
                }
            }
        }

        internal bool GetValue(int rowIndex, string propertyName, Font basicFont, Font virtualFont, out object value, out Font font, out bool isPacked)
        {
            value = null;
            font = null;
            isPacked = false;

            if (dr != null && dr.Count > 0)
            {
                var d = dr.GetItem(rowIndex);

                if (d != null)
                {
                    string fileName = d.GetField(FULL_NAME).StringValue;

                    var flagsField = d.GetField(FLAGS);

                    if (flagsField != null)
                    {
                        string flagsStr = flagsField.StringValue;
                        isPacked = flagsStr == ARCHIVE_ENTRY;
                    }

                    string realFileName = fileName;

                    // check if it's an archive
                    int l;
                    if ((l = fileName.IndexOf(Path.AltDirectorySeparatorChar)) != -1)
                    {
                        realFileName = fileName.Substring(0, l);
                    }

                    bool exists = LongPathFile.Exists(realFileName);
                    font = exists ? basicFont : virtualFont;

                    switch (propertyName)
                    {
                        case "FileName":
                            value = Path.GetFileName(fileName);
                            break;
                        case "FullPath":
                            value = fileName;
                            break;
                        case "Extension":
                            value = d.GetField(EXTENSION).StringValue;
                            break;
                        case "FileSize":
                            if (exists)
                            {
                                value = PDNUtils.IO.LongPath.GetFileSize(realFileName).ToString(CultureInfo.InvariantCulture);
                            }
                            break;
                    }

                    return value != null;
                }
            }

            return false;
        }

        internal bool IsPacked(int docId)
        {
            var flags = IndexingFacade.GetFieldValue(dr, docId, FileIndexingFields.Flags);
            bool isPacked = (((int) EntryFlags.ArchiveEntry).ToString(CultureInfo.InvariantCulture) == flags);
            return isPacked;
        }

        internal struct SearchStatusContainer
        {
            private readonly int rowCount;
            private readonly long elapsedMs;
            private readonly bool waiting;

            internal SearchStatusContainer(int rowCount, long elapsedMs, bool waiting)
            {
                this.rowCount = rowCount;
                this.elapsedMs = elapsedMs;
                this.waiting = waiting;
            }

            internal int RowCount { get { return rowCount; } }

            internal long ElapsedMilliseconds { get { return elapsedMs; } }

            internal bool Waiting { get { return waiting; } }
        }
    }
}