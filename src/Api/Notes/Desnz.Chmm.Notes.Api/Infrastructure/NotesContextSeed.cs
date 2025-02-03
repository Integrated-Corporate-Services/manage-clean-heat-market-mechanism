using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Notes.Api.Infrastructure;

public class NotesContextSeed
{
    public async Task SeedAsync(NotesContext context)
    {
        using (context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
