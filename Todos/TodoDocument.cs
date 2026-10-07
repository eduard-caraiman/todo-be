namespace todo_be.Todos;

public class TodoDocument
{
    public int Id { get; set; }

    public int TodoId { get; set; }

    public Todo Todo { get; set; } = null!;

    public Guid DocumentId { get; set; }

    public required string FileName { get; set; }

    public long Size { get; set; }

    public DateTime CreatedAt { get; set; }
}