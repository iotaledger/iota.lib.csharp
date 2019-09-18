using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using IotaSharp.Model;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class InputValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <returns></returns>
        public static bool IsTrits(sbyte[] trits)
        {
            foreach (var trit in trits)
            {
                if (trit < -1 || trit > 1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is an address.
        /// Address must contain a checksum to be valid
        /// </summary>
        /// <param name="address">The address to validate.</param>
        /// <returns>
        ///     <c>true</c> if the specified string is an address; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAddress(string address)
        {
            return address.Length == Constants.ADDRESS_LENGTH_WITH_CHECKSUM && IsTrytes(address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsAddressWithoutChecksum(string address)
        {
            return address.Length == Constants.ADDRESS_LENGTH_WITHOUT_CHECKSUM && IsTrytes(address);
        }

        /// <summary>
        /// Checks whether the specified address is an address and throws and exception if the address is invalid.
        /// Addresses must contain a checksum to be valid.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool CheckAddress(string address)
        {
            if (!IsAddress(address))
                throw new ArgumentException(Constants.INVALID_ADDRESSES_INPUT_ERROR);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool CheckAddressWithoutChecksum(string address)
        {
            if (!IsAddressWithoutChecksum(address))
            {
                throw new ArgumentException(Constants.INVALID_ADDRESSES_INPUT_ERROR);
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string contains only characters from the trytes alphabet (see <see cref="Constants.TRYTE_ALPHABET"/>)
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///   <c>true</c> if the specified trytes are trytes otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTrytes(string trytes, int length)
        {
            string regex = "^[9A-Z]{" + (length == 0 ? "0," : length.ToString(CultureInfo.InvariantCulture)) + "}$";
            var regexTrytes = new Regex(regex);
            return regexTrytes.IsMatch(trytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static bool IsTrytes(string trytes)
        {
            return IsTrytes(trytes, trytes.Length);
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
            return long.TryParse(value, out _);
        }

        /// <summary>
        /// Checks if input is correct hash.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsHash(string hash)
        {
            if (hash.Length == Constants.ADDRESS_LENGTH_WITH_CHECKSUM)
            {
                if (!IsTrytes(hash, Constants.ADDRESS_LENGTH_WITH_CHECKSUM))
                {
                    return false;
                }
            }
            else
            {
                if (!IsTrytes(hash, Constants.ADDRESS_LENGTH_WITHOUT_CHECKSUM))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified array contains only valid hashes.
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public static bool IsArrayOfHashes(string[] hashes)
        {
            if (hashes == null)
                return false;

            foreach (var hash in hashes)
            {
                if (!IsHash(hash))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string array contains only trytes
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static bool IsArrayOfTrytes(string[] trytes)
        {
            return IsArrayOfTrytes(trytes, Constants.TRANSACTION_LENGTH);
        }

        /// <summary>
        /// Determines whether the specified string array contains only trytes.
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="length">The length each String should be</param>
        /// <returns></returns>
        private static bool IsArrayOfTrytes(string[] trytes, int length)
        {
            return trytes.ToList().TrueForAll(element => IsTrytes(element, length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <returns></returns>
        public static bool IsArrayOfRawTransactionTrytes(string[] trytes)
        {
            foreach (var tryte in trytes)
            {
                // This part of the value trits exceed iota max supply when used
                if (!IsNinesTrytes(tryte.Substring(2279, 2295 - 2279), 16))
                {
                    return false;
                }
            }

            return IsArrayOfTrytes(trytes, Constants.TRANSACTION_LENGTH);
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
            return Regex.IsMatch(trytes,
                "^[9]{" + (length == 0 ? "0," : length.ToString(CultureInfo.InvariantCulture)) + "}$");
        }

        /// <summary>
        /// Checks if the tag is valid.
        /// Alias of IsValidTag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsTag(string tag)
        {
            return IsValidTag(tag);
        }

        /// <summary>
        /// Checks if the tag is valid. The string must not be empty and must contain trytes.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static bool IsValidTag(string tag)
        {
            return tag != null && tag.Length <= Constants.TAG_LENGTH && IsTrytes(tag);
        }

        /// <summary>
        /// Determines whether the specified transfer is valid.
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public static bool IsValidTransfer(Transfer transfer)
        {

            if (transfer == null)
            {
                return false;
            }

            if (!IsAddress(transfer.Address))
            {
                return false;
            }

            // Check if message is correct trytes encoded of any length
            if (transfer.Message == null || !IsTrytes(transfer.Message, transfer.Message.Length))
            {
                return false;
            }

            // Check if tag is correct trytes encoded and not longer than 27 trytes
            if (!IsTag(transfer.Tag))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified transfers are valid.
        /// </summary>
        /// <param name="transfers"></param>
        /// <returns></returns>
        public static bool IsValidTransfersCollection(List<Transfer> transfers)
        {
            // Input validation of transfers object
            if (transfers == null || transfers.Count == 0)
                return false;

            foreach (var transfer in transfers)
            {
                if (!IsValidTransfer(transfer))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public static bool IsValidInputsCollection(Input[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
                return false;

            foreach (var input in inputs)
            {
                if (!IsValidInput(input))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidInput(Input input)
        {
            if (input == null)
                return false;

            if (!IsAddress(input.Address))
                return false;

            if (input.KeyIndex < 0)
                return false;

            return IsValidSecurityLevel(input.Security);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static bool IsValidSeed(string seed)
        {
            return seed.Length <= Constants.SEED_LENGTH_MAX && IsTrytes(seed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool IsValidSecurityLevel(int level)
        {
            return level >= Constants.MIN_SECURITY_LEVEL && level <= Constants.MAX_SECURITY_LEVEL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string PadSeedIfNecessary(string seed)
        {
            while (seed.Length < Constants.ADDRESS_LENGTH_WITHOUT_CHECKSUM) seed += '9';
            return seed;
        }
    }
}
