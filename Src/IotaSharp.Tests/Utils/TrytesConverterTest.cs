using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Utils
{
    [TestClass]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class TrytesConverterTest
    {
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        [TestMethod]
        public void ShouldConvertStringToTrytes()
        {
            Assert.AreEqual(TrytesConverter.AsciiToTrytes("Z"), "IC");
            Assert.AreEqual(TrytesConverter.AsciiToTrytes("IOTA SHARP"), "SBYBCCKBEABCRBKBACZB");
        }

        [TestMethod]
        public void ShouldConvertTrytesToString()
        {
            Assert.AreEqual(TrytesConverter.TrytesToAscii("IC"), "Z");
            Assert.AreEqual(TrytesConverter.TrytesToAscii("SBYBCCKBEABCRBKBACZB"), "IOTA SHARP");
        }

        [TestMethod]
        public void ShouldConvertBackAndForth()
        {
            string str = RandomString(1000);
            string back = TrytesConverter.TrytesToAscii(TrytesConverter.AsciiToTrytes(str));

            Assert.AreEqual(str, back);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
