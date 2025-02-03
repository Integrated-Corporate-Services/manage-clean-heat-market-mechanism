using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.YearEnd.Api.Infrastructure;

public class YearEndContextSeed
{
    public async Task SeedAsync(YearEndContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
            await SeedData(context);
        }
    }

    private async Task SeedData(YearEndContext context)
    {
    }
}
