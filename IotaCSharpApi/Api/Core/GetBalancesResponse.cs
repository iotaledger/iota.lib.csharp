using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    public class GetBalancesResponse : IotaResponse
    {
        public List<long> Balances { get; set; }

        public string Milestone { get; set; }

        public int MilestoneIndex { get; set; }

        public override string ToString()
        {
            return $"{nameof(Balances)}: {Balances}, {nameof(Milestone)}: {Milestone}, {nameof(MilestoneIndex)}: {MilestoneIndex}";
        }
    }
}