using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Logs.Entities;

public enum EventInboxStatus
{
    Received = 1,
    Processing = 2,
    Processed = 3,
    Failed = 4,
    DeadLetter = 5
}

