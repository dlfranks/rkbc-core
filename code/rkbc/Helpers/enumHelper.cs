using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace rkbc.web.Helpers
{
    public static class EnumHelper
    {
        // Get the Name value of the Display attribute if the   
        // enum has one, otherwise use the value converted to title case.  
        public static string GetDisplayName<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            var attr = value.GetAttributeOfType<TEnum, DisplayAttribute>();
            return attr == null ? value.ToString().ToSpacedTitleCase() : attr.Name;
        }
        public static string GetDiscription<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            var attr = value.GetAttributeOfType<TEnum, DisplayAttribute>();
            return attr == null ? value.ToString().ToSpacedTitleCase() : attr.Description;
        }

        // Get the ShortName value of the Display attribute if the   
        // enum has one, otherwise use the value converted to title case.  
        public static string GetDisplayShortName<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            var attr = value.GetAttributeOfType<TEnum, DisplayAttribute>();
            return attr == null ? value.ToString().ToSpacedTitleCase() : attr.ShortName;
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        private static T GetAttributeOfType<TEnum, T>(this TEnum value)
            where TEnum : struct, IConvertible
            where T : Attribute
        {
            var result = value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttributes(false)
                        .OfType<T>()
                        .LastOrDefault();
            return result;
        }
        /// <summary>
        /// Converts camel case or pascal case to separate words with title case
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSpacedTitleCase(this string s)
        {
            //https://stackoverflow.com/a/155486/150342
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo
               .ToTitleCase(Regex.Replace(s,
                            "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 "));
        }
    }
}
