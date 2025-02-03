using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desnz.Chmm.Identity.UnitTests.Helpers
{
    internal static class FileHelper
    {
        public static IFormFile CreateFormFile(string fileName)
        {
            var content = "File content goes here.";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.Length).Returns(stream.Length);
            file.Setup(f => f.OpenReadStream()).Returns(stream);
            return file.Object;
        }
    }
}
