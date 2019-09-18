using System;
using System.Diagnostics.CodeAnalysis;

namespace IotaSharp.Pow
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class Sponge : ICurl
    {
        /// <summary>
        /// 
        /// </summary>
        public const int HASH_LENGTH = 243;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract ICurl Absorb(sbyte[] trits, int offset, int length);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public ICurl Absorb(sbyte[] trits)
        {
            return Absorb(trits, 0, trits.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract sbyte[] Squeeze(sbyte[] trits, int offset, int length);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public sbyte[] Squeeze(sbyte[] trits)
        {
            return Squeeze(trits, 0, trits.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ICurl Reset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ICurl Clone();

        /// <summary>
        /// for Curl
        /// </summary>
        public sbyte[] State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public sbyte[] Rate
        {
            get
            {
                if (State != null && State.Length >= HASH_LENGTH)
                {
                    sbyte[] rate = new sbyte[HASH_LENGTH];
                    Array.Copy(State, rate, HASH_LENGTH);
                    return rate;
                }

                return null;
            }
        }
    }
}
