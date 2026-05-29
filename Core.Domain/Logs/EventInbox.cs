using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Logs.Entities;

[Table("event_inbox")]
public class EventInbox
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("event_id")]
    public Guid EventId { get; set; }

    [Required]
    [Column("message_type")]
    public string MessageType { get; set; }

    [Required]
    [Column("payload")]
    public string Payload { get; set; }

    [Required]
    [Column("status")]
    public EventInboxStatus Status { get; set; }

    [Column("attempt_count")]
    public int AttemptCount { get; set; }

    [Column("received_at")]
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }

    [Column("last_error")]
    public string? LastError { get; set; }
}