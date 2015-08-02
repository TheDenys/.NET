using System.Collections.Generic;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class NotSolved_P501_EightDivisors : RunableBase
    {
        private int targetDivisors = 8;
        private long n = 1000000000000;

        [Run(0)]
        protected void SolveIt()
        {
            DebugFormat("n={0}", n);
            //n = 10000000;
            var count = GetRange(1, n).AsParallel().Count(i => Common.CountDivisors(i, targetDivisors) == targetDivisors);
            DebugFormat("n={0} Count={1}", n, count);
        }

        IEnumerable<long> GetRange(long start, long count)
        {
            while (count-- > 0)
                yield return start++;
        }
    }
}