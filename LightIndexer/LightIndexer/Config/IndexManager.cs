using System;
using System.Collections.Generic;
using System.IO;
using LightIndexer.Lucene;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using log4net;

namespace LightIndexer.Config
{
    /// <summary>
    /// Class provides methods for work with index 
    /// </summary>
    public sealed class IndexManager : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private object _lock = new object();

        private readonly DirectoryType directoryType;

        private readonly string directoryPath;

        private Directory _directory;

        private IndexReader _indexReader;

        private Searcher _searcher;

        // maximum terms amount, lucene default is 10000
        private const int MAX_FIELD_LENGTH = 50000;

        /// <summary>
        /// Static ctor. Initializes Lucene parameters.
        /// </summary>
        static IndexManager()
        {
            // this allows us make really long queries
            // we need it for proper work of phrase queries
            BooleanQuery.MaxClauseCount = int.MaxValue;
        }

        internal IndexManager(DirectoryType directoryType, string directoryPath)
        {
            this.directoryType = directoryType;
            this.directoryPath = directoryPath;
        }

        /// <summary>
        /// Calls Close() on Directory, IndexReader and Search object instances.
        /// Used to refresh IndexReader. Could be used for releasing resources as well.
        /// </summary>
        internal void Close()
        {
            // sync
            lock (_lock)
            {
                Dispose(true);
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="Directory"/> configured against the configuration
        /// </summary>
        private Directory GetDirectory(DirectoryType directoryType, string directoryPath)
        {
            lock (_lock)
            {
                if (_directory == null)
                {
                    switch (directoryType)
                    {
                        case DirectoryType.RAM:
                            _directory = new RAMDirectory();
                            break;
                        case DirectoryType.DISK:
                            {
                                // if path doesn't exist we create an empty index and close it
                                // so we'll have pre-created index
                                if (!System.IO.Directory.Exists(directoryPath))
                                {
                                    using (_InitializeDirectory(directoryPath))
                                    using (_InitializeWriter(_directory, GetAnalyzer(), true))
                                    {
                                        //this is empty intentionally, all the magic already happened in upper usings
                                    }
                                }

                                _directory = _InitializeDirectory(directoryPath);
                                break;
                            }
                        default:
                            throw new InvalidOperationException("can't initialize lucene directory");
                    }
                }

                return _directory;
            }
        }

        private static FSDirectory _InitializeDirectory(string dirPath)
        {
            return FSDirectory.Open(new DirectoryInfo(dirPath), new SimpleFSLockFactory());
        }

        /// <summary>
        /// Get the new instance of <see cref="Analyzer"/>
        /// Now it uses <see cref="WhitespaceAnalyzerLowerCase"/>
        /// </summary>
        internal Analyzer GetAnalyzer()
        {
            return new WhitespaceAnalyzerLowerCase();
        }

        /// <summary>
        /// Get the new instance of <see cref="WhitespaceAnalyzerLowerCase"/>
        /// </summary>
        internal Analyzer GetAnalyzerLowerCase()
        {
            return new WhitespaceAnalyzerLowerCase();
        }

        /// <summary>
        /// Opens an IndexReader. Next call returns the same instance.
        /// Use Close() method to reset indexreader.
        /// </summary>
        public IndexReader GetOpenIndexReader()
        {
            lock (_lock)
            {
                if (_indexReader == null)
                {
                    _indexReader = IndexReader.Open(GetDirectory(directoryType, directoryPath), true);
                }
            }
            return _indexReader;
        }

        /// <summary>
        /// Gets an <see cref="IndexSearcher"/> instance. Next call returns the same instance.
        /// Use Close() to reset.
        /// </summary>
        internal Searcher GetSearcher()
        {
            lock (_lock)
            {
                if (_searcher == null)
                {
                    _searcher = new IndexSearcher(GetOpenIndexReader());
                }
            }
            return _searcher;
        }

        /// <summary>
        /// Instantiate <see cref="IndexWriter"/> according to application configuration.
        /// </summary>
        /// <returns>new instance of <see cref="IndexWriter"/></returns>
        internal IndexWriter OpenIndexWriter()
        {
            return _InitializeWriter(GetDirectory(directoryType, directoryPath), GetAnalyzer(), false);
        }

        public void DeleteItems(IEnumerable<string> paths)
        {
            lock (_lock)
            {
                using (var w = _InitializeWriter(GetDirectory(directoryType, directoryPath), GetAnalyzer(), false))
                {
                    w.DeleteDocuments(QueryBuilder.MakeQueryForDelete(paths));
                    w.Commit();
                    Refresh();
                }
            }
        }

        public void DeleteIndex()
        {
            lock (_lock)
            {
                using (var w = _InitializeWriter(GetDirectory(directoryType, directoryPath), GetAnalyzer(), true))
                {
                    w.Commit();
                }

                Refresh();
            }
        }

        private IndexWriter _InitializeWriter(Directory directory, Analyzer analyzer, bool create)
        {
            try
            {
                var writer = new IndexWriter(directory, analyzer, create, IndexWriter.MaxFieldLength.LIMITED);
                writer.SetMaxFieldLength(MAX_FIELD_LENGTH);
                //writer.SetTermIndexInterval(1024);
                return writer;
            }
            catch (Exception ex)
            {
                log.Error("Couldn't initialize writer.", ex);
                throw;
            }
        }

        private void Refresh()
        {
            if (_directory != null)
            {
                _directory.Dispose();
                _directory = null;
            }

            if (_indexReader != null)
            {
                _indexReader.Dispose();
                _indexReader = null;
            }

            if (_searcher != null)
            {
                _searcher.Dispose();
                _searcher = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposable)
        {
            if (!isDisposable)
            {
                return;
            }

            if (_directory != null)
            {
                _directory.Dispose();
                _directory = null;
            }

            if (_indexReader != null)
            {
                _indexReader.Dispose();
                _indexReader = null;
            }

            if (_searcher != null)
            {
                _searcher.Dispose();
                _searcher = null;
            }
        }

        ~IndexManager()
        {
            Dispose(false);
        }
    }
}