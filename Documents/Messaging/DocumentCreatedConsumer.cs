using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using todo_be.Documents.Messages;
using todo_be.Todos.Service;

namespace todo_be.Documents.Messaging;

public class DocumentCreatedConsumer : BackgroundService
{
    private const int MaxRetries = 3;

    private readonly IConnection _connection;
    private readonly ILogger<DocumentCreatedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DocumentCreatedConsumer(
        IConnection connection,
        ILogger<DocumentCreatedConsumer> logger,
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
            queue: DocumentMessageNames.CreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.CreatedRetryQueue
            });

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.CreatedRetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 5000,
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.CreatedQueue
            });

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.CreatedDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<DocumentCreated>(eventArgs.Body.Span)
                    ?? throw new InvalidOperationException("Mesajul DocumentCreated nu poate fi deserializat.");

                _logger.LogInformation(
                    "Am primit mesajul {MessageId} pentru documentul {DocumentId} și todo-ul {TodoId}.",
                    message.MessageId,
                    message.DocumentId,
                    message.TodoId);

                using var scope = _scopeFactory.CreateScope();
                var todoDocumentService = scope.ServiceProvider.GetRequiredService<ITodoDocumentService>();

                await todoDocumentService.LinkDocumentAsync(
                    message.TodoId,
                    message.DocumentId,
                    message.FileName,
                    message.Size,
                    stoppingToken);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                var retryCount = RabbitMqRetryHelper.GetRetryCount(
                    eventArgs.BasicProperties.Headers,
                    DocumentMessageNames.CreatedQueue);

                if (retryCount >= MaxRetries)
                {
                    _logger.LogError(
                        exception,
                        "Mesajul DocumentCreated a eșuat de {RetryCount} ori și este mutat în DLQ.",
                        retryCount);

                    await RabbitMqRetryHelper.PublishToDeadLetterQueueAsync(
                        channel,
                        eventArgs,
                        DocumentMessageNames.CreatedDeadLetterQueue);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    return;
                }

                _logger.LogWarning(
                    exception,
                    "Mesajul DocumentCreated a eșuat. Va fi reîncercat. Încercare {NextRetry}/{MaxRetries}.",
                    retryCount + 1,
                    MaxRetries);

                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _logger.LogInformation("Consumer-ul ascultă queue-ul {QueueName}.", DocumentMessageNames.CreatedQueue);

        await channel.BasicConsumeAsync(
            queue: DocumentMessageNames.CreatedQueue,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
