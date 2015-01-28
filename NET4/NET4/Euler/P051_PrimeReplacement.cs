using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P051_PrimeReplacement : RunableBase
    {
        private IDictionary<Tuple<int, int>, IEnumerable<int[]>> combiCache = new Dictionary<Tuple<int, int>, IEnumerable<int[]>>();
        private ISet<long> primeSet = new SortedSet<long>();

        [Run(0)]
        protected void SolveIt()
        {
            long n = 10;
            int cnt = 8;

            while (!GetReplacements(n++, cnt)) ;
        }

        private bool GetReplacements(long n, int targetCount)
        {
            // split n

            if (!CachedPrime(n))
                return false;

            var nums = n.ToString(CultureInfo.InvariantCulture).ToCharArray();
            var tmp = new char[nums.Length];
            var maxAsteriskCount = nums.Length - 1;
            List<long> smallestRepl = new List<long>(10);

            for (int asterisks = 1; asterisks <= maxAsteriskCount; asterisks++)
            {
                var comb = CombinationsMNCached(asterisks, nums.Length);

                foreach (var positions in comb)
                {
                    smallestRepl.Clear();
                    int countPrime = 0;

                    foreach (char c in Enumerable.Range(48, 10))
                    {
                        Array.Copy(nums, tmp, nums.Length);

                        foreach (var position in positions)
                        {
                            tmp[position] = c;
                        }

                        int repl = Convert.ToInt32(new String(tmp));

                        if (repl >= n)
                        {
                            if (CachedPrime(repl))
                            {
                                smallestRepl.Add(repl);
                                countPrime++;
                                //DebugFormat("replacement: {0}", repl);
                            }
                        }
                    }

                    //DebugFormat("countprime:{0}", countPrime);

                    if (countPrime == targetCount)
                    {
                        DebugFormat("n={0} target={1} positions:[{2}] smallest={3}", n, targetCount, string.Join(",", positions), smallestRepl.Min());
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerable<int[]> CombinationsMNCached(int m, int n)
        {
            var key = Tuple.Create(m, n);

            if (combiCache.ContainsKey(key))
                return combiCache[key];

            var combinationsMn = Common.CombinationsMN(m, n);
            combiCache.Add(key, combinationsMn);
            return combinationsMn;
        }

        private bool CachedPrime(long n)
        {
            if (primeSet.Contains(n))
                return true;

            if (Common.IsPrime(n))
            {
                primeSet.Add(n);
                return true;
            }

            return false;
        }
    }
}