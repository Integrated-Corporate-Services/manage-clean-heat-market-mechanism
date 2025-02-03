using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Configuration.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.AutoMapper;

public class ConfigurationAutoMapperProfileTests
{
    [Fact]
    public void AutoMapper_CanValidateConfiguration()
    {
        var sut = new MapperConfiguration(cfg =>
        {
            cfg.Internal().MethodMappingEnabled = false;
            cfg.AddProfile<ConfigurationAutoMapperProfile>();
        }).CreateMapper();

        sut.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
