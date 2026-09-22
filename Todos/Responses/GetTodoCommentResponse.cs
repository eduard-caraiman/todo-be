namespace todo_be.Todos;

public class GetTodoCommentResponse
{
    public int Id { get; set; }
    public int TodoId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }


    public static GetTodoCommentResponse From(TodoComment todoComment)
    {
        return new GetTodoCommentResponse
        {
            Id = todoComment.Id,
            TodoId = todoComment.TodoId,
            Content = todoComment.Content,
            CreatedAt = todoComment.CreatedAt,
        };
    }
}