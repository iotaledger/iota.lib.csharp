using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetInclusionStatesResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<bool> States { get; set; }
    }
}
