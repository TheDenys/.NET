using System;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P005_SmallestMultiple : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            var n = 20;
            var s = find(n);
            DebugFormat("res({0}):{1}", n, s);
        }

        long find(int n)
        {
            var lim = n%2 == 0 ? n + 1 : n;
            var firstDiv = (Int32)Math.Ceiling(lim / 2d);
            int[] dividers = Enumerable.Range(firstDiv, n - firstDiv).ToArray();

            long s = n;

            while (!dividers.All(d => s % d == 0))
            {
                s = s + n;
            }

            return s;
        }
    }
}