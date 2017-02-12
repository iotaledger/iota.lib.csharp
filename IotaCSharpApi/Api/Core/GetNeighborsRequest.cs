namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Gets the neighbors of the node
    /// </summary>
    /// <seealso cref="Iota.Lib.CSharp.Api.Core.IotaRequest" />
    public class GetNeighborsRequest : IotaRequest
    {
        public GetNeighborsRequest() : base(Core.Command.GetNeighbors.GetCommandString())
        {
        }
    }
}