using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace NET4.TestClasses
{
    [TestFixture]
    public class TestNUnit
    {

        [TestCase(1, 1, Result = 2)]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Test(Description = "Use TestCaseSource")]
        [TestCaseSource("TestCaseDataListForMultiply")]
        public int Multiply(int x, int y)
        {
            return x * y;
        }

        private IEnumerable<TestCaseData> TestCaseDataListForMultiply
        {
            get
            {
                yield return new TestCaseData(1, 1).Returns(1);
                yield return new TestCaseData(2, 2).Returns(4);
                yield return new TestCaseData(3, 3).Returns(9);
            }
        }


    }
}