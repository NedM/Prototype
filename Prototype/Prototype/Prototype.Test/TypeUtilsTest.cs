using System;
using System.Diagnostics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prototype.Test
{
    [TestClass]
    public class TypeUtilsTest
    {
        [TestMethod]
        public void TestTryConvert_String()
        {
            string input = "This is a string";
            string expectedOutput = input;
            string result = TypeUtils.TryConvert<string>(input);
            TraceOutput(input, result, expectedOutput);
            result.Should().BeEquivalentTo(expectedOutput);

            input = string.Empty;
            expectedOutput = input;
            result = TypeUtils.TryConvert<string>(input, expectedOutput);
            TraceOutput(input, result, expectedOutput);
            result.Should().BeEquivalentTo(expectedOutput);

            input = null;
            expectedOutput = "n/a";
            result = TypeUtils.TryConvert<string>(null, "n/a");
            TraceOutput(input, result, expectedOutput);
            result.ShouldBeEquivalentTo(expectedOutput);
        }

        [TestMethod]
        public void TestTryConvert_Int()
        {
            string input = "1234";
            int expectedInt = 1234;
            int intResult = TypeUtils.TryConvert<int>(input);
            TraceOutput(input, intResult, expectedInt);
            intResult.ShouldBeEquivalentTo(expectedInt);

            input = "this is not an int";
            expectedInt = -1;
            intResult = TypeUtils.TryConvert<int>(input, -1);
            TraceOutput(input, intResult, expectedInt);
            intResult.ShouldBeEquivalentTo(expectedInt);

            input = "this is not an int";
            expectedInt = default(int);
            intResult = TypeUtils.TryConvert<int>(input);
            TraceOutput(input, intResult, expectedInt);
            intResult.ShouldBeEquivalentTo(expectedInt);
        }

        [TestMethod]
        public void TestTryConvert_NullableInt()
        {
            string input = "321";
            int expectedInt = 321;
            int? nullableIntResult = TypeUtils.TryConvert<int?>(input, null);
            TraceOutput(input, nullableIntResult, expectedInt);
            nullableIntResult.ShouldBeEquivalentTo(expectedInt);

            input = null;
            expectedInt = 100;
            nullableIntResult = TypeUtils.TryConvert<int?>(input, 100);
            TraceOutput(input, nullableIntResult, expectedInt);
            nullableIntResult.ShouldBeEquivalentTo(expectedInt);

            input = "THis is not an int";
            int? expectedNullableInt = default(int?);
            nullableIntResult = TypeUtils.TryConvert<int?>(input);
            TraceOutput(input, nullableIntResult, expectedNullableInt);
            nullableIntResult.ShouldBeEquivalentTo(expectedNullableInt);
        }

        [TestMethod]
        public void TestTryConvert_Decimal()
        {
            string input = "12.34";
            decimal expectedDecimal = 12.34m;
            decimal decResult = TypeUtils.TryConvert<decimal>(input);
            TraceOutput(input, decResult, expectedDecimal);
            decResult.ShouldBeEquivalentTo(expectedDecimal);

            input = "1111";
            expectedDecimal = 1111m;
            decResult = TypeUtils.TryConvert<decimal>(input);
            TraceOutput(input, decResult, expectedDecimal);
            decResult.ShouldBeEquivalentTo(expectedDecimal);

            input = "This is not a decimal";
            expectedDecimal = default(decimal);
            decResult = TypeUtils.TryConvert<decimal>(input);
            TraceOutput(input, decResult, expectedDecimal);
            decResult.ShouldBeEquivalentTo(expectedDecimal);
        }

        [TestMethod]
        public void TestTryConvert_Boolean()
        {
            string input = Boolean.TrueString;
            bool expectedBool = true;
            bool boolResult = TypeUtils.TryConvert<bool>(input, false);
            TraceOutput(input, boolResult, expectedBool);
            boolResult.ShouldBeEquivalentTo(expectedBool);

            input = "this is not a boolean";
            expectedBool = true;
            boolResult = TypeUtils.TryConvert<bool>(input, true);
            TraceOutput(input, boolResult, expectedBool);
            boolResult.ShouldBeEquivalentTo(expectedBool);

            input = "this is not a boolean";
            expectedBool = default(bool);
            boolResult = TypeUtils.TryConvert<bool>(input);
            TraceOutput(input, boolResult, expectedBool);
            boolResult.ShouldBeEquivalentTo(expectedBool);
        }

        [TestMethod]
        public void TestTryConvertPerformance()
        {
            double NUM_ITERATIONS = Math.Pow(10, 6);
            Stopwatch watch = new Stopwatch();

            Trace.WriteLine("Converting ints");
            watch.Start();
            for (double i = 0; i < NUM_ITERATIONS; i++)
            {
                int test = TypeUtils.TryConvert<int>("1234");
            }
            watch.Stop();
            Trace.WriteLine("Elapsed Time: " + watch.Elapsed.TotalSeconds);
            watch.Reset();

            Trace.WriteLine("Converting int?s");
            watch.Start();
            for (double i = 0; i < NUM_ITERATIONS; i++)
            {
                int? test = TypeUtils.TryConvert<int?>("1234");
            }
            watch.Stop();
            Trace.WriteLine("Elapsed Time: " + watch.Elapsed.TotalSeconds);
            watch.Reset();
        }

        private void TraceOutput(object input, object result, object expected)
        {
            string message =
                string.Format("Result of conversion of \"{0}\": \"{1}\". Expectation was \"{2}\". Match? {3}",
                              (null == input ? "NULL" : input.ToString()),
                              (null == result ? "NULL" : result.ToString()),
                              (null == expected) ? "NULL" : expected.ToString(),
                              ((result == null && expected == null) || result.Equals(expected))
                                  ? "Yes"
                                  : "No");
            Trace.WriteLine(message);
        }
    }
}
