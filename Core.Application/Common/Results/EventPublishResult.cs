using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Common.Results
{
    public sealed record EventPublishResult
    {
        public Guid EventId { get; init; }
        public string PayloadHash { get; init; } = default!;
        public long EntityId { get; init; }
    }
}
