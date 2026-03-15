using Microsoft.OpenApi.Models;
namespace ResumeBuilder.API.Extensions;
public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Resume Builder API", Version = "v1", Description = "Resume Builder API - Onion Architecture + ASP.NET Core 8 + SQL Server" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Description = "JWT Authorization. Enter: Bearer {token}", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }});
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var xml in xmlFiles) c.IncludeXmlComments(xml);
        });
        return services;
    }
}
