using System.Linq;
using System.Text;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class allows to convert between ASCII and tryte encoded strings 
    /// </summary>
    public class TrytesConverter
    {
        /// <summary>
        /// Converts the ASCII encoded string to trytes
        /// </summary>
        /// <param name="inputString">ASCII encoded string</param>
        /// <returns>tryte encoded string</returns>
        public static string ToTrytes(string inputString)
        {
            StringBuilder trytes = new StringBuilder();

            for (int i = 0; i < inputString.Length; i++)
            {
                char asciiValue = inputString.ElementAt(i);

                // If not recognizable ASCII character, replace with space
                if (asciiValue > 255)
                {
                    asciiValue = (char) 32;
                }

                int firstValue = asciiValue%27;
                int secondValue = (asciiValue - firstValue)/27;

                string trytesValue = Constants.TryteAlphabet[firstValue].ToString() +
                                     Constants.TryteAlphabet[secondValue];

                trytes.Append(trytesValue);
            }

            return trytes.ToString();
        }

        /// <summary>
        /// Converts the specified tryte encoded String to ASCII
        /// </summary>
        /// <param name="inputTrytes">tryte encoded string</param>
        /// <returns>an ASCII encoded string</returns>
        public static string ToString(string inputTrytes)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < inputTrytes.Length; i += 2)
            {
                // get a trytes pair

                int firstValue = Constants.TryteAlphabet.IndexOf(inputTrytes[(i)]);
                int secondValue = Constants.TryteAlphabet.IndexOf(inputTrytes[(i + 1)]);

                int decimalValue = firstValue + secondValue*27;

                string character = ((char) decimalValue).ToString();

                builder.Append(character);
            }

            return builder.ToString();
        }
    }
}