using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTrytesResponse:IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Trytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Trytes)}: {string.Join(",", Trytes)}";
        }
    }
}
