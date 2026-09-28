namespace todo_be.Todos.Requests;

public class UpdateTodoRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsCompleted { get; set; }
    public int[]? CategoryIds { get; set; }


    public void ApplyTo(Todo todo)
    {
        todo.Title = this.Title;
        todo.Description = this.Description;
        todo.IsCompleted = this.IsCompleted;
    }
}