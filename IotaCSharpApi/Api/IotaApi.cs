using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Exception;
using Iota.Lib.CSharp.Api.Model;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api
{
    /// <summary>
    /// This class provides access to the core API methods and the proposed calls
    /// </summary>
    public class IotaApi : IotaCoreApi
    {
        private ICurl curl;

        /// <summary>
        /// Creates an api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        public IotaApi(string host, int port) : this(host, port, new Curl())
        {
        }

        /// <summary>
        /// Creates an api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        /// <param name="curl">a custom curl implementation to be used to perform the pow. Use the other constructor in order to use the default curl implementation provided by the library </param>
        public IotaApi(string host, int port, ICurl curl) : base(host, port)
        {
            this.curl = curl ?? throw new ArgumentNullException(nameof(curl));
        }

        /// <summary>
        /// Gets all possible inputs of a seed and returns them with the total balance. 
        /// This is either done deterministically (by genearating all addresses until findTransactions is empty and doing getBalances),
        /// or by providing a key range to use for searching through.
        /// </summary>
        /// <param name="seed">tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="start">Starting key index</param>
        /// <param name="end">Ending key index</param>
        /// <param name="threshold">The minimum threshold of accumulated balances from the inputs that is required</param>
        /// <returns>The inputs (see <see cref="Input"/>) </returns>
        public Inputs GetInputs(string seed, int start, int end, long threshold = 100)
        {
            InputValidator.CheckIfValidSeed(seed);

            seed = InputValidator.PadSeedIfNecessary(seed);

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
                string[] allAddresses = new string[end - start];

                for (int i = start; i < end; i++)
                {
                    string address = IotaApiUtils.NewAddress(seed, i, false, curl);
                    allAddresses[i] = address;
                }

                return GetBalanceAndFormat(allAddresses, start, threshold);
            }

            //  Case 2: iterate till threshold || end
            //
            //  Either start from index: 0 or start (if defined) until threshold is reached.
            //  Calls getNewAddress and deterministically generates and returns all addresses
            //  We then do getBalance, format the output and return it
            else
            {
                string[] addresses = GetNewAddress(seed, start, true, 0, true);
                return GetBalanceAndFormat(addresses, start, threshold);
            }
        }

        /// <summary>
        /// Gets the balances of the specified addresses and calculates the total balance till the threshold is reached.
        /// </summary>
        /// <param name="addresses">addresses</param>
        /// <param name="start">start index</param>
        /// <param name="threshold">the threshold </param>
        /// <returns>an Inputs object</returns>
        /// <exception cref="NotEnoughBalanceException">is thrown if threshold exceeds the sum of balance of the specified addresses</exception>
        private Inputs GetBalanceAndFormat(string[] addresses, int start, long threshold = 100)
        {
            GetBalancesResponse getBalancesResponse = GetBalances(addresses.ToList(), 100);

            List<long> balances = getBalancesResponse.Balances;

            Inputs inputs = new Inputs() {InputsList = new List<Input>(), TotalBalance = 0};

            bool threshholdReached = false;

            for (int i = 0; i < addresses.Length; i++)
            {
                if (balances[i] > 0)
                {
                    inputs.InputsList.Add(new Input()
                    {
                        Address = addresses[i],
                        Balance = balances[i],
                        KeyIndex = start + i
                    });

                    inputs.TotalBalance += balances[i];

                    if (inputs.TotalBalance >= threshold)
                    {
                        threshholdReached = true;
                        break;
                    }
                }
            }

            if (threshholdReached)
            {
                return inputs;
            }

            throw new NotEnoughBalanceException();
        }

        /// <summary>
        /// Main purpose of this function is to get an array of transfer objects as input, and then prepare the transfer by generating the correct bundle,
        /// as well as choosing and signing the inputs if necessary (if it's a value transfer). The output of this function is an array of the raw transaction data (trytes)
        /// </summary>
        /// <param name="seed">81-tryte encoded address of recipient</param>
        /// <param name="transfers">the transfers to prepare</param>
        /// <param name="inputs">Optional (default null). The inputs</param>
        /// <param name="remainderAddress">Optional (default null). if defined, this address will be used for sending the remainder value (of the inputs) to.</param>
        /// <returns>a list containing the trytes of the new bundle</returns>
        public List<string> PrepareTransfers(string seed, Transfer[] transfers, List<Input> inputs = null,
            string remainderAddress = null)
        {
            InputValidator.CheckTransferArray(transfers);

            // Create a new bundle
            Bundle bundle = new Bundle();
            List<string> signatureFragments = new List<string>();
            long totalValue = 0;
            string tag = "";

            //
            //  Iterate over all transfers, get totalValue
            //  and prepare the signatureFragments, message and tag
            //
            foreach (Transfer t in transfers)
            {
                int signatureMessageLength = 1;

                // If message longer than 2187 trytes, increase signatureMessageLength (add 2nd transaction)
                if (t.Message.Length > 2187)
                {
                    // Get total length, message / maxLength (2187 trytes)
                    signatureMessageLength += (int) Math.Floor(((double) t.Message.Length/2187));

                    var msgCopy = t.Message;

                    // While there is still a message, copy it
                    while (msgCopy != null)
                    {
                        var fragment = msgCopy.Substring(0, 2187 > msgCopy.Length ? msgCopy.Length : 2187);
                        msgCopy = msgCopy.Substring(2187, msgCopy.Length - 2187);

                        // Pad remainder of fragment
                        for (var j = 0; fragment.Length < 2187; j++)
                        {
                            fragment += '9';
                        }

                        signatureFragments.Add(fragment);
                    }
                }
                else
                {
                    // Else, get single fragment with 2187 of 9's trytes
                    string fragment = "";

                    if (t.Message != null)
                    {
                        fragment = t.Message.Substring(0,
                            t.Message.Length < 2187 ? t.Message.Length : 2187);
                    }

                    for (var j = 0; fragment.Length < 2187; j++)
                    {
                        fragment += '9';
                    }

                    signatureFragments.Add(fragment);
                }

                // get current timestamp in seconds
                long timestamp = IotaApiUtils.CreateTimeStampNow();

                // If no tag defined, get 27 tryte tag.
                tag = t.Tag ?? "999999999999999999999999999";

                // Pad for required 27 tryte length
                for (var j = 0; tag.Length < 27; j++)
                {
                    tag += '9';
                }

                // Add first entries to the bundle
                // Slice the address in case the user provided a checksummed one
                bundle.AddEntry(signatureMessageLength, t.Address, t.Value, tag, timestamp);
                // Sum up total value
                totalValue += t.Value;
            }

            // Get inputs if we are sending tokens
            if (totalValue != 0)
            {
                //  Case 1: user provided inputs
                //
                //  Validate the inputs by calling getBalances
                if (inputs != null && inputs.Count > 0)
                {
                    // Get list if addresses of the provided inputs
                    List<string> inputAddresses = new List<string>();
                    foreach (var input in inputs)
                    {
                        inputAddresses.Add(input.Address);
                    }

                    GetBalancesResponse balances = GetBalances(inputAddresses, 100);

                    List<Input> confirmedInputs = new List<Input>();

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
                    if (totalValue > totalBalance)
                    {
                        throw new NotEnoughBalanceException(totalValue);
                    }

                    return AddRemainder(seed, confirmedInputs, bundle, tag, totalValue, remainderAddress,
                        signatureFragments);
                }

                //  Case 2: Get inputs deterministically
                //
                //  If no inputs provided, derive the addresses from the seed and
                //  confirm that the inputs exceed the threshold
                else
                {
                    List<Input> inputList = GetInputs(seed, 0, 0, (int) totalValue).InputsList;
                    return AddRemainder(seed, inputList, bundle, tag, totalValue, remainderAddress, signatureFragments);
                }
            }
            else
            {
                // If no input required, don't sign and simply finalize the bundle
                bundle.FinalizeBundle(curl);
                bundle.AddTrytes(signatureFragments);

                List<String> bundleTrytes = new List<string>();
                bundle.Transactions.ForEach(tx => bundleTrytes.Add(tx.ToTransactionTrytes()));

                bundleTrytes.Reverse();
                return bundleTrytes;
            }
        }

        private List<string> AddRemainder(string seed, List<Input> inputs, Bundle bundle, string tag, long totalValue,
            string remainderAddress, List<string> signatureFragments)
        {
            long totalTransferValue = totalValue;

            foreach (Input input in inputs)
            {
                var thisBalance = input.Balance;
                var toSubtract = 0 - thisBalance;
                var timestamp = IotaApiUtils.CreateTimeStampNow();

                // Add input as bundle entry
                bundle.AddEntry(2, input.Address, toSubtract, tag, timestamp);
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
                        IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, curl);
                    }
                    else if (remainder > 0)
                    {
                        // Generate a new Address by calling getNewAddress
                        string address = GetNewAddress(seed)[0];

                        // Remainder bundle entry
                        bundle.AddEntry(1, address, remainder, tag, timestamp);

                        // function for signing inputs
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, curl);
                    }
                    else
                    {
                        // If there is no remainder, do not add transaction to bundle
                        // simply sign and return
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments, curl);
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
        /// Generates a new address from a seed and returns the remainderAddress. This is either done deterministically, or by providing the index of the new remainderAddress 
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="index">Optional (default null). Key index to start search from. If the index is provided, the generation of the address is not deterministic.</param>
        /// <param name="checksum">Optional (default false). Adds 9-tryte address checksum</param>
        /// <param name="total">Optional (default 1)Total number of addresses to generate.</param>
        /// <param name="returnAll">If true, it returns all addresses which were deterministically generated (until findTransactions returns null)</param>
        /// <returns>an array of strings with the specifed number of addresses</returns>
        public string[] GetNewAddress(string seed, int index = 0, bool checksum = false, int total = 0,
            bool returnAll = false)
        {
            List<string> allAdresses = new List<string>();

            // TODO make two different functions out of this

            // Case 1: total
            //
            // If total number of addresses to generate is supplied, simply generate
            // and return the list of all addresses
            if (total > 0)
            {
                // Increase index with each iteration
                for (int i = index; i < index + total; i++)
                {
                    allAdresses.Add(IotaApiUtils.NewAddress(seed, i, checksum, curl));
                }

                return allAdresses.ToArray();
            }

            //  Case 2: no total provided
            //
            //  Continue calling findTransactions to see if address was already created
            //  if null, return list of addresses
            //
            else
            {
                List<string> addresses = new List<string>();

                for (int i = index;; i++)
                {
                    string newAddress = IotaApiUtils.NewAddress(seed, i, checksum, curl);
                    FindTransactionsResponse response = FindTransactionsByAddresses(newAddress);

                    if (returnAll)
                    {
                        addresses.Add(newAddress);
                    }

                    if (response.Hashes.Count == 0)
                        break;
                }

                return addresses.ToArray();
            }
        }

        /// <summary>
        /// Gets the transfers which are associated with a seed. 
        /// The transfers are determined by either calculating deterministically which addresses were already used, 
        /// or by providing a list of indexes to get the transfers from.
        /// </summary>
        /// <param name="seed">tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="inclusionStates">If True, it gets the inclusion states of the transfers.</param>
        /// <param name="start">the address start index</param>
        /// <param name="end">the address end index</param>
        /// <returns>An Array of Bundle object that represent the transfers</returns>
        public Bundle[] GetTransfers(string seed, int? start, int? end, bool inclusionStates = false)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            if (!start.HasValue)
                start = 0;
            if (!end.HasValue)
                end = 0;

            // If start value bigger than end, return error
            // or if difference between end and start is bigger than 500 keys
            if (start.Value > end.Value || end.Value > (start + 500))
            {
                throw new System.Exception("Invalid inputs provided: start, end");
            }

            // first call findTransactions
            // If a transaction is non tail, get the tail transactions associated with it
            // add it to the list of tail transactions

            string[] addresses = GetNewAddress(seed, start.Value, false, end.Value, true);
            Bundle[] bundles = BundlesFromAddresses(addresses, inclusionStates);

            return bundles;
        }

        private Bundle[] BundlesFromAddresses(string[] addresses, bool inclusionStates)
        {
            List<Transaction> trxs = FindTransactionObjects(addresses);
            // set of tail transactions
            List<string> tailTransactions = new List<string>();
            List<string> nonTailBundleHashes = new List<string>();

            foreach (Transaction trx in trxs)
            {
                // Sort tail and nonTails
                if (long.Parse(trx.CurrentIndex) == 0)
                {
                    tailTransactions.Add(trx.Hash);
                }
                else
                {
                    if (nonTailBundleHashes.IndexOf(trx.Bundle) == -1)
                    {
                        nonTailBundleHashes.Add(trx.Bundle);
                    }
                }
            }

            List<Transaction> bundleObjects = FindTransactionObjectsByBundle(nonTailBundleHashes.ToArray());
            foreach (Transaction trx in bundleObjects)
            {
                // Sort tail and nonTails
                if (long.Parse(trx.CurrentIndex) == 0)
                {
                    if (tailTransactions.IndexOf(trx.Hash) == -1)
                    {
                        tailTransactions.Add(trx.Hash);
                    }
                }
            }

            List<Bundle> finalBundles = new List<Bundle>();
            string[] tailTxArray = tailTransactions.ToArray();

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

                if (gisr?.States == null || gisr.States.Count == 0)
                    throw new ArgumentException("Inclusion states not found");
            }
            
            GetInclusionStatesResponse finalInclusionStates = gisr;

            Parallel.ForEach(tailTransactions, param =>
            {
                try
                {
                    Bundle b = GetBundle(param);

                    if (inclusionStates)
                    {
                        bool thisInclusion = finalInclusionStates != null && finalInclusionStates.States[tailTxArray.ToList().IndexOf(param)];

                        foreach (Transaction t in b.Transactions)
                        {
                            t.Persistance = thisInclusion;
                        }
                    }

                    finalBundles.Add(b);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Bundle error: " + ex.Message);
                }
            });

            finalBundles.Sort();
            Bundle[] returnValue = new Bundle[finalBundles.Count];

            for (int i = 0; i < finalBundles.Count; i++)
            {
                returnValue[i] = new Bundle(finalBundles[i].Transactions, finalBundles[i].Transactions.Count);
            }

            return returnValue;
        }

        /// <summary>
        /// Finds the transaction objects.
        /// </summary>
        /// <param name="adresses">The adresses.</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> FindTransactionObjects(string[] adresses)
        {
            FindTransactionsResponse ftr = FindTransactions(adresses.ToList(), null, null, null);
            if (ftr?.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// Gets the transactions objects.
        /// </summary>
        /// <param name="hashes">The hashes in trytes</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> GetTransactionsObjects(string[] hashes)
        {
            if (!InputValidator.IsArrayOfHashes(hashes))
            {
                throw new IllegalStateException("Not an Array of Hashes: " + hashes.ToString());
            }

            GetTrytesResponse trytesResponse = GetTrytes(hashes);

            List<Transaction> trxs = new List<Transaction>();

            foreach (string tryte in trytesResponse.Trytes)
            {
                trxs.Add(new Transaction(tryte, curl));
            }

            return trxs;
        }

        /// <summary>
        /// Finds the transaction objects by bundle.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a list of Transaction objects</returns>
        public List<Transaction> FindTransactionObjectsByBundle(string[] bundles)
        {
            FindTransactionsResponse ftr = FindTransactions(null, null, null, bundles.ToList());
            if (ftr == null || ftr.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// Replays the bundle.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude.</param>
        /// <returns>an array of boolean that indicate which transactions have been replayed successfully</returns>
        public bool[] ReplayBundle(string transaction, int depth, int minWeightMagnitude)
        {
            List<string> bundleTrytes = new List<string>();

            Bundle bundle = GetBundle(transaction);

            bundle.Transactions.ForEach((t) => bundleTrytes.Add(t.ToTransactionTrytes()));

            List<Transaction> trxs = SendTrytes(bundleTrytes.ToArray(), depth, minWeightMagnitude).ToList();

            bool[] successful = new bool[trxs.Count];

            for (int i = 0; i < trxs.Count; i++)
            {
                FindTransactionsResponse response = FindTransactionsByBundles(trxs[i].Bundle);
                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        /// Finds the transactions by bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByBundles(params string[] bundles)
        {
            return FindTransactions(null, null, null, bundles.ToList());
        }

        /// <summary>
        /// Finds the transactions by approvees.
        /// </summary>
        /// <param name="approvees">The approvees.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByApprovees(params string[] approvees)
        {
            return FindTransactions(null, null, approvees.ToList(), null);
        }

        /// <summary>
        /// Finds the transactions by digests.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByDigests(params string[] bundles)
        {
            return FindTransactions(null, bundles.ToList(), null, null);
        }

        /// <summary>
        /// Finds the transactions by addresses.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByAddresses(params string[] addresses)
        {
            return FindTransactions(addresses.ToList(), null, null, null);
        }

        /// <summary>
        /// Gets the latest inclusion.
        /// </summary>
        /// <param name="hashes">The hashes.</param>
        /// <returns>a GetInclusionStatesResponse cotaining the inclusion state of the specified hashes</returns>
        public GetInclusionStatesResponse GetLatestInclusion(string[] hashes)
        {
            string[] latestMilestone = { GetNodeInfo().LatestSolidSubtangleMilestone };
            return GetInclusionStates(hashes, latestMilestone);
        }

        /// <summary>
        /// Wrapper function that basically does prepareTransfers, as well as attachToTangle and finally, it broadcasts and stores the transactions locally.
        /// </summary>
        /// <param name="seed">tryte-encoded seed</param>
        /// <param name="depth">depth</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude</param>
        /// <param name="transfers">Array of transfer objects</param>
        /// <param name="inputs">Optional (default null). List of inputs used for funding the transfer</param>
        /// <param name="address">Optional (default null). If defined, this address will be used for sending the remainder value (of the inputs) to</param>
        /// <returns> an array of the boolean that indicates which Transactions where sent successully</returns>
        public bool[] SendTransfer(string seed, int depth, int minWeightMagnitude, Transfer[] transfers,
            Input[] inputs = null, string address = null)
        {
            List<string> trytes = PrepareTransfers(seed, transfers, inputs?.ToList(), address);
            Transaction[] trxs = SendTrytes(trytes.ToArray(), depth, minWeightMagnitude);

            bool[] successful = new bool[trxs.Length];

            for (int i = 0; i < trxs.Length; i++)
            {
                FindTransactionsResponse response = FindTransactionsByBundles(trxs[i].Bundle);
                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        /// Sends the trytes.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="minWeightMagnitude">Optional (default 18). The minimum weight magnitude.</param>
        /// <returns>an Array of Transactions</returns>
        public Transaction[] SendTrytes(string[] trytes, int depth, int minWeightMagnitude = 18)
        {
            GetTransactionsToApproveResponse transactionsToApproveResponse = GetTransactionsToApprove(depth);

            AttachToTangleResponse attachToTangleResponse =
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

            List<Transaction> trx = new List<Transaction>();

            foreach (string tx in attachToTangleResponse.Trytes)
            {
                trx.Add(new Transaction(tx, curl));
            }

            return trx.ToArray();
        }

        /// <summary>
        /// This function returns the bundle which is associated with a transaction. Input can by any type of transaction (tail and non-tail). 
        /// If there are conflicting bundles (because of a replay for example) it will return multiple bundles. 
        /// It also does important validation checking (signatures, sum, order) to ensure that the correct bundle is returned.
        /// </summary>
        /// <param name="transaction">the transaction encoded in trytes</param>
        /// <returns>an array of bundle, if there are multiple arrays it means that there are conflicting bundles.</returns>
        public Bundle GetBundle(string transaction)
        {
            Bundle bundle = TraverseBundle(transaction, null, new Bundle());

            if (bundle == null)
                throw new ArgumentException("Unknown Bundle");

            long totalSum = 0;
            string bundleHash = bundle.Transactions[0].Bundle;

            curl.Reset();

            List<Signature> signaturesToValidate = new List<Signature>();

            for (int index = 0; index < bundle.Transactions.Count; index++)
            {
                Transaction bundleTransaction = bundle.Transactions[index];
                long bundleValue = long.Parse(bundleTransaction.Value);
                totalSum += bundleValue;

                if (long.Parse(bundleTransaction.CurrentIndex) != index)
                    throw new InvalidBundleException("The index of the bundle " + bundleTransaction.CurrentIndex + " did not match the expected index " + index);

                // Get the transaction trytes
                string thisTxTrytes = bundleTransaction.ToTransactionTrytes().Substring(2187, 162);

                // Absorb bundle hash + value + timestamp + lastIndex + currentIndex trytes.
                curl.Absorb(Converter.ToTrits(thisTxTrytes));

                // Check if input transaction
                if (bundleValue < 0)
                {
                    string address = bundleTransaction.Address;
                    Signature sig = new Signature();
                    sig.Address = address;
                    sig.SignatureFragments.Add(bundleTransaction.SignatureFragment);

                    // Find the subsequent txs with the remaining signature fragment
                    for (int i = index; i < bundle.Length - 1; i++)
                    {
                        var newBundleTx = bundle[i + 1];

                        // Check if new tx is part of the signature fragment
                        if (newBundleTx.Address == address && long.Parse(newBundleTx.Value) == 0)
                        {
                            sig.SignatureFragments.Add(newBundleTx.SignatureFragment);
                        }
                    }

                    signaturesToValidate.Add(sig);
                }
            }

            // Check for total sum, if not equal 0 return error
            if (totalSum != 0)
                throw new InvalidBundleException("Invalid Bundle Sum");

            int[] bundleFromTrxs = new int[243];
            curl.Squeeze(bundleFromTrxs);
            string bundleFromTxString = Converter.ToTrytes(bundleFromTrxs);

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
            foreach (Signature aSignaturesToValidate in signaturesToValidate)
            {
                String[] signatureFragments = aSignaturesToValidate.SignatureFragments.ToArray();
                string address = aSignaturesToValidate.Address;
                bool isValidSignature = new Signing().ValidateSignatures(address, signatureFragments, bundleHash);

                if (!isValidSignature)
                    throw new InvalidSignatureException();
            }

            return bundle;
        }

        private Bundle TraverseBundle(string trunkTransaction, string bundleHash, Bundle bundle)
        {
            GetTrytesResponse gtr = GetTrytes(trunkTransaction);

            if (gtr.Trytes.Count == 0)
                throw new InvisibleBundleTransactionException();

            string trytes = gtr.Trytes[0];

            Transaction transaction = new Transaction(trytes, curl);

            // If first transaction to search is not a tail, return error
            if (bundleHash == null && transaction.Index != 0)
            {
                throw new InvalidTailTransactionException();
            }

            // If no bundle hash, define it
            if (bundleHash == null)
            {
                bundleHash = transaction.Bundle;
            }

            // If different bundle hash, return with bundle
            if (bundleHash != transaction.Bundle)
            {
                return bundle;
            }

            // If only one bundle element, return
            if (long.Parse(transaction.LastIndex) == 0 && long.Parse(transaction.CurrentIndex) == 0)
            {
                return new Bundle(new List<Transaction>() {transaction}, 1);
            }

            // Define new trunkTransaction for search
            var trunkTx = transaction.TrunkTransaction;

            // Add transaction object to bundle
            bundle.Transactions.Add(transaction);

            // Continue traversing with new trunkTx
            return TraverseBundle(trunkTx, bundleHash, bundle);
        }

        /// <summary>
        /// Wrapper function that broadcasts and stores the specified trytes
        /// </summary>
        /// <param name="trytes">trytes</param>
        public void BroadcastAndStore(List<string> trytes)
        {
            BroadcastTransactions(trytes);
            StoreTransactions(trytes);
        }
    }
}