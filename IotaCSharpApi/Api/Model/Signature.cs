using System;
using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Model
{
    public class Signature
    {
        string address;
        List<string> signatureFragments;

        public Signature()
        {
            this.signatureFragments = new List<string>();
        }

        public String getAddress()
        {
            return address;
        }

        public void setAddress(String address)
        {
            this.address = address;
        }

        public List<String> getSignatureFragments()
        {
            return signatureFragments;
        }

        public void setSignatureFragments(List<String> signatureFragments)
        {
            this.signatureFragments = signatureFragments;
        }
    }
}