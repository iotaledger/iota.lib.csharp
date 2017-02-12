using System.Collections.Generic;
using System.Linq;

namespace Iota.Lib.CSharp.Api.Model
{
    public class Inputs
    {
        public List<Input> InputsList { get; set; }
        public long TotalBalance { get; set; }

        public override string ToString()
        {
            return $"Inputs:\n {string.Join(",", InputsList.Select(i => "[" +i + "]"+"\n"))}{nameof(TotalBalance)}: {TotalBalance}";
        }
    }
}