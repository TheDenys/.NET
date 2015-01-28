using System.Collections.Generic;

namespace LightIndexer.Indexing
{
    public class FilterOptions
    {
        public IEnumerable<KeyValuePair<FileIndexingFields, string>> FilterStrings { get; set; }
    }
}