namespace Iota.Lib.CSharp.Api.Pow
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Sponge : ICurl
    {

        /// <summary>
        /// 
        /// </summary>
        public const int HashLength = 243;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public abstract void Absorb(int[] trits, int offset, int length);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        public void Absorb(int[] trits) => Absorb(trits,0,trits.Length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract void Squeeze(int[] trits, int offset, int length);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public void Squeeze(int[] trits) => Squeeze(trits,0,trits.Length);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract void Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ICurl Clone();
    }
}
