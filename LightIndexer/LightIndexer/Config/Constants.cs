namespace LightIndexer.Config
{
    /// <summary>
    /// constants used in project
    /// </summary>
    internal static class Constants
    {
        internal const string EXCLUDE = "EXCLUDE";
        internal const string IGNORED_EXTENSIONS = "IGNORED_EXTENSIONS";
        internal const string LUCENE_DIRECTORY_TYPE = "LUCENE_DIRECTORY_TYPE";
        internal const string LUCENE_DIRECTORY_PATH = "LUCENE_DIRECTORY_PATH";

        internal const int MAX_THREADS = 0;//50

        internal const int MAX_FILE_SIZE = 50000000;// 50 Mb should be enough...
    }
}