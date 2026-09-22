namespace todo_be.Tests;

using todo_be.Todos;

public class UpdateTodoRequestTests
{
    [Fact]
    public void ApplyTo_UpdatesEditableProperties()
    {
     
        var todo = new Todo
        {
            Title = "Titlu vechi",
            Description = "Descriere veche",
            IsCompleted = false
        };
        var request = new UpdateTodoRequest
        {
            Title = "Titlu nou",
            Description = "Descriere noua",
            IsCompleted = true
        };

      
        request.ApplyTo(todo);

      
        Assert.Equal("Titlu nou", todo.Title);
        Assert.Equal("Descriere noua", todo.Description);
        Assert.True(todo.IsCompleted);
    }
}
