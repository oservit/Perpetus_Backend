namespace Core.Application.Messaging.Publishers;

using Core.Application.Messaging.Interfaces;

public abstract class BasePublisher
{
    protected readonly IMessageBus Bus;

    protected BasePublisher(IMessageBus bus)
    {
        Bus = bus;
    }

    protected Task PublishAsync<T>(T message, string routingKey)
        => Bus.PublishAsync(message, routingKey);
}