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
}