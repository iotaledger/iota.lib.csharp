using System.Collections.Generic;

namespace Iota.Api.Standard.Core
{
    /// <summary>
    ///     Response of <see cref="GetBalancesRequest" />
    /// </summary>
    public class GetBalancesResponse : IotaResponse
    {
        /// <summary>
        ///     Gets or sets the balances.
        /// </summary>
        /// <value>
        ///     The balances.
        /// </value>
        public List<long> Balances { get; set; }

        /// <summary>
        ///     Gets or sets the references.
        /// </summary>
        /// <value>
        ///     The references.
        /// </value>
        public List<string> References { get; set; }

        /// <summary>
        ///     Gets or sets the index of the milestone.
        /// </summary>
        /// <value>
        ///     The index of the milestone.
        /// </value>
        public int MilestoneIndex { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"{nameof(Balances)}: {string.Join(",", Balances)}, {nameof(References)}: {string.Join(",", References)}, {nameof(MilestoneIndex)}: {MilestoneIndex}";
        }
    }
}