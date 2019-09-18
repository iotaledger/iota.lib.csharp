using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GetNewAddressResponse : IotaResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Addresses { get; set; }
    }
}
