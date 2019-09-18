using System;
using IotaSharp.Pow;

namespace IotaSharp.Utils
{
    /// <summary>
    /// This class defines utility methods to add/remove the checksum to/from an address.
    /// </summary>
    public static class Checksum
    {
        /// <summary>
        ///     Adds the checksum to the specified address
        /// </summary>
        /// <param name="address">An address without checksum</param>
        /// <returns>The address with the appended checksum </returns>
        public static string AddChecksum(this string address)
        {
            InputValidator.CheckAddressWithoutChecksum(address);
            var addressWithChecksum = address;
            addressWithChecksum += CalculateChecksum(address);
            return addressWithChecksum;
        }

        /// <summary>
        ///     Removes the checksum from the specified address with checksum
        /// </summary>
        /// <param name="address">The address with checksum or without checksum.</param>
        /// <returns>the specified address without checksum</returns>

        public static string RemoveChecksum(this string address)
        {
            if (InputValidator.IsAddress(address)) return RemoveChecksumFromAddress(address);

            if (InputValidator.IsAddressWithoutChecksum(address)) return address;

            throw new ArgumentException(Constants.INVALID_ADDRESSES_INPUT_ERROR);
        }

        /// <summary>
        /// Determines whether the specified address with checksum has a valid checksum.
        /// </summary>
        /// <param name="addressWithChecksum"></param>
        /// <returns></returns>
        public static bool IsValidChecksum(this string addressWithChecksum)
        {
            var addressWithoutChecksum = RemoveChecksum(addressWithChecksum);
            var addressWithRecalculateChecksum = addressWithoutChecksum + CalculateChecksum(addressWithoutChecksum);
            return addressWithRecalculateChecksum.Equals(addressWithChecksum, StringComparison.Ordinal);
        }

        private static string CalculateChecksum(string address)
        {
            ICurl curl = SpongeFactory.Create(SpongeFactory.Mode.KERL);
            curl.Reset();
            curl.Absorb(Converter.ToTrits(address));
            sbyte[] checksumTrits = new sbyte[Sponge.HASH_LENGTH];
            curl.Squeeze(checksumTrits);
            string checksum = Converter.ToTrytes(checksumTrits);
            return checksum.Substring(72, 9);
        }

        

        private static string RemoveChecksumFromAddress(string addressWithChecksum)
        {
            return addressWithChecksum.Substring(0, Constants.ADDRESS_LENGTH_WITHOUT_CHECKSUM);
        }
    }
}
