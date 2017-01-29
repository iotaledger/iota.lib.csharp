namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class GetNeighborsRequest : IotaRequest
    {
        public GetNeighborsRequest() : base(Core.Command.GetNeighbors.GetCommandString())
        {
        }
    }
}