using todo_be.Categories.Responses;

namespace todo_be.Todos.Responses;

public class GetTodoResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public ICollection<GetTodoCommentResponse> Comments { get; set; } = [];
    public ICollection<GetCategoryResponse> Categories { get; set; } = [];
    public ICollection<GetTodoDocumentResponse> Documents { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static GetTodoResponse From(Todo todo)
    {
        return new GetTodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            Comments = todo.Comments.Select(comment => GetTodoCommentResponse.From(comment)).ToArray(),
            Categories = todo.Categories.Select(category => GetCategoryResponse.From(category)).ToArray(),
            Documents = todo.Documents.Select(document => GetTodoDocumentResponse.From(document)).ToArray(),
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }
}