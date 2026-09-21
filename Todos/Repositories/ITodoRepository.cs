namespace todo_be.Todos.Repositories;

public interface ITodoRepository
{
    Task<Todo[]> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> CreateAsync(Todo todo);
    Task<TodoComment> CreateCommentAsync(TodoComment todoComment);
    Task<Todo?> GetByIdForUpdateAsync(int id);
    void Remove(Todo todo);
    Task SaveChangesAsync();
}