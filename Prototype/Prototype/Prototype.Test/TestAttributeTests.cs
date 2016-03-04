using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prototype.Test
{
    [TestClass]
    public class TestAttributeTests
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Trace.WriteLine("AssemblyInit operational.");
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Trace.WriteLine("ClassInit is a go.");
        }

        [TestInitialize]
        public void TestInit()
        {
            Trace.WriteLine("TestInit is status green.");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Trace.WriteLine("TestCleanup online.");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Trace.WriteLine("ClassCleanup is receiving you 5 by 5");
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Trace.WriteLine("AssemblyCleanup signing off...");
        }

        [TestMethod]
        public void TestMethod0()
        {
            Trace.WriteLine("This is TestMethod0 to ground control...");
        }

        [TestMethod]
        public void TestMethod1()
        {
            Trace.WriteLine("This is TestMethod1 checking in...");
        }

    }
}
