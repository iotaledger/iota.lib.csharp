using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class FindTransactionsResponse: IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Hashes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Hashes)}: {string.Join(",", Hashes)}";
        }
    }
}
