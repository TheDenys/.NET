namespace LightIndexer.Indexing
{
    public class SearchOptions
    {
        public string SearchPath { get; set; }
        public string SearchString { get; set; }
        public bool IgnoreCase { get; set; }
        public bool MatchWholeWord { get; set; }
        public bool Wildcard { get; set; }
        public bool Regexp { get; set; }
        public int Slop { get; set; }
        public FilterOptions FilterOptions { get; set; }

        public override string ToString()
        {
            return string.Format(
                @"SearchPath:""{0}"" Searchstring:""{1}"" IgnoreCase:{2} MatchWholeWord:{3} WildCard:{4} Regexp:{5} Slop:{6}",
                SearchPath,
                SearchString,
                IgnoreCase,
                MatchWholeWord,
                Wildcard,
                Regexp,
                Slop);
        }
    }
}