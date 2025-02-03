using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Common.Services.FileService;
using Xunit;
using FluentAssertions;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Testing.Common;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Quarterly.SupportingEvidence;

public class DeleteQuarterlySupportingEvidenceCommandHandlerTests : TestClaimsBase
{
    private Mock<ILogger<DeleteQuarterlySupportingEvidenceCommandHandler>> _mockLogger;
    private Mock<ICurrentUserService> _mockUserService;
    private Mock<IFileService> _mockFileService;
    private Mock<IOrganisationService> _mockOrganisationService;

    private readonly DeleteQuarterlySupportingEvidenceCommandHandler _handler;

    public DeleteQuarterlySupportingEvidenceCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DeleteQuarterlySupportingEvidenceCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            new Mock<ISchemeYearService>(MockBehavior.Strict).Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new DeleteQuarterlySupportingEvidenceCommandHandler(
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
        var fileName = "test.csv";

        var command = new DeleteQuarterlySupportingEvidenceCommand
        (
            organisationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            fileName
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

        var command = new DeleteQuarterlySupportingEvidenceCommand
        (
            organisationId,
            schemeYearId,
            schemeYearQuarterId,
            fileName
        );
        var expectedResult = Responses.Ok();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.QuarterlySupportingEvidence, $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}/{fileName}")).ReturnsAsync(new FileUploadResponse(null, null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
