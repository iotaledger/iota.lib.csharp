using System;
using Iota.Api.Pow;
using Iota.Api.Utils;
using Iota.MAM.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.MAM.Tests.Core
{
    [TestClass]
    public class MaskTest
    {
        [TestMethod]
        public void ItCanUnmask()
        {
            int[] payload = Converter.ToTrits("AAMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9MESSAGEFORYOU9");
            int[] keys = Converter.ToTrits("MYMERKLEROOTHASH");

            //int index = 5;
            ICurl curl = new Curl(CurlMode.CurlP27);
            //add_assign(&mut keys, index as isize);
            int[] cipher = new int[payload.Length];
            Array.Copy(payload,cipher,payload.Length);

            MaskHelper.Mask(cipher, keys, curl);
            curl.Reset();
            MaskHelper.UnMask(cipher, keys, curl);

            Assert.AreEqual(Converter.ToTrytes(cipher),Converter.ToTrytes(payload));
            
        }
        [TestMethod]
        public void ItCanUnmaskSlice()
        {
            int[] payload = Converter.ToTrits("AAMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9AMESSAGEFORYOU9MESSAGEFORYOU9");
            ICurl curl = new Curl(CurlMode.CurlP27);

            int[] cipher = new int[payload.Length];
            Array.Copy(payload, cipher, payload.Length);

            MaskHelper.MaskSlice(cipher, curl);
            curl.Reset();
            MaskHelper.UnMaskSlice(cipher, curl);

            Assert.AreEqual(Converter.ToTrytes(cipher), Converter.ToTrytes(payload));
        }
    }
}
