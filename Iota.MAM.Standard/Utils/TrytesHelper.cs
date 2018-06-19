using System;
using System.Linq;

namespace Iota.MAM.Utils
{
    public class TrytesHelper
    {
        public static string KeyGen(int length)
        {
            var charset = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random((int) DateTime.Now.Ticks);

            return new string(Enumerable.Repeat(charset, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}