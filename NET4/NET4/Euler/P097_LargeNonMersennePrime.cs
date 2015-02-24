using System;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P097_LargeNonMersennePrime : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            var m = 28433;
            var power = 7830457;
            Func<long, long> mod10B = n => n % 10000000000;
            long acc = 2;
            for (int i = 1; i < power; i++)
            {
                acc = mod10B(acc * 2);
            }

            acc = mod10B(28433 * acc) + 1;
            DebugFormat("raw res: {0}", acc);
        }
    }
}