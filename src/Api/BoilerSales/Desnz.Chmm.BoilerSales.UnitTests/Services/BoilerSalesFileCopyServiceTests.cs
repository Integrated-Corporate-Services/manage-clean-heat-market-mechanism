using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Testing.Common;
using Moq;
using Xunit;
using static Desnz.Chmm.Common.Services.FileService;

namespace Desnz.Chmm.BoilerSales.UnitTests.Services;

public class BoilerSalesFileCopyServiceTests
{
    private readonly IBoilerSalesFileCopyService _service;
    private readonly Mock<IFileService> _mockFileService;

    public BoilerSalesFileCopyServiceTests()
    {
        _mockFileService = new Mock<IFileService>();
        _service = new BoilerSalesFileCopyService(_mockFileService.Object);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_PrepareForEditing_Annually()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        await _service.PrepareForEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_PrepareForEditing_Annually_ErrorDeletingfile()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null, "Error deleting S3 file"));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        var result = await _service.PrepareForEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_PrepareForEditing_Annually_ErrorCopyingfile()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null, "Error copying S3 file"));

        //Act
        var result = await _service.PrepareForEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_PrepareForEditing_Quarterly()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1"}));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        await _service.PrepareForEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()),Times.Exactly(1));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Annually()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        Assert.Single(result.VerificationStatements);
        Assert.Single(result.SupportingEvidences);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Annually_WhenDeletingS3File_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null, "Error"));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));

        Assert.Null(result.VerificationStatements);
        Assert.Null(result.SupportingEvidences);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Annually_WhenCopyingS3File_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null, "Error"));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

        Assert.Null(result.VerificationStatements);
        Assert.Null(result.SupportingEvidences);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Quarterly()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        Assert.Single(result.SupportingEvidences);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Quarterly_WhenDeletingS3File_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null, "Error"));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));

        Assert.Null(result.SupportingEvidences);
        Assert.Equal(1, result.Errors.Count);
    }

    [Fact]
    public async Task BoilerSalesFileCopyServiceTests_CanCall_ConcludeEditing_Quarterly_WhenCopyingS3File_Fails()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;
        var _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;

        _mockFileService.Setup(x => x.GetFileNamesAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "s1" }));
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileUploadResponse(null, null));
        _mockFileService.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FileCopyResponse(null, null, "Error"));

        //Act
        var result = await _service.ConcludeEditing(_organisationId, _schemeYearId, _schemeYearQuarterOneId);

        //Assert
        _mockFileService.Verify(x => x.DeleteObjectNonVersionedBucketAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        _mockFileService.Verify(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

        Assert.Null(result.SupportingEvidences);
        Assert.Equal(1, result.Errors.Count);
    }
}
