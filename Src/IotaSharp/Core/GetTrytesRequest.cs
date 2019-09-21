namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTrytesRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public GetTrytesRequest(string[] hashes) : base(Core.Command.GetTrytes)
        {
            Hashes = hashes;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Hashes { get; }
    }
}
