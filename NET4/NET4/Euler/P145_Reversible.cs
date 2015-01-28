using System;
using System.Linq;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P145_Reversible
    {
        [Run(0)]
        protected void FindReversible()
        {
            Func<int, int> getReversed = (int i) =>
            {
                int buf = 0;
                do buf = (buf * 10) + (i % 10); while ((i /= 10) >= 1);
                return buf;
            };

            Func<int, bool> areAllDigitsOdd = (int i) =>
            {
                while (((i % 10) % 2 != 0) && (i /= 10) >= 1) ;
                return i == 0;
            };

            Func<int, bool> isReversible = (int i) =>
            {
                if (i % 10 == 0) return false;
                return areAllDigitsOdd(i + getReversed(i));
            };

            int UPPER = 1000000000;
            //UPPER = 1000;

            int counter = Enumerable.Range(0, UPPER).AsParallel().Count(isReversible);

            ConsolePrint.print("count={0}", counter);
        }
     
    }
}