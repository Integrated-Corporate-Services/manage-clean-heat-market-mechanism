using Amazon.S3;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Services.FileServiceTests
{
    public class FileServiceGetFileNamesTests
    {
        private readonly Mock<ILogger<FileService>> _mockFileServiceLogger;
        private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
        private readonly Mock<IClamAvService> _mockClamAvService;

        private static readonly string testBucketName = "my-s3-bucket";
        private static readonly string testEnvironmentName = "dev";
        private static readonly string fullTestBucketName = $"chmm-{testEnvironmentName}-{testBucketName}";

        public FileServiceGetFileNamesTests()
        {
            _mockFileServiceLogger = new Mock<ILogger<FileService>>();
            _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
            _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig() { EnvironmentName = testEnvironmentName });
            _mockClamAvService = new Mock<IClamAvService>();
        }

        [Fact]
        internal async Task Get_File_Names_Returns_File_Name_List()
        {
            var orgId = Guid.NewGuid().ToString();
            var yearId = Guid.NewGuid().ToString();

            var fileList = new List<string> { "File1.csv", "File2.csv", "File3.csv" };
            var keyList = fileList.Select(f => $"{orgId}/{yearId}/{f}").ToList();

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetAllObjectKeysAsync(It.Is<string>(s => s == fullTestBucketName), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .ReturnsAsync(() => keyList);

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.GetFileNamesAsync(testBucketName, orgId);

            Assert.Equal(3, actionResult.Count);
            Assert.True(fileList.SequenceEqual(actionResult));
        } 
    }
}
