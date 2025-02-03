using Desnz.Chmm.Common.Authorization.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Desnz.Chmm.Common.Swagger
{
    public class ApiKeyHeaderFilter : IOperationFilter
    {
        public ApiKeyHeaderFilter() { }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = SwaggerConstants.ApiKeyHeader,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                }
            });
        }
    }
}
