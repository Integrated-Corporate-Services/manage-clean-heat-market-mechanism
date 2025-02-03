using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Desnz.Chmm.Common.Extensions;
using FluentValidation;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.YearEnd.Api.Infrastructure;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Common.Authorization.Handlers;
using Desnz.Chmm.Common.Authorization.Requirements;

namespace Desnz.Chmm.YearEnd.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var apiKeyPolicy = configuration.GetSection(ConfigurationValueConstants.ApiKeyPolicy).Get<ApiKeyPolicyConfig>()
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ApiKeyPolicy));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConstants.RequireApiKeyPolicy, policy =>
                    policy.Requirements.Add(new ApiKeyRequirement(apiKeyPolicy.HeaderName, apiKeyPolicy.ApiKey)));
            });
            services.AddTransient<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
        }

        public static void AddControllersWithProblemDetails(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (ctx, exception) => true;
                options.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;

                options.Map<ValidationException>(e => e.ToProblemDetails());
            })
                .AddControllers()
                .AddNewtonsoftJson()
                .AddProblemDetailsConventions();
        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IAuditItemRepository, AuditItemRepository>();
        }

        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var oneLoginAuthConfig = configuration.GetSection(ConfigurationValueConstants.OneLoginAuth).Get<OneLoginAuthConfig>()
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.OneLoginAuth));
            services.AddHttpClient<IOneLoginService, OneLoginService>(config => { config.BaseAddress = new Uri(oneLoginAuthConfig.Authority); });

            var creditLedgerApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.CreditLedgerApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.CreditLedgerApiUrl));
            services.AddHttpClient<ICreditLedgerService, CreditLedgerService>(config => { config.BaseAddress = new Uri(creditLedgerApiUrl); });

            var identityApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.IdentityApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.IdentityApiUrl));
            services.AddHttpClient<IOrganisationService, OrganisationService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
            services.AddHttpClient<ILicenceHolderService, LicenceHolderService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
            services.AddHttpClient<IIdentityService, IdentityService>(config => { config.BaseAddress = new Uri(identityApiUrl); });

            var obligationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ObligationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ObligationApiUrl));
            services.AddHttpClient<IObligationService, ObligationService>(config => { config.BaseAddress = new Uri(obligationApiUrl); });

            var yearEndApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.YearEndApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.YearEndApiUrl));
            services.AddHttpClient<IYearEndService, YearEndService>(config => { config.BaseAddress = new Uri(yearEndApiUrl); });

            var configurationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ConfigurationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ConfigurationApiUrl));
            services.AddHttpClient<ISchemeYearService, SchemeYearService>(config => { config.BaseAddress = new Uri(configurationApiUrl); });

        }

        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<YearEndContext>(options =>
            {
                string? connectionString = null;
#if DEBUG
                connectionString = configuration.GetConnectionString("YearEndDb");
#else
                connectionString = configuration.GetValue<string>("ConnectionString");
#endif
                options.UseNpgsql(connectionString
                    ?? throw new InvalidOperationException($"Configuration missing value for YearEndDb connection string"),
                    ConfigureSqlOptions);
            });
            services.AddDbContext<AuditingContext>(options =>
            {
                string? connectionString = null;
#if DEBUG
                connectionString = configuration.GetConnectionString("AuditingDb");
#else
                connectionString = configuration.GetValue<string>("AuditingConnectionString");
#endif
                options.UseNpgsql(connectionString
                    ?? throw new InvalidOperationException($"Configuration missing value for AuditingDb connection string"),
                    ConfigureSqlOptions);

            });
        }

        private static void ConfigureSqlOptions(NpgsqlDbContextOptionsBuilder sqlOptions)
        {
            sqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName);

            // Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
        }
    }
}
