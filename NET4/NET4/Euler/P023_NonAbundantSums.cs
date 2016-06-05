using System;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P023_NonAbundantSums : RunableBase
    {
        [Run(0)]
        public void Solve()
        {
            // inspired by
            //http://www.mathblog.dk/project-euler-23-find-positive-integers-not-sum-of-abundant-numbers/
            var theoreticalLimit = 28123;

            long[] primes = Common.GetPrimes((long)Math.Sqrt(theoreticalLimit)).ToArray();

            var abundants = Enumerable.Range(2, theoreticalLimit - 2)
                .Where(i => sumOfFactorsPrime(i, primes) > i)
                .ToList();

            // Make all the sums of two abundant numbers
            bool[] canBeWrittenasAbundent = new bool[theoreticalLimit + 1];
            for (int i = 0; i < abundants.Count; i++)
            {
                for (int j = i; j < abundants.Count; j++)
                {
                    if (abundants[i] + abundants[j] <= theoreticalLimit)
                    {
                        canBeWrittenasAbundent[abundants[i] + abundants[j]] = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Sum the numbers which are not sums of two abundant numbers
            var sum = canBeWrittenasAbundent.Select((canBe, pos) => canBe ? 0 : pos).Sum();
            DebugFormat("Non-abundant sums: {0}", sum);
        }

        private long sumOfFactorsPrime(int number, long[] primelist)
        {
            long n = number;
            long sum = 1;
            long p = primelist[0];
            long j;
            int i = 0;

            while (p * p <= n && n > 1 && i < primelist.Length)
            {
                p = primelist[i];
                i++;
                if (n % p == 0)
                {
                    j = p * p;
                    n = n / p;
                    while (n % p == 0)
                    {
                        j = j * p;
                        n = n / p;
                    }
                    sum = sum * (j - 1) / (p - 1);
                }
            }

            //A prime factor larger than the square root remains, so add that
            if (n > 1)
            {
                sum *= n + 1;
            }
            return sum - number;
        }
    }
}