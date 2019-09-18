using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class IotaClientTest
    {
        private const string TEST_BUNDLE =
            "XZKJUUMQOYUQFKMWQZNTFMSS9FKJLOEV9DXXXWPMQRTNCOUSUQNTBIJTVORLOQPLYZOTMLFRHYKMTGZZU";

        private const string TEST_ADDRESS_UNSPENT =
            "D9UZTBEAT9DMZKMCPEKSBEOWPAUFWKOXWPO9LOHZVTE9HAVTAKHWAIXCJKDJFGUOBOULUFTJZKWTEKCHDAPJEJXEDD";

        private const string TEST_ADDRESS_SPENT =
            "9SEGQNQHFHCAI9QXTVGBGTIZQDV9RSCGCGPQSPLNCNN9DSENFMLTD9SETUSYZCYG9JYPIAMXFHNT9YRFZMMRCMESPB";

        private const string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "YJNQ9EQWSXUMLFCIUZDCAJZSAXUQNZSY9AKKVYKKFBAAHRSTKSHUOCCFTQVPPASPGGC9YGNLDQNOUWCAW";

        private const string TEST_ADDRESS_WITH_CHECKSUM =
            "YJNQ9EQWSXUMLFCIUZDCAJZSAXUQNZSY9AKKVYKKFBAAHRSTKSHUOCCFTQVPPASPGGC9YGNLDQNOUWCAWGWIJNRJMX";

        private const string TEST_HASH =
            "OOAARHCXXCPMNZPUEYOOUIUCTWZSQGKNIECIKRBNUUJEVMLJAWGCXREXEQGNJUJKUXXQAWWAZYKB99999";

        private const string TAG = "IOTA9TAG9999999999999999";

        private static IotaClient _iotaClient;

        [TestInitialize]
        public void CreateProxyInstance()
        {
            _iotaClient = new IotaClient("https://nodes.devnet.iota.org:443");
        }

        [TestMethod]
        public void ShouldGetNodeInfo()
        {
            var nodeInfo = _iotaClient.GetNodeInfo();
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

        [Ignore]
        [TestMethod]
        public void ShouldGetNeighbors()
        {

        }

        [Ignore]
        [TestMethod]
        public void ShouldAddNeighbors()
        {

        }

        [Ignore]
        [TestMethod]
        public void ShouldRemoveNeighbors()
        {

        }

        [TestMethod]
        public void ShouldGetTips()
        {
            var tips = _iotaClient.GetTips();
            Assert.IsNotNull(tips);
        }

        [TestMethod]
        public void ShouldFindTransactionsByAddresses()
        {
            var trans = _iotaClient.FindTransactionsByAddresses(TEST_ADDRESS_WITH_CHECKSUM);
            Assert.IsNotNull(trans.Hashes);
            Assert.IsTrue(trans.Hashes.Count > 0);
        }

        [TestMethod]
        public void ShouldFindTransactionsByApprovees()
        {
            var trans = _iotaClient.FindTransactionsByApprovees(TEST_ADDRESS_WITHOUT_CHECKSUM);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldFindTransactionsByBundles()
        {
            var trans = _iotaClient.FindTransactionsByBundles(TEST_HASH);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldFindTransactionsByTags()
        {
            var trans = _iotaClient.FindTransactionsByTags(TAG);
            Assert.IsNotNull(trans.Hashes);
        }

        [TestMethod]
        public void ShouldGetTrytes()
        {
            var res = _iotaClient.GetTrytes(TEST_HASH);
            Assert.IsNotNull(res.Trytes);
        }

        [TestMethod]
        public void ShouldNotGetInclusionStates()
        {
            var argumentException = Assert.ThrowsException<ArgumentException>(() =>
            {
                _iotaClient.GetInclusionStates(new[] {TEST_HASH},
                    new[] {"ZIJGAJ9AADLRPWNCYNNHUHRRAC9QOUDATEDQUMTNOTABUVRPTSTFQDGZKFYUUIE9ZEBIVCCXXXLKX9999"});
            });

            Assert.IsTrue(
                argumentException.Message.StartsWith("{\"error\":\"One of the tips is absent\",\"duration\":"));
        }

        [TestMethod]
        public void ShouldGetInclusionStates()
        {
            var res =
                _iotaClient.GetInclusionStates(
                    new[] {TEST_HASH},
                    new[] {_iotaClient.GetNodeInfo().LatestSolidSubtangleMilestone});
            Assert.IsNotNull(res.States);
        }

        [TestMethod] // very long execution
        public void ShouldGetTransactionsToApprove()
        {
            var res = _iotaClient.GetTransactionsToApprove(15);
            Assert.IsNotNull(res.TrunkTransaction);
            Assert.IsNotNull(res.BranchTransaction);
        }

        [TestMethod]
        public void ShouldInvalidDepth()
        {
            var argumentException = Assert.ThrowsException<ArgumentException>(() =>
            {
                _iotaClient.GetTransactionsToApprove(17);
            });

            Assert.IsTrue(
                argumentException.Message.StartsWith("{\"error\":\"Invalid depth input\",\"duration\":"));
        }

        [TestMethod]
        public void FindTransactionsWithValidTags()
        {
            var test = TEST_BUNDLE;

            var resp = _iotaClient.FindTransactions(
                new[] {test}.ToList(), new[] {TAG}.ToList(),
                new[] {test}.ToList(), new[] {test}.ToList());

            Assert.IsNotNull(resp);
        }

        [TestMethod]
        public void FindTransactionsFailIfInvalidTagIsProvided()
        {
            var test = TEST_BUNDLE;

            var argumentException = Assert.ThrowsException<ArgumentException>(() =>
            {
                _iotaClient.FindTransactions(
                    new[] {test}.ToList(), new[] {test}.ToList(),
                    new[] {test}.ToList(), new[] {test}.ToList());
            });

            Assert.AreEqual("Invalid tag provided.", argumentException.Message);
        }

        [TestMethod]
        public void ShouldGetBalances()
        {
            var res = _iotaClient.GetBalances(100, new List<string> {TEST_ADDRESS_WITH_CHECKSUM}, null);
            Assert.IsNotNull(res.Balances);
            Assert.IsNotNull(res.References);
            Assert.IsNotNull(res.MilestoneIndex);
            Assert.IsNotNull(res.Duration);
        }

        [TestMethod]
        public void AddressIsSpentFrom()
        {
            var ret = _iotaClient.WereAddressesSpentFrom(TEST_ADDRESS_SPENT);
            Assert.IsTrue(ret.States[0]);
        }

        [TestMethod]
        public void AddressIsNotSpentFrom()
        {
            var ret = _iotaClient.WereAddressesSpentFrom(TEST_ADDRESS_UNSPENT);
            Assert.IsFalse(ret.States[0]);
        }
    }
}
