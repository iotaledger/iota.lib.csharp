using System;
using IotaSharp.MAM.Utils;
using IotaSharp.Pow;

namespace IotaSharp.MAM.Merkle
{
    public class MerkleTree
    {
        private static readonly sbyte[] NullHash = new sbyte[Constants.HashLength];

        public static MerkleNode CreateMerkleTree(
            sbyte[] seed, int index, uint count,
            int security,
            ICurl c1,
            ICurl c2,
            ICurl c3)
        {
            if (count == 0)
                return null;

            if (count == 1)
                return CreateMerkleLeaf(seed, index, security, c1, c2, c3);

            uint ct = NextPowerOfTwo(count);
            return CreateMerkleNode(seed, index, count, ct, security, c1, c2, c3);
        }

        public static MerkleSibling CreateMerkleBranch(
            MerkleNode merkleTree, int index)
        {
            if (merkleTree == null || index >= merkleTree.Count)
                throw new ArgumentOutOfRangeException();

            if (merkleTree.IsLeaf)
                return null;

            int leftCount = merkleTree.LeftNode.Count;
            bool goLeft = index < leftCount;

            MerkleNode sibling;
            MerkleNode child;
            if (goLeft)
            {
                sibling = merkleTree.RightNode;
                child = merkleTree.LeftNode;
            }
            else
            {
                sibling = merkleTree.LeftNode;
                child = merkleTree.RightNode;
            }

            sbyte[] hash = sibling.Slice();

            if (!goLeft)
            {
                index = index - leftCount;
            }

            MerkleSibling nextSibling = CreateMerkleBranch(child, index);

            return new MerkleSibling(hash, nextSibling);
        }

        // root
        public static int Root(sbyte[] address, sbyte[] hashes, int index, ICurl curl)
        {
            int i = 1;
            int numBeforeEnd = 0;
            sbyte[] outTrits = new sbyte[Constants.HashLength];
            Array.Copy(address, outTrits, Constants.HashLength);

            for (int n = 0; n < hashes.Length / Constants.HashLength; n++)
            {
                curl.Reset();
                if ((i & index) == 0)
                {
                    numBeforeEnd += 1;

                    curl.Absorb(outTrits);
                    curl.Absorb(hashes, n * Constants.HashLength, Constants.HashLength);
                }
                else
                {
                    curl.Absorb(hashes, n * Constants.HashLength, Constants.HashLength);
                    curl.Absorb(outTrits);
                }

                i <<= 1;

                Array.Copy(curl.Rate, outTrits, Constants.HashLength);

            }

            return numBeforeEnd;
        }

        #region Private Method

        private static MerkleNode CreateMerkleNode(
            sbyte[] seed, int index,
            uint remainingCount, uint remainingWidth,
            int security,
            ICurl c1, ICurl c2, ICurl c3)
        {
            if (remainingCount == 0)
                return null;

            if (remainingWidth == 0)
                return null;

            if (remainingWidth == 1)
                return CreateMerkleLeaf(seed, index, security, c1, c2, c3);

            return Combine(seed, index, remainingCount, remainingWidth, security, c1, c2, c3);
        }

        private static MerkleNode Combine(
            sbyte[] seed, int index,
            uint count, uint remainingWidth,
            int security,
            ICurl c1, ICurl c2, ICurl c3)
        {
            uint rightCount = NextPowerOfTwo(count) >> 1;
            uint leftCount = count - rightCount;

            var left = CreateMerkleNode(seed, index, leftCount, remainingWidth >> 1, security, c1, c2, c3);
            var right = CreateMerkleNode(seed, (int) (index + leftCount), rightCount, remainingWidth >> 1, security, c1,
                c2,
                c3);

            if (left != null && (left.IsLeaf || left.IsFullNode))
            {
                c1.Absorb(left.Hash);
            }
            else
            {
                c1.Absorb(NullHash);
            }

            if (right != null && (right.IsLeaf || right.IsFullNode))
            {
                c1.Absorb(right.Hash);
            }
            else
            {
                c1.Absorb(NullHash);
            }

            sbyte[] hash = new sbyte[Constants.HashLength];
            c1.Squeeze(hash, 0, hash.Length);
            c1.Reset();

            return new MerkleNode(left, hash, right);

        }

        private static MerkleNode CreateMerkleLeaf(
            sbyte[] seed, int index,
            int security,
            ICurl c1, ICurl c2, ICurl c3)
        {
            sbyte[] hash = ComputeHash(seed, index, security, c1, c2, c3);

            return new MerkleNode(null, hash, null);
        }

        private static sbyte[] ComputeHash(
            sbyte[] seed, int index,
            int security,
            ICurl c1, ICurl c2, ICurl c3)
        {
            sbyte[] subseed = HashHelper.Subseed(seed, index, c1);

            c1.Reset();

            //iss::subseed_to_digest(&subseed, security, &mut hash, c1, c2, c3);
            sbyte[] digest = HashHelper.SubseedToDigest(subseed, security, c1, c2, c3);

            c1.Reset();
            c2.Reset();
            c3.Reset();

            //iss::address(&mut hash, c1);
            sbyte[] address = HashHelper.Address(digest, c1);

            c1.Reset();

            return address;
        }

        private static uint NextPowerOfTwo(uint count)
        {
            int i = 0;
            uint result = 0;
            for (; i < 32; i++)
            {
                result = (uint) 1 << i;
                if (count <= result)
                    break;
            }

            if (i < 32)
                return result;

            throw new ArgumentOutOfRangeException();

        }

        #endregion
    }
}
