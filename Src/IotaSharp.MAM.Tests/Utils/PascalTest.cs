using IotaSharp.MAM.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.MAM.Tests.Utils
{
    [TestClass]
    public class PascalTest
    {
        [TestMethod]
        public void EncodeNumbers()
        {
            for (long i = 0; i < 1000; i++)
            {
                TestEncoding(i);
            }

            for (long i = 0; i < 1000; i++)
            {
                TestEncoding(-i);
            }

            for (long i = 10000000; i < 10000100; i++)
            {
                TestEncoding(i);
            }

            for (long i = 10000000; i < 10000100; i++)
            {
                TestEncoding(-i);
            }

            TestEncoding(long.MaxValue / 1000);
            TestEncoding(long.MinValue / 1000);

        }

        private void TestEncoding(long input)
        {
            int length = Pascal.EncodedLength(input);
            sbyte[] trits = new sbyte[length];

            Pascal.Encode(input, trits);

            var result = Pascal.Decode(trits);

            Assert.AreEqual(input, result.Item1);
            Assert.AreEqual(trits.Length, result.Item2);

        }
    }
}
