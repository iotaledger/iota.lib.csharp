using System.Linq;
using Iota.Api.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Api.Tests
{
    [TestClass]
    public class IotaCoreApiTest
    {
        private static readonly string TEST_BUNDLE =
            "XZKJUUMQOYUQFKMWQZNTFMSS9FKJLOEV9DXXXWPMQRTNCOUSUQNTBIJTVORLOQPLYZOTMLFRHYKMTGZZU";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "PNGMCSNRCTRHCHPXYTPKEJYPCOWKOMRXZFHH9N9VDIKMNVAZCMIYRHVJIAZARZTUETJVFDMBEBIQE9QTHBFWDAOEFA";

        private static readonly string TEST_HASH =
            "OAATQS9VQLSXCLDJVJJVYUGONXAXOFMJOZNSYWRZSWECMXAQQURHQBJNLD9IOFEPGZEPEMPXCIVRX9999";

        private static IotaApi _iotaApi;

        [TestInitialize]
        public void CreateProxyInstance()
        {
            _iotaApi = new IotaApi("node.iotawallet.info", 14265);
        }

        [TestMethod]
        public void ShouldGetNodeInfo()
        {
            var nodeInfo = _iotaApi.GetNodeInfo();
            Assert.IsNotNull(nodeInfo.AppVersion);
            Assert.IsNotNull(nodeInfo.AppName);
            Assert.IsNotNull(nodeInfo.JreVersion);
            Assert.IsNotNull(nodeInfo.JreAvailableProcessors);
            Assert.IsNotNull(nodeInfo.JreFreeMemory);
            Assert.IsNotNull(nodeInfo.JreMaxMemory);
            Assert.IsNotNull(nodeInfo.JreTotalMemory);
            Assert.IsNotNull(nodeInfo.LatestMilestone);
            Assert.IsNotNull(nodeInfo.LatestMilestoneIndex);
            Assert.IsNotNull(nodeInfo.LatestSolidSubtangleMilestone);
            Assert.IsNotNull(nodeInfo.LatestSolidSubtangleMilestoneIndex);
            Assert.IsNotNull(nodeInfo.Neighbors);
            Assert.IsNotNull(nodeInfo.PacketsQueueSize);
            Assert.IsNotNull(nodeInfo.Time);
            Assert.IsNotNull(nodeInfo.Tips);
            Assert.IsNotNull(nodeInfo.TransactionsToRequest);
        }

        [TestMethod]
        public void ShouldGetNodeInfoWithHttps()
        {
            var iotaApi = new IotaApi("nodes.iota.cafe", 443, "https");

            var nodeInfo = iotaApi.GetNodeInfo();
            Assert.IsNotNull(nodeInfo.AppVersion);
            Assert.IsNotNull(nodeInfo.AppName);
            Assert.IsNotNull(nodeInfo.JreVersion);
            Assert.IsNotNull(nodeInfo.JreAvailableProcessors);
            Assert.IsNotNull(nodeInfo.JreFreeMemory);
            Assert.IsNotNull(nodeInfo.JreMaxMemory);
            Assert.IsNotNull(nodeInfo.JreTotalMemory);
            Assert.IsNotNull(nodeInfo.LatestMilestone);
            Assert.IsNotNull(nodeInfo.LatestMilestoneIndex);
            Assert.IsNotNull(nodeInfo.LatestSolidSubtangleMilestone);
            Assert.IsNotNull(nodeInfo.LatestSolidSubtangleMilestoneIndex);
            Assert.IsNotNull(nodeInfo.Neighbors);
            Assert.IsNotNull(nodeInfo.PacketsQueueSize);
            Assert.IsNotNull(nodeInfo.Time);
            Assert.IsNotNull(nodeInfo.Tips);
            Assert.IsNotNull(nodeInfo.TransactionsToRequest);
        }


        [TestMethod]
        public void ShouldGetNeighbors()
        {
            var neighbors = _iotaApi.GetNeighbors();
            Assert.IsNotNull(neighbors.Neighbors);
        }

        [TestMethod]
        public void ShouldAddNeighbors()
        {
            try
            {
                var res = _iotaApi.AddNeighbors("udp://8.8.8.8:14265");
                Assert.IsNotNull(res);
            }
            catch (IotaApiException e)
            {
                Assert.IsTrue(e.Message.Contains("not available on this node"));
            }
        }

        [TestMethod]
        public void ShouldRemoveNeighbors()
        {
            try
            {
                var res = _iotaApi.RemoveNeighbors("udp://8.8.8.8:14265");
                Assert.IsNotNull(res);
            }
            catch (IotaApiException e)
            {
                Assert.IsTrue(e.Message.Contains("not available on this node"));
            }
        }

        [TestMethod]
        public void ShouldGetTips()
        {
            var tips = _iotaApi.GetTips();
            Assert.IsNotNull(tips);
        }

        [TestMethod]
        public void ShouldFindTransactionsByAddresses()
        {
            var trans = _iotaApi.FindTransactionsByAddresses(TEST_ADDRESS_WITH_CHECKSUM);
            Assert.IsNotNull(trans.Hashes);
            Assert.IsTrue(trans.Hashes.Count > 0);
        }

        [TestMethod]
        public void ShouldFindTransactionsByApprovees()
        {
            var trans = _iotaApi.FindTransactionsByApprovees(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldFindTransactionsByBundles()
        {
            var trans = _iotaApi.FindTransactionsByBundles(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldFindTransactionsByDigests()
        {
            var trans = _iotaApi.FindTransactionsByDigests(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldGetTrytes()
        {
            var res = _iotaApi.GetTrytes(TEST_HASH);
            Assert.IsNotNull(res.Trytes);
        }

        [TestMethod]
        [ExpectedException(typeof(IotaApiException), "One of the tips absents")]
        public void ShouldNotGetInclusionStates()
        {
            var res = _iotaApi.GetInclusionStates(new[] {TEST_HASH},
                new[] {"DNSBRJWNOVUCQPILOQIFDKBFJMVOTGHLIMLLRXOHFTJZGRHJUEDAOWXQRYGDI9KHYFGYDWQJZKX999999"});
            Assert.IsNotNull(res.States);
        }

        [TestMethod]
        public void ShouldGetInclusionStates()
        {
            var res =
                _iotaApi.GetInclusionStates(
                    new[] {TEST_HASH},
                    new[] {_iotaApi.GetNodeInfo().LatestSolidSubtangleMilestone});
            Assert.IsNotNull(res.States);
        }

        [TestMethod] // very long execution
        public void ShouldGetTransactionsToApprove()
        {
            var res = _iotaApi.GetTransactionsToApprove(15);
            Assert.IsNotNull(res.TrunkTransaction);
            Assert.IsNotNull(res.BranchTransaction);
        }

        [TestMethod]
        public void ShouldFindTransactions()
        {
            var test = TEST_BUNDLE;
            // ReSharper disable once UnusedVariable
            var resp = _iotaApi.FindTransactions(new[] {test}.ToList(),
                new[] {test}.ToList(), new[] {test}.ToList(), new[] {test}.ToList());
        }

        [TestMethod]
        public void ShouldGetBalances()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var res = _iotaApi.GetBalances(new[] {TEST_ADDRESS_WITH_CHECKSUM}.ToList(), 100);
            Assert.IsNotNull(res.Balances);
            Assert.IsNotNull(res.References);
            Assert.IsNotNull(res.MilestoneIndex);
        }
    }
}