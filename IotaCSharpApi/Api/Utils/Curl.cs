using System;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// (c) 2016 Come-from-Beyond
    /// 
    /// Curl belongs to the sponge function family.
    /// 
    /// </summary>
    public class Curl : ICurl
    {
        public const int HASH_LENGTH = 243;
        private static readonly int STATE_LENGTH = 3*HASH_LENGTH;

        private const int NUMBER_OF_ROUNDS = 27;
        private static readonly int[] TRUTH_TABLE = {1, 0, -1, 1, -1, 0, -1, 1, 0};

        private int[] state = new int[STATE_LENGTH];

        public ICurl Clone()
        {
            return new Curl();
        }

        public ICurl Absorb(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(trits, offset, state, 0, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);

            return this;
        }

        public ICurl Absorb(int[] trits)
        {
            return Absorb(trits, 0, trits.Length);
        }

        public int[] Squeeze(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(state, 0, trits, offset, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);

            return state;
        }

        public int[] Squeeze(int[] trits)
        {
            return Squeeze(trits, 0, trits.Length);
        }


        public ICurl Transform()
        {
            int[] scratchpad = new int[STATE_LENGTH];
            int scratchpadIndex = 0;
            for (int round = 0; round < NUMBER_OF_ROUNDS; round++)
            {
                Array.Copy(state, 0, scratchpad, 0, STATE_LENGTH);
                for (int stateIndex = 0; stateIndex < STATE_LENGTH; stateIndex++)
                {
                    state[stateIndex] =
                        TRUTH_TABLE[
                            scratchpad[scratchpadIndex] +
                            scratchpad[scratchpadIndex += (scratchpadIndex < 365 ? 364 : -365)]*3 + 4];
                }
            }

            return this;
        }

        public ICurl Reset()
        {
            for (int stateIndex = 0; stateIndex < STATE_LENGTH; stateIndex++)
            {
                state[stateIndex] = 0;
            }

            return this;
        }

        public int[] State
        {
            get { return state; }
            set { state = value; }
        }
    }
}