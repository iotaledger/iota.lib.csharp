namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTransactionsToApproveResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}";
        }
    }
}
