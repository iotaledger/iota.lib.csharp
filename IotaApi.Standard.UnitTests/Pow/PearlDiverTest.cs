using System;
using System.Text;
using Iota.Api.Standard.Pow;
using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{
    public class PearlDiverTest
    {
        private const int TryteLength = 2673;
        private const int MinWeightMagnitude = 12;
        private const int NumCores = -1;

        [Fact]
        public void ShouldGenerateHashWithTrytesAtEnd()
        {
            Sponge curl = new Curl(CurlMode.CurlP81);
            PearlDiver pearlDiver = new PearlDiver();
            int[] initialTrits = Converter.ToTrits(GetRandomTrytes());
            int[] tritsAfterPoW = new int[Sponge.HashLength];

            bool result = pearlDiver.Search(initialTrits, MinWeightMagnitude, NumCores);
            curl.Absorb(initialTrits, 0, initialTrits.Length);
            curl.Squeeze(tritsAfterPoW, 0, Sponge.HashLength);
            string hash = Converter.ToTrytes(tritsAfterPoW);
            string subHash = hash.Substring(Sponge.HashLength / 3 - MinWeightMagnitude / 3);
            bool success = InputValidator.IsNinesTrytes(subHash,subHash.Length);

            Assert.True(result);  //Pearldiver was successfull finding a hash
            Assert.True(success); //Hash contains the correct amount of nines at the end 
        }

        private string GetRandomTrytes()
        {
            StringBuilder trytes = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < TryteLength; i++)
            {
                trytes.Append(Constants.TryteAlphabet[random.Next(27)]);
            }

            return trytes.ToString();
        }
    }
}
