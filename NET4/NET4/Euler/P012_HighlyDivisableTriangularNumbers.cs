using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P012_HighlyDivisableTriangularNumbers : RunableBase
    {
        [Run(0)]
        public void Solve()
        {
            long n = 1;
            long divisors = 0;

            do
            {
                var t = Common.GetSumOf(n);
                divisors = Common.CountDivisors(t);
                if (divisors >= 500)
                    DebugFormat("n: {0} t: {1} d: {2}", n, t, divisors);
                n++;
            } while (divisors < 500);
        }
    }
}