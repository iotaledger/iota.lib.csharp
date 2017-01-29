using System.Collections.Generic;
using System.Linq;
using Iota.Lib.CSharp.Api.Core;

namespace Iota.Lib.CSharp.Api
{
    /// <summary>
    /// This class provides access to the Iota core API
    /// </summary>
    public class IotaCoreApi
    {
        private readonly IGenericIotaCoreApi _genericIotaCoreApi;

        public IotaCoreApi(string host, int port)
        {
            _genericIotaCoreApi = new GenericIotaCoreApi(host, port);
        }

        public AddNeighborsResponse AddNeighbors(params string[] uris)
        {
            return
                _genericIotaCoreApi.Request<AddNeighborsRequest, AddNeighborsResponse>(
                    new AddNeighborsRequest(uris.ToList()));
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
        /// <returns>The returned value is a different set of tryte values which you can input into broadcastTransactions and storeTransactions. 
        /// The returned tryte value, the last 243 trytes basically consist of the: trunkTransaction + branchTransaction + nonce. 
        /// These are valid trytes which are then accepted by the network.</returns>
        public AttachToTangleResponse AttachToTangle(string trunkTransaction, string branchTransaction,
            string[] trytes, int minWeightMagnitude = 18)
        {
            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction,
                trytes, minWeightMagnitude);
            return _genericIotaCoreApi.Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        public BroadcastTransactionsResponse BroadcastTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(
                    new BroadcastTransactionsRequest(trytes));
        }

        public FindTransactionsResponse FindTransactions(List<string> addresses, List<string> tags,
            List<string> approves, List<string> bundles)
        {
            FindTransactionsRequest findTransactionsRequest = new FindTransactionsRequest(bundles, addresses, tags,
                approves);
            return
                _genericIotaCoreApi.Request<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        public GetBalancesResponse GetBalances(List<string> addresses, long threshold = 100)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses, threshold);
            return _genericIotaCoreApi.Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        public GetInclusionStatesResponse GetInclusionStates(string[] transactions, string[] milestone)
        {
            return
                _genericIotaCoreApi.Request<GetInclusionStatesRequest, GetInclusionStatesResponse>(
                    new GetInclusionStatesRequest(transactions, milestone));
        }


        public StoreTransactionsResponse StoreTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<StoreTransactionsRequest, StoreTransactionsResponse>(
                    new StoreTransactionsRequest(trytes));
        }

        public GetNeighborsResponse GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            return _genericIotaCoreApi.Request<GetNeighborsRequest, GetNeighborsResponse>(getNeighborsRequest);
        }

        public GetNodeInfoResponse GetNodeInfo()
        {
            return _genericIotaCoreApi.Request<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        public GetTipsResponse GetTips()
        {
            GetTipsRequest getTipsRequest = new GetTipsRequest();
            return _genericIotaCoreApi.Request<GetTipsRequest, GetTipsResponse>(getTipsRequest);
        }

        public GetTransactionsToApproveResponse GetTransactionsToApprove(int depth)
        {
            GetTransactionsToApproveRequest getTransactionsToApproveRequest = new GetTransactionsToApproveRequest(depth);
            return
                _genericIotaCoreApi.Request<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(
                    getTransactionsToApproveRequest);
        }

        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() {Hashes = hashes};
            return _genericIotaCoreApi.Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        public InterruptAttachingToTangleResponse InterruptAttachingToTangle()
        {
            InterruptAttachingToTangleRequest request = new InterruptAttachingToTangleRequest();
            return
                _genericIotaCoreApi.Request<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(
                    request);
        }

        public RemoveNeighborsResponse RemoveNeighbors(params string[] uris)
        {
            RemoveNeighborsRequest removeNeighborsRequest = new RemoveNeighborsRequest(uris.ToList());
            return _genericIotaCoreApi.Request<RemoveNeighborsRequest, RemoveNeighborsResponse>(removeNeighborsRequest);
        }
    }
}