namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoveNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uris"></param>
        public RemoveNeighborsRequest(string[] uris)
            : base(Core.Command.RemoveNeighbors)
        {
            Uris = uris;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Uris { get; }
    }
}
