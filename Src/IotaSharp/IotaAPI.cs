using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IotaSharp.Core;
using IotaSharp.Exception;
using IotaSharp.Model;
using IotaSharp.Pow;
using IotaSharp.Utils;
using NLog;

namespace IotaSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaAPI
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        public IotaClient IotaClient { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PearlDiverLocalPoW LocalPoW { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="depth"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="transfers"></param>
        /// <param name="inputs"></param>
        /// <param name="remainderAddress"></param>
        /// <param name="validateInputs"></param>
        /// <param name="validateInputAddresses"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        public SendTransferResponse SendTransfer(
            string seed, int security,
            int depth, int minWeightMagnitude,
            Transfer[] transfers, Input[] inputs,
            string remainderAddress,
            bool validateInputs, bool validateInputAddresses,
            Transaction[] tips)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var trytes = PrepareTransfers(seed, security, transfers,
                remainderAddress, inputs, tips, validateInputs);

            if (validateInputAddresses)
                ValidateTransfersAddresses(seed, security, trytes);

            string reference = tips != null && tips.Length > 0 ? tips[0].CurlHash() : null;

            var transactions = SendTrytes(trytes.ToArray(), depth, minWeightMagnitude, reference);

            var successful = new bool[transactions.Length];

            for (var i = 0; i < transactions.Length; i++)
            {
                var response = IotaClient.FindTransactionsByBundles(transactions[i].Bundle);

                successful[i] = response.Hashes.Count != 0;
            }

            stopwatch.Stop();

            return new SendTransferResponse(transactions, successful, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="transfers"></param>
        /// <param name="remainder"></param>
        /// <param name="inputs"></param>
        /// <param name="tips"></param>
        /// <param name="validateInputs"></param>
        /// <returns></returns>
        public List<string> PrepareTransfers(
            string seed, int security,
            Transfer[] transfers,
            string remainder,
            Input[] inputs,
            Transaction[] tips,
            bool validateInputs)
        {
            // validate seed
            if (!InputValidator.IsValidSeed(seed))
                throw new IllegalStateException(Constants.INVALID_SEED_INPUT_ERROR);


            if (!InputValidator.IsValidSecurityLevel(security))
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);

            if (remainder != null && !InputValidator.IsAddress(remainder))
            {
                throw new ArgumentException(Constants.INVALID_ADDRESSES_INPUT_ERROR);
            }

            // Input validation of transfers object
            if (!InputValidator.IsValidTransfersCollection(transfers.ToList()))
            {
                throw new ArgumentException(Constants.INVALID_TRANSFERS_INPUT_ERROR);
            }

            if (inputs != null && !InputValidator.IsValidInputsCollection(inputs))
            {
                throw new ArgumentException(Constants.INVALID_ADDRESSES_INPUT_ERROR);
            }

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
                if (transfer.Message.Length > Constants.MESSAGE_LENGTH)
                {
                    // Get total length, message / maxLength (2187 trytes)
                    signatureMessageLength +=
                        (int) Math.Floor((double) transfer.Message.Length / Constants.MESSAGE_LENGTH);

                    var msgCopy = transfer.Message;

                    // While there is still a message, copy it
                    while (!string.IsNullOrEmpty(msgCopy))
                    {
                        var fragment = msgCopy.Substring(0, 2187 > msgCopy.Length ? msgCopy.Length : 2187);
                        msgCopy = msgCopy.Substring(fragment.Length, msgCopy.Length - fragment.Length);

                        // Pad remainder of fragment
                        if (fragment.Length < 2187)
                            fragment = fragment.PadRight(2187, '9');
                       
                        signatureFragments.Add(fragment);
                    }
                }
                else
                {
                    // Else, get single fragment with 2187 of 9's trytes
                    var fragment = transfer.Message.PadRight(Constants.MESSAGE_LENGTH, '9');

                    signatureFragments.Add(fragment);
                }

                // get current timestamp in seconds
                var timestamp = (long) Math.Floor((double) TimeStamp.Now() / 1000);

                // If no tag defined, get 27 tryte tag.

                tag = string.IsNullOrEmpty(transfer.Tag) ? "999999999999999999999999999" : transfer.Tag;

                // Pad for required 27 tryte length
                if (tag.Length < Constants.TAG_LENGTH)
                    tag = tag.PadRight(Constants.TAG_LENGTH, '9');

                // Add first entries to the bundle
                // Slice the address in case the user provided a checksummed one
                bundle.AddEntry(signatureMessageLength, transfer.Address, transfer.Value, tag, timestamp);
                // Sum up total value
                totalValue += transfer.Value;
            }

            // Get inputs if we are sending tokens
            if (totalValue != 0)
            {
                // validate seed
                if (!InputValidator.IsValidSeed(seed))
                    throw new IllegalStateException(Constants.INVALID_SEED_INPUT_ERROR);


                //  Case 1: user provided inputs
                //  Validate the inputs by calling getBalances
                if (inputs != null && inputs.Length > 0)
                {
                    if (!validateInputs)
                    {
                        return AddRemainder(seed, security, inputs.ToList(), bundle, tag, totalValue, remainder,
                            signatureFragments);
                    }

                    // Get list if addresses of the provided inputs
                    var inputAddresses = new List<string>();
                    foreach (var input in inputs) inputAddresses.Add(input.Address);

                    List<string> tipHashes = null;
                    if (tips != null)
                    {
                        tipHashes = new List<string>();
                        foreach (var tx in tips)
                        {
                            tipHashes.Add(tx.CurlHash());
                        }
                    }

                    var balances = IotaClient.GetBalances(100, inputAddresses, tipHashes);

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

                            // if we've already reached the intended input value, break out of loop
                            if (totalBalance >= totalValue)
                            {
                                Log.Info("Total balance already reached ");
                                break;
                            }
                        }
                    }

                    // Return not enough balance error
                    if (totalValue > totalBalance) throw new IllegalStateException(Constants.NOT_ENOUGH_BALANCE_ERROR);

                    return AddRemainder(seed, security, confirmedInputs, bundle, tag, totalValue, remainder,
                        signatureFragments);
                }

                //  Case 2: Get inputs deterministically
                //
                //  If no inputs provided, derive the addresses from the seed and
                //  confirm that the inputs exceed the threshold
                var inputList = GetInputs(seed, security, 0, 0, (int) totalValue).Inputs;
                return AddRemainder(seed, security, inputList, bundle, tag, totalValue, remainder,
                    signatureFragments);
            }

            // If no input required, don't sign and simply finalize the bundle
            bundle.FinalizeBundle(SpongeFactory.Create(SpongeFactory.Mode.KERL));
            bundle.AddTrytes(signatureFragments);

            var bundleTrytes = new List<string>();
            bundle.Transactions.ForEach(tx => bundleTrytes.Add(tx.ToTrytes()));

            bundleTrytes.Reverse();
            return bundleTrytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="depth"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public Transaction[] SendTrytes(string[] trytes, int depth, int minWeightMagnitude, string reference)
        {
            var transactionsToApproveResponse = IotaClient.GetTransactionsToApprove(depth, reference);

            // attach to tangle - do pow
            AttachToTangleResponse attachToTangleResponse;
            if (LocalPoW == null)
            {
                attachToTangleResponse =
                    IotaClient.AttachToTangle(transactionsToApproveResponse.TrunkTransaction,
                        transactionsToApproveResponse.BranchTransaction, minWeightMagnitude, trytes);
            }
            else
            {
                attachToTangleResponse =
                    LocalPoW.AttachToTangle(transactionsToApproveResponse.TrunkTransaction,
                        transactionsToApproveResponse.BranchTransaction, minWeightMagnitude, trytes);
            }

            try
            {
                BroadcastAndStore(attachToTangleResponse.Trytes.ToArray());
            }
            catch (System.Exception)
            {
                return Array.Empty<Transaction>();
            }

            var trx = new List<Transaction>();

            foreach (var tx in attachToTangleResponse.Trytes) trx.Add(new Transaction(tx));
            return trx.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="threshold"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        public GetBalancesAndFormatResponse GetInputs(string seed, int security, int start, int end, long threshold,
            params string[] tips)
        {
            // validate the seed
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new IllegalStateException(Constants.INVALID_SEED_INPUT_ERROR);
            }

            seed = InputValidator.PadSeedIfNecessary(seed);

            if (!InputValidator.IsValidSecurityLevel(security))
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);

            // If start value bigger than end, return error
            if (start > end)
                throw new ArgumentException("start must be smaller than end", nameof(start));

            // or if difference between end and start is bigger than 500 keys
            if (end - start > 500)
                throw new ArgumentException("total number of keys exceeded 500");


            Stopwatch stopwatch = Stopwatch.StartNew();

            //  Case 1: start and end
            //
            //  If start and end is defined by the user, simply iterate through the keys
            //  and call getBalances
            if (end != 0)
            {
                var allAddresses = new string[end - start];

                for (var i = start; i < end; i++)
                {
                    var address = IotaApiUtils.NewAddress(seed, security, i, true,
                        SpongeFactory.Create(SpongeFactory.Mode.KERL));
                    allAddresses[i] = address;
                }

                return GetBalanceAndFormat(allAddresses, tips, threshold, start, security, stopwatch);
            }

            {
                //  Case 2: iterate till threshold 
                //
                //  Either start from index: 0 or start (if defined) until threshold is reached.
                List<Input> allInputs = new List<Input>();

                bool thresholdReached = true;
                long currentTotal = 0;

                for (int i = start; thresholdReached; i++)
                {

                    string address = IotaApiUtils.NewAddress(seed, security, i, true,
                        SpongeFactory.Create(SpongeFactory.Mode.KERL));

                    // Received input, this epoch or previous
                    GetBalancesResponse response =
                        IotaClient.GetBalances(100, new List<string>() {address}, tips.ToList());
                    var balance = response.Balances[0];

                    if (balance > 0)
                    {
                        // Is it already spent from?
                        WereAddressesSpentFromResponse wasSpent = IotaClient.WereAddressesSpentFrom(address);
                        if (wasSpent.States.Length > 0 && !wasSpent.States[0])
                        {
                            // We can use this!
                            allInputs.Add(new Input
                            {
                                Address = address, Balance = balance, KeyIndex = i, Security = security
                            });
                            currentTotal += balance;

                            if (threshold != 0 && threshold <= currentTotal)
                            {
                                // Stop because we found threshold
                                thresholdReached = false;
                            }
                        }
                    }
                    else
                    {
                        // Check if there was any activity at all
                        FindTransactionsResponse tx = IotaClient.FindTransactionsByAddresses(address);
                        if (tx.Hashes.Count == 0 || i - start > 500)
                        {
                            // Stop because we reached our limit or no activity
                            thresholdReached = false;
                        }
                    }

                }

                stopwatch.Stop();
                return new GetBalancesAndFormatResponse(allInputs, currentTotal, stopwatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        public void BroadcastAndStore(params string[] trytes)
        {
            IotaClient.BroadcastTransactions(trytes);
            IotaClient.StoreTransactions(trytes);
        }

        private void ValidateTransfersAddresses(string seed, int security, List<string> trytes)
        {
            HashSet<string> addresses = new HashSet<string>();
            List<Transaction> inputTransactions = new List<Transaction>();
            List<string> inputAddresses = new List<string>();


            foreach (var trx in trytes)
            {
                addresses.Add(new Transaction(trx).Address);
                inputTransactions.Add(new Transaction(trx));
            }

            var hashes = IotaClient.FindTransactionsByAddresses(addresses.ToArray()).Hashes;
            List<Transaction> transactions = FindTransactionObjectsByHashes(hashes.ToArray());

            // Get addresses until first unspent
            var addressRequest = new AddressRequest(seed, security) {Amount = 0, Checksum = true};
            var gna = GenerateNewAddresses(addressRequest);

            // Get inputs for this seed, until we fund an unused address
            var gbr = GetInputs(seed, security, 0, 0, 0);

            foreach (var input in gbr.Inputs)
            {
                inputAddresses.Add(input.Address);
            }

            //check if send to input
            foreach (var trx in inputTransactions)
            {
                if (trx.Value > 0 && inputAddresses.Contains(trx.Address))
                    throw new ArgumentException("Send to inputs!");
            }

            foreach (var trx in transactions)
            {
                //check if destination address is already in use
                if (trx.Value < 0 && !inputAddresses.Contains(trx.Address))
                {
                    throw new ArgumentException("Sending to a used address.");
                }

                //check if key reuse
                if (trx.Value < 0 && gna.Addresses.Contains(trx.Address))
                {
                    throw new ArgumentException("Private key reuse detect!");
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public List<Transaction> FindTransactionObjectsByHashes(string[] hashes)
        {
            if (!InputValidator.IsArrayOfHashes(hashes))
                throw new IllegalStateException("Not an Array of Hashes: " + hashes);

            var trytesResponse = IotaClient.GetTrytes(hashes);

            var trxs = new List<Transaction>();

            foreach (var tryte in trytesResponse.Trytes) trxs.Add(new Transaction(tryte));
            return trxs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securitySum"></param>
        /// <param name="inputAddress"></param>
        /// <param name="remainderAddress"></param>
        /// <param name="transfers"></param>
        /// <param name="tips"></param>
        /// <param name="testMode"></param>
        /// <returns></returns>
        public List<Transaction> InitiateTransfer(
            int securitySum, string inputAddress, string remainderAddress,
            List<Transfer> transfers, List<Transaction> tips, bool testMode)
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
            if (!InputValidator.IsValidTransfersCollection(transfers))
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
                if (transfer.Message.Length > Constants.MESSAGE_LENGTH)
                {

                    // Get total length, message / maxLength (2187 trytes)
                    signatureMessageLength +=
                        (int) Math.Floor((double) transfer.Message.Length / Constants.MESSAGE_LENGTH);

                    String msgCopy = transfer.Message;

                    // While there is still a message, copy it

                    while (!string.IsNullOrEmpty(msgCopy))
                    {

                        string fragment = msgCopy.Substring(0, Constants.MESSAGE_LENGTH);
                        msgCopy = msgCopy.Substring(Constants.MESSAGE_LENGTH,
                            msgCopy.Length - Constants.MESSAGE_LENGTH);

                        // Pad remainder of fragment
                        fragment = fragment.PadRight(Constants.MESSAGE_LENGTH, '9');


                        signatureFragments.Add(fragment);
                    }

                }
                else
                {

                    // Else, get single fragment with 2187 of 9's trytes
                    String fragment = transfer.Message;

                    if (transfer.Message.Length < Constants.MESSAGE_LENGTH)
                    {
                        fragment = fragment.PadRight(Constants.MESSAGE_LENGTH, '9');
                    }

                    signatureFragments.Add(fragment);

                }

                tag = transfer.Tag;

                // pad for required 27 tryte length
                if (transfer.Tag.Length < Constants.TAG_LENGTH)
                {
                    tag = tag.PadRight(Constants.TAG_LENGTH, '9');
                }

                // get current timestamp in seconds
                long timestamp = (long) Math.Floor(GetCurrentTimestampInSeconds());

                // Add first entry to the bundle
                bundle.AddEntry(signatureMessageLength, transfer.Address, transfer.Value, tag, timestamp);
                // Sum up total value
                totalValue += transfer.Value;
            }

            // Get inputs if we are sending tokens
            if (totalValue != 0)
            {
                List<string> tipHashes = null;
                if (tips != null)
                {
                    tipHashes = new List<string>();
                    foreach (var tx in tips)
                    {
                        tipHashes.Add(tx.CurlHash());
                    }
                }


                GetBalancesResponse balancesResponse =
                    IotaClient.GetBalances(100, new List<string> {inputAddress}, tipHashes);
                var balances = balancesResponse.Balances;

                long totalBalance = 0;

                foreach (var balance in balances)
                {
                    totalBalance += balance;
                }

                // get current timestamp in seconds
                long timestamp = (long) Math.Floor(GetCurrentTimestampInSeconds());

                // bypass the balance checks during unit testing
                //TODO remove this ugliness
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

                bundle.FinalizeBundle(SpongeFactory.Create(SpongeFactory.Mode.CURLP81));
                bundle.AddTrytes(signatureFragments);

                return bundle.Transactions;
            }
            else
            {
                throw new System.Exception("Invalid value transfer: the transfer does not require a signature.");
            }
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
                var timestamp = TimeStamp.Now();

                // Add input as bundle entry
                // use input.Security
                bundle.AddEntry(input.Security, input.Address, toSubtract, tag, timestamp);
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
                        return IotaApiUtils.SignInputsAndReturn(
                            seed, inputs, bundle, signatureFragments,
                            SpongeFactory.Create(SpongeFactory.Mode.KERL));
                    }

                    if (remainder > 0)
                    {
                        AddressRequest addressRequest = new AddressRequest(seed, security);
                        var res = GenerateNewAddresses(addressRequest);

                        // Remainder bundle entry
                        bundle.AddEntry(1, res.Addresses[0], remainder, tag, timestamp);

                        // function for signing inputs
                        return IotaApiUtils.SignInputsAndReturn(
                            seed, inputs, bundle, signatureFragments,
                            SpongeFactory.Create(SpongeFactory.Mode.KERL));
                    }

                    // If there is no remainder, do not add transaction to bundle
                    // simply sign and return
                    return IotaApiUtils.SignInputsAndReturn(
                        seed, inputs, bundle, signatureFragments,
                        SpongeFactory.Create(SpongeFactory.Mode.KERL));
                }

                // If multiple inputs provided, subtract the totalTransferValue by
                // the inputs balance
                totalTransferValue -= thisBalance;
            }

            throw new IllegalStateException(Constants.NOT_ENOUGH_BALANCE_ERROR);
        }

        private GetBalancesAndFormatResponse GetBalanceAndFormat(
            string[] addresses,
            string[] tips,
            long threshold,
            int start,
            int security,
            Stopwatch stopwatch = null)
        {
            if (stopwatch == null)
                stopwatch = Stopwatch.StartNew();

            if (!InputValidator.IsValidSecurityLevel(security))
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);

            var getBalancesResponse = IotaClient.GetBalances(100, addresses.ToList(), tips.ToList());

            var balances = getBalancesResponse.Balances;

            // If threshold defined, keep track of whether reached or not
            // else set default to true

            var inputList = new List<Input>();
            long totalBalance = 0;

            var thresholdReached = threshold == 0;

            for (var i = 0; i < addresses.Length; i++)
                if (balances[i] > 0)
                {
                    inputList.Add(new Input
                    {
                        Address = addresses[i],
                        Balance = balances[i],
                        KeyIndex = start + i,
                        Security = security
                    });

                    totalBalance += balances[i];

                    if (totalBalance >= threshold)
                    {
                        thresholdReached = true;
                        break;
                    }
                }

            stopwatch.Stop();
            if (thresholdReached)
                return new GetBalancesAndFormatResponse(inputList, totalBalance, stopwatch.ElapsedMilliseconds);

            throw new IllegalStateException(Constants.NOT_ENOUGH_BALANCE_ERROR);
        }

        private double GetCurrentTimestampInSeconds()
        {
            DateTime now = DateTime.UtcNow;
            DateTime epoch = new DateTime
                (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (now - epoch).TotalSeconds;
        }

        /// <summary>
        /// This does not mean that these addresses are safe to use (unspent)
        /// </summary>
        /// <param name="addressRequest"></param>
        /// <returns></returns>
        public GetNewAddressResponse GetAddressesUnchecked(AddressRequest addressRequest)
        {
            var stopwatch = Stopwatch.StartNew();

            List<string> addresses = new List<string>();

            for (int i = 0; i < addressRequest.Amount; i++)
                addresses.Add(
                    IotaApiUtils.NewAddress(
                        addressRequest.Seed,
                        addressRequest.SecurityLevel, i,
                        addressRequest.Checksum,
                        SpongeFactory.Create(SpongeFactory.Mode.KERL)));

            stopwatch.Stop();

            return new GetNewAddressResponse
            {
                Addresses = addresses,
                Duration = stopwatch.ElapsedMilliseconds
            };
        }

        /// <summary>
        /// Generates new addresses, meaning addresses which were not spend from, according to the connected node.
        /// </summary>
        /// <param name="addressRequest"></param>
        /// <returns></returns>
        public GetNewAddressResponse GenerateNewAddresses(AddressRequest addressRequest)
        {
            var stopwatch = Stopwatch.StartNew();

            List<string> addresses;

            if (addressRequest.Amount == 0)
            {
                string unusedAddress = GetFirstUnusedAddress(
                    addressRequest.Seed, addressRequest.SecurityLevel,
                    addressRequest.Index, addressRequest.Checksum);

                addresses = new List<string>
                {
                    unusedAddress
                };

            }
            else
            {
                addresses = GetAddresses(
                    addressRequest.Seed, addressRequest.SecurityLevel,
                    addressRequest.Index, addressRequest.Checksum,
                    addressRequest.Amount, addressRequest.AddSpendAddresses);
            }

            stopwatch.Stop();
            return new GetNewAddressResponse
            {
                Addresses = addresses,
                Duration = stopwatch.ElapsedMilliseconds
            };
        }

        private List<string> GetAddresses(string seed, int securityLevel, int index, bool checksum, int amount,
            bool addSpendAddresses)
        {
            List<string> addresses = new List<string>();

            for (int i = index, numUnspentFound = 0; numUnspentFound < amount; i++)
            {
                string newAddress = IotaApiUtils.NewAddress(seed, securityLevel, i, checksum,
                    SpongeFactory.Create(SpongeFactory.Mode.KERL));

                if (!IsAddressSpent(newAddress, checksum))
                {
                    addresses.Add(newAddress);
                    numUnspentFound++;
                }
                else if (addSpendAddresses)
                {
                    addresses.Add(newAddress);
                }
            }

            return addresses;
        }

        private string GetFirstUnusedAddress(string seed, int securityLevel, int index, bool checksum)
        {
            while (true)
            {
                string newAddress =
                    IotaApiUtils.NewAddress(
                        seed, securityLevel, index, checksum,
                        SpongeFactory.Create(SpongeFactory.Mode.KERL));
                if (!IsAddressSpent(newAddress, checksum))
                {
                    return newAddress;
                }

                index++;
            }
        }

        private bool IsAddressSpent(string newAddress, bool checksum)
        {
            string address = checksum ? newAddress : newAddress.AddChecksum();
            var response = IotaClient.FindTransactionsByAddresses(address);

            if (response.Hashes.Count == 0)
            {
                var spentFromResponse = IotaClient.WereAddressesSpentFrom(address);
                return spentFromResponse.States[0];
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public GetBundleResponse GetBundle(string transaction)
        {
            if (!InputValidator.IsHash(transaction))
            {
                throw new ArgumentException(Constants.INVALID_HASHES_INPUT_ERROR);
            }

            var stopWatch = Stopwatch.StartNew();

            Bundle bundle = TraverseBundle(transaction, null, new Bundle());
            if (bundle == null)
            {
                throw new ArgumentException(Constants.INVALID_BUNDLE_ERROR);
            }

            if (!BundleValidator.IsBundle(bundle))
            {
                throw new ArgumentException(Constants.INVALID_BUNDLE_ERROR);
            }

            stopWatch.Stop();
            return new GetBundleResponse(bundle.Transactions, stopWatch.ElapsedMilliseconds);
        }

        private Bundle TraverseBundle(string trunkTx, string bundleHash, Bundle bundle)
        {
            var gtr = IotaClient.GetTrytes(trunkTx);

            if (gtr != null)
            {

                if (gtr.Trytes.Count == 0)
                {
                    throw new ArgumentException(Constants.INVALID_BUNDLE_ERROR);
                }

                Transaction trx = new Transaction(gtr.Trytes[0]);
                if (trx.Bundle == null)
                {
                    throw new ArgumentException(Constants.INVALID_TRYTES_INPUT_ERROR);
                }

                // If first transaction to search is not a tail, return error
                if (bundleHash == null && trx.CurrentIndex != 0)
                {
                    throw new ArgumentException(Constants.INVALID_TAIL_HASH_INPUT_ERROR);
                }

                // If no bundle hash, define it
                if (bundleHash == null)
                {
                    bundleHash = trx.Bundle;
                }

                // If different bundle hash, return with bundle
                if (!bundleHash.Equals(trx.Bundle, StringComparison.Ordinal))
                {
                    bundle.Length = bundle.Transactions.Count;
                    return bundle;
                }

                // If only one bundle element, return
                if (trx.LastIndex == 0 && trx.CurrentIndex == 0)
                {
                    return new Bundle(new List<Transaction> {trx}, 1);
                }

                // Define new trunkTransaction for search
                trunkTx = trx.TrunkTransaction;
                // Add transaction object to bundle
                bundle.Transactions.Add(trx);

                // Continue traversing with new trunkTx
                return TraverseBundle(trunkTx, bundleHash, bundle);
            }

            throw new ArgumentException(Constants.GET_TRYTES_RESPONSE_ERROR);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public GetInclusionStatesResponse GetLatestInclusion(string[] hashes)
        {
            string[] latestMilestone = {IotaClient.GetNodeInfo().LatestSolidSubtangleMilestone};
            return IotaClient.GetInclusionStates(hashes, latestMilestone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public List<Transaction> FindTransactionObjectsByAddresses(string[] addresses)
        {
            var ftr = IotaClient.FindTransactionsByAddresses(addresses);
            if (ftr?.Hashes == null)
            {
                return new List<Transaction>();
            }

            // get the transaction objects of the transactions
            return FindTransactionObjectsByHashes(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="security"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inclusionStates"></param>
        /// <returns></returns>
        public GetTransferResponse GetTransfers(string seed, int security, int start, int end, bool inclusionStates)
        {
            // validate seed
            if (!InputValidator.IsValidSeed(seed))
                throw new IllegalStateException(Constants.INVALID_SEED_INPUT_ERROR);

            if (start < 0 || start > end || end > (start + 500))
                throw new ArgumentException(Constants.INVALID_INPUT_ERROR);

            if (!InputValidator.IsValidSecurityLevel(security))
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);

            var stopWatch = Stopwatch.StartNew();

            var addressRequest = new AddressRequest(seed, security)
            {
                Index = start,
                Checksum = true,
                Amount = end,
                AddSpendAddresses = true
            };

            var gnr = GenerateNewAddresses(addressRequest);

            if (gnr?.Addresses != null)
            {
                var bundles = BundlesFromAddresses(inclusionStates, gnr.Addresses.ToArray());

                stopWatch.Stop();
                return new GetTransferResponse(bundles, stopWatch.ElapsedMilliseconds);
            }

            stopWatch.Stop();
            return new GetTransferResponse(Array.Empty<Bundle>(), stopWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inclusionStates"></param>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public Bundle[] BundlesFromAddresses(bool inclusionStates, params string[] addresses)
        {
            List<Transaction> trxs = FindTransactionObjectsByAddresses(addresses);
            // set of tail transactions
            List<string> tailTransactions = new List<string>();
            List<string> nonTailBundleHashes = new List<string>();

            foreach (var trx in trxs)
            {
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                {
                    tailTransactions.Add(trx.CurlHash());
                }
                else
                {
                    if (nonTailBundleHashes.IndexOf(trx.Bundle) == -1)
                    {
                        nonTailBundleHashes.Add(trx.Bundle);
                    }
                }
            }

            List<Transaction> bundleObjects =
                FindTransactionObjectsByBundle(nonTailBundleHashes.ToArray());

            foreach (var trx in bundleObjects)
            {
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                {
                    var hash = trx.CurlHash();
                    if (tailTransactions.IndexOf(hash) == -1)
                    {
                        tailTransactions.Add(hash);
                    }
                }
            }

            List<Bundle> finalBundles = new List<Bundle>();
            var tailTxArray = tailTransactions.ToArray();

            // If inclusionStates, get the confirmation status
            // of the tail transactions, and thus the bundles
            GetInclusionStatesResponse gisr = null;
            if (tailTxArray.Length != 0 && inclusionStates)
            {
                gisr = GetLatestInclusion(tailTxArray);
                if (gisr?.States == null || gisr.States.Count == 0)
                {
                    throw new IllegalStateException(Constants.GET_INCLUSION_STATE_RESPONSE_ERROR);
                }
            }

            GetInclusionStatesResponse finalInclusionStates = gisr;
            try
            {
                Parallel.ForEach(tailTxArray, tailTx =>
                {
                    try
                    {
                        GetBundleResponse bundleResponse = GetBundle(tailTx);
                        Bundle gbr = new Bundle(bundleResponse.Transactions,
                            bundleResponse.Transactions.Count);
                        if (gbr.Transactions != null)
                        {
                            if (inclusionStates)
                            {
                                bool thisInclusion = false;
                                if (finalInclusionStates != null)
                                {
                                    thisInclusion = finalInclusionStates.States[tailTxArray.ToList().IndexOf(tailTx)];
                                }

                                foreach (var t in gbr.Transactions)
                                {
                                    t.Persistence = thisInclusion;
                                }
                            }

                            finalBundles.Add(gbr);
                        }

                        // If error returned from getBundle, simply ignore it because the bundle was most likely incorrect
                    }
                    catch (ArgumentException)
                    {
                        Log.Warn(Constants.GET_BUNDLE_RESPONSE_ERROR);
                    }
                });
            }
            catch (AggregateException)
            {
                return null;
            }

            finalBundles.Sort();
            Bundle[] returnValue = new Bundle[finalBundles.Count];
            for (int i = 0; i < finalBundles.Count; i++)
            {
                returnValue[i] = new Bundle(finalBundles[i].Transactions,
                    finalBundles[i].Transactions.Count);
            }

            return returnValue;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public List<Transaction> FindTransactionObjectsByBundle(params string[] bundles)
        {
            var ftr = IotaClient.FindTransactionsByBundles(bundles);
            if (ftr?.Hashes == null)
            {
                return new List<Transaction>();
            }

            // get the transaction objects of the transactions
            return FindTransactionObjectsByHashes(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tailTransactionHash"></param>
        /// <param name="depth"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public ReplayBundleResponse ReplayBundle(string tailTransactionHash, int depth, int minWeightMagnitude,
            string reference)
        {
            if (!InputValidator.IsHash(tailTransactionHash))
            {
                throw new ArgumentException(Constants.INVALID_TAIL_HASH_INPUT_ERROR);
            }

            var stopWatch = Stopwatch.StartNew();

            GetBundleResponse bundleResponse = GetBundle(tailTransactionHash);
            Bundle bundle = new Bundle(bundleResponse.Transactions, bundleResponse.Transactions.Count);
            return ReplayBundle(bundle, depth, minWeightMagnitude, reference, stopWatch);

        }

        private ReplayBundleResponse ReplayBundle(Bundle bundle, int depth, int minWeightMagnitude, string reference,
            Stopwatch stopWatch)
        {

            List<string> bundleTrytes = new List<string>();

            foreach (var trx in bundle.Transactions)
            {
                bundleTrytes.Add(trx.ToTrytes());
            }

            bundleTrytes.Reverse();


            var trxs = SendTrytes(bundleTrytes.ToArray(), depth, minWeightMagnitude, reference);

            bool[] successful = new bool[trxs.Length];

            for (int i = 0; i < trxs.Length; i++)
            {

                var response = IotaClient.FindTransactionsByBundles(trxs[i].Bundle);

                successful[i] = response.Hashes.Count != 0;
            }

            stopWatch.Stop();
            return new ReplayBundleResponse(new Bundle(trxs.ToList(), trxs.Length), successful,
                stopWatch.ElapsedMilliseconds);

        }
    }
}
