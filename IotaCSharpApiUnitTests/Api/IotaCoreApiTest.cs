using System.Linq;
using Iota.Lib.CSharp.Api;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api
{
    [TestClass]
    public class IotaCoreApiTest
    {
        private static string TEST_BUNDLE =
            "XZKJUUMQOYUQFKMWQZNTFMSS9FKJLOEV9DXXXWPMQRTNCOUSUQNTBIJTVORLOQPLYZOTMLFRHYKMTGZZU";

        private static string TEST_ADDRESS_WITH_CHECKSUM =
            "PNGMCSNRCTRHCHPXYTPKEJYPCOWKOMRXZFHH9N9VDIKMNVAZCMIYRHVJIAZARZTUETJVFDMBEBIQE9QTHBFWDAOEFA";

        private static string TEST_HASH =
            "OAATQS9VQLSXCLDJVJJVYUGONXAXOFMJOZNSYWRZSWECMXAQQURHQBJNLD9IOFEPGZEPEMPXCIVRX9999";

        private static IotaApi iotaApi;

        [TestInitialize]
        public void createProxyInstance()
        {
            iotaApi = new IotaApi("node.iotawallet.info", 14265);
        }

        [TestMethod]
        public void shouldGetNodeInfo()
        {
            GetNodeInfoResponse nodeInfo = iotaApi.GetNodeInfo();
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
        public void shouldGetNeighbors()
        {
            GetNeighborsResponse neighbors = iotaApi.GetNeighbors();
            Assert.IsNotNull(neighbors.Neighbors);
        }

        [TestMethod]
        public void shouldAddNeighbors()
        {
            AddNeighborsResponse res = iotaApi.AddNeighbors("udp://8.8.8.8:14265");
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void shouldRemoveNeighbors()
        {
            RemoveNeighborsResponse res = iotaApi.RemoveNeighbors("udp://8.8.8.8:14265");
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void shouldGetTips()
        {
            GetTipsResponse tips = iotaApi.GetTips();
            Assert.IsNotNull(tips);
        }

        [TestMethod]
        public void shouldFindTransactionsByAddresses()
        {
            FindTransactionsResponse trans = iotaApi.FindTransactionsByAddresses(TEST_ADDRESS_WITH_CHECKSUM);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void shouldFindTransactionsByApprovees()
        {
            FindTransactionsResponse trans = iotaApi.FindTransactionsByApprovees(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void shouldFindTransactionsByBundles()
        {
            FindTransactionsResponse trans = iotaApi.FindTransactionsByBundles(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void shouldFindTransactionsByDigests()
        {
            FindTransactionsResponse trans = iotaApi.FindTransactionsByDigests(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void shouldGetTrytes()
        {
            GetTrytesResponse res = iotaApi.GetTrytes(TEST_HASH);
            Assert.IsNotNull(res.Trytes);
        }

        [TestMethod, ExpectedException(typeof(IotaApiException), "One of the tips absents")]
        public void shouldNotGetInclusionStates()
        {
            GetInclusionStatesResponse res = iotaApi.GetInclusionStates(new[] {TEST_ADDRESS_WITH_CHECKSUM},
                new[] {"DNSBRJWNOVUCQPILOQIFDKBFJMVOTGHLIMLLRXOHFTJZGRHJUEDAOWXQRYGDI9KHYFGYDWQJZKX999999"});
            Assert.IsNotNull(res.States);
        }

        [TestMethod]
        public void shouldGetInclusionStates()
        {
            GetInclusionStatesResponse res =
                iotaApi.GetInclusionStates(
                    new[] {"DBPECSH9YLSSTQDGERUHJBBJTKVUDBMTJLG9WPHBINGHIFOSJMDJLARTVOXXWEFQJLLBINOHCZGYFSMUEXWPPMTOFW"},
                    new[] { iotaApi.GetNodeInfo().LatestSolidSubtangleMilestone });
            Assert.IsNotNull(res.States);
        }

        [TestMethod] // very long execution
        public void shouldGetTransactionsToApprove()
        {
            GetTransactionsToApproveResponse res = iotaApi.GetTransactionsToApprove(27);
            Assert.IsNotNull(res.TrunkTransaction);
            Assert.IsNotNull(res.BranchTransaction);
        }

        [TestMethod]
        public void shouldFindTransactions()
        {
            string test = TEST_BUNDLE;
            FindTransactionsResponse resp = iotaApi.FindTransactions(new[] {test}.ToList(),
                new[] {test}.ToList(), new[] {test}.ToList(), new[] {test}.ToList());
        }

        [TestMethod]
        public void shouldGetBalances()
        {
            GetBalancesResponse res = iotaApi.GetBalances(new[] {TEST_ADDRESS_WITH_CHECKSUM}.ToList(), 100);
            Assert.IsNotNull(res.Balances);
            Assert.IsNotNull(res.Milestone);
            Assert.IsNotNull(res.MilestoneIndex);
        }
    }
}