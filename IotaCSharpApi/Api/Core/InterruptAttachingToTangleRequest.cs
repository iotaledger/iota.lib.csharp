namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 28.04.2016.
    /// </summary>
    public class InterruptAttachingToTangleRequest : IotaRequest
    {
        public InterruptAttachingToTangleRequest() : base(Core.Command.InterruptAttachingToTangle.GetCommandString())
        {
        }
    }
}