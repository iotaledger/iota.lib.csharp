using Iota.Api.Standard.Pow;
using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{ 
    public class KerlTest
    {
        [Fact]
        public void ShouldCreateValidHash_1()
        {
            int[] trits = Converter.ToTrits("GYOMKVTSNHVJNCNFBBAH9AAMXLPLLLROQY99QN9DLSJUHDPBLCFFAIQXZA9BKMBJCYSFHFPXAHDWZFEIZ");
            Kerl kerl = new Kerl();
            int[] hashTrits = new int[trits.Length];

            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            kerl.Squeeze(hashTrits, 0, 243);
            string hash = Converter.ToTrytes(hashTrits);

            Assert.Equal("OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW", hash);
        }

        [Fact]
        public void ShouldCreateValidHash_2()
        {
            int[] trits = Converter.ToTrits("9MIDYNHBWMBCXVDEFOFWINXTERALUKYYPPHKP9JJFGJEIUY9MUDVNFZHMMWZUYUSWAIOWEVTHNWMHANBH");
            Kerl kerl = new Kerl();
            int[] hashTrits = new int[trits.Length * 2];

            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            string hash = Converter.ToTrytes(hashTrits);

            Assert.Equal("G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA", hash);
        }

        [Fact]
        public void ShouldCreateValidHash_3()
        {
            int[] trits = Converter.ToTrits("G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");
            Kerl kerl = new Kerl();
            int[] hashTrits = new int[trits.Length];

            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            string hash = Converter.ToTrytes(hashTrits);

            Assert.Equal("LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX", hash);
        }
    }
}