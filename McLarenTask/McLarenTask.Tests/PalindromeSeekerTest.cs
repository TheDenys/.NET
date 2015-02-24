using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace McLarenTask.Tests
{
    [TestFixture]
    public class PalindromeSeekerTest
    {
        [TestCaseSource("PalindromesSource")]
        public void Find(string s, IEnumerable<Palindrome> expectedPalindromes)
        {
            int expectedCount = expectedPalindromes.Count();

            PalindromeSeeker ps = new PalindromeSeeker(s);
            var palindromes = ps.GetFirstNLongestPalindromes(expectedCount).ToList();

            Assert.AreEqual(expectedCount, palindromes.Count(), "results count mismatch");

            CollectionAssert.AreEqual(expectedPalindromes, palindromes);
        }

        IEnumerable<TestCaseData> PalindromesSource()
        {
            yield return new TestCaseData(
                "sqrrqabccbatudefggfedvwhijkllkjihxymnnmzpop",
                new[]
                {
                    new Palindrome("hijkllkjih",23),
                    new Palindrome("defggfed",13),
                    new Palindrome("abccba",5),
                });
            yield return new TestCaseData(
                "cofffoc",
                new[]
                {
                    new Palindrome("cofffoc",0),
                }).SetName("odd length palindrome");
            yield return new TestCaseData(
                "coffffoc",
                new[]
                {
                    new Palindrome("coffffoc",0),
                }).SetName("even length palindrome");
            yield return new TestCaseData(
                "",
                Enumerable.Empty<Palindrome>()).SetName("empty input string");
            yield return new TestCaseData(
                null,
                Enumerable.Empty<Palindrome>()).Throws(typeof(ArgumentNullException)).SetName("null input string");
        }
    }
}