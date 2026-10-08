using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using todo_be.Documents.Messages;
using todo_be.Todos.Service;

namespace todo_be.Documents.Messaging;

public class DocumentDeletedConsumer : BackgroundService
{
    private const int MaxRetries = 3;

    private readonly IConnection _connection;
    private readonly ILogger<DocumentDeletedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DocumentDeletedConsumer(
        IConnection connection,
        ILogger<DocumentDeletedConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.DeletedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.DeletedRetryQueue
            });

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.DeletedRetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 5000,
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.DeletedQueue
            });

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.DeletedDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<DocumentDeleted>(eventArgs.Body.Span)
                    ?? throw new InvalidOperationException("Mesajul DocumentDeleted nu poate fi deserializat.");

                using var scope = _scopeFactory.CreateScope();
                var todoDocumentService = scope.ServiceProvider.GetRequiredService<ITodoDocumentService>();

                var removed = await todoDocumentService.RemoveLinkAsync(
                    message.TodoId,
                    message.DocumentId,
                    stoppingToken);

                _logger.LogInformation(
                    "Legătura dintre todo-ul {TodoId} și documentul {DocumentId} a fost {Result}.",
                    message.TodoId,
                    message.DocumentId,
                    removed ? "ștearsă" : "deja inexistentă");

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                var retryCount = RabbitMqRetryHelper.GetRetryCount(
                    eventArgs.BasicProperties.Headers,
                    DocumentMessageNames.DeletedQueue);

                if (retryCount >= MaxRetries)
                {
                    _logger.LogError(
                        exception,
                        "Mesajul DocumentDeleted a eșuat de {RetryCount} ori și este mutat în DLQ.",
                        retryCount);

                    await RabbitMqRetryHelper.PublishToDeadLetterQueueAsync(
                        channel,
                        eventArgs,
                        DocumentMessageNames.DeletedDeadLetterQueue);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    return;
                }

                _logger.LogWarning(
                    exception,
                    "Mesajul DocumentDeleted a eșuat. Va fi reîncercat. Încercare {NextRetry}/{MaxRetries}.",
                    retryCount + 1,
                    MaxRetries);

                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _logger.LogInformation("Consumer-ul ascultă queue-ul {QueueName}.", DocumentMessageNames.DeletedQueue);

        await channel.BasicConsumeAsync(
            queue: DocumentMessageNames.DeletedQueue,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
