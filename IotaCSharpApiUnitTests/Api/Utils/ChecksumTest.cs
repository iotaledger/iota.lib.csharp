using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api.Utils
{
    [TestClass]
    public class ChecksumTest
    {
        private static readonly string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "RVORZ9SIIP9RCYMREUIXXVPQIPHVCNPQ9HZWYKFWYWZRE9JQKG9REPKIASHUUECPSQO9JT9XNMVKWYGVA";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "RVORZ9SIIP9RCYMREUIXXVPQIPHVCNPQ9HZWYKFWYWZRE9JQKG9REPKIASHUUECPSQO9JT9XNMVKWYGVAFOXM9MUBX";

        [TestMethod]
        public void ShouldAddChecksum()
        {
            Assert.AreEqual(Checksum.AddChecksum(TEST_ADDRESS_WITHOUT_CHECKSUM), TEST_ADDRESS_WITH_CHECKSUM);
        }

        [TestMethod]
        public void ShouldRemoveChecksum()
        {
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM.RemoveChecksum(), TEST_ADDRESS_WITHOUT_CHECKSUM);
        }

        [TestMethod]
        public void ShouldIsValidChecksum()
        {
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM.IsValidChecksum(), true);
        }
    }
}