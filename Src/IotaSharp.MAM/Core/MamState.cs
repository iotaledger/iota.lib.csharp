namespace IotaSharp.MAM.Core
{
    public class MamState
    {
        public string Seed { get; set; }
        public MamChannel Channel { get; set; }

        public void ChangeMode(MamMode mode, string sideKey = null)
        {
            Channel.Mode = mode;
            if (mode == MamMode.Restricted)
            {
                Channel.SideKey = sideKey;
            }
        }
    }
}