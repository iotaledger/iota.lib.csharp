using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Lib.CSharp.Api.Exception;
using Iota.Lib.CSharp.Api.Model;
using RestSharp.Extensions;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class provides methods to validate the parameters of different iota API methods
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Determines whether the specified string is an adrdress.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is an address; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAddress(string address)
        {
            if (address.Length == Constants.AddressLengthWithoutChecksum ||
                address.Length == Constants.AddressLengthWithChecksum)
            {
                return IsTrytes(address, address.Length);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsHash(string hash)
        {
            return IsTrytes(hash, 81);
        }

        /// <summary>
        /// Checks whether the specified address is an address and throws and exception if the address is invalid
        /// </summary>
        /// <param name="address">address to check</param>
        /// <exception cref="InvalidAddressException">exception which is thrown when the address is invalid</exception>
        public static bool CheckAddress(string address)
        {
            if (!IsAddress(address))
                throw new InvalidAddressException(address);

            return true;
        }

        /// <summary>
        /// Determines whether the specified string represents an integer value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> the specified string represents an integer value; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValue(string value)
        {
            // ReSharper disable once NotAccessedVariable
            long tempValue;
            return long.TryParse(value, out tempValue);
            //return Regex.IsMatch(value, @"^(-){0,1}\d+$");
        }

        /// <summary>
        /// Determines whether the specified array contains only valid hashes
        /// </summary>
        /// <param name="hashes">The hashes.</param>
        /// <returns>
        ///   <c>true</c> the specified array contains only valid hashes; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArrayOfHashes(string[] hashes)
        {
            if (hashes == null)
                return false;

            foreach (string hash in hashes)
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
        /// Determines whether the specified string contains only characters from the trytes alphabet (see <see cref="Constants.TryteAlphabet"/>)
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified trytes are trytes otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTrytes(string trytes, int length)
        {
            string regex = "^[9A-Z]{" + (length == 0 ? "0," : length.ToString()) + "}$";
            var regexTrytes = new Regex(regex);
            return regexTrytes.IsMatch(trytes);
        }

        /// <summary>
        /// Determines whether the specified string array contains only trytes
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified array contains only valid trytes otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArrayOfTrytes(string[] trytes, int length )
        {
           return trytes.ToList().TrueForAll(element => IsTrytes(element, length));
        }

        /// <summary>
        /// Determines whether the specified transfers are valid
        /// </summary>
        /// <param name="transfers">The transfers.</param>
        /// <returns>
        ///   <c>true</c> if the specified transfers are valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTransfersCollectionValid(ICollection<Transfer> transfers)
        {
            foreach (Transfer transfer in transfers)
            {
                if (!IsValidTransfer(transfer))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified transfer is valid.
        /// </summary>
        /// <param name="transfer">The transfer.</param>
        /// <returns>
        ///   <c>true</c> if the specified transfer is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTransfer(Transfer transfer)
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

        /// <summary>
        /// Checks the specified specified transfers are valid. If not, an exception is thrown.
        /// </summary>
        /// <param name="transactionsArray">The transactions array.</param>
        /// <exception cref="System.Exception">Not a transfer array</exception>
        public static void CheckTransferArray(Transfer[] transactionsArray)
        {
            if (!IsTransfersCollectionValid(transactionsArray.ToList()))
                throw new System.Exception("Not a transfer array");
        }

        /// <summary>
        /// Checks if the seed is valid. If not, an exception is thrown.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <exception cref="IllegalStateException">
        /// Invalid Seed: Format not in trytes
        /// or
        /// Invalid Seed: Seed too long
        /// </exception>
        public static void CheckIfValidSeed(string seed)
        {
            // validate the seed
            if (!IsTrytes(seed, 0))
            {
                throw new IllegalStateException("Invalid Seed: Format not in trytes");
            }

            // validate & if needed pad seed
            if (seed.Length > 81)
            {
                throw new IllegalStateException("Invalid Seed: Seed too long");
            }
        }

        /// <summary>
        /// Pads the seed if necessary.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public static string PadSeedIfNecessary(string seed)
        {
            while (seed.Length < Constants.AddressLengthWithoutChecksum) seed += 9;
            return seed;
        }

        /// <summary>
        /// Checks if the specified array is an array of trytes. If not an exception is thrown.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <exception cref="InvalidTryteException"></exception>
        public static void CheckIfArrayOfTrytes(string[] trytes)
        {
            if(!IsArrayOfTrytes(trytes, 2673))
                throw new InvalidTryteException();
        }

        /// <summary>
        /// Determines whether the specified string consist only of '9'.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified string consist only of '9'; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNinesTrytes(string trytes, int length)
        {
            return trytes.Matches("^[9]{" + (length == 0 ? "0," : length.ToString()) + "}$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static bool IsValidSeed(string seed)
        {
            return IsTrytes(seed, seed.Length);
        }
    }
}