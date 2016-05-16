using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P021_AmicableNumbers : RunableBase
    {
        [Run(0)]
        public void Solve()
        {
            DebugFormat("amicable sum: {0}", SumAmicable(10000));
        }

        static int SumAmicable(int n)
        {
            return Enumerable
                .Range(2, n - 2)
                .AsParallel()
                .Where(a =>
                {
                    uint da = SumDivisors((uint)a); // d(220)=284, d(284)=220, a=220, b=284
                    uint db = SumDivisors(da);
                    return db == a && a != da;
                })
                .Sum();
        }

        static uint LeastPower(uint a, uint x)
        {
            uint b = a;
            while (x % b == 0) b *= a;
            return b;
        }

        static uint SumDivisors(uint x)
        {
            uint t = x;
            uint result = 1;

            //handle 2
            {
                uint p = LeastPower(2, t);
                result *= p - 1;
                t /= p / 2;
            }

            //handle odd factors
            for (uint i = 3; i * i <= t; i += 2)
            {
                uint p = LeastPower(i, t);
                result *= (p - 1) / (i - 1);
                t /= p / i;
            }

            //t is 1 or prime
            if (1 < t)
                result *= 1 + t;

            return result - x;
        }
    }
}