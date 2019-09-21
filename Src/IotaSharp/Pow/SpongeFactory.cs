using System.Diagnostics.CodeAnalysis;

namespace IotaSharp.Pow
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class SpongeFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public enum Mode
        {
#pragma warning disable CS1591 
            CURLP81,
            CURLP27,
            KERL,
#pragma warning restore CS1591 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ICurl Create(Mode mode)
        {
            switch (mode)
            {
                case Mode.CURLP81:
                    return new Curl(mode);
                case Mode.CURLP27:
                    return new Curl(mode);
                case Mode.KERL:
                    return new Kerl();
                default:
                    return null;
            }
        }
    }
}
