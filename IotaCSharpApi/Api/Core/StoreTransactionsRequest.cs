using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Store transactions into the local storage. The trytes to be used for this call are returned by attachToTangle.
    /// </summary>
    public class StoreTransactionsRequest : IotaRequest
    {
        public StoreTransactionsRequest(List<string> trytes) : base(Core.Command.StoreTransactions.GetCommandString())
        {
            this.Trytes = trytes;
        }

        public List<string> Trytes { get; set; }
    }
}