using System.Diagnostics.CodeAnalysis;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Utils
{
    [TestClass]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class ChecksumTest
    {
        private static readonly string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZCCOZVXMTXC";

        [TestMethod]
        public void ShouldAddChecksum()
        {
            Assert.AreEqual(TEST_ADDRESS_WITHOUT_CHECKSUM.AddChecksum(), TEST_ADDRESS_WITH_CHECKSUM);
        }

        [TestMethod]
        public void ShouldRemoveChecksum()
        {
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM.RemoveChecksum(), TEST_ADDRESS_WITHOUT_CHECKSUM);
        }

        [TestMethod]
        public void ShouldIsValidChecksum()
        {
            Assert.IsTrue(TEST_ADDRESS_WITH_CHECKSUM.IsValidChecksum());
        }
    }
}
