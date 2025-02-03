using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Annual;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Annual;

public class SubmitAnnualBoilerSalesCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<SubmitAnnualBoilerSalesCommandHandler>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly SubmitAnnualBoilerSalesCommandHandler _handler;

    public SubmitAnnualBoilerSalesCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<SubmitAnnualBoilerSalesCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);
        _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>(MockBehavior.Strict);
        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            EndDate = SchemeYearConstants.EndDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate,
            BoilerSalesSubmissionEndDate = SchemeYearConstants.BoilerSalesSubmissionEndDate
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

        _handler = new SubmitAnnualBoilerSalesCommandHandler(
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
        var schemeYearId = SchemeYearConstants.Id;
        var httpContext = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

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
        var schemeYearId = SchemeYearConstants.Id;
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Pending
        }, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
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
        var schemeYearId = SchemeYearConstants.Id;
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.EndDate.AddDays(-1));

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AfterBoilerSalesSubmissionEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.BoilerSalesSubmissionEndDate.AddDays(1));

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AnnualBoilerSalesAlreadyExist()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var prefix = $"{organisationId}/{schemeYearId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), false, false, false)).ReturnsAsync(GetMockAnnualBoilerSales());
        
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
        var schemeYearId = SchemeYearConstants.Id;
        var prefix = $"{organisationId}/{schemeYearId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var boilerSalesId = Guid.NewGuid();

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var expectedResult = Responses.BadRequest($"Failed to create yearly obligation for organisation with Id: {organisationId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.AnnualVerificationStatement, prefix)).ReturnsAsync(new List<string> { "testvs.csv" });
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.AnnualSupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockAnnualBoilerSalesRepository.Setup(x => x.Create(It.IsAny<AnnualBoilerSales>(), true)).ReturnsAsync(boilerSalesId);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), false, false, false)).ReturnsAsync((AnnualBoilerSales?)null);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Delete(It.IsAny<AnnualBoilerSales>(), true)).Returns(Task.CompletedTask);
        var mockSubmitAnnualObligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockObligationService.Setup(x => x.SubmitAnnualObligation(It.IsAny<CreateAnnualObligationCommand>())).Returns(Task.FromResult(mockSubmitAnnualObligationResponse));
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().BeEmpty();
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_Valid()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var prefix = $"{organisationId}/{schemeYearId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var boilerSalesId = Guid.NewGuid();

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
   
        var expectedResult = (ActionResult<Guid>)Responses.Created(boilerSalesId);

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.AnnualVerificationStatement, prefix)).ReturnsAsync(new List<string> { "testvs.csv" });
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.AnnualSupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockAnnualBoilerSalesRepository.Setup(x => x.Create(It.IsAny<AnnualBoilerSales>(), true)).ReturnsAsync(boilerSalesId);
        _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), false, false, false)).ReturnsAsync((AnnualBoilerSales?)null);
        var mockSubmitAnnualObligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
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
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitAnnualBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {schemeYearId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    private AnnualBoilerSales GetMockAnnualBoilerSales()
    {
        return new AnnualBoilerSales(Guid.NewGuid(), Guid.NewGuid(), 0, 0, new List<AnnualBoilerSalesFile>());
    }

}
