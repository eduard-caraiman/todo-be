using todo_be.Documents.Messages;

namespace todo_be.Documents.Messaging;

public interface IDocumentDeletePublisher
{
    Task PublishAsync(
        DocumentDeleteRequested message,
        CancellationToken cancellationToken = default);
}
