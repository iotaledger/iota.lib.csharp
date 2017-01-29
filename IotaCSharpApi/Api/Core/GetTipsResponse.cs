using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Created by Adrian on 29.04.2016.
    /// </summary>
    public class GetTipsResponse : IotaResponse
    {
        public List<string> Hashes { get; set; }
    }
}