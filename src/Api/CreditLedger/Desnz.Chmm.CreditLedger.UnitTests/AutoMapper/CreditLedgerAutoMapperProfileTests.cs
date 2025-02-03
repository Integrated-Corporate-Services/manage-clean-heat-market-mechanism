using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.CreditLedger.Api.AutoMapper;
using Desnz.Chmm.CreditLedger.Api.Entities;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Xunit;

namespace Desnz.Chmm.CreditLedger.UnitTests.AutoMapper;

public class CreditLedgerAutoMapperProfileTests
{
    private readonly IMapper _mapper;

    public CreditLedgerAutoMapperProfileTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.Internal().MethodMappingEnabled = false;
            cfg.AddProfile<CreditLedgerAutoMapperProfile>();
        }).CreateMapper();
    }

    [Fact]
    public void AutoMapper_CanValidateConfiguration()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
