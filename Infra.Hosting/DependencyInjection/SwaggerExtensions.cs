using Microsoft.Extensions.DependencyInjection;

namespace Infra.Hosting.DependencyInjection;

public static class SwaggerExtensions
{
    public static IServiceCollection AddApiSwagger(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        return services;
    }
}