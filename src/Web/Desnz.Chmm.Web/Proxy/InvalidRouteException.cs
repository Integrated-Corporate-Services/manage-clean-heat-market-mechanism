namespace Desnz.Chmm.Web.Proxy;

public class InvalidRouteException : Exception
{
    public InvalidRouteException(string route) : base($"Could not find a route defined in proxy config for '{route}'") { }
}
