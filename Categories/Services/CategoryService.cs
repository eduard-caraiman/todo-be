using todo_be.Categories.Repositories;
using todo_be.Categories.Requests;

namespace todo_be.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> CreateAsync(CreateCategoryRequest request)
    {
        var newCategory = request.To();

        return await _categoryRepository.CreateAsync(newCategory);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }
}