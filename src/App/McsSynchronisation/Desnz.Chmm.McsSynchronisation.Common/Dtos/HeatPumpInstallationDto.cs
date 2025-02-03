using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class HeatPumpInstallationDto : McsInstallationDto
    {
        /// <summary>
        /// Temporary holds the calculated credit
        /// </summary>
        [JsonProperty("Credit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Credit")]
        public decimal Credit { get; set; }

    }

    public class CalculatedInstallationCreditDto : CreditCalculationDto
    {
        /// <summary>
        /// Temporary holds the calculated credit
        /// </summary>
        [JsonProperty("Credit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Credit")]
        public decimal Credit { get; set; }

        /// <summary>
        /// Scheme year boiler sales are reported for
        /// </summary>
        public Guid? SchemeYearId { get; set; }
    }
}

