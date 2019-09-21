using System;
using System.Diagnostics.CodeAnalysis;
using IotaSharp.Exception;
using IotaSharp.Utils;
using Org.BouncyCastle.Crypto.Digests;

namespace IotaSharp.Pow
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Kerl : Sponge
    {
        private const int BIT_HASH_LENGTH = 384;
        private const int BYTE_HASH_LENGTH = BIT_HASH_LENGTH / 8;

        private readonly KeccakDigest _keccak;
        private readonly byte[] _byteState;

        private readonly sbyte[] _tritState;

        /// <summary>
        /// 
        /// </summary>
        public Kerl()
        {
            _keccak = new KeccakDigest(BIT_HASH_LENGTH);
            _byteState = new byte[BYTE_HASH_LENGTH];
            _tritState = new sbyte[HASH_LENGTH];
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
            if (length % 243 != 0)
                throw new IllegalStateException("Illegal length: " + length);

            do
            {
                Array.Copy(trits, offset, _tritState, 0, HASH_LENGTH);

                //convert to bits
                _tritState[HASH_LENGTH - 1] = 0;
                FixedBigIntConverter.FromTritsToBytes(_tritState, _byteState);
                
                //run keccak
                _keccak.BlockUpdate(_byteState, 0, _byteState.Length);
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
            if (length % 243 != 0) throw new IllegalStateException("Illegal length: " + length);

            do
            {
                _keccak.DoFinal(_byteState, 0);

                //convert to trits
                FixedBigIntConverter.FromBytesToTrits(_byteState, _tritState);

                //copy with offset
                _tritState[HASH_LENGTH - 1] = 0;
                Array.Copy(_tritState, 0, trits, offset, HASH_LENGTH);

                //calculate hash again
                for (var i = _byteState.Length; i-- > 0;) _byteState[i] = (byte)(_byteState[i] ^ 0xFF);

                _keccak.BlockUpdate(_byteState, 0, _byteState.Length);
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
            _keccak.Reset();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ICurl Clone()
        {
            return new Kerl();
        }
    }
}
