using Infra.Messaging.RabbitMQ.Topology.Abstractions;
using RabbitMQ.Client;

namespace Infra.Messaging.RabbitMQ.Topology.Samples;

public class ProductTopology : IRabbitMqTopology
{
    public async Task ConfigureAsync(IChannel channel)
    {
        // =====================================================
        // EXCHANGES
        // =====================================================

        const string eventsExchange = "perpetus.events";

        const string deadLetterExchange = "perpetus.dlx";

        await channel.ExchangeDeclareAsync(
            exchange: eventsExchange,
            type: ExchangeType.Topic,
            durable: true);

        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchange,
            type: ExchangeType.Topic,
            durable: true);

        // =====================================================
        // DEAD LETTER QUEUE
        // =====================================================

        const string deadLetterQueue =
            "product.created.dlq";

        await channel.QueueDeclareAsync(
            queue: deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: deadLetterQueue,
            exchange: deadLetterExchange,
            routingKey: "product.created");

        // =====================================================
        // MAIN QUEUE
        // =====================================================

        const string mainQueue =
            "product.created.queue";

        var arguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", deadLetterExchange }
        };

        await channel.QueueDeclareAsync(
            queue: mainQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);

        await channel.QueueBindAsync(
            queue: mainQueue,
            exchange: eventsExchange,
            routingKey: "product.created");
    }
}