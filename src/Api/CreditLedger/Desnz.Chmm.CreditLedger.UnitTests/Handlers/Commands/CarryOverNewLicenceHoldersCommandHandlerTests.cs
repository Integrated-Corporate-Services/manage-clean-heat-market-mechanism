using Desnz.Chmm.CreditLedger.Api.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Handlers.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Obligation.Common.Dtos;
using System.Linq.Expressions;
using System.Net;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Mediator;
using FluentAssertions;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Constants;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Commands;

public class CarryOverNewLicenceHoldersCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;

    private readonly Mock<ILogger<CarryOverNewLicenceHoldersCommandHandler>> _mockLogger;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
    private readonly Mock<IObligationService> _mockObligationService;
    private readonly Mock<CreditLedgerCalculator> _mockInstallationCreditCalculator;

    private readonly CarryOverNewLicenceHoldersCommandHandler _handler;

    private static readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private HttpObjectResponse<LicenceHolderExistsDto> _licenceHoldersData;
    private HttpObjectResponse<List<LicenceHolderLinkDto>> _licenceHolderLinkData;

    public CarryOverNewLicenceHoldersCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<CarryOverNewLicenceHoldersCommandHandler>>();
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
        _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);
        _mockInstallationCreditCalculator = new Mock<CreditLedgerCalculator>(MockBehavior.Strict);

        var schemeYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id
        }, null);

        var nextSchemeYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Year2025Id
        }, null);

        var schemeYearConfigurationData = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), new ObligationCalculationsDto
        {
            CreditCarryOverPercentage = 10.0m
        }, null);

        var licence1Id = Guid.NewGuid();
        var licence2Id = Guid.NewGuid();
        _licenceHoldersData = new HttpObjectResponse<LicenceHolderExistsDto>(new HttpResponseMessage(HttpStatusCode.OK), new LicenceHolderExistsDto
        {
            Exists = true
        }, null);

        var obligationData = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
        {
            new(_organisationId, 100)
        }, null);

        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearData);
        _mockSchemeYearService.Setup(x => x.GetNextSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(nextSchemeYearData);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(schemeYearConfigurationData);
        _mockLicenceHolderService.Setup(x => x.Exists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(_licenceHoldersData);
        _mockTransactionRepository.Setup(x => x.GetAllTransactions(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>());
        _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(obligationData);

        var _dateTimeProvider = new DateTimeOverrideProvider();

        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new CarryOverNewLicenceHoldersCommandHandler(
            _mockLogger.Object,
            _dateTimeProvider,
            _mockSchemeYearService.Object,
            _mockLicenceHolderService.Object,
            _mockTransactionRepository.Object,
            _mockCreditLedgerRepository.Object,
            _mockInstallationCreditCalculator.Object,
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_LicenceHolders_NotLoaded()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<LicenceHolderExistsDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockLicenceHolderService.Setup(x => x.Exists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest("Failed to get all Licence Holders, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(_schemeYearId, Guid.NewGuid(), Guid.NewGuid(), new DateOnly()), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_LicenceHolders_AreNotFound()
    {
        // Arrange
        var licenceHolderId = Guid.NewGuid();
        _licenceHoldersData.Result.Exists = false;
        _mockLicenceHolderService.Setup(x => x.Exists(licenceHolderId, It.IsAny<string>())).ReturnsAsync(_licenceHoldersData);

        var expectedResult = Responses.BadRequest($"Failed to get Licence Holder with Id: {licenceHolderId}");

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(_schemeYearId, licenceHolderId, Guid.NewGuid(), new DateOnly()), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_Organisation_IsNotFound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get Organisation with Id: {organisationId}, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), Guid.NewGuid(), new DateOnly()), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_SchemeYearIsNotFound()
    {
        // Arrange
        var schemeYearId = Guid.NewGuid();
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);

        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(previousSchemeYearId, It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {previousSchemeYearId}, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, new DateOnly()), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_CurrentSchemeYear_IsNotFound()
    {
        // Arrange
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);

        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);
        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.InternalServerError), null, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);

        var expectedResult = Responses.BadRequest($"Failed to get current Scheme Year, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, new DateOnly()), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_StartDate_IsNotEarlierThan_SurrenderDayDate()
    {
        // Arrange
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();

        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var sameDate = new DateOnly(2024, 1, 1);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            SurrenderDayDate = sameDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var expectedResult = Responses.NoContent();

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, sameDate), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_InstallationCredits_NotFound()
    {
        // Arrange
        var licenceHolderId = Guid.NewGuid();
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();

        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var startDate = new DateOnly(2024, 1, 1);
        var surrenderDayDate = new DateOnly(2024, 1, 2);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            SurrenderDayDate = surrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        //var credit = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2024, 1, 2), 50, false);
        var credit = new InstallationCredit(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<bool>());

        //_mockCreditLedgerRepository.Setup(m => m.GetAll(It.Is<Expression<Func<InstallationCredit, bool>>?>(y => y.Compile()(credit)),
        //                                               It.IsAny<bool>())).Returns(Task.FromResult(new List<InstallationCredit>() { credit }));

        _mockCreditLedgerRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<InstallationCredit, bool>>>(), false)).ReturnsAsync(new List<InstallationCredit>() );

        var expectedResult = Responses.NoContent();


        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, startDate), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_ObligationCalculations_NotFound()
    {
        // Arrange
        var licenceHolderId = Guid.NewGuid();
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();

        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var startDate = new DateOnly(2024, 1, 1);
        var surrenderDayDate = new DateOnly(2024, 1, 2);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            SurrenderDayDate = surrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var credit = new InstallationCredit(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<bool>());
        _mockCreditLedgerRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<InstallationCredit, bool>>>(), false)).ReturnsAsync(new List<InstallationCredit> { credit });

        var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);

        var expectedErrorMsg = $"Failed to get Obligation Calculations for Scheme Year with Id: {_schemeYearId}";
        var expectedResult = Responses.BadRequest(expectedErrorMsg);


        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, startDate), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_CarryOver_IsZero()
    {
        // Arrange
        var licenceHolderId = Guid.NewGuid();
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();

        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var startDate = new DateOnly(2024, 1, 1);
        var surrenderDayDate = new DateOnly(2024, 1, 2);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            SurrenderDayDate = surrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var credit = new InstallationCredit(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<bool>());
        _mockCreditLedgerRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<InstallationCredit, bool>>>(), false)).ReturnsAsync(new List<InstallationCredit> { credit });
        
        var obligationCalculations = new ObligationCalculationsDto();
        var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);

        _mockInstallationCreditCalculator.Setup(x => x.CalculateNewLicenceHoldersCarryOver(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(0m);

        var expectedResult = Responses.NoContent();

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, startDate), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_CarryOver_IsGreaterThanZero()
    {
        // Arrange
        var licenceHolderId = Guid.NewGuid();
        var previousSchemeYearId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();

        var organisationStatus = new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        };
        var getOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(), organisationStatus, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(getOrganisationResponse);

        var startDate = new DateOnly(2024, 1, 1);
        var surrenderDayDate = new DateOnly(2024, 1, 2);
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            SurrenderDayDate = surrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var credit = new InstallationCredit(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<bool>());
        _mockCreditLedgerRepository.Setup(r => r.GetAll(It.IsAny<Expression<Func<InstallationCredit, bool>>>(), false)).ReturnsAsync(new List<InstallationCredit> { credit });

        var obligationCalculations = new ObligationCalculationsDto();
        var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);

        _mockInstallationCreditCalculator.Setup(x => x.CalculateNewLicenceHoldersCarryOver(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(7.5m);

        _mockTransactionRepository.Setup(c => c.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        var expectedResult = Responses.NoContent();

        // Act
        var result = await _handler.Handle(new CarryOverNewLicenceHoldersCommand(organisationId, Guid.NewGuid(), previousSchemeYearId, startDate), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => x.Count == 2), true), Times.Once);
    }
}