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
                    durable: true
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

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            body: body
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
        _semaphore.Dispose();
    }
}