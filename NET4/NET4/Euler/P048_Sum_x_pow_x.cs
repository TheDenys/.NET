using System.Linq;
using System.Numerics;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P048_Sum_x_pow_x
    {

        [Run(0)]
        protected void GetSum()
        {
            var r = Enumerable.Range(1, 1000).Select((i => BigInteger.Pow(i, i))).Aggregate(BigInteger.Add);
            ConsolePrint.print("res: {0}", r.ToString());
        }

    }
}