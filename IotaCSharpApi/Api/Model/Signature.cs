using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Model
{
    /// <summary>
    /// Thic class represents a signature
    /// </summary>
    public class Signature
    {
        public Signature()
        {
            this.SignatureFragments = new List<string>();
        }

        public string Address { get; set; }

        public List<string> SignatureFragments { get; set; }
    }
}