namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the core api request 'InterruptAttachingToTangle'
    /// </summary>
    public class InterruptAttachingToTangleRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptAttachingToTangleRequest"/> class.
        /// </summary>
        public InterruptAttachingToTangleRequest() : base(Core.Command.InterruptAttachingToTangle.GetCommandString())
        {
        }
    }
}