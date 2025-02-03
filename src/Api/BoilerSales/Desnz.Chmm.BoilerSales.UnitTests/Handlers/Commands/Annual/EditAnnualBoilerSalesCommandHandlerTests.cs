using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Annual;

public class EditAnnualBoilerSalesCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<EditAnnualBoilerSalesCommandHandler>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IBoilerSalesFileCopyService> _mockFileService;
    private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly EditAnnualBoilerSalesCommandHandler _handler;

    public EditAnnualBoilerSalesCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EditAnnualBoilerSalesCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IBoilerSalesFileCopyService>(MockBehavior.Strict);
        _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>(MockBehavior.Strict);
        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            EndDate = SchemeYearConstants.EndDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate,
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        _dateTimeProvider = new DateTimeOverrideProvider();
        var date = SchemeYearConstants.EndDate.AddDays(1); // OK date is a day after the end date
        _dateTimeProvider.OverrideDate(date);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new EditAnnualBoilerSalesCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            _mockAnnualBoilerSalesRepository.Object,
            _mockObligationService.Object,
            _dateTimeProvider,
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

        var command = new EditAnnualBoilerSalesCommand
        (
             organisationId,
             schemeYearId,
             5_000,
             6_000
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
    public async Task ShouldReturnBadRequest_When_BeforeEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.EndDate.AddDays(-1));

        var command = new EditAnnualBoilerSalesCommand
        (
             organisationId,
             schemeYearId,
             5_000,
             6_000
        );
        var expectedResult = Responses.BadRequest($"Cannot adjust annual boiler sales figures outside adjustment window for Scheme Year with Id: {SchemeYearConstants.Id}");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AfterSurrenderDayDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddDays(1));

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );
        var expectedResult = Responses.BadRequest($"Cannot adjust annual boiler sales figures outside adjustment window for Scheme Year with Id: {SchemeYearConstants.Id}");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
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

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );
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

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );

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
    public async Task ShouldReturnOk_If_BoilerSalesAreApproved()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );

        var expectedResult = Responses.Ok();

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);
        mockBoilerSales.Approve();

        var prefix = $"{organisationId}/{schemeYearId}";
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Update(mockBoilerSales, true)).Returns(Task.CompletedTask);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.ConcludeEditing(organisationId, schemeYearId)).ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingSession(new List<string>(), new List<string>(), new List<string>()));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), false, false, false)).ReturnsAsync((AnnualBoilerSales?)null);
        var mockSubmitAnnualObligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitAnnualObligation(It.IsAny<CreateAnnualObligationCommand>())).Returns(Task.FromResult(mockSubmitAnnualObligationResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnOk_If_Valid_Approved()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );

        var expectedResult = Responses.Ok();

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);
        mockBoilerSales.Approve();

        var prefix = $"{organisationId}/{schemeYearId}";
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Update(mockBoilerSales, true)).Returns(Task.CompletedTask);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.ConcludeEditing(organisationId, schemeYearId)).ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingSession(new List<string>(), new List<string>(), new List<string>()));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), false, false, false)).ReturnsAsync((AnnualBoilerSales?)null);
        var mockSubmitAnnualObligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitAnnualObligation(It.IsAny<CreateAnnualObligationCommand>())).Returns(Task.FromResult(mockSubmitAnnualObligationResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_SubmitAnnualObligation_Fails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var prefix = $"{organisationId}/{schemeYearId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );
        var boilerSalesId = Guid.NewGuid();

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var expectedResult = Responses.BadRequest($"Failed to create yearly obligation for organisation with Id: {organisationId}, problem: null");

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.ConcludeEditing(organisationId, schemeYearId)).ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingSession(new List<string>(), new List<string>(), new List<string>()));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Update(mockBoilerSales, true)).Returns(Task.CompletedTask);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Delete(It.IsAny<AnnualBoilerSales>(), true)).Returns(Task.CompletedTask);
        var mockSubmitAnnualObligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockObligationService.Setup(x => x.SubmitAnnualObligation(It.IsAny<CreateAnnualObligationCommand>())).Returns(Task.FromResult(mockSubmitAnnualObligationResponse));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_Invalid_SchemeYearId()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var schemeYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );
        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {schemeYearId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearData);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnError_When_ConcludingS3FilesForEditing_Fails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new EditAnnualBoilerSalesCommand
        (
            organisationId,
            schemeYearId,
            5_000,
            6_000
        );

        var expectedResult = new ObjectResult("Error concluding S3 files for editing: exception: ERROR")
        {
            StatusCode = 500
        };

        var mockBoilerSales = new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 1, 1, null);
        mockBoilerSales.Approve();

        var prefix = $"{organisationId}/{schemeYearId}";
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(Task.FromResult<AnnualBoilerSales?>(mockBoilerSales));
        _mockAnnualBoilerSalesRepository.Setup(x => x.Update(mockBoilerSales, true)).Returns(Task.CompletedTask);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.ConcludeEditing(organisationId, schemeYearId)).ReturnsAsync(new BoilerSalesFileCopyService.ConcludedEditingSession(new List<string>(), new List<string>(), new List<string> { "ERROR"}));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
}
