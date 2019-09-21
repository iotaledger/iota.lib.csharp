namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AddNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uris"></param>
        public AddNeighborsRequest(string[] uris)
            : base(Core.Command.AddNeighbors)
        {
            Uris = uris;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Uris { get; }
    }
}
