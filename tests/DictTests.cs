using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using S397ConfigEditor;

namespace tests
{
    [TestClass]
    public class DictTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dict1 = new Dictionary<string, dynamic>()
        {
            { "A", 1 },
            { "B", "Hello" },
            { "C", 3.14 }
        };

            var dict2 = new Dictionary<string, dynamic>()
        {
            { "A", 1 },
            { "B", "World" },
            { "D", DateTime.Now }
        };

            var diff = WriteDict.GetDictionaryDifference(dict1, dict2);
            Assert.AreEqual(3, diff.Count);
        }
    }
}
