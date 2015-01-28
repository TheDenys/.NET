using System;
using System.IO;
using LightIndexer.Indexing;
using log4net;
using Lucene.Net.Documents;

namespace LightIndexer.Lucene
{

    public enum IndexingType
    {
        Keyword,
        Text,
        UnIndexed,
        UnStored
    }

    public static class FieldFactory
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Field Text(string name, string value) { return new Field(name, value, Field.Store.YES, Field.Index.ANALYZED); }
        public static Field Text(string name, TextReader value) { return new Field(name, value); }
        public static Field Keyword(string name, string value) { return new Field(name, value, Field.Store.YES, Field.Index.NOT_ANALYZED); }
        public static Field UnIndexed(string name, string value) { return new Field(name, value, Field.Store.YES, Field.Index.NO); }
        public static Field UnStored(string name, string value) { return new Field(name, value, Field.Store.NO, Field.Index.NOT_ANALYZED); }

        public static Field BuildField(IndexingType indexingType, FileIndexingFields name, string value)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("BuildField({0}, {1}, {2})", indexingType, name, value);
            }

            //string f2s = name.ToString();
            switch (indexingType)
            {
                case IndexingType.Keyword:
                    return Keyword(name.F2S(), value);
                case IndexingType.Text:
                    return Text(name.F2S(), value);
                case IndexingType.UnIndexed:
                    return UnIndexed(name.F2S(), value);
                case IndexingType.UnStored:
                    return UnStored(name.F2S(), value);
                default:
                    throw new InvalidOperationException(string.Format("unknown indexing type {0}, name:[{1}], value:[{2}]", indexingType, name, value));
            }
        }
    }
}