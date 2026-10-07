using todo_be.Documents.Messages;

namespace todo_be.Documents.Messaging;

public interface IDocumentUploadPublisher
{
    Task PublishAsync(
        DocumentUploadRequested message,
        CancellationToken cancellationToken = default);
}