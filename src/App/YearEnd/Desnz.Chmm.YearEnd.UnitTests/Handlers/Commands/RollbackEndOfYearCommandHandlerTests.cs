using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.YearEnd.Api.Handlers.Commands;
using Desnz.Chmm.YearEnd.Common.Commands;
using Desnz.Chmm.Testing.Common;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Desnz.Chmm.YearEnd.Api.Controllers;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Configuration.Common.Commands;

namespace Desnz.Chmm.YearEnd.UnitTests.Handlers.Commands
{
    public class RollbackEndOfYearCommandHandlerTests
    {
        private readonly Mock<ILogger<BaseRequestHandler<RollbackEndOfYearCommand, ActionResult>>> _mockLogger;
        private readonly Mock<IObligationService> _mockObligationService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILicenceHolderService> _mockLicenceHolderService;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IOptions<ApiKeyPolicyConfig>> _mockOptionsMcsSynchronisationPolicyConfig;
        private readonly RollbackEndOfYearCommandHandler _handler;
        
        private static readonly Guid _organisationId = Guid.NewGuid();
        private static readonly Guid _schemeYearId = SchemeYearConstants.Id;

        private static readonly string _headerName = "api-key";
        private static readonly string _apiKey = "1234";
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IYearEndService> _mockYearEndService;

        public RollbackEndOfYearCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<BaseRequestHandler<RollbackEndOfYearCommand, ActionResult>>>();

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
            var licenceHolders = new HttpObjectResponse<List<LicenceHolderDto>>(new HttpResponseMessage(HttpStatusCode.OK),
            new List<LicenceHolderDto>
            {
                            new LicenceHolderDto {Id  = Guid.NewGuid() }
            }, null);
            _mockLicenceHolderService.Setup(x => x.GetAll(It.IsAny<string>())).ReturnsAsync(licenceHolders);

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
            
            _mockOptionsMcsSynchronisationPolicyConfig = new Mock<IOptions<ApiKeyPolicyConfig>>();
            _mockOptionsMcsSynchronisationPolicyConfig.Setup(x => x.Value).Returns(new ApiKeyPolicyConfig() { HeaderName = _headerName, ApiKey = _apiKey });
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            _mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(GetMockContext());
            _mockIdentityService = new Mock<IIdentityService>();
            const string token = "TOKEN";
            var response = new HttpObjectResponse<string>(new HttpResponseMessage(HttpStatusCode.OK), token, null);
            _mockIdentityService.Setup(x => x.GetJwtToken(It.IsAny<GetJwtTokenCommand>())).Returns(Task.FromResult(response));
            _mockYearEndService = new Mock<IYearEndService>();

            _handler = new RollbackEndOfYearCommandHandler(
                _mockLogger.Object,
                _mockCreditLedgerService.Object,
                _mockObligationService.Object,
                _schemeYearService.Object,
                _mockIdentityService.Object, //_mockOrganisationService.Object,
                _mockYearEndService.Object, //_mockLicenceHolderService.Object,
                _mockOptionsMcsSynchronisationPolicyConfig.Object,
                _mockHttpContextAccessor.Object
                );
        }
        private HttpContext GetMockContext()
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["HeaderName"] = _headerName;
            mockHttpContext.Request.Headers["api-key"] = _apiKey;
            return mockHttpContext;
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_Failing_IdentityToken()
        {
            // Arrange
            var response = new HttpObjectResponse<string>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _mockIdentityService.Setup(x => x.GetJwtToken(It.IsAny<GetJwtTokenCommand>())).Returns(Task.FromResult(response));

            // Act
            var command = new RollbackEndOfYearCommand(Guid.NewGuid());
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_IncorrectSchemeYear()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new RollbackEndOfYearCommand(Guid.NewGuid());
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ShouldReturnBadRequest_When_Incorrect_NextSchemeYear()
        {
            // Arrange
            var badRequestResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _schemeYearService.Setup(x => x.GetNextSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new RollbackEndOfYearCommand(_schemeYearId);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ShouldReturnBadRequest_When_CreditLedgerServiceCannotLoad()
        {
            // Arrange
            var schemeYearServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _schemeYearService.Setup(x => x.RollbackGenerateNextYearsSchemea(It.IsAny<RollbackGenerateNextYearsSchemeCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearServiceOkResponse);

            var yearEndServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockYearEndService.Setup(x => x.RollbackRedemption(It.IsAny<RollbackRedemptionCommand>(), It.IsAny<string>())).ReturnsAsync(yearEndServiceOkResponse);

            var obligationsServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockObligationService.Setup(x => x.RollbackCarryForwardObligation(It.IsAny<RollbackCarryForwardObligationCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(obligationsServiceOkResponse);

            var badRequestResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockCreditLedgerService.Setup(x => x.RollbackCarryOverCredit(It.IsAny<RollbackCarryOverCreditCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(badRequestResponse);

            // Act
            var command = new RollbackEndOfYearCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task ShouldReturnBadRequest_When_WhenAllServices_Successfully_Load()
        {
            // Arrange
            var schemeYearServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _schemeYearService.Setup(x => x.RollbackGenerateNextYearsSchemea(It.IsAny<RollbackGenerateNextYearsSchemeCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearServiceOkResponse);

            var yearEndServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockYearEndService.Setup(x => x.RollbackRedemption(It.IsAny<RollbackRedemptionCommand>(), It.IsAny<string>())).ReturnsAsync(yearEndServiceOkResponse);

            var obligationsServiceOkResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockObligationService.Setup(x => x.RollbackCarryForwardObligation(It.IsAny<RollbackCarryForwardObligationCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(obligationsServiceOkResponse);

            var creditLedgerServiceResponse = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            _mockCreditLedgerService.Setup(x => x.RollbackCarryOverCredit(It.IsAny<RollbackCarryOverCreditCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(creditLedgerServiceResponse);

            // Act
            var command = new RollbackEndOfYearCommand(SchemeYearConstants.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
