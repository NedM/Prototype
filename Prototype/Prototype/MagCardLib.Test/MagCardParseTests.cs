using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagCardLib.Test
{
    [TestClass]
    public class MagCardParseTests
    {
        [TestMethod]
        public void Parse()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.CARD_DATA);
            Assert.AreEqual(parsed.ToString(), MagCardTestData.CARD_DATA);
        }

        [TestMethod]
        public void Parse_Track2Only()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.Valid.Track2Only.MASTER_CARD);
            Assert.AreEqual(parsed.ToString(), MagCardTestData.Valid.Track2Only.MASTER_CARD);
        }
    }
}
