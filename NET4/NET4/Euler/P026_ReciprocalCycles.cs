using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P026_ReciprocalCycles : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            long limit = 999;

            for (long i = limit; i > 2; i--)
            {
                if (!Common.IsPrime(i))
                    continue;

                if (Test(i))
                {
                    DebugFormat("d = {0}", i, BigInteger.Divide(BigInteger.Pow(10, 100), i));
                    break;
                }
            }
        }

        bool Test(long d)
        {
            int period = 1;

            var ten = new BigInteger(10);
            var bigD = new BigInteger(d);

            while (BigInteger.ModPow(ten, period, bigD) != BigInteger.One)
            {
                period++;
            }

            if (period == d - 1)
            {
                return true;
            }

            return false;
        }
    }
}