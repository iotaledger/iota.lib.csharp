using System;

namespace Iota.Api.Pow
{
    /// <summary>
    /// </summary>
    public abstract class Sponge : ICurl
    {
        /// <summary>
        /// </summary>
        public const int HashLength = 243;

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public abstract void Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        public void Absorb(int[] trits)
        {
            Absorb(trits, 0, trits.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract void Squeeze(int[] trits, int offset, int length);

        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public void Squeeze(int[] trits)
        {
            Squeeze(trits, 0, trits.Length);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public abstract void Reset();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public abstract ICurl Clone();

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        public int[] State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Rate
        {
            get
            {
                if (State != null && State.Length >= HashLength)
                {
                    int[] rate = new int[HashLength];
                    Array.Copy(State, rate, HashLength);
                    return rate;
                }

                return null;
            }
        }
    }
}