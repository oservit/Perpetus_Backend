using Contracts.Messages.Envelopes;
using Contracts.Messages.Events;
using RabbitMQ.Client;
using Core.Domain.Logs.Interfaces;
using Core.Domain.Samples.Entities;
using Core.Domain.Samples.Interfaces;

namespace Workers.LedgerWriter.Consumers.Samples;

public class ProductCreatedConsumer
    : BaseConsumer<ProductCreatedEvent>
{
    private readonly IProductRepository _productRepository;

    public ProductCreatedConsumer(
        IProductRepository productRepository,
        IEventInboxRepository eventInboxRepository)
        : base(eventInboxRepository)
    {
        _productRepository = productRepository;
    }

    public override string QueueName
        => "product.created.queue";

    protected override async Task HandleAsync(
        ProductCreatedEvent message,
        Envelope envelope,
        IChannel channel)
    {
        var product = new Product
        {
            Name = message.Name,
            Category = message.Category,
            UnitPrice = message.UnitPrice,
            CreatedAt = message.CreatedAt
        };

        await _productRepository.InsertAsync(product);
    }
}