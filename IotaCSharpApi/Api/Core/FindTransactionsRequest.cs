using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    public class FindTransactionsRequest : IotaRequest
    {
        public FindTransactionsRequest(List<string> bundles, List<string> addresses, List<string> tags,
            List<string> approvees) : base(Core.Command.FindTransactions.GetCommandString())
        {
            Bundles = bundles;
            Addresses = addresses;
            Tags = tags;
            Approvees = approvees;

            if (Bundles == null)
                Bundles = new List<string>();
            if (Addresses == null)
                Addresses = new List<string>();
            if (Tags == null)
                Tags = new List<string>();
            if (Approvees == null)
                Approvees = new List<string>();
        }

        public List<string> Bundles { get; set; }

        public List<string> Addresses { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Approvees { get; set; }
    }
}