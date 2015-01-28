using System;
using LightIndexer.Lucene;
using Lucene.Net.Documents;
using FIF = LightIndexer.Indexing.FileIndexingFields;

namespace LightIndexer.Indexing
{
    public class MockDataRetriever : IDataRetriever<Document>
    {
        private string[] items = {
                                     @"program.state",
                                     @"c:\path1\path1\folder1",
                                     @"c:\path1\path1\folder1",
                                     @"c:\path1\path1\folder1",
                                     @"c:\path1\path1\folder1",
                                     @"c:\path1\path1\folder1",

                                     @"c:\path1\path2\folder1",
                                     @"c:\path1\path2\folder1",
                                     @"c:\path1\path2\folder1",
                                     @"c:\path1\path2\folder1",
                                     @"c:\path1\path2\folder1",


                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     @"c:\path129\path1\folder1",
                                     
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",
                                     @"c:\path10\path1\folder1",

                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",
                                     @"c:\path11\path1\folder1",


                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                     @"c:\path12\path1\folder1",
                                 };

        private static readonly string FullName = FileIndexingFields.FullName.ToString();

        public int Count
        {
            get { return new Random().Next(20, 100); }
        }

        public Document GetItem(int rowIndex)
        {
            var fn = items[rowIndex % items.Length];
            var d = new Document();
            d.Add(FieldFactory.Keyword(FIF.FullName.F2S(), fn));
            d.Add(FieldFactory.Keyword(FIF.Extension.F2S(), fn));
            return d;
        }
    }
}