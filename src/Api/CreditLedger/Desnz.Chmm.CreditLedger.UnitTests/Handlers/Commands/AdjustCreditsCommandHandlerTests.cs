using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Handlers.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.CreditLedger.Constants;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using JWT;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries;

public class AdjustCreditsCommandHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<BaseRequestHandler<AdjustCreditsCommand, ActionResult>>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
    private readonly CreditLedgerCalculator _mockInstallationCreditCalculator;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly DateTimeOverrideProvider _datetimeProvider;
    private readonly AdjustCreditsCommandHandler _handler;

    private static readonly Guid _userId = Guid.NewGuid();
    private static readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid _schemeYearId = SchemeYearConstants.Id;

    // Different responses for organisation status
    private static HttpObjectResponse<OrganisationStatusDto> organisationNotFound = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
    private static HttpObjectResponse<OrganisationStatusDto> activeOrganisationFound = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
    {
        Status = OrganisationConstants.Status.Active
    }, null);
    private static HttpObjectResponse<OrganisationStatusDto> pendingOrganisationFound = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
    {
        Status = OrganisationConstants.Status.Pending
    }, null);

    private HttpObjectResponse<List<LicenceHolderLinkDto>> GetLicenceHoldersResponse =
        new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderLinkDto>
        {
            new LicenceHolderLinkDto
            {
                Id = Guid.NewGuid(),
                LicenceHolderId = Guid.NewGuid(),
                OrganisationId = _organisationId
            },
            new LicenceHolderLinkDto
            {
                Id = Guid.NewGuid(),
                LicenceHolderId = Guid.NewGuid(),
                OrganisationId = _organisationId
            }
        }, null);
    private readonly HttpObjectResponse<SchemeYearDto> _schemeYear;

    public AdjustCreditsCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<BaseRequestHandler<AdjustCreditsCommand, ActionResult>>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
        _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
        _mockInstallationCreditCalculator = new CreditLedgerCalculator(new Mock<ILogger<CreditLedgerCalculator>>().Object);
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);

        _datetimeProvider = new DateTimeOverrideProvider();

       _schemeYear = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            EndDate = SchemeYearConstants.EndDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate,
       }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(_schemeYear);

        var mockCurrentUser = GetMockManufacturerUser(_userId, _organisationId);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _mockSchemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new AdjustCreditsCommandHandler(
            _mockLogger.Object,
            _mockUserService.Object,
            _datetimeProvider,
            _mockTransactionRepository.Object,
            _mockLicenceHolderService.Object,
            _mockCreditLedgerRepository.Object,
            validator,
            _mockInstallationCreditCalculator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
    {
        // Arrange
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(organisationNotFound);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
    {
        // Arrange
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(pendingOrganisationFound);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_IncorrectSchemeYear()
    {
        // Arrange
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(pendingOrganisationFound);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, Guid.NewGuid(), 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_BeforeEndDate()
    {
        // Arrange
        var creditSum = 1;
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(GetLicenceHoldersResponse);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditSum);

        _datetimeProvider.OverrideDate(SchemeYearConstants.EndDate.AddDays(-1));

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 5, _userId, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow)
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, _schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, -7M);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_BeforeStartDate()
    {
        // Arrange
        var creditSum = 1;
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(GetLicenceHoldersResponse);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditSum);

        _datetimeProvider.OverrideDate(SchemeYearConstants.StartDate.AddDays(-1));

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 5, _userId, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow)
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, _schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, -7M);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AfterSurrenderDayDate()
    {
        // Arrange
        var creditSum = 1;
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(GetLicenceHoldersResponse);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditSum);

        _datetimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddDays(1));

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 5, _userId, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow)
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, _schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, -7M);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AdjustedAmount_GreaterThan_Available()
    {
        // Arrange
        var creditSum = 1;
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(GetLicenceHoldersResponse);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditSum);

        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 5, _userId, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow)
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, _schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, -7M);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldGenerateTransaction_When_SuccessfullyTransferingCredits()
    {
        // Arrange
        _datetimeProvider.OverrideDate(_schemeYear.Result.EndDate.AddDays(1));

        var creditSum = 1;
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(GetLicenceHoldersResponse);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), _schemeYearId)).ReturnsAsync(creditSum);
        var transactions = new List<Transaction>
        {
            new Transaction(_schemeYearId, _organisationId, 5, _userId, CreditLedgerConstants.TransactionType.AdminAdjustment, DateTime.UtcNow)
        };
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, _schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);
        _mockTransactionRepository.Setup(x => x.AddTransaction(It.IsAny<Transaction>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        // Act
        var command = new AdjustCreditsCommand(_organisationId, _schemeYearId, 100.5M);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
        _mockTransactionRepository.Verify(i => i.AddTransaction(It.IsAny<Transaction>(), It.IsAny<bool>()), Times.Once());
    }
}
