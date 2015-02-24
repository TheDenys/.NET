using System;
using System.Collections.Generic;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P092_SquareDigitChains : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            Func<int, int> getSquareSumOfDigits = (n) =>
            {
                var digits = Common.GetDigits(n).ToList();
                var r = digits.Aggregate(0, (acc, el) => { return acc + (el * el); });
                return r;
            };

            Func<int, int, bool> unwind = (n, limit) =>
            {
                HashSet<int> hs = new HashSet<int>();
                int t = n;
                while (hs.Add(t = getSquareSumOfDigits(t)))
                    if (t == limit)
                        return true;
                return false;
            };

            int lim = 89;
            int end = 10000000;
            var res = Enumerable.Range(1, end).AsParallel().Select(i => unwind(i, lim)).Count(i => i);
            DebugFormat("sum: {0}", res);
        }


    }
}