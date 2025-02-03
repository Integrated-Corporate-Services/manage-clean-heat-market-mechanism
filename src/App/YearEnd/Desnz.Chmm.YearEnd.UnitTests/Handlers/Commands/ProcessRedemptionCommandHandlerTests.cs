using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.YearEnd.Api.Handlers.Commands;
using Desnz.Chmm.YearEnd.Common.Commands;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Desnz.Chmm.YearEnd.UnitTests.Handlers.Commands
{
    public class ProcessRedemptionCommandHandlerTests
    {
        private readonly Mock<ILogger<BaseRequestHandler<ProcessRedemptionCommand, ActionResult>>> _mockLogger;
        private readonly Mock<IObligationService> _mockObligationService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;
        private readonly ProcessRedemptionCommandHandler _handler;
        
        private static readonly Guid _organisationId = Guid.NewGuid();
        private static readonly Guid _schemeYearId = SchemeYearConstants.Id;

        private static readonly string _headerName = "api-key";
        private static readonly string _apiKey = "1234";

        public ProcessRedemptionCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<BaseRequestHandler<ProcessRedemptionCommand, ActionResult>>>();

            // Setup OK scheme year service
            _schemeYearService = new Mock<ISchemeYearService>();
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id
            }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(_schemeYearId, It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
            var schemYearNextData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Year2025Id
            }, null);
            _schemeYearService.Setup(x => x.GetNextSchemeYear(_schemeYearId, It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearNextData);

            // Setup OK licence holder service
            _mockLicenceHolderService = new Mock<ILicenceHolderService>(MockBehavior.Strict);
            var licenceHolderId = Guid.NewGuid();
            var licenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK),
            new List<LicenceHolderDto>
            {
                new LicenceHolderDto {Id  = licenceHolderId }
            }, null);
            _mockLicenceHolderService.Setup(x => x.GetAll(It.IsAny<string>())).ReturnsAsync(licenceHolders);
            var licenceHolderLinks = new HttpObjectResponse<List<LicenceHolderLinkDto>>(new HttpResponseMessage(HttpStatusCode.OK),
            new List<LicenceHolderLinkDto>
            {
                new LicenceHolderLinkDto {Id  = Guid.NewGuid(), OrganisationId = _organisationId, LicenceHolderId = licenceHolderId, StartDate = SchemeYearConstants.StartDate }
            }, null);
            _mockLicenceHolderService.Setup(x => x.GetAllLinks(It.IsAny<string>())).ReturnsAsync(licenceHolderLinks);

            // Setup OK organisation service
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            var organisations = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
            {
                new ViewOrganisationDto{ Id = _organisationId}
            }, null);
            _mockOrganisationService.Setup(x => x.GetActiveManufacturers(It.IsAny<string>())).ReturnsAsync(organisations);

            // Setup obligation services
            _mockObligationService = new Mock<IObligationService>(MockBehavior.Strict);
            var obligationResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockObligationService.Setup(x => x.RedeemObligations(It.IsAny<RedeemObligationsCommand>(), It.IsAny<string>())).ReturnsAsync(obligationResponse);
            var rollbackResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockObligationService.Setup(x => x.RollbackRedeemObligations(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(rollbackResponse);

            _mockCreditLedgerService = new Mock<ICreditLedgerService>(MockBehavior.Strict);
            var creditResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockCreditLedgerService.Setup(x => x.RedeemCredits(It.IsAny<RedeemCreditsCommand>(), It.IsAny<string>())).ReturnsAsync(creditResponse);

            _handler = new ProcessRedemptionCommandHandler(
                _mockLogger.Object,
                _mockCreditLedgerService.Object,
                _mockObligationService.Object,
                _mockOrganisationService.Object,
                _mockLicenceHolderService.Object,
                _schemeYearService.Object);
        }
        private HttpContext GetMockContext()
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["HeaderName"] = _headerName;
            mockHttpContext.Request.Headers["api-key"] = _apiKey;
            return mockHttpContext;
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_IncorrectSchemeYear()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(Guid.NewGuid());
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_LicenceHoldersCannotLoad()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockLicenceHolderService.Setup(x => x.GetAll(It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationServiceCannotLoad()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockOrganisationService.Setup(x => x.GetActiveManufacturers(It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_ObligationServiceCannotLoad()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_CreditLedgerServiceCannotLoad()
        {
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, 1)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            var badRequestResponse = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task WhenCreditsAndObligationsAreZero_GenerateNoRedemptions()
        {
            var totalObligations = 0;
            var totalCredits = 0;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 0
            ), It.IsAny<string>()), Times.Once);
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.Is<RedeemCreditsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 0
            ), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task WhenMoreCreditsThanObligations_RedeemObligationValue()
        {
            var totalObligations = 100;
            var totalCredits = 200;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.Is<RedeemCreditsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task WhenMoreObligationsThanCredits_RedeemCreditValue()
        {
            var totalObligations = 200;
            var totalCredits = 100;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits
            ), It.IsAny<string>()), Times.Once);
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.Is<RedeemCreditsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits
            ), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task WhenObligationsEqualCredits_RedeemSameValue()
        {
            var totalObligations = 100;
            var totalCredits = 100;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.Is<RedeemCreditsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task WhenCreditRedemptionFails_DoNotLogObligations()
        {
            var totalObligations = 100;
            var totalCredits = 100;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            var badRequestResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockObligationService.Setup(x => x.RedeemObligations(It.IsAny<RedeemObligationsCommand>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.IsAny<RedeemCreditsCommand>(), It.IsAny<string>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task WhenObligationRedemptionFails_RollbackCreditRedemptionLogging()
        {
            var totalObligations = 100;
            var totalCredits = 100;
            // Arrange
            var obligations = new HttpObjectResponse<List<ObligationTotalDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ObligationTotalDto>
            {
                new ObligationTotalDto(_organisationId, totalObligations)
            }, null);
            _mockObligationService.Setup(x => x.GetSchemeYearObligationTotals(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(obligations);

            // Setup OK credit ledger service
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, totalCredits)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);

            var badRequestResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockCreditLedgerService.Setup(x => x.RedeemCredits(It.IsAny<RedeemCreditsCommand>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new ProcessRedemptionCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockObligationService.Verify(x => x.RedeemObligations(It.Is<RedeemObligationsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);
            
            _mockCreditLedgerService.Verify(x => x.RedeemCredits(It.Is<RedeemCreditsCommand>(i =>
                i.SchemeYearId == SchemeYearConstants.Id &&
                i.Redemptions.Count == 1 &&
                i.Redemptions.First().Value == -totalCredits &&
                i.Redemptions.First().Value == -totalObligations
            ), It.IsAny<string>()), Times.Once);

            _mockObligationService.Verify(x => x.RollbackRedeemObligations(SchemeYearConstants.Id, It.IsAny<string>()), Times.Once);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
