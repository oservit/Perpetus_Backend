namespace Contracts.Messages.Events;

public record ProductCreatedEvent(
    string Name,
    string? Category,
    decimal? UnitPrice,
    DateTime CreatedAt
);