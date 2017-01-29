using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Returns information about your node
    /// </summary>
    public class AddNeighborsRequest : IotaRequest
    {
        public List<string> Uris { get; set; }

        public AddNeighborsRequest(List<string> uris) : base(Core.Command.AddNeighbors.GetCommandString())
        {
            Uris = uris;
        }
    }
}