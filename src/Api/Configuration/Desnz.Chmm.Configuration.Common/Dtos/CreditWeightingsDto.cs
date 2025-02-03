using Desnz.Chmm.Configuration.Common.ValueObjects;

namespace Desnz.Chmm.Configuration.Common.Dtos
{
    public class CreditWeightingsDto
    {
        public Guid Id { get; set; }
        public Guid? SchemeYearId { get; set; }
        public int TotalCapacity { get; set; }
        public List<HeatPumpTechnologyTypeWeightingDto>? HeatPumpTechnologyTypeWeightings { get; set; }
        public List<AlternativeSystemFuelTypeWeightingDto>? AlternativeSystemFuelTypeWeightings { get; set; }

        public CreditWeightingDictionary ToWeightingDictionary()
        {
            return new CreditWeightingDictionary(this);
        }
    }
}
