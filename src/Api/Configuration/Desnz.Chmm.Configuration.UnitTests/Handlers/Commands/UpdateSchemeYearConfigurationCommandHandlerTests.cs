using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Desnz.Chmm.Configuration.UnitTests.Handlers.Commands;

public class UpdateSchemeYearConfigurationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISchemeYearRepository> _mockSchemeYearRepository;
    private readonly Mock<ILogger<UpdateSchemeYearConfigurationCommandHandler>> _mockLogger;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly UpdateSchemeYearConfigurationCommandHandler _handler;

    public UpdateSchemeYearConfigurationCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _mockSchemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);
        _mockLogger = new Mock<ILogger<UpdateSchemeYearConfigurationCommandHandler>>();

        _dateTimeProvider = new DateTimeOverrideProvider();

        _mockSchemeYearRepository.Setup(x => x.UnitOfWork).Returns(_mockUnitOfWork.Object);

        _handler = new UpdateSchemeYearConfigurationCommandHandler(_mockLogger.Object, _dateTimeProvider, _mockSchemeYearRepository.Object);
    }

    [Fact]
    public async void ShouldReturnNotFoundResponse_When_SchemeYearNotFound()
    {
        // Arrange
        var schemeYearId = Guid.NewGuid();
        var request = new UpdateSchemeYearConfigurationCommand
        {
            SchemeYearId = schemeYearId
        };

        _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), true, true, true)).ReturnsAsync((SchemeYear?)null);

        var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {schemeYearId}");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async void ShouldReturnBadRequestResponse_When_InSchemeYear()
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
        var schemeYearId = currentYear.Id;

        var request = new UpdateSchemeYearConfigurationCommand
        {
            SchemeYearId = schemeYearId
        };
        _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), true, true, true)).ReturnsAsync(currentYear);
        
        var expectedResult = Responses.BadRequest($"Cannot edit the configuration of Scheme Year with Id: {schemeYearId}");

        _dateTimeProvider.OverrideDate(date.AddDays(1));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async void ShouldReturnNoContent_When_SchemeYearUpdatedSuccessfully()
    {
        // Arrange
        var date = new DateOnly(2024, 2, 13);
        var quarters = new List<SchemeYearQuarter>
        {
            new SchemeYearQuarter("Test quarter", date, date)
            };
        var rValue = new AlternativeSystemFuelTypeWeightingValue(1M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable);
        var fValue = new AlternativeSystemFuelTypeWeightingValue(0.5M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel);
        var weightings = new CreditWeighting(
                1,
                new List<HeatPumpTechnologyTypeWeighting>
                {
                    new HeatPumpTechnologyTypeWeighting("Test HP weighting", 1)
                },
                new List<AlternativeSystemFuelTypeWeighting>
                {
                    new AlternativeSystemFuelTypeWeighting("Test AF weighting", rValue),
                    new AlternativeSystemFuelTypeWeighting("Test AF weighting", fValue)
                }
            );

        var obligationCalculations = new ObligationCalculations(1, 1, 1, 1, 1, 1, 1m, 1m);

        var currentYear = new SchemeYear("2024", 2024, date, date, date, date, date, date, date, date, quarters, obligationCalculations, weightings);
        var schemeYearId = currentYear.Id;

        _dateTimeProvider.OverrideDate(date.AddDays(-1));

        var request = new UpdateSchemeYearConfigurationCommand
        {
            SchemeYearId = schemeYearId,
            CreditCarryOverPercentage = 1001,
            GasBoilerSalesThreshold = 1002,
            OilBoilerSalesThreshold = 1003,
            PercentageCap = 1004,
            TargetMultiplier = 1005,
            TargetRate = 1006
        };

        var expectedResult = Responses.NoContent();

        _mockSchemeYearRepository.Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), true, true, true)).ReturnsAsync(currentYear);
        _mockSchemeYearRepository.Setup(x => x.GetAlternativeSystemFuelTypeWeightingValueById(rValue.Id, It.IsAny<bool>())).ReturnsAsync(rValue);
        _mockSchemeYearRepository.Setup(x => x.GetAlternativeSystemFuelTypeWeightingValueById(fValue.Id, It.IsAny<bool>())).ReturnsAsync(fValue);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        currentYear.ObligationCalculations.CreditCarryOverPercentage.Should().Be(request.CreditCarryOverPercentage);
        currentYear.ObligationCalculations.GasBoilerSalesThreshold.Should().Be(request.GasBoilerSalesThreshold);
        currentYear.ObligationCalculations.OilBoilerSalesThreshold.Should().Be(request.OilBoilerSalesThreshold);
        currentYear.ObligationCalculations.PercentageCap.Should().Be(request.PercentageCap);
        currentYear.ObligationCalculations.TargetMultiplier.Should().Be(request.TargetMultiplier);
        currentYear.ObligationCalculations.TargetRate.Should().Be(request.TargetRate);
    }
}
