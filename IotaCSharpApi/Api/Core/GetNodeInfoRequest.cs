namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class GetNodeInfoRequest : IotaRequest
    {
        public GetNodeInfoRequest() : base(Core.Command.GetNodeInfo.GetCommandString())
        {
        }
    }
}