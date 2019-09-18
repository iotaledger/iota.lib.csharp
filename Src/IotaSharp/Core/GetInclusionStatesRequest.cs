namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetInclusionStatesRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="tips"></param>
        public GetInclusionStatesRequest(string[] transactions, string[] tips)
            : base(Core.Command.GetInclusionStates)
        {
            Transactions = transactions;
            Tips = tips;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Transactions { get; }

        /// <summary>
        /// 
        /// </summary>
        public string[] Tips { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Transactions)}: {Transactions}, {nameof(Tips)}: {Tips}";
        }
    }
}
