using Microsoft.EntityFrameworkCore;

namespace todo_be.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }
    public DbSet<TodoComment> TodoComments { get; set; }
}