using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class McsInstallationDto : CreditCalculationDto
    {

        [JsonProperty("IsAlternativeHeatingSystemPresent", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("IsAlternativeHeatingSystemPresent")]
        public bool? IsAlternativeHeatingSystemPresent { get; set; }

        /// <summary>
        /// When was the other heating system installed?
        /// </summary>
        [JsonProperty("AlternativeHeatingAgeID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("AlternativeHeatingAgeID")]
        public int? AlternativeHeatingAgeId { get; set; }

        [JsonProperty("MPAN", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("MPAN")]
        public string? Mpan { get; set; }

        /// <summary>
        /// Certificates count for this installation
        /// </summary>
        [JsonProperty("HowManyCertificates", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("HowManyCertificates")]
        public int? CertificatesCount { get; set; }
    }
}

