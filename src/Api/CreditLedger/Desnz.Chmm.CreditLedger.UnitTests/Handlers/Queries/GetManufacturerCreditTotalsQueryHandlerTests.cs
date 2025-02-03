using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Desnz.Chmm.CreditLedger.Api.Handlers.Queries;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using System.Net;
using Desnz.Chmm.Common.Mediator;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries;

public class GetManufacturerCreditTotalsQueryHandlerDerived : GetManufacturerCreditTotalsQueryHandler
{
    public GetManufacturerCreditTotalsQueryHandlerDerived(ILogger<BaseRequestHandler<GetManufacturerCreditTotalsQuery, ActionResult<List<PeriodCreditTotals>>>> logger,
                                                          IInstallationCreditRepository creditLedgerRepository,
                                                          ILicenceHolderService licenceHolderService,
                                                          IRequestValidator requestValidator,
                                                          IMcsSynchronisationService mcsSynchronisationService) : base(logger, creditLedgerRepository, licenceHolderService, requestValidator, mcsSynchronisationService)
    {
    }

    internal new IEnumerable<PeriodCreditTotals> GetCreditTotals(IList<InstallationCredit> installationCredits, List<InstallationRequestSummaryDto> installationRequests)
    {
        return base.GetCreditTotals(installationCredits, installationRequests);
    }
}
public class GetManufacturerCreditTotalsQueryHandlerTests : TestClaimsBase
{
    private readonly Mock<ILogger<BaseRequestHandler<GetManufacturerCreditTotalsQuery, ActionResult<List<PeriodCreditTotals>>>>> _mockLogger;
    private readonly Mock<IInstallationCreditRepository> _mockCreditLedgerRepository;
    private readonly Mock<IRequestValidator> _mockRequestValidator;
    private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
    private readonly Mock<IMcsSynchronisationService> _mockMcsSynchronisationService;


    private static readonly Guid _userId = Guid.NewGuid();
    private static readonly Guid _organisationId = Guid.NewGuid();
    private static readonly Guid _schemeYearId = SchemeYearConstants.Id;

    private readonly GetManufacturerCreditTotalsQueryHandler _handler;

    public GetManufacturerCreditTotalsQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<BaseRequestHandler<GetManufacturerCreditTotalsQuery, ActionResult<List<PeriodCreditTotals>>>>>();
        _mockCreditLedgerRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
        _mockRequestValidator = new Mock<IRequestValidator>(MockBehavior.Strict);
        _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
        _mockMcsSynchronisationService = new Mock<IMcsSynchronisationService>(MockBehavior.Strict);


        _handler = new GetManufacturerCreditTotalsQueryHandler(_mockLogger.Object,
                                                               _mockCreditLedgerRepository.Object,
                                                               _mockLicenceHolderService.Object,
                                                               _mockRequestValidator.Object,
                                                               _mockMcsSynchronisationService.Object);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
    {
        // Arrange
        var _organisationId = Guid.NewGuid();
        var _schemeYearId = SchemeYearConstants.Id;

        ActionResult? errorMessage = Responses.BadRequest("error_message");

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             null,
                                                                             null))
            .Returns(Task.FromResult<ActionResult?>(errorMessage));

        var query = new GetManufacturerCreditTotalsQuery(It.IsAny<Guid>(), It.IsAny<Guid>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_LicenceHoldersNotFound()
    {
        // Arrange

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             null,
                                                                             null))
            .Returns(Task.FromResult<ActionResult?>(null));

        var installationRequests = new List<InstallationRequestSummaryDto>()
        {
            new InstallationRequestSummaryDto(Guid.NewGuid(), new DateTime(), new DateTime(2025, 1, 1), new DateTime(2025, 12, 31), new string[0], new List<string>())
        };
        _mockMcsSynchronisationService.Setup(x => x.GetInstallationRequests(It.IsAny<Guid>(), null))
            .Returns(Task.FromResult<HttpObjectResponse<List<InstallationRequestSummaryDto>>>(new CustomHttpObjectResponse<List<InstallationRequestSummaryDto>>(installationRequests)));


        var getLicenceHolderResponse = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(It.IsAny<Guid>())).ReturnsAsync(getLicenceHolderResponse);

        var query = new GetManufacturerCreditTotalsQuery(It.IsAny<Guid>(), It.IsAny<Guid>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task ShouldReturnBadRequest_When_InstallationRequestsNotFound()
    {
        // Arrange

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             null,
                                                                             null))
            .Returns(Task.FromResult<ActionResult?>(null));

        var installationRequests = new List<InstallationRequestSummaryDto>()
        {
            new InstallationRequestSummaryDto(Guid.NewGuid(), new DateTime(), new DateTime(2025, 1, 1), new DateTime(2025, 12, 31), new string[0], new List<string>())
        };
        _mockMcsSynchronisationService.Setup(x => x.GetInstallationRequests(It.IsAny<Guid>(), null))
            .Returns(Task.FromResult<HttpObjectResponse<List<InstallationRequestSummaryDto>>>(new CustomHttpObjectResponse<List<InstallationRequestSummaryDto>>(installationRequests)));


        var getLicenceHolderResponse = new HttpObjectResponse<List<InstallationRequestSummaryDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockMcsSynchronisationService.Setup(x => x.GetInstallationRequests(It.IsAny<Guid>(), null)).ReturnsAsync(getLicenceHolderResponse);

        var query = new GetManufacturerCreditTotalsQuery(It.IsAny<Guid>(), It.IsAny<Guid>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }


    [Fact]
    public async Task ShouldReturnCorrectResults_When_Ok()
    {
        // Arrange
        var installationRequests = new List<InstallationRequestSummaryDto>()
        {
            new InstallationRequestSummaryDto(Guid.NewGuid(), new DateTime(), new DateTime(2025, 1, 1), new DateTime(2025, 12, 31), new string[0], new List<string>())
        };
        _mockMcsSynchronisationService.Setup(x => x.GetInstallationRequests(It.IsAny<Guid>(), null))
            .Returns(Task.FromResult<HttpObjectResponse<List<InstallationRequestSummaryDto>>>(new CustomHttpObjectResponse<List<InstallationRequestSummaryDto>>(installationRequests)));


        _mockLicenceHolderService.Setup(x => x.GetLinkedToHistory(It.IsAny<Guid>())).ReturnsAsync(GetLicenceHoldersResponse);

        _mockRequestValidator.Setup(x => x.ValidateSchemeYearAndOrganisation(It.IsAny<Guid>(),
                                                                             It.IsAny<bool>(),
                                                                             It.IsAny<Guid>(),
                                                                             null,
                                                                             null))
            .Returns(Task.FromResult<ActionResult?>(null));

        _mockCreditLedgerRepository.Setup(x => x.GetInstallationCredits(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), It.IsAny<Guid>()))
            .Returns(Task.FromResult<IList<InstallationCredit>>(new List<InstallationCredit>() { new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 1, 1), 23, true) }));

        var query = new GetManufacturerCreditTotalsQuery(It.IsAny<Guid>(), It.IsAny<Guid>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.Single(result.Value);
    }


    private static HttpObjectResponse<List<LicenceHolderLinkDto>> GetLicenceHoldersResponse()
    {
        return new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<LicenceHolderLinkDto>
        {
            new LicenceHolderLinkDto
            {
                Id = Guid.NewGuid(),
                LicenceHolderId = Guid.NewGuid(),
                OrganisationId = _organisationId,
                StartDate = new DateOnly(2024, 2, 3)
            },
            new LicenceHolderLinkDto
            {
                Id = Guid.NewGuid(),
                LicenceHolderId = Guid.NewGuid(),
                OrganisationId = _organisationId,
                StartDate = new DateOnly(2024, 2, 4)
            }
        }, null);
    }

    [Fact]
    public async Task GetCreditTotals_All_Empty_Lists()

    {
        var installationRequests = new List<InstallationRequestSummaryDto>();
        var installationCredits = new List<InstallationCredit>();


        GetManufacturerCreditTotalsQueryHandlerDerived handler = new GetManufacturerCreditTotalsQueryHandlerDerived(_mockLogger.Object,
                                                               _mockCreditLedgerRepository.Object,
                                                               _mockLicenceHolderService.Object,
                                                               _mockRequestValidator.Object,
                                                               _mockMcsSynchronisationService.Object);


        var result = handler.GetCreditTotals(installationCredits, installationRequests);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCreditTotals_AllPeriods_InThe_SameSchemeYear()

    {
        var installationRequestPeriod1 = new InstallationRequestSummaryDto(Guid.NewGuid(), new DateTime(), new DateTime(2025, 3, 1), new DateTime(2025, 3, 31), new string[0], new List<string>());
        var installationRequests = new List<InstallationRequestSummaryDto>() { installationRequestPeriod1 };

        var creditBeforePeriod = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 2, 28), 0, true);
        var creditAfterPeriod = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 4, 1), 0, true);
        var creditWithinPeriod1 = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 3, 1), 1, true);
        var creditWithinPeriod2 = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 3, 1), 0.5m, true);
        var creditWithinPeriod3 = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 3, 1), 1, false);
        var creditWithinPeriod4 = new InstallationCredit(Guid.NewGuid(), 1, Guid.NewGuid(), new DateOnly(2025, 3, 31), 1, false);

        var installationCredits = new List<InstallationCredit>
        { 
            creditBeforePeriod, creditAfterPeriod, creditWithinPeriod1, creditWithinPeriod2, creditWithinPeriod3, creditWithinPeriod4
        };


        var handler = new GetManufacturerCreditTotalsQueryHandlerDerived(_mockLogger.Object,
                                                               _mockCreditLedgerRepository.Object,
                                                               _mockLicenceHolderService.Object,
                                                               _mockRequestValidator.Object,
                                                               _mockMcsSynchronisationService.Object);


        var result = handler.GetCreditTotals(installationCredits, installationRequests);

        Assert.Single(result);
        Assert.Equal(2, result.Single().HeatPumpsInstallations);
        Assert.Equal(2, result.Single().HybridHeatPumpsInstallations);
        Assert.Equal(2, result.Single().HeatPumpsGeneratedCredits);
        Assert.Equal(1.5m, result.Single().HybridHeatPumpsGeneratedCredits);
    }


}
