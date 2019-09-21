namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetNodeInfoRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public GetNodeInfoRequest()
            : base(Core.Command.GetNodeInfo)
        {

        }
    }
}
