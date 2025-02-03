using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Notes.Api.AutoMapper;
using Xunit;

namespace Desnz.Chmm.Notes.UnitTests.AutoMapper;

public class NotesAutoMapperProfileTests
{
    [Fact]
    public void AutoMapper_CanValidateConfiguration()
    {
        var sut = new MapperConfiguration(cfg =>
        {
            cfg.Internal().MethodMappingEnabled = false;
            cfg.AddProfile<NotesAutoMapperProfile>();
        }).CreateMapper();

        sut.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
