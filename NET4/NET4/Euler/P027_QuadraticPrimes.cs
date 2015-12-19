using System;
using System.Linq;
using System.Threading.Tasks;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P027_QuadraticPrimes : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            Func<int, int, int, int> func = (n, a, b) => n * n + a * n + b;

            Func<int, int, Task<int>> countPrimeSeq = async (a, b) =>
            {
                int n = -1, f = 0;
                if (b < 3 || !Common.IsPrime(b)) return 0;
                while ((f = func(++n, a, b)) > 2 && Common.IsPrime(f)) ;
                return n;
            };

            //var rr = countPrimeSeq(1, 41);

            int primesQuantity = 0;
            int coeffA = 0;
            int coeffB = 0;

            object cmpLock = new object();

            System.Threading.Tasks.Parallel.ForEach(Enumerable.Range(1, 999), (i) =>
            {
                for (int j = 1; j < 1000; j++)
                {
                    int tmpA = 0, tmpB = 0;

                    var t1 = countPrimeSeq(i, j);
                    var t2 = countPrimeSeq(i, -j);
                    var t3 = countPrimeSeq(-i, j);
                    var t4 = countPrimeSeq(-i, -j);

                    var q1 = t1.Result;
                    var q2 = t2.Result;
                    var q3 = t3.Result;
                    var q4 = t4.Result;
                    var qMax = Math.Max(Math.Max(q1, q2), Math.Max(q3, q4));

                    if (q1 == qMax)
                    {
                        tmpA = i; tmpB = j;
                    }
                    else if (q2 == qMax)
                    {
                        tmpA = i; tmpB = -j;
                    }
                    else if (q3 == qMax)
                    {
                        tmpA = -i; tmpB = j;
                    }
                    else if (q4 == qMax)
                    {
                        tmpA = -i; tmpB = -j;
                    }

                    lock (cmpLock)
                    {
                        if (primesQuantity < qMax)
                        {
                            primesQuantity = qMax;
                            coeffA = tmpA;
                            coeffB = tmpB;
                            DebugFormat("n={0} a={1} b={2}", primesQuantity, tmpA, tmpB);
                        }
                    }
                }
            });

            DebugFormat("Product ab: {0}", coeffA * coeffB);
        }
    }
}