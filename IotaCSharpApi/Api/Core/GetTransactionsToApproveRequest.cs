namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class GetTransactionsToApproveRequest : IotaRequest
    {
        public int Depth { get; }

        public GetTransactionsToApproveRequest(int depth)
            : base(Core.Command.GetTransactionsToApprove.GetCommandString())
        {
            Depth = depth;
        }
    }
}