using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P035_CircularPrimes : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            int sum = Enumerable.Range(2, 1000000)
                .AsParallel()
                .Count(i => Common.IsPrime(i) && Common.GetRotations(i).All(n => Common.IsPrime(n)));

            DebugFormat("sum:{0}", sum);//55
        }
    }
}