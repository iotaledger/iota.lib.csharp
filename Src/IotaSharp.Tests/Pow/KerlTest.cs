using System.Diagnostics.CodeAnalysis;
using IotaSharp.Pow;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Pow
{
    [TestClass]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class KerlTest
    {
        [TestMethod]
        public void ShouldCreateValidHash1()
        {
            sbyte[] trits =
                Converter.ToTrits("GYOMKVTSNHVJNCNFBBAH9AAMXLPLLLROQY99QN9DLSJUHDPBLCFFAIQXZA9BKMBJCYSFHFPXAHDWZFEIZ");
            Kerl kerl = (Kerl) SpongeFactory.Create(SpongeFactory.Mode.KERL);
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            sbyte[] hashTrits = new sbyte[trits.Length];
            kerl.Squeeze(hashTrits, 0, 243);
            string hash = Converter.ToTrytes(hashTrits);
            Assert.AreEqual(hash, "OXJCNFHUNAHWDLKKPELTBFUCVW9KLXKOGWERKTJXQMXTKFKNWNNXYD9DMJJABSEIONOSJTTEVKVDQEWTW");
        }

        [TestMethod]
        public void ShouldCreateValidHash2()
        {
            sbyte[] trits =
                Converter.ToTrits("9MIDYNHBWMBCXVDEFOFWINXTERALUKYYPPHKP9JJFGJEIUY9MUDVNFZHMMWZUYUSWAIOWEVTHNWMHANBH");
            Kerl kerl = (Kerl) SpongeFactory.Create(SpongeFactory.Mode.KERL);
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            sbyte[] hashTrits = new sbyte[trits.Length * 2];
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            string hash = Converter.ToTrytes(hashTrits);
            Assert.AreEqual(hash,
                "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");
        }

        [TestMethod]
        public void ShouldCreateValidHash3()
        {
            sbyte[] trits =
                Converter.ToTrits(
                    "G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");
            Kerl kerl = (Kerl) SpongeFactory.Create(SpongeFactory.Mode.KERL);
            kerl.Reset();
            kerl.Absorb(trits, 0, trits.Length);
            sbyte[] hashTrits = new sbyte[trits.Length];
            kerl.Squeeze(hashTrits, 0, 243 * 2);
            string hash = Converter.ToTrytes(hashTrits);
            Assert.AreEqual(hash,
                "LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX");
        }
    }
}
