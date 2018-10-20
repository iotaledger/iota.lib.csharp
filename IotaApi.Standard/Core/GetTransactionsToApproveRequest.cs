namespace Iota.Api.Core
{
    /// <summary>
    /// This class represents the core API call 'GetTransactionsToApprove'
    /// </summary>
    public class GetTransactionsToApproveRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTransactionsToApproveRequest"/> class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <param name="reference"></param>
        public GetTransactionsToApproveRequest(int depth, string reference)
            : base(Core.Command.GetTransactionsToApprove.GetCommandString())
        {
            Depth = depth;
            Reference = reference;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get; }

        public string Reference { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth},{nameof(Reference)}: {Reference}";
        }
    }
}