using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Identity.Api.Infrastructure;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Api.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Desnz.Chmm.Identity.Api.Exceptions;
using Desnz.Chmm.Common.Extensions;
using FluentValidation;
using Desnz.Chmm.Common.Constants;
using Microsoft.OpenApi.Models;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Infrastructure.Repositories;

namespace Desnz.Chmm.Identity.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddControllersWithProblemDetails(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (ctx, exception) => true;
                options.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;

                options.Map<ChmmIdentityException>(e => e.ToProblemDetails());
                options.Map<UserException>(e => e.ToProblemDetails());
                options.Map<ValidationException>(e => e.ToProblemDetails());
            })
                .AddControllers()
                .AddNewtonsoftJson()
                .AddProblemDetailsConventions();
        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IRolesRepository, RolesRepository>();
            services.AddTransient<IOrganisationsRepository, OrganisationsRepository>();
            services.AddTransient<IOrganisationDecisionCommentsRepository, OrganisationDecisionCommentsRepository>();
            services.AddTransient<IOrganisationDecisionFilesRepository, OrganisationDecisionFilesRepository>();
            services.AddTransient<IOrganisationStructureFilesRepository, OrganisationStructureFilesRepository>();
            services.AddTransient<ILicenceHolderRepository, LicenceHolderRepository>();
            services.AddTransient<ILicenceHolderLinkRepository, LicenceHolderLinkRepository>();
            services.AddTransient<IIdentityNotificationService, IdentityNotificationService>();

            services.AddTransient<IAuditItemRepository, AuditItemRepository>();
        }

        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var oneLoginAuthConfig = configuration.GetSection(ConfigurationValueConstants.OneLoginAuth).Get<OneLoginAuthConfig>()
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.OneLoginAuth));
            services.AddHttpClient<IOneLoginService, OneLoginService>(config => { config.BaseAddress = new Uri(oneLoginAuthConfig.Authority); });

            var configurationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ConfigurationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ConfigurationApiUrl));
            services.AddHttpClient<ISchemeYearService, SchemeYearService>(config => { config.BaseAddress = new Uri(configurationApiUrl); });

            var creditLedgerApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.CreditLedgerApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.CreditLedgerApiUrl));
            services.AddHttpClient<ICreditLedgerService, CreditLedgerService>(config => { config.BaseAddress = new Uri(creditLedgerApiUrl); });
        }

        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext>(options =>
            {
                string? connectionString = null;
#if DEBUG
                connectionString = configuration.GetConnectionString("IdentityDb");
#else
                connectionString = configuration.GetValue<string>("ConnectionString");
#endif
                options.UseNpgsql(connectionString 
                    ?? throw new InvalidOperationException($"Configuration missing value for IdentityDb connection string"), 
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
