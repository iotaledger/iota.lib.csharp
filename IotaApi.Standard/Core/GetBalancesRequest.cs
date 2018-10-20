using System.Collections.Generic;

namespace Iota.Api.Core
{
    /// <summary>
    /// This class represents the core api request 'GetBalances'
    /// </summary>
    public class GetBalancesRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBalancesRequest"/> class.
        /// </summary>
        /// <param name="threshold">The confirmation threshold, should be set to 100.</param>
        /// <param name="addresses">The array list of addresses you want to get the confirmed balance from.</param>
        /// <param name="tips">The starting points we walk back from to find the balance of the addresses</param>
        public GetBalancesRequest(long threshold, List<string> addresses, List<string> tips)
            : base(Core.Command.GetBalances.GetCommandString())
        {
            Addresses = addresses;
            Threshold = threshold;
            Tips = tips;
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
        /// The starting points we walk back from to find the balance of the addresses
        /// </summary>
        public List<string> Tips { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(Threshold)}: {Threshold}, {nameof(Addresses)}: {string.Join(",", Addresses)},{nameof(Tips)}: {string.Join(",", Tips)}";
        }
    }
}