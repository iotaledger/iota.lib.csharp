using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetBalancesRequest:IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threshold"></param>
        /// <param name="addresses"></param>
        /// <param name="tips"></param>
        public GetBalancesRequest(long threshold, List<string> addresses, List<string> tips)
            : base(Core.Command.GetBalances)
        {
            Addresses = addresses;
            Threshold = threshold;
            Tips = tips;
        }
        /// <summary>
        /// 
        /// </summary>
        public long Threshold { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Addresses { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Tips { get; }
    }
}
