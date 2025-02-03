using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Common.Infrastructure;

public class AuditingContextSeed
{
    public async Task SeedAsync(AuditingContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
