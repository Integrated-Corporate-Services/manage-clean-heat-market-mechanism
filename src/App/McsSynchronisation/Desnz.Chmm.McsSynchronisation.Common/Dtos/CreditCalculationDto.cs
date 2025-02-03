using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class CreditCalculationDto
    {
        [JsonProperty("AirTypeTechnologyID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("AirTypeTechnologyID")]
        public int? AirTypeTechnologyId { get; set; }

        [JsonProperty("AlternativeHeatingFuelID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("AlternativeHeatingFuelID")]
        public int? AlternativeHeatingFuelId { get; set; }

        /// <summary>
        /// What is the additional source/system/technology?
        /// </summary>
        [JsonProperty("AlternativeHeatingSystemID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("AlternativeHeatingSystemID")]
        public int? AlternativeHeatingSystemId { get; set; }

        /// <summary>
        /// The installation commissioning date
        /// </summary>
        [JsonProperty("CommissioningDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("CommissioningDate")]
        public DateTime? CommissioningDate { get; set; }

        [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ID")]
        public int? MidId { get; set; }

        /// <summary>
        /// Is this a new build property?
        /// </summary>
        [JsonProperty("IsNewBuildID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("IsNewBuildID")]
        public int? IsNewBuildId { get; set; }

        /// <summary>
        /// Returns True if the heat pump was installed with another fossil-fuel-based one
        /// </summary>
        [JsonProperty("IsHybrid", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("IsHybrid")]
        public bool? IsHybrid { get; set; }

        /// <summary>
        /// Renewable system design Id
        /// </summary>
        [JsonProperty("RenewableSystemDesignID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("RenewableSystemDesignID")]
        public int? RenewableSystemDesignId { get; set; }

        [JsonProperty("TechnologyTypeID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("TechnologyTypeID")]
        public int? TechnologyTypeId { get; set; }

        /// <summary>
        /// Total capacity in kWh
        /// </summary>
        [JsonProperty("TotalCapacity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("TotalCapacity")]
        public decimal? TotalCapacity { get; set; }

        [JsonProperty("Products", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Products")]
        public List<McsProductDto> HeatPumpProducts { get; set; }

        public CreditCalculationDto()
        {
            HeatPumpProducts = new List<McsProductDto>();
        }

        /// <summary>
        /// Extends the use of AlternativeHeatingSystemID. 
        /// When False, assume AlternativeHeatingSystem values.
        /// When True, assume TechnologyType values
        /// </summary>
        [JsonProperty("IsSystemSelectedAsMCSTechnology", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("IsSystemSelectedAsMCSTechnology")]
        public bool? IsSystemSelectedAsMCSTechnology { get; set; }
    }
}