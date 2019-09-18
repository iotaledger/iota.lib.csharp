using System.Collections.Generic;
using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetBundleResponse:IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="duration"></param>
        public GetBundleResponse(List<Transaction> transactions, long duration)
        {
            Transactions = transactions;
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Transaction> Transactions { get; }
    }
}
