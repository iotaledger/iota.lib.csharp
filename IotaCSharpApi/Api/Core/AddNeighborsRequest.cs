using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{

    /// <summary>
    /// Request to add a neighbor to the node
    /// </summary>
    /// <seealso cref="Iota.Lib.CSharp.Api.Core.IotaRequest" />
    public class AddNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// Gets or sets the uris.
        /// </summary>
        /// <value>
        /// The uris.
        /// </value>
        public List<string> Uris { get; set; }

        public AddNeighborsRequest(List<string> uris) : base(Core.Command.AddNeighbors.GetCommandString())
        {
            Uris = uris;
        }
    }
}