using System;
using System.Collections.Generic;
using System.Threading;
using PDNUtils.Help;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P243_Resilience : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            double target = 15499d / 94744d;

            bool found = false;
            double maxResilience = 1;

            int primeBase = 1;

            List<Tuple<int, long, double>> values = new List<Tuple<int, long, double>>(2);

            // looking for the best candidate by producing primorals with their phi
            while (maxResilience > target)
            {
                var product = Common.Primorial(++primeBase);

                if (product.Item1 == primeBase)
                {
                    if (values.Count == 2) values.RemoveAt(0);
                    values.Add(product);

                    maxResilience = product.Item3 / (product.Item2 - 1);
                    DebugFormat("Res({0})={1}", product, maxResilience);
                }
            }

            // found the best candidate
            var bestCandidate = values[0];
            long next = bestCandidate.Item2;
            double phi = bestCandidate.Item3;

            // now looking for the actual result, multiplying the value by the smallest prime (2).
            while (true)
            {
                next *= 2;
                phi *= 2;
                maxResilience = phi / (next - 1);

                if (maxResilience < target)
                {
                    DebugFormat("Res({0})={1}", next, maxResilience);
                    break;
                }
            }
        }

        double Resilience(long d)
        {
            return ((double)EulerPhi(d) / (d - 1));
        }

        long EulerPhi(long n)
        {
            if (Common.IsPrime(n)) return n - 1;

            long phi = 1;

            Common.Range(2, n - 2).ForEachParallel((i) =>
            {
                if (Common.GCD(n, i) == 1)
                    Interlocked.Increment(ref phi);
            });

            return phi;
        }
    }
}