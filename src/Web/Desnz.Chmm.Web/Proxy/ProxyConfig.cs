namespace Desnz.Chmm.Web.Proxy;

public class ProxyConfig
{
    public bool UseCorrelationId { get; set; }
    public ProxyConfigService[] Services { get; set; }
}

public class ProxyConfigService
{
    public string BaseUri { get; set; }
    public string[] Routes { get; set; }
}
