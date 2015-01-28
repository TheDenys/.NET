using System;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LightIndexer.Lucene.Search
{

    /// <summary> Subclass of FilteredTermEnum for enumerating all terms that match the
    /// specified wildcard filter term.
    /// <p>
    /// Term enumerations are always ordered by Term.compareTo().  Each term in
    /// the enumeration is greater than all that precede it.
    /// 
    /// </summary>
    /// <version>  $Id: WildcardTermEnum.java 329859 2005-10-31 17:05:36Z bmesser $
    /// </version>
    public class ContainsTermEnum : FilteredTermEnum
    {
        internal String field = "";
        internal String text = "";
        internal bool endEnum = false;

        /// <summary> Creates a new <code>WildcardTermEnum</code>.  Passing in a
        /// {@link Lucene.Net.index.Term Term} that does not contain a
        /// <code>WILDCARD_CHAR</code> will cause an exception to be thrown.
        /// <p>
        /// After calling the constructor the enumeration is already pointing to the first 
        /// valid term if such a term exists.
        /// </summary>
        public ContainsTermEnum(IndexReader reader, Term term)
        {
            field = term.Field;
            text = term.Text;

            SetEnum(reader.Terms(new Term(field, string.Empty)));
        }

        protected override bool TermCompare(Term term)
        {
            if (field == term.Field)
            {
                var searchText = term.Text;
                return searchText.Contains(text);
            }
            endEnum = true;
            return false;
        }

        public override float Difference()
        {
            return 1.0f;
        }

        public override bool EndEnum()
        {
            return endEnum;
        }
    }
}