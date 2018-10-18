using System.Collections.Generic;
using Iota.Api.Model;
using Iota.Api.Pow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Api.Tests.Pow
{
    [TestClass]
    public class LocalPoWTest
    {
        private static readonly string TEST_SEED1 =
            "IHDEENZYITYVYSPKAURUZAQKGVJEREFDJMYTANNXXGPZ9GJWTEOJJ9IPMXOGZNQLSNMFDSQOTZAEETUEA";

        private static readonly string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2 =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private static readonly string TEST_MESSAGE = "JUSTANOTHERJOTATEST";
        private static readonly string TEST_TAG = "JOTASPAM9999999999999999999";
        private static readonly int MIN_WEIGHT_MAGNITUDE = 14;
        private static readonly int DEPTH = 9;

        private IotaApi _iotaClient;

        [TestInitialize]
        public void Setup()
        {
            _iotaClient = new IotaApi("node03.iotatoken.nl", 15265)
            {
                LocalPow = new PearlDiverLocalPoW()
            };
        }

        [TestMethod]
        public void ShouldSendTransfer()
        {
            var transfers = new List<Transfer>
            {
                new Transfer(TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2, 0, TEST_MESSAGE, TEST_TAG)
            };
            var result = _iotaClient.SendTransfer(
                TEST_SEED1, 2, DEPTH, MIN_WEIGHT_MAGNITUDE, transfers.ToArray(),
                null, null, false, false);
            Assert.IsNotNull(result);
        }
    }
}