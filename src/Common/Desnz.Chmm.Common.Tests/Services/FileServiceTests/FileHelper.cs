using Microsoft.AspNetCore.Http;
using System.Text;

namespace Desnz.Chmm.Common.Tests.Services.FileServiceTests
{
    internal static class FileHelper
    {
        internal static IFormFile CreateTestFormFile(string fileName, int? targetSize = null)
        {
            // Create a test file with some content
            var content = "Test file content.";
            if (targetSize.HasValue)
            {
                content = GenerateLargeString(targetSize.Value);
            }

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Create an IFormFile with the provided file name and stream
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain",
            };

            return formFile;
        }

        private static string GenerateLargeString(int targetSizeInBytes)
        {
            const int chunkSize = 1000; // You can adjust this based on your needs
            char[] chunk = new char[chunkSize];
            StringBuilder stringBuilder = new StringBuilder(targetSizeInBytes);

            // Use Parallel.For to generate chunks of characters in parallel
            Parallel.For(0, (targetSizeInBytes / chunkSize) + 1, i =>
            {
                // Generate a chunk of characters
                for (int j = 0; j < chunkSize; j++)
                {
                    // You can customize the logic to generate characters as needed
                    chunk[j] = 'A';
                }

                // Append the chunk to the StringBuilder in a thread-safe manner
                lock (stringBuilder)
                {
                    stringBuilder.Append(chunk);
                }
            });

            // Trim the StringBuilder to the desired size
            return stringBuilder.ToString().Substring(0, targetSizeInBytes);
        }
    }
}
