using FluentValidation;

namespace todo_be.Todos;

public class CreateTodoCommentRequest
{
    public required string Content { get; set; }
}


public class CreateTodoCommentRequestValidator : AbstractValidator<CreateTodoCommentRequest>
{
    public CreateTodoCommentRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required"); 
    }
}