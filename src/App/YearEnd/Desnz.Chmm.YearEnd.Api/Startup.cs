using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Configuration.Settings;
using Hellang.Middleware.ProblemDetails;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.YearEnd.Api.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Configuration;

namespace Desnz.Chmm.YearEnd.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<OneLoginAuthConfig>(_configuration.GetSection(ConfigurationValueConstants.OneLoginAuth));
        services.Configure<ChmmJwtAuthConfig>(_configuration.GetSection(ConfigurationValueConstants.ChmmJwtAuth));
        services.Configure<GovukNotifyConfig>(_configuration.GetSection(ConfigurationValueConstants.GovukNotify));
        services.Configure<EnvironmentConfig>(_configuration.GetSection(ConfigurationValueConstants.EnvironmentConfig));
        services.Configure<ApiKeyPolicyConfig>(_configuration.GetSection(ConfigurationValueConstants.ApiKeyPolicy));

        services.AddAutoMapper(cfg => cfg.Internal().MethodMappingEnabled = false, typeof(Startup).Assembly);

        services.AddJwtBearerAuthentication(_configuration);
        services.AddAuthorizationPolicies(_configuration);

        services.AddDbContexts(_configuration);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CHMM Year End", Version = "v1" });
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { In = ParameterLocation.Header, Description = "Please enter JWT with Bearer into field in the format 'Bearer [JWT token]'", Name = "Authorization", Type = SecuritySchemeType.ApiKey });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Id = "Bearer"
                            , Type = ReferenceType.SecurityScheme
                        },
                        Scheme = "bearer",
                        Name = "security",
                        In = ParameterLocation.Header
                    }, new List<string>()
                }
            });
            c.EnableAnnotations();
        });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpClientServices(_configuration);

        services.AddMediator<Startup>();
        services.AddNotification(_configuration);

        var environmentConfig = _configuration.GetSection(ConfigurationValueConstants.EnvironmentConfig);
        var env = environmentConfig.GetValue<string>("EnvironmentName");

        services.AddGlobalServices(_configuration, env);

        services.AddApplicationServices();

        services.AddControllersWithProblemDetails(_environment);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseProblemDetails();
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/yearend/swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "CHMM Year End V1");
            c.RoutePrefix = "api/yearend/swagger";
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        string currentNamespace = typeof(Startup)?.Namespace ?? "";
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync($"Welcome to running {currentNamespace} ASP.NET Core on AWS Lambda");
            });
        });
    }
}
