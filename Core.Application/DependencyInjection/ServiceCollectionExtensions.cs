using Microsoft.Extensions.DependencyInjection;

using Core.Application.Samples.Mappers;
using Core.Application.Samples.Services;

namespace Core.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ProductMapper>();

        services.AddScoped<ProductService>();

        return services;
    }
}