using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace NET4.Euler.Tests
{
    [TestFixture]
    public class CommonTests
    {
        [Test]
        public void CombinationsTest()
        {
            var combinations = Common.CombinationsMN(3, 9);

            foreach (var combination in combinations)
            {
                Trace.WriteLine(string.Join("", combination));
            }
        }

        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 2)]
        [TestCase(4, Result = 3)]
        [TestCase(5, Result = 2)]
        [TestCase(6, Result = 4)]
        [TestCase(7, Result = 2)]
        [TestCase(8, Result = 4)]
        [TestCase(9, Result = 3)]
        [TestCase(10, Result = 4)]
        [TestCase(11, Result = 2)]
        [TestCase(12, Result = 6)]
        [TestCase(24, Result = 8)]
        [TestCase(30, Result = 8)]
        [TestCase(40, Result = 8)]
        [TestCase(42, Result = 8)]
        [TestCase(54, Result = 8)]
        [TestCase(56, Result = 8)]
        [TestCase(66, Result = 8)]
        [TestCase(70, Result = 8)]
        [TestCase(78, Result = 8)]
        [TestCase(88, Result = 8)]
        [TestCase(1000, Result = 16)]
        [TestCase(1001, Result = 8)]
        public long CountDivisorsTests(long n)
        {
            return Common.CountDivisors(n);
        }

        [TestCase(100, Result = 10)]
        [TestCase(1000, Result = 180)]
        [TestCase(1000000, Result = 224427)]
        [TestCase(5000000, Result = 1118592)]
        [TestCase(10000000, Result = 2228418)]
        public long CountNumbersWith8DivisorsTests(int n)
        {
            return Enumerable.Range(1, n).Count(i => Common.CountDivisors(i, 8) == 8);
        }

        [TestCase(1000000000000, Result = -1)]
        public long CountDivisors8Tests(long n)
        {
            return Common.CountDivisors(n, 8);
        }

        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 1)]
        [TestCase(3, Result = 2)]
        [TestCase(4, Result = 3)]
        [TestCase(5, Result = 5)]
        [TestCase(6, Result = 8)]
        [TestCase(7, Result = 13)]
        public int Fib(int n)
        {
            return (int)Common.Fib(n);
        }

        [TestCase(3, Result = 12)]
        [TestCase(1000, Result = 4782)]
        public int CountFibDigits(int count)
        {
            int digitsCount = 0;
            int n = 0;

            do
            {
                n++;
                digitsCount = Common.CountDigits(Common.Fib(n));
            } while (digitsCount < count);

            Trace.WriteLine("n=" + n);

            return n;
        }
    }
}