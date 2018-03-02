namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class GetNodeInfoRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNodeInfoRequest"/> class.
        /// </summary>
        public GetNodeInfoRequest() : base(Core.Command.GetNodeInfo.GetCommandString())
        {
        }
    }
}