using RabbitMQ.Client;

namespace Infra.Messaging.RabbitMQ.Topology.Abstractions;

public interface IRabbitMqTopology
{
    Task ConfigureAsync(IChannel channel);
}