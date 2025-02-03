using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Handlers.Commands;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Commands;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;
using Xunit;
using static Desnz.Chmm.Common.Constants.RepositoryConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Commands
{

    public class AmendObligationCommandHandlerTests : TestClaimsBase
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;
        private readonly Mock<IObligationCalculator> _mockObligationCalculator;
        private readonly Mock<ILogger<AmendObligationCommandHandler>> _mockLogger;
        private readonly Mock<IValidationMessenger> _mockValidationMessenger;
        private RequestValidator _validator;
        private readonly DateTimeOverrideProvider _datetimeProvider;
        private AmendObligationCommandHandler _handler;

        public AmendObligationCommandHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
            _mockObligationCalculator = new Mock<IObligationCalculator>(MockBehavior.Strict);
            _mockValidationMessenger = new Mock<IValidationMessenger>(MockBehavior.Strict);
            
            var (surrenderDate, _) = DateTime.Now.AddDays(1);
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                SurrenderDayDate = surrenderDate
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
            _mockLogger = new Mock<ILogger<AmendObligationCommandHandler>>();

            _validator = new RequestValidator(
                _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                _mockSchemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _datetimeProvider = new DateTimeOverrideProvider();
            _handler = new AmendObligationCommandHandler(
                _mockLogger.Object,
                _mockTransactionRepository.Object, 
                _mockCurrentUserService.Object,
                _datetimeProvider,
                _validator,
                _mockObligationCalculator.Object,
                _mockValidationMessenger.Object);
        }

        [Theory]
        [InlineData(105, 10, true)]
        [InlineData(-20, 10, false)]
        internal async Task ShouldReturnCreatedResult_When_Valid(int obligationAmendment, int startingBalance, bool shouldSucceed)
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new AmendObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Value = obligationAmendment };

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);

            var transactions = new List<Transaction>();
            _mockTransactionRepository.Setup(x => x.GetAll(
                It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<SortOrder>(), It.IsAny<bool>())).ReturnsAsync(transactions);

            _mockObligationCalculator.Setup(x => x.CalculateCurrentObligationBalance(It.IsAny<List<Transaction>>())).Returns(startingBalance);

            _mockValidationMessenger.Setup(x => x.InvalidObligationAmendment(organisationId)).Returns(Responses.BadRequest($"Total obligation cannot be amended to a negative balance. Organisation Id: {organisationId}"));

            ObjectResult expectedResult = shouldSucceed 
                ? Responses.Created(organisationId)
                : Responses.BadRequest($"Total obligation cannot be amended to a negative balance. Organisation Id: {organisationId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new AmendObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Value = 105 };

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Failed to get Organisation with Id: {organisationId}, problem: {JsonConvert.SerializeObject(mockGetOrganisationResponse.Problem)}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new AmendObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Value = 105 };

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Pending
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Organisation with Id: {organisationId} has an invalid status: Pending";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_BeforeEndDate()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new AmendObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Value = 105 };

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _datetimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddDays(-1));

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Active
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Cannot adjust Obligations after end of Scheme Year with Id: {SchemeYearConstants.Id}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            _handler = new AmendObligationCommandHandler(
                _mockLogger.Object,
                _mockTransactionRepository.Object,
                _mockCurrentUserService.Object,
                _datetimeProvider,
                _validator,
                _mockObligationCalculator.Object,
                _mockValidationMessenger.Object);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_AfterSurrenderDayDate()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new AmendObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Value = 105 };

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            _datetimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddDays(1));

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Active
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Cannot adjust Obligations after end of Scheme Year with Id: {SchemeYearConstants.Id}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
