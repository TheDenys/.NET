using System;
using System.Collections.Generic;
using System.IO;
using FIF = LightIndexer.Indexing.FileIndexingFields;
using IT = LightIndexer.Lucene.IndexingType;

namespace LightIndexer.Indexing
{

    public static class FileIndexingFieldsMethods
    {
        private static readonly IDictionary<FileIndexingFields, string> _f2s = new Dictionary<FileIndexingFields, string>(8);

        static FileIndexingFieldsMethods()
        {
            var values = Enum.GetValues(typeof(FileIndexingFields));

            foreach (FileIndexingFields value in values)
            {
                _f2s.Add(value, ((int)value).ToString());
            }
        }

        public static string F2S(this FileIndexingFields value)
        {
            return _f2s[value];
        }

    }

    /// <summary>
    /// Enumaration of possible fields for indexed file
    /// </summary>
    public enum FileIndexingFields : int
    {
        /// <summary>
        /// Full file name
        /// </summary>
        FullName = 0,

        /// <summary>
        /// File path, i.e. directory name
        /// </summary>
        Path = 1,

        /// <summary>
        /// File extension, e.g. ".txt", ".pdf", etc.
        /// </summary>
        Extension = 2,

        /// <summary>
        /// File name with extension
        /// </summary>
        NameWithExtension = 3,
        
        /// <summary>
        /// File name without extension
        /// </summary>
        NameWithoutExtension = 4,

        /// <summary>
        /// Full file name in lowercase
        /// </summary>
        FullNameLowerCase = 5,

        /// <summary>
        /// Flags used to identify what kind of entry it is: physical file, file in archive, etc.
        /// </summary>
        Flags = 6,

        /// <summary>
        /// File content
        /// </summary>
        Content = 7,
    }

    [Flags]
    public enum EntryFlags : byte
    {
        Undefined = 0,
        PhysicalFile = 1,
        ArchiveEntry = 2,
    }

    /// <summary>
    /// Mappings for fields and fucntions
    /// </summary>
    public static class MappingConfig
    {
        private static readonly string ENTRY_FLAGS_PHYSICAL = ((int)EntryFlags.PhysicalFile).ToString();
        private static readonly string ENTRY_FLAGS_ARCHIVE = ((int)EntryFlags.ArchiveEntry).ToString();

        /// <summary>
        /// Mappings FieldName - IndexingType - function to get information from fileinfo
        /// </summary>
        internal static readonly IDictionary<FIF, KeyValuePair<IT, Func<FileInfo, string>>> FUNC_MAPPINGS =
            new Dictionary<FIF, KeyValuePair<IT, Func<FileInfo, string>>>
                                                                     {
                                                                         { FIF.FullName, new KeyValuePair<IT, Func<FileInfo, string>>(IT.UnIndexed,fi=>fi.FullName) },
                                                                         { FIF.Extension, new KeyValuePair<IT, Func<FileInfo, string>>(IT.Keyword,fi=>fi.Extension) },//used for sort
                                                                         { FIF.Path, new KeyValuePair<IT, Func<FileInfo, string>>(IT.UnStored,fi=>fi.DirectoryName) },//used for sort
                                                                         { FIF.NameWithExtension, new KeyValuePair<IT, Func<FileInfo, string>>(IT.UnStored,fi=>fi.Name) },//used for sort
                                                                         { FIF.NameWithoutExtension, new KeyValuePair<IT, Func<FileInfo, string>>(IT.UnStored,fi=>Path.GetFileNameWithoutExtension(fi.Name)) },//used for sort
                                                                         { FIF.FullNameLowerCase, new KeyValuePair<IT, Func<FileInfo, string>>(IT.UnStored,fi=>fi.FullName.ToLowerInvariant()) },
                                                                     };

        /// <summary>
        /// Mappings FieldName - IndexingType - function to get information from fileinfo
        /// Func arguments: fileinfo, bool - true for physical file, otherwise false
        /// </summary>
        internal static readonly IDictionary<FIF, KeyValuePair<IT, Func<string, bool, string>>> LONG_FUNC_MAPPINGS =
            new Dictionary<FIF, KeyValuePair<IT, Func<string, bool, string>>>
                                                                     {
                                                                         { FIF.FullName, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnIndexed,(fi,_)=>fi) },
                                                                         { FIF.Extension, new KeyValuePair<IT, Func<string, bool, string>>(IT.Keyword,(fi,_)=>Path.GetExtension(fi)) },//used for sort
                                                                         { FIF.Path, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnStored,(fi,_)=>PDNUtils.Help.Utils.GetDirectoryName(fi)) },//used for sort
                                                                         { FIF.NameWithExtension, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnStored,(fi,_)=>Path.GetFileName(fi)) },//used for sort
                                                                         { FIF.NameWithoutExtension, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnStored,(fi,_)=>Path.GetFileNameWithoutExtension(fi)) },//used for sort
                                                                         { FIF.FullNameLowerCase, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnStored,(fi,_)=>fi.ToLowerInvariant()) },
                                                                         { FIF.Flags, new KeyValuePair<IT, Func<string, bool, string>>(IT.UnIndexed,(fi,physical)=>physical?ENTRY_FLAGS_PHYSICAL:ENTRY_FLAGS_ARCHIVE) },
                                                                     };
    }
}