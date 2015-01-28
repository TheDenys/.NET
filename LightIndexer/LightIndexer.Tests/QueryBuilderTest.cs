using System;
using System.Collections.Generic;
using LightIndexer.Lucene;
using Lucene.Net.Search;
using NUnit.Framework;

namespace LightIndexer.Tests
{
    [TestFixture]
    public class QueryBuilderTest
    {
        [Test]
        public void ParseSearchPath()
        {
            IList<string> contain = new List<string>();
            IList<string> wild = new List<string>();

            Func<string, Query> makeContainsFunc = (s) =>
            {
                contain.Add(s);
                return null;
            };

            Func<string, Query> makeWildcardFunc = (s) =>
            {
                wild.Add(s);
                return null;
            };

            QueryBuilder.ParseSearchPath("string;*wild;foobar;c:\\;xxx?;", makeContainsFunc, makeWildcardFunc);

            CollectionAssert.Contains(contain,"string");
            CollectionAssert.Contains(contain, "c:\\");
            CollectionAssert.Contains(contain, "foobar");
            CollectionAssert.Contains(wild, "*wild");
            CollectionAssert.Contains(wild, "xxx?");
        }
    }
}
