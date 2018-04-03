using System.Collections.Generic;

namespace Iota.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="GetTipsRequest"/>
    /// </summary>
    public class GetTipsResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the hashes.
        /// </summary>
        /// <value>
        /// The hashes.
        /// </value>
        public List<string> Hashes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Hashes)}: {string.Join(",", Hashes)}";
        }
    }
}