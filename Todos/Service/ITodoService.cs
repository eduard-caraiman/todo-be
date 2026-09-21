namespace todo_be.Todos.Service;

public interface ITodoService
{
    Task<Todo[]> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> CreateAsync(CreateTodoRequest request);
    Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request);
    Task<bool> ToggleCompletedAsync(int id);
    Task<TodoComment?> CreateCommentAsync(int todoId, CreateTodoCommentRequest request);
    Task<bool> RemoveAsync(int id);
}