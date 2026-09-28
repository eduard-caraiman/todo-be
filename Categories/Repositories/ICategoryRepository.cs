namespace todo_be.Categories.Repositories;

public interface ICategoryRepository
{
    Task<Category[]> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
}