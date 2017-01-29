namespace Iota.Lib.CSharp.Api.Model
{
    public class Neighbor
    {
        public string Address { get; set; }
        public long NumberOfAllTransactions { get; set; }
        public long NumberOfInvalidTransactions { get; set; }
        public long NumberOfNewTransactions { get; set; }
    }
}