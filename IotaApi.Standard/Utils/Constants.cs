namespace Iota.Api.Standard.Utils
{
    /// <summary>
    ///     This class defines different constants that are used accros the library
    /// </summary>
    public static class Constants
    {
        public const string TRYTE_ALPHABET = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const int SEED_LENGTH_MAX = 81;

        public const string EMPTY_HASH = "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        public const int ADDRESS_LENGTH_WITHOUT_CHECKSUM = 81;

        public const int ADDRESS_LENGTH_WITH_CHECKSUM = 90;

        public const int MESSAGE_LENGTH = 2187;

        public const int TAG_LENGTH = 27;

        public const int RADIX = 3;

        public const int MAX_TRIT_VALUE = 1;

        public const int MIN_TRIT_VALUE = -1;
    }
}