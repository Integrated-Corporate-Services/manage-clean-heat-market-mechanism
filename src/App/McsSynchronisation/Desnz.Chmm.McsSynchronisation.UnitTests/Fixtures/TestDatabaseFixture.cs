using Desnz.Chmm.McsSynchronisation.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Fixtures;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=localhost;Port=5432;Database=chmm-mcssynchronisation;User Id=user;Password=password;Include Error Detail=true;";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                _ = CreateContext(clearData: true);
                _databaseInitialized = true;
            }
        }
    }

    public McsSynchronisationContext CreateContext(bool clearData = false)
    {
        var context = new McsSynchronisationContext(new DbContextOptionsBuilder<McsSynchronisationContext>()
                        .UseNpgsql(ConnectionString)
                        .Options);

        if (clearData)
        {
            using (context)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        return context;
    }
}
