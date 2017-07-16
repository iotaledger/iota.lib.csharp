namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the core API request 'GetNeighbors'
    /// </summary>
    /// <seealso cref="IotaRequestBase" />
    public class GetNeighborsRequest : IotaRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNeighborsRequest"/> class.
        /// </summary>
        public GetNeighborsRequest() : base(Core.Command.GetNeighbors.GetCommandString())
        {
        }
    }
}