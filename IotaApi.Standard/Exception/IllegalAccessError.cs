namespace Iota.Api.Exception
{
    /// <summary>
    ///     This exception occurs when certain core API calls on the node are disabled
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IllegalAccessError : System.Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public IllegalAccessError()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public IllegalAccessError(string s):base(s)
        {

        }
    }
}