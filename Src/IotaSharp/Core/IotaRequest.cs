using System.Globalization;

namespace IotaSharp.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class IotaRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public IotaRequest(Command command)
        {
            Command = FirstCharToLower(command.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public string Command { get; set; }

        private string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var firstChar = char.ToLower(input[0], CultureInfo.InvariantCulture);

            return firstChar + input.Substring(1);
        }
    }
}
