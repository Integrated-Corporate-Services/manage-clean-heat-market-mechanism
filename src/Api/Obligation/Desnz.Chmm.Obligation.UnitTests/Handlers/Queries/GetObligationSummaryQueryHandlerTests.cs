using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Handlers.Queries;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Queries;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.RepositoryConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Queries
{
    public class GetObligationSummaryQueryHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILogger<GetObligationSummaryQueryHandler>> _mockLogger;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly GetObligationSummaryQueryHandler _handler;

        public GetObligationSummaryQueryHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<GetObligationSummaryQueryHandler>>();
            var mockObligationLogger = new Mock<ILogger<ObligationCalculator>>();

            _schemeYearService = new Mock<ISchemeYearService>();
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), 
                new SchemeYearDto()
                {
                    Id = SchemeYearConstants.Id,
                    Year = SchemeYearConstants.Year
                }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var validator = new RequestValidator(
                _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                _schemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new GetObligationSummaryQueryHandler(
                _mockLogger.Object,
                _mockTransactionRepository.Object,
                new ObligationCalculator(mockObligationLogger.Object),
                validator);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetObligationSummaryQuery(organisationId, SchemeYearConstants.Id);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Failed to get Organisation with Id: {organisationId}, problem: {JsonConvert.SerializeObject(mockGetOrganisationResponse.Problem)}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_SchemeYearNotFound()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = Guid.NewGuid();

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>();

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);
            var expectedErrorMsg = $"Failed to get Scheme Year with Id: {schemeYearId}, problem: null";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);


            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var query = new GetObligationSummaryQuery(organisationId, SchemeYearConstants.Id);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Pending
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Organisation with Id: {organisationId} has an invalid status: {OrganisationConstants.Status.Pending}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldIncludeBroughtForward_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.BroughtFowardFromPreviousYear, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.ObligationsBroughtForward.Should().Be(10);
        }

        [Fact]
        internal async Task ShouldIncludeCarryForward_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.CarryForwardToNextYear, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.ObligationsCarriedOver.Should().Be(10);
        }

        [Fact]
        internal async Task ShouldIncludeAdjustments_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 10, DateTime.UtcNow),
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 20, DateTime.UtcNow),
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 30, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.ObligationAmendments.Count().Should().Be(3);
            result.Value.ObligationAmendments.OrderBy(i => i.Value).First().Value.Should().Be(10);
            result.Value.ObligationAmendments.OrderBy(i => i.Value).Last().Value.Should().Be(30);
        }

        [Fact]
        internal async Task ShouldIncludeAnnual_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AnnualSubmission, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.FinalObligations.Should().Be(10);
        }

        [Fact]
        internal async Task ShouldIncludeAndSumQuarterly_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.QuarterlySubmission, organisationId, schemeYearId, 10, DateTime.UtcNow),
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.QuarterlySubmission, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.FinalObligations.Should().Be(20);
        }

        [Fact]
        internal async Task ShouldSumCorrectTotals_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.QuarterlySubmission, organisationId, schemeYearId, 10, DateTime.UtcNow),
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.QuarterlySubmission, organisationId, schemeYearId, 10, DateTime.UtcNow),
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AdminAdjustment, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.GeneratedObligations.Should().Be(20);
            result.Value.FinalObligations.Should().Be(30);
        }

        [Fact]
        internal async Task ShouldIncludeRedeemed_If_Available()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var schemeYearId = SchemeYearConstants.Id;

            var query = new GetObligationSummaryQuery(organisationId, schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var transtactions = new List<Transaction>()
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.Redeemed, organisationId, schemeYearId, 10, DateTime.UtcNow),
            };

            _mockTransactionRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(),
            SortOrder.Ascending, false)).ReturnsAsync(transtactions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ObligationSummaryDto>>();

            result.Value.ObligationsPaidOff.Should().Be(10);
        }
    }
}
