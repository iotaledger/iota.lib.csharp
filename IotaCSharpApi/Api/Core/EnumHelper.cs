using System;
using System.ComponentModel;
using System.Reflection;

namespace Iota.Lib.CSharp.Api.Core
{
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetCommandString(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
}