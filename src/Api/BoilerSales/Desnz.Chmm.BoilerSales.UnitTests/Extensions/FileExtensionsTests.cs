using Desnz.Chmm.BoilerSales.Api.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Extensions
{
    public class FileExtensionsTests
    {
        public FileExtensionsTests(){}

        [Fact]
        public void BuildObjectKeyForAnnualBoilerSales_ShouldComposeTheObjectKeyForAnnualBoilerSalesUpload_When_CalledOnAnIFormFile()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();
            var fileName = "test.csv";
            var file = GetFormFile(fileName);

            var expectedResult = $"{organisationId}/{schemeYearId}/{fileName}";

            // Act
            var result = file.BuildObjectKeyForAnnualBoilerSales(organisationId, schemeYearId);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void BuildObjectKeyForAnnualBoilerSales_ShouldComposeTheObjectKeyForAnnualBoilerSalesUpload_When_CalledOnAnString()
        {
            // Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();
            var fileName = "test.csv";

            var expectedResult = $"{organisationId}/{schemeYearId}/{fileName}";

            // Act
            var result = fileName.BuildObjectKeyForAnnualBoilerSales(organisationId, schemeYearId);

            // Assert
            result.Should().Be(expectedResult);
        }

        private IFormFile GetFormFile(string fileName)
        {
            var content = "Test file content.";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain",
            };
            return formFile;
        }
    }
}
