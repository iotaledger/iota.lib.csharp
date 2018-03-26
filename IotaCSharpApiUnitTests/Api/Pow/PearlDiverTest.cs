using System;
using System.Text;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Pow;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api.Pow
{
    [TestClass]
    public class PearlDiverTest
    {

        private const  int TryteLength = 2673;
        private const int MinWeightMagnitude = 9;
        private const int NumCores = -1; // use n-1 cores

        private static readonly Random Random = new Random();
        private PearlDiver _pearlDiver;
        private int[] _hashTrits;

        [TestInitialize]
        public void Setup()
        {
            _pearlDiver = new PearlDiver();
            _hashTrits = new int[Sponge.HashLength];
        }


        [TestMethod]
        public async Task TestRandomTryteHash()
        {
            string testTrytes = GetRandomTrytes();
            
            string hash = await GetHashFor(testTrytes);

            string subHash = hash.Substring(Sponge.HashLength / 3 - MinWeightMagnitude / 3);

            bool success = InputValidator.IsNinesTrytes(subHash,subHash.Length);
            if (!success)
            {
                Console.WriteLine(testTrytes);
            }

            Assert.IsTrue(success, "The hash should have n nines");
        }

        [TestMethod]
        [Ignore]
        public async Task TestRandomTryteHash100()
        {
            for (int i = 0; i < 100; i++)
            {
                string testTrytes = GetRandomTrytes();

                string hash = await GetHashFor(testTrytes);

                string subHash = hash.Substring(Sponge.HashLength / 3 - MinWeightMagnitude / 3);

                bool success = InputValidator.IsNinesTrytes(subHash, subHash.Length);
                if (!success)
                {
                    Console.WriteLine(testTrytes);
                }

                Assert.IsTrue(success, "The hash should have n nines");
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

        private async Task<string> GetHashFor(string trytes)
        {
            Sponge curl = new Curl(CurlMode.CurlP81);
            int[] myTrits = Converter.ToTrits(trytes);
            
            bool result  = await _pearlDiver.Search(myTrits, MinWeightMagnitude, NumCores);
            
            Assert.IsTrue(result,"Search Failed");
           
            curl.Absorb(myTrits, 0, myTrits.Length);
            curl.Squeeze(_hashTrits, 0, Sponge.HashLength);
            curl.Reset();

            return Converter.ToTrytes(_hashTrits);
        }
    }
}
