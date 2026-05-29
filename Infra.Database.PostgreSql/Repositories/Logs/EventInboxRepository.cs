using Core.Domain.Logs.Entities;
using Core.Domain.Logs.Interfaces;
using Dapper;
using Infra.Database.Abstractions.Interfaces;
using Infra.Database.PostgreSql.Repositories.Common;

namespace Infra.Database.PostgreSql.Repositories.Logs;

public class EventInboxRepository
    : PostgreSqlBaseRepository<EventInbox>,
      IEventInboxRepository
{
    public EventInboxRepository(
        IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    public async Task<EventInbox?> GetByEventIdAsync(
        Guid eventId)
    {
        const string sql = """
            SELECT *
            FROM event_inbox
            WHERE event_id = @EventId
            LIMIT 1
            """;

        using var conn = CreateConnection();

        return await conn.QueryFirstOrDefaultAsync<EventInbox>(
            sql,
            new
            {
                EventId = eventId
            });
    }
}