using System.Collections.Generic;
using System.Linq;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class provides access to the Iota core API
    /// </summary>
    public class IotaCoreApi
    {
        private readonly IGenericIotaCoreApi _genericIotaCoreApi;

        /// <summary>
        /// Creates a core api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        public IotaCoreApi(string host, int port)
        {
            _genericIotaCoreApi = new GenericIotaCoreApi(host, port);
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
            InputValidator.CheckIfArrayOfTrytes(trytes);

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction,
                trytes, minWeightMagnitude);
            return _genericIotaCoreApi.Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        /// <summary>
        /// Broadcasts the transactions.
        /// </summary>
        /// <param name="trytes">The transactions in trytes representation</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public FindTransactionsResponse FindTransactions(List<string> addresses, List<string> tags,
            List<string> approves, List<string> bundles)
        {
            FindTransactionsRequest findTransactionsRequest = new FindTransactionsRequest(bundles, addresses, tags,
                approves);
            return
                _genericIotaCoreApi.Request<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        /// <summary>
        /// Gets the balances.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns> It returns the confirmed balance which a list of addresses have at the latest confirmed milestone. 
        /// In addition to the balances, it also returns the milestone as well as the index with which the confirmed balance was determined. 
        /// The balances is returned as a list in the same order as the addresses were provided as input.</returns>
        public GetBalancesResponse GetBalances(List<string> addresses, long threshold = 100)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses, threshold);
            return _genericIotaCoreApi.Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// Gets the inclusion states.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="milestones">The milestones.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public StoreTransactionsResponse StoreTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<StoreTransactionsRequest, StoreTransactionsResponse>(
                    new StoreTransactionsRequest(trytes));
        }

        /// <summary>
        /// Gets the node information.
        /// </summary>
        /// <returns>information about the node.</returns>
        public GetNodeInfoResponse GetNodeInfo()
        {
            return _genericIotaCoreApi.Request<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        /// <summary>
        /// Gets the tips.
        /// </summary>
        /// <returns>a list of tips</returns>
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
        /// <returns></returns>
        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() {Hashes = hashes};
            return _genericIotaCoreApi.Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// Interrupts and completely aborts the attachToTangle process.
        /// </summary>
        /// <returns></returns>
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
        /// <returns>Returns the set of neighbors the node is connected to as well as their activity count. The activity counter is reset after restarting IRI.</returns>
        public GetNeighborsResponse GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            return _genericIotaCoreApi.Request<GetNeighborsRequest, GetNeighborsResponse>(getNeighborsRequest);
        }

        /// <summary>
        /// Adds the neighbor(s) to the node.  It should be noted that this is only temporary, and the added neighbors will be removed from your set of neighbors after you relaunch IRI.
        /// </summary>
        /// <param name="uris">The uris of the neighbors to add. The URI (Unique Resource Identification) format is "udp://IPADDRESS:PORT" </param>
        /// <returns>Returns the number of added Neighbors</returns>
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
        /// <returns>Returns the number of remvoved neighbors</returns>
        public RemoveNeighborsResponse RemoveNeighbors(params string[] uris)
        {
            RemoveNeighborsRequest removeNeighborsRequest = new RemoveNeighborsRequest(uris.ToList());
            return _genericIotaCoreApi.Request<RemoveNeighborsRequest, RemoveNeighborsResponse>(removeNeighborsRequest);
        }
    }
}