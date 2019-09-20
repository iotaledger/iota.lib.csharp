using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using IotaSharp.Core;
using IotaSharp.Exception;
using IotaSharp.Model;
using IotaSharp.Pow;
using IotaSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IotaSharp.Tests
{

    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class IotaAPITest
    {
        private static IotaAPI _iotaApi;

        // Contains 6000 iota
        private const string TEST_SEED1 =
            "IHDEENZYITYVYSPKAURUZAQKGVJEREFDJMYTANNXXGPZ9GJWTEOJJ9IPMXOGZNQLSNMFDSQOTZAEETUEA";

        // Empty
        private const string TEST_SEED2 =
            "KHFKUYFYITYPJHFKAURUZAQKGVJEREFDJMYTAGHFEGPZ9GJWTEJGF9IHFUPOZNQLSNMFDSQOTHGPEJGKD";

        // contains 1000 iota
        private const string TEST_SEED3 =
            "9JFTUEPOTYPJHFKAURUZAQKGVJEREFDJMYTAGHFEGPZ9GJWTEJGF9IHFUPOZNQLSNMFDJOEMFLLSDKGJD";

        private const string TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_1 =
            "MALAZGDVZIAQQRTNYJDSZMY9VE9LAHQKTVCUOAGZUCX9IBUMODFFTMGUIUAXGLWZQ9CYRSLYBM9QBIBYAEIAOPKXEA";

        private const string TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2 =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZCCOZVXMTXC";

        private const string TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_3 =
            "ASCZZOBQDMNHLELQKWJBMRETMHBTF9V9TNKYDIFW9PDXPUHPVVGHMSWPVMNJHSJF99QFCMNTPCPGS9DT9XAFKJVO9X";

        private const string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_1 =
            "MALAZGDVZIAQQRTNYJDSZMY9VE9LAHQKTVCUOAGZUCX9IBUMODFFTMGUIUAXGLWZQ9CYRSLYBM9QBIBYA";

        private const string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2 =
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZC";

        private const string TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_3 =
            "ASCZZOBQDMNHLELQKWJBMRETMHBTF9V9TNKYDIFW9PDXPUHPVVGHMSWPVMNJHSJF99QFCMNTPCPGS9DT9";

        private const string TEST_HASH =
            "PXAOYSAMBOOVTZTCXL9CPLEOHS9EFHAGOESSEZSOTIDPGBERPFPTSFW9SKEL9RKPPFYDPCIMRIPAZ9999";

        //Has non-0 trits in the value area which exceeds max IOTA supply
        private const string TEST_INVALID_TRYTES =
            "BYSWEAUTWXHXZ9YBZISEK9LUHWGMHXCGEVNZHRLUWQFCUSDXZHOFHWHL9MQPVJXXZLIXPXPXF9KYEREFSKCPKYIIKPZVLHUTDFQKKVVBBN9ATTLPCNPJDWDEVIYYLGPZGCWXOBDXMLJC9VO9QXTTBLAXTTBFUAROYEGQIVB9MJWJKXJMCUPTWAUGFZBTZCSJVRBGMYXTVBDDS9MYUJCPZ9YDWWQNIPUAIJXXSNLKUBSCOIJPCLEFPOXFJREXQCUVUMKSDOVQGGHRNILCO9GNCLWFM9APMNMWYASHXQAYBEXF9QRIHIBHYEJOYHRQJAOKAQ9AJJFQ9WEIWIJOTZATIBOXQLBMIJU9PCGBLVDDVFP9CFFSXTDUXMEGOOFXWRTLFGV9XXMYWEMGQEEEDBTIJ9OJOXFAPFQXCDAXOUDMLVYRMRLUDBETOLRJQAEDDLNVIRQJUBZBO9CCFDHIX9MSQCWYAXJVWHCUPTRSXJDESISQPRKZAFKFRULCGVRSBLVFOPEYLEE99JD9SEBALQINPDAZHFAB9RNBH9AZWIJOTLBZVIEJIAYGMC9AZGNFWGRSWAXTYSXVROVNKCOQQIWGPNQZKHUNODGYADPYLZZZUQRTJRTODOUKAOITNOMWNGHJBBA99QUMBHRENGBHTH9KHUAOXBVIVDVYYZMSEYSJWIOGGXZVRGN999EEGQMCOYVJQRIRROMPCQBLDYIGQO9AMORPYFSSUGACOJXGAQSPDY9YWRRPESNXXBDQ9OZOXVIOMLGTSWAMKMTDRSPGJKGBXQIVNRJRFRYEZ9VJDLHIKPSKMYC9YEGHFDS9SGVDHRIXBEMLFIINOHVPXIFAZCJKBHVMQZEVWCOSNWQRDYWVAIBLSCBGESJUIBWZECPUCAYAWMTQKRMCHONIPKJYYTEGZCJYCT9ABRWTJLRQXKMWY9GWZMHYZNWPXULNZAPVQLPMYQZCYNEPOCGOHBJUZLZDPIXVHLDMQYJUUBEDXXPXFLNRGIPWBRNQQZJSGSJTTYHIGGFAWJVXWL9THTPWOOHTNQWCNYOYZXALHAZXVMIZE9WMQUDCHDJMIBWKTYH9AC9AFOT9DPCADCV9ZWUTE9QNOMSZPTZDJLJZCJGHXUNBJFUBJWQUEZDMHXGBPTNSPZBR9TGSKVOHMOQSWPGFLSWNESFKSAZY9HHERAXALZCABFYPOVLAHMIHVDBGKUMDXC9WHHTIRYHZVWNXSVQUWCR9M9RAGMFEZZKZ9XEOQGOSLFQCHHOKLDSA9QCMDGCGMRYJZLBVIFOLBIJPROKMHOYTBTJIWUZWJMCTKCJKKTR9LCVYPVJI9AHGI9JOWMIWZAGMLDFJA9WU9QAMEFGABIBEZNNAL9OXSBFLOEHKDGHWFQSHMPLYFCNXAAZYJLMQDEYRGL9QKCEUEJ9LLVUOINVSZZQHCIKPAGMT9CAYIIMTTBCPKWTYHOJIIY9GYNPAJNUJ9BKYYXSV9JSPEXYMCFAIKTGNRSQGUNIYZCRT9FOWENSZQPD9ALUPYYAVICHVYELYFPUYDTWUSWNIYFXPX9MICCCOOZIWRNJIDALWGWRATGLJXNAYTNIZWQ9YTVDBOFZRKO9CFWRPAQQRXTPACOWCPRLYRYSJARRKSQPR9TCFXDVIXLP9XVL99ERRDSOHBFJDJQQGGGCZNDQ9NYCTQJWVZIAELCRBJJFDMCNZU9FIZRPGNURTXOCDSQGXTQHKHUECGWFUUYS9J9NYQ9U9P9UUP9YMZHWWWCIASCFLCMSKTELZWUGCDE9YOKVOVKTAYPHDF9ZCCQAYPJIJNGSHUIHHCOSSOOBUDOKE9CJZGYSSGNCQJVBEFTZFJ9SQUHOASKRRGBSHWKBCBWBTJHOGQ9WOMQFHWJVEG9NYX9KWBTCAIXNXHEBDIOFO9ALYMFGRICLCKKLG9FOBOX9PDWNQRGHBKHGKKRLWTBEQMCWQRLHAVYYZDIIPKVQTHYTWQMTOACXZOQCDTJTBAAUWXSGJF9PNQIJ9AJRUMUVCPWYVYVARKR9RKGOUHHNKNVGGPDDLGKPQNOYHNKAVVKCXWXOQPZNSLATUJT9AUWRMPPSWHSTTYDFAQDXOCYTZHOYYGAIM9CELMZ9AZPWB9MJXGHOKDNNSZVUDAGXTJJSSZCPZVPZBYNNTUQABSXQWZCHDQSLGK9UOHCFKBIBNETK999999999999999999999999999999999999999999999999999999999999999999999999999999999NOXDXXKUDWLOFJLIPQIBRBMGDYCPGDNLQOLQS99EQYKBIU9VHCJVIPFUYCQDNY9APGEVYLCENJIOBLWNB999999999XKBRHUD99C99999999NKZKEKWLDKMJCI9N9XQOLWEPAYWSH9999999999999999999999999KDDTGZLIPBNZKMLTOLOXQVNGLASESDQVPTXALEKRMIOHQLUHD9ELQDBQETS9QFGTYOYWLNTSKKMVJAUXSIROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999IROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999";

        private const string TEST_TRYTES =
            "CCWCXCGDEAXCGDEAPCEAHDTCGDHDRAADTCGDGDPCVCTC9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999WITPXKWONPNUPBESJTXQTZTFXFSTDUWVGYHW9VRDULWGBKDVAAJLOSAEPUSCMSOIYLKMIZLPEKAOYAXMWFLDKQJI99999999999999999999UUC9999999999999999FUCK9YOUIVBUGXD99999999999B99999999IDPWGXASJFLLGCDGPQVXYGSNUESCZQCEKVREGLZX9FCYQVUWESEKWSMHZTGMJLGBOLKU9GILFITSJLZBWEHH9RSRNBPSKKUZBBJDSYHYWYHLJUOAFCKMXGTRFTZHKKWSVKGRHHJECSILLLZXKYJAYAQYEOINSZ9999OCYREQNINOB9XMLJOXMDFJDTXYO9PANNXQSW9HPFAAHEPTSPHTNEBOFFNRPKSUQNTKSACROOJPXF99999UUC9999999999999999FUCK9YOUPNMHCEJIE999999999L99999999IGMVBPROUATGVGQTHYIYFVQETRW";

        private const string TEST_MESSAGE = "JUSTANOTHERIOTATEST";
        private const string TEST_TAG = "IOTASHARPSPAM99999999999999";

        private const int MIN_WEIGHT_MAGNITUDE = 14;
        private const int MIN_WEIGHT_MAGNITUDE_DEV = 9;

        private const int DEPTH = 9;
        private const int DEPTH_DEV = 4;

        private static readonly string[] TEST_ADDRESSES =
        {
            "LXQHWNY9CQOHPNMKFJFIJHGEPAENAOVFRDIBF99PPHDTWJDCGHLYETXT9NPUVSNKT9XDTDYNJKJCPQMZCCOZVXMTXC",
            "P9UDUZMN9DEXCRQEKLJYSBSBZFCHOBPJSDKMLCCVJDOVOFDWMNBZRIRRZJGINOUMPJBMYYZEGRTIDUABDODCNSCYJD",
            "MIMVJEYREIIZLXOXQROMPJFCIX9NFVXD9ZQMNZERPI9OJIJFUWQ9WCTMKXEEPHYPWEZPHLJBJOFH9YTRBGCCKGIWYW",
            "FOJHXRVJRFMJTFDUWJYYZXCZIJXKQALLXMLKHZFDMHWTIBBXUKSNSUYJLKYRQBNXKRSUXZHDTPWXYD9YFHA9AWOMPX",
            "B9YNPQO9EXID9RDEEGLCBJBYKBLWHTOQOZKTLJDFPJZOPKJJTNUYUVVTDJPBCBYIWGPSCMNRZFGFHFSXHTIYXWAKZ9",
            "NQEFOAFIYKZOUXDFQ9X9PHCNSDETRTJZINZ9EYGKU99QJLDSTSC9VTBAA9FHLNLNYQXWLTNPRJDWCGIPPYIPAMUSVY",
            "CEGLBSXDJVXGKGOUHRGMAQDRVYXCQLXBKUDWKFFSIABCUYRATFPTEEDIFYGAASKFZYREHLBIXBTKP9KLCRTXEGJXKX",
            "QLOXU9GIQXPPE9UUT9DSIDSIESRIXMTGZJMKLSJTNBCRELAVLWVJLUOLKGFCWAEPEQWZWPBV9YZJJEHUSMBQHBROEZ",
            "XIRMYJSGQXMM9YPHJVVLAVGBBLEEMOOKHHBFWKEAXJFONZLNSLBCGPQEVDMMOGHFVRDSYTETIFOIVNCR9IUZLVJVWX",
            "PDVVBYBXMHZKADPAYOKQNDPHRSWTHAWQ9GRVIBOIMZQTYCWEPCDWDVRSOUNASVBDLBOAMVLYEVVCMAM9NPLXNSO9BD",
            "U9GAIAPUUQWJGISAZWPLHUELTZ9WSHWXS9JLPKOWHRRIVUKGWCTJMBULVMKTETTUNHZ9HWHBALUCJIROUBRIIYCUHC",
            "VFPMKZLLMDUOEKNBEKQZPTNZJZF9UHRWSTHXLWQQ9OAXTZQHTZPAWNJNXKAZFSDFWKFQEKZIGJTLWQFLOBXMPLSQNB",
            "IGHK9XIWOAYBZUEZHQLEXBPTXSWVANIOUZZCPNKUIJIJOJNAQCJWUJHYKCZOIKVAAHDGAWJZKLTPVQL9GJSCYKNWTW",
            "LXQPWMNXSUZTEYNC9ZBBFHY9YWCCOVKBNIIOUSVXZJZMJKJFDUWGUVXYCHGKUHEEIDHSGEWFAHVJPRIJTJCNU9GJAC",
            "AKFDX9PGGQLZUWRMZ9YBDF9CG9TWXCNALCSXSAWHFIMGXCSYCJLSWIQDGGVDRMNEKKECQEYAITGNLNJFQCFALBLRZX",
            "YX9QSPYMSFVOW9UVZRDVOCPYYMUTDHCCPKHMXQSJQJYIXVCHILKW9GBYJTYGLIKBTRQMDCYBMLLNGSSIKNQOHMSKTD",
            "DSYCJKNG9TAGJHSKZQ9XLKAKNSKJFZIPVEDGJFXRTFGENHZFQGXHWDBNXLLDABDMOYELPG9DIXSNJFWARNURMPPVKC",
            "9ANNACZYLDDPZILLQBQG9YMG9XJUMTAENDFQ9HMSSEFWYOAXPJTUXBFTSAXDJPAO9FKTWBBSCSFMOUR9IDQNHAFE9W",
            "WDTFFXHBHMFQQVXQLBFJFVVHVIIAVYM9PFAZCHMKET9ESMHIRHSMVDJBZTXPTAFVIASMSXRDCIYVWVQNODEVYEGMVW",
            "XCCPS9GMTSUB9DXPVKLTBDHOFX9PJMBYZQYQEXMRQDPGQPLWRGZGXODYJKGVFOHHYUJRCSXAIDGYSAWRBRXZJONAYW",
            "KVEBCGMEOPDPRCQBPIEMZTTXYBURGZVNH9PLHKPMM9D9FUKWIGLKZROGNSYIFHULLWQWXCNAW9HKKVIDCTGKHAVBJZ"
        };

        private const string Provider = "http://node05.iotatoken.nl:16265";
        private const string DevNetProvider = "https://nodes.devnet.thetangle.org:443";

        [TestInitialize]
        public void CreateProxyInstance()
        {
            _iotaApi = new IotaAPI()
            {
                IotaClient = new IotaClient(DevNetProvider),
                LocalPoW = new PearlDiverLocalPoW()
            };
        }

        [TestMethod]
        public void ShouldGetInputs()
        {
            // Address 0 should contain 1000
            var res = _iotaApi.GetInputs(TEST_SEED1, 2, 0, 5, 0);

            Assert.IsNotNull(res, "Error on getInputs should have thrown");
            Assert.IsTrue(res.TotalBalance > 0, "Res should have a balance(8)");
            Assert.IsNotNull(res.Inputs, "Error on getInputs should have thrown");
        }

        [TestMethod]
        public void ShouldCreateANewAddressWithChecksum()
        {
            var addressRequest = new AddressRequest(TEST_SEED1, 1)
            {
                Checksum = true,
                Amount = 5
            };

            GetNewAddressResponse res1 = _iotaApi.GetAddressesUnchecked(addressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_1, res1.Addresses[0]);

            AddressRequest secondAddressRequest = new AddressRequest(TEST_SEED1, 2)
            {
                Checksum = true,
                Amount = 5
            };

            GetNewAddressResponse res2 = _iotaApi.GetAddressesUnchecked(secondAddressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, res2.Addresses[0]);

            AddressRequest thirdAddressRequest = new AddressRequest(TEST_SEED1, 3)
            {
                Checksum = true,
                Amount = 5
            };
            GetNewAddressResponse res3 = _iotaApi.GetAddressesUnchecked(thirdAddressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_3, res3.Addresses[0]);
        }

        [TestMethod]
        public void ShouldCreateANewAddressWithoutChecksum()
        {
            var addressRequest = new AddressRequest(TEST_SEED1, 1)
            {
                Amount = 5
            };
            var res1 = _iotaApi.GetAddressesUnchecked(addressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_1, res1.Addresses[0]);

            var secondAddressRequest = new AddressRequest(TEST_SEED1, 2)
            {
                Amount = 5
            };
            var res2 = _iotaApi.GetAddressesUnchecked(secondAddressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_2, res2.Addresses[0]);

            AddressRequest thirdAddressRequest = new AddressRequest(TEST_SEED1, 3)
            {
                Amount = 5
            };
            var res3 = _iotaApi.GetAddressesUnchecked(thirdAddressRequest);
            Assert.AreEqual(TEST_ADDRESS_WITHOUT_CHECKSUM_SECURITY_LEVEL_3, res3.Addresses[0]);
        }


        [TestMethod]
        public void ShouldCreate100Addresses()
        {
            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2)
            {
                Amount = 100
            };
            GetNewAddressResponse res = _iotaApi.GetAddressesUnchecked(addressRequest);
            Assert.AreEqual(100, res.Addresses.Count);
        }

        [TestMethod]
        public void GenerateNewAddressesWithZeroIndexAndZeroAmountShouldGenerateOneAddresses()
        {
            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2) {Amount = 0};
            GetNewAddressResponse addressResponse = _iotaApi.GenerateNewAddresses(addressRequest);
            Assert.AreEqual(1, addressResponse.Addresses.Count);
        }

        [TestMethod]
        public void GenerateNewAddressesWithZeroAmountShouldGenerateOneAddresses()
        {
            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2) {Amount = 0, Index = 1};
            GetNewAddressResponse addressResponse = _iotaApi.GenerateNewAddresses(addressRequest);
            Assert.AreEqual(1, addressResponse.Addresses.Count);
        }

        [TestMethod]
        public void GenerateNewAddresses()
        {
            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2) {Amount = 1};
            GetNewAddressResponse firstAddressResponse = _iotaApi.GenerateNewAddresses(addressRequest);
            Assert.AreEqual(1, firstAddressResponse.Addresses.Count);
            Assert.IsNotNull(firstAddressResponse.Addresses[0]);
        }

        [TestMethod]
        public void GenerateNewAddressesWithSameIndexAndOneAmountShouldGenerateSameAddress()
        {
            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2) {Amount = 1};
            GetNewAddressResponse firstAddressResponse = _iotaApi.GenerateNewAddresses(addressRequest);
            GetNewAddressResponse secondAddressResponse = _iotaApi.GenerateNewAddresses(addressRequest);
            Assert.AreEqual(1, firstAddressResponse.Addresses.Count);
            Assert.AreEqual(1, secondAddressResponse.Addresses.Count);
            Assert.AreEqual(firstAddressResponse.Addresses[0], secondAddressResponse.Addresses[0]);
        }

        [TestMethod]
        public void ShouldPrepareTransfer()
        {
            List<Transfer> transfers = new List<Transfer>
            {
                new Transfer(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, 5, TEST_MESSAGE, TEST_TAG)
            };

            List<string> trytes =
                _iotaApi.PrepareTransfers(TEST_SEED1, 2, transfers.ToArray(), null, null, null, false);

            Assert.IsNotNull(trytes, "prepareTransfers should throw an error on failure");
            Assert.IsFalse(trytes.Count == 0, "prepareTransfers should throw an error on failure");

            Transaction first = new Transaction(trytes[0]);
            Assert.AreEqual(first.LastIndex, first.CurrentIndex,
                "prepareTransfers should have reversed bundle order for attachToTangle");
        }

        [TestMethod]
        public void ShouldPrepareTransferWithInputs()
        {
            List<Transfer> transfers = new List<Transfer>();

            GetBalancesAndFormatResponse rsp = _iotaApi.GetInputs(TEST_SEED1, 2, 0, 10, 0);

            List<Input> inputList = new List<Input>(rsp.Inputs);

            transfers.Add(new Transfer(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, 1, TEST_MESSAGE, TEST_TAG));
            List<string> trytes = _iotaApi.PrepareTransfers(TEST_SEED1, 2, transfers.ToArray(), null,
                inputList.ToArray(), null, true);

            Assert.IsNotNull(trytes, "prepareTransfers should throw an error on failure");
            Assert.IsFalse(trytes.Count == 0, "prepareTransfers should throw an error on failure");

            Transaction first = new Transaction(trytes[0]);
            Assert.AreEqual(first.LastIndex, first.CurrentIndex,
                "prepareTransfers should have reversed bundle order for attachToTangle");
        }

        [TestMethod]
        public void ShouldFailTransfer()
        {
            try
            {
                List<Transfer> transfers = new List<Transfer>
                {
                    new Transfer(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, 100, TEST_MESSAGE, TEST_TAG)
                };

                _iotaApi.PrepareTransfers(TEST_SEED2, 2, transfers.ToArray(), null, null, null, false);

                Assert.Fail("prepareTransfers should have thrown an error due to lack of balance on the seed");
            }
            catch (IllegalStateException e)
            {
                Assert.AreEqual(Constants.NOT_ENOUGH_BALANCE_ERROR, e.Message,
                    "Message should say that there is not enough balance");
            }
        }

        [TestMethod]
        public void ShouldFailTransferWithInputs()
        {
            try
            {
                List<Transfer> transfers = new List<Transfer>
                {
                    new Transfer(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, 1, TEST_MESSAGE, TEST_TAG)
                };
                _iotaApi.PrepareTransfers(TEST_SEED2, 2, transfers.ToArray(), null, Array.Empty<Input>(), null, true);

                Assert.Fail("prepareTransfer should have thrown an error on wrong/lack of inputs");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(Constants.INVALID_ADDRESSES_INPUT_ERROR, e.Message,
                    "Message should say that the input is invalid");
            }
        }

        [TestMethod]
        public void ShouldGetLastInclusionState()
        {
            var iotaApi = new IotaAPI
            {
                IotaClient = new IotaClient(Provider)
            };

            GetInclusionStatesResponse res = iotaApi.GetLatestInclusion(new[]
            {
                TEST_HASH
            });

            Assert.IsNotNull(res.States, "States should be an array of booleans");
            Assert.IsTrue(res.States[0], "Hash should have been seen as confirmed");
        }

        [TestMethod]
        public void ShouldFindTransactionObjects()
        {
            List<Transaction> ftr = _iotaApi.FindTransactionObjectsByAddresses(TEST_ADDRESSES);
            Assert.IsNotNull(ftr, "findTransactionObjectsByAddresses should not return null on failure");

            Assert.IsFalse(ftr.Count == 0, "findTransactionObjectsByAddresses should find multiple transactions");
        }

        //[TestMethod]
        //public void ShouldGetAccountData()
        //{
        //    GetAccountDataResponse gad = _iotaApi.GetAccountData(TEST_SEED3, 2, 0, true, 0, true, 0, 10, true, 0);
        //    assertThat("GetAccountDataResponse should not return null on failure", gad, IsNull.notNullValue());
        //}

        [TestMethod]
        public void ShouldNotGetBundle()
        {
            Assert.ThrowsException<ArgumentException>(() => { _iotaApi.GetBundle("SADASD"); });
        }

        [TestMethod]
        public void ShouldGetBundle()
        {
            var iotaApi = new IotaAPI
            {
                IotaClient = new IotaClient(Provider)
            };

            GetBundleResponse gbr = iotaApi.GetBundle(TEST_HASH);
            Assert.IsNotNull(gbr, "GetBundleResponse should not return null on failure");
        }

        [TestMethod]
        public void ShouldCheckConsistency()
        {
            GetNodeInfoResponse gni = _iotaApi.IotaClient.GetNodeInfo();
            CheckConsistencyResponse ccr = _iotaApi.IotaClient.CheckConsistency(gni.LatestSolidSubtangleMilestone);

            Assert.IsNotNull(ccr, "CheckConsistencyResponse should not return null on failure");
            Assert.IsTrue(ccr.State, "Latest milestone should always be consistent");
        }


        [TestMethod]
        public void ShouldGetTransfers()
        {
            var iotaApi = new IotaAPI
            {
                IotaClient = new IotaClient(Provider)
            };

            GetTransferResponse gtr = iotaApi.GetTransfers(TEST_SEED3, 2, 0, 10, false);
            Assert.IsNotNull(gtr.TransferBundles, "GetTransfers should return GetTransferResponse object on success");
            Assert.IsTrue(gtr.TransferBundles.Length > 0, "GetTransfers should return more than 0 transfers");
        }

        [TestMethod]
        public void ShouldReplayBundle()
        {
            var iotaApi = new IotaAPI
            {
                IotaClient = new IotaClient(Provider)
            };

            ReplayBundleResponse rbr = iotaApi.ReplayBundle(TEST_HASH, DEPTH, MIN_WEIGHT_MAGNITUDE, null);
            Assert.IsNotNull(rbr, "Bundle should be replayed");
        }

        [TestMethod]
        public void ShouldNotSendTrytes()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _iotaApi.SendTrytes(new[] {TEST_INVALID_TRYTES}, DEPTH, MIN_WEIGHT_MAGNITUDE_DEV, null);
            });
        }

        [TestMethod]
        public void ShouldGetTrytes()
        {
            GetTrytesResponse trytes = _iotaApi.IotaClient.GetTrytes(TEST_HASH);
            Assert.IsNotNull(trytes);
            Assert.AreEqual(1, trytes.Trytes.Count, "getTrytes should send back 1 transaction trytes");
        }

        [TestMethod]
        public void ShouldFailBeforeSnapshotTimeStamp()
        {
            var iotaApi = new IotaAPI
            {
                IotaClient = new IotaClient(Provider)
            };

            Assert.ThrowsException<ArgumentException>(() => { iotaApi.BroadcastAndStore(TEST_TRYTES); },
                "Transaction did not fail on old timestamp value");
        }

        [TestMethod]
        public void ShouldSendTrytes()
        {
            var response =
                _iotaApi.SendTrytes(new[] {TEST_TRYTES}, DEPTH_DEV, MIN_WEIGHT_MAGNITUDE_DEV, null);
            Assert.AreEqual(1, response.Length, "Sending 1 transaction received unexpected amount");
        }

        [TestMethod]
        public void ShouldNotSendTransfer()
        {
            try
            {
                List<Transfer> transfers = new List<Transfer>
                {
                    // Address is spent
                    new Transfer(TEST_ADDRESS_WITH_CHECKSUM_SECURITY_LEVEL_2, 2, TEST_MESSAGE, TEST_TAG)
                };

                _iotaApi.SendTransfer(TEST_SEED1, 2, DEPTH, MIN_WEIGHT_MAGNITUDE_DEV, transfers.ToArray(), null, null,
                    false, true,
                    null);
                Assert.Fail("Transaction did not fail on spent address");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(Constants.SENDING_TO_USED_ADDRESS_ERROR, e.Message,
                    "Message should say we try to use a used address");
            }
        }

        [TestMethod]
        public void ShouldSendTransferWithoutInputs()
        {
            List<Transfer> transfers = new List<Transfer>();

            AddressRequest addressRequest = new AddressRequest(TEST_SEED1, 2) {Checksum = true};
            string address = _iotaApi.GenerateNewAddresses(addressRequest).Addresses[0];
            transfers.Add(new Transfer(address, 1, TEST_MESSAGE, TEST_TAG));

            SendTransferResponse str = _iotaApi.SendTransfer(TEST_SEED1, 2, DEPTH_DEV, MIN_WEIGHT_MAGNITUDE_DEV,
                transfers.ToArray(),
                null, null, false, true, null);

            Assert.IsNotNull(str.Results, "Sending transfer should have returned multiple transactions");

            Assert.AreEqual(0, str.Results[0].Item1.CurrentIndex, "Returned transfers should have normal bundle order");
        }

        [Ignore]
        [TestMethod]
        public void ShouldSendTransferWithInputs()
        {
            List<Input> inputList = new List<Input>();
            List<Transfer> transfers = new List<Transfer>();

            GetBalancesAndFormatResponse rsp = _iotaApi.GetInputs(TEST_SEED3, 2, 0, 0, 1);

            //TODO(guojiancong): rep.Inputs.Count == 0 ???
            inputList.AddRange(rsp.Inputs);

            AddressRequest addressRequest = new AddressRequest(TEST_SEED3, 2) {Checksum = true};
            string address = _iotaApi.GenerateNewAddresses(addressRequest).Addresses[0];
            transfers.Add(new Transfer(address, 1, TEST_MESSAGE, TEST_TAG));

            // validate Inputs to true would mean we have to spent all balance in once. Now we double spent but its devnet
            SendTransferResponse str = _iotaApi.SendTransfer(TEST_SEED3, 2, DEPTH_DEV, MIN_WEIGHT_MAGNITUDE_DEV,
                transfers.ToArray(),
                inputList.ToArray(), null, false, true, null);

            Assert.IsNotNull(str.Results, "Sending transfer should have returned multiple transactions");
        }
    }
}
