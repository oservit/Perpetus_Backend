using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Messages.Envelopes;

public class Envelope
{
    public Guid EventId { get; set; }

    public string MessageType { get; set; }

    public string RoutingKey { get; set; }

    public string Payload { get; set; }

    public DateTime CreatedAt { get; set; }
}
