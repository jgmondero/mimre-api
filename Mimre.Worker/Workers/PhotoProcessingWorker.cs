using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Mimre.Application.Features.Photos.Commands.ProcessPhoto;
using Mimre.Infrastructure.Queue;
using System.Text.Json;

namespace Mimre.Worker.Workers;

public class PhotoProcessingWorker(
    ILogger<PhotoProcessingWorker> logger,
    IServiceScopeFactory scopeFactory,
    IOptions<QueueSettings> queueOptions) : BackgroundService
{
    private readonly QueueSettings _queueSettings = queueOptions.Value;
    private QueueClient? _queueClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Photo Processing Worker started.");

        _queueClient = new QueueClient(
            _queueSettings.ConnectionString,
            _queueSettings.PhotoProcessingQueue);

        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextMessageAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in photo processing loop.");
            }

            // Poll every 2 seconds when queue is empty
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }

        logger.LogInformation("Photo Processing Worker stopped.");
    }

    private async Task ProcessNextMessageAsync(CancellationToken ct)
    {
        // Dequeue one message, keep it invisible for 5 minutes while processing
        QueueMessage[] messages = await _queueClient!.ReceiveMessagesAsync(
            maxMessages: 1,
            visibilityTimeout: TimeSpan.FromMinutes(5),
            cancellationToken: ct);

        if (messages.Length == 0) return;

        var message = messages[0];

        try
        {
            var payload = JsonSerializer.Deserialize<PhotoProcessingMessage>(
                System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(message.MessageText)));

            if (payload is null)
            {
                logger.LogWarning("Failed to deserialize queue message: {MessageText}", message.MessageText);
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);
                return;
            }

            logger.LogInformation("Processing photo {PhotoId}.", payload.PhotoId);

            // Dispatch into a fresh DI scope so DbContext and services are scoped correctly
            using var scope = scopeFactory.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            await sender.Send(new ProcessPhotoCommand(payload.PhotoId), ct);

            // Delete the message only after successful processing
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);

            logger.LogInformation("Photo {PhotoId} processed successfully.", payload.PhotoId);
        }
        catch (Exception ex)
        {
            // Leave the message in the queue — it will become visible again after
            // the visibility timeout and will be retried automatically.
            // After a configured number of retries Azure moves it to the dead-letter queue.
            logger.LogError(ex, "Failed to process photo from queue message {MessageId}.", message.MessageId);
        }
    }

    private record PhotoProcessingMessage(Guid PhotoId);
}
