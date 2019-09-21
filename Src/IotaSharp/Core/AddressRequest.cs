namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="securityLevel"></param>
        public AddressRequest(string seed, int securityLevel)
        {
            Seed = seed;
            SecurityLevel = securityLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Seed { get; }

        /// <summary>
        /// 
        /// </summary>
        public int SecurityLevel { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool Checksum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AddSpendAddresses { get; set; }
    }
}
