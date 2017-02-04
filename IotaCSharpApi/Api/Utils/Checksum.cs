using Iota.Lib.CSharp.Api.Exception;

namespace Iota.Lib.CSharp.Api.Utils
{
    public static class Checksum
    {
        public static string AddChecksum(this string address)
        {
            InputValidator.CheckAddress(address);
            string addressWithChecksum = address;
            addressWithChecksum += CalculateChecksum(address);
            return addressWithChecksum;
        }

        public static string RemoveChecksum(this string addressWithChecksum)
        {
            if (IsAddressWithChecksum(addressWithChecksum))
            {
                return GetAddress(addressWithChecksum);
            }
            throw new InvalidAddressException(addressWithChecksum);
        }

        internal static string GetAddress(string addressWithChecksum)
        {
            return addressWithChecksum.Substring(0, Constants.AddressLengthWithoutChecksum);
        }

        public static bool IsValidChecksum(this string addressWithChecksum)
        {
            string addressWithoutChecksum = RemoveChecksum(addressWithChecksum);
            string adressWithRecalculateChecksum = addressWithoutChecksum + CalculateChecksum(addressWithoutChecksum);
            return adressWithRecalculateChecksum.Equals(addressWithChecksum);
        }


        private static bool IsAddressWithChecksum(string addressWithChecksum)
        {
            return addressWithChecksum.IsAddress() && addressWithChecksum.Length == Constants.AddressLengthWithChecksum;
        }

        private static string CalculateChecksum(string address)
        {
            Curl curl = new Curl();
            curl.Reset();
            curl.State = Converter.CopyTrits(address, curl.State);
            curl.Transform();
            return Converter.ToTrytes(curl.State).Substring(0, 9);
        }
    }
}