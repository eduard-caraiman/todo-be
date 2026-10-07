namespace todo_be.Todos.Responses;

public class GetTodoDocumentResponse
{
    public Guid DocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public long Size { get; set; }

    public DateTime CreatedAt { get; set; }

    public static GetTodoDocumentResponse From(TodoDocument todoDocument)
    {
        return new GetTodoDocumentResponse
        {
            DocumentId = todoDocument.DocumentId,
            FileName = todoDocument.FileName,
            Size = todoDocument.Size,
            CreatedAt = todoDocument.CreatedAt
        };
    }
}