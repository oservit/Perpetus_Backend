namespace Core.Application.Samples.Publishers;

using Core.Application.Messaging.Publishers;
using Contracts.Messages.Events;
using Core.Application.Messaging.Interfaces;

public class ProductPublisher : BasePublisher
{
    public ProductPublisher(IMessageBus bus)
        : base(bus)
    {
    }

    public Task ProductCreated(ProductCreatedEvent evt)
        => PublishAsync(evt, "product.created");

    /*
    public Task ProductUpdated(ProductUpdatedEvent evt)
        => PublishAsync(evt, "product.updated");

    public Task ProductDeleted(Guid productId)
        => PublishAsync(
            new ProductDeletedEvent(productId),
            "product.deleted");
    */
}