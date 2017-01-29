using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    public class RemoveNeighborsRequest : IotaRequest
    {
        public List<string> Uris { get; set; }

        public RemoveNeighborsRequest(List<string> uris) : base(Core.Command.RemoveNeighbors.GetCommandString())
        {
            Uris = uris;
        }
    }
}