using Microsoft.Extensions.DependencyInjection;

using Core.Application.Samples.Mappers;
using Core.Application.Samples.Services;
using Core.Application.Samples.Publishers;
using Core.Application.Messaging.Interfaces;

namespace Core.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // =========================
        // MAPPERS
        // =========================
        services.AddScoped<ProductMapper>();

        // =========================
        // SERVICES
        // =========================
        services.AddScoped<ProductService>();

        // =========================
        // PUBLISHERS
        // =========================
        services.AddScoped<ProductPublisher>();

        return services;
    }
}