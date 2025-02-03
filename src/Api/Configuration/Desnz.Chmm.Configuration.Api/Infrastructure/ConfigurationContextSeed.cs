using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Configuration.Api.Infrastructure;

public class ConfigurationContextSeed
{
    public async Task SeedAsync(ConfigurationContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
