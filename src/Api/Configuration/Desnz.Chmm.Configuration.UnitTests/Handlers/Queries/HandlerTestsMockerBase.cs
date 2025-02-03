using Desnz.Chmm.Configuration.Api.Constants;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Testing.Common;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class HandlerTestsMockerBase
    {

        public CreditWeighting GenerateCreditWeightings(Guid id)
        {
            var zeroValue = new AlternativeSystemFuelTypeWeightingValue(0M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Other);
            var halfValue = new AlternativeSystemFuelTypeWeightingValue(0.5M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel);
            var wholeValue = new AlternativeSystemFuelTypeWeightingValue(1M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable);

            return new CreditWeighting(id, 70, new List<HeatPumpTechnologyTypeWeighting>()
                {
                    new HeatPumpTechnologyTypeWeighting("AirSourceHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("ExhaustAirHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("GasAbsorbtionHeatPump",0),
                    new HeatPumpTechnologyTypeWeighting("GroundWaterSourceHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("HotWaterHeatPump",0),
                    new HeatPumpTechnologyTypeWeighting("SolarAssistedHeatPump",1)
                },
                new List<AlternativeSystemFuelTypeWeighting>
                {
                    new AlternativeSystemFuelTypeWeighting("Anthracite", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("BiogasLandfillCommunityHeatingOnly", zeroValue ),
                    new AlternativeSystemFuelTypeWeighting("BulkWoodPellets", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("Coal", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("DualFuelMineralAndWood", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Electricity", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("MainsGas", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Oil", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("WasteCombustionCommunityHeatingOnly", zeroValue ),
                    new AlternativeSystemFuelTypeWeighting("WoodChips", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("WoodLogs", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("NotApplicableNoOtherHeatingSource", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("BioliquidHvoBioLpg", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Solar", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("Other", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("LPG", halfValue )
                }
            );
        }
        public CreditWeighting GenerateCreditWeightings()
        {
            var zeroValue = new AlternativeSystemFuelTypeWeightingValue(0M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Other);
            var halfValue = new AlternativeSystemFuelTypeWeightingValue(0.5M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.FossilFuel);
            var wholeValue = new AlternativeSystemFuelTypeWeightingValue(1M, ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes.Renewable);

            return new CreditWeighting(70, new List<HeatPumpTechnologyTypeWeighting>()
                {
                    new HeatPumpTechnologyTypeWeighting("AirSourceHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("ExhaustAirHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("GasAbsorbtionHeatPump",0),
                    new HeatPumpTechnologyTypeWeighting("GroundWaterSourceHeatPump",1),
                    new HeatPumpTechnologyTypeWeighting("HotWaterHeatPump",0),
                    new HeatPumpTechnologyTypeWeighting("SolarAssistedHeatPump",1)
                },
                new List<AlternativeSystemFuelTypeWeighting>
                {
                    new AlternativeSystemFuelTypeWeighting("Anthracite", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("BiogasLandfillCommunityHeatingOnly", zeroValue ),
                    new AlternativeSystemFuelTypeWeighting("BulkWoodPellets", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("Coal", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("DualFuelMineralAndWood", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Electricity", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("MainsGas", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Oil", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("WasteCombustionCommunityHeatingOnly", zeroValue ),
                    new AlternativeSystemFuelTypeWeighting("WoodChips", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("WoodLogs", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("NotApplicableNoOtherHeatingSource", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("BioliquidHvoBioLpg", halfValue ),
                    new AlternativeSystemFuelTypeWeighting("Solar", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("Other", wholeValue ),
                    new AlternativeSystemFuelTypeWeighting("LPG", halfValue )
                }
            );
        }

        public List<SchemeYear> GenerateSchemeYears()
        {
            return new List<SchemeYear>
            {
                GenerateSchemeYear()
            };
        }

        public SchemeYear GenerateSchemeYear()
        {
            var schemeYear = new SchemeYear(
                SchemeYearConstants.Name, 
                SchemeYearConstants.Year, 
                SchemeYearConstants.StartDate, 
                SchemeYearConstants.EndDate,
                SchemeYearConstants.TradingWindowStartDate,
                SchemeYearConstants.TradingWindowEndDate, 
                SchemeYearConstants.CreditGenerationWindowStartDate, 
                SchemeYearConstants.CreditGenerationWindowEndDate, 
                SchemeYearConstants.BoilerSalesSubmissionEndDate, 
                SchemeYearConstants.SurrenderDayDate,
                new List<SchemeYearQuarter>
                {
                    new SchemeYearQuarter(SchemeYearConstants.QuarterOneName, SchemeYearConstants.QuarterOneStartDate, SchemeYearConstants.QuarterOneEndDate),
                    new SchemeYearQuarter(SchemeYearConstants.QuarterTwoName, SchemeYearConstants.QuarterTwoStartDate, SchemeYearConstants.QuarterTwoEndDate),
                    new SchemeYearQuarter(SchemeYearConstants.QuarterThreeName, SchemeYearConstants.QuarterThreeStartDate, SchemeYearConstants.QuarterThreeEndDate),
                    new SchemeYearQuarter(SchemeYearConstants.QuarterFourName, SchemeYearConstants.QuarterFourStartDate, SchemeYearConstants.QuarterFourEndDate)
                },
                new ObligationCalculations(1, 2, 4, 8, 16, 32, 64, 128),
                creditWeightings: GenerateCreditWeightings());

            return schemeYear;
        }

        public SchemeYearQuarter GenerateSchemeYearQuarter()
        {
            return new SchemeYearQuarter(SchemeYearConstants.QuarterOneName, SchemeYearConstants.QuarterOneStartDate, SchemeYearConstants.QuarterOneEndDate);
        }
    }
}