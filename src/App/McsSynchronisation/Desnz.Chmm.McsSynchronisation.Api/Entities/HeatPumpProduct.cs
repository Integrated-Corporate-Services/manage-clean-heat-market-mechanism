using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Newtonsoft.Json;

namespace Desnz.Chmm.McsSynchronisation.Api.Entities
{
    public class HeatPumpProduct : McsProductDto
    {
        [JsonIgnore]
        public List<HeatPumpInstallation> HeatPumpInstallations { get; set; }
    }
}
