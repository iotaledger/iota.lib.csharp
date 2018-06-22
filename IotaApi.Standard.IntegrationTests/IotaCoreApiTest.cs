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
                var response = _iotaApi.AddNeighbors("udp://8.8.8.8:14265");
                Assert.NotNull(response);
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
                var response = _iotaApi.RemoveNeighbors("udp://8.8.8.8:14265");
                Assert.NotNull(response);
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
            var response = _iotaApi.GetTrytes(TEST_HASH);
            Assert.NotNull(response.Trytes);
        }

        [Fact]
        public void ShouldThrowOnInvalidMileStone()
        {
            Assert.Throws<IotaApiException>(
                () => _iotaApi.GetInclusionStates(new[] { TEST_HASH }, new[] { "DNSBRJWNOVUCQPILOQIFDKBFJMVOTGHLIMLLRXOHFTJZGRHJUEDAOWXQRYGDI9KHYFGYDWQJZKX999999" })
            );
        }

        [Fact]
        public void ShouldGetInclusionStates()
        {
            var response = _iotaApi.GetInclusionStates( new[] {TEST_HASH}, new[] {_iotaApi.GetNodeInfo().LatestSolidSubtangleMilestone});
            Assert.NotNull(response.States);
        }

        [Fact]
        public void ShouldGetTransactionsToApprove()
        {
            var response = _iotaApi.GetTransactionsToApprove(7);

            Assert.NotNull(response.TrunkTransaction);
            Assert.NotNull(response.BranchTransaction);
        }

        [Fact]
        public void ShouldFindTransactions()
        {
            var test = TEST_BUNDLE;
            // responseharper disable once UnusedVariable
            var responsep = _iotaApi.FindTransactions(new[] {test}.ToList(),
                new[] {test}.ToList(), new[] {test}.ToList(), new[] {test}.ToList());
        }

        [Fact]
        public void ShouldGetBalances()
        {
            var response = _iotaApi.GetBalances(new[] {TEST_ADDRESS_WITH_CHECKSUM}.ToList(), 100);
            Assert.NotNull(response.Balances);
            Assert.NotNull(response.References);
            Assert.False(response.MilestoneIndex == 0);
        }
    }
}