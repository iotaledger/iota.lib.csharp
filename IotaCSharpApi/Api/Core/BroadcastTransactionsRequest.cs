using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Broadcast a list of transactions to all neighbors. The input trytes for this call are provided by attachToTangle
    /// </summary>
    public class BroadcastTransactionsRequest : IotaRequest
    {
        public BroadcastTransactionsRequest(List<string> trytes)
            : base(Core.Command.BroadcastTransactions.GetCommandString())
        {
            Trytes = trytes;
        }

        public List<string> Trytes { get; set; }
    }
}