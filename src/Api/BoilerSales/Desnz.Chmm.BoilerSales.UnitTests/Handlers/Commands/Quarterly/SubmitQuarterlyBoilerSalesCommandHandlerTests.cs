using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Commands.Quarterly;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
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
using static Desnz.Chmm.BoilerSales.Constants.BoilerSalesConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Commands.Quarterly;

public class SubmitQuarterlyBoilerSalesCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<SubmitQuarterlyBoilerSalesCommandHandler>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;

    private readonly SubmitQuarterlyBoilerSalesCommandHandler _handler;

    public SubmitQuarterlyBoilerSalesCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<SubmitQuarterlyBoilerSalesCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockFileService = new Mock<IFileService>(MockBehavior.Strict);
        _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>(MockBehavior.Strict);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            EndDate = SchemeYearConstants.EndDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate,
            Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { 
                Id = SchemeYearConstants.QuarterOneId,
                EndDate = SchemeYearConstants.QuarterOneEndDate
            } }
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

        _handler = new SubmitQuarterlyBoilerSalesCommandHandler(
            _mockLogger.Object,
            _mockFileService.Object,
            _mockQuarterlyBoilerSalesRepository.Object,
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
        var schemeYearQuarterId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
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
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Pending
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
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
    public async Task ShouldReturnBadRequest_When_OnQuarterEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
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
    public async Task ShouldReturnBadRequest_When_29DaysAfterQuarterEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = Guid.NewGuid();
        var schemeYearQuarterId = Guid.NewGuid();
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate.AddDays(29));

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
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
    public async Task ShouldReturnBadRequest_When_SubmitQuarterlyObligation_Fails()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Create(It.IsAny<QuarterlyBoilerSales>(), false)).ReturnsAsync(Guid.NewGuid());
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AlreadySubmittedValues()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.BadRequest();

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Create(It.IsAny<QuarterlyBoilerSales>(), false)).ReturnsAsync(Guid.NewGuid());
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(organisationId, schemeYearId, schemeYearQuarterId, 12,12,null)
            });

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


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
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {schemeYearId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_Invalid_SchemeYearQuarterId()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = Guid.NewGuid();

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year Quarter for Scheme Year with Id: {schemeYearId}, problem: null");

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_OneDayAfterEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate.AddDays(1));

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.Created(Guid.NewGuid());

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Create(It.IsAny<QuarterlyBoilerSales>(), false)).ReturnsAsync(Guid.NewGuid());
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_28DaysAfterEndDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        _dateTimeProvider.OverrideDate(SchemeYearConstants.QuarterOneEndDate.AddDays(28));

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.Created(Guid.NewGuid());

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Create(It.IsAny<QuarterlyBoilerSales>(), false)).ReturnsAsync(Guid.NewGuid());
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnCreatedResult_When_Valid()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearId = SchemeYearConstants.Id;
        var schemeYearQuarterId = SchemeYearConstants.QuarterOneId;

        var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);

        var prefix = $"{organisationId}/{schemeYearId}/{schemeYearQuarterId}";
        var mockCurrentUser = GetMockManufacturerUser(Guid.NewGuid(), organisationId);
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);

        var command = new SubmitQuarterlyBoilerSalesCommand
        {
            OrganisationId = organisationId,
            SchemeYearId = schemeYearId,
            SchemeYearQuarterId = schemeYearQuarterId,
            Gas = 5_000,
            Oil = 6_000
        };
        var expectedResult = (ActionResult<Guid>)Responses.Created(Guid.NewGuid());

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        _mockFileService.Setup(x => x.GetFileNamesAsync(Buckets.QuarterlySupportingEvidence, prefix)).ReturnsAsync(new List<string> { "testse.csv" });
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Create(It.IsAny<QuarterlyBoilerSales>(), false)).ReturnsAsync(Guid.NewGuid());
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>?>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

        var httpResponse2 = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
        _mockObligationService.Setup(x => x.SubmitQuarterlyObligation(It.IsAny<CreateQuarterlyObligationCommand>())).Returns(Task.FromResult(httpResponse2));


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
