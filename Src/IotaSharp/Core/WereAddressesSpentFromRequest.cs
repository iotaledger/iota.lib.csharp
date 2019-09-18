using System.Collections.Generic;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class WereAddressesSpentFromRequest : IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        public WereAddressesSpentFromRequest(List<string> addresses)
            : base(Core.Command.WereAddressesSpentFrom)
        {
            Addresses = addresses;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Addresses { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Addresses)}: {string.Join(",", Addresses)}";
        }
    }
}
