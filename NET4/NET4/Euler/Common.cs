using System;
using System.Collections.Generic;

namespace NET4.Euler
{
    public static class Common
    {
        public static int CountDigits(long n)
        {
            return (int)Math.Log10(n) + 1;
        }

        public static int[] GetDigits(int num)
        {
            var countDigits = CountDigits(num);
            int[] digits = new int[countDigits];
            int p = digits.Length - 1;

            while (num > 0)
            {
                digits[p--] = num % 10;
                num = num / 10;
            }

            return digits;
        }

        public static long GetNumber(IEnumerable<int> digits)
        {
            int res = 0;

            foreach (var digit in digits)
            {
                res = res * 10 + digit;
            }

            return res;
        }

        public static IEnumerable<int> GetRotations(int n)
        {
            int c = CountDigits(n);
            int[] digits = GetDigits(n);
            int shift = 0;

            while (++shift < c)
            {
                int res = 0;

                for (int i = shift; i < c + shift; i++)
                {
                    res = res * 10 + digits[i % c];
                }

                yield return res;
            }
        }

        public static bool IsPalindrome(uint n)
        {
            long num = n, rev = 0;

            while (num > 0)
            {
                var dig = num % 10;
                rev = rev * 10 + dig;
                num = num / 10;
            }

            return n == rev;
        }

        public static bool IsPrime(long i)
        {
            //TODO: get from pdf for task
            if (i == 1) return false;
            if (i < 4) return true;
            if (i % 2 == 0) return false;
            if (i < 9) return true;
            if (i % 3 == 0) return false;

            for (long f = 5; f <= Math.Sqrt(i); f += 6)
            {
                if ((i % f == 0) || (i % (f + 2)) == 0)
                    return false;
            }

            return true;
        }

        public static uint BinReverse(uint n)
        {
            uint v = n;

            // swap odd and even bits
            v = ((v >> 1) & 0x55555555) | ((v & 0x55555555) << 1);
            // swap consecutive pairs
            v = ((v >> 2) & 0x33333333) | ((v & 0x33333333) << 2);
            // swap nibbles ... 
            v = ((v >> 4) & 0x0F0F0F0F) | ((v & 0x0F0F0F0F) << 4);
            // swap bytes
            v = ((v >> 8) & 0x00FF00FF) | ((v & 0x00FF00FF) << 8);
            // swap 2-byte long pairs
            v = (v >> 16) | (v << 16);

            int trail = CountZeroBitsTrailing(v);

            v >>= trail;

            return v;
        }

        public static bool IsBinaryPalindrome(uint n)
        {
            return (CountZeroBitsTrailing(n) == 0) && n == BinReverse(n);
        }

        public static bool IsTwiceSquare(long n)
        {
            if (n % 2 != 0) return false;
            var sqrt = Math.Sqrt(n / 2);
            if (Math.Truncate(sqrt).CompareTo(sqrt) != 0)
                return false;
            return true;
        }

        internal static int CountZeroBitsTrailing(uint n)
        {
            if (n == 0)
                return sizeof(int);

            uint v = (uint)n;
            int c = 0;
            v = (v ^ (v - 1)) >> 1;

            for (c = 0; v != 0; c++)
            {
                v >>= 1;
            }

            return c;
        }

        public static int CombinationsCount(int m, int n)
        {
            return (int)(Fact(n) / (Fact(m) * Fact(n - m)));
        }

        public static long Fact(int n)
        {
            if (n > 1)
            {
                return n * Fact(n - 1);
            }
            return 1;
        }

        public static IEnumerable<int[]> CombinationsMN(int m, int n)
        {
            // sample n = 7, m =3
            // 012 013 014 015 016 023 024 025 026 034 035 036 045 046 056
            // 123 124 125 126 134 135 136 145 146 156
            // 234 235 236 245 246 256
            // 345 346 356
            // 456

            var combinationsCount = Common.CombinationsCount(m, n);
            //DebugFormat("combinations: {0}", combinations);

            var posArr = new int[m];
            List<int[]> prod = new List<int[]>(combinationsCount);

            for (int i = 0; i < m; i++)
            {
                getLevel(posArr, prod, n, m, i);
            }

            //DebugFormat("count: {0}", prod.Count);
            return prod;
        }

        private static void getLevel(int[] posArr, IList<int[]> prod, int n, int m, int level)
        {
            for (posArr[level] = level > 0 ? posArr[level - 1] + 1 : 0; posArr[level] <= n - m + level; posArr[level]++)
            {
                if (level == m - 1)
                {
                    int[] positions = new int[m];
                    Array.Copy(posArr, positions, m);
                    prod.Add(positions);
                    //DebugFormat("posArr:{0}", string.Join(" ", posArr));
                }
                else if (level < m - 1)
                {
                    getLevel(posArr, prod, n, m, level + 1);
                }
            }
        }

        // http://bigintegers.blogspot.nl/2014/01/the-kth-permutation.html
        public static uint[] KthPerm(uint k, uint n)  //  k=0,n=3 ==> {0,1,2}
        {                                               //  k=1,n=3 ==> {0,2,1}
            if (n == 0) return new uint[] { };
            uint[] perm = new uint[n++];
            uint[] fdic = new uint[n];                  //  in factoradic form, no factorials!
            uint i = 1, d, m = n - 1;
            do
            {
                d = k / i;
                fdic[m--] = k - d * i++;
                k = d;
            }
            while (m != ~0u);
            for (k = n-- - 2; k != ~0u; k--)
            {
                m = perm[k++] = fdic[k];
                for (i = k--; i < n; i++) if (perm[i] >= m) perm[i]++;
            }
            return perm;
        }

        public static uint[] NextPerm(uint[] perm)    //  {0,1,2} ==> {0,2,1}
        {
            int len = perm.Length - 1;
            if (len <= 0) return perm;
            int i = len;
            while (i > 0 && perm[i--] <= perm[i]) ;
            uint swap = perm[i];
            int j = len;
            while (j > 0 && perm[j] <= swap) j--;
            if (j == 0)
                do
                {
                    swap = perm[len];
                    perm[len--] = perm[j];
                    perm[j++] = swap;
                }
                while (j < len);
            else
            {
                perm[i++] = perm[j];
                perm[j] = swap;
                while (len > i)
                {
                    swap = perm[len];
                    perm[len--] = perm[i];
                    perm[i++] = swap;
                }
            }
            return perm;
        }
    }
}