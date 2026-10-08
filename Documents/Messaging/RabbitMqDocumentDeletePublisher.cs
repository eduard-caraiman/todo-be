using System.Text.Json;
using RabbitMQ.Client;
using todo_be.Documents.Messages;

namespace todo_be.Documents.Messaging;

public class RabbitMqDocumentDeletePublisher : IDocumentDeletePublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqDocumentDeletePublisher> _logger;

    public RabbitMqDocumentDeletePublisher(
        IConnection connection,
        ILogger<RabbitMqDocumentDeletePublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync(
        DocumentDeleteRequested message,
        CancellationToken cancellationToken = default)
    {
        await using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.DeleteRequestedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.DeleteRetryQueue
            });

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: DocumentMessageNames.DeleteRequestedQueue,
            mandatory: true,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "Mesajul de ștergere {MessageId} a fost publicat pentru documentul {DocumentId} și todo-ul {TodoId}.",
            message.MessageId,
            message.DocumentId,
            message.TodoId);
    }
}

