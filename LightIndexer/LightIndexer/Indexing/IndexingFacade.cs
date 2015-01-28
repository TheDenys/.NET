using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using LightIndexer.Config;
using LightIndexer.Lucene;
using log4net;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using PDNUtils.Worker;
using FIF = LightIndexer.Indexing.FileIndexingFields;
using Utils = PDNUtils.Help.Utils;

namespace LightIndexer.Indexing
{

    public delegate void ShowDelegate(DirectoryWalker walker, long total);

    public class IndexingFacade
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string FULL_NAME = FIF.FullName.F2S();
        private static readonly string PATH = FIF.Path.F2S();
        private static readonly string EXTENSION = FileIndexingFields.Extension.F2S();
        private static readonly string NAME_WITHOUT_EXTENSION = FIF.NameWithoutExtension.F2S();

        public static void IndexWrite(string path, ShowDelegate show, int? depth)
        {
            var total = Utils.GetFilesCount(path, depth);

            using (IndexManager defaultIndexManager = Configurator.GetDefaultIndexManager())
            {
                using (var writer = defaultIndexManager.OpenIndexWriter())
                {
                    var di = new DirectoryInfo(path);

                    var walker = new DirectoryWalker(true); //, ignoredExt);

                    long current = 0;

                    Thread showThread = new Thread(() =>
                    {
                        if (show != null)
                        {
                            show(walker, total);
                        }
                    }
                        );

                    showThread.Start();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    {
                        walker.Walk(di, fi =>
                        {
                            Document doc = null;
                            try
                            {
                                doc = DocumentBuilder.GetDocument(fi);

                                if (doc != null)
                                {
                                    writer.AddDocument(doc);
                                }
                            }
                            catch (Exception e)
                            {
                                log.Error(
                                    string.Format("failed to index {0}\n{1}", fi.FullName, e.Message), e);
                            }
                            finally
                            {
                                //docBuilder.ReleaseIndexerDocument(doc);
                            }

                            Interlocked.Increment(ref current);
                        });

                    }

                    sw.Stop();
                    log.InfoFormat("indexed in: {0}ms", sw.ElapsedMilliseconds);

                    sw.Restart();

                    writer.Optimize();

                    sw.Stop();
                    log.InfoFormat("optimized in: {0}ms", sw.ElapsedMilliseconds);

                    showThread.Join();
                }
            }
        }

        static TopDocs _GetTopDocs(SearchOptions input)
        {
            log.DebugFormat("_GetTopDocs: {0}", input);

            IndexReader reader = null;

            IndexManager defaultIndexManager = Configurator.GetDefaultIndexManager();
            reader = defaultIndexManager.GetOpenIndexReader();

            if (reader.NumDocs() == 0)
            {
                log.Warn("index has no documents");
                return null;
            }

            Searcher searcher = defaultIndexManager.GetSearcher();

            var sw = new Stopwatch();

            sw.Start();

            var query = new QueryBuilder(reader, input).PrepareQuery();

            sw.Stop();

            var prepared = sw.ElapsedMilliseconds;

            sw.Restart();

            var sortFieldPath = new SortField(PATH, CultureInfo.InvariantCulture);
            var sortFieldExtension = new SortField(EXTENSION, CultureInfo.InvariantCulture);
            var sortFieldNameWithoutExtension = new SortField(NAME_WITHOUT_EXTENSION, CultureInfo.InvariantCulture);
            var sort = new Sort(new SortField[] { sortFieldPath, sortFieldExtension, sortFieldNameWithoutExtension });
            var topDocs = searcher.Search(query, null, reader.NumDocs(), sort);

            sw.Stop();

            var executed = sw.ElapsedMilliseconds;
            var docsCount = topDocs.ScoreDocs.Length;

            log.DebugFormat("_GetTopDocs: {0} prep:{1}ms, exec:{2}ms, documents:{3}", input, prepared, executed, docsCount);

            return topDocs;
        }

        public static IEnumerable<string> SearchInIndex(SearchOptions input)
        {
            var topDocs = _GetTopDocs(input);

            var sw = new Stopwatch();
            sw.Start();

            Searcher searcher = Configurator.GetDefaultIndexManager().GetSearcher();

            var res = new List<string>(100);

            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var doc = searcher.Doc(scoreDoc.Doc);
                res.Add(doc.GetField(FULL_NAME).StringValue);
            }
            sw.Stop();

            var processed = sw.ElapsedMilliseconds;
            var resultLength = topDocs.ScoreDocs.Length;

            log.DebugFormat("SearchInIndex: {0} gather results:{1}ms, results:{2}", input, processed, resultLength);

            return res;
        }

        public static IList<int> DocIds(SearchOptions input)
        {
            var res = new List<int>(100);

            var topDocs = _GetTopDocs(input);

            if (topDocs != null)
            {
                res.AddRange(topDocs.ScoreDocs.Select(scoreDoc => scoreDoc.Doc));
            }

            return res;
        }

        private static Document InnerGetDocument(int docId)
        {
            try
            {
                Searcher searcher = Configurator.GetDefaultIndexManager().GetSearcher();

                if (searcher.MaxDoc != 0)
                {
                    return searcher.Doc(docId);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return null;
        }

        public static Document GetDocument(int docId)
        {
            return InnerGetDocument(docId);
        }

        public static string GetFieldValue(IDataRetriever<Document> dr, int docId, FileIndexingFields fieldType)
        {
            var document = dr.GetItem(docId);
            var field = document.GetField(fieldType.F2S());
            string value = null;

            if (field == null)
            {
                var fullname = document.GetField(FULL_NAME).StringValue;
                switch (fieldType)
                {
                    case FileIndexingFields.Path:
                        value = PDNUtils.Help.Utils.GetDirectoryName(fullname);
                        break;
                    case FileIndexingFields.NameWithoutExtension:
                        value = Path.GetFileNameWithoutExtension(fullname);
                        break;
                    case FileIndexingFields.NameWithExtension:
                        value = Path.GetFileName(fullname);
                        break;
                    default:
                        log.Warn("field is null " + fieldType);
                        break;
                }
            }
            else
            {
                value = field.StringValue;
            }

            return value;
        }

        public static int Count
        {
            get
            {
                return Configurator.GetDefaultIndexManager().GetOpenIndexReader().NumDocs();
            }
        }

    }
}