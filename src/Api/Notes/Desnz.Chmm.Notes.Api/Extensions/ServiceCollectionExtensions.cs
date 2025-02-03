using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Desnz.Chmm.Common.Extensions;
using FluentValidation;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Notes.Api.Infrastructure;
using Desnz.Chmm.Common.Infrastructure;
using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;

namespace Desnz.Chmm.Notes.Api.Extensions
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
            services.AddTransient<IAuditItemRepository, AuditItemRepository>();
            services.AddTransient<IManufacturerNotesRepository, ManufacturerNotesRepository>();
            services.AddTransient<IRequestValidator, RequestValidator>();
            services.AddTransient<IValidationMessenger, ValidationMessenger>();
        }

        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            var identityApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.IdentityApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.IdentityApiUrl));
            services.AddHttpClient<IOrganisationService, OrganisationService>(config => { config.BaseAddress = new Uri(identityApiUrl); });

            var configurationApiUrl = configuration.GetValue<string>(ConfigurationValueConstants.ConfigurationApiUrl)
                ?? throw new InvalidOperationException(ConfigurationValueConstants.GetErrorMessage(ConfigurationValueConstants.ConfigurationApiUrl));
            services.AddHttpClient<ISchemeYearService, SchemeYearService>(config => { config.BaseAddress = new Uri(configurationApiUrl); });
        }

        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotesContext>(options =>
            {
                string? connectionString = null;
#if DEBUG
                connectionString = configuration.GetConnectionString("NotesDb");
#else
                connectionString = configuration.GetValue<string>("ConnectionString");
#endif
                options.UseNpgsql(connectionString
                    ?? throw new InvalidOperationException($"Configuration missing value for NotesDb connection string"),
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
