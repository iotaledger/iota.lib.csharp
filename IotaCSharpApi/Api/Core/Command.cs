using System.ComponentModel;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This enumeration defines the core API call commands
    /// </summary>
    public enum Command
    {
        [Description("getNodeInfo")] GetNodeInfo,

        [Description("getNewAddress")] GetNewAddress,

        [Description("getTips")] GetTips,

        [Description("getTransfers")] GetTransfers,

        [Description("getBundle")] GetBundle,

        [Description("transfer")] Transfer,

        [Description("findTransactions")] FindTransactions,

        [Description("getTransactionsToApprove")] GetTransactionsToApprove,

        [Description("attachToTangle")] AttachToTangle,

        [Description("getBalances")] GetBalances,

        [Description("getInclusionStates")] GetInclusionStates,

        [Description("getTrytes")] GetTrytes,

        [Description("getNeighbors")] GetNeighbors,

        [Description("addNeighbors")] AddNeighbors,

        [Description("removeNeighbors")] RemoveNeighbors,

        [Description("interruptAttachingToTangle")] InterruptAttachingToTangle,

        [Description("broadcastTransactions")] BroadcastTransactions,

        [Description("storeTransactions")] StoreTransactions
    }
}