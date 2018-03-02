namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This enumeration defines the core API call commands
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// Get information about the node.
        /// </summary>
        GetNodeInfo,

        /// <summary>
        /// Gets the tips of the node
        /// </summary>
        GetTips,

        /// <summary>
        /// Finds the transactions using different search criteria <see cref="FindTransactionsRequest"/>
        /// </summary>
        FindTransactions,

        /// <summary>
        /// Gets the transactions to approve
        /// </summary>
        GetTransactionsToApprove,

        /// <summary>
        /// Attaches to the tangle
        /// </summary>
        AttachToTangle,

        /// <summary>
        /// Gets the balances
        /// </summary>
        GetBalances,

        /// <summary>
        /// Gets the inclusion state
        /// </summary>
        GetInclusionStates,

        /// <summary>
        /// Gets the trytes
        /// </summary>
        GetTrytes,

        /// <summary>
        /// Gets the neighbours of the node
        /// </summary>
        GetNeighbors,

        /// <summary>
        /// Adds neighbours to the node
        /// </summary>
        AddNeighbors,

        /// <summary>
        /// Removes neighbours from the node
        /// </summary>
        RemoveNeighbors,

        /// <summary>
        /// Interrupt attaching to the tangle
        /// </summary>
        InterruptAttachingToTangle,

        /// <summary>
        /// Broadcasts transactions
        /// </summary>
        BroadcastTransactions,

        /// <summary>
        /// Stores transactions
        /// </summary>
        StoreTransactions
    }
}