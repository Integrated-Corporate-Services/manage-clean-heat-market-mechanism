using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Extensions;
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
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.RepositoryConstants;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Commands
{
    public class CreateQuarterlyObligationCommandHandlerTests : TestClaimsBase
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<IBoilerSalesService> _mockBoilerSalesService;
        private readonly Mock<ILogger<CreateQuarterlyObligationCommandHandler>> _mockLogger;
        private readonly Mock<IObligationCalculator> _mockObligationCalculator;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;
        private readonly ClaimsPrincipal _mockUser;
        private readonly CreateQuarterlyObligationCommandHandler _handler;

        public CreateQuarterlyObligationCommandHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockBoilerSalesService = new Mock<IBoilerSalesService>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<CreateQuarterlyObligationCommandHandler>>();
            _mockObligationCalculator = new Mock<IObligationCalculator>();
            _mockSchemeYearService = new Mock<ISchemeYearService>();

            _mockUser = GetMockAdminUser(Guid.NewGuid());
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(_mockUser);

            var httpResponse = new HttpObjectResponse<CreditWeightingsDto>(new HttpResponseMessage(HttpStatusCode.OK), null, null);
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

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

            _handler = new CreateQuarterlyObligationCommandHandler(_mockLogger.Object,
                                                                   _mockTransactionRepository.Object,
                                                                   _mockCurrentUserService.Object,
                                                                   _mockBoilerSalesService.Object,
                                                                   _mockObligationCalculator.Object,
                                                                   _mockSchemeYearService.Object,
                                                                   validator);
        }

        [Fact]
        internal async Task ShouldCreateAsExcluded_When_Annual_Exists()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateQuarterlyObligationCommand() { 
                OrganisationId = organisationId, 
                SchemeYearId = SchemeYearConstants.Id, 
                Gas = 0,
                Oil = 0, 
                TransactionDate = DateTime.MinValue,
                SchemeYearQuarterId = SchemeYearConstants.QuarterOneId
            };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            // This transaction hasn't been submitted yet.
            Transaction? nullTransaction = null;
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransaction(organisationId,
                                                    command.SchemeYearId,
                                                    command.SchemeYearQuarterId,
                                                    It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);

            // Annual Transaction has been submitted
            _mockTransactionRepository
                .Setup(x => x.GetAnnualTransaction(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(new Transaction(_mockUser.GetUserId().Value, TransactionType.AnnualSubmission, organisationId, SchemeYearConstants.Id, 100, DateTime.Now, null));

            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, command.OrganisationId, command.SchemeYearId, 0, DateTime.MinValue);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransactions(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(quarterlyTransactions);

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), It.IsAny<bool>())).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<QuarterlyBoilerSalesDto> { new QuarterlyBoilerSalesDto { OrganisationId = organisationId } }, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));


            var expectedResult = Responses.Created(organisationId);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            _mockTransactionRepository.Verify(x => x.Create(It.Is<Transaction>(obj => obj.IsExcluded == true), It.IsAny<bool>()), Times.Once);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldCreateAsIncluded_When_Annual_Does_Not_Exists()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateQuarterlyObligationCommand()
            {
                OrganisationId = organisationId,
                SchemeYearId = SchemeYearConstants.Id,
                Gas = 0,
                Oil = 0,
                TransactionDate = DateTime.MinValue,
                SchemeYearQuarterId = SchemeYearConstants.QuarterOneId
            };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            // This transaction hasn't been submitted yet.
            Transaction? nullTransaction = null;
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransaction(organisationId,
                                                    command.SchemeYearId,
                                                    command.SchemeYearQuarterId,
                                                    It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);

            // Annual Transaction has been submitted
            _mockTransactionRepository
                .Setup(x => x.GetAnnualTransaction(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);

            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, command.OrganisationId, command.SchemeYearId, 0, DateTime.MinValue);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransactions(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(quarterlyTransactions);

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), It.IsAny<bool>())).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<QuarterlyBoilerSalesDto> { new QuarterlyBoilerSalesDto { OrganisationId = organisationId } }, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));

            var expectedResult = Responses.Created(organisationId);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            _mockTransactionRepository.Verify(x => x.Create(It.Is<Transaction>(obj => obj.IsExcluded == false), It.IsAny<bool>()), Times.Once);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnCreatedResult_When_Valid()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateQuarterlyObligationCommand()
            {
                OrganisationId = organisationId,
                SchemeYearId = SchemeYearConstants.Id,
                Gas = 0,
                Oil = 0,
                TransactionDate = DateTime.MinValue,
                SchemeYearQuarterId = SchemeYearConstants.QuarterOneId
            };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<QuarterlyBoilerSalesDto> { new QuarterlyBoilerSalesDto { OrganisationId = organisationId} }, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));

            // This transaction hasn't been submitted yet.
            Transaction? nullTransaction = null;
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransaction(organisationId,
                                                    command.SchemeYearId,
                                                    command.SchemeYearQuarterId,
                                                    It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);

            // Annual Transaction has been submitted
            _mockTransactionRepository
                .Setup(x => x.GetAnnualTransaction(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);

            // Nothing has been submitted yet
            var quarterlyTransactions = new List<Transaction>();
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransactions(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(quarterlyTransactions);

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedResult = Responses.Created(organisationId);

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
            var command = new CreateQuarterlyObligationCommand() { Override = true, OrganisationId = organisationId, SchemeYearId = SchemeYearConstants.Id, SchemeYearQuarterId = SchemeYearConstants.QuarterOneId, Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue);
            var annualTransactions = new List<Transaction> { annualTransaction };
            var quarterlyTransaction = new Transaction(Guid.NewGuid(), TransactionType.QuarterlySubmission, organisationId, SchemeYearConstants.Id, 0, DateTime.MinValue, SchemeYearConstants.QuarterOneId);
            var quarterlyTransactions = new List<Transaction> { quarterlyTransaction };

            _mockTransactionRepository.Setup(x => x.GetQuarterTransactions(organisationId, command.SchemeYearId, It.IsAny<bool>()))
                .ReturnsAsync(quarterlyTransactions);


            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(annualTransactions));
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(y => y.Compile()(quarterlyTransaction)), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(quarterlyTransactions));
            _mockTransactionRepository.Setup(x => x.GetQuarterTransaction(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(quarterlyTransaction));

            _mockTransactionRepository.Setup(x => x.Remove(It.IsAny<List<Transaction>>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);
            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<QuarterlyBoilerSalesDto> { new QuarterlyBoilerSalesDto { OrganisationId = organisationId } }, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));


            _mockTransactionRepository.Setup(x => x.GetAnnualTransaction(organisationId, command.SchemeYearId, It.IsAny<bool>())).ReturnsAsync((Transaction)null);

            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var expectedResult = Responses.Created(organisationId);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_GetQuarterlyBoilerSales_Fails()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateQuarterlyObligationCommand()
            {
                OrganisationId = organisationId,
                SchemeYearId = SchemeYearConstants.Id,
                Gas = 0,
                Oil = 0,
                TransactionDate = DateTime.MinValue,
                SchemeYearQuarterId = SchemeYearConstants.QuarterOneId
            };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.BadGateway), null, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));

            // This transaction hasn't been submitted yet.
            Transaction? nullTransaction = null;
            _mockTransactionRepository
                .Setup(x => x.GetQuarterTransaction(organisationId,
                                                    command.SchemeYearId,
                                                    command.SchemeYearQuarterId,
                                                    It.IsAny<bool>()))
                .ReturnsAsync(nullTransaction);
            _mockTransactionRepository.Setup(x => x.Create(It.IsAny<Transaction>(), true)).ReturnsAsync(organisationId);
            _mockTransactionRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var errorMessage = $"Failed to get Quarter boiler sales data for organisation with Id: {organisationId}, problem: null";
            var expectedResult = Responses.NotFound(errorMessage);

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
            var command = new CreateQuarterlyObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

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
            var command = new CreateQuarterlyObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Retired
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Organisation with Id: {organisationId} has an invalid status: Retired";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task GenerateCredits_With_Invalid_SchemeYearId_Returns_Bad_Request()
        {
            //Arrange
            var organisationId = Guid.NewGuid();
            var command = new CreateQuarterlyObligationCommand() { OrganisationId = organisationId, SchemeYearId = Guid.NewGuid(), Gas = 0, Oil = 0, TransactionDate = DateTime.MinValue };

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            _mockTransactionRepository.Setup(x => x.GetAll(x => x.SchemeYearId == command.SchemeYearId &&
                                                                    x.SchemeYearQuarterId == command.SchemeYearQuarterId &&
                                                                    x.OrganisationId == command.OrganisationId &&
                                                                    x.TransactionType == TransactionType.QuarterlySubmission, It.IsAny<SortOrder>(), It.IsAny<bool>()))
                                        .ReturnsAsync(new List<Transaction>());
            _mockTransactionRepository.Setup(x => x.GetAll(x => x.SchemeYearId == command.SchemeYearId &&
                                                                x.OrganisationId == command.OrganisationId &&
                                                                x.TransactionType == TransactionType.AnnualSubmission, It.IsAny<SortOrder>(), It.IsAny<bool>())).ReturnsAsync(new List<Transaction>());

            var mockGetQuarterlyBoilerSalesResponse = new HttpObjectResponse<List<QuarterlyBoilerSalesDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<QuarterlyBoilerSalesDto> { new QuarterlyBoilerSalesDto { OrganisationId = organisationId } }, null);
            _mockBoilerSalesService.Setup(x => x.GetQuarterlyBoilerSales(command.OrganisationId, command.SchemeYearId)).Returns(Task.FromResult(mockGetQuarterlyBoilerSalesResponse));

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {command.SchemeYearId}, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
