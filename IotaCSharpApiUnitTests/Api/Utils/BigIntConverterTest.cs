using System;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;

namespace Iota.Lib.CSharpTests.Api.Utils
{
    [TestClass]
    public class BigIntConverterTest
    {
        private static readonly Random Random = new Random();
        // trits<->BigInteger<->byte

        [TestMethod]
        public void TestTritsAndBigInt()
        {
            int[] inputTrits = new int[243];
            for (int i = 0; i < inputTrits.Length; i++)
            {
                inputTrits[i] = Random.Next(3) - 1;
            }

            var bigInt = BigIntConverter.BigIntFromTrits(inputTrits, 0, inputTrits.Length);

            int[] outputTrits = new int[inputTrits.Length];
            BigIntConverter.TritsFromBigInt(bigInt, outputTrits, 0, outputTrits.Length);

            for (int i = 0; i < inputTrits.Length; i++)
            {
                Assert.AreEqual(inputTrits[i], outputTrits[i]);
            }
        }

        [TestMethod]
        public void TestBigIntAndByte()
        {
            byte[] bytes = new byte[48];
            BigInteger bigInt0 = new BigInteger("-123456");

            BigIntConverter.BytesFromBigInt(bigInt0, bytes, 0, bytes.Length);
            var bigInt1 = BigIntConverter.BigIntFromBytes(bytes, 0, bytes.Length);

            Assert.AreEqual(bigInt0,bigInt1);
        }

        [TestMethod]
        public void TestFixedBigInt()
        {
            int[] inputTrits = new int[243];
            int[] outputTrits = new int[243];
            byte[] bytes = new byte[384/8];

            for (int i = 0; i < inputTrits.Length; i++)
            {
                inputTrits[i] = Random.Next(3) - 1;
            }

            inputTrits[inputTrits.Length - 1] = 0;
            FixedBigIntConverter.FromTritsToBytes(inputTrits,bytes);
            FixedBigIntConverter.FromBytesToTrits(bytes,outputTrits);
            outputTrits[outputTrits.Length - 1] = 0;

            for (int i = 0; i < inputTrits.Length; i++)
            {
                Assert.AreEqual(inputTrits[i], outputTrits[i]);
            }
        }
    }
}
