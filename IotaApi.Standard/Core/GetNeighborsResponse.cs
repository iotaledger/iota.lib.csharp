using System.Collections.Generic;
using Iota.Api.Model;

namespace Iota.Api.Core
{
    /// <summary>
    /// Response of <see cref="GetNeighborsRequest"/>
    /// </summary>
    public class GetNeighborsResponse
    {
        /// <summary>
        /// Gets or sets the neighbors.
        /// </summary>
        /// <value>
        /// The neighbors.
        /// </value>
        public List<Neighbor> Neighbors { get; set; }
    }
}