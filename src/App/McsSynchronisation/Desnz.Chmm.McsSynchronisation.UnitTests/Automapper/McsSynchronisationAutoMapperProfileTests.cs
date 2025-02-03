using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.McsSynchronisation.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Automapper
{
    public class McsSynchronisationAutoMapperProfileTests
    {
        [Fact]
        public void AutoMapper_CanValidateConfiguration()
        {
            var sut = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<McsSynchronisationAutoMapperProfile>();
            }).CreateMapper();
            sut.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
