using System;
using Constants = Iota.MAM.Utils.Constants;

namespace Iota.MAM.Merkle
{
    public class MerkleSibling
    {
        public MerkleSibling(int[] hash, MerkleSibling nextSibling)
        {
            Hash = hash;
            NextSibling = nextSibling;
        }

        public int[] Hash { get; protected set; }
        public MerkleSibling NextSibling { get; protected set; }

        public int Length
        {
            get
            {
                if (NextSibling == null)
                    return 1;

                return 1 + NextSibling.Length;
            }
        }

        public int[] Siblings
        {
            get
            {
                int tritsLength = Length * Constants.HashLength;
                int[] trits = new int[tritsLength];

                WriteBranch(tritsLength - Constants.HashLength, trits);

                return trits;
            }
        }

        private void WriteBranch(int index, int[] trits)
        {
            Array.Copy(Hash, 0, trits, index, Constants.HashLength);
            if (index != 0)
            {
                NextSibling.WriteBranch(index - Constants.HashLength, trits);
            }
        }
    }
}
