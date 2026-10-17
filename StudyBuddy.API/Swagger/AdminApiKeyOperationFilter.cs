using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using StudyBuddy.API.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StudyBuddy.API.Swagger
{
    public sealed class AdminApiKeyOperationFilter : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
            var controllerName =
                (context.ApiDescription.ActionDescriptor as ControllerActionDescriptor)?
                    .ControllerName;

            if (!string.Equals(
                    controllerName,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        AdminApiKeyAuthenticationHandler.SchemeName,
                        context.Document)] = new List<string>()
                },
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        context.Document)] = new List<string>()
                }
            };
        }
    }
}
