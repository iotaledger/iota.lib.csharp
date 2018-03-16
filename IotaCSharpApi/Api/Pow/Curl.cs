using System;
using System.ComponentModel;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Pow
{
    /// <summary>
    ///     (c) 2016 Come-from-Beyond
    ///     Curl belongs to the sponge function family.
    /// </summary>
    public class Curl : Sponge
    {
        private const int StateLength = 3 * HashLength;

        private const int NumberOfRoundsP81 = 81;
        private const int NumberOfRoundsP27 = 27;

        private static readonly int[] TruthTable = {1, 0, -1, 2, 1, -1, 0, 2, -1, 1, 0};
        private readonly int _numberOfRounds;
        private readonly int[] _scratchpad = new int[StateLength];
        private readonly long[] _stateHigh;
        private readonly long[] _stateLow;

        /// <summary>
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="mode"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public Curl(bool pair, CurlMode mode)
        {
            switch (mode)
            {
                case CurlMode.CurlP27:
                    _numberOfRounds = NumberOfRoundsP27;
                    break;
                case CurlMode.CurlP81:
                    _numberOfRounds = NumberOfRoundsP81;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Only Curl-P-27 and Curl-P-81 are supported.");
            }

            if (pair)
            {
                _stateHigh = new long[StateLength];
                _stateLow = new long[StateLength];
                State = null;
                Set();
            }
            else
            {
                State = new int[StateLength];
                _stateHigh = null;
                _stateLow = null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="mode"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public Curl(CurlMode mode)
        {
            switch (mode)
            {
                case CurlMode.CurlP27:
                    _numberOfRounds = NumberOfRoundsP27;
                    break;
                case CurlMode.CurlP81:
                    _numberOfRounds = NumberOfRoundsP81;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Only Curl-P-27 and Curl-P-81 are supported.");
            }

            State = new int[StateLength];
            _stateHigh = null;
            _stateLow = null;
        }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        public int[] State { get; set; }

        /// <summary>
        ///     Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
        public override void Absorb(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(trits, offset, State, 0, length < HashLength ? length : HashLength);
                Transform();
                offset += HashLength;
            } while ((length -= HashLength) > 0);
        }

        private void Transform()
        {
            var scratchpadIndex = 0;

            for (var round = 0; round < _numberOfRounds; round++)
            {
                Array.Copy(State, 0, _scratchpad, 0, StateLength);
                for (var stateIndex = 0; stateIndex < StateLength; stateIndex++)
                {
                    var prevScratchpadIndex = scratchpadIndex;
                    if (scratchpadIndex < 365)
                        scratchpadIndex += 364;
                    else
                        scratchpadIndex += -365;

                    State[stateIndex] =
                        TruthTable[
                            _scratchpad[prevScratchpadIndex]
                            + (_scratchpad[scratchpadIndex] << 2)
                            + 5];
                }
            }
        }

        /// <summary>
        ///     Resets this state.
        /// </summary>
        /// <returns>
        ///     the ICurl instance (used for method chaining)
        /// </returns>
        public override void Reset()
        {
            for (var stateIndex = 0; stateIndex < StateLength; stateIndex++) State[stateIndex] = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public void Reset(bool pair)
        {
            if (pair)
                Set();
            else
                Reset();
        }

        /// <summary>
        ///     Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        ///     the squeezed trits
        /// </returns>
        public override void Squeeze(int[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(State, 0, trits, offset, length < HashLength ? length : HashLength);
                Transform();
                offset += HashLength;
            } while ((length -= HashLength) > 0);
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>a new instance</returns>
        public virtual ICurl Clone()
        {
            return new Curl(CurlMode.CurlP81);
        }

        /// <summary>
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Absorb(Tuple<long[], long[]> pair, int offset, int length)
        {
            int o = offset, l = length;
            do
            {
                Array.Copy(pair.Item1, o, _stateLow, 0, l < HashLength ? l : HashLength);
                Array.Copy(pair.Item2, o, _stateHigh, 0, l < HashLength ? l : HashLength);

                PairTransform();
                o += HashLength;
            } while ((l -= HashLength) > 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Tuple<long[], long[]> Squeeze(Tuple<long[], long[]> pair, int offset, int length)
        {
            int o = offset, l = length;
            var low = pair.Item1;
            var hi = pair.Item2;
            do
            {
                Array.Copy(_stateLow, 0, low, o, l < HashLength ? l : HashLength);
                Array.Copy(_stateHigh, 0, hi, o, l < HashLength ? l : HashLength);

                PairTransform();
                o += HashLength;
            } while ((l -= HashLength) > 0);

            return new Tuple<long[], long[]>(low, hi);
        }

        #region Private

        private void Set()
        {
            for (var i = 0; i < _stateLow.Length; i++) _stateLow[i] = (long) Converter.HIGH_LONG_BITS;

            for (var i = 0; i < _stateHigh.Length; i++) _stateHigh[i] = (long) Converter.HIGH_LONG_BITS;
        }

        private void PairTransform()
        {
            var curlScratchpadLow = new long[StateLength];
            var curlScratchpadHigh = new long[StateLength];
            var curlScratchpadIndex = 0;
            for (var round = _numberOfRounds; round-- > 0;)
            {
                Array.Copy(_stateLow, 0, curlScratchpadLow, 0, StateLength);
                Array.Copy(_stateHigh, 0, curlScratchpadHigh, 0, StateLength);
                for (var curlStateIndex = 0; curlStateIndex < StateLength; curlStateIndex++)
                {
                    var alpha = curlScratchpadLow[curlScratchpadIndex];
                    var beta = curlScratchpadHigh[curlScratchpadIndex];
                    var gamma = curlScratchpadHigh[curlScratchpadIndex += curlScratchpadIndex < 365 ? 364 : -365];
                    var delta = (alpha | ~gamma) & (curlScratchpadLow[curlScratchpadIndex] ^ beta);
                    _stateLow[curlStateIndex] = ~delta;
                    _stateHigh[curlStateIndex] = (alpha ^ gamma) | delta;
                }
            }
        }

        #endregion
    }
}