using Lucene.Net.Analysis;

namespace LightIndexer.Lucene
{
    public class WhitespaceTokenizerLC : CharTokenizer
	{
		/// <summary>Construct a new WhitespaceTokenizer. </summary>
        public WhitespaceTokenizerLC(System.IO.TextReader in_Renamed)
            : base(in_Renamed)
		{
		}
		
		/// <summary>Collects only characters which do not satisfy
		/// {@link Character#isWhitespace(char)}.
		/// </summary>
		protected override bool IsTokenChar(char c)
		{
			return !System.Char.IsWhiteSpace(c);
		}

        /// <summary>Collects only characters which satisfy
        /// {@link Character#isLetter(char)}.
        /// </summary>
        protected override char Normalize(char c)
        {
            return System.Char.ToLowerInvariant(c);
        }

	}
}