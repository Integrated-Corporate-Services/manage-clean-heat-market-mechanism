using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Mediator;
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
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Commands
{
    public class CreateAnnualObligationCommandHandlerTests : TestClaimsBase
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILogger<CreateAnnualObligationCommandHandler>> _mockLogger;
        private readonly Mock<IObligationCalculator> _mockObligationCalculator;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;

        private readonly CreateAnnualObligationCommandHandler _handler;

        public CreateAnnualObligationCommandHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<CreateAnnualObligationCommandHandler>>();
            _mockObligationCalculator = new Mock<IObligationCalculator>();

            _mockSchemeYearService = new Mock<ISchemeYearService>();
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var obligationCalculationsData = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), new ObligationCalculationsDto()
            {
                GasBoilerSalesThreshold = SchemeYearConstants.GasBoilerSalesThreshold,
                OilBoilerSalesThreshold = SchemeYearConstants.OilBoilerSalesthreshold
            }, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(obligationCalculationsData);

            var validator = new RequestValidator(
                _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                _mockSchemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new CreateAnnualObligationCommandHandler(
                _mockLogger.Object,
                _mockTransactionRepository.Object,
                _mockCurrentUserService.Object,
                _mockObligationCalculator.Object,
                _mockSchemeYearService.Object,
                validator);
        }

        [Fact]
        internal async Task ShouldReturnCreatedResult_When_Valid()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateAnnualObligationCommand() { OrganisationId = organisationId, SchemeYearId = SchemeYearConstants.Id, Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var annualTransactions = new List<Transaction>();
            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };

            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(annualTransactions));
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(quarterlyTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(quarterlyTransactions));

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedResult = Responses.Created(organisationId);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_AnnualObligation_Already_Created()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateAnnualObligationCommand() { OrganisationId = organisationId, SchemeYearId = SchemeYearConstants.Id, Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var annualTransactions = new List<Transaction> { annualTransaction };
            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };

            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(annualTransactions));
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(quarterlyTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(quarterlyTransactions));

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedErrorMsg = $"Annual Obligation for Scheme Year with Id: {command.SchemeYearId} and Organisation with Id {command.OrganisationId} have already been submitted.";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnCreated_When_AnnualObligation_Already_Created_And_Performing_Override()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateAnnualObligationCommand() { Override = true, OrganisationId = organisationId, SchemeYearId = SchemeYearConstants.Id, Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var annualTransactions = new List<Transaction> { annualTransaction };
            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };

            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(annualTransactions));
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(quarterlyTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(quarterlyTransactions));

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedResult = Responses.Created(organisationId);

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
            var command = new CreateAnnualObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

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
            var command = new CreateAnnualObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Pending
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Organisation with Id: {command.OrganisationId} has an invalid status: {OrganisationConstants.Status.Pending}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
