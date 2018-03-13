namespace Iota.Lib.CSharp.Api.Pow
{
    /// <summary>
    /// This interface abstracts the curl hashing algorithm
    /// </summary>
    public interface ICurl
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        ICurl Clone();

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Absorb(int[] trits);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the squeezed trits</returns>
        int[] Squeeze(int[] trits, int offset, int length);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the squeezed trits</returns>
        int[] Squeeze(int[] trits);

        /// <summary>
        /// Transforms this instance.
        /// </summary>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Transform();

        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        ICurl Reset();

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        int[] State { get; set; }
    }
}