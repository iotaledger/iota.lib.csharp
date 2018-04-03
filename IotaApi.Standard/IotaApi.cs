using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iota.Api.Core;
using Iota.Api.Exception;
using Iota.Api.Model;
using Iota.Api.Pow;
using Iota.Api.Utils;

namespace Iota.Api
{
    /// <summary>
    ///     This class provides access to the core API methods and the proposed calls
    /// </summary>
    public class IotaApi : IotaCoreApi
    {
        private readonly ICurl _curl;

        /// <summary>
        ///     Creates an api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        /// <param name="protocol">http or https</param>
        public IotaApi(string host, int port, string protocol = "http") : this(host, port, new Kerl(),protocol)
        {
        }

        /// <summary>
        ///     Creates an api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        /// <param name="curl">
        ///     a custom curl implementation to be used to perform the pow. Use the other constructor in order to
        ///     use the default curl implementation provided by the library
        /// </param>
        /// <param name="protocol"></param>
        public IotaApi(string host, int port, ICurl curl,string protocol) : base(host, port,protocol)
        {
            _curl = curl ?? throw new ArgumentNullException(nameof(curl));
        }

        /// <summary>
        ///     Gets all possible inputs of a seed and returns them with the total balance.
        ///     This is either done deterministically (by genearating all addresses until findTransactions is empty and doing
        ///     getBalances),
        ///     or by providing a key range to use for searching through.
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="security">The Security level of private key / seed.</param>
        /// <param name="start">Starting key index</param>
        /// <param name="end">Ending key index</param>
        /// <param name="threshold">The minimum threshold of accumulated balances from the inputs that is required</param>
        /// <returns>The inputs (see <see cref="Input" />) </returns>
        public Inputs GetInputs(string seed, int security, int start, int end, long threshold)
        {
            InputValidator.CheckIfValidSeed(seed);

            seed = InputValidator.PadSeedIfNecessary(seed);

            if (security < 1)
                throw new ArgumentException("invalid security level provided");

            // If start value bigger than end, return error
            if (start > end)
                throw new ArgumentException("start must be smaller than end", nameof(start));

            // or if difference between end and start is bigger than 500 keys
            if (end - start > 500)
                throw new ArgumentException("total number of keys exceeded 500");

            //  Case 1: start and end
            //
            //  If start and end is defined by the user, simply iterate through the keys
            //  and call getBalances
            if (end != 0)
            {
                var allAddresses = new string[end - start];

                for (var i = start; i < end; i++)
                {
                    var address = IotaApiUtils.NewAddress(seed, security, i, false, _curl);
                    allAddresses[i] = address;
                }

                return GetBalanceAndFormat(allAddresses, threshold, start, security);
            }

            //  Case 2: iterate till threshold || end
            //
            //  Either start from index: 0 or start (if defined) until threshold is reached.
            //  Calls getNewAddress and deterministically generates and returns all addresses
            //  We then do getBalance, format the output and return it

            var addresses = GetNewAddress(seed, security, start, false, 0, true);
            return GetBalanceAndFormat(addresses, threshold, start, security);
        }

        /// <summary>
        ///     Gets the balances of the specified addresses and calculates the total balance till the threshold is reached.
        /// </summary>
        /// <param name="addresses">addresses</param>
        /// <param name="threshold">the threshold </param>
        /// <param name="start">start index</param>
        /// <param name="security"></param>
        /// <returns>an Inputs object</returns>
        /// <exception cref="NotEnoughBalanceException">
        ///     is thrown if threshold exceeds the sum of balance of the specified
        ///     addresses
        /// </exception>
        private Inputs GetBalanceAndFormat(
            string[] addresses,
            long threshold, int start,
            int security)
        {
            if (security < 1)
                throw new ArgumentException("invalid security level provided");

            var getBalancesResponse = GetBalances(addresses.ToList(), 100);

            var balances = getBalancesResponse.Balances;

            var inputs = new Inputs {InputsList = new List<Input>(), TotalBalance = 0};

            var threshholdReached = threshold == 0;

            for (var i = 0; i < addresses.Length; i++)
                if (balances[i] > 0)
                {
                    inputs.InputsList.Add(new Input
                    {
                        Address = addresses[i],
                        Balance = balances[i],
                        KeyIndex = start + i,
                        Security = security
                    });

                    inputs.TotalBalance += balances[i];

                    if (inputs.TotalBalance >= threshold)
                    {
                        threshholdReached = true;
                        break;
                    }
                }

            if (threshholdReached)
                return inputs;


            throw new NotEnoughBalanceException();
        }

        /// <summary>
        ///     Main purpose of this function is to get an array of transfer objects as input, and then prepare the transfer by
        ///     generating the correct bundle,
        ///     as well as choosing and signing the inputs if necessary (if it's a value transfer). The output of this function is
        ///     an array of the raw transaction data (trytes)
        /// </summary>
        /// <param name="seed">81-tryte encoded address of recipient</param>
        /// <param name="security"></param>
        /// <param name="transfers">the transfers to prepare</param>
        /// <param name="inputs">Optional (default null). The inputs</param>
        /// <param name="remainderAddress">
        ///     Optional (default null). if defined, this address will be used for sending the remainder
        ///     value (of the inputs) to.
        /// </param>
        /// <param name="validateInputs"></param>
        /// <returns>a list containing the trytes of the new bundle</returns>
        public List<string> PrepareTransfers(
            string seed, int security,
            Transfer[] transfers,
            string remainderAddress,
            List<Input> inputs,
            bool validateInputs)
        {
            // validate seed
            if (!InputValidator.IsValidSeed(seed))
                throw new IllegalStateException("Invalid seed provided.");
            

            if(security<1)
                throw new ArgumentException("Invalid security level provided.");

            // Input validation of transfers object
            InputValidator.CheckTransferArray(transfers);

            // Create a new bundle
            var bundle = new Bundle();
            var signatureFragments = new List<string>();

            long totalValue = 0;
            var tag = "";

            //
            //  Iterate over all transfers, get totalValue
            //  and prepare the signatureFragments, message and tag
            //
            foreach (var transfer in transfers)
            {
                // remove the checksum of the address if provided
                transfer.Address = transfer.Address.RemoveChecksum();
                
                var signatureMessageLength = 1;

                // If message longer than 2187 trytes, increase signatureMessageLength (add 2nd transaction)
                if (transfer.Message.Length > Constants.MessageLength)
                {
                    // Get total length, message / maxLength (2187 trytes)
                    signatureMessageLength += (int) Math.Floor((double) transfer.Message.Length / Constants.MessageLength);

                    var msgCopy = transfer.Message;

                    // While there is still a message, copy it
                    while (!string.IsNullOrEmpty(msgCopy))
                    {
                        var fragment = msgCopy.Substring(0, 2187 > msgCopy.Length ? msgCopy.Length : 2187);
                        msgCopy = msgCopy.Substring(2187, msgCopy.Length - 2187);

                        // Pad remainder of fragment
                        while (fragment.Length < 2187) fragment += '9';
                        
                        signatureFragments.Add(fragment);
                    }
                }
                else
                {
                    // Else, get single fragment with 2187 of 9's trytes
                    var fragment = string.Empty;

                    if (!string.IsNullOrEmpty(transfer.Message))
                        fragment = transfer.Message.Substring(0,
                            transfer.Message.Length < 2187 ? transfer.Message.Length : 2187);

                    while (fragment.Length < 2187) fragment += '9';

                    signatureFragments.Add(fragment);
                }

                // get current timestamp in seconds
                var timestamp = (long)Math.Floor((double)IotaApiUtils.CreateTimeStampNow()/1000);

                // If no tag defined, get 27 tryte tag.
                
                tag = string.IsNullOrEmpty(transfer.Tag) ? "999999999999999999999999999" : transfer.Tag;
                

                // Pad for required 27 tryte length
                while (tag.Length < 27) tag += '9';


                // Add first entries to the bundle
                // Slice the address in case the user provided a checksummed one
                bundle.AddEntry(signatureMessageLength, transfer.Address, transfer.Value, tag, timestamp);
                // Sum up total value
                totalValue += transfer.Value;
            }

            // Get inputs if we are sending tokens
            if (totalValue != 0)
                if (inputs != null && inputs.Count > 0)
                {
                    // Get list if addresses of the provided inputs
                    var inputAddresses = new List<string>();
                    foreach (var input in inputs) inputAddresses.Add(input.Address);

                    var balances = GetBalances(inputAddresses, 100);

                    var confirmedInputs = new List<Input>();

                    long totalBalance = 0;
                    for (var i = 0; i < balances.Balances.Count; i++)
                    {
                        var thisBalance = balances.Balances[i];
                        totalBalance += thisBalance;

                        // If input has balance, add it to confirmedInputs
                        if (thisBalance > 0)
                        {
                            var inputEl = inputs[i];
                            inputEl.Balance = thisBalance;

                            confirmedInputs.Add(inputEl);
                        }
                    }

                    // Return not enough balance error
                    if (totalValue > totalBalance) throw new NotEnoughBalanceException(totalValue);

                    return AddRemainder(seed, security, confirmedInputs, bundle, tag, totalValue, remainderAddress,
                        signatureFragments);
                }

                //  Case 2: Get inputs deterministically
                //
                //  If no inputs provided, derive the addresses from the seed and
                //  confirm that the inputs exceed the threshold
                else
                {
                    var inputList = GetInputs(seed, security, 0, 0, (int) totalValue).InputsList;
                    return AddRemainder(seed, security, inputList, bundle, tag, totalValue, remainderAddress,
                        signatureFragments);
                }

            // If no input required, don't sign and simply finalize the bundle
            bundle.FinalizeBundle(_curl.Clone());
            bundle.AddTrytes(signatureFragments);

            var bundleTrytes = new List<string>();
            bundle.Transactions.ForEach(tx => bundleTrytes.Add(tx.ToTransactionTrytes()));

            bundleTrytes.Reverse();
            return bundleTrytes;
        }


        private List<string> AddRemainder(
            string seed,
            int security,
            List<Input> inputs,
            Bundle bundle,
            string tag,
            long totalValue,
            string remainderAddress,
            List<string> signatureFragments)
        {
            var totalTransferValue = totalValue;

            foreach (var input in inputs)
            {
                var thisBalance = input.Balance;
                var toSubtract = 0 - thisBalance;
                var timestamp = IotaApiUtils.CreateTimeStampNow();

                // Add input as bundle entry
                bundle.AddEntry(security, input.Address, toSubtract, tag, timestamp);
                // If there is a remainder value
                // Add extra output to send remaining funds to

                if (thisBalance >= totalTransferValue)
                {
                    var remainder = thisBalance - totalTransferValue;

                    // If user has provided remainder address
                    // Use it to send remaining funds to
                    if (remainder > 0 && remainderAddress != null)
                    {
                        // Remainder bundle entry
                        bundle.AddEntry(1, remainderAddress, remainder, tag, timestamp);

                        // function for signing inputs
                        IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, _curl);
                    }
                    else if (remainder > 0)
                    {
                        // Generate a new Address by calling getNewAddress
                        // ReSharper disable RedundantArgumentDefaultValue
                        var address = GetNewAddress(seed, security, 0, false, 0, false)[0];
                        // ReSharper restore RedundantArgumentDefaultValue

                        // Remainder bundle entry
                        bundle.AddEntry(1, address, remainder, tag, timestamp);

                        // function for signing inputs
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, _curl);
                    }
                    else
                    {
                        // If there is no remainder, do not add transaction to bundle
                        // simply sign and return
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, _curl);
                    }
                }
                // If multiple inputs provided, subtract the totalTransferValue by
                // the inputs balance
                else
                {
                    totalTransferValue -= thisBalance;
                }
            }

            throw new NotEnoughBalanceException(totalValue);
        }


        /// <summary>
        ///     Generates a new address from a seed and returns the remainderAddress. This is either done deterministically, or by
        ///     providing the index of the new remainderAddress
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="security"></param>
        /// <param name="index">
        ///     Optional (default null). Key index to start search from. If the index is provided, the generation
        ///     of the address is not deterministic.
        /// </param>
        /// <param name="checksum">Optional (default false). Adds 9-tryte address checksum</param>
        /// <param name="total">Optional (default 1)Total number of addresses to generate.</param>
        /// <param name="returnAll">
        ///     If true, it returns all addresses which were deterministically generated (until
        ///     findTransactions returns null)
        /// </param>
        /// <returns>an array of strings with the specifed number of addresses</returns>
        public string[] GetNewAddress(string seed, int security, int index = 0, bool checksum = false, int total = 0,
            bool returnAll = false)
        {
            var allAdresses = new List<string>();

            // TODO make two different functions out of this

            // Case 1: total
            //
            // If total number of addresses to generate is supplied, simply generate
            // and return the list of all addresses
            if (total > 0)
            {
                // Increase index with each iteration
                for (var i = index; i < index + total; i++)
                    allAdresses.Add(IotaApiUtils.NewAddress(seed, security, i, checksum, new Kerl()));

                return allAdresses.ToArray();
            }

            //  Case 2: no total provided
            //
            //  Continue calling findTransactions to see if address was already created
            //  if null, return list of addresses
            //

            var addresses = new List<string>();

            for (var i = index;; i++)
            {
                var newAddress = IotaApiUtils.NewAddress(seed, security, i, checksum, new Kerl());
                var response = FindTransactionsByAddresses(newAddress);

                if (returnAll) addresses.Add(newAddress);

                if (response.Hashes.Count == 0)
                    break;
            }

            return addresses.ToArray();
        }

        /// <summary>
        ///     Gets the transfers which are associated with a seed.
        ///     The transfers are determined by either calculating deterministically which addresses were already used,
        ///     or by providing a list of indexes to get the transfers from.
        /// </summary>
        /// <param name="seed">tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="inclusionStates">If True, it gets the inclusion states of the transfers.</param>
        /// <param name="security"></param>
        /// <param name="start">the address start index</param>
        /// <param name="end">the address end index</param>
        /// <returns>An Array of Bundle object that represent the transfers</returns>
        public Bundle[] GetTransfers(string seed, int security, int? start, int? end, bool inclusionStates = false)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            if (!start.HasValue)
                start = 0;
            if (!end.HasValue)
                end = 0;

            // If start value bigger than end, return error
            // or if difference between end and start is bigger than 500 keys
            if (start.Value > end.Value || end.Value > start + 500)
                throw new System.Exception("Invalid inputs provided: start, end");

            // first call findTransactions
            // If a transaction is non tail, get the tail transactions associated with it
            // add it to the list of tail transactions

            var addresses = GetNewAddress(seed, security, start.Value, false,
                end.Value, true);


            var bundles = BundlesFromAddresses(addresses, inclusionStates);
            return bundles;
        }

        private Bundle[] BundlesFromAddresses(string[] addresses, bool inclusionStates)
        {
            var trxs = FindTransactionObjects(addresses);
            // set of tail transactions
            var tailTransactions = new List<string>();
            var nonTailBundleHashes = new List<string>();

            foreach (var trx in trxs)
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                {
                    tailTransactions.Add(trx.Hash);
                }
                else
                {
                    if (nonTailBundleHashes.IndexOf(trx.Bundle) == -1) nonTailBundleHashes.Add(trx.Bundle);
                }

            var bundleObjects = FindTransactionObjectsByBundle(nonTailBundleHashes.ToArray());
            foreach (var trx in bundleObjects)
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                    if (tailTransactions.IndexOf(trx.Hash) == -1)
                        tailTransactions.Add(trx.Hash);

            var finalBundles = new List<Bundle>();
            var tailTxArray = tailTransactions.ToArray();

            // If inclusionStates, get the confirmation status
            // of the tail transactions, and thus the bundles
            GetInclusionStatesResponse gisr = null;
            if (inclusionStates)
            {
                try
                {
                    gisr = GetLatestInclusion(tailTxArray);
                }
                catch (IllegalAccessError)
                {
                    // suppress exception (the check is done below)
                }

                if (gisr == null || gisr.States == null || gisr.States.Count == 0)
                    throw new ArgumentException("Inclusion states not found");
            }


            var finalInclusionStates = gisr;

            Parallel.ForEach(tailTransactions, param =>
            {
                try
                {
                    var b = GetBundle(param);

                    if (inclusionStates)
                    {
                        var thisInclusion = finalInclusionStates != null &&
                                            finalInclusionStates.States[tailTxArray.ToList().IndexOf(param)];
                        foreach (var t in b.Transactions) t.Persistance = thisInclusion;
                    }

                    finalBundles.Add(b);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Bundle error: " + ex.Message);
                }
            });

            finalBundles.Sort();
            var returnValue = new Bundle[finalBundles.Count];
            for (var i = 0; i < finalBundles.Count; i++)
                returnValue[i] = new Bundle(finalBundles[i].Transactions, finalBundles[i].Transactions.Count);
            return returnValue;
        }

        /// <summary>
        ///     Finds the transaction objects.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> FindTransactionObjects(string[] addresses)
        {
            var addressesWithoutChecksum =
                addresses.Select(address => address.RemoveChecksum()).ToList();

            var ftr = FindTransactions(addressesWithoutChecksum, null, null, null);
            if (ftr?.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }

        /// <summary>
        ///     Gets the transactions objects.
        /// </summary>
        /// <param name="hashes">The hashes in trytes</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> GetTransactionsObjects(string[] hashes)
        {
            if (!InputValidator.IsArrayOfHashes(hashes))
                throw new IllegalStateException("Not an Array of Hashes: " + hashes);

            var trytesResponse = GetTrytes(hashes);

            var trxs = new List<Transaction>();

            foreach (var tryte in trytesResponse.Trytes) trxs.Add(new Transaction(tryte, _curl));
            return trxs;
        }

        /// <summary>
        ///     Finds the transaction objects by bundle.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a list of Transaction objects</returns>
        public List<Transaction> FindTransactionObjectsByBundle(string[] bundles)
        {
            var ftr = FindTransactions(null, null, null, bundles.ToList());
            if (ftr == null || ftr.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }


        /// <summary>
        ///     Replays the bundle.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude.</param>
        /// <returns>an array of boolean that indicate which transactions have been replayed successfully</returns>
        public bool[] ReplayBundle(string transaction, int depth, int minWeightMagnitude)
        {
            //StopWatch stopWatch = new StopWatch();

            var bundleTrytes = new List<string>();

            var bundle = GetBundle(transaction);

            bundle.Transactions.ForEach(t => bundleTrytes.Add(t.ToTransactionTrytes()));

            var trxs = SendTrytes(bundleTrytes.ToArray(), depth, minWeightMagnitude).ToList();

            var successful = new bool[trxs.Count];

            for (var i = 0; i < trxs.Count; i++)
            {
                var response = FindTransactionsByBundles(trxs[i].Bundle);
                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        ///     Finds the transactions by bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse" /></returns>
        public FindTransactionsResponse FindTransactionsByBundles(params string[] bundles)
        {
            return FindTransactions(null, null, null, bundles.ToList());
        }

        /// <summary>
        ///     Finds the transactions by approvees.
        /// </summary>
        /// <param name="approvees">The approvees.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse" /></returns>
        public FindTransactionsResponse FindTransactionsByApprovees(params string[] approvees)
        {
            return FindTransactions(null, null, approvees.ToList(), null);
        }


        /// <summary>
        ///     Finds the transactions by digests.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse" /></returns>
        public FindTransactionsResponse FindTransactionsByDigests(params string[] bundles)
        {
            return FindTransactions(null, bundles.ToList(), null, null);
        }

        /// <summary>
        ///     Finds the transactions by addresses.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse" /></returns>
        public FindTransactionsResponse FindTransactionsByAddresses(params string[] addresses)
        {
            var addressesWithoutChecksum = new List<string>();
            foreach (var address in addresses)
            {
                var address0 = address.RemoveChecksum();
                addressesWithoutChecksum.Add(address0);
            }

            return FindTransactions(addressesWithoutChecksum, null, null, null);
        }

        /// <summary>
        ///     Gets the latest inclusion.
        /// </summary>
        /// <param name="hashes">The hashes.</param>
        /// <returns>a GetInclusionStatesResponse cotaining the inclusion state of the specified hashes</returns>
        public GetInclusionStatesResponse GetLatestInclusion(string[] hashes)
        {
            string[] latestMilestone = {GetNodeInfo().LatestSolidSubtangleMilestone};
            return GetInclusionStates(hashes, latestMilestone);
        }


        /// <summary>
        ///     Wrapper function that basically does prepareTransfers, as well as attachToTangle and finally, it broadcasts and
        ///     stores the transactions locally.
        /// </summary>
        /// <param name="seed">tryte-encoded seed</param>
        /// <param name="security"></param>
        /// <param name="depth">depth</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude</param>
        /// <param name="transfers">Array of transfer objects</param>
        /// <param name="inputs">Optional (default null). List of inputs used for funding the transfer</param>
        /// <param name="remainderAddress">
        ///     Optional (default null). If defined, this address will be used for sending the remainder value
        ///     (of the inputs) to
        /// </param>
        /// <param name="validateInputs"></param>
        /// <param name="validateInputAddresses"></param>
        /// <returns> an array of the boolean that indicates which Transactions where sent successully</returns>
        public bool[] SendTransfer(
            string seed, int security, int depth,
            int minWeightMagnitude, Transfer[] transfers,
            Input[] inputs, string remainderAddress,
            bool validateInputs, bool validateInputAddresses)
        {
            var trytes = PrepareTransfers(seed, security, transfers,
                remainderAddress, inputs?.ToList(), validateInputs);
            var trxs = SendTrytes(trytes.ToArray(), depth, minWeightMagnitude);

            var successful = new bool[trxs.Length];

            for (var i = 0; i < trxs.Length; i++)
            {
                var response = FindTransactionsByBundles(trxs[i].Bundle);

                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        ///     Sends the trytes.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="minWeightMagnitude">Optional (default 14). The minimum weight magnitude.</param>
        /// <returns>an Array of Transactions</returns>
        public Transaction[] SendTrytes(string[] trytes, int depth, int minWeightMagnitude = 14)
        {
            var transactionsToApproveResponse = GetTransactionsToApprove(depth);

            var attachToTangleResponse =
                AttachToTangle(transactionsToApproveResponse.TrunkTransaction,
                    transactionsToApproveResponse.BranchTransaction, trytes, minWeightMagnitude);
            try
            {
                BroadcastAndStore(attachToTangleResponse.Trytes);
            }
            catch (System.Exception)
            {
                return new Transaction[0];
            }

            var trx = new List<Transaction>();

            foreach (var tx in attachToTangleResponse.Trytes) trx.Add(new Transaction(tx, _curl));
            return trx.ToArray();
        }

        /// <summary>
        ///     This function returns the bundle which is associated with a transaction. Input can by any type of transaction (tail
        ///     and non-tail).
        ///     If there are conflicting bundles (because of a replay for example) it will return multiple bundles.
        ///     It also does important validation checking (signatures, sum, order) to ensure that the correct bundle is returned.
        /// </summary>
        /// <param name="transaction">the transaction encoded in trytes</param>
        /// <returns>an array of bundle, if there are multiple arrays it means that there are conflicting bundles.</returns>
        public Bundle GetBundle(string transaction)
        {
            if (!InputValidator.IsHash(transaction))
            {
                throw new ArgumentException("Invalid hashes provided.");
            }

            var bundle = TraverseBundle(transaction, null, new Bundle());

            if (bundle == null)
                throw new ArgumentException("Unknown Bundle");

            long totalSum = 0;
            var bundleHash = bundle.Transactions[0].Bundle;

            ICurl curl = new Kerl();
            curl.Reset();

            var signaturesToValidate = new List<Signature>();

            for (var index = 0; index < bundle.Transactions.Count; index++)
            {
                var bundleTransaction = bundle.Transactions[index];
                var bundleValue = bundleTransaction.Value;
                totalSum += bundleValue;

                if (bundleTransaction.CurrentIndex != index)
                    throw new InvalidBundleException("The index of the bundle " + bundleTransaction.CurrentIndex +
                                                     " did not match the expected index " + index);

                // Get the transaction trytes
                var thisTxTrytes = bundleTransaction.ToTransactionTrytes().Substring(2187, 162);

                // Absorb bundle hash + value + timestamp + lastIndex + currentIndex trytes.
                curl.Absorb(Converter.ToTrits(thisTxTrytes));

                // Check if input transaction
                if (bundleValue < 0)
                {
                    var address = bundleTransaction.Address;
                    var sig = new Signature {Address = address};
                    sig.SignatureFragments.Add(bundleTransaction.SignatureMessageFragment);

                    // Find the subsequent txs with the remaining signature fragment
                    for (var i = index + 1; i < bundle.Transactions.Count; i++)
                    {
                        var newBundleTx = bundle.Transactions[i];

                        // Check if new tx is part of the signature fragment
                        if (newBundleTx.Address == address && newBundleTx.Value == 0)
                        {
                            if (sig.SignatureFragments.IndexOf(newBundleTx.SignatureMessageFragment) == -1)
                                sig.SignatureFragments.Add(newBundleTx.SignatureMessageFragment);
                        }

                    }

                    signaturesToValidate.Add(sig);
                }
            }

            // Check for total sum, if not equal 0 return error
            if (totalSum != 0)
                throw new InvalidBundleException("Invalid Bundle Sum");

            var bundleFromTrxs = new int[243];
            curl.Squeeze(bundleFromTrxs);
            var bundleFromTxString = Converter.ToTrytes(bundleFromTrxs);

            // Check if bundle hash is the same as returned by tx object
            if (!bundleFromTxString.Equals(bundleHash))
                throw new InvalidBundleException("Invalid Bundle Hash");
            // Last tx in the bundle should have currentIndex === lastIndex
            bundle.Length = bundle.Transactions.Count;
            if (
                !bundle.Transactions[bundle.Length - 1].CurrentIndex.Equals(
                    bundle.Transactions[bundle.Length - 1].LastIndex))
                throw new InvalidBundleException("Invalid Bundle");

            // Validate the signatures
            foreach (var aSignaturesToValidate in signaturesToValidate)
            {
                var signatureFragments = aSignaturesToValidate.SignatureFragments.ToArray();
                var address = aSignaturesToValidate.Address;
                var isValidSignature = new Signing().ValidateSignatures(address, signatureFragments, bundleHash);

                if (!isValidSignature)
                    throw new InvalidSignatureException();
            }

            return bundle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="index"></param>
        /// <param name="checksum"></param>
        /// <param name="total"></param>
        /// <param name="returnAll"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inclusionStates"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public AccountData GetAccountData(String seed, int security, int index, bool checksum, int total,
            bool returnAll, int start, int end, bool inclusionStates, long threshold)
        {

            if (start > end || end > (start + 1000))
            {
                throw new ArgumentException("Invalid input provided.");
            }

            var addresses = GetNewAddress(seed, security, index, checksum, total, returnAll);
            var bundle = GetTransfers(seed, security, start, end, inclusionStates);
            var inputs = GetInputs(seed, security, start, end, threshold);

            return new AccountData(new List<string>(addresses), bundle, inputs.InputsList, inputs.TotalBalance);
        }
        
        /// <summary>
        ///     Wrapper function that broadcasts and stores the specified trytes
        /// </summary>
        /// <param name="trytes">trytes</param>
        public void BroadcastAndStore(List<string> trytes)
        {
            BroadcastTransactions(trytes);
            StoreTransactions(trytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securitySum"></param>
        /// <param name="inputAddress"></param>
        /// <param name="remainderAddress"></param>
        /// <param name="transfers"></param>
        /// <param name="testMode"></param>
        /// <returns></returns>
        public List<Transaction> InitiateTransfer(
            int securitySum, string inputAddress, string remainderAddress,
            List<Transfer> transfers, bool testMode)
        {
            // validate input address
            if (!InputValidator.IsAddress(inputAddress))
                throw new ArgumentException("Invalid addresses provided.");

            // validate remainder address
            if (remainderAddress != null && !InputValidator.IsAddress(remainderAddress))
            {
                throw new ArgumentException("Invalid addresses provided.");
            }

            // Input validation of transfers object
            if (!InputValidator.IsTransfersCollectionValid(transfers))
            {
                throw new ArgumentException("Invalid transfers provided.");
            }

            // Create a new bundle
            Bundle bundle = new Bundle();

            long totalValue = 0;
            List<String> signatureFragments = new List<string>();
            String tag = "";
            //

            //  Iterate over all transfers, get totalValue
            //  and prepare the signatureFragments, message and tag
            foreach (Transfer transfer in transfers)
            {

                // remove the checksum of the address if provided
                if (transfer.Address.IsValidChecksum())
                {
                    transfer.Address = transfer.Address.RemoveChecksum();
                }

                int signatureMessageLength = 1;

                // If message longer than 2187 trytes, increase signatureMessageLength (add next transaction)
                if (transfer.Message.Length > Constants.MessageLength)
                {

                    // Get total length, message / maxLength (2187 trytes)
                    signatureMessageLength += (int)Math.Floor((double)transfer.Message.Length / Constants.MessageLength);

                    String msgCopy = transfer.Message;

                    // While there is still a message, copy it
                    
                    while (!string.IsNullOrEmpty(msgCopy))
                    {

                        string fragment = msgCopy.Substring(0, Constants.MessageLength);
                        msgCopy = msgCopy.Substring(Constants.MessageLength, msgCopy.Length - Constants.MessageLength);

                        // Pad remainder of fragment
                        fragment = fragment.PadRight(Constants.MessageLength, '9');
                        

                        signatureFragments.Add(fragment);
                    }

                }
                else
                {

                    // Else, get single fragment with 2187 of 9's trytes
                    String fragment = transfer.Message;

                    if (transfer.Message.Length < Constants.MessageLength)
                    {
                        fragment = fragment.PadRight(Constants.MessageLength, '9');
                    }

                    signatureFragments.Add(fragment);

                }

                tag = transfer.Tag;

                // pad for required 27 tryte length
                if (transfer.Tag.Length < Constants.TagLength)
                {
                    tag = tag.PadRight(Constants.TagLength, '9');
                }

                // get current timestamp in seconds
                long timestamp = (long)Math.Floor(GetCurrentTimestampInSeconds());

                // Add first entry to the bundle
                bundle.AddEntry(signatureMessageLength, transfer.Address, transfer.Value, tag, timestamp);
                // Sum up total value
                totalValue += transfer.Value;
            }

            // Get inputs if we are sending tokens
            if (totalValue != 0)
            {
                GetBalancesResponse balancesResponse = GetBalances(new List<string> { inputAddress }, 100);
                var balances = balancesResponse.Balances;

                long totalBalance = 0;
                
                foreach (var balance in balances)
                {
                    totalBalance += balance;
                }

                // get current timestamp in seconds
                long timestamp = (long)Math.Floor(GetCurrentTimestampInSeconds());

                // bypass the balance checks during unit testing
                if (testMode)
                    totalBalance += 1000;

                if (totalBalance > 0)
                {

                    long toSubtract = 0 - totalBalance;

                    // Add input as bundle entry
                    // Only a single entry, signatures will be added later
                    bundle.AddEntry(securitySum, inputAddress, toSubtract, tag, timestamp);
                }
                // Return not enough balance error
                if (totalValue > totalBalance)
                {
                    throw new IllegalStateException("Not enough balance.");
                }

                // If there is a remainder value
                // Add extra output to send remaining funds to
                if (totalBalance > totalValue)
                {

                    long remainder = totalBalance - totalValue;

                    // Remainder bundle entry if necessary
                    if (remainderAddress == null)
                    {
                        throw new IllegalStateException("No remainder address defined.");
                    }

                    bundle.AddEntry(1, remainderAddress, remainder, tag, timestamp);
                }

                bundle.FinalizeBundle(new Curl(CurlMode.CurlP81));
                bundle.AddTrytes(signatureFragments);

                return bundle.Transactions;
            }
            else
            {
                throw new System.Exception("Invalid value transfer: the transfer does not require a signature.");
            }
        }

        private Bundle TraverseBundle(string trunkTransaction, string bundleHash, Bundle bundle)
        {
            var gtr = GetTrytes(trunkTransaction);

            if (gtr.Trytes.Count == 0)
                throw new InvisibleBundleTransactionException();

            var trytes = gtr.Trytes[0];

            var transaction = new Transaction(trytes, _curl);

            // If first transaction to search is not a tail, return error
            if (bundleHash == null && transaction.CurrentIndex != 0) throw new InvalidTailTransactionException();

            // If no bundle hash, define it
            if (bundleHash == null) bundleHash = transaction.Bundle;

            // If different bundle hash, return with bundle
            if (bundleHash != transaction.Bundle) return bundle;

            // If only one bundle element, return
            if (transaction.LastIndex == 0 && transaction.CurrentIndex == 0)
                return new Bundle(new List<Transaction> { transaction }, 1);

            // Define new trunkTransaction for search
            var trunkTx = transaction.TrunkTransaction;

            // Add transaction object to bundle
            bundle.Transactions.Add(transaction);

            // Continue traversing with new trunkTx
            return TraverseBundle(trunkTx, bundleHash, bundle);
        }

        private double GetCurrentTimestampInSeconds()
        {
            DateTime now = DateTime.UtcNow;
            DateTime epoch = new DateTime
                (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (now - epoch).TotalSeconds;
        }
    }
    
}