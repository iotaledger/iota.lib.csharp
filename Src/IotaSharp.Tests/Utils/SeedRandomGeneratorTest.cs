using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Utils
{
    [TestClass]
    public class SeedRandomGeneratorTest
    {
        [TestMethod]
        public void ShouldGenerateNewSeed()
        {
            string generatedSeed = SeedRandomGenerator.GenerateNewSeed();
            Assert.IsTrue(InputValidator.IsValidSeed(generatedSeed));
            Assert.AreEqual(generatedSeed.Length, Constants.SEED_LENGTH_MAX);
        }
    }
}
