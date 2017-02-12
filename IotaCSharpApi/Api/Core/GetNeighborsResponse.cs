using System.Collections.Generic;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Response of <see cref="GetNeighborsRequest"/>
    /// </summary>
    public class GetNeighborsResponse
    {
        public List<Neighbor> Neighbors { get; set; }
    }
}