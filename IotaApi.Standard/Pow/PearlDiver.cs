using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iota.Api.Standard.Pow
{
    public class PearlDiver
    {
        private enum State
        {
            RUNNING,
            CANCELLED,
            COMPLETED
        }

        private const int TRANSACTION_LENGTH = 8019;
        private const int CURL_HASH_LENGTH = 243;
        private const int CURL_STATE_LENGTH = CURL_HASH_LENGTH * 3;

        private const long HIGH_BITS = -1;
        private const long LOW_BITS = 0;

        private readonly object SyncObj = new object();
        private State state;

        public void Cancel()
        {
            lock (SyncObj)
            {
                state = State.CANCELLED;
            }
        }

        public bool Search(int[] transactionTrits, int minWeightMagnitude, int numberOfThreads)
        {
            ValidateParameters(transactionTrits, minWeightMagnitude);

            lock(SyncObj)
            {
                state = State.RUNNING;
            }

            long[] midStateLow = new long[CURL_STATE_LENGTH];
            long[] midStateHigh = new long[CURL_STATE_LENGTH];

            InitializeMidCurlStates(transactionTrits, midStateLow, midStateHigh);

            if (numberOfThreads <= 0)
            {
                double procsAvail = numberOfThreads * 8 / 10;
                numberOfThreads = Convert.ToInt32(Math.Max(1.0, Math.Floor(procsAvail)));
            }

            List<Task> workers = new List<Task>(numberOfThreads);
            while (numberOfThreads-- > 0)
            {
                long[] midStateCopyLow = midStateLow.Clone() as long[];
                long[] midStateCopyHigh = midStateHigh.Clone() as long[];
                Task worker = new Task(() => Perform(numberOfThreads, transactionTrits, minWeightMagnitude, midStateCopyLow, midStateCopyHigh));
                workers.Add(worker);
                worker.Start();
            }
            foreach(Task worker in workers)
            {
                try
                {
                    worker.Wait();
                }
                catch (AggregateException)
                {
                    lock(SyncObj)
                    {
                        state = State.CANCELLED;
                    }
                }
            }
            return state == State.COMPLETED;
        }

        private void ValidateParameters(int[] transactionTrits, int minWeightMagnitude)
        {
            if (transactionTrits.Length != TRANSACTION_LENGTH)
            {
                throw new ArgumentException("Invalid transaction trits length: " + transactionTrits.Length);
            }
            if (minWeightMagnitude < 0 || minWeightMagnitude > CURL_HASH_LENGTH)
            {
                throw new ArgumentException("Invalid min weight magnitude: " + minWeightMagnitude);
            }
        }

        private static void InitializeMidCurlStates(int[] transactionTrits, long[] midStateLow, long[] midStateHigh)
        {
            for (int i = CURL_HASH_LENGTH; i < CURL_STATE_LENGTH; i++)
            {
                midStateLow[i] = HIGH_BITS;
                midStateHigh[i] = HIGH_BITS;
            }

            int offset = 0;
            long[] curlScratchpadLow = new long[CURL_STATE_LENGTH];
            long[] curlScratchpadHigh = new long[CURL_STATE_LENGTH];
            for (int i = (TRANSACTION_LENGTH - CURL_HASH_LENGTH) / CURL_HASH_LENGTH; i-- > 0;)
            {

                for (int j = 0; j < CURL_HASH_LENGTH; j++)
                {
                    switch (transactionTrits[offset++])
                    {
                        case 0:
                            midStateLow[j] = HIGH_BITS;
                            midStateHigh[j] = HIGH_BITS;
                            break;
                        case 1:
                            midStateLow[j] = LOW_BITS;
                            midStateHigh[j] = HIGH_BITS;
                            break;
                        default:
                            midStateLow[j] = HIGH_BITS;
                            midStateHigh[j] = LOW_BITS;
                            break;
                    }
                }
                Transform(midStateLow, midStateHigh, curlScratchpadLow, curlScratchpadHigh);
            }

            for (int i = 0; i < 162; i++)
            {
                switch (transactionTrits[offset++])
                {
                    case 0:
                        midStateLow[i] = HIGH_BITS;
                        midStateHigh[i] = HIGH_BITS;
                        break;
                    case 1:
                        midStateLow[i] = LOW_BITS;
                        midStateHigh[i] = HIGH_BITS;
                        break;
                    default:
                        midStateLow[i] = HIGH_BITS;
                        midStateHigh[i] = LOW_BITS;
                        break;
                }
            }

            unchecked
            {
                midStateLow[162 + 0] = (long)0b1101101101101101101101101101101101101101101101101101101101101101L;
                midStateHigh[162 + 0] = (long)0b1011011011011011011011011011011011011011011011011011011011011011L;
                midStateLow[162 + 1] = (long)0b1111000111111000111111000111111000111111000111111000111111000111L;
                midStateHigh[162 + 1] = (long)0b1000111111000111111000111111000111111000111111000111111000111111L;
                midStateLow[162 + 2] = (long)0b0111111111111111111000000000111111111111111111000000000111111111L;
                midStateHigh[162 + 2] = (long)0b1111111111000000000111111111111111111000000000111111111111111111L;
                midStateLow[162 + 3] = (long)0b1111111111000000000000000000000000000111111111111111111111111111L;
                midStateHigh[162 + 3] = (long)0b0000000000111111111111111111111111111111111111111111111111111111L; 
            }
        }

        private void Perform(int threadIndex, int[] transactionTrits, int minWeightMagnitude, long[] midStateCopyLow, long[] midStateCopyHigh)
        {
            for (int i = 0; i < threadIndex; i++)
            {
                Increment(midStateCopyLow, midStateCopyHigh, 162 + CURL_HASH_LENGTH / 9,
                    162 + (CURL_HASH_LENGTH / 9) * 2);
            }

            long[] stateLow = new long[CURL_STATE_LENGTH];
            long[] stateHigh = new long[CURL_STATE_LENGTH];

            long[] scratchpadLow = new long[CURL_STATE_LENGTH];
            long[] scratchpadHigh = new long[CURL_STATE_LENGTH];

            int maskStartIndex = CURL_HASH_LENGTH - minWeightMagnitude;
            long mask = 0;
            while (state == State.RUNNING && mask == 0)
            {

                Increment(midStateCopyLow, midStateCopyHigh, 162 + (CURL_HASH_LENGTH / 9) * 2,
                    CURL_HASH_LENGTH);

                Copy(midStateCopyLow, midStateCopyHigh, stateLow, stateHigh);
                Transform(stateLow, stateHigh, scratchpadLow, scratchpadHigh);

                mask = HIGH_BITS;
                for (int i = maskStartIndex; i < CURL_HASH_LENGTH && mask != 0; i++)
                {
                    mask &= ~(stateLow[i] ^ stateHigh[i]);
                }
            }
            if (mask != 0)
            {
                lock (SyncObj)
                {
                    if (state == State.RUNNING)
                    {
                        state = State.COMPLETED;
                        long outMask = 1;
                        while ((outMask & mask) == 0)
                        {
                            outMask <<= 1;
                        }
                        for (int i = 0; i < CURL_HASH_LENGTH; i++)
                        {
                            transactionTrits[TRANSACTION_LENGTH - CURL_HASH_LENGTH + i] =
                                (midStateCopyLow[i] & outMask) == 0 ? 1 : (midStateCopyHigh[i] & outMask) == 0 ? -1 : 0;
                        }
                    }
                }
            }
        }

        private static void Copy(long[] srcLow, long[] srcHigh, long[] destLow, long[] destHigh)
        {
            Array.Copy(srcLow, 0, destLow, 0, CURL_STATE_LENGTH);
            Array.Copy(srcHigh, 0, destHigh, 0, CURL_STATE_LENGTH);
        }

        private static void Transform(long[] stateLow, long[] stateHigh, long[] scratchpadLow, long[] scratchpadHigh)
        {

            for (int round = 0; round < 81; round++)
            {
                Copy(stateLow, stateHigh, scratchpadLow, scratchpadHigh);

                int scratchpadIndex = 0;
                for (int stateIndex = 0; stateIndex < CURL_STATE_LENGTH; stateIndex++)
                {
                    long alpha = scratchpadLow[scratchpadIndex];
                    long beta = scratchpadHigh[scratchpadIndex];
                    if (scratchpadIndex < 365)
                    {
                        scratchpadIndex += 364;
                    }
                    else
                    {
                        scratchpadIndex += -365;
                    }
                    long gamma = scratchpadHigh[scratchpadIndex];
                    long delta = (alpha | (~gamma)) & (scratchpadLow[scratchpadIndex] ^ beta);

                    stateLow[stateIndex] = ~delta;
                    stateHigh[stateIndex] = (alpha ^ gamma) | delta;
                }
            }
        }

        private static void Increment(long[] midCurlStateCopyLow, long[] midCurlStateCopyHigh, int fromIndex, int toIndex)
        {
            for (var i = fromIndex; i < toIndex; i++)
                if (midCurlStateCopyLow[i] == LOW_BITS)
                {
                    midCurlStateCopyLow[i] = HIGH_BITS;
                    midCurlStateCopyHigh[i] = LOW_BITS;
                }
                else
                {
                    if (midCurlStateCopyHigh[i] == LOW_BITS)
                        midCurlStateCopyHigh[i] = HIGH_BITS;
                    else
                        midCurlStateCopyLow[i] = LOW_BITS;

                    break;
                }
        }

    }
}