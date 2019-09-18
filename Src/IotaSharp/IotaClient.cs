using System;
using System.Collections.Generic;
using System.Linq;
using IotaSharp.Core;
using IotaSharp.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IotaSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaClient
    {
        private readonly JsonWebClient _jsonWebClient;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uriString"></param>
        public IotaClient(string uriString)
        {
            _jsonWebClient = new JsonWebClient();
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            UriString = uriString;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UriString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GetNodeInfoResponse GetNodeInfo()
        {
            return Request<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GetTipsResponse GetTips()
        {
            return Request<GetTipsRequest, GetTipsResponse>(new GetTipsRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"> without checksum</param>
        /// <param name="tags"></param>
        /// <param name="approvees"></param>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public FindTransactionsResponse FindTransactions(
            List<string> addresses, List<string> tags,
            List<string> approvees, List<string> bundles)
        {
            if (addresses != null)
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    addresses[i] = addresses[i].RemoveChecksum();
                }
            }

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (!InputValidator.IsTag(tag))
                        throw new ArgumentException(Constants.INVALID_TAG_INPUT_ERROR);
                }
            }

            if (approvees != null)
            {
                foreach (var approve in approvees)
                {
                    if (!InputValidator.IsHash(approve))
                        throw new ArgumentException(Constants.INVALID_BUNDLE_HASH_ERROR);
                }
            }

            if (bundles != null)
            {
                foreach (var bundle in bundles)
                {
                    if (!InputValidator.IsHash(bundle))
                        throw new ArgumentException(Constants.INVALID_BUNDLE_HASH_ERROR);

                }
            }


            var findTransactionsRequest = new FindTransactionsRequest(addresses, tags, approvees, bundles);
            return
                Request<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        /// <summary>
        /// Find the transactions by addresses
        /// </summary>
        /// <param name="addresses">An array of addresses</param>
        /// <returns></returns>
        public FindTransactionsResponse FindTransactionsByAddresses(params string[] addresses)
        {
            return FindTransactions(addresses.ToList(), null, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="approvees"></param>
        /// <returns></returns>
        public FindTransactionsResponse FindTransactionsByApprovees(params string[] approvees)
        {
            return FindTransactions(null, null, approvees.ToList(), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public FindTransactionsResponse FindTransactionsByBundles(params string[] bundles)
        {
            return FindTransactions(null, null, null, bundles.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public FindTransactionsResponse FindTransactionsByTags(params string[] tags)
        {
            return FindTransactions(null, tags.ToList(), null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            var getTrytesRequest = new GetTrytesRequest(hashes);
            return Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="milestones"></param>
        /// <returns></returns>
        public GetInclusionStatesResponse GetInclusionStates(string[] transactions, string[] milestones)
        {
            return
                Request<GetInclusionStatesRequest, GetInclusionStatesResponse>(
                    new GetInclusionStatesRequest(transactions, milestones));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public GetTransactionsToApproveResponse GetTransactionsToApprove(int depth, string reference = null)
        {
            var getTransactionsToApproveRequest =
                new GetTransactionsToApproveRequest(depth, reference);
            return
                Request<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(
                    getTransactionsToApproveRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threshold">The confirmation threshold, should be set to 100.</param>
        /// <param name="addresses"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        public GetBalancesResponse GetBalances(long threshold, List<string> addresses, List<string> tips)
        {
            List<string> addressesWithoutChecksum = new List<string>();
            foreach (var address in addresses)
            {
                addressesWithoutChecksum.Add(address.RemoveChecksum());
            }

            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(threshold, addressesWithoutChecksum, tips);
            return Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public WereAddressesSpentFromResponse WereAddressesSpentFrom(params string[] addresses)
        {
            List<string> addressesWithoutChecksum = new List<string>();
            foreach (var address in addresses)
            {
                addressesWithoutChecksum.Add(address.RemoveChecksum());
            }

            WereAddressesSpentFromRequest wereAddressesSpentFromRequest =
                new WereAddressesSpentFromRequest(addressesWithoutChecksum);
            return Request<WereAddressesSpentFromRequest, WereAddressesSpentFromResponse>(
                wereAddressesSpentFromRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tails"></param>
        /// <returns></returns>
        public CheckConsistencyResponse CheckConsistency(params string[] tails)
        {
            if (!InputValidator.IsArrayOfHashes(tails))
                throw new ArgumentException("Invalid hashes provided.");

            var checkConsistencyRequest = new CheckConsistencyRequest(tails);
            return Request<CheckConsistencyRequest, CheckConsistencyResponse>(checkConsistencyRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InterruptAttachingToTangleResponse InterruptAttachingToTangle()
        {
            return
                Request<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(
                    new InterruptAttachingToTangleRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trunkTransaction"></param>
        /// <param name="branchTransaction"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public AttachToTangleResponse AttachToTangle(
            string trunkTransaction, string branchTransaction,
            int minWeightMagnitude, string[] trytes)
        {
            if (!InputValidator.IsHash(trunkTransaction))
                throw new ArgumentException(Constants.INVALID_HASH_INPUT_ERROR);

            if (!InputValidator.IsHash(branchTransaction))
                throw new ArgumentException(Constants.INVALID_HASH_INPUT_ERROR);

            if (!InputValidator.IsArrayOfRawTransactionTrytes(trytes))
                throw new ArgumentException(Constants.INVALID_TRYTES_INPUT_ERROR);

            var attachToTangleRequest =
                new AttachToTangleRequest(
                    trunkTransaction, branchTransaction, minWeightMagnitude, trytes);
            return Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public BroadcastTransactionsResponse BroadcastTransactions(string[] trytes)
        {
            if (!InputValidator.IsArrayOfRawTransactionTrytes(trytes))
                throw new ArgumentException(Constants.INVALID_ATTACHED_TRYTES_INPUT_ERROR);

            return
                Request<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(
                    new BroadcastTransactionsRequest(trytes));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public StoreTransactionsResponse StoreTransactions(string[] trytes)
        {
            if (!InputValidator.IsArrayOfRawTransactionTrytes(trytes))
                throw new ArgumentException(Constants.INVALID_ATTACHED_TRYTES_INPUT_ERROR);

            return
                Request<StoreTransactionsRequest, StoreTransactionsResponse>(
                    new StoreTransactionsRequest(trytes));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GetNeighborsResponse GetNeighbors()
        {
            return Request<GetNeighborsRequest, GetNeighborsResponse>(new GetNeighborsRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uris"></param>
        /// <returns></returns>
        public AddNeighborsResponse AddNeighbors(params string[] uris)
        {
            return
                Request<AddNeighborsRequest, AddNeighborsResponse>(
                    new AddNeighborsRequest(uris));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uris"></param>
        /// <returns></returns>
        public RemoveNeighborsResponse RemoveNeighbors(params string[] uris)
        {
            return
                Request<RemoveNeighborsRequest, RemoveNeighborsResponse>(
                    new RemoveNeighborsRequest(uris));
        }

        private TResponse Request<TRequest, TResponse>(TRequest request) where TResponse : new()
        {
            return _jsonWebClient.GetPOSTResponseSync<TResponse>(
                new Uri(UriString),
                JsonConvert.SerializeObject(request, _jsonSerializerSettings));
        }
    }
}
