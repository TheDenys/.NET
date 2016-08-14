using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P024_LexicographicPermutations : RunableBase
    {
        [Run(0)]
        public void SolveIt()
        {
            var kthPerm = Common.KthPerm(999999, 10);
            DebugFormat("1 000 000 th perm of [0-9] is: {0}.", kthPerm);
        }
    }
}