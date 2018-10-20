namespace Iota.Api.Core
{
    public class CheckConsistencyRequest : IotaRequest
    {
        public string[] Tails { get; set; }

        public CheckConsistencyRequest(string[] tails) : base(Core.Command.CheckConsistency.GetCommandString())
        {
            Tails = tails;
        }

        public override string ToString()
        {
            return $"{nameof(Tails)}: {string.Join(",", Tails)}";
        }
    }
}
