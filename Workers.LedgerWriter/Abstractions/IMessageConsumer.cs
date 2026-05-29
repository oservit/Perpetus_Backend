namespace Workers.LedgerWriter.Abstractions;

using RabbitMQ.Client;

public interface IMessageConsumer
{
    string QueueName { get; }

    Task StartAsync(IChannel channel);
}