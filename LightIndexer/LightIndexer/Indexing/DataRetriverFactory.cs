using Lucene.Net.Documents;

namespace LightIndexer.Indexing
{
    /// <summary>
    /// Factory for IDataRetiriever
    /// </summary>
    public static class DataRetriverFactory
    {
        
        /// <summary>
        /// Returns concrete <see cref="IDataRetriever{T}"/> implementation instance configured against <paramref name="searchOptions"/>
        /// </summary>
        /// <param name="searchOptions">Search options for data retriever</param>
        /// <returns>data retriever</returns>
        public static IDataRetriever<Document> GetDataRetriever(SearchOptions searchOptions)
        {
            //return new MockDataRetriever();
            return new LuceneDataRetriever(searchOptions);
        }

    }
}