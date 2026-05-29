using Core.Application.Messaging.Interfaces;
using Infra.Messaging.RabbitMQ.Configurations;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infra.Messaging.RabbitMQ.Publishers;

public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const string ExchangeName = "perpetus.events";

    public RabbitMqMessageBus(RabbitMqSettings settings)
    {
        _factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password,
            Port = settings.Port
        };
    }

    private async Task EnsureInitializedAsync()
    {
        if (_channel is not null) return;

        await _semaphore.WaitAsync();
        try
        {
            if (_channel is null)
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false
                );
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task PublishAsync<T>(T message, string routingKey)
    {
        await EnsureInitializedAsync();

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,

            Headers = new Dictionary<string, object?>
            {
                { "event-type", typeof(T).Name },
                { "published-at", DateTime.UtcNow.ToString("O") }
            }
        };

        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
        finally
        {
            _semaphore.Dispose();
        }
    }
}