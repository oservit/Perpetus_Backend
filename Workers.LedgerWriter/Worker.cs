using Infra.Messaging.RabbitMQ.Configurations;
using Infra.Messaging.RabbitMQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Workers.LedgerWriter.Abstractions;

namespace Workers.LedgerWriter;

public class Worker : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        RabbitMqSettings settings,
        IServiceProvider serviceProvider)
    {
        _settings = settings;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Port = _settings.Port
        };

        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync();

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 50,
            global: false);

        Console.WriteLine("RabbitMQ conectado.");

        await RabbitMqTopologyInitializer.InitializeAsync(channel);

        // ================================
        // CONSUMERS (auto-discovery via DI)
        // ================================
        using (var scope = _serviceProvider.CreateScope())
        {
            var consumers = scope.ServiceProvider
                .GetServices<IMessageConsumer>();

            foreach (var consumer in consumers)
            {
                await consumer.StartAsync(channel);
            }

            Console.WriteLine("Consumers iniciados.");
        }

        // ================================
        // KEEP ALIVE
        // ================================
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}