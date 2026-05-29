using Core.Domain.Common.Interfaces;
using Core.Domain.Logs.Entities;

namespace Core.Domain.Logs.Interfaces;

public interface IEventInboxRepository
    : IBaseRepository<EventInbox>
{
    Task<EventInbox?> GetByEventIdAsync(Guid eventId);
}