using System;
using System.Collections.Generic;
using IotaSharp.Model;
using IotaSharp.Pow;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests.Utils
{
    [TestClass]
    public class MultisigTest
    {
        private const string TestSeed1 = "ABCDFG";
        private const string TestSeed2 = "FDSAG";

        private const string RemainderAddress =
            "NZRALDYNVGJWUVLKDWFKJVNYLWQGCWYCURJIIZRLJIKSAIVZSGEYKTZRDBGJLOA9AWYJQB9IPWRAKUC9FBDRZJZXZG";

        private const string ReceiveAddress =
            "ZGHXPZYDKXPEOSQTAQOIXEEI9K9YKFKCWKYYTYAUWXK9QZAVMJXWAIZABOXHHNNBJIEBEUQRTBWGLYMTX";

        private const string TestTag = "JOTASPAM9999999999999999999";


        private IotaAPI _iotaAPI;

        [TestInitialize]
        public void CreateApiClientInstance()
        {
            _iotaAPI = new IotaAPI()
            {
                IotaClient = new IotaClient("https://nodes.devnet.iota.org:443")
        };
        }

        [TestMethod]
        public void BasicMultiSigTest()
        {
            Multisig ms = new Multisig();

            // First co-signer uses security level 3 and index 0 for the private key
            string digestOne = ms.GetDigest(TestSeed1, 3, 0);

            // We initiate the multisig address generation by absorbing the key digest
            ms.AddAddressDigest(new[] { digestOne });

            // Second cosigner also uses security level 3 and index 0  for the private key
            string digestTwo = ms.GetDigest(TestSeed2, 3, 0);

            // Add the multisig by absorbing the second cosigners key digest
            ms.AddAddressDigest(new[] { digestTwo });

            // finally we generate the multisig address itself
            string multiSigAddress = ms.FinalizeAddress();

            Console.WriteLine("MultisigAddress = " + multiSigAddress);


            bool isValidMultisigAddress = ms.ValidateAddress(multiSigAddress,
                new[] { Converter.ToTrits(digestOne), Converter.ToTrits(digestTwo) });

            Console.WriteLine("Is a valid multisig address " + isValidMultisigAddress);

            Assert.IsTrue(isValidMultisigAddress, "Address is not a valid multisigAddress");

            List<Transfer> transfers = new List<Transfer>
            {
                new Transfer(ReceiveAddress.AddChecksum(), 999, "", TestTag)
            };

            List<Transaction> trxs =
                _iotaAPI.InitiateTransfer(6, multiSigAddress.AddChecksum(), RemainderAddress, transfers, null, true);

            Bundle bundle = new Bundle(trxs, trxs.Count);

            bundle = ms.AddSignature(bundle, multiSigAddress, ms.GetKey(TestSeed1, 0, 3));

            bundle = ms.AddSignature(bundle, multiSigAddress, ms.GetKey(TestSeed2, 0, 3));


            Signing sgn = new Signing(new Kerl());

            bool isValidSignature = sgn.ValidateSignatures(bundle, multiSigAddress);
            Console.WriteLine("Result of multi-signature validation is " + isValidSignature);
            Assert.IsTrue(isValidSignature, "MultiSignature not valid");

        }
    }
}
