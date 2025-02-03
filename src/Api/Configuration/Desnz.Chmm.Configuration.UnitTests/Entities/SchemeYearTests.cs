using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Entities;
using FluentAssertions;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Entities
{
    public class SchemeYearTests
    {
        [Fact]
        public void ShouldGenerateNextYearFromCurrent()
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
                    });

            var obligationCalculations = new ObligationCalculations(1, 1, 1, 1,1, 1, 1m, 1m);
            var schemeYear = new SchemeYear("2024", 2024, date, date, date, date, date, date, date, date, quarters, obligationCalculations, weightings);

            var expectedDate = new DateOnly(2025, 2, 13);

            // Act
            var values = schemeYear.GenerateFuelTypeWeightingValues();
            var result = schemeYear.GenerateNext(values);

            // Assert
            result.Name.Should().Be("2025");
            result.Year.Should().Be(2025);
            result.StartDate.Should().Be(expectedDate);
            result.EndDate.Should().Be(expectedDate);
            result.TradingWindowStartDate.Should().Be(expectedDate);
            result.TradingWindowEndDate.Should().Be(expectedDate);
            result.CreditGenerationWindowStartDate.Should().Be(expectedDate);
            result.CreditGenerationWindowEndDate.Should().Be(expectedDate);
            result.BoilerSalesSubmissionEndDate.Should().Be(expectedDate);

            result.Quarters.Count.Should().Be(1);
            result.CreditWeightings.Should().NotBeNull();
            result.CreditWeightings.TotalCapacity.Should().Be(1);

            var quarter = result.Quarters.First();
            quarter.Name.Should().Be("Test quarter");
            quarter.StartDate.Should().Be(expectedDate);
            quarter.EndDate.Should().Be(expectedDate);

            var weighting = result.CreditWeightings;
            weighting.AlternativeSystemFuelTypeWeightings.Count.Should().Be(1);
            weighting.HeatPumpTechnologyTypeWeightings.Count.Should().Be(1);

            var alternativeSystemFuelTypeWeighting = weighting.AlternativeSystemFuelTypeWeightings.First();
            alternativeSystemFuelTypeWeighting.AlternativeSystemFuelTypeWeightingValue.Value.Should().Be(1);
            alternativeSystemFuelTypeWeighting.Code.Should().Be("Test AF weighting");

            var heatPumpTechnologyTypeWeightings = weighting.HeatPumpTechnologyTypeWeightings.First();
            heatPumpTechnologyTypeWeightings.Value.Should().Be(1);
            heatPumpTechnologyTypeWeightings.Code.Should().Be("Test HP weighting");
        }
    }
}
