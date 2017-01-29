using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    public class GetBalancesRequest : IotaRequest
    {
        public GetBalancesRequest(List<string> addresses, long threshold = 100)
            : base(Core.Command.GetBalances.GetCommandString())
        {
            Addresses = addresses;
            Threshold = threshold;
        }

        public long Threshold { get; }

        public List<string> Addresses { get; }
    }
}