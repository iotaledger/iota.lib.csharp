using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// </summary>
    public class GetBalancesResponse : IotaResponse
    {
        public List<long> Balances { get; set; }

        public string Milestone { get; set; }

        public int MilestoneIndex { get; set; }
    }
}