

using Core.Application.Messaging.Publishers;
using Contracts.Messages.Events;
using Core.Application.Messaging.Interfaces;

namespace Core.Application.Samples.Publishers;
public class ProductPublisher : BasePublisher
{
    public ProductPublisher(IMessageBus bus)
        : base(bus)
    {
    }

    public async Task<Guid> ProductCreated(ProductCreatedEvent evt)
    {
        return await PublishAsync(evt,"product.created");
    }
}