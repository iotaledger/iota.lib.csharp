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
        /// <summary>
        /// The hash length
        /// </summary>
        public const int HashLength = 243;
        private static readonly int StateLength = 3*HashLength;
        private const int NumberOfRounds = 27;
        private static readonly int[] TruthTable = {1, 0, -1, 1, -1, 0, -1, 1, 0};

        private int[] state = new int[StateLength];

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>a new instance</returns>
        public ICurl Clone()
        {
            return new Curl();
        }

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        public ICurl Absorb(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(trits, offset, state, 0, length < HashLength ? length : HashLength);
                Transform();
                offset += HashLength;
            } while ((length -= HashLength) > 0);

            return this;
        }

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public ICurl Absorb(int[] trits)
        {
            return Absorb(trits, 0, trits.Length);
        }

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// the squeezed trits
        /// </returns>
        public int[] Squeeze(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(state, 0, trits, offset, length < HashLength ? length : HashLength);
                Transform();
                offset += HashLength;
            } while ((length -= HashLength) > 0);

            return state;
        }

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>
        /// the squeezed trits
        /// </returns>
        public int[] Squeeze(int[] trits)
        {
            return Squeeze(trits, 0, trits.Length);
        }

        /// <summary>
        /// Transforms this instance.
        /// </summary>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public ICurl Transform()
        {
            int[] scratchpad = new int[StateLength];
            int scratchpadIndex = 0;
            for (int round = 0; round < NumberOfRounds; round++)
            {
                Array.Copy(state, 0, scratchpad, 0, StateLength);
                for (int stateIndex = 0; stateIndex < StateLength; stateIndex++)
                {
                    state[stateIndex] =
                        TruthTable[
                            scratchpad[scratchpadIndex] +
                            scratchpad[scratchpadIndex += (scratchpadIndex < 365 ? 364 : -365)]*3 + 4];
                }
            }

            return this;
        }

        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public ICurl Reset()
        {
            for (int stateIndex = 0; stateIndex < StateLength; stateIndex++)
            {
                state[stateIndex] = 0;
            }

            return this;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public int[] State
        {
            get { return state; }
            set { state = value; }
        }
    }
}