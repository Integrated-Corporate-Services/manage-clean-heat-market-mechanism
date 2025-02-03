using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Configuration.Settings;
using Hellang.Middleware.ProblemDetails;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.SystemAudit.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Desnz.Chmm.SystemAudit.Api;

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

        services.AddJwtBearerAuthentication(_configuration);
        services.AddAuthorizationPolicies();

        services.AddDbContexts(_configuration);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CHMM SystemAudit", Version = "v1" });
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
            c.RouteTemplate = "api/SystemAudit/swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "CHMM SystemAudit API V1");
            c.RoutePrefix = "api/SystemAudit/swagger";
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
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