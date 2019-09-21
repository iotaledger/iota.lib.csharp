namespace IotaSharp.Pow
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICurl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ICurl Absorb(sbyte[] trits, int offset, int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        ICurl Absorb(sbyte[] trits);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        sbyte[] Squeeze(sbyte[] trits, int offset, int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        sbyte[] Squeeze(sbyte[] trits);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICurl Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICurl Clone();

        /// <summary>
        /// 
        /// </summary>
        sbyte[] State { get; set; }

        /// <summary>
        /// HashLength of state
        /// </summary>
        sbyte[] Rate { get; }
    }
}
