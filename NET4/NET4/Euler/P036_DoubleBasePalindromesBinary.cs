using System;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P036_DoubleBasePalindromesBinary : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            uint lower = 1;
            uint higher = 999999;

            long sum = 0;

            for (uint i = lower; i <= higher; i++)
            {
                bool isPalindrome = Common.IsPalindrome(i);

                if (isPalindrome)
                {
                    if (Common.IsBinaryPalindrome(i))
                    {
                        sum += i;
                        DebugFormat("dec={0} bin={1}, sum={2}", i, Convert.ToString(i,2), sum);
                    }
                }
            }

            DebugFormat("pal:{0}", sum);
        }
    }
}