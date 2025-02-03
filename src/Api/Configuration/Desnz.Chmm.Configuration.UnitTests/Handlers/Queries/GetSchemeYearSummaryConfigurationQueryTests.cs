using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Handlers.Queries;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Handlers.Queries
{
    public class GetSchemeYearSummaryConfigurationQueryTests : HandlerTestsMockerBase
    {
        private readonly Mock<ILogger<BaseRequestHandler<GetSchemeYearSummaryConfigurationQuery, ActionResult<SchemeYearSummaryConfigurationDto>>>> _mockLogger;
        private readonly SchemeYear _previousSchemeYear;
        private readonly SchemeYear _schemeYear;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private GetSchemeYearSummaryConfigurationQueryHandler _handler;

        public GetSchemeYearSummaryConfigurationQueryTests()
        {
            _mockLogger = new Mock<ILogger<BaseRequestHandler<GetSchemeYearSummaryConfigurationQuery, ActionResult<SchemeYearSummaryConfigurationDto>>>>();

            _previousSchemeYear = GenerateSchemeYear();
            var values = _previousSchemeYear.GenerateFuelTypeWeightingValues();
            _schemeYear = _previousSchemeYear.GenerateNext(values);
            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);
            SchemeYear? nullSchemeYear = null;
            _schemeYearRepository
                .Setup(x => x.GetSchemeYearById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(nullSchemeYear);
            _schemeYearRepository
                .Setup(x => x.GetSchemeYearById(_previousSchemeYear.Id, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(_previousSchemeYear);
            _schemeYearRepository
                .Setup(x => x.GetSchemeYearById(_schemeYear.Id, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(_schemeYear);

            _dateTimeProvider = new DateTimeOverrideProvider();

            _handler = new GetSchemeYearSummaryConfigurationQueryHandler(_mockLogger.Object, _schemeYearRepository.Object, _dateTimeProvider);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_SchemeYearNotFound()
        {
            //Arrange
            var schemeYearId = Guid.NewGuid();

            var query = new GetSchemeYearSummaryConfigurationQuery(schemeYearId);

            var expectedErrorMsg = $"Failed to get Scheme Year with Id: {schemeYearId}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task Should_ReturnSchemeYearName()
        {
            //Arrange
            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(_schemeYear.Name, result.Value.CurrentSchemeYear);
        }

        [Fact]
        internal async Task Should_ReturnObligationCalculationPercentage()
        {
            //Arrange
            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(_schemeYear.ObligationCalculations.TargetRate, result.Value.ObligationCalculationPercentage);
        }

        [Fact]
        internal async Task Should_ReturnBeforeEndOfSchemeYear_WhenBeforeEndOfSchemeYear()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.EndDate);

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsAfterEndOfSchemeYearDate);
        }

        [Fact]
        internal async Task Should_ReturnAfterEndOfSchemeYear_WhenAfterEndOfSchemeYear()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.EndDate.AddDays(1));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsAfterEndOfSchemeYearDate);
        }

        [Fact]
        internal async Task Should_ReturnBeforeSurrenderDay_WhenBeforeSurrenderDay()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate);

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsAfterSurrenderDay);
        }

        [Fact]
        internal async Task Should_ReturnAfterSurrenderDay_WhenAfterSurrenderDay()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate.AddDays(1));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsAfterSurrenderDay);
        }

        [Fact]
        internal async Task Should_ReturnAfterPreviousSchemeYearSurrenderDate_WhenAfterPreviousSchemeYearSurrenderDate()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_previousSchemeYear.SurrenderDayDate.AddDays(1));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsAfterPreviousSchemeYearSurrenderDate);
        }

        [Fact]
        internal async Task Should_ReturnBeforePreviousSchemeYearSurrenderDate_WhenBeforePreviousSchemeYearSurrenderDate()
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_previousSchemeYear.SurrenderDayDate.AddDays(-1));

            var query = new GetSchemeYearSummaryConfigurationQuery(_previousSchemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsAfterPreviousSchemeYearSurrenderDate);
        }

        [Fact]
        internal async Task Should_ReturnBeforePreviousSchemeYearSurrenderDate_WhendNoPreviousYear()
        {
            //Arrange
            var query = new GetSchemeYearSummaryConfigurationQuery(_previousSchemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsAfterPreviousSchemeYearSurrenderDate);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        internal async Task Should_ReturnIsNotWithinAmendObligationsWindow_WhenBeforeStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.StartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinAmendObligationsWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        internal async Task Should_ReturnIsNotWithinAmendObligationsWindow_WhenOnOrAfterSurrenderDayDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinAmendObligationsWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal async Task Should_ReturnIsWithinAmendObligationsWindow_WhenOnOrAfterStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.StartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinAmendObligationsWindow);
        }

        [Theory]
        [InlineData(-1)]
        internal async Task Should_ReturnIsWithinAmendObligationsWindow_WhenBeforeSurrenderDayDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinAmendObligationsWindow);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        internal async Task Should_ReturnIsAmendCreditsWindow_WhenBeforeStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.StartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinAmendCreditsWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        internal async Task Should_ReturnIsNotAmendCreditsWindow_WhenOnOrAfterSurrenderDayDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinAmendCreditsWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal async Task Should_ReturnIsWithinAmendCreditsWindow_WhenOnOrAfterStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.StartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinAmendCreditsWindow);
        }

        [Theory]
        [InlineData(-1)]
        internal async Task Should_ReturnIsWithinAmendCreditsWindow_WhenBeforeSurrenderDayDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.SurrenderDayDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinAmendCreditsWindow);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        internal async Task Should_ReturnIsNotWithinCreditTranferWindow_WhenBeforeTradingWindowStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.TradingWindowStartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinCreditTranferWindow);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        internal async Task Should_ReturnIsNotWithinCreditTranferWindow_WhenAfterTradingWindowEndDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.TradingWindowEndDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(false, result.Value.IsWithinCreditTranferWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal async Task Should_ReturnIsWithinCreditTranferWindow_WhenOnOrAfterTradingWindowStartDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.TradingWindowStartDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinCreditTranferWindow);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        internal async Task Should_ReturnIsWithinCreditTranferWindow_WhenOnOrBeforeTradingWindowEndDate(int addDays)
        {
            //Arrange
            _dateTimeProvider.OverrideDate(_schemeYear.TradingWindowEndDate.AddDays(addDays));

            var query = new GetSchemeYearSummaryConfigurationQuery(_schemeYear.Id);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.Equal(true, result.Value.IsWithinCreditTranferWindow);
        }
    }
}
