using System;
using System.Collections.Generic;
using System.Linq;

namespace McLarenTask
{
    /// <summary>
    /// Class which looks for palindromes in text
    /// </summary>
    public class PalindromeSeeker
    {
        private readonly string text;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="text">Text which may contain palindromes</param>
        public PalindromeSeeker(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            this.text = text;
        }

        /// <summary>
        /// Gets not more than top N longest palindromes from text sorted by length descending
        /// </summary>
        /// <param name="n">Limit</param>
        /// <returns>Collection of palindromes</returns>
        public IEnumerable<Palindrome> GetFirstNLongestPalindromes(int n)
        {
            return GetPalindromes().OrderByDescending(p => p.Length).Take(n);
        }

        /// <summary>
        /// Finds all palindromes in text
        /// </summary>
        /// <returns>Collection of palindromes</returns>
        public IEnumerable<Palindrome> GetPalindromes()
        {
            string s = text;

            // in loop we look for signs of palindrome (2 same letters in a row ('aa','bb',etc) or 2 same letters split by anything ('oko','ana', etc.))
            for (int pos = 1; pos < (s.Length - 1); pos++)
            {
                char prev = s[pos - 1];
                char current = s[pos];
                char next = s[pos + 1];

                Palindrome res = null;

                int length = 0;

                if (prev == current)// even length palindrome
                {
                    res = Search(s, pos, true);
                    length = res.Length;
                }

                if (prev == next)// odd length palindrome
                {
                    var resTmp = Search(s, pos, false);

                    if (resTmp.Length > length)// let's return longer palindrome, e.g. "afffb" -> "fff" not "ff"
                        res = resTmp;
                }

                if (res != null)
                    yield return res;
            }
        }

        // here we know the middle of palindrome and if it's even or odd
        // so we're trying to find the longest substring which is a palindrome
        // input:       ff
        // 1st loop:   offo
        // 2d loop:   coffoc
        // etc.
        // For even palindrom pos points to start of second half of palindrome, for odd to the exact middle
        // coffoc  tanat
        //    ^      ^
        //
        // that's why initial prevPos = pos - (even ? 2 : 1) and nextPos = pos + 1
        //
        // coffoc  tanat
        //  ^  ^    ^ ^
        // all next loops can just move pointers with step 1:
        //
        // coffoc  tanat
        // ^    ^  ^   ^
        Palindrome Search(string s, int pos, bool even)
        {
            int length = even ? 2 : 3;// minimum length of even palindrome is 2, odd 3
            string substring = s.Substring(pos - 1, length);// save current substring as next characters may be not equal
            // set initial prevPos and nextPos according to even/odd
            int prevPos = pos - (even ? 2 : 1);
            int nextPos = pos + 1;
            int prevPosOut = prevPos;

            // positions have to be within string and characters have to be equal to satisfy palindrome rule
            while ((prevPos >= 0) && (nextPos <= s.Length - 1) && s[prevPos] == s[nextPos])
            {
                length += 2;// we move 1 character left and 1 right which makes 2
                substring = s.Substring(prevPos, nextPos - prevPos + 1);// let's remember it as next characters may be not equal
                prevPosOut = prevPos--;// remember the prevPos before decrementing (moving left) it as it will be used as palindrome index
                nextPos++;//incrementing next position (moving right)
            }

            return new Palindrome(substring, prevPosOut);
        }
    }
}