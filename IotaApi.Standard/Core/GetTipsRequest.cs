namespace Iota.Api.Core
{
    /// <summary>
    /// This class represents the core API request 'GetTips'
    /// </summary>
    public class GetTipsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTipsRequest"/> class.
        /// </summary>
        public GetTipsRequest() : base(Core.Command.GetTips.GetCommandString())
        {
        }
    }
}