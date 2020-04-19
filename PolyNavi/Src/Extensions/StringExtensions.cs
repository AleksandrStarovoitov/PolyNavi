using System;
using System.Linq;

namespace PolyNavi.Extensions
{
    internal static class StringExtensions
    {
        internal static string FirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException("Empty string", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        }
    }
}
