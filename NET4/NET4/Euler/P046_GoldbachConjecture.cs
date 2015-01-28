using System.Collections.Generic;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P046_GoldbachConjecture : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            long odd = 7;

            while (true)
            {
                odd += 2;

                if (Common.IsPrime(odd))
                    continue;

                if (!IsSumPrimeAndSquare(odd))
                {
                    DebugFormat("n = {0}", odd);
                    //break;
                }

            }
        }

        protected bool IsSumPrimeAndSquare(long n)
        {
            foreach (long prime in GetPrimes())
            {
                if (prime > n)
                    break;

                if (Common.IsTwiceSquare(n - prime))
                    return true;
            }

            DebugFormat("!!! n={0}", n);
            return false;
        }

        IEnumerable<long> GetPrimes()
        {
            long n = 3;

            while (true)
            {
                if (Common.IsPrime(n))
                    yield return n;

                n++;
            }
        }
    }
}