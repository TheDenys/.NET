using LightIndexer.Config;
using log4net;
using Lucene.Net.Documents;
using Microsoft.Experimental.IO;
using PDNUtils.Help;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FF = LightIndexer.Lucene.FieldFactory;
using FIF = LightIndexer.Indexing.FileIndexingFields;

namespace LightIndexer.Indexing
{
    /// <summary>
    /// Class with methods for building a lucene document
    /// </summary>
    public static class DocumentBuilder
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string CONTENT_NAME = FIF.Content.F2S();

        private static readonly IEnumerable<string> ignoredExtensions = Configurator.IgnoredExtensions;

        private static readonly object _ignoredExtensionsCacheLocker = new object();
        private static readonly ISet<string> ignoredExtensionsCache = new HashSet<string>();

        private static readonly Lazy<ExcludeMatcher> excludeMatcher = Configurator.ExcludeMatcher;

        /// <summary>
        /// Use this function to define if we skip this file content or not.
        /// It allows us to skip binary files according to file extension.
        /// Set of extensions to skip is defined in app.config file.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        internal static bool LongSkip(string fi)
        {
            if (ignoredExtensions == null || ignoredExtensions.Count() == 0)
            {
                return false;
            }
            string extension = Path.GetExtension(fi);
            var fileExt = extension.ToLowerInvariant();
            if (ignoredExtensionsCache.Contains(fileExt))
            {
                return true;
            }
            else
            {
                var skip =
                    ignoredExtensions.Where(ext => extension != null && extension.ToLowerInvariant().Contains(ext)).Count() > 0;

                if (skip)
                {
                    lock (_ignoredExtensionsCacheLocker)
                    {
                        ignoredExtensionsCache.Add(fileExt);
                    }
                }

                return skip;
            }
        }

        /// <summary>
        /// Builds a <see cref="Document"/> instance based on <paramref name="fileInfo"/>
        /// </summary>
        /// <param name="fileInfo">file to be indexed</param>
        /// <returns>document instance</returns>
        public static IndexerDocument LongGetDocument(string fileInfo)
        {
            TextReader textReader = null;
            var doc = new Document();

            // add fields to document
            // fm.Key - field name
            // fm.Value.Key - indexing type
            // fm.Value.Value - function for retrieving information
            MappingConfig.LONG_FUNC_MAPPINGS.ForEach(fm => doc.Add(FF.BuildField(fm.Value.Key, fm.Key, fm.Value.Value(fileInfo, true))), false);

            var exclude = excludeMatcher.Value.IsExcluded(fileInfo);

            if (exclude)
            {
                return new IndexerDocument(null, null);
            }

            var isBigger = LongIsBiggerThanMaxSize(fileInfo);
            var skip = LongSkip(fileInfo);

            if (log.IsDebugEnabled && skip) { log.DebugFormat("file {0} skipped", fileInfo); }

            // we pass this instance to IndexerDocument so it can release FileStream later by call close on it.
            // For some reason FileStream is not released by call close on TextReader.
            FileStream fs = null;

            // huge and skipped files will have empty content, most probably source code file won't be bigger than 50Mb
            textReader =
                (isBigger || skip) ?
                (TextReader)new StringReader(string.Empty) :
                (TextReader)new StreamReader(fs = LongPathFile.Open(fileInfo, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.None));

            doc.Add(FF.Text(CONTENT_NAME, textReader));            // add content field

            var iDoc = new IndexerDocument(doc, fs);
            return iDoc;
        }

        /// <summary>
        /// Builds a <see cref="Document"/> instance based on <paramref name="fileInfo"/>
        /// </summary>
        /// <param name="fileInfo">file to be indexed</param>
        /// <returns>document instance</returns>
        public static IndexerDocument GetDocumentFromStream(string fileInfo, Stream stream)
        {
            TextReader textReader = null;
            var doc = new Document();

            // add fields to document
            // fm.Key - field name
            // fm.Value.Key - indexing type
            // fm.Value.Value - function for retrieving information
            MappingConfig.LONG_FUNC_MAPPINGS.ForEach(fm => doc.Add(FF.BuildField(fm.Value.Key, fm.Key, fm.Value.Value(fileInfo, false))), false);

            // huge and skipped files will have empty content, most probably source code file won't be bigger than 50Mb
            textReader = new StreamReader(stream);

            doc.Add(FF.Text(CONTENT_NAME, textReader));            // add content field

            // we pass this instance to IndexerDocument so it can release FileStream later by call close on it.
            // For some reason FileStream is not released by call close on TextReader.
            var iDoc = new IndexerDocument(doc, stream);
            return iDoc;
        }

        static bool IsBiggerThanMaxSize(FileInfo fileInfo)
        {
            bool isBiggerThanMaxSize = fileInfo.Length >= Constants.MAX_FILE_SIZE;
            return isBiggerThanMaxSize;
        }

        static bool LongIsBiggerThanMaxSize(string fileInfo)
        {
            long fileSize = PDNUtils.IO.LongPath.GetFileSize(fileInfo);
            bool isBiggerThanMaxSize = fileSize >= Constants.MAX_FILE_SIZE;

            if (log.IsInfoEnabled && isBiggerThanMaxSize)
            {
                log.InfoFormat("file {0} size {1} is bigger than {2}", fileInfo, fileSize, Constants.MAX_FILE_SIZE);
            }

            return isBiggerThanMaxSize;
        }

    }
}