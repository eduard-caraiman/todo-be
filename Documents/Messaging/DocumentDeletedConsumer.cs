using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using todo_be.Documents.Messages;
using todo_be.Todos.Service;

namespace todo_be.Documents.Messaging;

public class DocumentDeletedConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<DocumentDeletedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DocumentDeletedConsumer(IConnection connection, ILogger<DocumentDeletedConsumer> logger, IServiceScopeFactory scopeFactory)
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
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
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
        };

        _logger.LogInformation("Consumer-ul ascultă queue-ul {QueueName}.", DocumentMessageNames.DeletedQueue);

        await channel.BasicConsumeAsync(
            queue: DocumentMessageNames.DeletedQueue,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
