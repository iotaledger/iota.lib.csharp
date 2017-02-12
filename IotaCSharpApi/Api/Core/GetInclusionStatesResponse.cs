using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    public class GetInclusionStatesResponse : IotaResponse
    {
        public List<bool> States { get; set; }
    }
}