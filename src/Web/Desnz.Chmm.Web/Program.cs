namespace Desnz.Chmm.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder);
        startup.Run();
    }
}