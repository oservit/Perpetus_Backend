using Contracts.Messages.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Core.Domain.Samples.Entities;
using Core.Domain.Samples.Interfaces;
using Workers.LedgerWriter.Abstractions;

namespace Workers.LedgerWriter.Consumers.Samples;

public class ProductCreatedConsumer : IMessageConsumer
{
    private readonly IProductRepository _productRepository;

    public ProductCreatedConsumer(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public string QueueName => "product.created.queue";

    public async Task HandleAsync(IChannel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                var message =
                    JsonSerializer.Deserialize<ProductCreatedEvent>(json);

                var product = new Product
                {
                    Name = message!.Name,
                    Category = message.Category,
                    UnitPrice = message.UnitPrice,
                    CreatedAt = message.CreatedAt
                };

                await _productRepository.InsertAsync(product);

                Console.WriteLine($"[SAVED PRODUCT] {product.Name}");

                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");

                await channel.BasicNackAsync(args.DeliveryTag, false, false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine($"Consumindo fila: {QueueName}");
    }
}