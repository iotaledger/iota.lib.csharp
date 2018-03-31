using System.Text;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    ///     This class allows to convert between ASCII and tryte encoded strings
    /// </summary>
    public class TrytesConverter
    {
        /// <summary>
        ///     Converts the ASCII encoded string to trytes
        /// </summary>
        /// <param name="inputString">ASCII encoded string</param>
        /// <returns>tryte encoded string</returns>
        public static string ToTrytes(string inputString)
        {
            var trytes = new StringBuilder();

            foreach (var input in inputString)
            {
                var asciiValue = input;

                // If not recognizable ASCII character, replace with space
                if (asciiValue > 255) asciiValue = ' ';

                trytes.Append(Constants.TryteAlphabet[asciiValue % 27]);
                trytes.Append(Constants.TryteAlphabet[asciiValue / 27]);
            }

            return trytes.ToString();
        }
        
        /// <summary>
        ///     Converts the specified tryte encoded String to ASCII
        /// </summary>
        /// <param name="inputTrytes">tryte encoded string</param>
        /// <returns>an ASCII encoded string</returns>
        public static string ToString(string inputTrytes)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < inputTrytes.Length; i += 2)
            {
                // get a trytes pair

                var firstValue = TryteToDecimal(inputTrytes[i]);
                var secondValue = TryteToDecimal(inputTrytes[i + 1]);
                var decimalValue = firstValue + secondValue * 27;

                builder.Append((char) decimalValue);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Tryte To Decimal, '9' = 0
        /// </summary>
        /// <param name="tryte"></param>
        /// <returns></returns>
        public static int TryteToDecimal(char tryte)
        {
            if (tryte == '9')
                return 0;

            return tryte - 'A' + 1;
        }
    }
}