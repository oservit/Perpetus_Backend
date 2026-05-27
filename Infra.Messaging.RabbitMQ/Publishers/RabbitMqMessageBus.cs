using Core.Application.Messaging.Interfaces;
using Infra.Messaging.RabbitMQ.Configurations;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infra.Messaging.RabbitMQ.Publishers;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqMessageBus(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public async Task PublishAsync<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
            Port = _settings.Port
        };

        using var connection = await factory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        const string exchangeName = "perpetus.events";

        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true);

        var routingKey = typeof(T).Name;

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            body: body);

        Console.WriteLine($"Evento publicado: {routingKey}");
    }
}