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
using static Desnz.Chmm.Common.Constants.IdentityConstants;
using static Desnz.Chmm.Common.Services.FileService;
using Xunit;
using FluentAssertions;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Annual.SupportingEvidence;

public class DeleteAnnualSupportingEvidenceCommandHandlerTests : TestClaimsBase
{
    private Mock<ILogger<DeleteAnnualSupportingEvidenceCommandHandler>> _mockLogger;
    private Mock<ICurrentUserService> _mockUserService;
    private Mock<IFileService> _mockFileService;
    private Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly HttpObjectResponse<SchemeYearDto> _schemeYear;

    private readonly DeleteAnnualSupportingEvidenceCommandHandler _handler;

    public DeleteAnnualSupportingEvidenceCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DeleteAnnualSupportingEvidenceCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        _schemeYear = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            EndDate = new DateOnly(2025, 1, 1)
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(_schemeYear);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new DeleteAnnualSupportingEvidenceCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationNotFound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var httpContext = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        var fileName = "test.csv";

        var command = new DeleteAnnualSupportingEvidenceCommand
        (
            organisationId,
            Guid.NewGuid(),
            fileName
        );
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(httpContext);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnAllAnnualVerificationStatementFileNames_When_FilesExist()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var fileName = "test.csv";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new DeleteAnnualSupportingEvidenceCommand
        (
            organisationId,
            schemeYearId,
            fileName
        );
        var expectedResult = Responses.Ok();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser); ;
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.DeleteObjectNonVersionedBucketAsync(Buckets.AnnualSupportingEvidence, $"{organisationId}/{schemeYearId}/{fileName}")).ReturnsAsync(new FileUploadResponse(null, null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
