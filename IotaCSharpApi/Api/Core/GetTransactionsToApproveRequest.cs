namespace Iota.Lib.CSharp.Api.Core
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
        public GetTransactionsToApproveRequest(int depth)
            : base(Core.Command.GetTransactionsToApprove.GetCommandString())
        {
            Depth = depth;
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth}";
        }
    }
}