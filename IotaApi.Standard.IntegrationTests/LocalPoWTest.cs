using System.Collections.Generic;
using Iota.Api.Standard.Model;
using Iota.Api.Standard.Pow;
using Xunit;

namespace Iota.Api.Standard.IntegrationTests
{
    public class LocalPoWTest
    {
        private const string TEST_SEED1 =
            "IHDEENZYITYVYSPKAURUZAQKGVJEREFDJMYTANNXXGPZ9GJWTEOJJ9IPMXOGZNQLSNMFDSQOTZAEETUEA";

        private const string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2 =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private const string TEST_MESSAGE = "JUSTANOTHERJOTATEST";
        private const string TEST_TAG = "JOTASPAM9999999999999999999";
        private const int MIN_WEIGHT_MAGNITUDE = 14;
        private const int DEPTH = 9;

        private IotaApi _iotaClient;

        public LocalPoWTest()
        {
            _iotaClient = new IotaApi("node.iotawallet.info", 14265)
            {
                LocalPow = new PearlDiverLocalPoW()
            };
        }

        [Fact]
        public void ShouldSendTransfer()
        {
            var transfers = new List<Transfer>
            {
                new Transfer(TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2, 0, TEST_MESSAGE, TEST_TAG)
            };
            var result = _iotaClient.SendTransfer( TEST_SEED1, 2, DEPTH, MIN_WEIGHT_MAGNITUDE, transfers.ToArray(), null, null, false, false);
            Assert.NotNull(result);
        }
    }
}