using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.Configuration.Common.ValueObjects
{
    public class CreditWeightingDictionary
    {
        public CreditWeightingDictionary(CreditWeightingsDto creditWeighting)
        {
            TechnologyTypesWeightings = creditWeighting.HeatPumpTechnologyTypeWeightings.ToDictionary(x => x.Code, x => x.Value);
            AlternativeSystemFuelTypesWeightings = creditWeighting.AlternativeSystemFuelTypeWeightings.ToDictionary(x => x.Code, x => x.AlternativeSystemFuelTypeWeightingValue.Value);
            TotalCapacity = creditWeighting.TotalCapacity;
            SchemeYearId = creditWeighting.SchemeYearId;
        }
        public decimal TotalCapacity { get; private set; }
        public Dictionary<string, decimal> TechnologyTypesWeightings { get; private set; }
        public Dictionary<string, decimal> AlternativeSystemFuelTypesWeightings { get; private set; }
        public Guid? SchemeYearId { get; private set; }
    }
}
