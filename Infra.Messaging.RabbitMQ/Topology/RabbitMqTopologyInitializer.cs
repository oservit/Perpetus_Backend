using Infra.Messaging.RabbitMQ.Topology.Abstractions;
using Infra.Messaging.RabbitMQ.Topology.Samples;
using RabbitMQ.Client;

namespace Infra.Messaging.RabbitMQ.Topology;

public static class RabbitMqTopologyInitializer
{
    public static async Task InitializeAsync(
        IChannel channel)
    {
        var topologies =
            new List<IRabbitMqTopology>
            {
                new ProductTopology()
            };

        foreach (var topology in topologies)
        {
            await topology.ConfigureAsync(channel);
        }
    }
}