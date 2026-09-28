using todo_be.Categories.Requests;

namespace todo_be.Categories.Services;

public interface ICategoryService
{
    Task<Category> CreateAsync(CreateCategoryRequest request);
    Task<Category?> GetByIdAsync(int id);
}