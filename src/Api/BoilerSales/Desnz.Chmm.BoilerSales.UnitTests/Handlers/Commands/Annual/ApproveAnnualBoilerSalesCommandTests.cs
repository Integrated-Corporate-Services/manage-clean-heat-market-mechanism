using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Annual;

public class ApproveAnnualBoilerSalesCommandTests : TestClaimsBase
{
    private readonly Mock<ILogger<BaseRequestHandler<ApproveAnnualBoilerSalesCommand, ActionResult>>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly HttpObjectResponse<SchemeYearDto> _schemeYear;

    private readonly ApproveAnnualBoilerSalesCommandHandler _handler;

    public ApproveAnnualBoilerSalesCommandTests()
    {
        _mockLogger = new Mock<ILogger<BaseRequestHandler<ApproveAnnualBoilerSalesCommand, ActionResult>>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>(MockBehavior.Strict);
        _mockObligationService = new Mock<IObligationService>();
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

        _handler = new ApproveAnnualBoilerSalesCommandHandler(
            _mockLogger.Object,
            _mockAnnualBoilerSalesRepository.Object,
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var httpContext = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);

        var command = new ApproveAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId
        };
        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(httpContext);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Pending
        }, null);

        var command = new ApproveAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId
        };
        var expectedResult = Responses.BadRequest($"Organisation with Id: {organisationId} has an invalid status: {getOrganisationResponse.Result.Status}");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AnnualDataIsNotFound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new ApproveAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId
        };
        var expectedResult = Responses.NotFound($"Failed to get Annual boiler sales data for organisation with Id: {organisationId}");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult<AnnualBoilerSales?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_BoilerSalesStatusIsNotSubmitted()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new ApproveAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId
        };

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);
        mockBoilerSales.Approve();
        var expectedResult = Responses.BadRequest($"Annual boiler sales data for: {organisationId} has an invalid status: {mockBoilerSales.Status}");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public async Task ShouldCallApproveOnBoilerSalesAndReturnOk_If_Found()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new ApproveAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId
        };
        var expectedResult = Responses.Ok();

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1,1,null);

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));
        _mockAnnualBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var httpResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitAnnualObligation(It.IsAny<CreateAnnualObligationCommand>())).Returns(Task.FromResult(httpResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        Assert.Equal(BoilerSalesStatus.Approved, mockBoilerSales.Status);
        _mockAnnualBoilerSalesRepository.Verify(i => i.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
