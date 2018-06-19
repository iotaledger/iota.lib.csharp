namespace Iota.MAM.Core
{
    public class MamChannel
    {
        public string SideKey { get; set; }
        public MamMode Mode { get; set; }
        public string NextRoot { get; set; }
        public int Security { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public int NextCount { get; set; }
        public int Index { get; set; }
    }
}