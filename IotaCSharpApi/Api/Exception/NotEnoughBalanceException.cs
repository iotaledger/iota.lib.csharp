namespace Iota.Lib.CSharp.Api.Exception
{
    public class NotEnoughBalanceException : System.Exception
    {
        public NotEnoughBalanceException(long totalBalance, long totalValue) : base("Not enough balance")
        {
        }

        public NotEnoughBalanceException() : base("Not enough balance")
        {
        }

        public NotEnoughBalanceException(string message) : base(message)
        {
        }
    }
}