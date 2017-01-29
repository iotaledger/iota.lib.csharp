namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class GetTipsRequest : IotaRequest
    {
        public GetTipsRequest() : base(Core.Command.GetTips.GetCommandString())
        {
        }
    }
}