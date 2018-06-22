using System;
using System.Collections.Generic;

namespace Iota.Api.Standard.Utils
{
    public class ArrayUtils
    {
        public static IEnumerable<T> SliceRow<T>(T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        public static T[] SubArray<T>(T[] data, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            T[] result = new T[endIndex - startIndex];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }

        public static T[] SubArray2<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Pads an array of type int with zeros until a given size is reached.
        /// </summary>
        /// <param name="oldArray">The array.</param>
        /// <param name="newLength">The new length.</param>
        /// <returns>An int array of the new size.</returns>
        public static int[] PadArrayWithZeros(int[] oldArray, int newLength)
        {
            if (oldArray.Length > newLength)
            {
                throw new ArgumentException("The desired length must be larger then the size of the array");
            }
            int[] newArray = new int[newLength];
            Array.Copy(oldArray, newArray, oldArray.Length);

            int index = oldArray.Length;
            while (index < newLength)
            {
                newArray[index] = 0;
                index++;
            }
            return newArray;
        }

        /// <summary>
        /// Fills a trit-array with approriate zeroes so that  the length can be divided by the RADIX
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <returns>The filled trit-array</returns>
        public static int[] PadTritArrayWithZeroes(int[] trits)
        {
            List<int> list = new List<int>(trits);
            switch (list.Count % Constants.RADIX)
            {
                case 0:
                    break;
                case 1:
                    list.Add(0);
                    list.Add(0);
                    break;
                case 2:
                    list.Add(0);
                    break;
            }

            return list.ToArray();
        }
    }
}
