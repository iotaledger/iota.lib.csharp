using System;

namespace IotaSharp.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeStamp
    {
        internal static long Now()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}
