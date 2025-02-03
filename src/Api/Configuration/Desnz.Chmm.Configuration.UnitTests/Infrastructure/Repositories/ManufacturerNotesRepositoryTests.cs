using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Infrastructure;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Infrastructure.Repositories;

public class SchemeYearConfigurationRepositoryTests
{
    private readonly Mock<ILogger<SchemeYearRepository>> logger;
    private readonly Mock<IConfigurationContext> context;

    private readonly List<SchemeYear> Configuration;
    private readonly SchemeYearRepository repository;

    public SchemeYearConfigurationRepositoryTests()
    {
        logger = new Mock<ILogger<SchemeYearRepository>>();
        context = new Mock<IConfigurationContext>();

        var (date, time) = DateTime.Now;
        Configuration = new List<SchemeYear>
        {
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>()),
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>()),
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>()),
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>()),
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>()),
            new SchemeYear("", 1, date, date, date, date,date,date,date,date,new List<SchemeYearQuarter>())
        };

        context.Setup(x => x.SchemeYears)
            .Returns(Configuration.AsQueryable().BuildMockDbSet().Object);

        repository = new SchemeYearRepository(context.Object, logger.Object);
    }

    [Fact]
    public void Query_Without_Condition()
    {
        var response = repository.Query(null).ToList();
        Assert.Equal(6, response.Count);
    }

    [Fact]
    public void Query_With_Condition()
    {
        var response = repository.Query(x => true).ToList();
        Assert.Equal(6, response.Count);
    }

    [Fact]
    public void Query_With_Tracking()
    {
        var response = repository.Query(x => true, withTracking: true).ToList();
        Assert.Equal(6, response.Count);
    }
}
