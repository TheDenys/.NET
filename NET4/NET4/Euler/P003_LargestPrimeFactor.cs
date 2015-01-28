using System;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P003_LargestPrimeFactor : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            long n = 600851475143;

            var d = Lpf(n).Value;

            DebugFormat("lpf({0})={1}", n, d);
        }

        protected Trampoline<long> Lpf(long n)
        {
            Func<long, bool> prime = (_l) => Common.IsPrime(_l);

            for (int i = 2; i < n; i++)
            {
                long l = n % i;
                if (l == 0 && prime(l))
                {
                    return new Trampoline<long>(() => Lpf(n/i));
                }

            }

            return new Trampoline<long>(n);
        }
    }
}