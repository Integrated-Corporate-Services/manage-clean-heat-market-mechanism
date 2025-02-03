using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Web.Proxy;

namespace Desnz.Chmm.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var identityApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.IdentityApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.IdentityApiUrl));
            services.AddHttpClient<IIdentityService, IdentityService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
            services.AddHttpClient<IUserService, UserService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
            services.AddHttpClient<IRoleService, RoleService>(config => { config.BaseAddress = new Uri(identityApiUrl); });

            var boilerSalesApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.BoilerSalesApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.BoilerSalesApiUrl));
            services.AddHttpClient<IBoilerSalesService, BoilerSalesService>(config => { config.BaseAddress = new Uri(boilerSalesApiUrl); });
        }

        public static void AddProxyServices(this IServiceCollection services, IConfiguration configuration)
        {
            var proxyConfigSection = configuration.GetSection(ConfigurationValueConstants.Proxy);
            services.Configure<ProxyConfig>(proxyConfigSection);
            services.AddTransient<IProxyService, ProxyService>();
            services.AddHttpClient<IProxyHttpServiceClient, ProxyHttpServiceClient>(config => { });
        }
    }
}