using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Response of <see cref="AttachToTangleRequest"/>
    /// </summary>
    public class AttachToTangleResponse
    {
        public List<string> Trytes { get; set; }
    }
}