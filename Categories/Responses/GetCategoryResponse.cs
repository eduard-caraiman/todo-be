namespace todo_be.Categories.Responses;

public class GetCategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }


    public static GetCategoryResponse From(Category category)
    {
        return new GetCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}