using Desnz.Chmm.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Amazon.S3.Model;
using static Desnz.Chmm.Common.Services.FileService;
using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.Common.Tests.Services.FileServiceTests
{
    public class FileServiceUploadTests
    {
        private readonly Mock<ILogger<FileService>> _mockFileServiceLogger;
        private Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
        private Mock<IClamAvService> _mockClamAvService;

        private static readonly string _testFileName = "TestFile.csv";
        private static readonly string _testBucketName = "my-s3-bucket";
        private static readonly string _testEnvironmentName = "dev";
        private static readonly string _fullTestBucketName = $"chmm-{_testEnvironmentName}-{_testBucketName}";
        private static readonly AmazonS3Exception _fileNotFoundException = new AmazonS3Exception("Error", Amazon.Runtime.ErrorType.Receiver, "Error", "1", System.Net.HttpStatusCode.NotFound);

        public FileServiceUploadTests()
        {
            _mockFileServiceLogger = new Mock<ILogger<FileService>>();
            _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
            _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig() { EnvironmentName = _testEnvironmentName });
            _mockClamAvService = new Mock<IClamAvService>();
        }

        [Fact]
        internal async Task Uploading_Duplicate_File_Returns_Failure_Message()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetObjectMetadataResponse());

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object, 
                _mockFileServiceLogger.Object, 
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal("The selected file could not be uploaded – a file with the same name already exists", actionResult.ValidationError);
        }

        [Fact]
        internal async Task Uploading_Over_Max_File_Size_Returns_Failure_Message()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName, 6 * 1024 * 1024);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal("The selected file must be smaller than 5MB", actionResult.ValidationError);
        }

        [Fact]
        internal async Task Uploading_Invalid_File_Type_Returns_Failure_Message()
        {
            var invalidFileName = "testFile.txt";
            var file = FileHelper.CreateTestFormFile(invalidFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);

            var fileService = new FileService(
                _mockClamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, invalidFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal("The selected file must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, .bmp, .eml, or .msg", actionResult.ValidationError);
        }

        [Fact]
        internal async Task Uploading_Virus_File_Returns_Null_File_Key()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);

            var clamAvService = new Mock<IClamAvService>();
            clamAvService
                .Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(() => new ScanResult(_testFileName, new nClam.ClamScanResult($"{_testFileName}: FOUND")));

            var fileService = new FileService(
                clamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);


#if DEBUG
            Assert.Equal(string.Empty, actionResult.FileKey);
#else
            Assert.Null(actionResult.FileKey);
#endif
        }

        [Fact]
        internal async Task Uploading_Clean_File_Returns_Correct_File_Key()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);
            amazonS3.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new PutObjectResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var clamAvService = new Mock<IClamAvService>();
            clamAvService
                .Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(() => new ScanResult(_testFileName, new nClam.ClamScanResult($"{_testFileName}: OK")));

            var fileService = new FileService(
                clamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal(_testFileName, actionResult.FileKey);
        }

        [Fact]
        internal async Task Uploading_Clean_File_Returns_Empty_File_Key_If_Upload_Fails()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);
            amazonS3.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new PutObjectResponse() { HttpStatusCode = System.Net.HttpStatusCode.BadRequest });

            var clamAvService = new Mock<IClamAvService>();
            clamAvService
                .Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(() => new ScanResult(_testFileName, new nClam.ClamScanResult($"{_testFileName}: OK")));

            var fileService = new FileService(
                clamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal(string.Empty, actionResult.FileKey);
        }


        [Fact]
        internal async Task Uploading_File_HandlesException()
        {
            var file = FileHelper.CreateTestFormFile(_testFileName);

            var amazonS3 = new Mock<IAmazonS3>();
            amazonS3.Setup(s => s.GetObjectMetadataAsync(It.Is<string>(s => s == _fullTestBucketName), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(_fileNotFoundException);
            amazonS3.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception("Error"))
                .Verifiable();

            var clamAvService = new Mock<IClamAvService>();
            clamAvService
                .Setup(x => x.ScanAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(() => new ScanResult(_testFileName, new nClam.ClamScanResult($"{_testFileName}: OK")));

            var fileService = new FileService(
                clamAvService.Object,
                amazonS3.Object,
                _mockFileServiceLogger.Object,
                _mockOptionsGovukEnvironmentConfig.Object);

            var actionResult = await fileService.UploadFileAsync(_testBucketName, _testFileName, file);

            Assert.NotNull(actionResult);
            Assert.IsType<FileUploadResponse>(actionResult);
            Assert.Equal(_testFileName, actionResult.FileKey);
            Assert.Equal("Error uploading object to bucket: chmm-dev-my-s3-bucket at key: TestFile.csv", actionResult.ValidationError);
        }
    }
}
