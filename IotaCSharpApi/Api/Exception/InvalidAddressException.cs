using System;

namespace Iota.Lib.CSharp.Api.Exception
{
    public class InvalidAddressException : ArgumentException
    {
        public InvalidAddressException(string address) : base("The specified address '" + address + "' is invalid")
        {
        }
    }
}