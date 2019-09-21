using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace IotaSharp.Pow
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Curl : Sponge
    {
        private const int STATE_LENGTH = 3 * HASH_LENGTH;
        private const int NUMBER_OF_ROUNDSP81 = 81;
        private const int NUMBER_OF_ROUNDSP27 = 27;

        private static readonly sbyte[] TruthTable = {1, 0, -1, 2, 1, -1, 0, 2, -1, 1, 0};
        private readonly int _numberOfRounds;
        private readonly sbyte[] _scratchpad = new sbyte[STATE_LENGTH];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public Curl(SpongeFactory.Mode mode)
        {
            switch (mode)
            {
                case SpongeFactory.Mode.CURLP81:
                    _numberOfRounds = NUMBER_OF_ROUNDSP81;
                    break;
                case SpongeFactory.Mode.CURLP27:
                    _numberOfRounds = NUMBER_OF_ROUNDSP27;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Only Curl-P-27 and Curl-P-81 are supported.");

            }

            State = new sbyte[STATE_LENGTH];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override ICurl Absorb(sbyte[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(trits, offset, State, 0, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override sbyte[] Squeeze(sbyte[] trits, int offset, int length)
        {
            do
            {
                Array.Copy(State, 0, trits, offset, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);

            return trits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ICurl Reset()
        {
            Array.Clear(State, 0, State.Length);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ICurl Clone()
        {
            if (_numberOfRounds == NUMBER_OF_ROUNDSP27)
                return new Curl(SpongeFactory.Mode.CURLP27);

            if (_numberOfRounds == NUMBER_OF_ROUNDSP81)
                return new Curl(SpongeFactory.Mode.CURLP81);

            return null;
        }

        private void Transform()
        {
            var scratchpadIndex = 0;

            for (var round = 0; round < _numberOfRounds; round++)
            {
                Array.Copy(State, 0, _scratchpad, 0, STATE_LENGTH);
                for (var stateIndex = 0; stateIndex < STATE_LENGTH; stateIndex++)
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
    }
}
