using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Desnz.Chmm.Common.Extensions;
using FluentValidation;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.CreditLedger.Api.Infrastructure;
using Desnz.Chmm.CreditLedger.Api.Infrastructure.Repositories;
using Desnz.Chmm.CreditLedger.Api.Services;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.CommonValidation;

namespace Desnz.Chmm.CreditLedger.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
            services.AddTransient<IInstallationCreditRepository, InstallationCreditRepository>();
            services.AddTransient<ICreditLedgerCalculator, CreditLedgerCalculator>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IAuditItemRepository, AuditItemRepository>();
            services.AddTransient<IRequestValidator, RequestValidator>();
            services.AddTransient<IValidationMessenger, ValidationMessenger>();
        }

        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var oneLoginAuthConfig = configuration.GetSection(ConfigurationValueConstants.OneLoginAuth).Get<OneLoginAuthConfig>()
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.OneLoginAuth));
            services.AddHttpClient<IOneLoginService, OneLoginService>(config => { config.BaseAddress = new Uri(oneLoginAuthConfig.Authority); });

            var mcsSynchronisationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.McsSynchronisationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.McsSynchronisationApiUrl));
            services.AddHttpClient<IMcsSynchronisationService, McsSynchronisationService>(config => { config.BaseAddress = new Uri(mcsSynchronisationApiUrl); });

            var obligationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ObligationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ObligationApiUrl));
            services.AddHttpClient<IObligationService, ObligationService>(config => { config.BaseAddress = new Uri(obligationApiUrl); });

            var identityApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.IdentityApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.IdentityApiUrl));
            services.AddHttpClient<ILicenceHolderService, LicenceHolderService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
            services.AddHttpClient<IOrganisationService, OrganisationService>(config => { config.BaseAddress = new Uri(identityApiUrl); });

            var configurationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ConfigurationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ConfigurationApiUrl));
            services.AddHttpClient<ISchemeYearService, SchemeYearService>(config => { config.BaseAddress = new Uri(configurationApiUrl); });

            services.AddHttpClient<IUserService, UserService>(config => { config.BaseAddress = new Uri(identityApiUrl); });
        }

        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CreditLedgerContext>(options =>
            {
                string? connectionString = null;
#if DEBUG
                connectionString = configuration.GetConnectionString("CreditLedgerDb");
#else
                connectionString = configuration.GetValue<string>("ConnectionString");
#endif
                options.UseNpgsql(connectionString
                    ?? throw new InvalidOperationException($"Configuration missing value for CreditLedgerDb connection string"),
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
