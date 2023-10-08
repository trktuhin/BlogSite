using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogSite.Server.Migrations
{
    /// <inheritdoc />
    public partial class MadecommenterNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "CommentedById",
                table: "Comments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments",
                column: "CommentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "CommentedById",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CommentedById",
                table: "Comments",
                column: "CommentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
