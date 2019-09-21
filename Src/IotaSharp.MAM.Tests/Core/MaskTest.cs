using System;
using System.Diagnostics.CodeAnalysis;
using IotaSharp.MAM.Core;
using IotaSharp.Pow;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.MAM.Tests.Core
{
    [TestClass]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class MaskTest
    {
        [TestMethod]
        public void ItCanUnmask()
        {
            sbyte[] payload = Converter.ToTrits(
                "AAMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9MESSAGEFORYOU9");
            sbyte[] keys = Converter.ToTrits("MYMERKLEROOTHASH");

            //int index = 5;
            ICurl curl = new Curl(SpongeFactory.Mode.CURLP27);
            //add_assign(&mut keys, index as isize);
            sbyte[] cipher = new sbyte[payload.Length];
            Array.Copy(payload, cipher, payload.Length);

            MaskHelper.Mask(cipher, keys, curl);
            curl.Reset();
            MaskHelper.UnMask(cipher, keys, curl);

            Assert.AreEqual(Converter.ToTrytes(cipher), Converter.ToTrytes(payload));

        }

        [TestMethod]
        public void ItCanUnmaskSlice()
        {
            sbyte[] payload = Converter.ToTrits(
                "AAMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9MESSAGEFORYOU9");
            ICurl curl = new Curl(SpongeFactory.Mode.CURLP27);

            sbyte[] cipher = new sbyte[payload.Length];
            Array.Copy(payload, cipher, payload.Length);

            MaskHelper.MaskSlice(cipher, curl);
            curl.Reset();
            MaskHelper.UnMaskSlice(cipher, curl);

            Assert.AreEqual(Converter.ToTrytes(cipher), Converter.ToTrytes(payload));
        }
    }
}
