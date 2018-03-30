using System.Collections.Generic;

namespace Iota.Api.Standard.Core
{

    /// <summary>
    /// This class represents the core API request 'AddNeighbors'.
    /// It is used to add a neighbor to the node
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class AddNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// Gets or sets the uris.
        /// </summary>
        /// <value>
        /// The uris.
        /// </value>
        public List<string> Uris { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNeighborsRequest"/> class.
        /// </summary>
        /// <param name="uris">The uris of the neighbors to add.</param>
        public AddNeighborsRequest(List<string> uris) : base(Core.Command.AddNeighbors.GetCommandString())
        {
            Uris = uris;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Uris)}: {string.Join(",", Uris)}";
        }
    }
}