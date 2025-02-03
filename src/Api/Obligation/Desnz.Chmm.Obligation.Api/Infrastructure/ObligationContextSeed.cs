using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Obligation.Api.Infrastructure;

public class ObligationContextSeed
{
    public async Task SeedAsync(ObligationContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
