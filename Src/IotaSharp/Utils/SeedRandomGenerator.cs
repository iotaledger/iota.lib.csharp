using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class SeedRandomGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GenerateNewSeed()
        {
            return KeyGen(Constants.SEED_LENGTH_MAX);
        }

        private static string KeyGen(int length)
        {
            var charset = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random((int) DateTime.Now.Ticks);

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
