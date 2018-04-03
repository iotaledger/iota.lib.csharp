namespace Iota.Api.Pow
{
    /// <summary>
    /// This interface abstracts the curl hashing algorithm
    /// </summary>
    public interface ICurl
    {
        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        void Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        void Absorb(int[] trits);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the squeezed trits</returns>
        void Squeeze(int[] trits, int offset, int length);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the squeezed trits</returns>
        void Squeeze(int[] trits);


        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICurl Clone();
    }
}