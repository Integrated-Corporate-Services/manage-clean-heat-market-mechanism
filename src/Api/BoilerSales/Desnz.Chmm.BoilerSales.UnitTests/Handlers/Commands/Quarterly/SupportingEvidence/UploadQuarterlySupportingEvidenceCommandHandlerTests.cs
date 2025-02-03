using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Common.Services.FileService;
using Xunit;
using FluentAssertions;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Testing.Common;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Quarterly.SupportingEvidence;

public class UploadQuarterlySupportingEvidenceCommandHandlerTests : TestClaimsBase
{
    private Mock<ILogger<UploadQuarterlySupportingEvidenceCommandHandler>> _mockLogger;
    private Mock<ICurrentUserService> _mockUserService;
    private Mock<IOrganisationService> _mockOrganisationService;
    private Mock<IFileService> _mockFileService;

    private readonly UploadQuarterlySupportingEvidenceCommandHandler _handler;

    public UploadQuarterlySupportingEvidenceCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UploadQuarterlySupportingEvidenceCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            new Mock<ISchemeYearService>(MockBehavior.Strict).Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new UploadQuarterlySupportingEvidenceCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotfound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        var fileNames = new List<string>() { "test.csv" };

        var command = new UploadQuarterlySupportingEvidenceCommand
        (
            organisationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<IFormFile>() { GetFormFile() }
        );
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_ThereAreFileValidationErrors()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = Guid.NewGuid();
        var fileName = "test.csv";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new UploadQuarterlySupportingEvidenceCommand
        (
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            new List<IFormFile>() { GetFormFile() }
        );
        var expectedResult = Responses.BadRequest("Error");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.UploadFileAsync(Buckets.QuarterlySupportingEvidence, $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}/{fileName}", It.IsAny<IFormFile>())).ReturnsAsync(new FileUploadResponse(null, null, "Error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnAllQuarterlyVerificationStatementFileNames_When_FilesExist()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = Guid.NewGuid();
        var fileName = "test.csv";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new UploadQuarterlySupportingEvidenceCommand
        (
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            new List<IFormFile>() { GetFormFile() }
        );
        var expectedResult = Responses.Ok();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.UploadFileAsync(Buckets.QuarterlySupportingEvidence, $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}/{fileName}", It.IsAny<IFormFile>())).ReturnsAsync(new FileUploadResponse(null, null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    private IFormFile GetFormFile()
    {
        var content = "Test file content.";
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", "test.csv")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain",
        };
        return formFile;
    }
}
