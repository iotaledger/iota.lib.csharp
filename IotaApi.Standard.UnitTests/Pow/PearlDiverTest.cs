using System;
using System.Text;
using Iota.Api.Standard.Pow;
using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{
    public class PearlDiverTest
    {
        private const  int TryteLength = 2673;
        private const int MinWeightMagnitude = 9;
        private const int NumCores = -1;

        private static readonly Random Random = new Random();

        [Fact]
        public void ShouldGenerateHashWithTrytesAtEnd()
        {
            string testTrytes = GetRandomTrytes();
            
            string hash = GetHashFor(testTrytes);

            string subHash = hash.Substring(Sponge.HashLength / 3 - MinWeightMagnitude / 3);

            bool success = InputValidator.IsNinesTrytes(subHash,subHash.Length);
            if (!success)
            {
                Console.WriteLine(testTrytes);
            }

            Assert.True(success, "The hash should have n nines");
        }

        [Fact]
        public void ShouldGenerateHashWithTrytesAtEnd100()
        {
            for (int i = 0; i < 100; i++)
            {
                string testTrytes = GetRandomTrytes();

                string hash = GetHashFor(testTrytes);

                string subHash = hash.Substring(Sponge.HashLength / 3 - MinWeightMagnitude / 3);

                bool success = InputValidator.IsNinesTrytes(subHash, subHash.Length);
                if (!success)
                {
                    Console.WriteLine(testTrytes);
                }

                Assert.True(success, "The hash should have n nines");
            }
        }

        private string GetRandomTrytes()
        {
            var trytes = new StringBuilder();

            for (int i = 0; i < TryteLength; i++)
            {
                trytes.Append(Constants.TryteAlphabet[Random.Next(27)]);
            }

            return trytes.ToString();
        }

        private string GetHashFor(string trytes)
        {
            Sponge curl = new Curl(CurlMode.CurlP81);
            PearlDiver pearlDiver = new PearlDiver();
            int[] hashTrits = new int[Sponge.HashLength];
            int[] myTrits = Converter.ToTrits(trytes);
            
            bool result  = pearlDiver.Search(myTrits, MinWeightMagnitude, NumCores);
            
            curl.Absorb(myTrits, 0, myTrits.Length);
            curl.Squeeze(hashTrits, 0, Sponge.HashLength);
            curl.Reset();

            return Converter.ToTrytes(hashTrits);
        }
    }
}
