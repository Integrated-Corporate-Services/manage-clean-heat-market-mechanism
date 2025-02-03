using Desnz.Chmm.Common;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.YearEnd.Api.Infrastructure;

public class YearEndContext : DbContext, IUnitOfWork
{


    public YearEndContext(DbContextOptions<YearEndContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
    }
}
