using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the core api request 'GetBalances'
    /// </summary>
    public class GetBalancesRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBalancesRequest"/> class.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="threshold">The threshold.</param>
        public GetBalancesRequest(List<string> addresses, long threshold = 100)
            : base(Core.Command.GetBalances.GetCommandString())
        {
            Addresses = addresses;
            Threshold = threshold;
        }

        /// <summary>
        /// Gets the threshold.
        /// </summary>
        /// <value>
        /// The threshold.
        /// </value>
        public long Threshold { get; }

        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public List<string> Addresses { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Threshold)}: {Threshold}, {nameof(Addresses)}: {string.Join(",",Addresses)}";
        }
    }
}