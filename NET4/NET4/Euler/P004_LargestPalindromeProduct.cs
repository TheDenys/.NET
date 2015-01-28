using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.Euler
{
    [RunableClass]
    public class P004_LargestPalindromeProduct : RunableBase
    {
        [Run(0)]
        protected void SolveIt()
        {
            // 100 - 999
            uint lower = 100;
            uint higher = 999;

            uint maxPal = 0;

            for (uint i = higher; i >= lower; i--)
            {
                for (uint j = higher; j >= lower; j--)
                {
                    uint check = i * j;
                    bool isPalindrome = Common.IsPalindrome(check);

                    if (isPalindrome && maxPal < check)
                    {
                        maxPal = check;
                        DebugFormat("{0}", check);
                    }
                }
            }

            DebugFormat("pal:{0}", maxPal);
        }
    }
}