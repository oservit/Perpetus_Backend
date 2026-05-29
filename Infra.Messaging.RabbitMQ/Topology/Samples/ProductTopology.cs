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
        const string retryExchange = "perpetus.retry";
        const string deadLetterExchange = "perpetus.dlx";

        await channel.ExchangeDeclareAsync(
            exchange: eventsExchange,
            type: ExchangeType.Topic,
            durable: true);

        await channel.ExchangeDeclareAsync(
            exchange: retryExchange,
            type: ExchangeType.Topic,
            durable: true);

        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchange,
            type: ExchangeType.Topic,
            durable: true);

        // =====================================================
        // MAIN QUEUE
        // =====================================================

        const string mainQueue = "product.created.queue";

        var mainArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", retryExchange },
            { "x-dead-letter-routing-key", "product.created.retry" }
        };

        await channel.QueueDeclareAsync(
            queue: mainQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainArgs);

        await channel.QueueBindAsync(
            queue: mainQueue,
            exchange: eventsExchange,
            routingKey: "product.created");

        // =====================================================
        // RETRY QUEUE (única)
        // =====================================================

        const string retryQueue = "product.created.retry.queue";

        var retryArgs = new Dictionary<string, object?>
        {
            // opcional: delay global de retry
            { "x-message-ttl", 30000 },

            // se falhar retry de novo → DLQ final
            { "x-dead-letter-exchange", deadLetterExchange },
            { "x-dead-letter-routing-key", "product.created" }
        };

        await channel.QueueDeclareAsync(
            queue: retryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArgs);

        await channel.QueueBindAsync(
            queue: retryQueue,
            exchange: retryExchange,
            routingKey: "product.created.retry");

        // =====================================================
        // DLQ FINAL
        // =====================================================

        const string deadLetterQueue = "product.created.dlq";

        await channel.QueueDeclareAsync(
            queue: deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: deadLetterQueue,
            exchange: deadLetterExchange,
            routingKey: "product.created");
    }
}