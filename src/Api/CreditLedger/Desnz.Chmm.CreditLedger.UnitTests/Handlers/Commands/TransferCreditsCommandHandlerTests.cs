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
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.CreditLedger.Api.Handlers.Commands;
using Desnz.Chmm.CreditLedger.Common.Commands;
using IDateTimeProvider = Desnz.Chmm.Common.Providers.IDateTimeProvider;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries;

public class TransferCreditsCommandHandlerTests : TestClaimsBase
{
    // Mocks
    private readonly Mock<ILogger<TransferCreditsCommandHandler>> _mockLogger;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly Mock<IOrganisationService> _mockOrganisationService;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
    private readonly Mock<IDateTimeProvider> _dateTimeProvider;

    // Handler
    private readonly TransferCreditsCommandHandler _handler;

    // Data used in test
    private static readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid destinationOrganisationId = Guid.NewGuid();
    private static readonly Guid userId = Guid.NewGuid();
    private static Guid schemeYearId = SchemeYearConstants.Id;

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

    private static HttpObjectResponse<List<ViewOrganisationDto>> destinationActive = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
    {
        new ViewOrganisationDto
        {
            Id = destinationOrganisationId,
            LicenceHolders = new List<ViewOrganisationLicenceHolderDto>(),
            Name = "Name",
            Status = OrganisationConstants.Status.Active
        }
    }, null);
    private static HttpObjectResponse<List<ViewOrganisationDto>> destinationInactive = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
    {
        new ViewOrganisationDto
        {
            Id = destinationOrganisationId,
            LicenceHolders = new List<ViewOrganisationLicenceHolderDto>(),
            Name = "Name",
            Status = OrganisationConstants.Status.Pending
        }
    }, null);
    private static HttpObjectResponse<List<ViewOrganisationDto>> destinationNotAvailabe = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
    { }, null);
    private Mock<ISchemeYearService> _schemeYearService;

    public TransferCreditsCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<TransferCreditsCommandHandler>>();
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        _dateTimeProvider = new Mock<IDateTimeProvider>(MockBehavior.Strict);
        _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
        _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);

        var mockCurrentUser = GetMockManufacturerUser(userId, _organisationId);
        _mockUserService.SetupGet(x => x.CurrentUser).Returns(mockCurrentUser);

        _schemeYearService = new Mock<ISchemeYearService>();
        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            TradingWindowStartDate = SchemeYearConstants.TradingWindowStartDate,
            TradingWindowEndDate = SchemeYearConstants.TradingWindowEndDate,            
        }, null);
        _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var validator = new RequestValidator(
            _mockUserService.Object,
            _mockOrganisationService.Object,
            _schemeYearService.Object,
            new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

        _handler = new TransferCreditsCommandHandler(
            _mockLogger.Object,
            _mockUserService.Object,
            _dateTimeProvider.Object,
            _mockCreditLedgerRepository.Object,
            _mockTransactionRepository.Object,
            _mockLicenceHolderService.Object,
            _mockOrganisationService.Object,
            new CreditLedgerCalculator(new Mock<ILogger<CreditLedgerCalculator>>().Object),
            validator);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(organisationNotFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_DestinationOrganisationIsNotFound()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationNotAvailabe);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(pendingOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_DestinationOrganisationIsNotActive()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationInactive);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_BeforeTradingWindow()
    {
        // Arrange  
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(-1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_AfterTradingWindow()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowEndDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Tests for actually transfering credits
    [Fact]
    public async Task ShouldReturnBadRequest_When_NotEnoughCredits()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);
        SetupCreditValueCheck(schemeYearId, 10);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100000);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Tests for actually transfering credits
    [Fact]
    public async Task ShouldGenerateTransaction_When_SubmittingOnStartOfTransferWindow()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate);
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);

        _mockCreditLedgerRepository.Setup(x => x.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
        _mockCreditLedgerRepository.Verify(i => i.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>()), Times.Once());
    }

    [Fact]
    public async Task ShouldGenerateTransaction_When_SubmittingOnEndOfTransferWindow()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowEndDate);
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);

        _mockCreditLedgerRepository.Setup(x => x.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
        _mockCreditLedgerRepository.Verify(i => i.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>()), Times.Once());
    }

    [Fact]
    public async Task ShouldGenerateTransaction_When_SuccessfullyTransferingCredits()
    {
        // Arrange
        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationActive);

        _mockCreditLedgerRepository.Setup(x => x.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, destinationOrganisationId, schemeYearId, 100);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
        _mockCreditLedgerRepository.Verify(i => i.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>()), Times.Once());
    }

    private void SetupCreditValueCheck(Guid schemeYearId, int creditSeed)
    {
        var licenceHolderId = Guid.NewGuid();
        var licenceHolderLinks = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderLinkDto>()
        {
            new LicenceHolderLinkDto(){Id = Guid.NewGuid(), LicenceHolderId = licenceHolderId, OrganisationId = _organisationId}
        }, null);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(_organisationId)).ReturnsAsync(licenceHolderLinks);
        _mockCreditLedgerRepository.Setup(x => x.SumCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), schemeYearId)).ReturnsAsync(creditSeed);
        var transactions = new List<Transaction>();
        _mockTransactionRepository.Setup(x => x.GetTransactions(_organisationId, schemeYearId, It.IsAny<bool>())).ReturnsAsync(transactions);

        _mockCreditLedgerRepository.Setup(x => x.AddCreditTransfer(It.IsAny<CreditTransfer>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
        _mockCreditLedgerRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationAndDestinationOrganisationAreTheSame()
    {
        // Arrange
        var destinationAvailableResult = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
        {
            new ViewOrganisationDto
            {
                Id = _organisationId,
                LicenceHolders = new List<ViewOrganisationLicenceHolderDto>(),
                Name = "Name",
                Status = OrganisationConstants.Status.Active
            }
        }, null);

        _dateTimeProvider.Setup(x => x.UtcDateNow).Returns(SchemeYearConstants.TradingWindowStartDate.AddDays(1));
        _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(activeOrganisationFound);
        _mockOrganisationService.Setup(x => x.GetOrganisationsAvailableForTransfer(_organisationId, null)).ReturnsAsync(destinationAvailableResult);
        SetupCreditValueCheck(schemeYearId, 10000);

        // Act
        var command = new TransferCreditsCommand(_organisationId, _organisationId, schemeYearId, 10);
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
