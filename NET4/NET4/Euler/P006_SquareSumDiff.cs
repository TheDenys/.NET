using System;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P006_SquareSumDiff : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            var n = 100;
            var res = (Math.Pow(n * (n + 1), 2) / 2 - (2 * Math.Pow(n, 3) + 3 * Math.Pow(n, 2) + n) / 3) / 2;
            DebugFormat("res({0}):{1}", n, res);
        }
    }
}