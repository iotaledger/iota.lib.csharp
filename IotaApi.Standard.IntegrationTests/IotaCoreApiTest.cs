using System.Linq;
using Iota.Api.Standard.Exception;
using Xunit;

namespace Iota.Api.Standard.IntegrationTests
{
    public class IotaCoreApiTest
    {
        private static readonly string TEST_BUNDLE =
            "XZKJUUMQOYUQFKMWQZNTFMSS9FKJLOEV9DXXXWPMQRTNCOUSUQNTBIJTVORLOQPLYZOTMLFRHYKMTGZZU";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "PNGMCSNRCTRHCHPXYTPKEJYPCOWKOMRXZFHH9N9VDIKMNVAZCMIYRHVJIAZARZTUETJVFDMBEBIQE9QTHBFWDAOEFA";

        private static readonly string TEST_HASH =
            "OAATQS9VQLSXCLDJVJJVYUGONXAXOFMJOZNSYWRZSWECMXAQQURHQBJNLD9IOFEPGZEPEMPXCIVRX9999";

        private static IotaApi _iotaApi;

        public IotaCoreApiTest()
        {
            _iotaApi = new IotaApi("node.iotawallet.info", 14265);
        }

        [Fact]
        public void ShouldGetNodeInfo()
        {
            var nodeInfo = _iotaApi.GetNodeInfo();
            Assert.NotNull(nodeInfo.AppVersion);
            Assert.NotNull(nodeInfo.AppName);
            Assert.NotNull(nodeInfo.JreVersion);
        }

        [Fact]
        public void ShouldGetNeighbors()
        {
            var neighbors = _iotaApi.GetNeighbors();
            Assert.NotNull(neighbors.Neighbors);
        }

        [Fact]
        public void ShouldAddNeighbors()
        {
            try
            {
                var res = _iotaApi.AddNeighbors("udp://8.8.8.8:14265");
                Assert.NotNull(res);
            }
            catch (IotaApiException e)
            {
                Assert.Contains("not available on this node", e.Message);
            }
        }

        [Fact]
        public void ShouldRemoveNeighbors()
        {
            try
            {
                var res = _iotaApi.RemoveNeighbors("udp://8.8.8.8:14265");
                Assert.NotNull(res);
            }
            catch (IotaApiException e)
            {
                Assert.Contains("not available on this node", e.Message);
            }
        }

        [Fact]
        public void ShouldGetTips()
        {
            var tips = _iotaApi.GetTips();
            Assert.NotNull(tips);
        }

        [Fact]
        public void ShouldFindTransactionsByAddresses()
        {
            var trans = _iotaApi.FindTransactionsByAddresses(TEST_ADDRESS_WITH_CHECKSUM);
            Assert.NotNull(trans.Hashes);
            Assert.True(trans.Hashes.Count > 0);
        }

        [Fact]
        public void ShouldFindTransactionsByApprovees()
        {
            var trans = _iotaApi.FindTransactionsByApprovees(TEST_HASH);
            Assert.NotNull(trans.Hashes);
        }

        [Fact]
        public void ShouldFindTransactionsByBundles()
        {
            var trans = _iotaApi.FindTransactionsByBundles(TEST_HASH);
            Assert.NotNull(trans.Hashes);
        }

        [Fact]
        public void ShouldFindTransactionsByDigests()
        {
            var trans = _iotaApi.FindTransactionsByDigests(TEST_HASH);
            Assert.NotNull(trans.Hashes);
        }

        [Fact]
        public void ShouldGetTrytes()
        {
            var res = _iotaApi.GetTrytes(TEST_HASH);
            Assert.NotNull(res.Trytes);
        }

        [Fact]
        public void ShouldNotGetInclusionStates()
        {
            var res = _iotaApi.GetInclusionStates(new[] {TEST_HASH},
                new[] {"DNSBRJWNOVUCQPILOQIFDKBFJMVOTGHLIMLLRXOHFTJZGRHJUEDAOWXQRYGDI9KHYFGYDWQJZKX999999"});
            Assert.NotNull(res.States);
        }

        [Fact]
        public void ShouldGetInclusionStates()
        {
            var res =
                _iotaApi.GetInclusionStates(
                    new[] {TEST_HASH},
                    new[] {_iotaApi.GetNodeInfo().LatestSolidSubtangleMilestone});
            Assert.NotNull(res.States);
        }

        [Fact] // very long execution
        public void ShouldGetTransactionsToApprove()
        {
            var res = _iotaApi.GetTransactionsToApprove(27);
            Assert.NotNull(res.TrunkTransaction);
            Assert.NotNull(res.BranchTransaction);
        }

        [Fact]
        public void ShouldFindTransactions()
        {
            var test = TEST_BUNDLE;
            // ReSharper disable once UnusedVariable
            var resp = _iotaApi.FindTransactions(new[] {test}.ToList(),
                new[] {test}.ToList(), new[] {test}.ToList(), new[] {test}.ToList());
        }

        [Fact]
        public void ShouldGetBalances()
        {
            var res = _iotaApi.GetBalances(new[] {TEST_ADDRESS_WITH_CHECKSUM}.ToList(), 100);
            Assert.NotNull(res.Balances);
            Assert.NotNull(res.References);
            Assert.False(res.MilestoneIndex == 0);
        }
    }
}