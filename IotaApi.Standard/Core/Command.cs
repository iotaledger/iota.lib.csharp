using System.ComponentModel;

namespace Iota.Api.Standard.Core
{
    /// <summary>
    /// This enumeration defines the core API call commands
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// Adds neighbours to the node
        /// </summary>
        [Description("addNeighbors")] AddNeighbors,

        /// <summary>
        /// Attaches to the tangle
        /// </summary>
        [Description("attachToTangle")] AttachToTangle,

        /// <summary>
        /// Broadcasts transactions
        /// </summary>
        [Description("broadcastTransactions")] BroadcastTransactions,

        /// <summary>
        /// Finds the transactions using different search criteria
        /// </summary>
        [Description("findTransactions")] FindTransactions,

        /// <summary>
        /// Gets the balances
        /// </summary>
        [Description("getBalances")] GetBalances,

        /// <summary>
        /// Gets the inclusion state
        /// </summary>
        [Description("getInclusionStates")] GetInclusionStates,

        /// <summary>
        /// Gets the neighbours of the node
        /// </summary>
        [Description("getNeighbors")] GetNeighbors,
        
        /// <summary>
        /// Get information about the node.
        /// </summary>
        [Description("getNodeInfo")] GetNodeInfo,

        /// <summary>
        /// Gets the tips of the node
        /// </summary>
        [Description("getTips")] GetTips,
        
        /// <summary>
        /// Gets the transactions to approve
        /// </summary>
        [Description("getTransactionsToApprove")] GetTransactionsToApprove,
        
        /// <summary>
        /// Gets the trytes
        /// </summary>
        [Description("getTrytes")] GetTrytes,

        /// <summary>
        /// Interrupt attaching to the tangle
        /// </summary>
        [Description("interruptAttachingToTangle")] InterruptAttachingToTangle,
        
        /// <summary>
        /// Removes neighbours from the node
        /// </summary>
        [Description("removeNeighbors")] RemoveNeighbors,
        
        /// <summary>
        /// Stores transactions
        /// </summary>
        [Description("storeTransactions")] StoreTransactions,

        /// <summary>
        /// Get Missing Transactions
        /// </summary>
        [Description("getMissingTransactions")] GetMissingTransactions,

        /// <summary>
        /// Check Consistency
        /// </summary>
        [Description("checkConsistency")] CheckConsistency,

        /// <summary>
        /// Were Addresses SpentFrom
        /// </summary>
        [Description("wereAddressesSpentFrom")] WereAddressesSpentFrom,

    }
}