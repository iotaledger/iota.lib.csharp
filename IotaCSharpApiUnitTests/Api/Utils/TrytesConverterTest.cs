using System;
using System.Linq;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api.Utils
{
    [TestClass]
    public class TrytesConverterTest
    {
        [TestMethod]
        public void shouldConvertStringToTrytes()
        {
            Assert.AreEqual("IC", TrytesConverter.toTrytes("Z"));
            Assert.AreEqual(TrytesConverter.toTrytes("JOTA JOTA"), "TBYBCCKBEATBYBCCKB");
        }

        [TestMethod]
        public void shouldConvertTrytesToString()
        {
            Assert.AreEqual("Z", TrytesConverter.toString("IC"));
            Assert.AreEqual(TrytesConverter.toString("TBYBCCKBEATBYBCCKB"), "JOTA JOTA");
        }

        public void shouldConvertBackAndForth()
        {
            string str = RandomString(1000);
            string back = TrytesConverter.toString(TrytesConverter.toTrytes(str));
            Assert.AreEqual(str, back);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}