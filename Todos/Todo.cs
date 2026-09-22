namespace todo_be;

public class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<TodoComment> Comments { get; set; } = [];
}

public class TodoComment
{
    public int Id { get; set; }
    public int TodoId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}