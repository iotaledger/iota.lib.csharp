using System;
using System.Linq;
using Iota.Api.Standard.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Api.Standard.Tests.Utils
{
    [TestClass]
    public class TrytesConverterTest
    {
        private static readonly Random Random = new Random();

        [TestMethod]
        public void ShouldConvertStringToTrytes()
        {
            Assert.AreEqual("IC", TrytesConverter.ToTrytes("Z"));
            Assert.AreEqual(TrytesConverter.ToTrytes("JOTA JOTA"), "TBYBCCKBEATBYBCCKB");
        }

        [TestMethod]
        public void ShouldConvertTrytesToString()
        {
            Assert.AreEqual("Z", TrytesConverter.ToString("IC"));
            Assert.AreEqual(TrytesConverter.ToString("TBYBCCKBEATBYBCCKB"), "JOTA JOTA");
        }

        [TestMethod]
        public void ShouldConvertBackAndForth()
        {
            var str = RandomString(1000);
            var back = TrytesConverter.ToString(TrytesConverter.ToTrytes(str));
            Assert.AreEqual(str, back);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}