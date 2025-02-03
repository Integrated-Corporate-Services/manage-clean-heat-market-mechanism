using Desnz.Chmm.Common.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void CopyObjectWithFileName_SingleIFormFile_PropertiesCopiedCorrectly()
        {
            // Arrange
            var sourceObject = new TestClass
            {
                FileProperty = new FormFile(Stream.Null, 0, 0, "testFile", "test.txt")
            };

            // Act
            var result = sourceObject.CopyObjectWithFileName();

            // Assert
            Assert.IsType<ExpandoObject>(result);
            dynamic resultObject = result;
            Assert.Equal("test.txt", resultObject.FileProperty);
        }

        [Fact]
        public void CopyObjectWithFileName_ListOfIFormFiles_PropertiesCopiedCorrectly()
        {
            // Arrange
            var sourceObject = new TestClass
            {
                FileListProperty = new List<IFormFile>
            {
                new FormFile(Stream.Null, 0, 0, "file1", "file1.txt"),
                new FormFile(Stream.Null, 0, 0, "file2", "file2.txt")
            }
            };

            // Act
            var result = sourceObject.CopyObjectWithFileName();

            // Assert
            Assert.IsType<ExpandoObject>(result);
            dynamic resultObject = result;
            Assert.Equal(2, resultObject.FileListProperty.Count);
            Assert.Equal("file1.txt", resultObject.FileListProperty[0]);
            Assert.Equal("file2.txt", resultObject.FileListProperty[1]);
        }

        [Fact]
        public void CopyObjectWithFileName_NullSourceObject_ReturnsNullObject()
        {
            // Arrange
            TestClass sourceObject = null;

            // Act
            var result = sourceObject.CopyObjectWithFileName();

            // Assert
            Assert.Equal(null, result);
        }

        // Define a test class for testing purposes
        private class TestClass
        {
            public IFormFile FileProperty { get; set; }
            public List<IFormFile> FileListProperty { get; set; }
        }
    }
}
