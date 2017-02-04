namespace Iota.Lib.CSharp.Api.Utils
{
    public static class Constants
    {
        public static readonly string TryteAlphabet = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly int SeedLengthMax = 81;

        public static readonly string EmptyHash =
            "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        public static readonly int AddressLengthWithoutChecksum = 81;
        public static readonly int AddressLengthWithChecksum = 90;
    }
}