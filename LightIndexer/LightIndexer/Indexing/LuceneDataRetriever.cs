using System.Collections.Generic;
using Lucene.Net.Documents;

namespace LightIndexer.Indexing
{
    public class LuceneDataRetriever : IDataRetriever<Document>
    {
        private IList<int> docIds;

        public LuceneDataRetriever(SearchOptions searchOptions)
        {
            docIds = IndexingFacade.DocIds(searchOptions);
        }

        public int Count
        {
            get { return _Count(); }
        }

        private int _Count()
        {
            return docIds.Count;
        }

        public Document GetItem(int rowIndex)
        {
            if (_Count() > rowIndex)
                return IndexingFacade.GetDocument(docIds[rowIndex]);
            return null;
        }
    }
}