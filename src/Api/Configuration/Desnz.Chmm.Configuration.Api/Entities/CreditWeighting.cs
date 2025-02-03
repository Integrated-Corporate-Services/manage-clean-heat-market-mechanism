using Desnz.Chmm.Common.Entities;

namespace Desnz.Chmm.Configuration.Api.Entities
{
    /// <summary>
    /// Defines credit weightings for a scheme year
    /// </summary>
    public class CreditWeighting : Entity
    {
        /// <summary>
        /// Max capacity allowed for a combined install
        /// </summary>
        public int TotalCapacity { get; private set; }

        /// <summary>
        /// Id of the scheme year this quarter is assocated with
        /// </summary>
        public Guid SchemeYearId { get; private set; }

        /// <summary>
        /// The scheme year this quarter is associated with
        /// </summary>
        public SchemeYear SchemeYear { get; private set; }

        /// <summary>
        /// Heat pump technology weightings
        /// </summary>
        public IReadOnlyCollection<HeatPumpTechnologyTypeWeighting> HeatPumpTechnologyTypeWeightings => _heatPumpTechnologyTypeWeightings;

        /// <summary>
        /// Alternative system fuel type weightings
        /// </summary>
        public IReadOnlyCollection<AlternativeSystemFuelTypeWeighting> AlternativeSystemFuelTypeWeightings => _alternativeSystemFuelTypeWeightings;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected CreditWeighting() : base()
        { }
        public CreditWeighting(Guid schemeYearId, int totalCapacity, List<HeatPumpTechnologyTypeWeighting> heatPumpTechnologyTypeWeightings, List<AlternativeSystemFuelTypeWeighting> alternativeSystemFuelTypeWeightings)
        {
            SchemeYearId = schemeYearId;
            TotalCapacity = totalCapacity;
            _heatPumpTechnologyTypeWeightings = heatPumpTechnologyTypeWeightings;
            _alternativeSystemFuelTypeWeightings = alternativeSystemFuelTypeWeightings;
        }

        public CreditWeighting(int totalCapacity, List<HeatPumpTechnologyTypeWeighting> heatPumpTechnologyTypeWeightings, List<AlternativeSystemFuelTypeWeighting> alternativeSystemFuelTypeWeightings)
        {
            TotalCapacity = totalCapacity;
            _heatPumpTechnologyTypeWeightings = heatPumpTechnologyTypeWeightings;
            _alternativeSystemFuelTypeWeightings = alternativeSystemFuelTypeWeightings;
        }

        private List<HeatPumpTechnologyTypeWeighting> _heatPumpTechnologyTypeWeightings;
        private List<AlternativeSystemFuelTypeWeighting> _alternativeSystemFuelTypeWeightings;
    }
}
