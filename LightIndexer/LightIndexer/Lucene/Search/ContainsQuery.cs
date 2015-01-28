using System;
using System.Globalization;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LightIndexer.Lucene.Search
{

    [Serializable]
    public class ContainsQuery : MultiTermQuery
    {

        private readonly Term queryTerm;

        public ContainsQuery(Term term)
        {
            queryTerm = term;
        }

        protected override FilteredTermEnum GetEnum(IndexReader reader)
        {
            return new ContainsTermEnum(reader, queryTerm);
        }

        public override bool Equals(System.Object o)
        {
            if (o is ContainsQuery)
                return base.Equals(o);

            return false;
        }

        public override string ToString(string field)
        {
            return string.Format(CultureInfo.InvariantCulture, "Term:'{0}' Field:'{1}'", queryTerm.Text, field);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}