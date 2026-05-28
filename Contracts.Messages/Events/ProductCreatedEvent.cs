namespace Contracts.Messages.Events;

public record ProductCreatedEvent(
    Guid EventId,
    string Name,
    string? Category,
    decimal? UnitPrice,
    DateTime CreatedAt
);