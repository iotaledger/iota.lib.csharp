using System;
using System.Linq;
using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{
    public class TrytesConverterTest
    {
        private static readonly Random Random = new Random();

        [Fact]
        public void ShouldConvertStringToTrytes()
        {
            Assert.Equal("IC", TrytesConverter.ToTrytes("Z"));
            Assert.Equal("TBYBCCKBEATBYBCCKB", TrytesConverter.ToTrytes("JOTA JOTA"));
        }

        [Fact]
        public void ShouldConvertTrytesToString()
        {
            Assert.Equal("Z", TrytesConverter.ToString("IC"));
            Assert.Equal("JOTA JOTA", TrytesConverter.ToString("TBYBCCKBEATBYBCCKB"));
        }

        [Fact]
        public void ShouldConvertTryteString()
        {
            var str = RandomString(1000);
            var back = TrytesConverter.ToString(TrytesConverter.ToTrytes(str));
            Assert.Equal(str, back);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}