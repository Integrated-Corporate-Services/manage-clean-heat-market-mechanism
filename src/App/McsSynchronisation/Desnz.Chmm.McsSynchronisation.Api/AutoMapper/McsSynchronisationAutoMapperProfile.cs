using AutoMapper;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.McsSynchronisation.Api.AutoMapper
{
    public class McsSynchronisationAutoMapperProfile : Profile
    {
        public McsSynchronisationAutoMapperProfile()
        {
            CreateMap<HeatPumpProduct, McsProductDto>();
            CreateMap<HeatPumpInstallation, McsInstallationDto>();

            CreateMap<McsProductDto, HeatPumpProduct>()
                .ForMember(e => e.HeatPumpInstallations, c => c.Ignore());

            CreateMap<McsInstallationDto, HeatPumpInstallation>()
                .ForMember(e => e.Id, c => c.Ignore())
                .ForMember(e => e.InstallationRequestId, c => c.Ignore())
                .ForMember(e => e.InstallationRequest, c => c.Ignore())
                .ForMember(e => e.HeatPumpProducts, c => c.Ignore())
                .ForMember(e => e.Credits, c => c.Ignore());

            CreateMap<HeatPumpInstallation, HeatPumpInstallationDto>()
                .ForMember(e => e.Credit, c => c.Ignore());
            CreateMap<CreditCalculationDto, CalculatedInstallationCreditDto>()
                .ForMember(e => e.Credit, c => c.Ignore())
                .ForMember(e => e.SchemeYearId, c => c.Ignore());


            CreateMap<HeatPumpInstallation, CreditCalculationDto>();

            CreateMap<McsProductDto, CreateLicenceHolderCommand>()
                .ForMember(e => e.McsManufacturerId, c => c.MapFrom(o => o.ManufacturerId))
                .ForMember(e => e.McsManufacturerName, c => c.MapFrom(o => o.ManufacturerName));
        }
    }
}
