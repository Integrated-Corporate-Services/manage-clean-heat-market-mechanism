using System.Reflection;
using System.Text.RegularExpressions;

namespace Desnz.Chmm.Common.Extensions
{
    public static class TypeExtensions
    {
        private static readonly IDictionary<string, string> _auditFriendlyNames = new Dictionary<string, string>
        {
            { "Unit test", "Only for unit testing" }, // For unit testing only

            { "Add manufacturer note", "Add note" },
        };

        /// <summary>
        /// Returns the Name property of the type, adding a space in between each capital letter
        /// and removing any exclusion words from the end of the name.
        /// </summary>
        /// <param name="type">The type to get the friendly name from</param>
        /// <param name="exclusions">Any postfix exclusions</param>
        /// <returns>A friendly name for the given type</returns>
        public static string GetFriendlyName(this Type type, params string[] exclusions)
        {
            var name = type.Name;
            var unmapped = name.GetFriendlyName(exclusions);
            return _auditFriendlyNames.ContainsKey(unmapped) ? _auditFriendlyNames[unmapped] : unmapped;
        }

        /// <summary>
        /// Looks at the provided type and details if it has an attribute or not
        /// </summary>
        /// <typeparam name="TAttribute">The Attribute type to look for</typeparam>
        /// <param name="type">The type to interrogate</param>
        /// <returns>True if the attribute is found, False if not</returns>
        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return Attribute.IsDefined(type, typeof(TAttribute));
        }

        public static Dictionary<int, string> ToDictionary(this Type constantsType)
        {
            var fields = constantsType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var constants = fields.Where(f => f.IsLiteral && !f.IsInitOnly).ToList();
            var result = new Dictionary<int, string>();


            var values = constantsType.GetFields(BindingFlags.Static | BindingFlags.Public)
                                 .Where(x => x.IsLiteral && !x.IsInitOnly)
                                 .Select(x => x.GetValue(null)).Cast<string>();

            constants.ForEach(constant => result.Add((int)constant.GetValue(values), constant.Name));

            return result;
        }
    }
}
