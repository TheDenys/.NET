using LightIndexer.Indexing;
using log4net;
using PDNUtils.Help;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace LightIndexer.Config
{
    /// <summary>
    /// Class provides methods for convenient access to configuration
    /// </summary>
    public sealed class Configurator
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IEnumerable<string> ignoredExt;

        public static readonly Lazy<ExcludeMatcher> ExcludeMatcher = new Lazy<ExcludeMatcher>(() =>
        {
            var exclusions = ConfigurationManager.AppSettings[Constants.EXCLUDE];
            return new ExcludeMatcher(exclusions);
        });

        /// <summary>
        /// Collection of extensions which will be ignored from content indexing
        /// </summary>
        internal static IEnumerable<string> IgnoredExtensions
        {
            get
            {
                if (ignoredExt == null)
                {
                    var ignoredExtensionsString = ConfigurationManager.AppSettings[Constants.IGNORED_EXTENSIONS];
                    log.InfoFormat("ignored extensions:{0}", ignoredExtensionsString);
                    ignoredExt = ignoredExtensionsString.Split(new string[] { ",", ";", "|" },
                                                               StringSplitOptions.RemoveEmptyEntries);
                }
                return ignoredExt;
            }
        }

        /// <summary>
        /// RAM of FSDirectory type of lucene directory. Look in app.config
        /// </summary>
        internal static readonly DirectoryType LuceneDirectoryType = Utils.GetEnumObject<DirectoryType>(ConfigurationManager.AppSettings[Constants.LUCENE_DIRECTORY_TYPE]);

        /// <summary>
        /// path to the lucene index
        /// </summary>
        internal static string GetLuceneDirectoryPath()
        {
            if (LuceneDirectoryType == DirectoryType.DISK)
            {
                string luceneDirectoryPath = ConfigurationManager.AppSettings[Constants.LUCENE_DIRECTORY_PATH];

                log.InfoFormat("Indexing folder from app.config:'{0}'", luceneDirectoryPath);

                if (!Path.IsPathRooted(luceneDirectoryPath))
                {
                    string basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    luceneDirectoryPath = Path.GetFullPath(Path.Combine(basePath, luceneDirectoryPath));
                    log.InfoFormat("base:'{0}' indexing folder:'{1}'", basePath, luceneDirectoryPath);
                }

                return luceneDirectoryPath;
            }
            return null;
        }

        private static readonly IndexManager defaultIndexManager = new IndexManager(LuceneDirectoryType, GetLuceneDirectoryPath());

        public static IndexManager GetDefaultIndexManager()
        {
            return defaultIndexManager;
        }

    }
}