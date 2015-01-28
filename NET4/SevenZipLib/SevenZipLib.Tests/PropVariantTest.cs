using SevenZipLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace SevenZipLib.Tests
{


    /// <summary>
    ///This is a test class for PropVariantTest and is intended
    ///to contain all PropVariantTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PropVariantTest
    {
        private TestContext _context;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearTest()
        {
            PropVariant target = PropVariant.FromObject("Hello");
            Assert.AreNotEqual(VarEnum.VT_EMPTY, target.VarType);
            Assert.AreNotEqual(null, target.Value);
            target.Clear();
            Assert.AreEqual(VarEnum.VT_EMPTY, target.VarType);
            Assert.AreEqual(null, target.Value);
            target.Clear();
            Assert.AreEqual(VarEnum.VT_EMPTY, target.VarType);
            Assert.AreEqual(null, target.Value);
        }

        /// <summary>
        ///A test for SetBString
        ///</summary>
        [TestMethod()]
        public void SetBStringTest()
        {
            string expected = "Setting a BSTR";
            PropVariant target = new PropVariant();
            target.SetBString(expected);
            Assert.AreEqual(VarEnum.VT_BSTR, target.VarType);
            Assert.AreEqual(expected, target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetBool
        ///</summary>
        [TestMethod()]
        public void SetBoolTest()
        {
            bool[] expectedArray = { false, true };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetBool(expected);
                Assert.AreEqual(VarEnum.VT_BOOL, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_BOOL, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetBoolVector
        ///</summary>
        [TestMethod()]
        public void SetBoolVectorTest()
        {
            bool[] expected = { false, true, true, false, false };
            PropVariant target = new PropVariant();
            target.SetBoolVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_BOOL, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_BOOL, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetByte
        ///</summary>
        [TestMethod()]
        public void SetByteTest()
        {
            byte[] expectedArray = { byte.MinValue, 127, byte.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetByte(expected);
                Assert.AreEqual(VarEnum.VT_UI1, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_UI1, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetDateTime
        ///</summary>
        [TestMethod()]
        public void SetDateTimeTest()
        {
            DateTime[] expectedArray = { DateTime.Today, DateTime.Now };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetDateTime(expected);
                Assert.AreEqual(VarEnum.VT_FILETIME, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_FILETIME, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetDateTimeVector
        ///</summary>
        [TestMethod()]
        public void SetDateTimeVectorTest()
        {
            DateTime[] expected = { DateTime.Today, DateTime.Now };
            PropVariant target = new PropVariant();
            target.SetDateTimeVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_FILETIME, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_FILETIME, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetDecimal
        ///</summary>
        [TestMethod()]
        public void SetDecimalTest()
        {
            decimal[] expectedArray = { decimal.MinValue, decimal.Zero, decimal.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetDecimal(expected);
                Assert.AreEqual(VarEnum.VT_DECIMAL, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_DECIMAL, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetDouble
        ///</summary>
        [TestMethod()]
        public void SetDoubleTest()
        {
            double[] expectedArray = { double.MinValue, double.Epsilon, double.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetDouble(expected);
                Assert.AreEqual(VarEnum.VT_R8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_R8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetDoubleVector
        ///</summary>
        [TestMethod()]
        public void SetDoubleVectorTest()
        {
            double[] expected = { double.MinValue, double.Epsilon, double.MaxValue };
            PropVariant target = new PropVariant();
            target.SetDoubleVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_R8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_R8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetEmptyValue
        ///</summary>
        [TestMethod()]
        public void SetEmptyValueTest()
        {
            PropVariant target = PropVariant.FromObject("Hello");
            target.SetEmptyValue();
            Assert.AreEqual(VarEnum.VT_EMPTY, target.VarType);
            Assert.AreEqual(null, target.Value);
        }

        /// <summary>
        ///A test for SetIUnknown
        ///</summary>
        [TestMethod()]
        public void SetIUnknownTest()
        {
            string expected = "Hello World";
            PropVariant target = new PropVariant();
            target.SetIUnknown(expected);
            Assert.AreEqual(VarEnum.VT_UNKNOWN, target.VarType);
            Assert.AreEqual(expected, target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetInt
        ///</summary>
        [TestMethod()]
        public void SetIntTest()
        {
            int[] expectedArray = { int.MinValue, 798745651, int.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetInt(expected);
                Assert.AreEqual(VarEnum.VT_I4, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_I4, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetIntVector
        ///</summary>
        [TestMethod()]
        public void SetIntVectorTest()
        {
            int[] expected = { int.MinValue, 798745651, int.MaxValue };
            PropVariant target = new PropVariant();
            target.SetIntVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I4, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I4, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetLong
        ///</summary>
        [TestMethod()]
        public void SetLongTest()
        {
            long[] expectedArray = { long.MinValue, 3683673637637863783, long.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetLong(expected);
                Assert.AreEqual(VarEnum.VT_I8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_I8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetLongVector
        ///</summary>
        [TestMethod()]
        public void SetLongVectorTest()
        {
            long[] expected = { long.MinValue, 687258562786278622, long.MaxValue };
            PropVariant target = new PropVariant();
            target.SetLongVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetSByte
        ///</summary>
        [TestMethod()]
        public void SetSByteTest()
        {
            sbyte[] expectedArray = { sbyte.MinValue, 0, -45, 58, sbyte.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetSByte(expected);
                Assert.AreEqual(VarEnum.VT_I1, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_I1, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetSafeArray
        ///</summary>
        [TestMethod()]
        public void SetSafeArrayTest()
        {
            Array expected = new string[] { "Hello", "World", "One", "Two" };
            PropVariant target = new PropVariant();
            target.SetSafeArray(expected);
            Assert.AreEqual(VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetShort
        ///</summary>
        [TestMethod()]
        public void SetShortTest()
        {
            short[] expectedArray = { short.MinValue, -3445, 4898, short.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetShort(expected);
                Assert.AreEqual(VarEnum.VT_I2, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_I2, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetShortVector
        ///</summary>
        [TestMethod()]
        public void SetShortVectorTest()
        {
            short[] expected = { short.MinValue, -3445, 4898, short.MaxValue };
            PropVariant target = new PropVariant();
            target.SetShortVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I2, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_I2, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetString
        ///</summary>
        [TestMethod()]
        public void SetStringTest()
        {
            string expected = "Setting a BSTR";
            PropVariant target = new PropVariant();
            target.SetString(expected);
            Assert.AreEqual(VarEnum.VT_LPWSTR, target.VarType);
            Assert.AreEqual(expected, target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetStringVector
        ///</summary>
        [TestMethod()]
        public void SetStringVectorTest()
        {
            string[] expected = { "Hello", "World", "One", "Two" };
            PropVariant target = new PropVariant();
            target.SetStringVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetUInt
        ///</summary>
        [TestMethod()]
        public void SetUIntTest()
        {
            uint[] expectedArray = { uint.MinValue, 2727826, uint.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetUInt(expected);
                Assert.AreEqual(VarEnum.VT_UI4, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_UI4, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetUIntVector
        ///</summary>
        [TestMethod()]
        public void SetUIntVectorTest()
        {
            uint[] expected = { uint.MinValue, 2727826, uint.MaxValue };
            PropVariant target = new PropVariant();
            target.SetUIntVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI4, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI4, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetULong
        ///</summary>
        [TestMethod()]
        public void SetULongTest()
        {
            ulong[] expectedArray = { ulong.MinValue, 2727889278922326, ulong.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetULong(expected);
                Assert.AreEqual(VarEnum.VT_UI8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_UI8, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetULongVector
        ///</summary>
        [TestMethod()]
        public void SetULongVectorTest()
        {
            ulong[] expected = { ulong.MinValue, 2727289289287826, ulong.MaxValue };
            PropVariant target = new PropVariant();
            target.SetULongVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI8, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for SetUShort
        ///</summary>
        [TestMethod()]
        public void SetUShortTest()
        {
            ushort[] expectedArray = { ushort.MinValue, 9276, ushort.MaxValue };
            foreach (var expected in expectedArray)
            {
                PropVariant target = new PropVariant();
                target.SetUShort(expected);
                Assert.AreEqual(VarEnum.VT_UI2, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();

                target = PropVariant.FromObject(expected);
                Assert.AreEqual(VarEnum.VT_UI2, target.VarType);
                Assert.AreEqual(expected, target.Value);
                target.Clear();
            }
        }

        /// <summary>
        ///A test for SetUShortVector
        ///</summary>
        [TestMethod()]
        public void SetUShortVectorTest()
        {
            ushort[] expected = { ushort.MinValue, 9837, ushort.MaxValue };
            PropVariant target = new PropVariant();
            target.SetUShortVector(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI2, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();

            target = PropVariant.FromObject(expected);
            Assert.AreEqual(VarEnum.VT_VECTOR | VarEnum.VT_UI2, target.VarType);
            CollectionAssert.AreEqual(expected, (ICollection)target.Value);
            target.Clear();
        }

        /// <summary>
        ///A test for IsNullOrEmpty
        ///</summary>
        [TestMethod()]
        public void IsNullOrEmptyTest()
        {
            PropVariant target = new PropVariant();
            Assert.IsTrue(target.IsNullOrEmpty);
            target.VarType = VarEnum.VT_NULL;
            Assert.IsTrue(target.IsNullOrEmpty);
            target.SetInt(0);
            Assert.IsFalse(target.IsNullOrEmpty);
        }
    }
}