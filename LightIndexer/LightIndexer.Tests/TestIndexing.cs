using System;
using LightIndexer.Config;
using LightIndexer.Indexing;
using LightIndexer.Lucene;
using LightIndexer.Lucene.Search;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using NUnit.Framework;

namespace LightIndexer.Tests
{
    [TestFixture]
    public class TestIndexing
    {
        private IndexReader reader;
        private IndexSearcher searcher;
        private string _ContentFieldName = FileIndexingFields.Content.F2S();

        [TestFixtureSetUp]
        public void SetUp()
        {
            Directory dir = new RAMDirectory();
            using (
                var writer = new IndexWriter(dir, new IndexManager(DirectoryType.RAM, "").GetAnalyzer(), true,
                    IndexWriter.MaxFieldLength.LIMITED))
            {

                //test file
                const string testcontent1 = "ababagalamama abracadabra chubacabras <taG attRibute=\"attrValue\">";

                //add doc and field
                var doc = new Document();
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.FullName, "c:\\filename.xxx"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.Path, "c:\\"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.NameWithExtension,
                    "filename.xxx"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.NameWithoutExtension,
                    "filename"));
                doc.Add(FieldFactory.BuildField(IndexingType.Text, FileIndexingFields.Content, testcontent1));

                writer.AddDocument(doc);

                const string testcontent2 = "sovsem drugoy content kontingent abracadabra";

                doc = new Document();
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.FullName, "d:\\filename.xxx"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.Path, "d:\\"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.NameWithExtension,
                    "filename.xxx"));
                doc.Add(FieldFactory.BuildField(IndexingType.Keyword, FileIndexingFields.NameWithoutExtension,
                    "filename"));
                doc.Add(FieldFactory.BuildField(IndexingType.Text, FileIndexingFields.Content, testcontent2));

                writer.AddDocument(doc);

                writer.Optimize();
            }

            reader = IndexReader.Open(dir, true);
            searcher = new IndexSearcher(reader);

            var docnum = reader.NumDocs();
            Console.Out.WriteLine("docnum = {0}", docnum);
            Assert.That(docnum == 2);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }

        [Test]
        [TestCase("chubacabras", Result = 1)]
        [TestCase("abracadabra", Result = 2)]
        public int TestIndexSearch(string s)
        {
            var term = new Term(_ContentFieldName, s);
            var query = new TermQuery(term);

            TopDocs topDocs = searcher.Search(query, null, 2);
            return topDocs.ScoreDocs.Length;
        }

        [Test]
        public void TestPhraseSearch()
        {
            var term1 = new Term(_ContentFieldName, "abracadabra");
            var term2 = new Term(_ContentFieldName, "chubacabras");
            var query = new PhraseQuery();
            query.Add(term1);
            query.Add(term2);

            TopDocs topDocs = searcher.Search(query, null, 1);
            Assert.That(topDocs.ScoreDocs.Length == 1);
        }

        [Test]
        public void TestGapPhraseSearch()
        {
            var term1 = new Term(_ContentFieldName, "ababagalamama");
            var term2 = new Term(_ContentFieldName, "chubacabras");
            var query = new PhraseQuery();
            query.Add(term1);
            query.Add(term2);
            query.Slop = 1;

            TopDocs topDocs = searcher.Search(query, null, 1);
            Assert.That(topDocs.ScoreDocs.Length == 1);
        }

        [Test]
        public void TestReorderedPhraseSearch()
        {
            var term1 = new Term(_ContentFieldName, "chubacabras");
            var term2 = new Term(_ContentFieldName, "ababagalamama");
            var query = new PhraseQuery();
            query.Add(term1);
            query.Add(term2);
            query.Slop = 3;

            TopDocs topDocs = searcher.Search(query, null, 1);
            Assert.That(topDocs.ScoreDocs.Length == 1);
        }

        //[Test]
        //[Ignore]
        //public void TestCustomFilter()
        //{
        //        var path = "c:\\";
        //        var content = "abracadabra";
        //        var term = new Term(_ContentFieldName, content);
        //        var query = new TermQuery(term);

        //        Filter filter = new CustomFilter();

        //        TopDocs topDocs = searcher.Search(new MatchAllDocsQuery(), filter, reader.NumDocs());
        //        var x = topDocs.ScoreDocs.Length;
        //}

        //[Test]
        //public void TestQueryFilter()
        //{
        //    var path = "c:\\";
        //    var content = "abracadabra";
        //    var term = new Term(_ContentFieldName, content);
        //    var query = new TermQuery(term);

        //    QueryF
        //    Filter filter = ;
        //    FilteredQuery fq = new FilteredQuery(query, filter);

        //    TopDocs topDocs = searcher.Search(query, null, 2);
        //    return topDocs.scoreDocs.Length;
        //}

    }
}