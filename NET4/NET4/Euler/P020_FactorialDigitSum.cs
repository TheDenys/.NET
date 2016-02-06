using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    public class P020_FactorialDigitSum : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            long s = Common.GetSumOfDigigits(Common.FactBig(100));
            DebugFormat("100!={0} DSum={1}", Common.FactBig(100), s);
        }
    }
}