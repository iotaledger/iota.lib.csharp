namespace Iota.Lib.CSharp.Api.Model
{
    public class Transfer
    {
        public Transfer(string address, long value, string message, string tag)
        {
            this.Address = address;
            this.Value = value;
            this.Message = message;
            this.Tag = tag;
        }

        public string Address { get; set; }

        public string Hash { get; set; }

        public int Persistence { get; set; }

        public string Timestamp { get; set; }

        public long Value { get; set; }

        public string Message { get; set; }

        public string Tag { get; set; }
    }
}