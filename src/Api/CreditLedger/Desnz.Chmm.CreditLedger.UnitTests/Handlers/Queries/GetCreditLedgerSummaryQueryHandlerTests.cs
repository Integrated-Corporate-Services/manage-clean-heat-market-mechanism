using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Constants;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Handlers.Queries;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Common.Extensions;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries;

public class GetCreditLedgerSummaryQueryHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<BaseRequestHandler<GetCreditLedgerSummaryQuery, ActionResult<CreditLedgerSummaryDto>>>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;

    private readonly GetCreditLedgerSummaryQueryHandler _handler;

    private static readonly Guid _existingOrganisationId = Guid.NewGuid();
    private static readonly Guid _pendingOrganisationId = Guid.NewGuid();
    private static readonly Guid _existingUserId = Guid.NewGuid();
    private HttpObjectResponse<List<LicenceHolderLinkDto>> _getLicenceHoldersResponse =
        new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderLinkDto>
        {
        new LicenceHolderLinkDto
        {
            Id = Guid.NewGuid(),
            LicenceHolderId = Guid.NewGuid(),
            OrganisationId = _existingOrganisationId
        },
        new LicenceHolderLinkDto
        {
            Id = Guid.NewGuid(),
            LicenceHolderId = Guid.NewGuid(),
            OrganisationId = _existingOrganisationId
        }
        }, null);
    private Mock<ISchemeYearService> _mockSchemeYearService;

    public GetCreditLedgerSummaryQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<BaseRequestHandler<GetCreditLedgerSummaryQuery, ActionResult<CreditLedgerSummaryDto>>>>();

        // User Service
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var mockCurrentUser = GetMockManufacturerUser(_existingUserId, _existingOrganisationId);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

        _mockSchemeYearService = new Mock<ISchemeYearService>();
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        // Organisation Service
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        var activeOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Active
        }, null);
        _mockOrganisationService.Setup(x => x.GetStatus(_existingOrganisationId)).ReturnsAsync(activeOrganisation);

        // Credit ledger repository
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);

        // Transaction repository
        _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);

        // Licence holder service
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_existingOrganisationId)).ReturnsAsync(_getLicenceHoldersResponse);

        // Calculator
        var mockLogger = new Mock<ILogger<CreditLedgerCalculator>>();
        var installationCreditCalculator = new CreditLedgerCalculator(
            mockLogger.Object);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new GetCreditLedgerSummaryQueryHandler(
            _mockLogger.Object,
            _mockCreditLedgerRepository.Object,
            _mockTransactionRepository.Object,
            _mockLicenceHolderService.Object,
            installationCreditCalculator,
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
    {
        // Arrange
        var missingOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockOrganisationService.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(missingOrganisation);

        var query = new GetCreditLedgerSummaryQuery(Guid.NewGuid(), SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
    {
        // Arrange
        var pendingOrganisation = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
        {
            Status = OrganisationConstants.Status.Pending
        }, null);
        _mockOrganisationService.Setup(x => x.GetStatus(_pendingOrganisationId)).ReturnsAsync(pendingOrganisation);

        var query = new GetCreditLedgerSummaryQuery(_pendingOrganisationId, SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_LicenceHoldersNotFound()
    {
        // Arrange
        var getLicenceHolderResponse = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(It.IsAny<Guid>())).ReturnsAsync(getLicenceHolderResponse);

        var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ShouldReturnZeroBroughtForward_When_NoTransactionExists()
    {
        // Arrange
        _mockCreditLedgerRepository.Setup(x => x.GetInstallationCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(new List<InstallationCredit>());
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(0);

        _mockTransactionRepository.Setup(x => x.GetTransactions(_existingOrganisationId, SchemeYearConstants.Id, It.IsAny<bool>())).ReturnsAsync(new List<Transaction>());

        var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Value.CreditsBoughtForward);
    }

    [Fact]
    public async Task ShouldReturnZeroRedeemed_When_NoTransactionExists()
    {
        // Arrange
        _mockCreditLedgerRepository.Setup(x => x.GetInstallationCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(new List<InstallationCredit>());
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(0);

        _mockTransactionRepository.Setup(x => x.GetTransactions(_existingOrganisationId, SchemeYearConstants.Id, It.IsAny<bool>())).ReturnsAsync(new List<Transaction>());

        var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Value.CreditsRedeemed);
    }

    [Fact]
    public async Task ShouldReturnCorrectResults_When_Ok()
    {
        // Arrange
        var (date, _) = DateTime.Now;
        _mockCreditLedgerRepository.Setup(x => x.GetInstallationCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(
            new List<InstallationCredit>
            {
                new InstallationCredit(_getLicenceHoldersResponse.Result.First().Id, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(_getLicenceHoldersResponse.Result.First().Id, 1, SchemeYearConstants.Id, date, 50, false),
                new InstallationCredit(_getLicenceHoldersResponse.Result.First().Id, 1, SchemeYearConstants.Id, date, 100, true),
                new InstallationCredit(_getLicenceHoldersResponse.Result.First().Id, 1, SchemeYearConstants.Id, date, 100, true)
            });
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(
            50 + 50 + 100 + 100
            );

        _mockTransactionRepository.Setup(x => x.GetTransactions(_existingOrganisationId, SchemeYearConstants.Id, It.IsAny<bool>())).ReturnsAsync(
            new List<Transaction>
            {
                // Transfer In
                new Transaction(SchemeYearConstants.Id, Guid.NewGuid(), _existingOrganisationId, 30, Guid.NewGuid(), DateTime.Now),
                // Transfer Out
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, Guid.NewGuid(), 20, Guid.NewGuid(), DateTime.Now),
                // Carry Over
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 10, Guid.NewGuid(), CreditLedgerConstants.TransactionType.CarriedOverFromPreviousYear, DateTime.Now),
                // Admin adjustments
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 500, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, 380, Guid.NewGuid(), CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.Now),
                // Redemption
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -480, Guid.NewGuid(), CreditLedgerConstants.TransactionType.Redeemed, DateTime.Now),
                // Carry over
                new Transaction(SchemeYearConstants.Id, _existingOrganisationId, -120, Guid.NewGuid(), CreditLedgerConstants.TransactionType.CarriedOverToNextYear, DateTime.Now)
            });

        var query = new GetCreditLedgerSummaryQuery(_existingOrganisationId, SchemeYearConstants.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(100, result.Value.CreditsGeneratedByHeatPumps);
        Assert.Equal(200, result.Value.CreditsGeneratedByHybridHeatPumps);
        Assert.Equal(30-20, result.Value.CreditsTransferred);
        Assert.Equal(10, result.Value.CreditsBoughtForward);
        Assert.Equal(2, result.Value.CreditAmendments.Count);
        Assert.Equal(500, result.Value.CreditAmendments.First().Credits);
        Assert.Equal(380, result.Value.CreditAmendments.Last().Credits);

        Assert.Equal(1200, result.Value.CreditBalance);

        Assert.Equal(-480, result.Value.CreditsRedeemed);

        Assert.Equal(-120, result.Value.CreditsCarriedForward);
        Assert.Equal(-600, result.Value.CreditsExpired);
    }
}
