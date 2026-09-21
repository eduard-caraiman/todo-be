using Microsoft.EntityFrameworkCore;
using todo_be.Database;

namespace todo_be.Todos.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _dbContext;

    public TodoRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Todo[]> GetAllAsync()
    {
        return await _dbContext.Todos.Include(t => t.Comments).ToArrayAsync();
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await _dbContext.Todos.Include(t => t.Comments).SingleOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();

        return todo;
    }

    public async Task<Todo?> GetByIdForUpdateAsync(int id)
    {
        return await _dbContext.Todos
            .AsTracking()
            .Include(t => t.Comments)
            .SingleOrDefaultAsync(t => t.Id == id);
    }

    public void Remove(Todo todo)
    {
        _dbContext.Todos.Remove(todo);
    }

    public async Task<TodoComment> CreateCommentAsync(TodoComment todoComment)
    {
        _dbContext.TodoComments.Add(todoComment);
        await _dbContext.SaveChangesAsync();

        return todoComment;
    }
}