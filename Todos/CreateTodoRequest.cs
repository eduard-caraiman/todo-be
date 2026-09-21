using FluentValidation;

namespace todo_be.Todos;

public class CreateTodoRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsCompleted { get; set; }


    public Todo ToTodo()
    {
        return new Todo
        {
            Title = this.Title,
            Description = this.Description,
            IsCompleted = this.IsCompleted,
        };
    }
}

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
    }
}