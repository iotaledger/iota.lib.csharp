using Iota.Api.Standard.Pow;
using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{ 
    public class KerlTest
    {
        [Fact]
        public void ShouldCreateValidHash1()
        {
            var trits = Converter.ToTrits(
                "GYOMKVTSNHVJNCNFBBAH9AAMXLPLLLROQY99QN9DLSJUHDPBLCFFAIQXZA9BKMBJCYSFHFPXAHDWZFEIZ");
            var kerl = new Kerl();
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new int[trits.Length];
            kerl.Squeeze(hashTrits, 0, 243);
            var hash = Converter.ToTrytes(hashTrits);
            Assert.Equal("OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW", hash);
        }

        [Fact]
        public void ShouldCreateValidHash2()
        {
            var trits = Converter.ToTrits(
                "9MIDYNHBWMBCXVDEFOFWINXTERALUKYYPPHKP9JJFGJEIUY9MUDVNFZHMMWZUYUSWAIOWEVTHNWMHANBH");
            var kerl = new Kerl();
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new int[trits.Length * 2];
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            var hash = Converter.ToTrytes(hashTrits);
            Assert.Equal(
                "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA", hash);
        }

        [Fact]
        public void ShouldCreateValidHash3()
        {
            var trits = Converter.ToTrits(
                "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");
            var kerl = new Kerl();
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            var hashTrits = new int[trits.Length];
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            var hash = Converter.ToTrytes(hashTrits);
            Assert.Equal(
                "LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX", hash);
        }
    }
}