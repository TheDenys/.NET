using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightIndexer.Indexing;
using LightIndexer.Lucene.Search;
using Lucene.Net.Index;
using Lucene.Net.Search;
using PDNUtils.Help;
using FIF = LightIndexer.Indexing.FileIndexingFields;

namespace LightIndexer.Lucene
{
    public class QueryBuilder
    {
        private readonly IndexReader _reader;

        private readonly SearchOptions _searchOptions;

        private static readonly string _fieldNamePath = FileIndexingFields.FullNameLowerCase.F2S();

        private static readonly string _fieldNameContent = FileIndexingFields.Content.F2S();

        private static readonly char[] separator = new char[] { Path.PathSeparator };

        private static readonly char[] wildcards = new char[] { '*', '?' };

        public QueryBuilder(IndexReader reader, SearchOptions searchOptions)
        {
            this._reader = reader;
            this._searchOptions = searchOptions;
        }

        public Query PrepareQuery()
        {
            var res = new BooleanQuery();

            AddQueryPart(_searchOptions.SearchPath, PathQuery, res);
            AddQueryPart(_searchOptions.SearchString, ContentQuery, res);

            return res;
        }

        private void AddQueryPart(string s, Func<string , Query> queryFunc, BooleanQuery result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string lowerS = s.ToLowerInvariant();
                Query contentQuery = string.IsNullOrEmpty(lowerS) ? null : queryFunc(lowerS);
                result.Add(contentQuery, Occur.MUST);
            }
        }

        private Query ContentQuery(string input)
        {
            Query query = null;

            var splitted = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (_searchOptions.MatchWholeWord && splitted.Length == 1)
            {
                query = MakeTermQuery(input, _fieldNameContent);
            }
            else if (_searchOptions.MatchWholeWord && splitted.Length > 1)
            {
                query = MakeExactPhraseQuery(_reader, splitted, _searchOptions.Slop, _fieldNameContent);
            }
            else if (splitted.Length > 1)
            {
                query = MakeContainsPhraseQuery(_reader, splitted, _fieldNameContent);
            }
            else if (_searchOptions.Wildcard)
            {
                query = MakeWildcardQuery(input, _fieldNameContent);
            }
            else
            {
                query = MakeContainsQuery(_reader, input, _fieldNameContent);
            }

            return query;
        }

        internal static IEnumerable<Query> ParseSearchPath(
            string input,
            Func<string, Query> MakeContainsFunc,
            Func<string, Query> MakeWildcardFunc
            )
        {
            IList<Query> queries = new List<Query>();
            string[] paths = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            paths.ForEach(
                (s) =>
                {
                    if (s.IndexOfAny(wildcards) != -1)
                    {
                        queries.Add(MakeWildcardFunc(s));
                    }
                    else
                    {
                        queries.Add(MakeContainsFunc(s));
                    }
                },
                false);

            return queries;
        }

        private Query PathQuery(string input)
        {
            Func<string, Query> makeContainsQuery = (s) => MakeContainsQuery(_reader, s, _fieldNamePath);
            Func<string, Query> makeWildQuery = (s) => MakeWildcardQuery(s, _fieldNamePath);
            var queries = ParseSearchPath(input, makeContainsQuery, makeWildQuery);

            var count = queries.Count();
            switch (count)
            {
                case 0:
                case 1:
                    return queries.FirstOrDefault();
                default:
                    {
                        var res = new BooleanQuery();
                        queries.ForEach((q) => res.Add(q, Occur.SHOULD), false);
                        return res;
                    }
            }
        }

        protected Query MakeWildcardQuery(string wildcard, string _fieldName)
        {
            var term = new Term(_fieldName, wildcard);
            return new WildcardQuery(term);
        }

        protected Query MakeContainsQuery(IndexReader reader, string token, string _fieldName)
        {
            return new ContainsQuery(new Term(_fieldName, token));
        }

        protected MultiPhraseQuery MakeExactPhraseQuery(IndexReader reader, string[] searchArr, int slop, string _fieldName)
        {
            var query = new MultiPhraseQuery();
            query.Slop = slop;

            searchArr.ForEach(
                token => query.Add(new Term(_fieldName, token)),
                false);

            return query;
        }

        protected MultiPhraseQuery MakeContainsPhraseQuery(IndexReader reader, string[] searchArr, string _fieldName)
        {
            MultiPhraseQuery query = new MultiPhraseQuery();

            // phrase prefix
            Func<string, bool> conditionPrefix = s => s.EndsWith(searchArr[0]);
            // phrase suffix
            Func<string, bool> conditionSuffix = s => s.StartsWith(searchArr[searchArr.Length - 1]);
            Term[] termsStart, termsMid, termsEnd;

            SearchTerms(reader, conditionPrefix, null, conditionSuffix, _fieldName, out termsStart, out termsMid, out termsEnd);

            bool hasPrefix = termsStart != null && termsStart.Length > 0;
            bool hasSuffix = termsEnd != null && termsEnd.Length > 0;

            if (hasPrefix && hasSuffix)
            {
                query.Add(termsStart);

                for (int i = 1; i < searchArr.Length - 1; i++)
                {
                    query.Add(new Term(_fieldName, searchArr[i]));
                }

                query.Add(termsEnd);
            }

            return query;
        }

        protected Query MakeTermQuery(string token, string _fieldName)
        {
            return _TermQuery(token, _fieldName);
        }

        public static Query MakeQueryForDelete(IEnumerable<string> paths)
        {
            BooleanQuery query = new BooleanQuery();

            foreach (var path in paths)
            {
                var q = _TermQuery(path, _fieldNamePath);
                query.Add(q, Occur.SHOULD);
            }

            return query;
        }

        private static Query _TermQuery(string token, string _fieldName)
        {
            var term = new Term(_fieldName, token);
            Query termQuery = new TermQuery(term);
            return termQuery;
        }

        protected string GetLowerString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            return s.ToLowerInvariant();
        }

        protected void SearchTerms(
            IndexReader reader,
            Func<string, bool> conditionPrefix,
            Func<string, bool> conditionMid,
            Func<string, bool> conditionSuffix,
            string _fieldName,
            out Term[] prefixes,
            out Term[] mids,
            out Term[] suffixes)
        {
            var termsPrefixes = new HashSet<Term>();
            var termsMids = new HashSet<Term>();
            var termsSuffixes = new HashSet<Term>();

            using (TermEnum terms = reader.Terms(new Term(_fieldName, string.Empty)))
            {
                do
                {
                    var t = terms.Term;

                    if (t != null && t.Field.Equals(_fieldName))
                    {
                        if (conditionPrefix != null && conditionPrefix(t.Text))
                        {
                            termsPrefixes.Add(t);
                        }

                        if (conditionMid != null && conditionMid(t.Text))
                        {
                            termsMids.Add(t);
                        }

                        if (conditionSuffix != null && conditionSuffix(t.Text))
                        {
                            termsSuffixes.Add(t);
                        }
                    }
                    else
                    {
                        break; //no more terms with needed field name
                    }
                } while (terms.Next());
            }

            prefixes = termsPrefixes.ToArray();
            mids = termsMids.ToArray();
            suffixes = termsSuffixes.ToArray();
        }
    }
}