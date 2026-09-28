using todo_be.Categories.Requests;

namespace todo_be.Categories.Services;

public interface ICategoryService
{
    Task<Category[]> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(CreateCategoryRequest request);
    Task<bool> RemoveAsync(int id);
}