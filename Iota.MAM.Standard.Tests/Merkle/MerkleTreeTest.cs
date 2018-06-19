using Iota.Api.Pow;
using Iota.Api.Utils;
using Iota.MAM.Merkle;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.MAM.Tests.Merkle
{
    [TestClass]
    public class MerkleTreeTest
    {
        [TestMethod]
        public void ItDoesNotPanic()
        {
            string seed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ9ABCDEFGHIJKLMNOPQRSTUVWXYZ9ABCDEFGHIJKLMNOPQRSTUVWXYZ9";
            seed = InputValidator.PadSeedIfNecessary(seed);

            int[] trits = Converter.ToTrits(seed);

            ICurl c1 = new Curl(CurlMode.CurlP27);
            ICurl c2 = new Curl(CurlMode.CurlP27);
            ICurl c3 = new Curl(CurlMode.CurlP27);

            int start = 1;
            int index = 5;
            int treeDepth = 5;
            int leafCount = 9;
            int security = 1;
            //int[] digest = new int[MAM.Utils.Constants.DigestLength];

            var rootNode = MerkleTree.CreateMerkleTree(
                trits, start, (uint) leafCount, security, c1, c2, c3);

            var someBranch = MerkleTree.CreateMerkleBranch(rootNode, index);

            Assert.AreEqual(20, rootNode.Size);
            Assert.AreEqual(treeDepth, rootNode.Depth);
            Assert.AreEqual(leafCount, rootNode.Count);

            int branchLength = someBranch.Length;
            Assert.AreEqual(treeDepth - 1, branchLength);
        }
    }
}
