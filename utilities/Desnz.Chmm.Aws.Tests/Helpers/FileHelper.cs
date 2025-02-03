using Microsoft.AspNetCore.Http;
using System.Text;

namespace Desnz.Chmm.Aws.Tests.Helpers
{
    internal static class FileHelper
    {
        public static IFormFile CreateTestFormFile(string fileName)
        {
            // Create a test file with some content
            var content = "Test file content.";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Create an IFormFile with the provided file name and stream
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain",
            };

            return formFile;
        }
    }
}
