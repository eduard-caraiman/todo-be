using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_be.Migrations
{
    public partial class RenameTodoIsCompleteToIsCompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsComplete",
                table: "Todos",
                newName: "IsCompleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Todos",
                newName: "IsComplete");
        }
    }
}
