namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class GetTransactionsToApproveResponse : IotaResponse
    {
        public string TrunkTransaction { get; set; }

        public string BranchTransaction { get; set; }
    }
}