using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Handlers.Queries.Annual.VerificationStatement;
using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Queries.Annual;

public class GetAnnualVerificationStatementFileNamesQueryHandlerTests : TestClaimsBase
{
    private Mock<ILogger<GetAnnualVerificationStatementFileNamesQueryHandler>> _mockLogger;
    private Mock<ICurrentUserService> _mockUserService;
    private Mock<IOrganisationService> _mockOrganisationService;
    private Mock<IFileService> _mockFileService;

    private readonly GetAnnualVerificationStatementFileNamesQueryHandler _handler;

    public GetAnnualVerificationStatementFileNamesQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetAnnualVerificationStatementFileNamesQueryHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            new Mock<ISchemeYearService>(MockBehavior.Strict).Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new GetAnnualVerificationStatementFileNamesQueryHandler(
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

        var query = new GetAnnualVerificationStatementFileNamesQuery
        (
            organisationId,
            Guid.NewGuid()
        );
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().BeNull();
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnAllAnnualVerificationStatementFileNames_When_FilesExist()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), new OrganisationStatusDto(), null);
        var fileNames = new List<string>() { "test.csv" };

        var query = new GetAnnualVerificationStatementFileNamesQuery
        (
            organisationId,
            Guid.NewGuid()
        );
        var expectedResult = new List<string>() { "test.csv" };

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.AnnualVerificationStatement, $"{organisationId}/{query.SchemeYearId}")).ReturnsAsync(fileNames);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(expectedResult);
        result.Result.Should().BeNull();
    }
}
