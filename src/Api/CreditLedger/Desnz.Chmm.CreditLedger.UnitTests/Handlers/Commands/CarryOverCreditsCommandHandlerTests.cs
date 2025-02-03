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
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Constants;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Commands;

public class CarryOverCreditsCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<CarryOverCreditsCommandHandler>> _mockLogger;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
    private readonly Mock<IObligationService> _mockObligationService;

    private readonly CarryOverCreditsCommandHandler _handler;

    private static readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid _schemeYearId = SchemeYearConstants.Id;
    private HttpObjectResponse<List<LicenceHolderDto>> _licenceHoldersData;
    private HttpObjectResponse<List<LicenceHolderLinkDto>> _licenceHolderLinkData;

    public CarryOverCreditsCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<CarryOverCreditsCommandHandler>>();
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
        _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
        _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);

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
        _licenceHoldersData = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderDto>
        {
            new()
            {
                Id = licence1Id,
                McsManufacturerId = 1,
                Name = "Test",
            },
            new()
            {
                Id = licence2Id,
                McsManufacturerId = 2,
                Name = "Test 2",
            }
        }, null);
        _licenceHolderLinkData = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderLinkDto>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                StartDate = SchemeYearConstants.StartDate,
                EndDate = null,
                LicenceHolderId = licence1Id,
                LicenceHolderName = "Test",
                OrganisationId = _organisationId,
                OrganisationName = "Test"
            },
            new ()
            {
                Id = Guid.NewGuid(),
                StartDate = SchemeYearConstants.StartDate,
                EndDate = null,
                LicenceHolderId = licence2Id,
                LicenceHolderName = "Test 2",
                OrganisationId = _organisationId,
                OrganisationName = "Test"
            }
        }, null);

        var obligationData = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
        {
            new(_organisationId, 100)
        }, null);

        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearData);
        _mockSchemeYearService.Setup(x => x.GetNextSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(nextSchemeYearData);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(schemeYearConfigurationData);
        _mockLicenceHolderService.Setup(x => x.GetAll(It.IsAny<string>())).ReturnsAsync(_licenceHoldersData);
        _mockLicenceHolderService.Setup(x => x.GetAllLinks(It.IsAny<string>())).ReturnsAsync(_licenceHolderLinkData);
        _mockTransactionRepository.Setup(x => x.GetAllTransactions(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>());
        _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(obligationData);

        var _dateTimeProvider = new DateTimeOverrideProvider();

        var _installationCreditCalculator = new CreditLedgerCalculator(new Mock<ILogger<CreditLedgerCalculator>>().Object);


        _handler = new CarryOverCreditsCommandHandler(
            _mockLogger.Object,
            _dateTimeProvider,
            _mockSchemeYearService.Object,
            _mockLicenceHolderService.Object,
            _mockObligationService.Object,
            _mockTransactionRepository.Object,
            _mockCreditLedgerRepository.Object,
            _installationCreditCalculator
            );
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AlreadyRun()
    {
        // Arrange
        _mockTransactionRepository.Setup(x => x.GetAllTransactions(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), 1, Guid.NewGuid(), "Test", DateTime.UtcNow)
        });
        var expectedResult = Responses.BadRequest($"Carry Forward Credit is already generated for some organisations for the Scheme Year Id: {_schemeYearId}");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_SchemeYearIsNotFound()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {_schemeYearId}, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_SchemeYearConfigurationIsNotFound()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get Obligation Calculations for Scheme Year with Id: {_schemeYearId}");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_NextSchemeYearIsNotFound()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetNextSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get next Scheme Year for Scheme Year with Id: {_schemeYearId}, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_LicenceHoldersAreNotFound()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockLicenceHolderService.Setup(x => x.GetAll(It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest("Failed to get all Licence Holders, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task LoadData_ShouldReturnBadRequest_When_ObligationsAreNotFound()
    {
        // Arrange
        var httpResponse = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var expectedResult = Responses.BadRequest($"Failed to get Obligation Totals for Scheme Year with Id: {_schemeYearId}, problem: null");

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("100", 80, "10")]
    [InlineData("85", 80, "5")]
    [InlineData("7600", 2600, "760")]
    [InlineData("80", 100, null)]
    public async Task GenerateTransactions(string credits, int obligation, string? expectedCarryOver)
    {
        var licenceHolderId = _licenceHoldersData.Result.First().Id;
        var creditList = new List<OrganisationLicenceHolderCreditsDto> { new OrganisationLicenceHolderCreditsDto(licenceHolderId, _organisationId, decimal.Parse(credits)) };
        // Arrange
        _mockCreditLedgerRepository.Setup(x => x.SumCreditsByLicenceHolderAndOrganisation(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditList);
        _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(_schemeYearId, It.IsAny<string>())).ReturnsAsync(
            new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK),
                    new() { new ObligationTotalDto(_organisationId, obligation) }, null));
        var transactions = new List<Transaction>();
        _mockTransactionRepository.Setup(x => x.GetTransactions(_schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);
        _mockTransactionRepository.Setup(c => c.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        if (string.IsNullOrEmpty(expectedCarryOver))
        {
            _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => x.Count == 0), true), Times.Once);
            return;
        }

        _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => x.Count == 2), true), Times.Once);

        var expectedValue = decimal.Parse(expectedCarryOver);
        _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => x.Any(i => i.Entries.Any(e => e.Value == expectedValue))), true), Times.Once);
    }

    [Fact]
    public async Task SpecificDefect()
    {
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();
        var org3 = Guid.NewGuid();
        var licenceHolderId = _licenceHoldersData.Result.First().Id;
        var creditList = new List<OrganisationLicenceHolderCreditsDto> { 
            new OrganisationLicenceHolderCreditsDto(licenceHolderId, _organisationId, 7512M) };

        // Arrange
        _mockCreditLedgerRepository.Setup(x =>
        x.SumCreditsByLicenceHolderAndOrganisation(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditList);
        _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(_schemeYearId, It.IsAny<string>())).ReturnsAsync(
            new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK),
                    new() { 
                        new ObligationTotalDto(_organisationId, 2600),
                        new ObligationTotalDto(org1, 500),
                        new ObligationTotalDto(org2, 2500),
                        new ObligationTotalDto(org3, 800)
                    }, null));
        var transactions = new List<Transaction>
        {
            new Transaction(SchemeYearConstants.Id, org1, org2,    1500, Guid.NewGuid(), DateTime.Now),
            new Transaction(SchemeYearConstants.Id, org3, _organisationId,    150, Guid.NewGuid(), DateTime.Now),
            new Transaction(SchemeYearConstants.Id,  _organisationId, org2,   100, Guid.NewGuid(), DateTime.Now),
            new Transaction(SchemeYearConstants.Id, _organisationId, 38, org1, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);
        _mockTransactionRepository.Setup(c => c.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new CarryOverCreditsCommand(_schemeYearId), CancellationToken.None);

        // Assert
        _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => 
        x.Count(i => i.Entries.Any(x => x.OrganisationId == _organisationId)) == 2), true), Times.Once);

        var expectedValue = 760M;
        _mockTransactionRepository.Verify(x => x.AddTransactions(It.Is<List<Transaction>>(x => x.Any(i => i.Entries.Any(e => e.Value == expectedValue))), true), Times.Once);
    }
}