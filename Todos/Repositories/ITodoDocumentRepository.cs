namespace todo_be.Todos.Repositories;

public interface ITodoDocumentRepository
{
    Task CreateAsync(
        TodoDocument todoDocument,
        CancellationToken cancellationToken = default);
}