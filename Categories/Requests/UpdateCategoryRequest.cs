using FluentValidation;

namespace todo_be.Categories.Requests;

public class UpdateCategoryRequest
{
    public required string Name { get; set; }

    public void ApplyTo(Category category)
    {
        category.Name = this.Name;
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category Name is required");
    }
}