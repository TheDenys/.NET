using System;
using System.Collections.Generic;
using System.Numerics;

namespace NET4.Euler
{
    public static class Common
    {
        public static int CountDigits(long n)
        {
            return (int)Math.Log10(n) + 1;
        }

        public static int CountDigits(BigInteger n)
        {
            return (int)BigInteger.Log10(n) + 1;
        }

        public static byte[] GetDigits(long num)
        {
            var countDigits = CountDigits(num);
            var digits = new byte[countDigits];
            var p = digits.Length - 1;

            while (num > 0)
            {
                digits[p--] = (byte)(num % 10);
                num = num / 10;
            }

            return digits;
        }

        public static byte[] GetDigits(BigInteger num)
        {
            var countDigits = CountDigits(num);
            var digits = new byte[countDigits];
            var p = digits.Length - 1;

            while (num > 0)
            {
                BigInteger rem;
                num = BigInteger.DivRem(num, 10, out rem);
                digits[p--] = (byte)rem;
            }

            return digits;
        }

        public static long GetSumOfDigigits(BigInteger n)
        {
            long[] digits = new long[10];

            BigInteger divResult = n;
            BigInteger rem;

            do
            {
                divResult = BigInteger.DivRem(divResult, 10, out rem);
                digits[(int)rem]++;
            } while (divResult > 0);

            long sum = 0;

            for (int i = 1; i < 10; i++)
                sum += i * digits[i];

            return sum;
        }

        public static long GetNumber(IEnumerable<byte> digits)
        {
            long res = 0;

            foreach (var digit in digits)
            {
                res = res * 10 + digit;
            }

            return res;
        }

        public static IEnumerable<long> GetRotations(int n)
        {
            int c = CountDigits(n);
            byte[] digits = GetDigits(n);
            int shift = 0;

            while (++shift < c)
            {
                long res = 0;

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

        public static bool IsPermutation(long a, long b)
        {
            long[] digits = new long[10];

            do
            {
                digits[a % 10]++;
            } while ((a /= 10) > 0);

            do
            {
                digits[b % 10]--;
            } while ((b /= 10) > 0);

            for (int i = 0; i < 10; i++)
                if (digits[i] != 0)
                    return false;

            return true;
        }

        public static bool IsPrime(long i)
        {
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

        /// <summary>
        /// Returns prime numbers that are lower or equal to <paramref name="limit"/>.
        /// </summary>
        /// <param name="limit">The biggest number to check for primality.</param>
        /// <param name="seed">The lowest number to check for primality.</param>
        /// <returns>Collection of prime numbers.</returns>
        public static IEnumerable<long> GetPrimes(long limit, long seed = 2)
        {
            long n = seed;

            while (n <= limit)
            {
                if (Common.IsPrime(n))
                    yield return n;

                n++;
            }
        }

        public static long CountDivisors(long n, int divisorsLimit = -1)
        {
            if (n == 1)
                return 1;
            if (n == 2)
                return 2;
            if (n == 3)
                return 2;
            if (n == 4)
                return 3;
            if (n == 5)
                return 2;
            if (n == 6)
                return 4;
            if (n == 7)
                return 2;
            if (n == 8)
                return 4;
            if (n == 9)
                return 3;
            if (n == 10)
                return 4;
            if (n == 11)
                return 2;
            if (n == 12)
                return 6;

            long divisorsCount = 2;// 1 and itself

            if (n % 2 == 0)
                divisorsCount += 2;// 2 and half of itself

            if (n % 3 == 0)
                divisorsCount += 2;// 2 and half of itself

            if (n % 4 == 0)
                divisorsCount += 2;// 2 and half of itself

            //CancellationTokenSource cts = new CancellationTokenSource();

            //try
            //{
            //    System.Threading.Tasks.Parallel.For(3, n / 2 - 1, new ParallelOptions { CancellationToken = cts.Token },
            //        i =>
            //        {
            //            if (n % i == 0)
            //                Interlocked.Increment(ref divisorsCount);

            //            var buf = Interlocked.Read(ref divisorsCount);

            //            if (divisorsLimit != -1 && buf > divisorsLimit)
            //            {
            //                cts.Cancel();
            //            }
            //        });
            //}
            //catch (OperationCanceledException) { }

            //if (cts.IsCancellationRequested)
            //    return -1;

            var sqrt = Math.Sqrt(n);

            for (long i = 5; i <= sqrt; i++)
            {
                divisorsCount += (n % i == 0) ? (i != sqrt) ? 2 : 1 : 0;

                if (divisorsLimit != -1 && divisorsCount > divisorsLimit)
                    return -1;
            }

            return divisorsCount;
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

        public static BigInteger FactBig(int n)
        {
            if (n > 1)
            {
                return BigInteger.Multiply(n, FactBig(n - 1));
            }
            return BigInteger.One;
        }

        public static IEnumerable<int[]> CombinationsMN(int m, int n)
        {
            // sample m = 3, n = 7
            // 012 013 014 015 016 023 024 025 026 034 035 036 045 046 056
            // 123 124 125 126 134 135 136 145 146 156
            // 234 235 236 245 246 256
            // 345 346 356
            // 456

            var combinationsCount = CombinationsCount(m, n);
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

        public static long GetSumOf(long n)
        {
            return n * (n + 1) / 2;
        }

        public static BigInteger Fib(int n)
        {
            if (n == 1)
                return 1;

            if (n == 2)
                return 1;

            BigInteger fibN_2 = 1;
            BigInteger fibN_1 = 1;
            BigInteger fib = 0;

            for (int i = 3; i <= n; i++)
            {
                fib = fibN_1 + fibN_2;
                fibN_2 = fibN_1;
                fibN_1 = fib;
            }

            return fib;
        }

        /// <summary>
        /// Item1 is the biggest prime for making primoral.
        /// Item2 is the product of primes.
        /// Item3 is the totient function value.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Tuple<int, long, double> Primorial(int n)
        {
            int prime = 0;
            long product = 1;
            double phi = 1;

            for (int i = 2; i <= n; i++)
            {
                if (Common.IsPrime(i))
                {
                    prime = i;
                    product *= i;
                    phi *= 1d - 1d / i;
                }
            }

            phi *= product;

            return Tuple.Create(prime, product, phi);
        }

        public static long GCD(long a, long b)
        {
            long oldA, oldB;

            while (true)
            {
                if (b == 0)
                    return a;

                oldA = a;
                oldB = b;

                a = b;
                b = oldA % oldB;
            }
        }

        public static Trampoline<long> GCDTrampoline(long a, long b)
        {
            if (b == 0)
                return new Trampoline<long>(a);
            return new Trampoline<long>(() => GCDTrampoline(b, a % b));
        }

        public static IEnumerable<long> Range(long start, long count)
        {
            long n = start;
            long pos = 0;
            while (pos++ < count)
            {
                yield return n++;
            }
        }
    }

    public class Trampoline<T>
    {
        private readonly T value;
        private readonly Func<Trampoline<T>> continuation;

        public Trampoline(T value) { this.value = value; }
        public Trampoline(Func<Trampoline<T>> continuation) { this.continuation = continuation; }

        public T Value
        {
            get
            {
                Trampoline<T> val = this;

                while (val.continuation != null)
                {
                    val = val.continuation();
                }

                return val.value;
            }
        }
    }
}