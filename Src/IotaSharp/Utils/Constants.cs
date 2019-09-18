using System.Diagnostics.CodeAnalysis;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class Constants
    {
        /// <summary>
        /// 
        /// </summary>
        public const string NULL_HASH =
            "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        /// <summary>
        ///     This String contains all possible characters of the tryte alphabet
        /// </summary>
        public static readonly string TRYTE_ALPHABET = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The length of an IOTA seed
        /// </summary>
        public const int SEED_LENGTH_MAX = 81;

        /// <summary>
        /// The length of an address with checksum
        /// </summary>
        public const int ADDRESS_LENGTH_WITH_CHECKSUM = 90;

        /// <summary>
        /// The length of an address without checksum
        /// </summary>
        public const int ADDRESS_LENGTH_WITHOUT_CHECKSUM = 81;

        /// <summary>
        /// 
        /// </summary>
        public const int MESSAGE_LENGTH = 2187;

        /// <summary>
        /// The length of a transaction
        /// </summary>
        public const int TRANSACTION_LENGTH = 2673;

        /// <summary>
        /// The length of an tag in trytes
        /// </summary>
        public const int TAG_LENGTH = 27;

        /// <summary>
        /// 
        /// </summary>
        public const int MIN_SECURITY_LEVEL = 1;

        /// <summary>
        /// 
        /// </summary>
        public const int MAX_SECURITY_LEVEL = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int KEY_LENGTH = 6561;

#pragma warning disable CS1591
        public const string INVALID_TAG_INPUT_ERROR = "Invalid tag provided.";
        public const string INVALID_TRYTES_INPUT_ERROR = "Invalid trytes provided.";
        public const string INVALID_TRANSFERS_INPUT_ERROR = "Invalid transfers provided.";
        public const string INVALID_ADDRESSES_INPUT_ERROR = "Invalid addresses provided.";
        public const string INVALID_SEED_INPUT_ERROR = "Invalid seed provided.";
        public const string INVALID_SECURITY_LEVEL_INPUT_ERROR = "Invalid security level provided.";
        public const string INVALID_BUNDLE_HASH_ERROR = "Invalid bundle hash.";
        public const string INVALID_HASH_INPUT_ERROR = "Invalid hash provided.";
        public const string INVALID_HASHES_INPUT_ERROR = "Invalid hashes provided.";
        public const string INVALID_ATTACHED_TRYTES_INPUT_ERROR = "Invalid attached trytes provided.";

        public const string NOT_ENOUGH_BALANCE_ERROR = "Not enough balance.";
        public const string INVALID_BUNDLE_ERROR = "Invalid bundle.";
        public const string INVALID_BUNDLE_SUM_ERROR = "Invalid bundle sum.";
        public const string INVALID_SIGNATURES_ERROR = "Invalid signatures.";
        public const string INVALID_TAIL_HASH_INPUT_ERROR = "Invalid tail hash provided.";
        public const string INVALID_INPUT_ERROR = "Invalid input provided.";

        public const string GET_TRYTES_RESPONSE_ERROR = "Get trytes response was null.";
        public const string GET_BUNDLE_RESPONSE_ERROR = "Get bundle response was null.";
        public const string SENDING_TO_USED_ADDRESS_ERROR = "Sending to a used address.";
        public const string GET_INCLUSION_STATE_RESPONSE_ERROR = "Get inclusion state response was null.";
#pragma warning restore CS1591

    }
}
