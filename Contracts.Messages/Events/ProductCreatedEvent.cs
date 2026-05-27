namespace Contracts.Messages.Events;

public record ProductCreatedEvent(
    long ProductId,
    string Name,
    string? Category,
    decimal? UnitPrice,
    DateTime CreatedAt
);