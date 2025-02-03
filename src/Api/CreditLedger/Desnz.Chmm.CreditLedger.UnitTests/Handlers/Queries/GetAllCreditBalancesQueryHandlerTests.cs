using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Api.Handlers.Queries;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.CreditLedger.UnitTests.Handlers.Queries
{
    public class GetAllCreditBalancesQueryHandlerTests : TestClaimsBase
    {
        private readonly Mock<ILogger<BaseRequestHandler<GetAllCreditBalancesQuery, ActionResult<List<OrganisationCreditBalanceDto>>>>> _logger;
        private readonly Mock<IOrganisationService> _organisationService;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly Mock<ILicenceHolderService> _licenceHolderService;
        private readonly Mock<IInstallationCreditRepository> _installationCreditRepository;
        private readonly Mock<ITransactionRepository> _transactionRepository;
        private readonly Mock<ICurrentUserService> _mockUserService;
        private readonly GetAllCreditBalancesQueryHandler _handler;

        private static readonly Guid _organisationId = Guid.NewGuid();
        private HttpObjectResponse<List<LicenceHolderDto>> _licenceHoldersData;
        private HttpObjectResponse<List<LicenceHolderLinkDto>> _licenceHolderLinkData;

        public GetAllCreditBalancesQueryHandlerTests()
        {
            _logger = new Mock<ILogger<BaseRequestHandler<GetAllCreditBalancesQuery, ActionResult<List<OrganisationCreditBalanceDto>>>>>();
            _organisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _schemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
            _licenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
            _installationCreditRepository = new Mock<IInstallationCreditRepository>(MockBehavior.Strict);
            _transactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);

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

            var validator = new RequestValidator(
                _mockUserService.Object,
                _organisationService.Object,
                _schemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new GetAllCreditBalancesQueryHandler(
                _logger.Object,
                _organisationService.Object,
                _licenceHolderService.Object,
                _installationCreditRepository.Object,
                _transactionRepository.Object,
                new CreditLedgerCalculator(new Mock<ILogger<CreditLedgerCalculator>>().Object),
                validator
                );
        }

        [Fact]
        public async void WhenNoSchemeYear_Returns_BadRequest()
        {
            var blankSchemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _schemeYearService.Setup(i => i.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(blankSchemeYearResponse);

            var result = await _handler.Handle(new GetAllCreditBalancesQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void WhenNoOrganisations_Returns_BadRequest()
        {
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var blankOrganisationResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _organisationService.Setup(i => i.GetManufacturers(It.IsAny<string>())).ReturnsAsync(blankOrganisationResponse);

            var result = await _handler.Handle(new GetAllCreditBalancesQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void WhenNoLicenceHolders_Returns_BadRequest()
        {
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var organisationResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>(), null);
            _organisationService.Setup(i => i.GetManufacturers(It.IsAny<string>())).ReturnsAsync(organisationResponse);

            var blankLicenceHolderResponse = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _licenceHolderService.Setup(i => i.GetAll(It.IsAny<string>())).ReturnsAsync(blankLicenceHolderResponse);
            var blankLicenceHolderLinkResponse = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _licenceHolderService.Setup(x => x.GetAllLinks(It.IsAny<string>())).ReturnsAsync(blankLicenceHolderLinkResponse);

            var result = await _handler.Handle(new GetAllCreditBalancesQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void WhenDataAvailable_Returns_Balance()
        {
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var organisationResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
            {
                new ViewOrganisationDto
                {
                    Id = _organisationId
                }
            }, null);
            _organisationService.Setup(i => i.GetManufacturers(It.IsAny<string>())).ReturnsAsync(organisationResponse);

            _licenceHolderService.Setup(i => i.GetAll(It.IsAny<string>())).ReturnsAsync(_licenceHoldersData);
            _licenceHolderService.Setup(x => x.GetAllLinks(It.IsAny<string>())).ReturnsAsync(_licenceHolderLinkData);

            var licenceHolderId = _licenceHoldersData.Result.First().Id;
            var creditList = new List<OrganisationLicenceHolderCreditsDto> { new OrganisationLicenceHolderCreditsDto(licenceHolderId, _organisationId, 100) };
            _installationCreditRepository.Setup(x => x.SumCreditsByLicenceHolderAndOrganisation(It.IsAny<IEnumerable<LicenceOwnershipDto>>(), SchemeYearConstants.Id)).ReturnsAsync(creditList);

            var transactions = new List<Transaction>();
            _transactionRepository.Setup(x => x.GetTransactions(SchemeYearConstants.Id, It.IsAny<bool>())).ReturnsAsync(transactions);

            var result = await _handler.Handle(new GetAllCreditBalancesQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.Equal(100, result.Value.First().CreditBalance);
        }
    }
}
