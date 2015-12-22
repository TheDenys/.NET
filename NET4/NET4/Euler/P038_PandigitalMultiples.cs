using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P038_PandigitalMultiples : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            long max = 0;

            for (int multiplierMax = 2; multiplierMax <= 9; multiplierMax++)
            {
                long[] numbers = new long[multiplierMax];

                for (int multiplicant = 1; multiplicant <= 10000; multiplicant++)
                {
                    numbers[0] = multiplicant;

                    for (int multiplier = 2; multiplier <= multiplierMax; multiplier++)
                    {
                        numbers[multiplier - 1] = multiplicant * multiplier;
                    }

                    var concatenatedProductDigits = numbers.SelectMany(Common.GetDigits).ToList();

                    if (concatenatedProductDigits.Count != 9 || concatenatedProductDigits.Contains(0) || concatenatedProductDigits.Distinct().Count() != 9)
                        continue;

                    long concatenatedProduct = Common.GetNumber(concatenatedProductDigits);

                    if (max < concatenatedProduct)
                        max = concatenatedProduct;

                    DebugFormat("multiplicant:{0} prod:{1}", multiplicant, concatenatedProduct);
                }
            }

            DebugFormat("max:{0}", max);
        }
    }
}