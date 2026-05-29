using System.Text.Json;
using Contracts.Messages.Envelopes;
using Core.Application.Messaging.Interfaces;

namespace Core.Application.Messaging.Publishers;

public abstract class BasePublisher
{
    protected readonly IMessageBus Bus;

    protected BasePublisher(IMessageBus bus)
    {
        Bus = bus;
    }

    protected async Task<Guid> PublishAsync<T>(
        T message,
        string routingKey)
    {
        var eventId = Guid.NewGuid();

        var envelope = new Envelope
        {
            EventId = eventId,
            MessageType = typeof(T).Name,
            RoutingKey = routingKey,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow
        };

        await Bus.PublishAsync(
            envelope,
            routingKey);

        return eventId;
    }
}