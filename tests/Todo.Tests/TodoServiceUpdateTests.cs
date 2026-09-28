using todo_be.Categories;
using todo_be.Categories.Repositories;
using todo_be.Todos;
using todo_be.Todos.Repositories;
using todo_be.Todos.Requests;
using todo_be.Todos.Service;

namespace todo_be.Tests;

public class TodoServiceUpdateTests
{
    [Theory]
    [InlineData("missing")]
    [InlineData("invalid")]
    [InlineData("unchanged")]
    [InlineData("empty")]
    [InlineData("duplicates")]
    public async Task UpdateAsync_ValidatesBeforeSavingAndHandlesCategorySelection(string scenario)
    {
        var oldCategory = new Category { Id = 1, Name = "Old" };
        var newCategory = new Category { Id = 2, Name = "New" };
        var originalDate = new DateTime(2026, 1, 1);
        var todo = new Todo
        {
            Id = 10, Title = "Old title", Description = "Old description",
            IsCompleted = false, UpdatedAt = originalDate,
            Categories = new List<Category> { oldCategory }
        };
        var todoRepository = new TodoRepositoryStub(scenario == "missing" ? null : todo);
        var categoryRepository = new CategoryRepositoryStub(newCategory);
        var service = new TodoService(todoRepository, categoryRepository);
        var request = new UpdateTodoRequest
        {
            Title = "New title", Description = "New description", IsCompleted = true,
            CategoryIds = scenario switch
            {
                "unchanged" => null,
                "empty" => [],
                "invalid" => [2, 999],
                _ => [2, 2]
            }
        };

        var result = await service.UpdateAsync(10, request);

        if (scenario is "missing" or "invalid")
        {
            Assert.Null(result.Todo);
            Assert.Equal(scenario == "invalid", result.Error is not null);
            Assert.Equal(0, todoRepository.SaveCount);
            Assert.Equal("Old title", todo.Title);
            Assert.Equal(originalDate, todo.UpdatedAt);
            Assert.Same(oldCategory, Assert.Single(todo.Categories));
            if (scenario == "missing") Assert.Equal(0, categoryRepository.ReadCount);
            return;
        }

        Assert.Same(todo, result.Todo);
        Assert.Null(result.Error);
        Assert.Equal(1, todoRepository.SaveCount);
        Assert.Equal("New title", todo.Title);
        Assert.Equal("New description", todo.Description);
        Assert.True(todo.IsCompleted);
        if (scenario == "empty") Assert.Empty(todo.Categories);
        else Assert.Same(scenario == "unchanged" ? oldCategory : newCategory, Assert.Single(todo.Categories));
        if (scenario == "unchanged") Assert.Equal(0, categoryRepository.ReadCount);
        if (scenario == "duplicates") Assert.Equal(new[] { 2 }, categoryRepository.RequestedIds);
    }

    private sealed class TodoRepositoryStub(Todo? todo) : ITodoRepository
    {
        public int SaveCount { get; private set; }
        public Task<Todo?> GetByIdForUpdateAsync(int id) => Task.FromResult(todo);
        public Task SaveChangesAsync() { SaveCount++; return Task.CompletedTask; }
        public Task<Todo[]> GetAllAsync() => throw new NotSupportedException();
        public Task<Todo?> GetByIdAsync(int id) => throw new NotSupportedException();
        public Task<Todo> CreateAsync(Todo value) => throw new NotSupportedException();
        public Task<TodoComment> CreateCommentAsync(TodoComment comment) => throw new NotSupportedException();
        public void Remove(Todo value) => throw new NotSupportedException();
    }

    private sealed class CategoryRepositoryStub(Category category) : ICategoryRepository
    {
        public int ReadCount { get; private set; }
        public int[] RequestedIds { get; private set; } = [];
        public Task<Category[]> GetTrackedByIdsAsync(int[] ids)
        {
            ReadCount++;
            RequestedIds = ids;
            return Task.FromResult(ids.Contains(category.Id) ? new[] { category } : Array.Empty<Category>());
        }
        public Task<Category[]> GetAllAsync() => throw new NotSupportedException();
        public Task<Category?> GetByIdAsync(int id) => throw new NotSupportedException();
        public Task<Category?> GetTrackedByIdAsync(int id) => throw new NotSupportedException();
        public Task<Category> CreateAsync(Category value) => throw new NotSupportedException();
        public void Remove(Category value) => throw new NotSupportedException();
        public Task SaveChangesAsync() => throw new NotSupportedException();
    }
}
