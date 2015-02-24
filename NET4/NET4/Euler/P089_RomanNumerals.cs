using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P089_RomanNumerals : RunableBase
    {
        enum R
        {
            I = 1,
            V = 5,
            X = 10,
            L = 50,
            C = 100,
            D = 500,
            M = 1000,
        }

        [Run(1)]
        public void SolveIt()
        {
            var n = FromRoman(new[] { R.M, R.D, R.C, R.C, R.L, R.X, R.V, R.I });
        }

        static int FromRoman(IEnumerable<R> romanNumbers)
        {
            return 0;
        }

        static IEnumerable<R> ToRoman(int n)
        {
            return Enumerable.Empty<R>();
        }
    }
}