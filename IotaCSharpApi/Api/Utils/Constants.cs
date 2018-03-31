namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    ///     This class defines different constants that are used accros the library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     This String contains all possible characters of the tryte alphabet
        /// </summary>
        public static readonly string TryteAlphabet = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///     The maximum seed length
        /// </summary>
        public static readonly int SeedLengthMax = 81;

        /// <summary>
        ///     This String represents the empty hash consisting of '9'
        /// </summary>
        public static readonly string EmptyHash =
            "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        /// <summary>
        ///     The length of an address without checksum
        /// </summary>
        public static readonly int AddressLengthWithoutChecksum = 81;

        /// <summary>
        ///     The address length with checksum
        /// </summary>
        public static readonly int AddressLengthWithChecksum = 90;

        /// <summary>
        ///     The length of an message
        /// </summary>
        public static int MessageLength = 2187;
        /// <summary>
        /// 
        /// </summary>
        public static int TagLength = 27;
    }
}