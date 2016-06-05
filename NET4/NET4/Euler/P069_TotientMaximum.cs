using System;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P069_TotientMaximum : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            int limit = 1000000;
            Tuple<int, long, double> maxTot = null;

            int i = 0;

            while (true)
            {
                var tmp = Common.Primorial(i++);

                if (tmp.Item2 > limit)
                    break;

                if (maxTot == null || maxTot.Item2 / maxTot.Item3 < tmp.Item2 / tmp.Item3)
                    maxTot = tmp;
            }

            DebugFormat("maxTot={0}", maxTot);
        }
    }
}