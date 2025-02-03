using System.Text.RegularExpressions;

namespace Desnz.Chmm.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetFriendlyName(this string name, params string[] exclusions)
        {
            if (exclusions != null)
            {
                foreach (var exclusion in exclusions)
                {
                    if (name.EndsWith(exclusion, StringComparison.Ordinal))
                    {
                        name = name.Substring(0, name.Length - exclusion.Length);
                        break;
                    }
                }
            }

            // Add a space in front of each capital letter
            string result = Regex.Replace(name, "(?<!^)([A-Z])", " $1");

            // Make only the first letter capitalised
            result = string.Concat(
                result[..1].ToUpperInvariant(),
                result[1..].ToLowerInvariant());

            return result;
        }
    }
}
