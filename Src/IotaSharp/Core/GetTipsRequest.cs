namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTipsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTipsRequest"/> class.
        /// </summary>
        public GetTipsRequest() : base(Core.Command.GetTips)
        {
        }
    }
}
