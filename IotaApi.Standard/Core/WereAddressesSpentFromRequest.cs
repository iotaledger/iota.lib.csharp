using System;
using System.Collections.Generic;
using System.Text;

namespace Iota.Api.Core
{
    public class WereAddressesSpentFromRequest : IotaRequest
    {
        public string[] Addresses { get; set; }

        public WereAddressesSpentFromRequest(string[] addresses) : base(Core.Command.WereAddressesSpentFrom
            .GetCommandString())
        {
            Addresses = addresses;
        }

        public override string ToString()
        {
            return $"{nameof(Addresses)}: {string.Join(",", Addresses)}";
        }
    }
}
