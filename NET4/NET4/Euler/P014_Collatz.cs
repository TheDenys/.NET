using System;
using System.Collections.Generic;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P014_Collatz : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            int longestChain = 0;
            int res = 0;

            for (int i = 3; i < 999999; i++)
            {
                int l;
                if ((l=Collatz(i).Count()) > longestChain)
                {
                    longestChain = l;
                    res = i;
                }
            }

            DebugFormat("maxL:{0} n={1}", longestChain, res);
        }

        protected IEnumerable<long> Collatz(long p)
        {
            long res = p;
            Func<long, long> fun = n => n % 2 == 0 ? n / 2 : 3 * n + 1;
            while ((res = fun(res)) != 1)
            {
                yield return res;
            }
            yield return 1;
        }
    }
}