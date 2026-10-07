namespace todo_be.Todos.Service;

public interface ITodoDocumentService
{
    Task LinkDocumentAsync(
        int todoId,
        Guid documentId,
        string fileName,
        long size,
        CancellationToken cancellationToken = default);
}