using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Desnz.Chmm.McsSynchronisation.Common.Dtos
{
    public class McsProductDto
    {
        [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ID")]
        public int Id { get; set; }

        [JsonProperty("Code", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonProperty("ManufacturerID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ManufacturerID")]
        public int ManufacturerId { get; set; }

        [JsonProperty("Manufacturer", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Manufacturer")]
        public string ManufacturerName { get; set; }

        //[JsonProperty("FlowTemp", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonPropertyName("FlowTemp")]
        //public double FlowTemp { get; set; }

        //[JsonProperty("SCOP", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonPropertyName("SCOP")]
        //public double Scop { get; set; }


        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as McsProductDto);
        }

        public bool Equals(McsProductDto other)
        {
            return other is not null &&
                   Id == other.Id &&
                   Code == other.Code;
        }

        public override string? ToString()
        {
            return $"{Name}-{ManufacturerName}";
        }
    }
}
