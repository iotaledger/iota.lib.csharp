namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Helper class that extracts the command string corresponding to the different <see cref="Command"/>s
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetCommandString(this Command en)
        {
            var cs = en.ToString();
            return cs.Substring(0, 1).ToLower() + cs.Substring(1);
        }
    }
}