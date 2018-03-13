using Iota.Lib.CSharp.Api.Pow;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api
{
    [TestClass]
    public class KerlTest
    {
        [TestMethod]
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
            Assert.AreEqual(hash, "OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW");
        }

        [TestMethod]
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
            Assert.AreEqual(hash,
                "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");
        }

        [TestMethod]
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
            Assert.AreEqual(hash,
                "LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX");
        }
    }
}