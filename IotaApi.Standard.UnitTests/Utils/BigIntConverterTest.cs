using System;
using Iota.Api.Standard.Utils;
using Xunit;
using Org.BouncyCastle.Math;

namespace Iota.Api.Standard.UnitTests
{
    public class BigIntConverterTest
    {
        private static readonly Random Random = new Random();

        [Fact]
        public void ShouldConvertTritsAndBigInt()
        {
            var inputTrits = new int[243];
            for (var i = 0; i < inputTrits.Length; i++) inputTrits[i] = Random.Next(3) - 1;

            var bigInt = BigIntConverter.BigIntFromTrits(inputTrits, 0, inputTrits.Length);

            var outputTrits = new int[inputTrits.Length];
            BigIntConverter.TritsFromBigInt(bigInt, outputTrits, 0, outputTrits.Length);

            for (var i = 0; i < inputTrits.Length; i++) Assert.Equal(inputTrits[i], outputTrits[i]);
        }

        [Fact]
        public void ShouldConvertBigIntAndByte()
        {
            var bytes = new byte[48];
            var bigInt0 = new BigInteger("-123456");

            BigIntConverter.BytesFromBigInt(bigInt0, bytes, 0, bytes.Length);
            var bigInt1 = BigIntConverter.BigIntFromBytes(bytes, 0, bytes.Length);

            Assert.Equal(bigInt0, bigInt1);
        }

        [Fact]
        public void ShouldConvertFixedBigInt()
        {
            var inputTrits = new int[243];
            var outputTrits = new int[243];
            var bytes = new byte[384 / 8];

            for (var i = 0; i < inputTrits.Length; i++) inputTrits[i] = Random.Next(3) - 1;

            inputTrits[inputTrits.Length - 1] = 0;
            FixedBigIntConverter.FromTritsToBytes(inputTrits, bytes);
            FixedBigIntConverter.FromBytesToTrits(bytes, outputTrits);
            outputTrits[outputTrits.Length - 1] = 0;

            for (var i = 0; i < inputTrits.Length; i++) Assert.Equal(inputTrits[i], outputTrits[i]);
        }
    }
}