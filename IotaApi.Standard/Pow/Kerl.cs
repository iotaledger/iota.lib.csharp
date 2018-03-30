using System;
using Iota.Api.Standard.Exception;
using Iota.Api.Standard.Utils;
using Org.BouncyCastle.Crypto.Digests;

namespace Iota.Api.Standard.Pow
{
    /// <summary>
    /// </summary>
    public class Kerl : Sponge
    {
        private const int BitHashLength = 384;
        private const int ByteHashLength = BitHashLength / 8;

        private readonly byte[] _byteState;

        private readonly KeccakDigest _keccak;
        private readonly int[] _tritState;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Kerl()
        {
            _keccak = new KeccakDigest(BitHashLength);
            _byteState = new byte[ByteHashLength];
            _tritState = new int[HashLength];
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            _keccak.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ICurl Clone()
        {
            return new Kerl();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override void Absorb(int[] trits, int offset, int length)
        {
            if (length % 243 != 0)
                throw new IllegalStateException("Illegal length: " + length);

            do
            {
                //copy trits[offset:offset+length]
                Array.Copy(trits, offset, _tritState, 0, HashLength);

                //convert to bits
                _tritState[HashLength - 1] = 0;
                FixedBigIntConverter.FromTritsToBytes(_tritState, _byteState);
                //BigIntConverter.BytesFromBigInt(
                //    BigIntConverter.BigIntFromTrits(_tritState, 0, HashLength),
                //    _byteState, 0, ByteHashLength);

                //run keccak
                _keccak.BlockUpdate(_byteState, 0, _byteState.Length);
                offset += HashLength;
            } while ((length -= HashLength) > 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="trits"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="T:Iota.Lib.CSharp.Api.Exception.IllegalStateException"></exception>
        public override void Squeeze(int[] trits, int offset, int length)
        {
            if (length % 243 != 0) throw new IllegalStateException("Illegal length: " + length);

            do
            {
                _keccak.DoFinal(_byteState, 0);

                //convert to trits
                FixedBigIntConverter.FromBytesToTrits(_byteState, _tritState);
                //BigIntConverter.TritsFromBigInt(
                //    BigIntConverter.BigIntFromBytes(_byteState, 0, ByteHashLength), 
                //    _tritState, 0, HashLength);

                //copy with offset
                _tritState[HashLength - 1] = 0;
                Array.Copy(_tritState, 0, trits, offset, HashLength);

                //calculate hash again
                for (var i = _byteState.Length; i-- > 0;) _byteState[i] = (byte) (_byteState[i] ^ 0xFF);

                _keccak.BlockUpdate(_byteState, 0, _byteState.Length);
                offset += HashLength;
            } while ((length -= HashLength) > 0);
        }
    }
}