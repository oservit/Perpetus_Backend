using Core.Application.Messaging.Interfaces;

using Infra.Messaging.RabbitMQ.Configurations;
using Infra.Messaging.RabbitMQ.Publishers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Messaging.RabbitMQ.DependencyInjection;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection("RabbitMq")
            .Get<RabbitMqSettings>();

        services.AddSingleton(settings!);

        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

        return services;
    }
}