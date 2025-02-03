using AutoMapper;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Common.Dtos;

namespace Desnz.Chmm.Configuration.Api.AutoMapper;

/// <summary>
/// Automapper config for the Configuration service
/// </summary>
public class ConfigurationAutoMapperProfile : Profile
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public ConfigurationAutoMapperProfile()
    {
        CreateMap<SchemeYear, SchemeYearDto>();
        CreateMap<SchemeYearQuarter, SchemeYearQuarterDto>();
        CreateMap<CreditWeighting, CreditWeightingsDto>();
        CreateMap<HeatPumpTechnologyTypeWeighting, HeatPumpTechnologyTypeWeightingDto>();
        CreateMap<AlternativeSystemFuelTypeWeighting, AlternativeSystemFuelTypeWeightingDto>();
        CreateMap<AlternativeSystemFuelTypeWeightingValue, AlternativeSystemFuelTypeWeightingValueDto>();
    }
}
