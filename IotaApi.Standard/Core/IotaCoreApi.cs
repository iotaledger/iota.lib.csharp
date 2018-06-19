using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Api.Model;
using Iota.Api.Utils;

namespace Iota.Api.Core
{
    /// <summary>
    /// This class provides access to the Iota core API
    /// </summary>
    public class IotaCoreApi
    {
        private readonly IGenericIotaCoreApi _genericIotaCoreApi;

        /// <summary>
        /// 
        /// </summary>
        public ILocalPoW LocalPow { get; set; }

        /// <summary>
        /// Creates a core api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        /// <param name="protocol"></param>
        public IotaCoreApi(string host, int port, string protocol)
        {
            _genericIotaCoreApi = new GenericIotaCoreApi(host, port,protocol);
        }

        /// <summary>
        /// Attaches the specified transactions (trytes) to the Tangle by doing Proof of Work. 
        /// You need to supply branchTransaction as well as trunkTransaction 
        /// (basically the tips which you're going to validate and reference with this transaction)
        ///  - both of which you'll get through the getTransactionsToApprove API call.
        /// </summary>
        /// <param name="trunkTransaction">Trunk transaction to approve.</param>
        /// <param name="branchTransaction">Branch transaction to approve.</param>
        /// <param name="trytes">List of trytes (raw transaction data) to attach to the tangle.</param>
        /// <param name="minWeightMagnitude">Proof of Work intensity. Minimum value is 18</param>
        /// <returns>The returned value contains a different set of tryte values which you can input into broadcastTransactions and storeTransactions. 
        /// The returned tryte value, the last 243 trytes basically consist of the: trunkTransaction + branchTransaction + nonce. 
        /// These are valid trytes which are then accepted by the network.</returns>
        public AttachToTangleResponse AttachToTangle(string trunkTransaction, string branchTransaction,
            string[] trytes, int minWeightMagnitude = 18)
        {
            if (!InputValidator.IsHash(trunkTransaction))
                throw new ArgumentException("Invalid hashes provided.");

            if (!InputValidator.IsHash(branchTransaction))
                throw new ArgumentException("Invalid hashes provided.");

            if (!InputValidator.IsArrayOfTrytes(trytes, 2673))
                throw new ArgumentException("Invalid trytes provided.");

            if (LocalPow != null)
            {
                var response = new AttachToTangleResponse
                {
                    Trytes = new List<string>()
                };

                string previousTransaction = null;
                foreach (var t in trytes)
                {
                    var txn = new Transaction(t)
                    {
                        TrunkTransaction = previousTransaction ?? trunkTransaction,
                        BranchTransaction = previousTransaction == null ? branchTransaction : trunkTransaction
                    };

                    if (string.IsNullOrEmpty(txn.Tag) || Regex.IsMatch(txn.Tag, "9*"))
                        txn.Tag = txn.ObsoleteTag;
                    txn.AttachmentTimestamp = IotaApiUtils.CreateTimeStampNow();
                    txn.AttachmentTimestampLowerBound = 0;
                    txn.AttachmentTimestampUpperBound = 3_812_798_742_493L;

                    var resultTrytes = LocalPow.PerformPoW(txn.ToTransactionTrytes(), minWeightMagnitude);

                    previousTransaction = new Transaction(resultTrytes).Hash;

                    response.Trytes.Add(resultTrytes);
                }

                return response;
            }

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction,
                trytes, minWeightMagnitude);
            return _genericIotaCoreApi.Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        /// <summary>
        /// Broadcasts the transactions.
        /// </summary>
        /// <param name="trytes">The transactions in trytes representation</param>
        /// <returns>the BroadcastTransactionsResponse <see cref="BroadcastTransactionsResponse"/></returns>
        public BroadcastTransactionsResponse BroadcastTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(
                    new BroadcastTransactionsRequest(trytes));
        }

        /// <summary>
        /// Finds the transactions using the specified arguments as search criteria
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="approves">The approves.</param>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactions(List<string> addresses, List<string> tags,
            List<string> approves, List<string> bundles)
        {
            var findTransactionsRequest = new FindTransactionsRequest(bundles, addresses, tags, approves);
            return
                _genericIotaCoreApi.Request<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        /// <summary>
        /// Gets the balances.
        /// </summary>
        /// <param name="addresses">List of addresses you want to get the confirmed balance from</param>
        /// <param name="threshold">Confirmation threshold, should be set to 100.</param>
        /// <returns> It returns the confirmed balance which a list of addresses have at the latest confirmed milestone. 
        /// In addition to the balances, it also returns the milestone as well as the index with which the confirmed balance was determined. 
        /// The balances is returned as a list in the same order as the addresses were provided as input.</returns>
        public GetBalancesResponse GetBalances(List<string> addresses, long threshold = 100)
        {
            List<string> addressesWithoutChecksum = new List<string>();
            foreach (var address in addresses)
            {
                string address0 = address.RemoveChecksum();
                addressesWithoutChecksum.Add(address0);
            }

            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addressesWithoutChecksum, threshold);
            return _genericIotaCoreApi.Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// Gets the inclusion states of the specified transactions
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="milestones">The milestones.</param>
        /// <returns>a GetInclusionStatesResponse, see <see cref="GetInclusionStatesResponse"/></returns>
        public GetInclusionStatesResponse GetInclusionStates(string[] transactions, string[] milestones)
        {
            return
                _genericIotaCoreApi.Request<GetInclusionStatesRequest, GetInclusionStatesResponse>(
                    new GetInclusionStatesRequest(transactions, milestones));
        }

        /// <summary>
        /// Stores the specified transactions in trytes into the local storage. The trytes to be used for this call are returned by attachToTangle.
        /// </summary>
        /// <param name="trytes">The trytes representing the transactions</param>
        /// <returns>a <see cref="StoreTransactionsResponse"/></returns>
        public StoreTransactionsResponse StoreTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<StoreTransactionsRequest, StoreTransactionsResponse>(
                    new StoreTransactionsRequest(trytes));
        }

        /// <summary>
        /// Gets the node information.
        /// </summary>
        /// <returns>a <see cref="GetNodeInfoResponse"/> containing information about the node.</returns>
        public GetNodeInfoResponse GetNodeInfo()
        {
            return _genericIotaCoreApi.Request<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        /// <summary>
        /// Gets the tips.
        /// </summary>
        /// <returns>a <see cref="GetTipsResponse"/> containing a list of tips</returns>
        public GetTipsResponse GetTips()
        {
            GetTipsRequest getTipsRequest = new GetTipsRequest();
            return _genericIotaCoreApi.Request<GetTipsRequest, GetTipsResponse>(getTipsRequest);
        }

        /// <summary>
        /// Gets the transactions to approve.
        /// </summary>
        /// <param name="depth">The depth is the number of bundles to go back to determine the transactions for approval. 
        /// The higher your depth value, the more "babysitting" you do for the network (as you have to confirm more transactions).</param>
        /// <returns> trunkTransaction and branchTransaction (result of the Tip selection)</returns>
        public GetTransactionsToApproveResponse GetTransactionsToApprove(int depth)
        {
            GetTransactionsToApproveRequest getTransactionsToApproveRequest = new GetTransactionsToApproveRequest(depth);
            return
                _genericIotaCoreApi.Request<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(
                    getTransactionsToApproveRequest);
        }

        /// <summary>
        /// Gets the raw transaction data (trytes) of a specific transaction.
        /// These trytes can then be easily converted into the actual transaction object using the constructor of Transaction
        /// </summary>
        /// <param name="hashes">The hashes of the transactions</param>
        /// <returns>a <see cref="GetTrytesResponse"/> containing a list of trytes</returns>
        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() {Hashes = hashes};
            return _genericIotaCoreApi.Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// Interrupts and completely aborts the attachToTangle process.
        /// </summary>
        /// <returns>an <see cref="InterruptAttachingToTangleResponse"/></returns>
        public InterruptAttachingToTangleResponse InterruptAttachingToTangle()
        {
            InterruptAttachingToTangleRequest request = new InterruptAttachingToTangleRequest();
            return
                _genericIotaCoreApi.Request<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(
                    request);
        }

        /// <summary>
        /// Gets the neighbors the node is connected to
        /// </summary>
        /// <returns>A <see cref="GetNeighborsResponse"/> containing the set of neighbors the node is connected to as well as their activity count. The activity counter is reset after restarting IRI.</returns>
        public GetNeighborsResponse GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            return _genericIotaCoreApi.Request<GetNeighborsRequest, GetNeighborsResponse>(getNeighborsRequest);
        }

        /// <summary>
        /// Adds the neighbor(s) to the node.  It should be noted that this is only temporary, and the added neighbors will be removed from your set of neighbors after you relaunch IRI.
        /// </summary>
        /// <param name="uris">The uris of the neighbors to add. The URI (Unique Resource Identification) format is "udp://IPADDRESS:PORT" </param>
        /// <returns><see cref="AddNeighborsResponse"/> containing the number of added Neighbors</returns>
        public AddNeighborsResponse AddNeighbors(params string[] uris)
        {
            return
                _genericIotaCoreApi.Request<AddNeighborsRequest, AddNeighborsResponse>(
                    new AddNeighborsRequest(uris.ToList()));
        }

        /// <summary>
        /// Removes the neighbor(s) from the node. 
        /// </summary>
        /// <param name="uris">The uris of the neighbors to add. The URI (Unique Resource Identification) format is "udp://IPADDRESS:PORT"</param>
        /// <returns>A <see cref="RemoveNeighborsResponse"/> containing the number of removed neighbors</returns>
        public RemoveNeighborsResponse RemoveNeighbors(params string[] uris)
        {
            RemoveNeighborsRequest removeNeighborsRequest = new RemoveNeighborsRequest(uris.ToList());
            return _genericIotaCoreApi.Request<RemoveNeighborsRequest, RemoveNeighborsResponse>(removeNeighborsRequest);
        }
    }
}