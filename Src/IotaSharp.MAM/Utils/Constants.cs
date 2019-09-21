namespace IotaSharp.MAM.Utils
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class Constants
    {
        public const int Radix = 3;
        public const int TritsPerTryte = 3;
        public const int HashLength = 243;
        public const int TryteWidth = 3;
        public const int MaxTryteValue = 13;
        public const int MinTryteValue = -13;
        public const int KeyLength = ((HashLength / 3) / Radix) * HashLength;
        public const int DigestLength = HashLength;
        public const int AddressLength = HashLength;
        public const int SignatureLength = KeyLength;
        public const int MessageNonceLength = HashLength / 3;
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
