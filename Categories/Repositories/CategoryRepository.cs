using Microsoft.EntityFrameworkCore;
using todo_be.Database;

namespace todo_be.Categories.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Category[]> GetAllAsync()
    {
        return await _dbContext.Categories.ToArrayAsync();
    }

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        return await _dbContext.Categories.SingleOrDefaultAsync(c => categoryId == c.Id);
    }

    public async Task<Category?> GetTrackedByIdAsync(int categoryId)
    {
        return await _dbContext.Categories.AsTracking().SingleOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return category;
    }

    public void Remove(Category category)
    {
        _dbContext.Remove(category);
    }
}