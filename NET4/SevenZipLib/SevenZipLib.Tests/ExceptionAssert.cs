using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace SevenZipLib.Tests
{
    /// <summary>
    /// Contains assertion types that are not provided with the standard MSTest assertions.
    /// </summary>
    public static class ExceptionAssert
    {
        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException.
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <param name="blockToExecute">The block of code to execute to generate the exception.</param>
        public static void Throws<T>(Action blockToExecute) where T : System.Exception
        {
            try
            {
                blockToExecute();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(T), "Expected exception of type " + typeof(T) + " but type of " + ex.GetType() + " was thrown instead.");
                return;
            }

            Assert.Fail("Expected exception of type " + typeof(T) + " but no exception was thrown.");
        }

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException.
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <param name="expectedMessage">The expected message of the exception.</param>
        /// <param name="blockToExecute">The block of code to execute to generate the exception.</param>
        public static void Throws<T>(string expectedMessage, Action blockToExecute) where T : System.Exception
        {
            try
            {
                blockToExecute();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(T), "Expected exception of type " + typeof(T) + " but type of " + ex.GetType() + " was thrown instead.");
                Assert.AreEqual(expectedMessage, ex.Message, "Expected exception with the message '" + expectedMessage + "' but exception with message of '" + ex.Message + "' was thrown instead.");
                return;
            }

            Assert.Fail("Expected exception of type " + typeof(T) + " but no exception was thrown.");
        }

        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type TException.
        /// </summary>
        /// <typeparam name="TException">The type of exception expected.</typeparam>
        /// <param name="expectedMessagePattern">The expected pattern of the message of the exception.</param>
        /// <param name="blockToExecute">The block of code to execute to generate the exception.</param>
        public static void RegexThrows<T>(string expectedMessagePattern, Action blockToExecute) where T : System.Exception
        {
            try
            {
                blockToExecute();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(T), "Expected exception of type " + typeof(T) + " but type of " + ex.GetType() + " was thrown instead.");
                Assert.IsTrue(Regex.IsMatch(ex.Message, expectedMessagePattern), "Expected exception message with the pattern '" + expectedMessagePattern + "' but exception with message of '" + ex.Message + "' was thrown instead.");
                return;
            }

            Assert.Fail("Expected exception of type " + typeof(T) + " but no exception was thrown.");
        }
    }
}
