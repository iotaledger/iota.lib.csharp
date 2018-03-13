using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="transferBundle"></param>
        /// <param name="inputList"></param>
        /// <param name="totalBalance"></param>
        public AccountData(List<string> addresses,
            Bundle[] transferBundle, List<Input> inputList, long totalBalance)
        {
            Addresses = addresses;
            TransferBundle = transferBundle;
            InputList = inputList;
            TotalBalance = totalBalance;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Addresses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Bundle[] TransferBundle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Input> InputList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalBalance { get; set; }
    }
}
