using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using static Desnz.Chmm.Common.Constants.McsConstants;

namespace Desnz.Chmm.CreditLedger.UnitTests.Services;

public class CreditLedgerCalculator_Calculate_Tests
{
    private readonly Mock<ILogger<CreditLedgerCalculator>> _mockLogger;
    private readonly ICreditLedgerCalculator _calculator;
    public CreditLedgerCalculator_Calculate_Tests()
    {        
        _mockLogger = new Mock<ILogger<CreditLedgerCalculator>>();
        _calculator = new CreditLedgerCalculator(
            _mockLogger.Object);
    }

    [Fact]
    public void Calculate_Credit_Standalone_NoProducts()
    {
        var installation = GetZeroCreditDto();
        installation.HeatPumpProducts = new List<McsProductDto>();
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0, credit);
    }

    [Fact]
    public void Calculate_Credit_Standalone_NULL_Products()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();
        installation.HeatPumpProducts = null;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0, credit);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 1)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 1)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 1)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 1)]
    public void Calculate_Credit_Standalone_Not_Air_to_Air(int technologyTypeId, int heatPumpAirSourceTypeId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();


        int[] creditWorthyRenewableSystemDesigns = new int[]
            {
                RenewableSystemDesigns.SpaceHeatAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatDhwAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatAndDhw,
                RenewableSystemDesigns.SpaceHeatOnly
            };

        int assertedEqualRsd = 0;

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = false;
        
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;

        foreach (var rsd in creditWorthyRenewableSystemDesigns)
        {
            installation.RenewableSystemDesignId = rsd;
            var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
            Assert.Equal((decimal)expectedCredit, credit);

            assertedEqualRsd++;
        }
        Assert.Equal(creditWorthyRenewableSystemDesigns.Length, assertedEqualRsd);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    public void Calculate_Credit_Standalone_Exceeded_IndividualCapacity(int technologyTypeId, int heatPumpAirSourceTypeId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 46;
        installation.IsNewBuildId = NewBuildOptions.Yes;
        installation.IsHybrid = false;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }


    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    public void Calculate_Credit_Standalone_Exceeded_TotalCapacity(int technologyTypeId, int heatPumpAirSourceTypeId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 71;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = false;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Fact]
    public void Calculate_Credit_Standalone_NULL_TotalCapacity()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();

        installation.TotalCapacity = null;
        installation.IsNewBuildId = NewBuildOptions.No;

        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, 0)]
    public void Calculate_Credit_Standalone_Zero_When_NewBuild(int technologyTypeId, int heatPumpAirSourceTypeId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.Yes;
        installation.IsHybrid = false;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Fact]
    public void Calculate_Credit_Standalone_NULL_IsNewBuild()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);
        var installation = GetZeroCreditDto();

        installation.IsNewBuildId = null;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = HeatPumpTechnologyTypes.SolarAssistedHeatPump;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 1)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToAirSource, 1)]
    public void Calculate_Credit_Standalone_Air_to_Air(int technologyTypeId, int heatPumpAirSourceTypeId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = false;
        
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;

        int[] creditWorthyRenewableSystemDesigns = new int[]
            {
                RenewableSystemDesigns.SpaceHeatAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatDhwAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatAndDhw,
                RenewableSystemDesigns.SpaceHeatOnly
            };

        int assertedEqualRsd = 0;
        foreach (var rsd in creditWorthyRenewableSystemDesigns)
        {
            installation.RenewableSystemDesignId = rsd;
            var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
            Assert.Equal((decimal)expectedCredit, credit);
            assertedEqualRsd++;
        }
        Assert.Equal(creditWorthyRenewableSystemDesigns.Length, assertedEqualRsd);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToWaterSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    public void Calculate_Credit_Standalone_AnotherPurposeOnly_RenewableSystemDesign(int technologyTypeId, int heatPumpAirSourceTypeId, int renewableSystemDesignId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = false;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;
        installation.RenewableSystemDesignId = renewableSystemDesignId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, true, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GroundWaterSourceHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.SolarAssistedHeatPump, false, RenewableSystemDesigns.AnotherPurposeOnly, 0)]
    public void Calculate_Credit_Either_Standalone_Or_Hybrid_AnotherPurposeOnly_RenewableSystemDesign(int technologyTypeId, bool isHybrid, int renewableSystemDesignId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = isHybrid;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.RenewableSystemDesignId = renewableSystemDesignId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Fact]
    public void Calculate_Credit_Standalone_All_Non_Air_to_Air_Installations()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        int[] technologyTypes = new int[]
        {
            HeatPumpTechnologyTypes.AirSourceHeatPump,
            HeatPumpTechnologyTypes.ExhaustAirHeatPump,
            HeatPumpTechnologyTypes.GroundWaterSourceHeatPump,
            HeatPumpTechnologyTypes.SolarAssistedHeatPump
        };

        int[] renewableSystemDesigns = new int[]
        {
            RenewableSystemDesigns.SpaceHeatAndAnotherPurpose,
            RenewableSystemDesigns.SpaceHeatDhwAndAnotherPurpose,
            RenewableSystemDesigns.SpaceHeatAndDhw,
            RenewableSystemDesigns.SpaceHeatOnly,
        };

        var assertedEqual = 0;
        foreach (var (tt, rsd) in technologyTypes.SelectMany(tt => renewableSystemDesigns.Select(rsd => (tt, rsd))))
        {
            var installation = GetZeroCreditDto();

            installation.TotalCapacity = 45;
            installation.IsNewBuildId = NewBuildOptions.No;
            installation.IsHybrid = false;
            installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

            installation.TechnologyTypeId = tt;
            installation.RenewableSystemDesignId = rsd;
            installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;

            var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
            Assert.Equal(1m, credit);
            assertedEqual++;
        }

        Assert.Equal(technologyTypes.Length * renewableSystemDesigns.Length, assertedEqual);
    }

    [Theory]
    [InlineData(HeatPumpTechnologyTypes.AirSourceHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.DhwOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.ExhaustAirHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.DhwOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.GasAbsorbtionHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.DhwOnly, 0)]
    [InlineData(HeatPumpTechnologyTypes.HotWaterHeatPump, HeatPumpAirSourceTypes.AirToAirSource, RenewableSystemDesigns.DhwOnly, 0)]
    public void Calculate_Credit_Hybrid_ZeroCredit_TechnologyTypes(int technologyTypeId, int heatPumpAirSourceTypeId, int renewableSystemDesignId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = technologyTypeId;
        installation.AirTypeTechnologyId = heatPumpAirSourceTypeId;
        installation.RenewableSystemDesignId = renewableSystemDesignId;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Fact]
    public void Calculate_Credit_Hybrid_NULL_TechnologyType()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = null;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Fact]
    public void Calculate_Credit_Hybrid_Unknown_TechnologyType()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = 999;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }


    [Fact]
    public void Calculate_Credit_Hybrid_Unknown_AlternativeSystemFuelType()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = HeatPumpTechnologyTypes.GroundWaterSourceHeatPump;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.DhwOnly;
        installation.AlternativeHeatingFuelId = 999;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.IsSystemSelectedAsMCSTechnology = false;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Fact]
    public void Calculate_Credit_Hybrid_NULL_RenewableSystemDesign()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.RenewableSystemDesignId = null;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Fact]
    public void Calculate_Credit_Hybrid_NULL_AlternativeHeatingSystem()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = HeatPumpTechnologyTypes.GroundWaterSourceHeatPump;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.SpaceHeatAndAnotherPurpose;
        installation.AlternativeHeatingSystemId = null;
        installation.AlternativeHeatingFuelId = AlternativeSystemFuelTypes.MainsGas;
        installation.IsSystemSelectedAsMCSTechnology = false;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Fact]
    public void Calculate_Credit_Hybrid_NULL_AlternativeHeatingSystemFuel()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.IsSystemSelectedAsMCSTechnology = false;
        installation.TechnologyTypeId = HeatPumpTechnologyTypes.GroundWaterSourceHeatPump;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.SpaceHeatAndAnotherPurpose;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.AlternativeHeatingFuelId = null;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }


    [Fact]
    public void Calculate_Credit_Hybrid_NULL_IsSystemSelectedAsMCSTechnology()
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;
        installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

        installation.TechnologyTypeId = HeatPumpTechnologyTypes.GroundWaterSourceHeatPump;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.SpaceHeatAndAnotherPurpose;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.AlternativeHeatingFuelId = AlternativeSystemFuelTypes.Oil;
        installation.IsSystemSelectedAsMCSTechnology = null;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());
        Assert.Equal(0m, credit);
    }

    [Theory]
    [InlineData(AlternativeSystemFuelTypes.Anthracite, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.BiogasLandfillCommunityHeatingOnly, 0)]
    [InlineData(AlternativeSystemFuelTypes.BulkWoodPellets, 1)]
    [InlineData(AlternativeSystemFuelTypes.Coal, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.DualFuelMineralAndWood, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.Electricity, 1)]
    [InlineData(AlternativeSystemFuelTypes.MainsGas, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.Oil, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.WasteCombustionCommunityHeatingOnly, 0)]
    [InlineData(AlternativeSystemFuelTypes.WoodChips, 1)]
    [InlineData(AlternativeSystemFuelTypes.WoodLogs, 1)]
    [InlineData(AlternativeSystemFuelTypes.NotApplicableNoOtherHeatingSource, 1)]
    [InlineData(AlternativeSystemFuelTypes.BioliquidHvoBioLpg, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.Solar, 1)]
    [InlineData(AlternativeSystemFuelTypes.Other, 1)]
    [InlineData(AlternativeSystemFuelTypes.LPG, 0.5)]

    public async void Calculate_Credit_Hybrid_NonZeroCredit_TechnologyType(int alternativeHeatingFuelId, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        int[] creditWorthyTechnologyTypes = new int[]
            {
                HeatPumpTechnologyTypes.AirSourceHeatPump,
                HeatPumpTechnologyTypes.ExhaustAirHeatPump,
                HeatPumpTechnologyTypes.GroundWaterSourceHeatPump,
                HeatPumpTechnologyTypes.SolarAssistedHeatPump
            };

        int[] creditWorthyRenewableSystemDesigns = new int[]
            {
                RenewableSystemDesigns.SpaceHeatAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatDhwAndAnotherPurpose,
                RenewableSystemDesigns.SpaceHeatAndDhw,
                RenewableSystemDesigns.SpaceHeatOnly
            };

        int assertedEqualRsd = 0;
        foreach (var rsd in creditWorthyRenewableSystemDesigns)
        {
            int assertedEqualTt = 0;
            foreach (var tt in creditWorthyTechnologyTypes)
            {
                var installation = GetZeroCreditDto();

                installation.TotalCapacity = 45;
                installation.IsNewBuildId = NewBuildOptions.No;
                installation.IsHybrid = true;
                installation.HeatPumpProducts = new List<McsProductDto> { new McsProductDto { } };

                installation.TechnologyTypeId = tt;
                installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
                installation.RenewableSystemDesignId = rsd;
                installation.AlternativeHeatingFuelId = alternativeHeatingFuelId;
                installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
                installation.IsSystemSelectedAsMCSTechnology = false;

                var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());

                Assert.Equal((decimal)expectedCredit, credit);

                assertedEqualTt++;
            }

            Assert.Equal(creditWorthyTechnologyTypes.Length, assertedEqualTt);
            assertedEqualRsd++;
        }
        Assert.Equal(creditWorthyRenewableSystemDesigns.Length, assertedEqualRsd);
    }

    [Theory]
    [InlineData(AlternativeSystemFuelTypes.LPG, 0, 0)]
    [InlineData(AlternativeSystemFuelTypes.LPG, 1, 0.5)]
    [InlineData(AlternativeSystemFuelTypes.LPG, 2, 1.0)]
    [InlineData(AlternativeSystemFuelTypes.LPG, 3, 1.5)]

    public async void Calculate_Credit_Hybrid_NonZeroCredit_TechnologyType_MultipleLicenceHolders(int alternativeHeatingFuelId, int licenceHoldersCount, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = true;

        installation.HeatPumpProducts = Enumerable.Range(1, licenceHoldersCount).Select(x => new McsProductDto()).ToList();
        installation.TechnologyTypeId = HeatPumpTechnologyTypes.SolarAssistedHeatPump;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.SpaceHeatOnly;
        installation.AlternativeHeatingFuelId = alternativeHeatingFuelId;
        installation.AlternativeHeatingSystemId = AlternativeSystemTypes.BackBoiler;
        installation.IsSystemSelectedAsMCSTechnology = false;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());

        Assert.Equal((decimal)expectedCredit, credit);

    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void Calculate_Credit_Standalone_All_Non_Air_to_Air_Installations_MultipleLicenceHolders(int licenceHoldersCount, double expectedCredit)
    {
        var weightings = GenerateCreditWeightings(SchemeYearConstants.Id);

        var installation = GetZeroCreditDto();

        installation.TotalCapacity = 45;
        installation.IsNewBuildId = NewBuildOptions.No;
        installation.IsHybrid = false;
        installation.HeatPumpProducts = Enumerable.Range(1, licenceHoldersCount).Select(x => new McsProductDto()).ToList();

        installation.TechnologyTypeId = HeatPumpTechnologyTypes.SolarAssistedHeatPump;
        installation.RenewableSystemDesignId = RenewableSystemDesigns.SpaceHeatOnly;
        installation.AirTypeTechnologyId = HeatPumpAirSourceTypes.AirToWaterSource;

        var credit = _calculator.Calculate(installation, weightings.ToWeightingDictionary());

        Assert.Equal((decimal)expectedCredit, credit);
    }

    [Fact(Skip = "Used to generate the JSON string")]
    public void Can_SerializeObject()
    {
        var value = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0.2M };

        var data = new List<CreditWeightingsDto> { new CreditWeightingsDto { SchemeYearId = SchemeYearConstants.Id, HeatPumpTechnologyTypeWeightings = new List<HeatPumpTechnologyTypeWeightingDto> { new HeatPumpTechnologyTypeWeightingDto { Code = "C1", Value = 0.1m} }, AlternativeSystemFuelTypeWeightings = new List<AlternativeSystemFuelTypeWeightingDto> { new AlternativeSystemFuelTypeWeightingDto { Code = "C2", AlternativeSystemFuelTypeWeightingValue = value } } },
                                                   new CreditWeightingsDto { SchemeYearId = SchemeYearConstants.Year2025Id, HeatPumpTechnologyTypeWeightings = new List<HeatPumpTechnologyTypeWeightingDto> { new HeatPumpTechnologyTypeWeightingDto { Code = "C1", Value = 0.1m} }, AlternativeSystemFuelTypeWeightings = new List<AlternativeSystemFuelTypeWeightingDto> { new AlternativeSystemFuelTypeWeightingDto { Code = "C2", AlternativeSystemFuelTypeWeightingValue = value } } } };
        var s = JsonConvert.SerializeObject(data);
    }

    #region Private methods
    private CreditWeightingsDto GenerateCreditWeightings(Guid id)
    {
        var zeroValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0M };
        var halfValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 0.5M };
        var wholeValue = new AlternativeSystemFuelTypeWeightingValueDto { Value = 1M };

        return new CreditWeightingsDto()
        {
            Id = Guid.NewGuid(),
            TotalCapacity = 70,
            SchemeYearId = id,
            AlternativeSystemFuelTypeWeightings = new List<AlternativeSystemFuelTypeWeightingDto>
                {
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Anthracite",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BiogasLandfillCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BulkWoodPellets",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Coal",
                        AlternativeSystemFuelTypeWeightingValue =   halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "DualFuelMineralAndWood",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Electricity",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "MainsGas",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Oil",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WasteCombustionCommunityHeatingOnly",
                        AlternativeSystemFuelTypeWeightingValue =  zeroValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodChips",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "WoodLogs",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "NotApplicableNoOtherHeatingSource",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "BioliquidHvoBioLpg",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Solar",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "Other",
                        AlternativeSystemFuelTypeWeightingValue =  wholeValue
                    },
                    new AlternativeSystemFuelTypeWeightingDto{
                        Code = "LPG",
                        AlternativeSystemFuelTypeWeightingValue = halfValue
                    }
                },
            HeatPumpTechnologyTypeWeightings = new List<HeatPumpTechnologyTypeWeightingDto>
                {
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "AirSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "ExhaustAirHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GasAbsorbtionHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "GroundWaterSourceHeatPump",
                        Value =  1
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "HotWaterHeatPump",
                        Value =  0
                    },
                    new HeatPumpTechnologyTypeWeightingDto {
                        Code =  "SolarAssistedHeatPump",
                        Value =  1
                    }
                }
        };
    }

    private CreditCalculationDto GetZeroCreditDto()
    {
        return new CreditCalculationDto
        {
            TotalCapacity = 999m,
            IsNewBuildId = NewBuildOptions.Yes,
            RenewableSystemDesignId = RenewableSystemDesigns.AnotherPurposeOnly,
            TechnologyTypeId = HeatPumpTechnologyTypes.GasAbsorbtionHeatPump
        };
    }
    #endregion
}