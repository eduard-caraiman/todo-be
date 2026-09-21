namespace todo_be.Todos;

public class GetTodoCommentResponse
{
    public int Id { get; set; }
    public int TodoId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}