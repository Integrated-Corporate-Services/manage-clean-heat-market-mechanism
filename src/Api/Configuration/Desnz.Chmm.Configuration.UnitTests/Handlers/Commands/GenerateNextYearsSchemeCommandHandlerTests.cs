using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Handlers.Commands;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Commands;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Handlers.Commands
{
    public class GenerateNextYearsSchemeCommandHandlerTests
    {
        private readonly Mock<ISchemeYearRepository> _mockSchemeYearRepository;
        private readonly Mock<ILogger<GenerateNextYearsSchemeCommandHandler>> _mockLogger;

        private GenerateNextYearsSchemeCommandHandler _handler;

        public GenerateNextYearsSchemeCommandHandlerTests()
        {
            _mockSchemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<GenerateNextYearsSchemeCommandHandler>>();

            _handler = new GenerateNextYearsSchemeCommandHandler(_mockLogger.Object, _mockSchemeYearRepository.Object);
        }

        [Fact]
        public async void ShouldReturnNotFoundResponse_When_SchemeYearNotFound()
        {
            // Arrange
            var previousSchemeYearId = Guid.NewGuid();

            _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), true, true, false)).ReturnsAsync((SchemeYear?)null);
            
            var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {previousSchemeYearId}");

            // Act
            var result = await _handler.Handle(new GenerateNextYearsSchemeaCommand(previousSchemeYearId), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async void ShouldReturnTheSchemeYearId_When_NextYearsSchemeCreatedSuccessfully()
        {
            // Arrange
            var date = new DateOnly(2024, 2, 13);
            var quarters = new List<SchemeYearQuarter>
            {
                new SchemeYearQuarter("Test quarter", date, date)
            };
            var value = new AlternativeSystemFuelTypeWeightingValue(1M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable);
            var weightings = new CreditWeighting(
                    1,
                    new List<HeatPumpTechnologyTypeWeighting>
                    {
                        new HeatPumpTechnologyTypeWeighting("Test HP weighting", 1)
                    },
                    new List<AlternativeSystemFuelTypeWeighting>
                    {
                        new AlternativeSystemFuelTypeWeighting("Test AF weighting", value)
                    }
                );

            var obligationCalculations = new ObligationCalculations(1, 1, 1, 1, 1, 1, 1m, 1m);

            var currentYear = new SchemeYear("2024", 2024, date, date, date, date, date, date, date, date, quarters, obligationCalculations, weightings);

            var nextYearId = Guid.NewGuid();
            var expectedResult = Responses.Created(nextYearId);

            _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), true, true, false)).ReturnsAsync(currentYear);
            _mockSchemeYearRepository.Setup(x => x.CreateFuelTypeWeightingValues(It.IsAny<List<AlternativeSystemFuelTypeWeightingValue>>(), true)).ReturnsAsync(new List<AlternativeSystemFuelTypeWeightingValue> { value });
            _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), false, false, false)).ReturnsAsync(null as SchemeYear);
            _mockSchemeYearRepository.Setup(x => x.Create(It.IsAny<SchemeYear>(), true)).ReturnsAsync(nextYearId);

            // Act
            var result = await _handler.Handle(new GenerateNextYearsSchemeaCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
