using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTransferResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferBundles"></param>
        /// <param name="duration"></param>
        public GetTransferResponse(Bundle[] transferBundles, long duration)
        {
            TransferBundles = transferBundles;
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public Bundle[] TransferBundles { get; }
    }
}
