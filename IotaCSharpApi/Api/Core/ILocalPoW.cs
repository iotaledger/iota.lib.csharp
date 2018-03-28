namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILocalPoW
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trytes"></param>
        /// <param name="minWeightMagnitude"></param>
        /// <returns></returns>
        string PerformPoW(string trytes, int minWeightMagnitude);
    }
}
