using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetBalancesResponse:IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<long> Balances { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> References { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MilestoneIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{nameof(Balances)}: {string.Join(",", Balances)}, {nameof(References)}: {string.Join(",", References)}, {nameof(MilestoneIndex)}: {MilestoneIndex}";
        }
    }
}
