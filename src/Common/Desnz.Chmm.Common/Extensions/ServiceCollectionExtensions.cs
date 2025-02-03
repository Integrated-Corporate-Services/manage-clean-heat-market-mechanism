using Desnz.Chmm.Common.Authorization.Constants;
using Desnz.Chmm.Common.Authorization.Handlers;
using Desnz.Chmm.Common.Authorization.Requirements;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Mediator.Behaviours;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Desnz.Chmm.Common.Constants.ConfigurationValueConstants;

namespace Desnz.Chmm.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtAuthConfig = configuration.GetSection(ChmmJwtAuth).Get<ChmmJwtAuthConfig>()
                ?? throw new InvalidOperationException(GetErrorMessage(ChmmJwtAuth));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtAuthConfig.Issuer,
                    ValidAudience = jwtAuthConfig.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthConfig.SecurityKey))
                };
            });
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConstants.CanAccessOrganisation, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.Requirements.Add(new OrganisationRequirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, OrganisationAuthorizationHandler>();
        }

        public static void AddMediator<T>(this IServiceCollection services) where T : class
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<T>();

                cfg.AddOpenBehavior(typeof(DateTimeOverrideBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(AuditBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining<T>();

            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        }

        public static void AddNotification(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationService, NotificationService>();
        }

        public static void AddFileService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IClamAvService, ClamAvService>();
            services.AddTransient<IFileService, FileService>();
        }

        public static void AddGlobalServices(this IServiceCollection services, IConfiguration configuration, string environmentName)
        {
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();

            if (environmentName == "dev" || environmentName == "test" || environmentName == "staging")
            {
                services.AddScoped<IDateTimeProvider, DateTimeOverrideProvider>();
            }
            else
            {
                services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            }
        }
    }
}
