using System;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P70_TotientPermutation : RunableBase
    {
        [Run(1)]
        public void SolveIt()
        {
            int limit = 10000000;

            int midRange = (int)Math.Sqrt(limit);
            int rangePercent = 30;
            int rangeDelta = midRange * rangePercent / 100;

            long[] primes = Common.Range(midRange - rangeDelta, 2 * rangeDelta).Where(Common.IsPrime).ToArray();

            long res = primes[0];
            long phiRes = res - 1;
            double minRatio = double.MaxValue;

            for (int i = 0; i < primes.Length; i++)
            {
                for (int j = 0; j < primes.Length; j++)
                {
                    long n = primes[i] * primes[j];

                    if (n > limit)
                        break;

                    // per wiki https://en.wikipedia.org/wiki/Euler%27s_totient_function
                    // totient function is multiplicative for co-primes
                    // and for prime p: phi(p^k)=p^k(1 - 1/p), hence phi(p)=p-1
                    long phi = (primes[i] - 1) * (primes[j] - 1);

                    if (IsPermutation(n, phi))
                    {
                        var ratio = (double)n / phi;

                        if (minRatio > ratio)
                        {
                            minRatio = ratio;
                            res = n;
                            phiRes = phi;
                        }
                    }
                }
            }

            DebugFormat("n={0}, phi(n)={1}, n/phi(n)={2}", res, phiRes, minRatio);
        }

        bool IsPermutation(long a, long b)
        {
            long[] digits = new long[10];

            do
            {
                digits[a % 10]++;
            } while ((a /= 10) > 0);

            do
            {
                digits[b % 10]--;
            } while ((b /= 10) > 0);

            for (int i = 0; i < 10; i++)
                if (digits[i] != 0)
                    return false;

            return true;
        }
    }
}