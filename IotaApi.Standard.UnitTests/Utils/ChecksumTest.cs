using Iota.Api.Standard.Utils;
using Xunit;

namespace Iota.Api.Standard.UnitTests
{
    public class ChecksumTest
    {
        private const string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private const string TEST_ADDRESS_WITH_CHECKSUM =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZCCOZVXMTXC";

        [Fact]
        public void ShouldAddChecksum()
        {
            Assert.Equal(Checksum.AddChecksum(TEST_ADDRESS_WITHOUT_CHECKSUM), TEST_ADDRESS_WITH_CHECKSUM);
        }

        [Fact]
        public void ShouldRemoveChecksum()
        {
            Assert.Equal(TEST_ADDRESS_WITH_CHECKSUM.RemoveChecksum(), TEST_ADDRESS_WITHOUT_CHECKSUM);
        }

        [Fact]
        public void ShouldIsValidChecksum()
        {
            Assert.True(TEST_ADDRESS_WITH_CHECKSUM.IsValidChecksum());
        }
    }
}