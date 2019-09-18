using System.Collections.Generic;
using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetNeighborsResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Neighbor> Neighbors { get; set; }
    }
}
