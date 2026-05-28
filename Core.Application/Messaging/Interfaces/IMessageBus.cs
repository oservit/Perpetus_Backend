namespace Core.Application.Messaging.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string routingKey);
}