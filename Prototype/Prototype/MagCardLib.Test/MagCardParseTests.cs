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
            Assert.AreEqual(String.Compare(parsed.ToString(), MagCardTestData.CARD_DATA, StringComparison.Ordinal), 0);
        }

        [TestMethod]
        public void Parse_Track2Only_MasterCard()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.Valid.Track2Only.MASTER_CARD);
            Assert.AreEqual(
                String.CompareOrdinal(
                    parsed.Track2DataToString(),
                    MagCardTestData.Valid.Track2Only.MASTER_CARD.Substring(
                        0,
                        MagCardTestData.Valid.Track2Only.MASTER_CARD.Length - 1)), //Need to trim off the trailing '3' which is not track 2 data
                0);
        }

        [TestMethod]
        public void Parse_Track2Only_Other()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.Valid.Track2Only.OTHER);
            Assert.AreEqual(String.CompareOrdinal(parsed.Track2DataToString(), MagCardTestData.Valid.Track2Only.OTHER), 0);
        }

        [TestMethod]
        public void Parse_Track1Only_Other()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.Valid.Track1Only.OTHER);
            Assert.AreEqual(String.CompareOrdinal(parsed.Track1DataToString(), MagCardTestData.Valid.Track1Only.OTHER), 0);
        }

        [TestMethod]
        public void MagCardToString()
        {
            MagCard parsed = MagCard.Parse(MagCardTestData.CARD_DATA);
            string result = parsed.ToString(false);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result, string.Empty);
        }

    }
}
