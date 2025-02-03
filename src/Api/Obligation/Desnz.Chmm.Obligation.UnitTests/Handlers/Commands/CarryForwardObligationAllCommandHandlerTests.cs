using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Dtos;
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
using System.Linq.Expressions;
using System.Net;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;
using static Desnz.Chmm.Obligation.Api.Constants.TransactionConstants;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Commands
{

    public class CarryForwardObligationCommandHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILogger<CarryForwardObligationCommandHandler>> _mockLogger;
        private readonly Mock<IBoilerSalesService> _mockBoilerSalesService;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;
        private readonly Mock<ICarryForwardObligationCalculator> _mockCarryForwardObligationCalculator;
        private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;

        private readonly Guid _currentSchemeYearId;
        private readonly Guid _nextSchemeYearId;
        private readonly Guid _user;
        private readonly Guid _organisationId;
        private readonly Guid _organisation2Id;

        private List<SchemeYearDto> _schemeYears;
        private readonly CarryForwardObligationCommandHandler _handler;

        public CarryForwardObligationCommandHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<CarryForwardObligationCommandHandler>>();
            _mockBoilerSalesService = new Mock<IBoilerSalesService>(MockBehavior.Strict);
            _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
            _mockCarryForwardObligationCalculator = new Mock<ICarryForwardObligationCalculator> (MockBehavior.Strict);
            _mockCreditLedgerService = new Mock<ICreditLedgerService>();

            var datetimeProvider = new DateTimeProvider();

            _user = Guid.NewGuid();
            _organisationId = Guid.NewGuid();
            _organisation2Id = Guid.NewGuid();
            _currentSchemeYearId = SchemeYearConstants.Id;
            _nextSchemeYearId = SchemeYearConstants.Year2025Id;

            _handler = new CarryForwardObligationCommandHandler(_mockLogger.Object,
                                                                _mockOrganisationService.Object,
                                                                _mockBoilerSalesService.Object,
                                                                _mockCreditLedgerService.Object,
                                                                _mockSchemeYearService.Object,
                                                                _mockCarryForwardObligationCalculator.Object,
                                                                _mockTransactionRepository.Object,
                                                                datetimeProvider);
        }


        [Fact]
        internal async Task ShouldReturnBadRequest_When_SchemeYears_NotLoaded()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsCurrentYearBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get all Scheme Years, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_Invalid_Current_SchemeYearId()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsNextYearOnlyBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_Invalid_Next_SchemeYearId()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsCurrentYearOnlyBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get next Scheme Year for Scheme Year with Id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_Manufacturers_NotLoaded()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get all Organisations, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_NoManufacturers_Active()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkNotAllActive();

            var expectedResult = Responses.Ok();

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_GetObligationCalculations_NotLoaded()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get Obligation Calculations for Scheme Year with Id: {_currentSchemeYearId}, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_BoilerSalesSummary_NotLoaded()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get Boiler Sales summaries for Scheme Year with Id: {_currentSchemeYearId}, problem: null");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_No_BoilerSalesSummary_WereFound()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkEmptyList();

            var expectedResult = Responses.BadRequest($"No Boiler Sales found for scheme year id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_SomeAnnual_BoilerSalesSummary_NotApproved()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkAnnualSalesNotApproved();

            var expectedResult = Responses.BadRequest($"Unapproved Annual Boiler Sales found for scheme year id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_CreditBalances_NotLoaded()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesBadRequest();

            var expectedResult = Responses.BadRequest($"Failed to get Credit Balances for Scheme Year with Id: {_currentSchemeYearId}, problem: {{\"Type\":null,\"Title\":\"Error\",\"Status\":500,\"TraceId\":null,\"Detail\":\"Error\",\"Errors\":null,\"ExceptionDetails\":null}}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_No_CreditBalances()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesOkEmptyList();

            var expectedResult = Responses.BadRequest($"No Credit Balances found for scheme year id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_Null_ObligationTransactions()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesOkSingleOrganisation();
            SetupNullObligationTransactions();

            var expectedResult = Responses.BadRequest($"Failed to get annual Obligations for Scheme Year with Id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        internal async Task ShouldReturnBadRequest_When_CarryForwardObligation_IsAlreadyGenerated()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesOkSingleOrganisation();
            SetupNoObligationTransactionsAndSomeExistingTransactions();

            var expectedResult = Responses.BadRequest($"Carry Forward obligation is already generated for some organisations for the Scheme Year Id: {_currentSchemeYearId}");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockTransactionRepository.Verify(i => i.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        internal async Task ShouldReturnOK_When_Some_ObligationTransactions_Exist_CannotCalculate()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesOkSingleOrganisation();
            SetupSomeObligationTransactionsAndNoExistingTransactions();

            _mockCarryForwardObligationCalculator.Setup(x => x.Calculate(It.IsAny<List<Transaction>>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(1);
            _mockTransactionRepository.Setup(x => x.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

            var expectedResult = Responses.Ok();

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockTransactionRepository.Verify(m => m.AddTransactions(It.Is<List<Transaction>>(l => l.Count == 2), It.IsAny<bool>()), Times.Exactly(1));

            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        internal async Task ShouldReturnOK_When_Some_ObligationTransactions_Exist()
        {
            //Arrange
            var command = new CarryForwardObligationCommand(_currentSchemeYearId);

            SetupGetAllSchemeYearsOK();
            SetupGetManufacturersOkSomeActive();
            SetupGetObligationCalculationsOk();
            SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales();
            SetupGetAllCreditBalancesOk();
            SetupSomeObligationTransactionsAndNoExistingTransactions();

            _mockCarryForwardObligationCalculator.Setup(x => x.Calculate(It.IsAny<List<Transaction>>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(1);
            _mockTransactionRepository.Setup(x => x.AddTransactions(It.IsAny<List<Transaction>>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

            var expectedResult = Responses.Ok();

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockTransactionRepository.Verify(m => m.AddTransactions(It.Is<List<Transaction>>(l => l.Count == 4), It.IsAny<bool>()), Times.Once());

        }
        #region Private methods
        private void SetupGetAllSchemeYearsCurrentYearBadRequest()
        {
            var mockGetAllSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.BadGateway), null, null);
            _mockSchemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(mockGetAllSchemeYears);
        }

        private void SetupGetAllSchemeYearsCurrentYearOnlyBadRequest()
        {
            _schemeYears = new List<SchemeYearDto>
            {
                new SchemeYearDto { Id = _currentSchemeYearId, Year = SchemeYearConstants.Year}
            };

            var mockGetAllSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.OK), _schemeYears, null);
            _mockSchemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(mockGetAllSchemeYears);
        }

        private void SetupGetAllSchemeYearsNextYearOnlyBadRequest()
        {
            _schemeYears = new List<SchemeYearDto>
            {
                new SchemeYearDto { Id = SchemeYearConstants.Year2025Id, Year = SchemeYearConstants.Year+1}
            };

            var mockGetAllSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.OK), _schemeYears, null);
            _mockSchemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(mockGetAllSchemeYears);
        }

        private void SetupGetAllSchemeYearsOK()
        {
            _schemeYears = new List<SchemeYearDto>
            {
                new SchemeYearDto { Id = _currentSchemeYearId, Year = SchemeYearConstants.Year} ,
                new SchemeYearDto { Id = SchemeYearConstants.Year2025Id, Year = SchemeYearConstants.Year+1, PreviousSchemeYearId = _currentSchemeYearId}
            };

            var mockGetAllSchemeYears = new HttpObjectResponse<List<SchemeYearDto>>(new HttpResponseMessage(HttpStatusCode.OK), _schemeYears, null);
            _mockSchemeYearService.Setup(x => x.GetAllSchemeYears(It.IsAny<CancellationToken>())).ReturnsAsync(mockGetAllSchemeYears);
        }

        private void SetupGetManufacturersBadRequest()
        {
            var manufacturersResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.BadGateway), null, null);
            _mockOrganisationService.Setup(x => x.GetManufacturers(It.IsAny<string>())).ReturnsAsync(manufacturersResponse);
        }

        private void SetupGetManufacturersOkNotAllActive()
        {
            var manufacturers = new List<ViewOrganisationDto> 
            { 
                new ViewOrganisationDto{ Status = OrganisationConstants.Status.Retired} 
            };
            var manufacturersResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), manufacturers, null);
            _mockOrganisationService.Setup(x => x.GetManufacturers(It.IsAny<string>())).ReturnsAsync(manufacturersResponse);
        }
        private void SetupGetManufacturersOkSomeActive()
        {
            var manufacturers = new List<ViewOrganisationDto>
            {
                new ViewOrganisationDto{ Status = OrganisationConstants.Status.Retired},
                new ViewOrganisationDto{ Id = _organisationId, Status = OrganisationConstants.Status.Active},
                new ViewOrganisationDto{ Id = _organisation2Id, Status = OrganisationConstants.Status.Active}
            };
            var manufacturersResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), manufacturers, null);
            _mockOrganisationService.Setup(x => x.GetManufacturers(It.IsAny<string>())).ReturnsAsync(manufacturersResponse);
        }

        private void SetupGetObligationCalculationsBadRequest()
        {
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);
        }

        private void SetupGetObligationCalculationsOk()
        {
            var obligationCalculations = new ObligationCalculationsDto { GasCreditsCap = 300, OilCreditsCap = 50, PercentageCap = 35m, TargetMultiplier = 1.0m};
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);
        }


        private void SetupGetAllBoilerSalesSummaryBadRequest()
        {
            var mockGetBoilerSalesResponse = new HttpObjectResponse<List<BoilerSalesSummaryDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _mockBoilerSalesService.Setup(x => x.GetAllBoilerSalesSummary(It.IsAny<Guid>())).ReturnsAsync(mockGetBoilerSalesResponse);
        }

        private void SetupGetAllBoilerSalesSummaryOkEmptyList()
        {
            var boilerSalesSummaries = new List<BoilerSalesSummaryDto>();
            var mockGetBoilerSalesResponse = new HttpObjectResponse<List<BoilerSalesSummaryDto>>(new HttpResponseMessage(HttpStatusCode.OK), boilerSalesSummaries, null);
            _mockBoilerSalesService.Setup(x => x.GetAllBoilerSalesSummary(It.IsAny<Guid>())).ReturnsAsync(mockGetBoilerSalesResponse);
        }

        private void SetupGetAllBoilerSalesSummaryOkAnnualSalesNotApproved()
        {
            var boilerSalesSummaries = new List<BoilerSalesSummaryDto> 
            { 
                new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, BoilerSalesSummarySubmissionStatus.QuarterSubmitted),
                new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, BoilerSalesSummarySubmissionStatus.AnnualSubmitted)
            };
            var mockGetBoilerSalesResponse = new HttpObjectResponse<List<BoilerSalesSummaryDto>>(new HttpResponseMessage(HttpStatusCode.OK), boilerSalesSummaries, null);
            _mockBoilerSalesService.Setup(x => x.GetAllBoilerSalesSummary(It.IsAny<Guid>())).ReturnsAsync(mockGetBoilerSalesResponse);
        }

        private void SetupGetAllBoilerSalesSummaryOkApprovedAnnualSales()
        {
            var boilerSalesSummaries = new List<BoilerSalesSummaryDto>
            {
                //new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, false, false),
                new BoilerSalesSummaryDto(_organisationId, 20000, 1050, 0, 0, BoilerSalesSummarySubmissionStatus.AnnualApproved),
                new BoilerSalesSummaryDto(_organisation2Id, 0, 15000, 0, 0, BoilerSalesSummarySubmissionStatus.AnnualApproved)
            };
            var mockGetBoilerSalesResponse = new HttpObjectResponse<List<BoilerSalesSummaryDto>>(new HttpResponseMessage(HttpStatusCode.OK), boilerSalesSummaries, null);
            _mockBoilerSalesService.Setup(x => x.GetAllBoilerSalesSummary(It.IsAny<Guid>())).ReturnsAsync(mockGetBoilerSalesResponse);
        }




        private void SetupGetAllCreditBalancesBadRequest()
        {
            var badRequestResponse = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, new Chmm.Common.ValueObjects.ProblemDetails(500, "Error"));
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(badRequestResponse);
        }

        private void SetupGetAllCreditBalancesOkEmptyList()
        {
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(
                new HttpResponseMessage(HttpStatusCode.OK), 
                new List<OrganisationCreditBalanceDto>(), 
                null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);
        }

        private void SetupGetAllCreditBalancesOkSingleOrganisation()
        {
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, 0)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);
        }

        private void SetupGetAllCreditBalancesOk()
        {
            var creditBalance = new HttpObjectResponse<List<OrganisationCreditBalanceDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<OrganisationCreditBalanceDto>
            {
                new OrganisationCreditBalanceDto(_organisationId, 0),
                new OrganisationCreditBalanceDto(_organisation2Id, 0)
            }, null);
            _mockCreditLedgerService.Setup(x => x.GetAllCreditBalances(SchemeYearConstants.Id, It.IsAny<string>())).ReturnsAsync(creditBalance);
        }

        private void SetupNullObligationTransactions()
        {
            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, _organisationId, _currentSchemeYearId, 0, DateTime.MinValue);
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)),
                                                           It.IsAny<RepositoryConstants.SortOrder>(),
                                                           It.IsAny<bool>())).Returns(Task.FromResult<List<Transaction>>(null));
        }

        private void SetupNoObligationTransactionsAndSomeExistingTransactions()
        {
            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, _organisationId, _currentSchemeYearId, 0, DateTime.MinValue);
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)),
                                                                   It.IsAny<RepositoryConstants.SortOrder>(),
                                                                   It.IsAny<bool>()))
                .Returns(Task.FromResult(new List<Transaction>()));


            var existingTransaction = new Transaction(Guid.NewGuid(), TransactionType.BroughtFowardFromPreviousYear, _organisationId, _nextSchemeYearId, 0, DateTime.MinValue);
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<Expression<Func<Transaction, bool>>>(y => y.Compile()(existingTransaction)),
                                                                   It.IsAny<RepositoryConstants.SortOrder>(),
                                                                   It.IsAny<bool>()))
                .Returns(Task.FromResult(new List<Transaction> { existingTransaction }));
        }

        private void SetupSomeObligationTransactionsAndNoExistingTransactions()
        {
            var annualTransaction = new Transaction(Guid.NewGuid(), TransactionType.AnnualSubmission, _organisationId, _currentSchemeYearId, 0, DateTime.MinValue);
            var existingTransaction = new Transaction(Guid.NewGuid(), TransactionType.BroughtFowardFromPreviousYear, _organisationId, _nextSchemeYearId, 0, DateTime.MinValue);

            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<Expression<Func<Transaction, bool>>>(y => y.Compile()(annualTransaction)),
                                                                   It.IsAny<RepositoryConstants.SortOrder>(),
                                                                   It.IsAny<bool>()))
                .Returns(Task.FromResult(new List<Transaction> { existingTransaction }));

            
            _mockTransactionRepository.Setup(x => x.GetAll(It.Is<Expression<Func<Transaction, bool>>>(y => y.Compile()(existingTransaction)),
                                                                   It.IsAny<RepositoryConstants.SortOrder>(),
                                                                   It.IsAny<bool>()))
                .Returns(Task.FromResult(new List<Transaction> ()));
        }
        #endregion
    }
}
