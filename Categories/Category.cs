using todo_be.Todos;

namespace todo_be.Categories;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Todo> Todos { get; set; } = [];
}