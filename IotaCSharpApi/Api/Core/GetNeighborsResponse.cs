using System.Collections.Generic;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class GetNeighborsResponse
    {
        public List<Neighbor> Neighbors { get; set; }
    }
}