namespace Iota.Lib.CSharp.Api.Core
{
    public class GetInclusionStatesRequest : IotaRequest
    {
        private readonly string[] _transactions;
        private readonly string[] _tips;

        public GetInclusionStatesRequest(string[] transactions, string[] tips)
            : base(Core.Command.GetInclusionStates.GetCommandString())
        {
            _transactions = transactions;
            _tips = tips;
        }

        public string[] Transactions
        {
            get { return _transactions; }
        }

        public string[] Tips
        {
            get { return _tips; }
        }
    }
}