namespace todo_be.Todos.Repositories;

public interface ITodoDocumentRepository
{
    Task CreateAsync(TodoDocument todoDocument, CancellationToken cancellationToken = default);

    Task<TodoDocument?> GetByTodoIdAndDocumentIdAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveByTodoIdAndDocumentIdAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default);
}
