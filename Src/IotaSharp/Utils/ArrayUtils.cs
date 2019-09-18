using System;
using System.Collections.Generic;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ArrayUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static IEnumerable<T> SliceRow<T>(T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] data, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            T[] result = new T[endIndex - startIndex];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray2<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
