using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P255_HeronSquareRoot
    {

        [Run(0)]
        protected void GetSquare()
        {
            double lower = 10000000000000;//10^13
            double upper = 10000999999999;// ~500 000 ms
            //lower = 100000;
            //upper = 999999;


            Stopwatch sw = Stopwatch.StartNew();
            var r = RootIterations(lower, upper).AsParallel().Select(Root).Average();
            sw.Stop();


            var rounded = Math.Round(r, 10);
            ConsolePrint.print("avg:{0},\nrounded:{1},\nelapsed:{2}", r, rounded, sw.ElapsedMilliseconds);
        }

        private IEnumerable<double> RootIterations(double lower, double upper)
        {
            double current = lower;
            while (current <= upper) { yield return current++; }
        }

        private int Root(double n)
        {
            int itCount = 0;
            int d = GetDigitsCount(n);
            double xk1 = (d % 2 == 0) ? 7 * Math.Pow(10, (d - 2) / 2) : 2 * Math.Pow(10, (d - 1) / 2);
            double xk;

            do
            {
                xk = xk1;
                xk1 = Math.Floor((xk + Math.Ceiling(n / xk)) / 2);
                itCount++;
            } while (xk != xk1);

            return itCount;
        }

        private int GetDigitsCount(double i)
        {
            int count = 0;
            do { count++; } while ((i /= 10) >= 1);
            return count;
        }


    }
}