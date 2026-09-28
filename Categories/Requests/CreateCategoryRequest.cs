using FluentValidation;

namespace todo_be.Categories.Requests;

public class CreateCategoryRequest
{
    public required string Name { get; set; }


    public Category To()
    {
        return new Category
        {
            Name = this.Name,
        };
    }
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category Name is required");
    }
}