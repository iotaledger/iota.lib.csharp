namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public GetNeighborsRequest() : base(Core.Command.GetNeighbors)
        {
        }
    }
}
