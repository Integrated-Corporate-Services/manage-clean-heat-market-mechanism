using Amazon.S3;
using Amazon.S3.Model;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using static Desnz.Chmm.Common.Services.FileService;

namespace Desnz.Chmm.Common.Tests.Services.FileServiceTests
{
    public class FileServiceDeleteTests
    {
        private readonly Mock<ILogger<FileService>> _mockFileServiceLogger;
        private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
        private readonly Mock<IClamAvService> _mockClamAvService;

        private static readonly string quarantinedFileName = "Quarantined.docx";
        private static readonly string testFileName = "TestFile.csv";
        private static readonly string testBucketName = "my-s3-bucket";
        private static readonly string testEnvironmentName = "dev";
        private static readonly string fullTestBucketName = $"chmm-{testEnvironmentName}-{testBucketName}";
        private static readonly AmazonS3Exception fileNotFoundExecption = new AmazonS3Exception("Error", Amazon.Runtime.ErrorType.Receiver, "Error", "1", System.Net.HttpStatusCode.NotFound);

        public FileServiceDeleteTests()
        {
            _mockFileServiceLogger = new Mock<ILogger<FileService>>();
            _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
            _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig() { EnvironmentName = testEnvironmentName });
            _mockClamAvService = new Mock<IClamAvService>();
        }

        [Fact]
        internal async Task Deleting_File_That_Does_Not_Exist_Returns_Failure_Message()
        {
            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(fileNotFoundExecption);

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.DeleteObjectNonVersionedBucketAsync(testBucketName, testFileName);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal("The selected file does not exists", actionResult.ValidationError);
        }

        [Fact]
        internal async Task Deleting_File_That_Exists_Returns_Null_FileUploadResponse_Details()
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = fullTestBucketName,
                Key = testFileName
            };

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetObjectMetadataResponse());
            amazonS3.Setup(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new DeleteObjectResponse());

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.DeleteObjectNonVersionedBucketAsync(testBucketName, testFileName);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            amazonS3.Verify(s => s.DeleteObjectAsync(It.Is<DeleteObjectRequest>(s => s.BucketName == deleteRequest.BucketName && s.Key == deleteRequest.Key), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Null(actionResult.ValidationError);
        }

        [Fact]
        internal async Task Deleting_Quarantined_Returns_Failure_Message()
        {
            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetObjectMetadataResponse());

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.DeleteObjectNonVersionedBucketAsync(testBucketName, quarantinedFileName);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal("Deleting Quarantined.docx is not allowed.", actionResult.ValidationError);
        }

        [Fact]
        internal async Task Deleting_File_Handles_Exception()
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = fullTestBucketName,
                Key = testFileName
            };

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetObjectMetadataResponse());
            amazonS3.Setup(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception("Error"))
                .Verifiable();

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.DeleteObjectNonVersionedBucketAsync(testBucketName, testFileName);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            amazonS3.Verify(s => s.DeleteObjectAsync(It.Is<DeleteObjectRequest>(s => s.BucketName == deleteRequest.BucketName && s.Key == deleteRequest.Key), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("Error deleteting object at key: TestFile.csv from bucket: chmm-dev-my-s3-bucket", actionResult.ValidationError);
        }

    }
}