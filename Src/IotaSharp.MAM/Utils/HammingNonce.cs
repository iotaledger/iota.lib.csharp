using System;
using IotaSharp.Pow;

namespace IotaSharp.MAM.Utils
{
    /// Searches for a checksum given by hamming weight
    /// Returns true if it found a valid nonce for the checksum.
    /// It will start with a number of trits given by `length`, but may grow
    /// If security is 1, then the first 81 trits will sum to 0
    /// If security is 2, then the first 81 trits will not sum to 0, but the first 162 trits will.
    /// If security is 3, then neither the first 81 nor the first 162 trits will sum to zero, but
    /// the entire hash will sum to zero
    /// To prepare, you should absorb the length in trits
    public class HammingNonce
    {
        private const int Radix = 3;
        private const int NumberOfRoundsP81 = 81;
        private const int NumberOfRoundsP27 = 27;
        private readonly int _numberOfRounds;

        private const int CurlHashLength = 243;
        private const int CurlStateLength = CurlHashLength * 3;

        private readonly ulong[] _stateHigh;
        private readonly ulong[] _stateLow;

        public HammingNonce(SpongeFactory.Mode curlMode)
        {
            _stateLow = new ulong[CurlStateLength];
            _stateHigh = new ulong[CurlStateLength];

            switch (curlMode)
            {
                case SpongeFactory.Mode.CURLP27:
                    _numberOfRounds = NumberOfRoundsP27;
                    break;
                case SpongeFactory.Mode.CURLP81:
                    _numberOfRounds = NumberOfRoundsP81;
                    break;
            }
        }


        public bool Search(
            int security, int offset, int length,
            ICurl tcurl)
        {
            // 
            SearchPrepareTrits(tcurl.State, offset);

            //
            return SearchCpu(tcurl.State, offset, length, security);

        }

        private bool SearchCpu(sbyte[] state, int offset, int length, int security)
        {
            int count = Math.Min(length, CurlHashLength) - offset;

            // Ignore group incr
            int index = -1;
            while (index < 0)
            {
                int increaseLength = Increase(_stateLow, _stateHigh, offset + count * 2 / 3, count - count * 2 / 3);
                count =
                    (int) Math.Min(TritsHelper.RoundThird(offset + count * 2 / 3 + increaseLength), CurlHashLength) -
                    offset;

                // clone
                ulong[] stateLow = new ulong[CurlStateLength];
                ulong[] stateHigh = new ulong[CurlStateLength];
                Array.Copy(_stateLow, stateLow, CurlStateLength);
                Array.Copy(_stateHigh, stateHigh, CurlStateLength);

                // transform
                Transform(stateLow, stateHigh);

                // check
                index = Check(security, stateLow, stateHigh);
            }

            // output
            for (int i = 0; i < count; i++)
            {
                ulong low = (_stateLow[i] >> index) & 1;
                ulong high = (_stateHigh[i] >> index) & 1;

                if (low == 1 && high == 0)
                    state[i] = -1;
                else if (low == 0 && high == 1)
                    state[i] = 1;
                else if (low == 1 && high == 1)
                    state[i] = 0;
                else
                    state[i] = 0;
            }

            return false;
        }

        private int Check(int security, ulong[] stateLow, ulong[] stateHigh)
        {
            sbyte[] hash = new sbyte[CurlHashLength];

            for (int i = 0; i < 64; i++)
            {
                for (int n = 0; n < CurlHashLength; n++)
                {
                    ulong low = (stateLow[n] >> i) & 1;
                    ulong high = (stateHigh[n] >> i) & 1;

                    if (low == 1 && high == 0)
                        hash[n] = -1;
                    else if (low == 0 && high == 1)
                        hash[n] = 1;
                    else if (low == 1 && high == 1)
                        hash[n] = 0;
                    else
                        hash[n] = 0;

                }

                if (HashHelper.CheckSumSecurity(hash) == security)
                    return i;

            }

            return -1;
        }

        private void Transform(ulong[] stateLow, ulong[] stateHigh)
        {
            var curlScratchpadLow = new ulong[CurlStateLength];
            var curlScratchpadHigh = new ulong[CurlStateLength];
            var curlScratchpadIndex = 0;
            for (var round = _numberOfRounds; round-- > 0;)
            {
                Array.Copy(stateLow, 0, curlScratchpadLow, 0, CurlStateLength);
                Array.Copy(stateHigh, 0, curlScratchpadHigh, 0, CurlStateLength);
                for (var curlStateIndex = 0; curlStateIndex < CurlStateLength; curlStateIndex++)
                {
                    var alpha = curlScratchpadLow[curlScratchpadIndex];
                    var beta = curlScratchpadHigh[curlScratchpadIndex];
                    var gamma = curlScratchpadHigh[curlScratchpadIndex += curlScratchpadIndex < 365 ? 364 : -365];
                    var delta = (alpha | ~gamma) & (curlScratchpadLow[curlScratchpadIndex] ^ beta);
                    stateLow[curlStateIndex] = ~delta;
                    stateHigh[curlStateIndex] = (alpha ^ gamma) | delta;
                }
            }
        }

        private int Increase(ulong[] stateLow, ulong[] stateHigh, int index, int length)
        {
            for (int i = 0; i < length; i++)
            {
                ulong low = stateLow[index + i];
                ulong high = stateHigh[index + i];

                stateLow[index + i] = high ^ low;
                stateHigh[index + i] = low;

                if ((high & (~low)) == 0)
                    return length;
            }

            return length + 1;
        }

        private void SearchPrepareTrits(sbyte[] state, int offset)
        {
            for (int i = 0; i < CurlStateLength; i++)
            {
                switch (state[i])
                {
                    case -1:
                        _stateLow[i] = ulong.MaxValue;
                        _stateHigh[i] = ulong.MinValue;
                        break;
                    case 0:
                        _stateLow[i] = ulong.MaxValue;
                        _stateHigh[i] = ulong.MaxValue;
                        break;
                    case 1:
                        _stateLow[i] = ulong.MinValue;
                        _stateHigh[i] = ulong.MaxValue;
                        break;
                    default:
                        throw new InvalidCastException($"Invalid Trit: {state[i]}");
                }
            }

            Offset(offset, 0);
        }

        private void Offset(int offset, int value)
        {
            int longSize = sizeof(long) * 8;
            int numOffsetTrits = TritsOffset(longSize);

            int shift;
            int max = (int) Math.Pow(Radix, numOffsetTrits);

            if (value < 0)
            {
                shift = max + value % max;
            }
            else
            {
                shift = value % max;
            }

            ulong baseValue = Radix;
            int inShift = 0;

            for (int i = 0; i < numOffsetTrits; i++)
            {
                int numExpanded = (int) Math.Pow(Radix, i + 1);
                int outShift = numExpanded / Radix;
                if (shift != 0)
                {
                    int m = shift % numExpanded;
                    inShift += m;
                    shift -= m;
                }

                if (inShift < longSize)
                {
                    _stateHigh[offset + i] = (baseValue >> (inShift % numExpanded));
                }

                int j = numExpanded - inShift % numExpanded;
                while (j < longSize)
                {
                    _stateHigh[offset + i] |= baseValue.RotateLeft(j);
                    j += numExpanded;
                }

                _stateLow[offset + i] = _stateHigh[offset + i] >> outShift;

                j -= outShift;

                int k = j;
                if (j >= longSize)
                    k = j - numExpanded;

                if (k < 0)
                {
                    _stateLow[offset + i] |= baseValue.RotateRight(-k);
                }
                else
                {
                    _stateLow[offset + i] |= baseValue.RotateLeft(k);
                }

                baseValue |= baseValue.RotateLeft(numExpanded * 2 / 3);
                baseValue |= baseValue.RotateLeft(numExpanded / 3 * 2);
                inShift += outShift;
            }
        }

        private int TritsOffset(int longSize)
        {
            // get the number of trits needed for a full set of offset binary coded trits
            int o = 1;
            while (Math.Pow(Radix, o) < longSize)
            {
                o++;
            }

            return o;
        }
    }
}
