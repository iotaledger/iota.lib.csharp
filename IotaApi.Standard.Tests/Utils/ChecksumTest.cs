using Iota.Api.Standard.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Api.Standard.Tests.Utils
{
    [TestClass]
    public class ChecksumTest
    {
        private static readonly string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZCCOZVXMTXC";

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