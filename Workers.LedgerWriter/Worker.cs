using Infra.Messaging.RabbitMQ.Configurations;
using Infra.Messaging.RabbitMQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Workers.LedgerWriter.Consumers.Samples;

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

        Console.WriteLine("RabbitMQ conectado.");

        await RabbitMqTopologyInitializer.InitializeAsync(channel);

        // ================================
        // CONSUMERS (com scope correto)
        // ================================
        using (var scope = _serviceProvider.CreateScope())
        {
            var productCreatedConsumer =
                scope.ServiceProvider.GetRequiredService<ProductCreatedConsumer>();

            await productCreatedConsumer.HandleAsync(channel);
        }

        Console.WriteLine("Consumers iniciados.");

        // ================================
        // KEEP ALIVE
        // ================================
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}