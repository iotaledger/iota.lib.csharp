using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Response of <see cref="GetBalancesRequest"/>
    /// </summary>
    public class GetBalancesResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the balances.
        /// </summary>
        /// <value>
        /// The balances.
        /// </value>
        public List<long> Balances { get; set; }

        /// <summary>
        /// Gets or sets the milestone.
        /// </summary>
        /// <value>
        /// The milestone.
        /// </value>
        public string Milestone { get; set; }

        /// <summary>
        /// Gets or sets the index of the milestone.
        /// </summary>
        /// <value>
        /// The index of the milestone.
        /// </value>
        public int MilestoneIndex { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Balances)}: {string.Join(",", Balances)}, {nameof(Milestone)}: {Milestone}, {nameof(MilestoneIndex)}: {MilestoneIndex}";
        }
    }
}