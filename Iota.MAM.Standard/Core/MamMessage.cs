namespace Iota.MAM.Core
{
    public class MamMessage
    {
        public MamState State { get; set; }
        public string Payload { get; set; }
        public string Root { get; set; }
        public string Address { get; set; }
    }
}