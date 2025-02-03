using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Desnz.Chmm.McsSynchronisation.Common.Dtos;

public class MscReferenceDataDto
{
    [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("Description")]
    public string Description { get; set; }
}

public class AirTypeTechnologyDto : MscReferenceDataDto { }
public class AlternativeSystemFuelTypeDto : MscReferenceDataDto { }
public class AlternativeSystemTypeDto : MscReferenceDataDto { }
public class InstallationAgeDto : MscReferenceDataDto { }
public class ManufacturerDto : MscReferenceDataDto { }
public class NewBuildOptionDto : MscReferenceDataDto { }
public class RenewableSystemDesignDto : MscReferenceDataDto { }
public class TechnologyTypeDto : MscReferenceDataDto { }

public class MscReferenceData
{
    public List<AirTypeTechnologyDto> AirTypeTechnologies { get; set; }
    public List<AlternativeSystemFuelTypeDto> AlternativeSystemFuelTypes { get; set; }
    public List<AlternativeSystemTypeDto> AlternativeSystemTypes { get; set; }
    public List<InstallationAgeDto> InstallationAges { get; set; }
    public List<ManufacturerDto> Manufacturers { get; set; }
    public List<NewBuildOptionDto> NewBuildOptions { get; set; }
    public List<RenewableSystemDesignDto> RenewableSystemDesigns { get; set; }
    public List<TechnologyTypeDto> TechnologyTypes { get; set; }
    public List<McsProductDto> HeatPumpProducts { get; set; }
}
