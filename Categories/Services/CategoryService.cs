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


    public async Task<Category[]> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateAsync(CreateCategoryRequest request)
    {
        var newCategory = request.To();

        return await _categoryRepository.CreateAsync(newCategory);
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var foundCategory = await _categoryRepository.GetByIdAsync(id);
        if (foundCategory == null)
        {
            return false;
        }

        _categoryRepository.Remove(foundCategory);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}