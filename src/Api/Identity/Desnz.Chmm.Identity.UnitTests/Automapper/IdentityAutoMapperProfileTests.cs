using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Identity.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Automapper
{
    public class IdentityAutoMapperProfileTests
    {
        [Fact]
        public void AutoMapper_CanValidateConfiguration()
        {
            var sut = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<IdentityAutoMapperProfile>();
            }).CreateMapper();
            sut.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
