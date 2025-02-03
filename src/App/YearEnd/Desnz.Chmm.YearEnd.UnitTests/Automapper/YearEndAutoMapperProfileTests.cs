using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.YearEnd.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.YearEnd.UnitTests.Automapper
{
    public class YearEndAutoMapperProfileTests
    {
        [Fact]
        public void AutoMapper_CanValidateConfiguration()
        {
            var sut = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<YearEndAutoMapperProfile>();
            }).CreateMapper();
            sut.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
