namespace Iota.Lib.CSharp.Api.Utils
{
    public static class Constants
    {
        public static readonly string TRYTE_ALPHABET = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static int SEED_LENGTH_MAX = 81;

        public static string EMPTY_HASH =
            "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        public static readonly int AddressLengthWithoutChecksum = 81;
        public static readonly int AddressLengthWithChecksum = 90;
    }
}