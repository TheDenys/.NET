using System.IO;
using Lucene.Net.Analysis;

namespace LightIndexer.Lucene
{
    public class WhitespaceAnalyzerLowerCase:Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new WhitespaceTokenizerLC(reader);
        }
    }
}