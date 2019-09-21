namespace IotaSharp.Model
{
    /// <summary>
    /// This class represents a Transfer.
    /// </summary>
    public class Transfer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="address"></param>
        /// <param name="hash"></param>
        /// <param name="persistence"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        public Transfer(
            string timestamp, string address, string hash,
            bool persistence,
            long value, string message, string tag)
        {
            Timestamp = timestamp;
            Address = address;
            Hash = hash;
            Persistence = persistence;
            Value = value;
            Message = message;
            Tag = tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">must contain checksum</param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        public Transfer(string address, long value, string message = "", string tag = "")
        {
            Address = address;
            Value = value;
            Message = message;
            Tag = tag;
        }

        /// <summary>
        /// Timestamp
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Address, must contain checksum
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Persistence
        /// </summary>
        public bool Persistence { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public string Tag { get; set; }
    }
}
