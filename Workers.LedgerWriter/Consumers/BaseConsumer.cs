using Core.Domain.Logs.Entities;
using Core.Domain.Logs.Interfaces;
using Contracts.Messages.Envelopes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Workers.LedgerWriter.Abstractions;

namespace Workers.LedgerWriter.Consumers;

public abstract class BaseConsumer<TMessage>
    : IMessageConsumer
{
    public abstract string QueueName { get; }

    private readonly IEventInboxRepository _eventInboxRepository;

    private readonly JsonSerializerOptions _jsonOptions;

    protected BaseConsumer(
        IEventInboxRepository eventInboxRepository)
    {
        _eventInboxRepository = eventInboxRepository;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task StartAsync(IChannel channel)
    {
        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var deliveryTag = args.DeliveryTag;

            Envelope? envelope = null;

            try
            {
                envelope =
                    DeserializeEnvelope(args.Body.ToArray());

                if (envelope is null)
                {
                    await channel.BasicAckAsync(
                        deliveryTag,
                        false);

                    return;
                }

                await RegisterReceivedAsync(envelope);

                if (await IsAlreadyProcessedAsync(
                        envelope.EventId))
                {
                    Console.WriteLine(
                        $"[DUPLICATE] EventId={envelope.EventId}");

                    await channel.BasicAckAsync(
                        deliveryTag,
                        false);

                    return;
                }

                var message =
                    DeserializeMessage(envelope);

                await HandleAsync(
                    message,
                    envelope,
                    channel);

                await MarkSuccessAsync(envelope);

                await channel.BasicAckAsync(
                    deliveryTag,
                    false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[ERROR] {ex.Message}");

                if (envelope is null)
                {
                    await channel.BasicNackAsync(
                        deliveryTag,
                        false,
                        false);

                    return;
                }

                await HandleFailureAsync(
                    envelope,
                    ex,
                    channel,
                    deliveryTag);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine(
            $"Consumindo fila: {QueueName}");
    }

    private async Task RegisterReceivedAsync(
        Envelope envelope)
    {
        var existing =
            await _eventInboxRepository
                .GetByEventIdAsync(envelope.EventId);

        if (existing is not null)
            return;

        await _eventInboxRepository.InsertAsync(
            new EventInbox
            {
                EventId = envelope.EventId,
                MessageType = envelope.MessageType,
                Payload = envelope.Payload,
                Status = EventInboxStatus.Received,
                AttemptCount = 0,
                ReceivedAt = DateTime.UtcNow
            });
    }

    private async Task<bool> IsAlreadyProcessedAsync(
        Guid eventId)
    {
        var inbox =
            await _eventInboxRepository
                .GetByEventIdAsync(eventId);

        if (inbox is null)
            return false;

        return inbox.Status ==
               EventInboxStatus.Processed;
    }

    private async Task MarkSuccessAsync(
        Envelope envelope)
    {
        var inbox =
            await _eventInboxRepository
                .GetByEventIdAsync(envelope.EventId);

        if (inbox is null)
            return;

        inbox.Status =
            EventInboxStatus.Processed;

        inbox.ProcessedAt =
            DateTime.UtcNow;

        await _eventInboxRepository
            .UpdateAsync(inbox);
    }

    private async Task HandleFailureAsync(
        Envelope envelope,
        Exception ex,
        IChannel channel,
        ulong deliveryTag)
    {
        var inbox =
            await _eventInboxRepository
                .GetByEventIdAsync(envelope.EventId);

        if (inbox is null)
        {
            await channel.BasicNackAsync(
                deliveryTag,
                false,
                false);

            return;
        }

        inbox.AttemptCount++;

        inbox.LastError =
            ex.ToString();

        if (ShouldGoToDlq(inbox.AttemptCount))
        {
            inbox.Status =
                EventInboxStatus.DeadLetter;

            await _eventInboxRepository
                .UpdateAsync(inbox);

            Console.WriteLine(
                $"[DLQ] EventId={envelope.EventId}");

            await channel.BasicAckAsync(
                deliveryTag,
                false);

            return;
        }

        inbox.Status =
            EventInboxStatus.Failed;

        await _eventInboxRepository
            .UpdateAsync(inbox);

        Console.WriteLine(
            $"[RETRY] EventId={envelope.EventId} Attempt={inbox.AttemptCount}");

        await channel.BasicNackAsync(
            deliveryTag,
            false,
            false);
    }

    protected virtual bool ShouldGoToDlq(
        int attempt)
        => attempt >= 3;

    protected abstract Task HandleAsync(
        TMessage message,
        Envelope envelope,
        IChannel channel);

    private Envelope? DeserializeEnvelope(
        byte[] body)
    {
        var json =
            Encoding.UTF8.GetString(body);

        return JsonSerializer.Deserialize<Envelope>(
            json,
            _jsonOptions);
    }

    private TMessage DeserializeMessage(
        Envelope envelope)
    {
        if (string.IsNullOrWhiteSpace(
                envelope.Payload))
        {
            throw new InvalidOperationException(
                "Payload inválido.");
        }

        var message =
            JsonSerializer.Deserialize<TMessage>(
                envelope.Payload,
                _jsonOptions);

        if (message is null)
        {
            throw new InvalidOperationException(
                "Conteúdo inválido.");
        }

        return message;
    }
}