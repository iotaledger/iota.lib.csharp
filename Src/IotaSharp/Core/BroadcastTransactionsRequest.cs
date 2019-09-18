namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BroadcastTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        public BroadcastTransactionsRequest(string[] trytes)
            : base(Core.Command.BroadcastTransactions)
        {
            Trytes = trytes;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Trytes { get; }
    }
}
