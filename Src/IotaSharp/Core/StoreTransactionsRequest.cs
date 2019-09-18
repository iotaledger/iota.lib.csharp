using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class StoreTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        public StoreTransactionsRequest(string[] trytes)
            : base(Core.Command.StoreTransactions)
        {
            Trytes = trytes;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Trytes { get; }
    }
}
