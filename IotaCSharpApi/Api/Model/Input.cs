namespace Iota.Lib.CSharp.Api.Model
{
    public class Input
    {
        public string Address { get; set; }
        public long Balance { get; set; }
        public int KeyIndex { get; set; }

        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}, {nameof(Balance)}: {Balance}, {nameof(KeyIndex)}: {KeyIndex}";
        }
    }
}