using Microsoft.AspNetCore.Http;
using System.Dynamic;
using System.Reflection;

namespace Desnz.Chmm.Common.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Return a copy of the object, but with IFormFiles replaced by their file names
        /// </summary>
        /// <param name="sourceObject">Object to copy</param>
        /// <returns>The source object, but with files replaced with file names</returns>
        public static ExpandoObject CopyObjectWithFileName(this object sourceObject)
        {
            if (sourceObject == null) return null;

            ExpandoObject destinationObject = new ExpandoObject();

            foreach (PropertyInfo property in sourceObject.GetType().GetProperties())
            {
                object? value = property.GetValue(sourceObject);

                // Check if the property type is IFormFile or List<IFormFile>
                if (typeof(IFormFile).IsAssignableFrom(property.PropertyType))
                {
                    value = (value as IFormFile)?.FileName;
                }
                else if (property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                         typeof(IFormFile).IsAssignableFrom(property.PropertyType.GetGenericArguments()[0]))
                {
                    value = (value as List<IFormFile>)?.Select(file => file.FileName).ToList();
                }

                ((IDictionary<string, object>)destinationObject)[property.Name] = value;
            }

            return destinationObject;
        }
    }
}
