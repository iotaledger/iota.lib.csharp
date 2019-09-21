using System;
using System.Diagnostics.Contracts;
using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SendTransferResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="successfully"></param>
        /// <param name="duration"></param>
        public SendTransferResponse(
            Transaction[] transactions, 
            bool[] successfully, 
            long duration)
        {
            Contract.Assert(transactions.Length == successfully.Length);

            int length = transactions.Length;
            Results = new Tuple<Transaction, bool>[length];
            for (int i = 0; i < length; i++)
            {
                Results[i] = new Tuple<Transaction, bool>(transactions[i], successfully[i]);
            }

            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public Tuple<Transaction, bool>[] Results { get; }
    }
}
