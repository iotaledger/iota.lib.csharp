using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Lib.CSharp.Api.Exception;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class provides methods to validate the parameters of different iota API methods
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// validates an adress
        /// </summary>
        /// <param name="address">address to validate</param>
        /// <returns>Bool</returns>
        public static bool IsAddress(this string address)
        {
            if (address.Length == Constants.AddressLengthWithoutChecksum ||
                address.Length == Constants.AddressLengthWithChecksum)
            {
                return IsTrytes(address, address.Length);
            }
            return false;
        }

        /// <summary>
        /// Checks whether the specified address is an address
        /// </summary>
        /// <param name="address">address to check</param>
        /// <exception cref="InvalidAddressException">exception which is thrown when the address is invalid</exception>
        /// <returns></returns>
        public static void CheckAddress(string address)
        {
            if (!address.IsAddress())
                throw new InvalidAddressException(address);
        }

        /// <summary>
        /// Determines whether the specified string represents a signed integer
        /// </summary>
        /// <param name="value">a string</param>
        /// <returns></returns>
        public static bool isValue(String value)
        {
            return Regex.IsMatch(value, @"^(-){0,1}\d+$");
        }

        public static bool IsArrayOfHashes(string[] hashes)
        {
            if (hashes == null)
                return false;

            foreach (String hash in hashes)
            {
                // Check if address with checksum
                if (hash.Length == 90)
                {
                    if (!IsTrytes(hash, 90))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!IsTrytes(hash, 81))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// checks if input is correct trytes consisting of A-Z9 optionally validates length
        /// </summary>
        /// <param name="trytes">address to validate</param>
        /// <param name="length">address to validate</param>
        /// <returns>Bool</returns>
        public static bool IsTrytes(string trytes, int length)
        {
            // If no length specified, just validate the trytes
            var regexTrytes = new Regex("^[9A-Z]{" + (length == 0 ? "0," : length.ToString()) + "}$");
            return regexTrytes.IsMatch(trytes);
        }

        public static bool IsTransfersCollectionCorrect(List<Transfer> transfers)
        {
            foreach (Transfer transfer in transfers)
            {
                if (!IsTransfersArray(transfer))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsTransfersArray(Transfer transfer)
        {
            if (!IsAddress(transfer.Address))
            {
                return false;
            }

            // Check if message is correct trytes of any length
            if (!IsTrytes(transfer.Message, 0))
            {
                return false;
            }

            // Check if tag is correct trytes of {0,27} trytes
            return IsTrytes(transfer.Tag, 27);
        }

        public static void CheckTransferArray(Transfer[] transactionsArray)
        {
            if (!IsTransfersCollectionCorrect(transactionsArray.ToList()))
                throw new System.Exception("Not a transfer array");
        }

        public static void CheckSeed(string seed)
        {
            // validate the seed
            if (!InputValidator.IsTrytes(seed, 0))
            {
                throw new IllegalStateException("Invalid Seed: Format not in trytes");
            }

            // validate & if needed pad seed
            if (seed.Length > 81)
            {
                throw new IllegalStateException("Invalid Seed: Seed too long");
            }
        }

        public static String PadSeedIfNecessary(String seed)
        {
            while (seed.Length < 81) seed += 9;
            return seed;
        }
    }
}