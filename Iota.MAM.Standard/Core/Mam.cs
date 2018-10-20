using System;
using System.Collections.Generic;
using System.Linq;
using Iota.Api;
using Iota.Api.Model;
using Iota.Api.Pow;
using Iota.Api.Utils;
using Iota.MAM.Merkle;
using Iota.MAM.Utils;
using Constants = Iota.MAM.Utils.Constants;

namespace Iota.MAM.Core
{
    public class Mam
    {
        public Mam(IotaApi iota)
        {
            Iota = iota;
        }

        public IotaApi Iota { get; set; }

        public MamState InitMamState(string seed = null, int security = 2)
        {
            var channel = new MamChannel
            {
                SideKey = null,
                Mode = MamMode.Public,
                NextRoot = null,
                Security = security,
                Start = 0,
                Count = 1,
                NextCount = 1,
                Index = 0
            };

            if (string.IsNullOrEmpty(seed))
                seed = TrytesHelper.KeyGen(81);

            return new MamState
            {
                Seed = seed,
                Channel = channel
                //Subscribed
            };
        }

        public MamMessage CreateMamMessage(MamState state, string message)
        {
            var channel = state.Channel;
            var seed = state.Seed;
            var sideKey = channel.SideKey;

            if (string.IsNullOrEmpty(sideKey))
                sideKey = "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

            var seedTrits = Converter.ToTrits(seed);
            var messageTrits = Converter.ToTrits(message);
            var sideKeyTrits = Converter.ToTrits(sideKey);

            var security = channel.Security;
            var start = channel.Start;
            var count = channel.Count;
            var nextStart = start + count;
            var nextCount = channel.NextCount;
            var index = channel.Index;

            // set up merkle tree
            var rootMerkle = MerkleTree.CreateMerkleTree(
                seedTrits, start, (uint) count, security,
                new Curl(CurlMode.CurlP27),
                new Curl(CurlMode.CurlP27),
                new Curl(CurlMode.CurlP27));

            var nextRootMerkle = MerkleTree.CreateMerkleTree(
                seedTrits, nextStart, (uint) nextCount, security,
                new Curl(CurlMode.CurlP27),
                new Curl(CurlMode.CurlP27),
                new Curl(CurlMode.CurlP27));

            var rootBranch = MerkleTree.CreateMerkleBranch(rootMerkle, index);
            int[] rootSiblings = { };
            if (rootBranch != null)
                rootSiblings = rootBranch.Siblings;

            var rootTrits = rootMerkle.Slice();
            var nextRootTrits = nextRootMerkle.Slice();

            var maskedPayload =
                CreateMaskedPayload(seedTrits, messageTrits,
                    sideKeyTrits, rootTrits, rootSiblings, nextRootTrits,
                    start, index, security);

            var root = Converter.ToTrytes(rootTrits);
            var nextRoot = Converter.ToTrytes(nextRootTrits);
            var payload = Converter.ToTrytes(maskedPayload);

            // If the tree is exhausted.
            if (channel.Index == channel.Count - 1)
            {
                // change start to begining of next tree.
                channel.Start = channel.NextCount + channel.Start;
                // Reset index.
                channel.Index = 0;
            }
            else
            {
                //Else step the tree.
                channel.Index++;
            }

            // Advance Channel
            channel.NextRoot = nextRoot;
            state.Channel = channel;

            // Generate attachement address
            var address = channel.Mode != MamMode.Public ? HashHelper.Hash(root) : root;

            return new MamMessage
            {
                State = state,
                Payload = payload,
                Root = root,
                Address = address
            };
        }

        private int[] CreateMaskedPayload(
            int[] seedTrits, int[] messageTrits,
            int[] keyTrits, int[] rootTrits,
            int[] siblingsTrits, int[] nextRootTrits,
            int start, int index, int security)
        {
            ICurl curl = new Curl(CurlMode.CurlP27);
            ICurl encrCurl = new Curl(CurlMode.CurlP27);
            var hammingNonce = new HammingNonce(CurlMode.CurlP27);

            var minLength = GetPayloadMinLength(messageTrits.Length, siblingsTrits.Length, index, security);
            var payloadLength = (int) TritsHelper.RoundThird(minLength);
            var payload = new int[payloadLength];

            // Creates a signed, encrypted payload from a message

            // generate the key and the get the merkle tree hashes
            var messageLength = messageTrits.Length;

            var indexP = Pascal.EncodedLength(index);
            var messageP = Pascal.EncodedLength(messageLength);

            var siblingsLength = siblingsTrits.Length;
            var siblingsCount = siblingsTrits.Length / Constants.HashLength;
            var siblingsPascalLength = Pascal.EncodedLength(siblingsCount);
            var signatureLength = security * Constants.KeyLength;

            var nextRootStart = indexP + messageP;
            var nextEnd = nextRootStart + nextRootTrits.Length;
            var messageEnd = nextRootStart + Constants.HashLength + messageLength;
            var nonceEnd = messageEnd + Constants.MessageNonceLength;
            var signatureEnd = nonceEnd + signatureLength;
            var siblingsPascalEnd = signatureEnd + siblingsPascalLength;
            var siblingsEnd = siblingsPascalEnd + siblingsLength;

            encrCurl.Absorb(keyTrits);
            encrCurl.Absorb(rootTrits);

            var trits = new int[indexP];
            Pascal.Encode(index, trits);
            Array.Copy(trits, 0, payload, 0, indexP);

            trits = new int[messageP];
            Pascal.Encode(messageLength, trits);
            Array.Copy(trits, 0, payload, indexP, messageP);

            encrCurl.Absorb(payload, 0, nextRootStart);
            Array.Copy(nextRootTrits, 0, payload, nextRootStart, nextRootTrits.Length);
            Array.Copy(messageTrits, 0, payload, nextEnd, messageTrits.Length);
            MaskHelper.MaskSlice(payload, nextRootStart, messageEnd - nextRootStart, encrCurl);

            Array.Copy(encrCurl.State, curl.State, encrCurl.State.Length);


            hammingNonce.Search(security, 0, Constants.HashLength / 3, curl);
            Array.Copy(curl.State, 0, payload, messageEnd, Constants.MessageNonceLength);
            MaskHelper.MaskSlice(payload, messageEnd, nonceEnd - messageEnd, encrCurl);

            curl.Reset();
            var subseed = HashHelper.Subseed(seedTrits, start + index, curl);
            Array.Copy(subseed, 0, payload, nonceEnd, subseed.Length);

            curl.Reset();
            HashHelper.Key(payload, nonceEnd, signatureEnd - nonceEnd, security, curl);

            curl.Reset();
            HashHelper.Signature(encrCurl.Rate, payload, nonceEnd, signatureEnd - nonceEnd, curl);

            curl.Reset();

            trits = new int[siblingsPascalLength];
            Pascal.Encode(siblingsCount, trits);
            Array.Copy(trits, 0, payload, signatureEnd, siblingsPascalLength);
            Array.Copy(siblingsTrits, 0, payload, siblingsPascalEnd, siblingsLength);

            MaskHelper.MaskSlice(payload, nonceEnd, siblingsEnd - nonceEnd, encrCurl);
            encrCurl.Reset();

            return payload;
        }

        private int GetPayloadMinLength(int messageTritsLength, int siblingsTritsLength, int index, int security)
        {
            return
                Pascal.EncodedLength(index) +
                Pascal.EncodedLength(Constants.HashLength + messageTritsLength) + Constants.HashLength +
                messageTritsLength + Constants.MessageNonceLength + security * Constants.KeyLength +
                Pascal.EncodedLength(siblingsTritsLength / Constants.HashLength) + siblingsTritsLength;
        }

        public void Attach(string trytes, string address, int depth = 6, int mwm = 14)
        {
            var tag = "999999999999999999999999999";
            var transfers = new List<Transfer>
            {
                new Transfer(address, 0, trytes, tag)
            };

            // ReSharper disable once UnusedVariable
            var result = Iota.SendTransfer(TrytesHelper.KeyGen(81), 2, depth, mwm, transfers.ToArray(), null, null,
                false, true, null);
        }

        public Tuple<List<string>, string> Fetch(string root, MamMode mode, string sideKey = null,
            Action<string> action = null)
        {
            var messages = new List<string>();

            var nextRoot = root;
            var lastNextRoot = root;

            // ReSharper disable NotAccessedVariable
            var transactionCount = 0;
            var messageCount = 0;
            // ReSharper restore NotAccessedVariable

            while (true)
            {
                // Apply channel mode
                var address = mode != MamMode.Public ? HashHelper.Hash(nextRoot) : nextRoot;

                var response = Iota.FindTransactionsByAddresses(address);

                if (response.Hashes.Count == 0)
                    break;

                transactionCount += response.Hashes.Count;
                messageCount++;

                var messagesGen = TransactionsToMessages(response.Hashes);
                foreach (var payload in messagesGen)
                    try
                    {
                        var unmaskedMessage = DecodeMessage(payload, sideKey, nextRoot);
                        var message = unmaskedMessage.Item1;
                        lastNextRoot = unmaskedMessage.Item2;

                        if (action != null)
                            action.Invoke(message);
                        else
                            messages.Add(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                nextRoot = lastNextRoot;
            }

            return new Tuple<List<string>, string>(messages, nextRoot);
        }

        /// <summary>
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="sideKey"></param>
        /// <param name="root"></param>
        /// <returns>(message,nextRoot)</returns>
        public Tuple<string, string> DecodeMessage(string payload, string sideKey, string root)
        {
            if (string.IsNullOrEmpty(sideKey))
                sideKey = "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

            var payloadTrits = Converter.ToTrits(payload);
            var sideKeyTrits = Converter.ToTrits(sideKey);
            var rootTrits = Converter.ToTrits(root);

            ICurl curl = new Curl(CurlMode.CurlP27);

            // parse
            var result = Pascal.Decode(payloadTrits);
            var index = (int) result.Item1;
            var indexEnd = result.Item2;

            var tempTrits = new int[payloadTrits.Length - indexEnd];
            Array.Copy(payloadTrits, indexEnd, tempTrits, 0, tempTrits.Length);

            result = Pascal.Decode(tempTrits);
            var messageLength = (int) result.Item1;
            var messageLengthEnd = result.Item2;

            var nextRootStart = indexEnd + messageLengthEnd;
            var messageStart = nextRootStart + Constants.HashLength;
            var messageEnd = messageStart + messageLength;

            curl.Absorb(sideKeyTrits);
            curl.Absorb(rootTrits);

            if (messageLength > payloadTrits.Length)
                throw new ArgumentOutOfRangeException();

            curl.Absorb(payloadTrits, 0, nextRootStart);

            MaskHelper.UnMaskSlice(payloadTrits, nextRootStart, messageStart - nextRootStart, curl);
            MaskHelper.UnMaskSlice(payloadTrits, messageStart, messageEnd - messageStart, curl);

            var pos = messageEnd;
            MaskHelper.UnMaskSlice(payloadTrits, pos, Constants.MessageNonceLength, curl);
            pos += Constants.HashLength / 3;

            var hmac = new int[Constants.HashLength];
            Array.Copy(curl.Rate, hmac, Constants.HashLength);

            var security = HashHelper.CheckSumSecurity(hmac);
            MaskHelper.UnMaskSlice(payloadTrits, pos, payloadTrits.Length - pos, curl);

            if (security == 0)
            {
                // InvalidHash
                curl.Reset();
                throw new ApplicationException();
            }


            var sigEnd = pos + security * Constants.KeyLength;
            HashHelper.DigestBundleSignature(hmac, payloadTrits, pos, sigEnd - pos, curl);

            Array.Copy(curl.Rate, hmac, Constants.HashLength);
            curl.Reset();

            pos = sigEnd;
            tempTrits = new int[payloadTrits.Length - pos];
            Array.Copy(payloadTrits, pos, tempTrits, 0, tempTrits.Length);
            result = Pascal.Decode(tempTrits);

            curl.Absorb(hmac);
            if (result.Item1 != 0)
            {
                // get address lite
                Array.Copy(curl.Rate, hmac, Constants.HashLength);
                pos += result.Item2;
                var sibEnd = pos + (int) result.Item1;

                var siblings = new int[sibEnd - pos];
                Array.Copy(payloadTrits, pos, siblings, 0, siblings.Length);

                curl.Reset();
                MerkleTree.Root(hmac, siblings, index, curl);
            }

            if (!curl.Rate.SequenceEqual(rootTrits))
            {
                // InvalidSignature
                curl.Reset();
                throw new ApplicationException();
            }

            var message = Converter.ToTrytes(payloadTrits, messageStart, messageLength);
            var nextRoot = Converter.ToTrytes(payloadTrits, nextRootStart, Constants.HashLength);
            return new Tuple<string, string>(message, nextRoot);
        }

        private List<string> TransactionsToMessages(List<string> hashes)
        {
            var messages = new List<string>();

            var transactions = Iota.FindTransactionObjectsByHashes(hashes.ToArray());

            var bundles
                = new Dictionary<string, List<Transaction>>();

            foreach (var transaction in transactions)
            {
                if (bundles.ContainsKey(transaction.Bundle))
                    bundles[transaction.Bundle].Add(transaction);
                else
                    bundles.Add(transaction.Bundle, new List<Transaction> {transaction});

                if (bundles[transaction.Bundle].Count == transaction.LastIndex + 1)
                {
                    bundles[transaction.Bundle].Sort((x, y) => x.CurrentIndex.CompareTo(y.CurrentIndex));

                    var message = bundles[transaction.Bundle]
                        .Aggregate("", (acc, e) => acc + e.SignatureMessageFragment);

                    if (!string.IsNullOrEmpty(message))
                        messages.Add(message);
                }
            }

            return messages;
        }
    }
}