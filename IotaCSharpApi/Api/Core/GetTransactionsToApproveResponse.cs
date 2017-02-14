namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="GetTransactionsToApproveRequest"/>
    /// </summary>
    public class GetTransactionsToApproveResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        /// The trunk transaction.
        /// </value>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        /// The branch transaction.
        /// </value>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}";
        }
    }
}