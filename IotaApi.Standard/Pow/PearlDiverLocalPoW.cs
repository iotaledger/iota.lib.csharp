using Iota.Api.Standard.Core;
using Iota.Api.Standard.Exception;
using Iota.Api.Standard.Utils;

namespace Iota.Api.Standard.Pow
{
    /// <summary>
    /// </summary>
    public class PearlDiverLocalPoW : ILocalPoW
    {
        private readonly PearlDiver _pearlDiver = new PearlDiver();

        /// <summary>
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <returns></returns>
        /// <exception cref="IllegalStateException"></exception>
        public string PerformPoW(string trytes, int minWeightMagnitude)
        {
            var trits = Converter.ToTrits(trytes);

            if (!_pearlDiver.Search(trits, minWeightMagnitude, 0))
                throw new IllegalStateException("PearlDiver search failed");

            return Converter.ToTrytes(trits);
        }
    }
}