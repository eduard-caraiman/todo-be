using System.Text.Json;
using RabbitMQ.Client;
using todo_be.Documents.Messages;

namespace todo_be.Documents.Messaging;

public class RabbitMqDocumentUploadPublisher : IDocumentUploadPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqDocumentUploadPublisher> _logger;

    public RabbitMqDocumentUploadPublisher(
        IConnection connection,
        ILogger<RabbitMqDocumentUploadPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync(
        DocumentUploadRequested message,
        CancellationToken cancellationToken = default)
    {
        await using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: DocumentMessageNames.UploadRequestedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = DocumentMessageNames.UploadRetryQueue
            });

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: DocumentMessageNames.UploadRequestedQueue,
            mandatory: true,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "Mesajul de upload {MessageId} a fost publicat pentru todo-ul {TodoId}.",
            message.MessageId,
            message.TodoId);
    }
}
