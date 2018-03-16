using Iota.Lib.CSharp.Api.Exception;
using Iota.Lib.CSharp.Api.Pow;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    ///     This class defines utility methods to add/remove the checksum to/from an address
    /// </summary>
    public static class Checksum
    {
        /// <summary>
        ///     Adds the checksum to the specified address
        /// </summary>
        /// <param name="address">An address without checksum</param>
        /// <returns>The address with the appended checksum </returns>
        /// <exception cref="InvalidAddressException">is thrown when an invalid address is provided</exception>
        public static string AddChecksum(string address)
        {
            InputValidator.CheckAddress(address);
            var addressWithChecksum = address;
            addressWithChecksum += CalculateChecksum(address);
            return addressWithChecksum;
        }


        /// <summary>
        ///     Removes the checksum from the specified address with checksum
        /// </summary>
        /// <param name="address">The address with checksum or without checksum.</param>
        /// <returns>the specified address without checksum</returns>
        /// <exception cref="InvalidAddressException">is thrown when the specified address is not an address with checksum</exception>
        public static string RemoveChecksum(this string address)
        {
            if (IsAddressWithChecksum(address)) return GetAddress(address);

            if (IsAddressWithoutChecksum(address)) return address;

            throw new InvalidAddressException(address);
        }


        internal static string GetAddress(string addressWithChecksum)
        {
            return addressWithChecksum.Substring(0, Constants.AddressLengthWithoutChecksum);
        }

        /// <summary>
        ///     Determines whether the specified address with checksum has a valid checksum.
        /// </summary>
        /// <param name="addressWithChecksum">The address with checksum.</param>
        /// <returns>
        ///     <c>true</c> if the specified address with checksum has a valid checksum [the specified address with checksum];
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidChecksum(this string addressWithChecksum)
        {
            var addressWithoutChecksum = RemoveChecksum(addressWithChecksum);
            var adressWithRecalculateChecksum = addressWithoutChecksum + CalculateChecksum(addressWithoutChecksum);
            return adressWithRecalculateChecksum.Equals(addressWithChecksum);
        }


        private static bool IsAddressWithChecksum(string addressWithChecksum)
        {
            return InputValidator.IsAddress(addressWithChecksum) &&
                   addressWithChecksum.Length == Constants.AddressLengthWithChecksum;
        }

        private static bool IsAddressWithoutChecksum(string address)
        {
            return InputValidator.CheckAddress(address) && address.Length == Constants.AddressLengthWithoutChecksum;
        }

        private static string CalculateChecksum(string address)
        {
            // TODO inject curl
            ICurl curl = new Kerl();
            curl.Reset();
            curl.Absorb(Converter.ToTrits(address));
            var checksumTrits = new int[Curl.HashLength];
            curl.Squeeze(checksumTrits);
            var checksum = Converter.ToTrytes(checksumTrits);
            return checksum.Substring(72, 9);
        }
    }
}