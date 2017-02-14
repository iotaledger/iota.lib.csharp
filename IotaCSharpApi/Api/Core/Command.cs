using System.ComponentModel;

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
        [Description("getNodeInfo")] GetNodeInfo,

        /// <summary>
        /// Gets the tips of the node
        /// </summary>
        [Description("getTips")] GetTips,

        /// <summary>
        /// Finds the transactions using different search criteria <see cref="FindTransactionsRequest"/>
        /// </summary>
        [Description("findTransactions")] FindTransactions,

        /// <summary>
        /// Gets the transactions to approve
        /// </summary>
        [Description("getTransactionsToApprove")] GetTransactionsToApprove,

        /// <summary>
        /// Attaches to the tangle
        /// </summary>
        [Description("attachToTangle")] AttachToTangle,

        /// <summary>
        /// Gets the balances
        /// </summary>
        [Description("getBalances")] GetBalances,

        /// <summary>
        /// Gets the inclusion state
        /// </summary>
        [Description("getInclusionStates")] GetInclusionStates,

        /// <summary>
        /// Gets the trytes
        /// </summary>
        [Description("getTrytes")] GetTrytes,

        /// <summary>
        /// Gets the neighbours of the node
        /// </summary>
        [Description("getNeighbors")] GetNeighbors,

        /// <summary>
        /// Adds neighbours to the node
        /// </summary>
        [Description("addNeighbors")] AddNeighbors,

        /// <summary>
        /// Removes neighbours from the node
        /// </summary>
        [Description("removeNeighbors")] RemoveNeighbors,

        /// <summary>
        /// Interrupt attaching to the tangle
        /// </summary>
        [Description("interruptAttachingToTangle")] InterruptAttachingToTangle,

        /// <summary>
        /// Broadcasts transactions
        /// </summary>
        [Description("broadcastTransactions")] BroadcastTransactions,

        /// <summary>
        /// Stores transactions
        /// </summary>
        [Description("storeTransactions")] StoreTransactions
    }
}