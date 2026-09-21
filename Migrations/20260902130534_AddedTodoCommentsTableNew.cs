using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_be.Migrations
{
    /// <inheritdoc />
    public partial class AddedTodoCommentsTableNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoComment_Todos_TodoId",
                table: "TodoComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoComment",
                table: "TodoComment");

            migrationBuilder.RenameTable(
                name: "TodoComment",
                newName: "TodoComments");

            migrationBuilder.RenameIndex(
                name: "IX_TodoComment_TodoId",
                table: "TodoComments",
                newName: "IX_TodoComments_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoComments",
                table: "TodoComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoComments_Todos_TodoId",
                table: "TodoComments",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoComments_Todos_TodoId",
                table: "TodoComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoComments",
                table: "TodoComments");

            migrationBuilder.RenameTable(
                name: "TodoComments",
                newName: "TodoComment");

            migrationBuilder.RenameIndex(
                name: "IX_TodoComments_TodoId",
                table: "TodoComment",
                newName: "IX_TodoComment_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoComment",
                table: "TodoComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoComment_Todos_TodoId",
                table: "TodoComment",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
