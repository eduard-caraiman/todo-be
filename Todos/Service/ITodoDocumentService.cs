namespace todo_be.Todos.Service;

public interface ITodoDocumentService
{
    Task LinkDocumentAsync(
        int todoId,
        Guid documentId,
        string fileName,
        long size,
        CancellationToken cancellationToken = default);

    Task<TodoDocument?> GetByTodoIdAndDocumentIdAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveLinkAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default);
}
