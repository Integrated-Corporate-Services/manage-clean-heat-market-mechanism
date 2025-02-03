using Desnz.Chmm.Notes.Api.Infrastructure;

namespace Desnz.Chmm.Notes.Api;

/// <summary>
/// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
/// </summary>
public class LocalEntryPoint
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args);
        host.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddJsonFile("appsettings.Development.json");
        });
        var builder = host.Build();
        using (var scope = builder.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<NotesContext>();
            await new NotesContextSeed().SeedAsync(context);
        }
        builder.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}