using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using todo_be.Documents.Messages;
using todo_be.Todos.Service;

namespace todo_be.Documents.Messaging;

public class DocumentCreatedConsumer : BackgroundService
{
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
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var message = JsonSerializer.Deserialize<DocumentCreated>(eventArgs.Body.Span)
                ?? throw new InvalidOperationException(
                    "Mesajul DocumentCreated nu poate fi deserializat.");

            _logger.LogInformation(
                "Am primit mesajul {MessageId} pentru documentul {DocumentId} și todo-ul {TodoId}.",
                message.MessageId,
                message.DocumentId,
                message.TodoId);

            using var scope = _scopeFactory.CreateScope();

            var todoDocumentService = scope.ServiceProvider
                .GetRequiredService<ITodoDocumentService>();

            await todoDocumentService.LinkDocumentAsync(
                message.TodoId,
                message.DocumentId,
                message.FileName,
                message.Size,
                stoppingToken);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        _logger.LogInformation(
            "Consumer-ul ascultă queue-ul {QueueName}.",
            DocumentMessageNames.CreatedQueue);

        await channel.BasicConsumeAsync(
            queue: DocumentMessageNames.CreatedQueue,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
