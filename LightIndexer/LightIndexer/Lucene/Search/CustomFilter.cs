using System.Collections;
using LightIndexer.Indexing;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace LightIndexer.Lucene.Search
{
    public class CustomFilter:Filter
    {
        public override DocIdSet GetDocIdSet(global::Lucene.Net.Index.IndexReader reader)
        {
            var maxDoc = reader.MaxDoc;
            BitArray ba = new BitArray(maxDoc);
            int[] docs = new int[maxDoc];
            int[] freqs = new int[maxDoc];

            var td = reader.TermDocs(null);
            var read = td.Read(docs, freqs);
            
            if (read != 0)
            {
                foreach (var docId in docs)
                {
                    ba.Set(docId, true);
                }
            }
            
            return new DocIdBitSet(ba);
        }
    }
}