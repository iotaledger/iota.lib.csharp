using System;
using IotaSharp.MAM.Utils;

namespace IotaSharp.MAM.Merkle
{
    /// <summary>
    /// 
    /// </summary>
    public class MerkleSibling
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="nextSibling"></param>
        public MerkleSibling(sbyte[] hash, MerkleSibling nextSibling)
        {
            Hash = hash;
            NextSibling = nextSibling;
        }

        /// <summary>
        /// 
        /// </summary>
        public sbyte[] Hash { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public MerkleSibling NextSibling { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get
            {
                if (NextSibling == null)
                    return 1;

                return 1 + NextSibling.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sbyte[] Siblings
        {
            get
            {
                int tritsLength = Length * Constants.HashLength;
                sbyte[] trits = new sbyte[tritsLength];

                WriteBranch(tritsLength - Constants.HashLength, trits);

                return trits;
            }
        }

        private void WriteBranch(int index, sbyte[] trits)
        {
            Array.Copy(Hash, 0, trits, index, Constants.HashLength);
            if (index != 0)
            {
                NextSibling.WriteBranch(index - Constants.HashLength, trits);
            }
        }
    }
}
