using System;
using System.Linq;
using Iota.Api.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable StringLiteralTypo

namespace Iota.Api.Tests
{
    [TestClass]
    public class IotaCoreApiTest
    {
        private static readonly string TEST_BUNDLE =
            "YNSXQFSIGLIRWCCBLEABYWVCYXMDR9YSVGFFVM9JPJWRCLTFNMDFEFBGPRPQDROB99N9KRPXFYSPRHMJD";

        private static readonly string TEST_ADDRESS_UNSPENT =
            "D9UZTBEAT9DMZKMCPEKSBEOWPAUFWKOXWPO9LOHZVTE9HAVTAKHWAIXCJKDJFGUOBOULUFTJZKWTEKCHD";

        private static readonly string TEST_ADDRESS_SPENT =
            "9SEGQNQHFHCAI9QXTVGBGTIZQDV9RSCGCGPQSPLNCNN9DSENFMLTD9SETUSYZCYG9JYPIAMXFHNT9YRFZ";

        private static readonly string TEST_ADDRESS_WITH_CHECKSUM =
            "YJNQ9EQWSXUMLFCIUZDCAJZSAXUQNZSY9AKKVYKKFBAAHRSTKSHUOCCFTQVPPASPGGC9YGNLDQNOUWCAWGWIJNRJMX";

        private static readonly string TEST_HASH =
            "OAATQS9VQLSXCLDJVJJVYUGONXAXOFMJOZNSYWRZSWECMXAQQURHQBJNLD9IOFEPGZEPEMPXCIVRX9999";

        private static IotaApi _iotaApi;

        [TestInitialize]
        public void CreateProxyInstance()
        {
            _iotaApi = new IotaApi("nodes.testnet.iota.org", 80);
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

        [Ignore]
        [TestMethod]
        public void ShouldGetNeighbors()
        {
            //getNeighbors is by default disabled
            //var neighbors = _iotaApi.GetNeighbors();
            //Assert.IsNotNull(neighbors.Neighbors);
        }

        [TestMethod]
        [ExpectedException(typeof(IllegalAccessError))]
        public void ShouldAddNeighbors()
        {
            var res = _iotaApi.AddNeighbors("udp://8.8.8.8:14265");
            Assert.IsNotNull(res);
        }

        [TestMethod]
        [ExpectedException(typeof(IllegalAccessError))]
        public void ShouldRemoveNeighbors()
        {
            var res = _iotaApi.RemoveNeighbors("udp://8.8.8.8:14265");
            Assert.IsNotNull(res);
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
        [ExpectedException(typeof(ArgumentException))]
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

        [TestMethod] // very long execution
        public void ShouldInvalidDepth()
        {
            try
            {
                _iotaApi.GetTransactionsToApprove(27);
                Assert.Fail("Depth more then 15 is not supported by default");
            }
            catch (ArgumentException)
            {
                //TODO verify correct error
                //Good!
            }
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
            var res = _iotaApi.GetBalances(100, new[] {TEST_ADDRESS_WITH_CHECKSUM}.ToList(), null);
            Assert.IsNotNull(res.Balances);
            Assert.IsNotNull(res.References);
            Assert.IsNotNull(res.MilestoneIndex);
            Assert.IsNotNull(res.Duration);
        }

        [TestMethod]
        public void InvalidAddressSpentFrom()
        {
            try
            {
                //Addresses with checksum aren't allowed, remove last 9 characters!
                _iotaApi.WereAddressesSpentFrom(TEST_ADDRESS_WITH_CHECKSUM);
                Assert.Fail("failed to throw error on wrong address hash");
            }
            catch (ArgumentException)
            {
                //TODO verify correct error
                //Success
            }
        }

        [TestMethod]
        public void AddressIsSpentFrom()
        {
            var ret = _iotaApi.WereAddressesSpentFrom(TEST_ADDRESS_SPENT);
            Assert.IsTrue(ret.States[0]);
        }

        [TestMethod]
        public void AddressIsNotSpentFrom()
        {
            var ret = _iotaApi.WereAddressesSpentFrom(TEST_ADDRESS_UNSPENT);
            Assert.IsFalse(ret.States[0]);
        }

    }
}