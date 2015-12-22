using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class Unsolved_P443_GCD_sequence : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            long a = 24, b = 54;
            //var gcd = GCD(a, b).Value;
            //DebugFormat("gcd({0},{1})={2}", a, b, gcd);
            long n = 1000;
            var g = G(n).Value;
            DebugFormat("g({0})={1}", n, g);
        }

        private Trampoline<long> G(long n)
        {
            if (n == 4)
                return new Trampoline<long>(13);
            return new Trampoline<long>(G(n - 1).Value + Common.GCD(n, G(n - 1).Value));
        }
    }

}