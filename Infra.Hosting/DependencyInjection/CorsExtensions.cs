using Infra.Hosting.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Hosting.DependencyInjection;

public static class CorsExtensions
{
    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cors = configuration
            .GetSection("Cors")
            .Get<CorsOptions>();

        services.AddCors(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                if (cors?.AllowedOrigins?.Count > 0)
                {
                    policy
                        .WithOrigins(cors.AllowedOrigins.ToArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });

        return services;
    }
}