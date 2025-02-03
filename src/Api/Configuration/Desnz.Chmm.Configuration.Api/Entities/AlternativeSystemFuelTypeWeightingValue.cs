using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Configuration.Api.Constants;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    public class AlternativeSystemFuelTypeWeightingValue : Entity
    {
        /// <summary>
        /// Weighting value for the given fuel type
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Type of reference <see cref="ConfigurationConstants.AlternativeSystemFuelTypeWeightingValueTypes"/>
        /// </summary>
        public string Type { get; set; }
        public IReadOnlyCollection<AlternativeSystemFuelTypeWeighting> AlternativeSystemFuelTypeWeightings => _alternativeSystemFuelTypeWeightings;

        private List<AlternativeSystemFuelTypeWeighting> _alternativeSystemFuelTypeWeightings;

        public AlternativeSystemFuelTypeWeightingValue(decimal value, string type)
        {
            Value = value;
            Type = type;
        }
    }
}
