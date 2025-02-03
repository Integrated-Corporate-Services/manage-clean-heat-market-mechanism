using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure;

public class BoilerSalesSeed
{
    public async Task SeedAsync(BoilerSalesContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
