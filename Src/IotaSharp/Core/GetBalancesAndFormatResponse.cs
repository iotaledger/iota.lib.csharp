using System.Collections.Generic;
using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetBalancesAndFormatResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="totalBalance"></param>
        /// <param name="duration"></param>
        public GetBalancesAndFormatResponse(List<Input> inputs, long totalBalance, long duration)
        {
            Inputs = inputs;
            TotalBalance = totalBalance;
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Input> Inputs { get; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBalance { get; }
    }
}
