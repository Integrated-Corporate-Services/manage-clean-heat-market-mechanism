using AutoMapper;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;

namespace Desnz.Chmm.CreditLedger.Api.AutoMapper;

public class CreditLedgerAutoMapperProfile : Profile
{
    public CreditLedgerAutoMapperProfile()
    {
        CreateMap<McsInstallationDto, CalculatedInstallationCreditDto>()
            .ForMember(e => e.Credit, c => c.Ignore())
            .ForMember(e => e.SchemeYearId, c => c.Ignore());

        CreateMap<HeatPumpInstallationDto, CalculatedInstallationCreditDto>()
            .ForMember(e => e.Credit, c => c.Ignore())
            .ForMember(e => e.SchemeYearId, c => c.Ignore());

        CreateMap<CalculatedInstallationCreditDto, HeatPumpInstallationDto>()
            .ForMember(e => e.IsAlternativeHeatingSystemPresent, c => c.Ignore())
            .ForMember(e => e.AlternativeHeatingAgeId, c => c.Ignore())
            .ForMember(e => e.Mpan, c => c.Ignore())
            .ForMember(e => e.CertificatesCount, c => c.Ignore())
            .ForMember(e => e.IsNewBuildId, c => c.Ignore());
    }
}