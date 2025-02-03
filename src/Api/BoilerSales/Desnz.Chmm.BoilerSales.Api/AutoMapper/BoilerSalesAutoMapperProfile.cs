using AutoMapper;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Common.Dtos.Annual;
using Desnz.Chmm.BoilerSales.Common.Dtos.Quarterly;

namespace Desnz.Chmm.BoilerSales.Api.AutoMapper;

public class BoilerSalesAutoMapperProfile : Profile
{
    public BoilerSalesAutoMapperProfile()
    {
        CreateMap<AnnualBoilerSalesChange, AnnualBoilerSalesChangeDto>();
        CreateMap<AnnualBoilerSalesFile, AnnualBoilerSalesFileDto>();
        CreateMap<AnnualBoilerSales, AnnualBoilerSalesDto>();

        CreateMap<QuarterlyBoilerSalesChange, QuarterlyBoilerSalesChangeDto>();
        CreateMap<QuarterlyBoilerSalesFile, QuarterlyBoilerSalesFileDto>();
        CreateMap<QuarterlyBoilerSales, QuarterlyBoilerSalesDto>();
    }
}
