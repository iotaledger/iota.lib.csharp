using System;

namespace Iota.MAM.Merkle
{
    public class MerkleNode
    {
        public MerkleNode(MerkleNode left, int[] hash, MerkleNode right)
        {
            Hash = hash;
            LeftNode = left;
            RightNode = right;

            if (LeftNode != null)
                LeftNode.Parent = this;

            if (RightNode != null)
                RightNode.Parent = this;
        }

        public int[] Hash { get; protected set; }
        public MerkleNode LeftNode { get; protected set; }
        public MerkleNode RightNode { get; protected set; }
        public MerkleNode Parent { get; protected set; }
        public bool IsLeaf => LeftNode == null && RightNode == null;
        public bool IsFullNode => LeftNode != null && RightNode != null;

        public int Size
        {
            get
            {
                if (IsLeaf)
                    return 1;

                int leftSize = LeftNode?.Size ?? 0;
                int rightSize = RightNode?.Size ?? 0;

                return 1 + leftSize + rightSize;
            }
        }

        public int Depth
        {
            get
            {
                if (IsLeaf)
                    return 1;

                if (LeftNode != null)
                    return 1 + LeftNode.Depth;

                if (LeftNode == null && RightNode != null)
                    throw new Exception("Error!");

                return 0;
            }
        }

        public int Count
        {
            get
            {
                if (IsLeaf)
                    return 1;

                int leftCount = LeftNode?.Count ?? 0;
                int rightCount = RightNode?.Count ?? 0;

                return leftCount + rightCount;
            }
        }

        public int[] Slice()
        {
            int[] clone = new int[Hash.Length];
            Array.Copy(Hash, clone, Hash.Length);
            return clone;
        }

    }
}
