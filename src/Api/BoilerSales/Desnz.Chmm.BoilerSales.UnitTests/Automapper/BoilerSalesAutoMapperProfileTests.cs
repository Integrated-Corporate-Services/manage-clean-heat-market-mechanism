using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.BoilerSales.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.BoilerSales.UnitTests.Automapper
{
    public class BoilerSalesAutoMapperProfileTests
    {
        [Fact]
        public void AutoMapper_CanValidateConfiguration()
        {
            var sut = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<BoilerSalesAutoMapperProfile>();
            }).CreateMapper();
            sut.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
