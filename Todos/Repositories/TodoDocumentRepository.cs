using Microsoft.EntityFrameworkCore;
using todo_be.Database;

namespace todo_be.Todos.Repositories;

public class TodoDocumentRepository : ITodoDocumentRepository
{
    private readonly AppDbContext _dbContext;

    public TodoDocumentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(TodoDocument todoDocument, CancellationToken cancellationToken = default)
    {
        await _dbContext.TodoDocuments.AddAsync(todoDocument, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TodoDocument?> GetByTodoIdAndDocumentIdAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.TodoDocuments.SingleOrDefaultAsync(
            todoDocument => todoDocument.TodoId == todoId
                && todoDocument.DocumentId == documentId,
            cancellationToken);
    }

    public async Task<bool> RemoveByTodoIdAndDocumentIdAsync(
        int todoId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var todoDocument = await GetByTodoIdAndDocumentIdAsync(
            todoId,
            documentId,
            cancellationToken);

        if (todoDocument is null)
        {
            return false;
        }

        _dbContext.TodoDocuments.Remove(todoDocument);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
