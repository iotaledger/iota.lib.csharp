using IotaSharp.Model;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ReplayBundleResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newBundle"></param>
        /// <param name="successfully"></param>
        /// <param name="duration"></param>
        public ReplayBundleResponse(Bundle newBundle, bool[] successfully, long duration)
        {
            NewBundle = newBundle;
            Successfully = successfully;
            Duration = duration;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool[] Successfully { get; }

        /// <summary>
        /// 
        /// </summary>
        public Bundle NewBundle { get; }
    }
}
