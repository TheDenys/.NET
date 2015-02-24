using System;
using System.Globalization;

namespace McLarenTask
{
    /// <summary>
    /// Represents a string palindrome
    /// </summary>
    public class Palindrome : IEquatable<Palindrome>
    {
        private readonly string text;
        private readonly int index;

        /// <summary>
        /// Palindrome itself
        /// </summary>
        public string Text { get { return text; } }

        /// <summary>
        /// Palindrome index within input text
        /// </summary>
        public int Index { get { return index; } }

        /// <summary>
        /// Palindrome length
        /// </summary>
        public int Length { get { return text.Length; } }

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="text">Text of palindrome</param>
        /// <param name="index">Index in text</param>
        public Palindrome(string text, int index)
        {
            if (text == null) throw new ArgumentNullException("text");
            if (index < 0) throw new ArgumentOutOfRangeException("index", "Must be equal or greater than zero.");

            this.text = text;
            this.index = index;
        }

        /// <summary>
        /// Copy ctor.
        /// </summary>
        /// <param name="p">Palindrome instance.</param>
        public Palindrome(Palindrome p) : this(p.Text, p.Index) { }

        public override bool Equals(object obj)
        {
            Palindrome p = obj as Palindrome;

            if (p != null)
                return Equals(p);

            return false;
        }

        public bool Equals(Palindrome other)
        {
            if (other == null)
                return false;

            bool equals = object.Equals(this.Text, other.Text) && object.Equals(this.Index, other.Index);
            return equals;
        }

        public override int GetHashCode()
        {
            return text.GetHashCode() ^ index;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Text: {0}, Index: {1}, Length: {2}", Text, Index, Length);
        }
    }
}