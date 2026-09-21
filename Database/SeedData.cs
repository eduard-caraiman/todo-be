namespace todo_be.Database;

public static class SeedData
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        if (!context.Todos.Any())
        {
            context.Todos.AddRange(
                new Todo
                {
                    Id = 1, Title = "Fist Task",
                    Description = "test",
                    IsCompleted = false,
                    CreatedAt = new DateTime(2026, 8, 26),
                    UpdatedAt = new DateTime(2026, 8, 26),
                    Comments =
                    [
                        new TodoComment
                        {
                            Id = 1,
                            TodoId = 1,
                            Content = "First comment of the day",
                            CreatedAt = new DateTime(2026, 8, 26),
                        },
                        new TodoComment
                        {
                            Id = 2,
                            TodoId = 1,
                            Content = "Yeeeey",
                            CreatedAt = new DateTime(2026, 8, 26),
                        }
                    ]
                },
                new Todo
                {
                    Id = 2, Title = "Second Task",
                    Description = "test",
                    IsCompleted = true,
                    CreatedAt = new DateTime(2026, 5, 16),
                    UpdatedAt = new DateTime(2026, 5, 16),
                    Comments = []
                },
                new Todo
                {
                    Id = 3, Title = "Third Task",
                    Description = "We need to wait 1 day in order to be completed",
                    IsCompleted = true,
                    CreatedAt = new DateTime(2026, 2, 22),
                    UpdatedAt = new DateTime(2026, 2, 22),
                    Comments =
                    [
                        new TodoComment
                        {
                            Id = 1,
                            TodoId = 3,
                            Content = "Aici am un alt comentariu",
                            CreatedAt = new DateTime(2026, 8, 26),
                        }
                    ]
                }
            );

            context.SaveChanges();
        }
    }
}