using FluentValidation;

namespace todo_be.Todos;

public class CreateTodoCommentRequest
{
    public required string Content { get; set; }

    public TodoComment ToTodoComment(int todoId)
    {
        return new TodoComment
        {
            TodoId = todoId,
            Content = this.Content,
        };
    }
}

public class CreateTodoCommentRequestValidator : AbstractValidator<CreateTodoCommentRequest>
{
    public CreateTodoCommentRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
    }
}