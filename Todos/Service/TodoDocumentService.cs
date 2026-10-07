using todo_be.Todos.Repositories;

namespace todo_be.Todos.Service;

public class TodoDocumentService : ITodoDocumentService
{
    private readonly ITodoDocumentRepository _todoDocumentRepository;

    public TodoDocumentService(
        ITodoDocumentRepository todoDocumentRepository)
    {
        _todoDocumentRepository = todoDocumentRepository;
    }


    public async Task LinkDocumentAsync(
        int todoId,
        Guid documentId,
        string fileName,
        long size,
        CancellationToken cancellationToken = default)
    {
        var todoDocument = new TodoDocument
        {
            TodoId = todoId,
            DocumentId = documentId,
            FileName = fileName,
            Size = size,
            CreatedAt = DateTime.UtcNow
        };

        await _todoDocumentRepository.CreateAsync(
            todoDocument,
            cancellationToken);
    }
}