using System.Collections.Generic;
using System.Linq;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P032_ProductPandigital19 : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            ISet<int> prodSet = new SortedSet<int>();

            int multiplicant, multiplier, product;
            byte[] multiplicantDigits, multiplierDigits, prodDigits;

            for (multiplicant = 2; multiplicant <= 98; multiplicant++)
            {
                for (multiplier = 12; multiplier <= 10000; multiplier++)
                {
                    multiplicantDigits = Common.GetDigits(multiplicant);
                    multiplierDigits = Common.GetDigits(multiplier);
                    product = multiplicant * multiplier;
                    int prodDigitsCount = Common.CountDigits(product);
                    int digitsCount = multiplicantDigits.Length + multiplierDigits.Length + prodDigitsCount;

                    if (digitsCount != 9)
                        continue;

                    prodDigits = Common.GetDigits(product);

                    if (prodDigits.Where(d => d != 0).Distinct().Count() != 4)
                        continue;

                    var allUnion = multiplicantDigits.Union(multiplierDigits).Union(prodDigits).Where(d => d != 0).Distinct().ToList();

                    if (allUnion.Count == 9)
                    {
                        DebugFormat("{0} * {1} = {2}", multiplicant, multiplier, product);
                        prodSet.Add(product);
                    }
                }
            }

            long sum = prodSet.Sum();
            DebugFormat("sum:{0}", sum);
        }
    }
}