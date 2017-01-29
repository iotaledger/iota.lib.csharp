using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Model
{
    public class Inputs
    {
        public List<Input> InputsList { get; set; }
        public long TotalBalance { get; set; }

        public override string ToString()
        {
            return $"{nameof(InputsList)}: {InputsList}, {nameof(TotalBalance)}: {TotalBalance}";
        }
    }
}