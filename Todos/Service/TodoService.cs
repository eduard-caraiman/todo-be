using todo_be.Todos.Repositories;

namespace todo_be.Todos.Service;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(
        ITodoRepository todoRepository
    )
    {
        _todoRepository = todoRepository;
    }

    public async Task<Todo[]> GetAllAsync()
    {
        return await _todoRepository.GetAllAsync();
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await _todoRepository.GetByIdAsync(id);
    }


    public async Task<Todo> CreateAsync(CreateTodoRequest request)
    {
        var currentDate = DateTime.Now;
        var newTodo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            CreatedAt = currentDate,
            UpdatedAt = currentDate,
            Comments = []
        };

        return await _todoRepository.CreateAsync(newTodo);
    }

    public async Task<Todo?> UpdateAsync(int todoId, UpdateTodoRequest request)
    {
        var foundTodo = await _todoRepository.GetByIdForUpdateAsync(todoId);

        if (foundTodo is null)
        {
            return null;
        }

        foundTodo.Title = request.Title;
        foundTodo.Description = request.Description;
        foundTodo.IsCompleted = request.IsCompleted;
        foundTodo.UpdatedAt = DateTime.Now;

        await _todoRepository.SaveChangesAsync();

        return foundTodo;
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var foundTodo = await _todoRepository.GetByIdForUpdateAsync(id);

        if (foundTodo == null)
        {
            return false;
        }

        _todoRepository.Remove(foundTodo);
        await _todoRepository.SaveChangesAsync();

        return true;
    }

    public async Task<TodoComment?> CreateCommentAsync(int todoId, CreateTodoCommentRequest request)
    {
        var foundTodo = await _todoRepository.GetByIdAsync(todoId);

        if (foundTodo is null)
        {
            return null;
        }

        var newComment = new TodoComment
        {
            TodoId = todoId,
            Content = request.Content,
            CreatedAt = DateTime.Now
        };

        return await _todoRepository.CreateCommentAsync(newComment);
    }

    public async Task<bool> ToggleCompletedAsync(int id)
    {
        var foundTodo = await _todoRepository.GetByIdForUpdateAsync(id);

        if (foundTodo == null)
        {
            return false;
        }


        foundTodo.IsCompleted = !foundTodo.IsCompleted;
        foundTodo.UpdatedAt = DateTime.Now;

        await _todoRepository.SaveChangesAsync();


        return true;
    }
}