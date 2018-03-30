using System.Collections.Generic;
using Iota.Api.Standard.Model;

namespace Iota.Api.Standard.Core
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