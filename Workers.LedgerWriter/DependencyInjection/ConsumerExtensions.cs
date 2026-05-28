namespace Workers.LedgerWriter.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Workers.LedgerWriter.Consumers.Samples;

public static class ConsumerExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddScoped<ProductCreatedConsumer>();

        return services;
    }
}