using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.CreditLedger.Api.Infrastructure;

public class CreditLedgerSeed
{
    public async Task SeedAsync(CreditLedgerContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
